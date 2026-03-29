using BenchmarkDotNet.Attributes;
using Nice3point.BenchmarkDotNet.Revit;

namespace RevitAddIn.Benchmark.Benchmarks;

public class VolumeCalculationBenchmarks : RevitApiBenchmark
{
    private Document? _document;
    private Element[] _geometryElements = [];

    [Params(0, 1)]
    public int ElementSizeIndex { get; set; }
    private Element GeometryElement => _geometryElements[ElementSizeIndex];

    protected sealed override void OnGlobalSetup()
    {
        _document = Application.NewProjectDocument(UnitSystem.Metric);

        using var transaction = new Transaction(_document, "Seed model");
        transaction.Start();
        
        var level = Level.Create(_document, 1);
        var smallestWall = Wall.Create(_document, Line.CreateBound(new XYZ(0, 0, 0), new XYZ(1, 0, 0)), level.Id, false);
        var largestWall = Wall.Create(_document, Line.CreateBound(new XYZ(0, 1, 0), new XYZ(1000, 1, 0)), level.Id, false);
        largestWall.FindParameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM)!.Set(1000);

        transaction.Commit();
        
        _geometryElements = [smallestWall, largestWall];
    }

    protected sealed override void OnGlobalCleanup()
    {
        _document?.Close(false);
    }

    [Benchmark]
    public double Coarse_SolidsOnly_Foreach()
    {
        var totalVolume = 0d;
        var geometry = GeometryElement.get_Geometry(new Options { DetailLevel = ViewDetailLevel.Coarse });

        foreach (var geometryObject in geometry)
        {
            if (geometryObject is Solid { Volume: > 0 } solid)
            {
                totalVolume += solid.Volume;
            }
        }

        return totalVolume;
    }

    [Benchmark]
    public double Medium_SolidsOnly_Foreach()
    {
        var totalVolume = 0d;
        var geometry = GeometryElement.get_Geometry(new Options { DetailLevel = ViewDetailLevel.Medium });

        foreach (var geometryObject in geometry)
        {
            if (geometryObject is Solid { Volume: > 0 } solid)
            {
                totalVolume += solid.Volume;
            }
        }

        return totalVolume;
    }

    [Benchmark]
    public double Fine_SolidsOnly_Foreach()
    {
        var totalVolume = 0d;
        var geometry = GeometryElement.get_Geometry(new Options { DetailLevel = ViewDetailLevel.Fine });

        foreach (var geometryObject in geometry)
        {
            if (geometryObject is Solid { Volume: > 0 } solid)
            {
                totalVolume += solid.Volume;
            }
        }

        return totalVolume;
    }

    [Benchmark]
    public double Fine_SolidsOnly_Linq()
    {
        var geometry = GeometryElement.get_Geometry(new Options { DetailLevel = ViewDetailLevel.Fine });
        return geometry?.OfType<Solid>().Where(solid => solid.Volume > 0).Sum(solid => solid.Volume) ?? 0d;
    }

    [Benchmark]
    public double Fine_WithInstances_Recursive()
    {
        var geometry = GeometryElement.get_Geometry(new Options { DetailLevel = ViewDetailLevel.Fine });
        return CalculateVolumeRecursive(geometry);
    }

    [Benchmark]
    public double Fine_WithInstances_Stack()
    {
        var geometry = GeometryElement.get_Geometry(new Options { DetailLevel = ViewDetailLevel.Fine });
        return CalculateVolumeIterative(geometry);
    }

    private static double GetDiagonal(Element element)
    {
        var boundingBox = element.get_BoundingBox(null);
        return boundingBox?.Max.DistanceTo(boundingBox.Min) ?? double.MinValue;
    }

    private static double CalculateVolumeRecursive(GeometryElement geometryElement)
    {
        var totalVolume = 0d;

        foreach (var geometryObject in geometryElement)
        {
            totalVolume += geometryObject switch
            {
                Solid { Volume: > 0 } solid => solid.Volume,
                GeometryInstance instance => CalculateVolumeRecursive(instance.GetInstanceGeometry()),
                GeometryElement nested => CalculateVolumeRecursive(nested),
                _ => 0d
            };
        }

        return totalVolume;
    }

    private static double CalculateVolumeIterative(GeometryElement geometryElement)
    {
        var totalVolume = 0d;
        var stack = new Stack<GeometryElement>();
        stack.Push(geometryElement);

        while (stack.Count > 0)
        {
            var currentGeometry = stack.Pop();
            foreach (var geometryObject in currentGeometry)
            {
                switch (geometryObject)
                {
                    case Solid { Volume: > 0 } solid:
                        totalVolume += solid.Volume;
                        break;
                    case GeometryInstance instance:
                        stack.Push(instance.GetInstanceGeometry());
                        break;
                    case GeometryElement nestedGeometry:
                        stack.Push(nestedGeometry);
                        break;
                }
            }
        }

        return totalVolume;
    }
}