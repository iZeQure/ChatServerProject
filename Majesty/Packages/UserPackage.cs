using System;
using System.Collections.Generic;
using System.Text;

namespace Majesty.Packages
{
    class UserPackage : IPackage
    {


        public byte[] PackageBytes
        {
            get;
            set;
        }

        public string UsersMessage { get; set; }
    }
}
