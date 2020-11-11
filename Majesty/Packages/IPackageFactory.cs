namespace Majesty.Packages
{
    interface IPackageFactory
    {
        IPackage Create(string packageObject);
    }
}
