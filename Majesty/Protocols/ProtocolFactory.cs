using System;
using System.Collections.Generic;
using System.Text;

namespace Majesty.Protocols
{
    class ProtocolFactory : IProtocolFactory
    {
        public IProtocol Create(string protocolObject)
        {
            return protocolObject switch
            {
                "SimpleProtocol" => new SimpleProtocol(),
                "XmlProtocol" => new XmlProtocol(),
                "SymmetricProtocol" => new SymmetricProtocol(),
                "AsymmetricProtocol" => new AsymmetricProtocol(),
                _ => throw new NotSupportedException()
            };
        }
    }
}
