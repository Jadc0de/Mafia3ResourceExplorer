using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using Gibbed.IO;
using ManagedBass;
using ResourceEntry = FileFormats.Archive.ResourceEntry;
using System.ComponentModel;

namespace ResourceExplorer
{
    public partial class BnkViewer : Form, IResourceViewer
    {
        private PlayerControl _AudioPlaybackPanel;
        private ResourceEntry _ResourceEntry;
        private Endian _Endian;
        private string _Description;
        private ResourceFormats.MemFileResource _Resource;

        private byte[] _HeaderGeneral = null;
        private DIDXSection _DIDXSection = null;
        private FileStream _DIDXData = null;
        private Dictionary<uint, byte[]> _Section = new Dictionary<uint, byte[]>();
        private uint _LastPlaySound = 0;
        private string _LastWemPath = "";

        private string _PathCache = Path.Combine(Program.BinaryPath, "stream");
        private string _PathImaAdpcm = Path.Combine(Program.BinaryPath, "wwise_ima_adpcm.exe");
        private string _PathWw2Ogg = Path.Combine(Program.BinaryPath, "ww2ogg.exe");
        private string _PathRevorb = Path.Combine(Program.BinaryPath, "revorb.exe");
        private string _PathOggDec = Path.Combine(Program.BinaryPath, "oggdec.exe");
        private string _TempWemFile = Path.Combine(Program.BinaryPath, "input.wem");
        private string _TempOggFile = Path.Combine(Program.BinaryPath, "output.ogg");
        private string _TempWavFile  = Path.Combine(Program.BinaryPath, "output.wav");

        private Dictionary<int, long> _Zero = new Dictionary<int, long>();
        private bool _ZeroWrite = true;

        public BnkViewer()
        {
            InitializeComponent();

            _AudioPlaybackPanel = new PlayerControl();
            this._Container.Panel2.Controls.Add(_AudioPlaybackPanel);
            _AudioPlaybackPanel.Anchor = (((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right);
            _AudioPlaybackPanel.Location = new System.Drawing.Point(0, -1);
            _AudioPlaybackPanel.Name = "_AudioPlaybackPanel";
            _AudioPlaybackPanel.Size = new System.Drawing.Size(375, 196);
            _AudioPlaybackPanel.TabIndex = 1;
            _AudioPlaybackPanel.Volume = Properties.Settings.Default.BnkViewer_Volume;
            _AudioPlaybackPanel.Loop = Properties.Settings.Default.BnkViewer_Loop;

            Console.SetOut(new ControlWriter(textLog));
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (Form.ModifierKeys == Keys.None && keyData == Keys.Escape)
            {
                Close();
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }

        /*protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            _AudioPlaybackPanel.FreeStream();
        }*/

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);

            _AudioPlaybackPanel.FreeStream();
        }

        public void LoadResourceEntry(ResourceEntry resourceEntry, string description, Endian endian)
        {
            var resource = new ResourceFormats.MemFileResource();
            using (var data = new MemoryStream(resourceEntry.Data, false))
            {
                resource.Deserialize(resourceEntry.Version, data, endian);
            }

            _ResourceEntry = resourceEntry;
            _Endian = endian;
            _Description = description;
            _Resource = resource;

            //
            textLog.Clear();
            _LastPlaySound = 0;
            _LastWemPath = "";

            _HeaderGeneral = null;
            _DIDXSection = null;
            if (_DIDXData != null)
            {
                _DIDXData.Close();
                _DIDXData = null;
            }
            _Section.Clear();

            var input = new MemoryStream(resource.Data, false);

            if (input.ReadValueU32() != 0x44484b42)
            {
                Console.WriteLine("Invalid .bnk file");
                return;
            }

            uint headerSize = input.ReadValueU32();
            _HeaderGeneral = input.ReadBytes(headerSize);

#if DEBUG
            string hex = BitConverter.ToString(_HeaderGeneral).Replace("-", "");
            Console.WriteLine("header=" + hex);
#endif

            while (input.Position != input.Length)
            {
                uint section = input.ReadValueU32();
                if (section == 0x58444944) // DIDX
                {
                    _DIDXSection = ReadDIDXSection(input);
                }
                else if (section == 0x41544144) // DATA
                {
                    uint SectionSize = input.ReadValueU32(); ;
                    _DIDXData = new FileStream(_PathCache, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                    _DIDXData.WriteFromStream(input, SectionSize);
                    _DIDXData.Seek(0L, SeekOrigin.Begin);
                    Console.WriteLine("section='{0}' {1}", "DATA", SectionSize);
                }
                else if (section == 0x43524948) // HIRC
                {
#if _DEBUG
                    var posBegin = input.Position;
#endif

                    uint SectionSize = input.ReadValueU32();
                    _Section.Add(section, input.ReadBytes(SectionSize));
                    Console.WriteLine("section='{0}' {1}", "HIRC", SectionSize);
#if _DEBUG
                    var posEnd = input.Position;
                    input.Seek(posBegin, SeekOrigin.Begin);
                    var pHIRCSection = ReadHIRCSection(input);
                    input.Seek(posEnd, SeekOrigin.Begin);
#endif
                }
                else
                {
                    uint SectionSize = input.ReadValueU32();
                    byte[] buffer = input.ReadBytes(SectionSize);
                    _Section.Add(section, buffer);
                    string name = Encoding.ASCII.GetString(BitConverter.GetBytes(section));
                    Console.WriteLine("section='{0}' {1}", name, SectionSize);
                }
            }

            BuildEntryTree();

            input.Close();
        }

        private byte[] SaveResourceEntry()
        {
            byte[] result = null;

            if (!RebuildDIDXData())
            {
                return result;
            }

            using (var output = new MemoryStream())
            {
                output.Position = 0L;
                output.WriteValueU32(0x44484b42); // BKHD

                // Header
                output.WriteValueS32(_HeaderGeneral.Length);
                output.WriteBytes(_HeaderGeneral);

                // DIDX
                if (_DIDXSection != null)
                {
                    //uint SectionSize = (uint)(_DIDXSection.DIDXFiles.Count * 12);
                    //Debug.Assert(_DIDXSection.SectionSize == size);

                    output.WriteValueU32(0x58444944); // DIDX
                    output.WriteValueU32(_DIDXSection.SectionSize);

                    foreach (var wemFile in _DIDXSection.DIDXFiles)
                    {
                        output.WriteValueU32(wemFile.Id);
                        output.WriteValueU32((uint)wemFile.Offset);
                        output.WriteValueU32(wemFile.Length);
                    }
                }

                // DATA
                if (_DIDXData != null)
                {
                    _DIDXData.Seek(0L, SeekOrigin.Begin);
                    output.WriteValueU32(0x41544144); // DATA
                    output.WriteValueU32((uint)_DIDXData.Length);
                    output.WriteFromStream(_DIDXData, _DIDXData.Length);
                }

                foreach (var section in _Section)
                {
                    output.WriteValueU32(section.Key);
                    output.WriteValueS32(section.Value.Length);
                    output.WriteBytes(section.Value);
                }

                output.Flush();
                result = output.ToArray();
            }

            return result;
        }

        public DIDXSection ReadDIDXSection(Stream input)
        {
            DIDXSection retVar = new DIDXSection();
            List<WEMFile> WEMFileList = new List<WEMFile>();

            uint SectionSize = input.ReadValueU32();
            retVar.SectionSize = SectionSize;
            long EndOfDIDXSection = input.Position + SectionSize;

            Console.WriteLine("section='{0}' {1}", "DIDX", SectionSize);

            long zeroPos = 0L;
            if (_ZeroWrite)
            {
                _Zero.Clear();
            }

            while (input.Position != EndOfDIDXSection)
            {
                uint id = input.ReadValueU32();
                long offset = input.ReadValueS32();
                uint size = input.ReadValueU32();

                retVar.ObjCount++;

                WEMFile tempWEM = new WEMFile();

                tempWEM.Num = retVar.ObjCount;
                tempWEM.Id = id;
                tempWEM.Offset = offset;
                tempWEM.Length = size;

                WEMFileList.Add(tempWEM);

                if (_ZeroWrite)
                {
                    if (zeroPos != offset)
                    {
                        int num = (int)retVar.ObjCount - 2;
                        long zero = offset - zeroPos;

                        _Zero.Add(num, zero);
                        //Console.WriteLine("> " + zero);
                    }
                    zeroPos = offset + size;
                }

                // Console.WriteLine("[" + tempWEM.Num + "]" + " Position:" + offset + " Size:" + size + " ID:" + tempWEM.Id);
            }
            retVar.DIDXFiles = WEMFileList;

            return retVar;
        }

        public HIRCSection ReadHIRCSection(Stream input)
        {
            HIRCSection retVar = new HIRCSection();

            uint SectionSize = input.ReadValueU32();
            retVar.SectionSize = SectionSize;

            uint ObjCount = input.ReadValueU32();
            retVar.ObjCount = ObjCount;

            Console.WriteLine("ObjCount=" + ObjCount);

            for (int i = 0; i < ObjCount; i++)
            {
                Console.WriteLine("");

                var HIRCObject = new HIRC.Object();

                // single byte identifying type of object
                HIRCObject._ObjectType = (HIRC.Object.ObjectType)input.ReadValueU8();

                // length of object section (= 4-byte id field and additional bytes)
                HIRCObject.Length = input.ReadValueU32();

                // id of this object
                HIRCObject.Id = input.ReadValueU32();

                Console.WriteLine("Id={0} {1:G}", HIRCObject.Id, HIRCObject._ObjectType);

                // additional bytes, depending on type of object and section length
                var otherBytesPos = input.Position;
                HIRCObject.OtherBytes = input.ReadBytes(HIRCObject.Length - 4); // -4 becase of the Id

                var tempPos = input.Position;
                switch (HIRCObject._ObjectType)
                {
                case HIRC.Object.ObjectType.Sound:
                    HIRCObject._SoundObject = new HIRC.Object.SoundObject();
                    input.Seek(otherBytesPos, SeekOrigin.Begin);

                    input.ReadBytes(5); // 4 unknown + 1 SoundSource
                    HIRCObject._SoundObject.SoundFileID = input.ReadValueU32();

                    Console.WriteLine("WemID={0}", HIRCObject._SoundObject.SoundFileID);

                    input.Seek(tempPos, SeekOrigin.Begin);
                    break;

                case HIRC.Object.ObjectType.EventAction:
                    HIRCObject._EventActionData = new HIRC.Object.EventActionData();
                    input.Seek(otherBytesPos, SeekOrigin.Begin);

                    HIRCObject._EventActionData.EventActionScope = input.ReadValueU8();
                    HIRCObject._EventActionData.EventActionType = input.ReadValueU8();
                    // Wwise_IDs
                    HIRCObject._EventActionData.SoundObjectID = input.ReadValueU32();

                    Console.WriteLine("Scope={0} Type={1} WwiseID={2}",
                        HIRCObject._EventActionData.EventActionScope,
                        HIRCObject._EventActionData.EventActionType,
                        HIRCObject._EventActionData.SoundObjectID);

                    input.Seek(tempPos, SeekOrigin.Begin);
                    break;

                case HIRC.Object.ObjectType.Event:
                    HIRCObject._EventData = new HIRC.Object.EventData();
                    input.Seek(otherBytesPos, SeekOrigin.Begin);

                    HIRCObject._EventData.ActionCount = input.ReadValueU32();
                    HIRCObject._EventData.Actions = new uint[HIRCObject._EventData.ActionCount];
                    for (int action = 0; action < HIRCObject._EventData.ActionCount; action++)
                    {
                        HIRCObject._EventData.Actions[action] = input.ReadValueU32();
                        Console.WriteLine("Actions={0}", HIRCObject._EventData.Actions[action]);
                    }

                    input.Seek(tempPos, SeekOrigin.Begin);
                    break;

                    //default:
                    //    Debugger.Break();
                    //    break;
                }

                retVar.HIRCObject.Add(HIRCObject);
            }

            Console.WriteLine("");

            return retVar;
        }

        private void BuildEntryTree()
        {
            _EntryListView.BeginUpdate();
            _EntryListView.Items.Clear();
            if (_DIDXSection == null)
            {
                _EntryListView.EndUpdate();
                return;
            }
            foreach (WEMFile wem in _DIDXSection.DIDXFiles)
            {
                string[] items = new string[3];
                items[0] = wem.Num.ToString();
                items[1] = wem.Id.ToString("X8");
                //items[2] = wem.Length.ToString("N0");
                items[2] = Helpers.BytesToString(wem.Length);
                ListViewItem listItem = new ListViewItem(items);
                listItem.Tag = wem;
                _EntryListView.Items.Add(listItem);
            }
            _EntryListView.EndUpdate();
        }

        private void OpenResourceEntry(ListViewItem item)
        {
            if (item == null)
            {
                return;
            }

            var wemFile = item.Tag as WEMFile;
            if (wemFile == null || wemFile == default(WEMFile))
            {
                return;
            }

            if (_LastPlaySound == wemFile.Id)
            {
                _AudioPlaybackPanel.Reset();
                return;
            }

            textLog.Clear();

            _AudioPlaybackPanel.FreeStream();

            _LastPlaySound = wemFile.Id;

            string soundFilePath = ConvertWemToWav(wemFile);

            _AudioPlaybackPanel.LoadFile(soundFilePath);
        }

        private string ConvertWemToWav(WEMFile wemFile, bool convertOgg = false, int msTimeout = 0)
        {
            if (_DIDXData == null || (wemFile.Offset + wemFile.Length) > _DIDXData.Length)
            {
                return null;
            }

            bool error = false;
            string returnPath = null;
            bool findImaAdpcm = File.Exists(_PathImaAdpcm);
            bool findWw2Ogg = File.Exists(_PathWw2Ogg);
            bool findRevorb = File.Exists(_PathRevorb);
            bool findOggDec = File.Exists(_PathOggDec);

        _DIDXData.Seek(wemFile.Offset, SeekOrigin.Begin);
            byte[] buffer = _DIDXData.ReadBytes(wemFile.Length);

            _DIDXData.Seek(wemFile.Offset, SeekOrigin.Begin);
            var header = WaveFile.ReadHeader(_DIDXData, false);

            Console.WriteLine("ID " + wemFile.Id.ToString() + " " + header.ToString());

            // Create Wem file
            using (var fs = new FileStream(_TempWemFile, FileMode.Create))
            {
                fs.Write(buffer, 0, buffer.Length);
            }

            // ADPCM 105,2
            if (header.AudioFormat == WavFormatId.WWISE_ADPCM || header.AudioFormat == WavFormatId.VOXWARE_BYTE_ALIGNED)
            {
                if (findImaAdpcm && ProcessStart(_PathImaAdpcm, string.Format("-d \"{0}\" \"{1}\"", _TempWemFile, _TempWavFile), msTimeout) == 0)
                {
                    returnPath = _TempWavFile;
                    convertOgg = true;
                }
            }
            // XMA 357,358
            // else if ((ushort)header.AudioFormat == 357 || (ushort)header.AudioFormat == 358)
            // {
            //     System.Diagnostics.Debugger.Break();
            // }
            // PCM
            else if (header.AudioFormat == WavFormatId.WWISE_PCM)
            {
                try
                {
                    WaveFile.Create(buffer, _TempWavFile, header);
                    returnPath = _TempWavFile;
                    convertOgg = true;
                }
                catch
                {
                    returnPath = null;
                }
            }
            // Vorbis
            else if (header.AudioFormat == WavFormatId.WWISE_VORBIS)
            {
                string args = string.Format("{0} --pcb \"{1}\"", string.Format("\"{0}\" -o \"{1}\"", _TempWemFile, _TempOggFile), Path.Combine(Program.BinaryPath, "packed_codebooks_aoTuV_603.bin"));

                // Ww2Ogg (ww2ogg имяФайла.ogg (для стереофайлов с опцией --full-setup) )
                if (findWw2Ogg && ProcessStart(_PathWw2Ogg, args, msTimeout) != 0)
                {
                    error = true;
                    Console.WriteLine("error ww2ogg");
                }
                else if (findRevorb && ProcessStart(_PathRevorb, string.Format("\"{0}\"", _TempOggFile), msTimeout) != 0)
                {
                    Console.WriteLine("error revorb");
                }

                if (!error)
                {
                    if (convertOgg)
                    {
                        if (findOggDec && ProcessStart(_PathOggDec, string.Format("\"{0}\"", _TempOggFile), msTimeout) != 0)
                        {
                            Console.WriteLine("error oggdec");
                        }
                        else
                        {
                            returnPath = _TempWavFile;
                        }
                    }
                    else
                    {
                        returnPath = _TempOggFile;
                    }
                }
            }
            // Wav File ?
            else
            {
                using (var output = File.Create(_TempWavFile))
                {
                    output.Write(buffer, 0, buffer.Length);
                }
            }

            buffer = null;
            header = null;

            try
            {
                if (File.Exists(_TempWemFile))
                {
                    File.Delete(_TempWemFile);
                }
                if (convertOgg)
                {
                    if (File.Exists(_TempOggFile))
                    {
                        File.Delete(_TempOggFile);
                    }
                }
                else
                {
                    if (File.Exists(_TempWavFile))
                    {
                        File.Delete(_TempWavFile);
                    }
                }
            }
            catch { }

            return returnPath;
        }

        private int ProcessStart(string fileName, string arg, int msTimeout = 0)
        {
            Process process = Process.Start(new ProcessStartInfo(fileName, arg)
            {
                UseShellExecute = false,
                CreateNoWindow = true
            });
            int exitCode = 0;
            try
            {
                process.WaitForExit();
                exitCode = process.ExitCode;
            }
            finally
            {
                process.Dispose();
            }

            if (msTimeout > 0)
            {
                System.Threading.Thread.Sleep(msTimeout);
            }

            return exitCode;
        }

        private bool RebuildDIDXData()
        {
            if (_DIDXSection == null || _DIDXData == null) return false;

            List<byte[]> wemFiles = new List<byte[]>();

            foreach (WEMFile wem in _DIDXSection.DIDXFiles)
            {
                byte[] buffer = new byte[wem.Length];
                _DIDXData.Seek(wem.Offset, SeekOrigin.Begin);
                _DIDXData.Read(buffer, 0, buffer.Length);
                wemFiles.Add(buffer);
            }

            _DIDXData.Flush();
            _DIDXData.Close();
            _DIDXData.Dispose();
            _DIDXData = new FileStream(_PathCache, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);

            for (int i = 0; i < _DIDXSection.DIDXFiles.Count; i++)
            {
                byte[] buffer = wemFiles[i];
                _DIDXSection.DIDXFiles[i].Offset = _DIDXData.Position;
                _DIDXSection.DIDXFiles[i].Length = (uint)buffer.Length;
                _DIDXData.Write(buffer, 0, buffer.Length);

                if (_ZeroWrite && _Zero.ContainsKey(i))
                {
                    _DIDXData.Position += _Zero[i];
                }
            }
            _DIDXData.Flush();
            wemFiles.Clear();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            return true;
        }

        private void OnSave(object sender, EventArgs e)
        {
            byte[] entryData = SaveResourceEntry();

            if (entryData == null || entryData.Length == 0) return;

#if __DEBUG
            using (var output = File.Create("NEW.BNK"))
            {
                output.WriteBytes(entryData);
            }
#endif
            _Resource.Data = entryData;

            BuildEntryTree();

            // Save
            using (var data = new MemoryStream())
            {
                _Resource.Serialize(_ResourceEntry.Version, data, _Endian);
                data.Flush();
                _ResourceEntry.Data = data.ToArray();
            }

            entryData = null;

            // .NET Framework 4.5.1
            // GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect();
        }

        private void OnSaveToFile(object sender, EventArgs e)
        {
            _SaveFileDialog.FileName = PathHelper.GetFileName(_Description, ".bnk");

            if (_SaveFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            File.WriteAllBytes(_SaveFileDialog.FileName, _Resource.Data);
        }

        private void OnLoadFromFile(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_LastWemPath))
            {
                _OpenFileDialog.InitialDirectory = Properties.Settings.Default.BnkViewer_InputWem;
            }
            else
            {
                _OpenFileDialog.InitialDirectory = Path.GetDirectoryName(_LastWemPath) + Path.DirectorySeparatorChar;
                Properties.Settings.Default.BnkViewer_InputWem = _OpenFileDialog.InitialDirectory;
                Properties.Settings.Default.Save();
            }

            if (_OpenFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            _LastWemPath = _OpenFileDialog.FileName;

            _AudioPlaybackPanel.FreeStream();

            _Resource.Data = File.ReadAllBytes(_OpenFileDialog.FileName);

            // Save
            using (var data = new MemoryStream())
            {
                _Resource.Serialize(_ResourceEntry.Version, data, _Endian);
                data.Flush();
                _ResourceEntry.Data = data.ToArray();
            }

            LoadResourceEntry(_ResourceEntry, _Description, _Endian);
        }

        private void OnOpenContextMenu(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_EntryListView.Items.Count <= 0 || _EntryListView.SelectedItems.Count <= 0)
            {
                e.Cancel = true;
                return;
            }
        }

        private void OnContextMenuPlay(object sender, EventArgs e)
        {
            OpenResourceEntry(_EntryListView.SelectedItems[0]);
        }

        private void OnContextMenuReplaceWem(object sender, EventArgs e)
        {
            WEMFile wem = _EntryListView.SelectedItems[0].Tag as WEMFile;
            if (wem == null || wem == default(WEMFile))
            {
                return;
            }

            if (string.IsNullOrEmpty(_LastWemPath))
            {
                _OpenFileWem.InitialDirectory = Properties.Settings.Default.BnkViewer_InputWem;
            }
            else
            {
                _OpenFileWem.InitialDirectory = Path.GetDirectoryName(_LastWemPath) + Path.DirectorySeparatorChar;
                Properties.Settings.Default.BnkViewer_InputWem = _OpenFileWem.InitialDirectory;
                Properties.Settings.Default.Save();
            }

            if (_OpenFileWem.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            _LastWemPath = _OpenFileWem.FileName;


            byte[] buffer = File.ReadAllBytes(_OpenFileWem.FileName);

            _DIDXData.Seek(0, SeekOrigin.End);

            wem.Offset = _DIDXData.Position;
            wem.Length = (uint)buffer.Length;

            _DIDXData.Write(buffer, 0, buffer.Length);

            buffer = null;

            //_DIDXSection.DIDXFiles[wem.Num - 1].Position = 0;
            //_DIDXSection.DIDXFiles[wem.Num - 1].Size = 0;

            _AudioPlaybackPanel.FreeStream();
            _LastPlaySound = 0;
        }

        private void OnContextMenuExtractAll(object sender, EventArgs e)
        {
            if (_DIDXSection == null || _DIDXData == null)
            {
                return;
            }

            string dirExt = Program.ExtractPath(Program.FilePath);
            string dirType = Path.Combine(dirExt, "MemFile");
            string dirBnk = _Description;
            if (dirBnk.StartsWith("/") == true)
            {
                dirBnk = dirBnk.Substring(1);
            }
            dirBnk = dirBnk.Replace("/", "\\");
            dirBnk = Path.Combine(dirType, Path.GetDirectoryName(dirBnk), Path.GetFileNameWithoutExtension(dirBnk));

            if (!Directory.Exists(dirBnk))
            {
                Directory.CreateDirectory(dirBnk);
            }

            foreach (WEMFile wem in _DIDXSection.DIDXFiles)
            {
                if ((wem.Offset + wem.Length) <= _DIDXData.Length)
                {
                    byte[] buffer = new byte[wem.Length];
                    _DIDXData.Seek(wem.Offset, SeekOrigin.Begin);
                    _DIDXData.Read(buffer, 0, buffer.Length);
                    File.WriteAllBytes(Path.Combine(dirBnk, wem.Name), buffer);
                    buffer = null;
                }
            }

            Helpers.ShowInformation("Output: " + dirBnk + "\r\n\nCompleted.", "Extraction");
        }

        private void OnContextMenuConvertAll(object sender, EventArgs e)
        {
            if (_DIDXSection == null || _DIDXData == null)
            {
                return;
            }

            System.Threading.Thread thread = new System.Threading.Thread(() => ConvertAll(_DIDXSection));
            thread.Start();
        }

        private void ConvertAll(DIDXSection didxSection)
        {
            if (InvokeRequired == true)
            {
                Invoke((MethodInvoker)delegate
                {
                    UseWaitCursor = true;
                });
            }

            string dirExt = Program.ExtractPath(Program.FilePath);
            string dirType = Path.Combine(dirExt, "MemFile");
            string dirBnk = _Description;
            if (dirBnk.StartsWith("/") == true)
            {
                dirBnk = dirBnk.Substring(1);
            }
            dirBnk = dirBnk.Replace("/", "\\");
            dirBnk = Path.Combine(dirType, Path.GetDirectoryName(dirBnk), Path.GetFileNameWithoutExtension(dirBnk));

            if (!Directory.Exists(dirBnk))
            {
                Directory.CreateDirectory(dirBnk);
            }

            StringBuilder sb = new StringBuilder();
            foreach (WEMFile wem in didxSection.DIDXFiles)
            {
                string wemName = wem.Name;
                string sndFile = ConvertWemToWav(wem, true, 10);
                if (sndFile != null)
                {
                    string soundRename = Path.Combine(dirBnk, Path.ChangeExtension(wem.Name, Path.GetExtension(sndFile)));
                    try
                    {
                        if (File.Exists(soundRename))
                        {
                            File.Delete(soundRename);
                            System.Threading.Thread.Sleep(10);
                        }

                        File.Move(sndFile, Path.Combine(dirBnk, soundRename));
                    }
                    catch { }
                }

                sb.AppendFormat("{0} to {1} {2}\r\n",  wemName, Path.GetExtension(sndFile), sndFile != null ? "ok" : "error");
            }

            if (InvokeRequired == true)
            {
                Invoke((MethodInvoker)delegate
                {
                    UseWaitCursor = false;
                });
            }

            FlexibleMessageBox.Show("Output: " + dirBnk + "\r\n\n" + sb.ToString() + "\r\nCompleted.", "Conversion", MessageBoxButtons.OK);
        }

        private void OnEntryListView_DoubleClick(object sender, EventArgs e)
        {
            if (_EntryListView.Items.Count <= 0 || _EntryListView.SelectedItems.Count <= 0)
            {
                return;
            }

            OpenResourceEntry(_EntryListView.SelectedItems[0]);
        }

        private void OnEntryListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (_EntryListView.Items.Count <= 0 || _EntryListView.SelectedItems.Count <= 0)
            {
                return;
            }

            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space)
            {
                OpenResourceEntry(_EntryListView.SelectedItems[0]);
            }
        }

        private void OnEntryListView_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
            case Keys.Enter:
            case Keys.Space:
                e.IsInputKey = true;
                break;
            }
        }

    }
}