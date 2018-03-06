using System;
using System.Collections.Generic;
//using System.Drawing;
using System.IO;
using FileFormats;
using Gibbed.IO;

namespace ResourceFormats
{
    public class TableData : IResourceType
    {
        public ulong NameHash;
        public string Name;

        public List<Row> Rows = new List<Row>();
        public List<Column> Columns = new List<Column>();

        public override string ToString()
        {
            return Name;
        }

        public void Serialize(ushort version, Stream input, Endian endian)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(ushort version, Stream input, Endian endian)
        {
            NameHash = input.ReadValueU64(endian);
            Name = input.ReadStringU16(endian);

            if (version >= 2)
            {
                throw new NotSupportedException();
            }
            else
            {
                var columnCount = input.ReadValueU16(endian);

                var unk1 = input.ReadValueU32(endian);
                var unk2 = input.ReadValueU32(endian);

                var rowSize = input.ReadValueU32(endian);
                var rowCount = input.ReadValueU32(endian);
                var data = input.ReadToMemoryStream(rowSize * rowCount);

                Columns = new List<Column>();
                for (uint i = 0; i < columnCount; i++)
                {
                    Columns.Add(new Column()
                    {
                        NameHash = input.ReadValueU32(endian),
                        Type = (ColumnType)input.ReadValueU8(),
                        Unknown2 = input.ReadValueU8(),
                        Unknown3 = input.ReadValueU16(endian),
                    });
                }

                input = null;

                Rows.Clear();
                for (uint i = 0; i < rowCount; i++)
                {
                    var row = new Row();

                    data.Seek(i * rowSize, SeekOrigin.Begin);
                    foreach (var column in Columns)
                    {
                        if ((byte)column.Type > 163)
                        {
                            throw new FormatException();
                        }

                        switch (column.Type)
                        {
                            case ColumnType.Boolean:
                            {
                                var value = data.ReadValueU32(endian);
                                if (value != 0 && value != 1)
                                {
                                    throw new FormatException();
                                }
                                row.Values.Add(value != 0);
                                break;
                            }

                            case ColumnType.Float32:
                            {
                                var value = data.ReadValueF32(endian);
                                row.Values.Add(value);
                                break;
                            }

                            case ColumnType.Signed32:
                            {
                                var value = data.ReadValueS32(endian);
                                row.Values.Add(value);
                                break;
                            }

                            case ColumnType.Unsigned32:
                            {
                                var value = data.ReadValueU32(endian);
                                row.Values.Add(value);
                                break;
                            }

                            case ColumnType.Flags32:
                            {
                                var value = data.ReadValueU32(endian);
                                row.Values.Add(value);
                                break;
                            }

                            case ColumnType.Hash64:
                            {
                                var value = data.ReadValueU64(endian);
                                row.Values.Add(value);
                                break;
                            }

                            case ColumnType.String8:
                            {
                                string value = data.ReadString(8, true);
                                row.Values.Add(value);
                                break;
                            }

                            case ColumnType.String16:
                            {
                                string value = data.ReadString(16, true);
                                row.Values.Add(value);
                                break;
                            }

                            case ColumnType.String32:
                            {
                                string value = data.ReadString(32, true);
                                row.Values.Add(value);
                                break;
                            }

                            case ColumnType.String64:
                            {
                                string value = data.ReadString(64, true);
                                row.Values.Add(value);
                                break;
                            }

                            case ColumnType.Color:
                            {
                                float r = data.ReadValueF32(endian);
                                float g = data.ReadValueF32(endian);
                                float b = data.ReadValueF32(endian);
                                // TODO: de-stupidize this
                                row.Values.Add(string.Format("{0}, {1}, {2}", r, g, b));
                                break;
                            }

                            case ColumnType.Hash64AndString32:
                            {
                                var hash = data.ReadValueU64(endian);
                                string value = data.ReadString(32, true);
                                row.Values.Add(value);
                                break;
                            }

                            default:
                            {
                                throw new FormatException();
                            }
                        }
                    }

                    Rows.Add(row);
                }
            }
        }

        public class Column
        {
            public uint NameHash;
            public ColumnType Type;
            public byte Unknown2;
            public ushort Unknown3;

            public override string ToString()
            {
                return string.Format("{0:X8} : {1} ({2}, {3})",
                                     NameHash,
                                     Type,
                                     Unknown2,
                                     Unknown3);
            }
        }

        public class Row
        {
            public List<object> Values = new List<object>();

            public override string ToString()
            {
                var values = new string[Values.Count];
                for (int i = 0; i < Values.Count; i++)
                {
                    values[i] = Values[i].ToString();
                }
                return string.Join(", ", values);
            }
        }

        public enum ColumnType : byte
        {
            Boolean = 1,
            Float32 = 2,
            Signed32 = 3,
            Unsigned32 = 4,
            Flags32 = 5,
            Hash64 = 6,
            String8 = 8,
            String16 = 16,
            String32 = 32,
            String64 = 64,
            Color = 66,
            Hash64AndString32 = 132,
        }

        public static Type GetValueTypeForColumnType(ColumnType type)
        {
            switch (type)
            {
                case ColumnType.Boolean:
                {
                    return typeof(bool);
                }

                case ColumnType.Float32:
                {
                    return typeof(float);
                }

                case ColumnType.Signed32:
                {
                    return typeof(int);
                }

                case ColumnType.Unsigned32:
                {
                    return typeof(uint);
                }

                case ColumnType.Flags32:
                {
                    return typeof(uint);
                }

                case ColumnType.Hash64:
                {
                    return typeof(ulong);
                }

                case ColumnType.String8:
                case ColumnType.String16:
                case ColumnType.String32:
                case ColumnType.String64:
                {
                    return typeof(string);
                }

                ///*
                case ColumnType.Color:
                {
                    return typeof(string);
                }
                //*/

                case ColumnType.Hash64AndString32:
                {
                    return typeof(string);
                }
            }

            throw new ArgumentException("unhandled type", "type");
        }
    }
}
