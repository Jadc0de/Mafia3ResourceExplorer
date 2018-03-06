using System;
using System.IO;
using Gibbed.IO;

namespace FileFormats.Archive
{
    public struct ResourceType : IEquatable<ResourceType>
    {
        public uint Id;
        public string Name;
        public uint Parent;

        public void Write(Stream output, Endian endian)
        {
            output.WriteValueU32(Id, endian);
            output.WriteStringU32(Name, endian);
            output.WriteValueU32(Parent, endian);
        }

        public static ResourceType Read(Stream input, Endian endian)
        {
            ResourceType instance;
            instance.Id = input.ReadValueU32(endian);
            instance.Name = input.ReadStringU32(endian);
            instance.Parent = input.ReadValueU32(endian);
            return instance;
        }

        public bool Equals(ResourceType other)
        {
            return Id == other.Id &&
                   string.Equals(Name, other.Name) == true &&
                   Parent == other.Parent;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj) == true)
            {
                return false;
            }
            return obj is ResourceType && Equals((ResourceType)obj) == true;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (int)Id;
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int)Parent;
                return hashCode;
            }
        }

        public static bool operator ==(ResourceType left, ResourceType right)
        {
            return left.Equals(right) == true;
        }

        public static bool operator !=(ResourceType left, ResourceType right)
        {
            return left.Equals(right) == false;
        }
    }
}
