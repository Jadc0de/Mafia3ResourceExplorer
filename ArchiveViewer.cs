using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.XPath;
using System.Threading;
using FileFormats;
using FileFormats.Archive;
using ScintillaNET;
using Gibbed.IO;
using System.ComponentModel;

namespace ResourceExplorer
{
    public partial class ArchiveViewer : Form
    {
        private ArchiveFile _Archive = null;
        private List<string> _Descr = null;
        private Options compress = Options.None;

        public ArchiveViewer()
        {
            InitializeComponent();

            Scintilla.SetModulePath(Path.Combine(Application.StartupPath, "scilexer.dll"));

            FileFormats.Hashing.FileHash.Load();

            System.Reflection.PropertyInfo property = _SplitContainer.Panel2.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            property.SetValue(_SplitContainer.Panel2, true, null);

            _HintLabel.Text = "";

            compress = (Options)Enum.Parse(typeof(Options), Properties.Settings.Default.ArchiveViewer_Compress);
            if (compress == Options.Compress)
            {
                tsBest.Checked = true;
                tsNone.Checked = false;
            }
            else
            {
                tsBest.Checked = false;
                tsNone.Checked = true;
            }

            tsOne.Checked = Properties.Settings.Default.ArchiveViewer_OneBlock;
            if (Properties.Settings.Default.ArchiveViewer_OneBlock)
            {
                compress |= Options.OneBlock;
            }

            tsOpenSelected.Checked = Properties.Settings.Default.ArchiveViewer_OpenSelect;

            tsHexMode.Checked = Properties.Settings.Default.ArchiveViewer_HexMode;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            ResourceViewerFactory.Dispose();

            Properties.Settings.Default.Save();

            base.OnClosing(e);
        }

        private void RemoveControl()
        {
            if (_SplitContainer.Panel2.Controls.Count <= 0) return;
            // _SplitContainer.Panel2.Controls[0].Dispose();
            _SplitContainer.Panel2.Controls.Clear();
            // GC.Collect();
        }
		
        private void OnOpen(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Program.FilePath))
            {
                _OpenArchiveDialog.InitialDirectory = Properties.Settings.Default.ArchiveViewer_InputSDS;
            }
            else
            {
                _OpenArchiveDialog.InitialDirectory = Path.GetDirectoryName(Program.FilePath) + Path.DirectorySeparatorChar;
                Properties.Settings.Default.ArchiveViewer_InputSDS = _OpenArchiveDialog.InitialDirectory;
            }

            if (_OpenArchiveDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            Properties.Settings.Default.ArchiveViewer_InputSDS = Path.GetDirectoryName(_OpenArchiveDialog.FileName) + Path.DirectorySeparatorChar;
            Properties.Settings.Default.Save();

            RemoveControl();

            LoadArchive(_OpenArchiveDialog.FileName);
        }

        void ArchiveThreadLoad(string path)
        {
            if (InvokeRequired == true)
            {
                Invoke((MethodInvoker)delegate
                {
                    UseWaitCursor = true;
                    _EntryTreeView.Enabled = false;
                    _OpenArchiveButton.Enabled = false;
                    _SaveArchiveButton.Enabled = false;
                });
            }

            using (var input = File.OpenRead(path))
            {
                _Archive = new ArchiveFile();
                _Archive.Deserialize(input);
            }

            if (InvokeRequired == true)
            {
                Invoke((MethodInvoker)delegate
                {
                    UseWaitCursor = false;
                    _EntryTreeView.Enabled = true;
                    _OpenArchiveButton.Enabled = true;
                    _SaveArchiveButton.Enabled = true;
                });
            }
        }

        public void LoadArchive(string path)
        {
            if (_Archive != null)
            {
                _Archive.Dispose();
                _Archive = null;
            }
#if DEBUG
            using (var input = File.OpenRead(path))
            {
                _Archive = new ArchiveFile();
                _Archive.Deserialize(input);
            }
#else
            Thread thread = new Thread(() => ArchiveThreadLoad(path));
            thread.Start();

            while (thread.IsAlive)
            {
                Application.DoEvents();
            }
#endif
            if (_Archive == null) return;
            Program.FilePath = path;
            _Descr = LoadDescriptions(_Archive.ResourceInfoXml);
            BuildEntryTree();
        }

        private static List<string> LoadDescriptions(string xml)
        {
            var items = new List<string>();
            if (string.IsNullOrEmpty(xml) == false)
            {
                using (var reader = new StringReader(xml))
                {
                    var doc = new XPathDocument(reader);
                    var nav = doc.CreateNavigator();
                    var nodes = nav.Select("/xml/ResourceInfo/SourceDataDescription");
                    while (nodes.MoveNext() == true)
                    {
                        items.Add(nodes.Current.Value);
                    }
                }
            }
            return items;
        }

        private void BuildEntryTree()
        {
            _EntryTreeView.BeginUpdate();
            _EntryTreeView.Nodes.Clear();

            var root = new TreeNode()
            {
                // ReSharper disable LocalizableElement
                Text = Path.GetFileName(Program.FilePath) ?? "<unknown>",
                ImageKey = "SDS",
                SelectedImageKey = "SDS",
                // ReSharper restore LocalizableElement
            };

            var invalidParentNode = new TreeNode()
            {
                // ReSharper disable LocalizableElement
                Text = "(invalid parent)",
                // ReSharper restore LocalizableElement
            };

            var typeNodes = new Dictionary<string, TreeNode>();
            foreach (var type in _Archive.ResourceTypes)
            {
                var hasImage = _EntryTreeView.ImageList.Images.ContainsKey(type.Name);
                var node = new TreeNode()
                {
                    Text = type.Name,
                    ImageKey = hasImage == true ? type.Name : "",
                    SelectedImageKey = hasImage == true ? type.Name : "",
                };
                typeNodes.Add(type.Name, node);
            }

            foreach (var type in _Archive.ResourceTypes)
            {
                var node = typeNodes[type.Name];
                node.Tag = type;
                if (type.Parent == 0)
                {
                    root.Nodes.Add(node);
                    continue;
                }

                TreeNode parentNode;
                var parent = _Archive.ResourceTypes.SingleOrDefault(r => r.Id == type.Id + type.Parent);
                if (parent != default(FileFormats.Archive.ResourceType) && parent == type)
                {
                    parentNode = typeNodes[parent.Name];
                }
                else
                {
                    parentNode = invalidParentNode;
                }

                if (parent == type)
                {
                    throw new InvalidOperationException();
                }

                parentNode.Nodes.Add(node);

                if (string.IsNullOrEmpty(node.ImageKey) == true)
                {
                    node.ImageKey = parentNode.ImageKey;
                    node.SelectedImageKey = parentNode.SelectedImageKey;
                }
            }

            for (int i = 0; i < _Archive.ResourceEntries.Count; i++)
            {
                var resource = _Archive.ResourceEntries[i];
                // var descript = i < _Descr.Count && _Descr[i] != "not available" ? _Descr[i] : "unknown_" + i;
                var descript = i < _Descr.Count ? _Descr[i] : "unknown_" + i;
                bool findname = i < _Descr.Count ? false : true;

                var type = _Archive.ResourceTypes.SingleOrDefault(r => r.Id == resource.TypeId);
                if (type == default(FileFormats.Archive.ResourceType))
                {
                    throw new KeyNotFoundException();
                }

                //
                if (findname)
                {
                    if (type.Name == "XML")
                    {
                        int size_tag = BitConverter.ToInt32(resource.Data, 0);
                        string tag = System.Text.Encoding.ASCII.GetString(resource.Data, 4, size_tag);
                        byte unk1 = (byte)BitConverter.ToChar(resource.Data, size_tag + 4);
                        int size_name = BitConverter.ToInt32(resource.Data, size_tag + 4 + 1);
                        string name = System.Text.Encoding.ASCII.GetString(resource.Data, size_tag + 4 + 1 + 4, size_name);
                        byte unk3 = (byte)BitConverter.ToChar(resource.Data, size_tag + 4 + 1 + 4 + size_name);

                        descript = name;
                    }
                    else if (type.Name == "MemFile")
                    {
                        int size = BitConverter.ToInt32(resource.Data, 4);
                        descript = System.Text.Encoding.ASCII.GetString(resource.Data, 8, size);
                    }
                    else if (type.Name == "Texture")
                    {
                        ulong hash = BitConverter.ToUInt64(resource.Data, 0);
                        descript = FileFormats.Hashing.FileHash.Name(hash);
                        if (descript.IndexOf(".dds") == -1)
                        {
                            descript += ".dds";
                        }
                    }
                    else if (type.Name == "SystemObjectDatabase")
                    {
                        int datasize = BitConverter.ToInt32(resource.Data, 8);
                        int sizemp = BitConverter.ToInt32(resource.Data, 8 + 4 + 8 + 4 + datasize);
                        int sizenm = BitConverter.ToInt32(resource.Data, 8 + 4 + 8 + 4 + datasize + 4 + sizemp + 8);
                        descript = System.Text.Encoding.ASCII.GetString(resource.Data, 8 + 4 + 8 + 4 + datasize + 4 + sizemp + 8 + 4, sizenm);
                    }
                    else if (type.Name == "Flash")
                    {
                        int size = BitConverter.ToInt16(resource.Data, 0);
                        descript = System.Text.Encoding.ASCII.GetString(resource.Data, 2, size);
                    }
                    /*else if (type.Name == "Generic")
                    {
                        var generic = new ResourceFormats.GenericResource();
                        using (var stream = new MemoryStream(resource.Data, false))
                        {
                            generic.Deserialize(resource.Version, stream, _Archive.Endian);
                        }
                    }*/
                }
                //else
                //{
                //    FileFormats.Hashing.FileHash.Add(descript);
                //}

                var typeNode = typeNodes[type.Name];
                var node = new TreeNode(descript)
                {
                    Tag = resource,
                    ImageKey = typeNode.ImageKey,
                    SelectedImageKey = typeNode.SelectedImageKey,
                };

                typeNode.Nodes.Add(node);
            }

            _EntryTreeView.Nodes.Add(root);
            root.Expand();
            _EntryTreeView.EndUpdate();
        }

        private void OnSelectEntry(object sender, TreeViewEventArgs e)
        {
            if (_EntryTreeView.SelectedNode == null)
            {
                _HintLabel.Text = string.Format("\"{0}\"", Path.GetFileName(Program.FilePath));
                return;
            }

            var entry = _EntryTreeView.SelectedNode.Tag as FileFormats.Archive.ResourceEntry;
            if (entry == null)
            {
                _HintLabel.Text = string.Format("\"{0}\"", Path.GetFileName(Program.FilePath));
                return;
            }

            // string.Format(new System.Globalization.CultureInfo("en")
            _HintLabel.Text = string.Format("Version {0} Bytes {1:N0}", entry.Version, entry.Data.Length);

            if (tsOpenSelected.Checked)
            {
                OpenResourceEntry(_EntryTreeView.SelectedNode);
            }
        }

        private void OpenResourceEntry(TreeNode node)
        {
            if (node == null)
            {
                return;
            }

            var resourceEntry = node.Tag as FileFormats.Archive.ResourceEntry;
            if (resourceEntry == null)
            {
                return;
            }

            var type = _Archive.ResourceTypes.SingleOrDefault(r => r.Id == resourceEntry.TypeId);
            if (type == default(FileFormats.Archive.ResourceType))
            {
                throw new KeyNotFoundException();
            }

            RemoveControl();
            var viewer = ResourceViewerFactory.Create(type.Name, resourceEntry);
            ((Form)viewer).TopLevel = false;
            ((Form)viewer).FormBorderStyle = FormBorderStyle.None;
            ((Form)viewer).Dock = DockStyle.Fill;
            ((Form)viewer).Enabled = true;
            _SplitContainer.Panel2.Controls.Add(((Form)viewer));
            viewer.LoadResourceEntry(resourceEntry, node.Text, _Archive.Endian);
            viewer.Show();
        }

        private void OnOpenEntry1(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                _EntryTreeView.SelectedNode = e.Node;
            }

            if (!tsOpenSelected.Checked)
            {
                OpenResourceEntry(e.Node);
            }
        }

        private void OnOpenEntry2(object sender, EventArgs e)
        {
            OpenResourceEntry(_EntryTreeView.SelectedNode);
        }

        private void OnOpenContextMenu(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_EntryTreeView.SelectedNode == null ||
                _EntryTreeView.SelectedNode.Tag == null ||
                (_EntryTreeView.SelectedNode.Tag is FileFormats.Archive.ResourceType) == false)
            {
                e.Cancel = true;
                return;
            }
        }

        private void ArchiveThreadSave(string path)
        {
            if (InvokeRequired == true)
            {
                Invoke((MethodInvoker)delegate
                {
                    UseWaitCursor = true;
                    _EntryTreeView.Enabled = false;
                    _OpenArchiveButton.Enabled = false;
                    _SaveArchiveButton.Enabled = false;
                });
            }

            using (var output = File.Create(path))
            {
                _Archive.Serialize(output, compress);
            }

            if (InvokeRequired == true)
            {
                Invoke((MethodInvoker)delegate
                {
                    UseWaitCursor = false;
                    _EntryTreeView.Enabled = true;
                    _OpenArchiveButton.Enabled = true;
                    _SaveArchiveButton.Enabled = true;
                    Helpers.ShowInformation(string.Format("Completed {0}", path));
                });
            }
        }

        private void OnSaveArchive(object sender, EventArgs e)
        {
            if (_Archive == null) return;

            _SaveArchiveDialog.InitialDirectory = Path.GetDirectoryName(Program.FilePath);
            _SaveArchiveDialog.FileName = Path.GetFileName(Program.FilePath);
            if (_SaveArchiveDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

#if DEBUG
            using (var output = File.Create(this._SaveArchiveDialog.FileName))
            {
                this._Archive.Serialize(output, compress);
            }
#else
            Thread thread = new Thread(() => ArchiveThreadSave(_SaveArchiveDialog.FileName));
            thread.Start();
#endif
        }

        private void OnCheckNone(object sender, EventArgs e)
        {
            if (!tsNone.Checked)
            {
                tsBest.Checked = false;
                tsNone.Checked = true;

                compress &= ~Options.Compress;
                compress |= Options.None;

                Properties.Settings.Default.ArchiveViewer_Compress = compress.ToString("g");
            }
        }

        private void OnCheckBest(object sender, EventArgs e)
        {
            if (!tsBest.Checked)
            {
                tsNone.Checked = false;
                tsBest.Checked = true;

                compress &= ~Options.None;
                compress |= Options.Compress;

                Properties.Settings.Default.ArchiveViewer_Compress = compress.ToString("g");
            }
        }

        private void OnCheckOne(object sender, EventArgs e)
        {
            if (tsOne.Checked)
            {
                compress |= Options.OneBlock;
            }
            else
            {
                compress &= ~Options.OneBlock;
            }

            Properties.Settings.Default.ArchiveViewer_OneBlock = tsOne.Checked;
        }

        private void OnCheckHex(object sender, EventArgs e)
        {
            Properties.Settings.Default.ArchiveViewer_HexMode = tsHexMode.Checked;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            Properties.Settings.Default.ArchiveViewer_OpenSelect = tsOpenSelected.Checked;
            Properties.Settings.Default.Save();
        }

        private void OnPackageShow(object sender, EventArgs e)
        {
            using (var form = new PCKGViewer())
            {
                form.ShowDialog(this);
            }
        }

        private void OnFnvShow(object sender, EventArgs e)
        {
            using (var form = new HashViewer())
            {
                form.ShowDialog(this);
            }
        }

        private void OnAboutShow(object sender, EventArgs e)
        {
#if _DEBUG
            var packageSettings = new ManagedBass.PackageSettings(@"D:\Projects\VS2015\MafiaIIIResExplorer\Binary\Output\");
            var jobForm = new ManagedBass.JobForm(packageSettings);
            jobForm.Start(this);

            Dictionary<ulong, string> dds = new Dictionary<ulong, string>();

            /*using (var input = File.OpenRead("default.mtl"))
            {
                Utility.MaterialLib material = new Utility.MaterialLib(input);

                using (var sw = new StreamWriter("hashlist.txt", false))
                {
                    foreach (var m in material.Materials)
                    {
                        foreach (var s in m.Value.Samplers)
                        {
                            if (!dds.ContainsKey(s.textureHash))
                            {
                                dds.Add(s.textureHash, s.texture);
                            }
                        }
                    }

                    foreach (var s in dds.Values)
                    {
                        sw.WriteLine("{0}", s);
                    }
                }
            }*/

            // #2
            /*dds.Clear();
            using (var sr = new StreamReader("hashlist.txt"))
            {
                while (!sr.EndOfStream)
                {
                    string s = sr.ReadLine();
                    ulong textureHash = FileFormats.Hashing.FNV64.Hash(s);

                    if (!dds.ContainsKey(textureHash))
                    {
                        dds.Add(textureHash, s);
                    }
                }
            }

            using (var sw = new StreamWriter("hashlist.txt", false))
            {
                foreach (var s in dds.Values)
                {
                    sw.WriteLine("{0}", s);
                }
            }*/

            // #3
            /*dds.Clear();
            using (var input = File.OpenRead("default.mtl"))
            {
                Utility.MaterialLib material = new Utility.MaterialLib(input);

                using (var sw = new StreamWriter("materials.txt", false))
                {
                    foreach (var m in material.Materials.Values)
                    {
                        sw.WriteLine("{0:X16}", m.HashComplete.Swap());

                        foreach (var s in m.Samplers)
                        {
                            sw.WriteLine("{0}", s.texture);
                        }

                        sw.WriteLine();
                    }
                }
            }*/
#else
            using (var form = new About())
            {
                form.ShowDialog(this);
            }
#endif
        }

        private void OnContextMenuExtract(object sender, EventArgs e)
        {
            string pathSDS = Program.ExtractPath(Program.FilePath);
            var entryType = (ResourceType)_EntryTreeView.SelectedNode.Tag;
            string pathType = Path.Combine(pathSDS, entryType.Name);

            var type = _Archive.ResourceTypes.SingleOrDefault(r => r.Name == entryType.Name);
            if (type != null && type.Name == entryType.Name)
            {
                var ResourceEntrys = new List<ResourceEntry>(_Archive.ResourceEntries.Where(c => c.TypeId == type.Id));
                if (ResourceEntrys.Count > 0)
                {
                    if (!Directory.Exists(pathType))
                    {
                        Directory.CreateDirectory(pathType);
                    }
                }

                for (int i = 0; i < ResourceEntrys.Count; i++)
                {
                    // string descript = i < _Descr.Count ? _Descr[i] : "unknown_" + i + ".bin";

                    string descript = "unknown_" + i + ".bin";

                    switch (type.Name)
                    {
                    case "XML":
                        {
                            var resource = new ResourceFormats.XmlResource();
                            using (var data = new MemoryStream(ResourceEntrys[i].Data, false))
                            {
                                resource.Deserialize(ResourceEntrys[i].Version, data, _Archive.Endian);
                            }
                            string path = pathType + resource.Name.Replace("/", "\\");
                            if (!Directory.Exists(Path.GetDirectoryName(path)))
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName(path));
                            }
                            using (var sr = new StreamWriter(path + ".xml", false, System.Text.Encoding.UTF8))
                            {
                                sr.Write(resource.Content);
                            }
                            resource = null;
                        }
                        break;
                    case "MemFile":
                        {
                            var resource = new ResourceFormats.MemFileResource();
                            using (var data = new MemoryStream(ResourceEntrys[i].Data, false))
                            {
                                resource.Deserialize(ResourceEntrys[i].Version, data, _Archive.Endian);
                            }
                            string path = pathType + resource.Name.Replace("/", "\\");
                            if (!Directory.Exists(Path.GetDirectoryName(path)))
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName(path));
                            }
                            File.WriteAllBytes(path, resource.Data); //
                            resource = null;
                        }
                        break;
                    case "Texture":
                        {
                            var resource = new ResourceFormats.TextureResource();
                            using (var data = new MemoryStream(ResourceEntrys[i].Data, false))
                            {
                                resource.Deserialize(ResourceEntrys[i].Version, data, _Archive.Endian);
                            }
                            descript = FileFormats.Hashing.FileHash.Name(resource.NameHash);
                            if (!descript.Contains(".dds"))
                            {
                                descript += ".dds";
                            }
                            File.WriteAllBytes(Path.Combine(pathType, descript), resource.Data);
                            resource = null;
                        }
                        break;
                    case "Flash":
                        {
                            var resource = new ResourceFormats.FlashResource();
                            using (var data = new MemoryStream(ResourceEntrys[i].Data, false))
                            {
                                resource.Deserialize(ResourceEntrys[i].Version, data, _Archive.Endian);
                            }
                            string path = pathType + resource.Path.Replace("/", "\\");
                            if (!Directory.Exists(Path.GetDirectoryName(path)))
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName(path));
                            }
                            // gfx
                            File.WriteAllBytes(path, resource.Data); //

#if DEBUG
                            // swf (FWS)
                            if (resource.Data.Length >= 4)
                            {
                                resource.Data[0] = 0x46;
                                resource.Data[1] = 0x57;
                                resource.Data[2] = 0x53;
                            }
                            File.WriteAllBytes(Path.ChangeExtension(path, ".swf"), resource.Data);
#endif
                            resource = null;
                        }
                        break;
                    default:
                        {
                            File.WriteAllBytes(Path.Combine(pathType, descript), ResourceEntrys[i].Data);
                        }
                        break;
                    }

                    Application.DoEvents();
                }

                Helpers.ShowInformation("Output: " + pathType + "\r\nCompleted.", "Extracted");
            }
        }

        private void OnContextMenuDelete(object sender, EventArgs e)
        {
            if (_EntryTreeView.SelectedNode != null)
            {
                // Root node's Parent property is null, so do check
                if (_EntryTreeView.SelectedNode.Parent != null)
                {
                    _EntryTreeView.SelectedNode.Parent.Nodes.Remove(_EntryTreeView.SelectedNode);
                }
                else
                {
                    _EntryTreeView.Nodes.Remove(_EntryTreeView.SelectedNode);
                }
            }
        }

    }
}
