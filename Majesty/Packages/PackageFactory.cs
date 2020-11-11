using System;

namespace Majesty.Packages
{
    class PackageFactory : IPackageFactory
    {
        public IPackage Create(string packageObject)
        {
            return packageObject switch
            {
                "UserPackage" => new UserPackage(),
                _ => throw new NotSupportedException()
            };
        }
    }
}
