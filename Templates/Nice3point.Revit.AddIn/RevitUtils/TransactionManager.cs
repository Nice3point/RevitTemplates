using System;
using Autodesk.Revit.DB;

namespace Nice3point.Revit.AddIn.RevitUtils

public static class TransactionManager
{
    /// <summary>
    ///     The method used to create a single sub-transaction.
    /// </summary>
    public static void CreateSubTransaction(Document document, Action action)
    {
        using var transaction = new SubTransaction(document);
        transaction.Start();
        try
        {
            action?.Invoke();
            transaction.Commit();
        }
        catch (Exception)
        {
            if (!transaction.HasEnded()) transaction.RollBack();
        }
    }
    
    /// <summary>
    ///     The method used to create a single transaction.
    /// </summary>
    public static void CreateTransaction(Document document, string transactionName, Action action)
    {
        using var transaction = new Transaction(document);
        transaction.Start(transactionName);
        try
        {
            action?.Invoke();
            transaction.Commit();
        }
        catch (Exception)
        {
            if (!transaction.HasEnded()) transaction.RollBack();
        }
    }

    /// <summary>
    ///     The method used to create a group of transactions.
    /// </summary>
    public static void CreateGroupTransaction(Document document, string transactionName, Action action)
    {
        using var transaction = new TransactionGroup(document);
        transaction.Start(transactionName);
        try
        {
            action?.Invoke();
            transaction.Assimilate();
        }
        catch (Exception)
        {
            if (!transaction.HasEnded()) transaction.RollBack();
        }
    }
}