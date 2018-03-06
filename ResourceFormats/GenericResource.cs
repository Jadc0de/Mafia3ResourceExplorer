using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Gibbed.IO;
using ManagedBass;

namespace ResourceFormats
{
    public class GenericResource : IResourceType
    {
        public void Serialize(ushort version, Stream output, Endian endian)
        {
            throw new InvalidOperationException();
        }

        public void Deserialize(ushort version, Stream input, Endian endian)
        {
            throw new InvalidOperationException();
        }
    }
}