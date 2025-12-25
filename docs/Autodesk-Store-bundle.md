To publish your application to the Autodesk Store, you need to prepare a [bundle](https://www.autodesk.com/autodesk-university/class/AppBundle-Cross-Distribution-Autodesk-Products-App-Store-and-Forge-2020) according to a certain standard.
The template will create it for you, and pack it into a Zip, you just have to upload it to the Store.

## Build system setup

- Set option **Bundle support** when creating the solution.
- Open **build/appsettings.json** file
- Update **BundleOptions** section: **VendorName**, **VendorEmail**, **VendorUrl**.

    ![image](https://github.com/user-attachments/assets/4f9f9484-9831-42d6-ae85-8ced75472e1a)

> [!NOTE]
> For creating a bundle, a third-party package is used.
> You can find more detailed documentation about it here: [Autodesk.PackageBuilder](https://www.nuget.org/packages/Autodesk.PackageBuilder#readme-body-tab)

## Output files

```text
📂output
 ┗📜RevitAddIn.zip
   ┗📂RevitAddIn.bundle
     ┣📜PackageContents.xml
     ┗📂Contents
       ┣📂2020
       ┃ ┣📜RevitAddIn.addin
       ┃ ┗📂RevitAddIn
       ┃   ┗📜RevitAddIn.dll    
       ┗📂2021
         ┣📜RevitAddIn.addin
         ┗📂RevitAddIn
           ┗📜RevitAddIn.dll
```