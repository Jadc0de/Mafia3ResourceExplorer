using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Gibbed.IO;

namespace ResourceFormats
{
    public class XmlResource1
    {
        private static void ProcessNodes(Stream output, XmlNodeList nodes)
        {
            foreach (XmlNode node in nodes)
            {
                #region Text
                if (node.NodeType == XmlNodeType.Text)
                {
                    // 4 VALUE
                    if (node.Value != null)
                    {
                        output.WriteValueU16((ushort)(node.Value.Length + 1));  // unk1
                        output.WriteValueU8(4);                                 // nodeType
                        output.WriteValueU8(0);                                 // valueType 0
                        output.WriteValueU16((ushort)node.Value.Length);
                        output.WriteStringZ(node.Value);
                    }
                }
                #endregion

                if (node.NodeType == XmlNodeType.Element)
                {
                    string elementName = node.Name;
                    int childCount = node.ChildNodes != null ? node.ChildNodes.Count : 0;
                    int attributeCount = node.Attributes != null ? node.Attributes.Count : 0;

                    ////////////
                    int unk1 = (elementName.Length + 1) + (16 * childCount) + (16 * attributeCount);

                    output.WriteValueU16((ushort)unk1);                     // unk1
                    output.WriteValueU8(1);                                 // nodeType
                    output.WriteValueU8((byte)elementName.Length);          // nameLength
                    output.WriteValueU16((ushort)childCount);               // childCount
                    output.WriteValueU8((byte)attributeCount);              // attributeCount
                    output.WriteStringZ(elementName);

                    if (node.HasChildNodes)
                    {
                        ProcessNodes(output, node.ChildNodes);
                    }

                    #region Attribute
                    for (int i = 0; i < attributeCount; i++)
                    {
                        var attr = node.Attributes[i];

                        // 5 ATTRIBUTE
                        if (attr.Name != null)
                        {
                            output.WriteValueU16((ushort)(attr.Name.Length + 1));   // unk1 5555555555
                            output.WriteValueU8(5);                                 // nodeType
                            output.WriteValueU8((byte)attr.Name.Length);
                            output.WriteStringZ(attr.Name);

                            ////////////
                            // size += attr.Name.Length + 5;
                        }

                        // 4 VALUE
                        if (attr.Value != null)
                        {
                            output.WriteValueU16((ushort)(attr.Value.Length + 1));  // unk1 33333333333
                            output.WriteValueU8(4);                                 // nodeType
                            output.WriteValueU8(0);                                 // valueType 0
                            output.WriteValueU16((ushort)attr.Value.Length);
                            output.WriteStringZ(attr.Value);

                            ////////////
                            // size += attr.Value.Length + 4;
                        }
                    }
                    #endregion

                }
            }
        }

        public static void Serialize(Stream output, string content, Endian endian)
        {
            // output.Position = 0; Crash
            long pos = output.Position;

            output.WriteValueU16(0x5842);
            output.WriteValueU16(1);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(content);

            ProcessNodes(output, doc.ChildNodes);

#if DEBUG
            string _path = "D:/Projects/VS2015/MafiaIIIResExplorer/Binary/testfiles/Test.xml";
            using (var file = File.Create(_path))
            {
                output.Seek(pos, SeekOrigin.Begin);
                var buffer = output.ReadBytes((int)(output.Length - pos));
                file.Write(buffer, 0, buffer.Length);
            }
#endif
            // throw new NotImplementedException();
        }

        public static string Deserialize(Stream input, Endian endian)
        {
            if (input.ReadValueU16(endian) != 0x5842) // 'BX'
            {
                throw new FormatException();
            }

            // if (input.ReadValueU16(endian) > 1)

            ushort unk = input.ReadValueU16(endian);
            if (unk != 1)
            {
                throw new FormatException();
            }

            var root = (NodeEntry)DeserializeNodeEntry(input, endian);
            var settings = new XmlWriterSettings();
            settings.Indent = true;

            var output = new StringBuilder();
            var writer = XmlWriter.Create(output, settings);

            writer.WriteStartDocument();
            WriteXmlNode(writer, root);
            writer.WriteEndDocument();

            writer.Flush();
            return output.ToString();
        }

        private static void WriteXmlNode(XmlWriter writer, NodeEntry node)
        {
            writer.WriteStartElement(node.Name);

            foreach (var attribute in node.Attributes)
            {
                writer.WriteStartAttribute(attribute.Name);
                writer.WriteValue(attribute.Value == null ? "" : attribute.Value.ToString());
                writer.WriteEndAttribute();
            }

            foreach (var child in node.Children)
            {
                WriteXmlNode(writer, child);
            }

            if (node.Value != null)
            {
                writer.WriteValue(node.Value.ToString());
            }

            writer.WriteEndElement();
        }

        private static object DeserializeNodeEntry(Stream input, Endian endian)
        {
            ushort unk1 = input.ReadValueU16(endian);
            byte nodeType = input.ReadValueU8();

            switch (nodeType)
            {
                // ELEMENT
                case 1:
                {
                    var elementNameLength = input.ReadValueU8();
                    var childCount = input.ReadValueU16(endian);
                    var attributeCount = input.ReadValueU8();
                    var elementName = input.ReadString(elementNameLength + 1, true, Encoding.UTF8);

                    var node = new NodeEntry()
                    {
                        Name = elementName,
                    };

                    var children = new List<object>();
                    for (ushort i = 0; i < childCount; i++)
                    {
                        children.Add(DeserializeNodeEntry(input, endian));
                    }

                    if (children.Count == 1 && children[0] is DataValue)
                    {
                        node.Value = (DataValue)children[0];
                    }
                    else
                    {
                        foreach (var child in children)
                        {
                            node.Children.Add((NodeEntry)child);
                        }
                    }

                    for (byte i = 0; i < attributeCount; i++)
                    {
                        var child = DeserializeNodeEntry(input, endian);

                        if (child is NodeEntry)
                        {
                            var data = (NodeEntry)child;

                            if (data.Children.Count != 0 || data.Attributes.Count != 0)
                            {
                                throw new FormatException();
                            }

                            var attribute = new AttributeEntry()
                            {
                                Name = data.Name,
                                Value = data.Value,
                            };
                            node.Attributes.Add(attribute);
                        }
                        else
                        {
                            node.Attributes.Add((AttributeEntry)child);
                        }
                    }

#if DEBUG
                    ushort check = (ushort)((elementName.Length + 1) + (16 * childCount) + (16 * attributeCount));
                    if (check != unk1)
                    {
                        System.Diagnostics.Debugger.Break();
                    }

#endif
                    return node;
                }

                // ATTRIBUTE VALUE 
                case 4:
                {
                    var valueType = input.ReadValueU8();
                    if (valueType == 0)
                    {
                        var valueLength = input.ReadValueU16(endian);
                        var value = input.ReadString(valueLength + 1, true, Encoding.UTF8);

#if DEBUG
                        if ((value.Length + 1) != unk1)
                        {
                            System.Diagnostics.Debugger.Break();
                        }
#endif

                        return new DataValue(DataType.String, value);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }

                // ATTRIBUTE 
                case 5:
                {
                    var nameLength = input.ReadValueU8();
                    var name = input.ReadString(nameLength + 1, true, Encoding.UTF8);
                    //name = "ATTRIBUTE_" + name;

                    var attribute = new NodeEntry()
                    {
                        Name = name,
                    };

                    attribute.Value = (DataValue)DeserializeNodeEntry(input, endian);

#if DEBUG
                    if ((name.Length + 1) != unk1)
                    {
                        System.Diagnostics.Debugger.Break();
                    }
#endif

                    return attribute;
                }

                default:
                {
                    throw new NotImplementedException();
                }
            }
        }

        private class NodeEntry
        {
            public string Name;
            public DataValue Value;
            public List<NodeEntry> Children = new List<NodeEntry>();
            public List<AttributeEntry> Attributes = new List<AttributeEntry>();
        }

        private class AttributeEntry
        {
            public string Name;
            public DataValue Value;
        }

        private enum DataType
        {
            String = 0,
        }

        private class DataValue
        {
            public DataType Type;
            public object Value;

            public DataValue(DataType type, object value)
            {
                Type = type;
                Value = value;
            }

            public override string ToString()
            {
                return Value.ToString();
            }
        }

    }
}
