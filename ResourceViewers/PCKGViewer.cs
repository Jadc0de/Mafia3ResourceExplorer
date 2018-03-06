using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Gibbed.IO;

namespace ResourceExplorer
{
    public partial class PCKGViewer : Form
    {
        public PCKGViewer()
        {
            InitializeComponent();

            resultLabel.Text = string.Empty;

            textBoxInput.Text = Properties.Settings.Default.PCKGViewer_InputPckg;
            textBoxOutput.Text = Properties.Settings.Default.PCKGViewer_OutputPckg;
        }

        public class PckgEntry
        {
            public ulong NameHash;
            public uint Offset;
            public uint Unk1;
            public uint Size;
            public uint Size2;
            public ushort Widgth;
            public ushort Heigth;
            public uint Unk2;
        }

        public class PckgFile
        {
            public List<PckgEntry> Entries = new List<PckgEntry>();

            public static int EstimateHeaderSize(int count)
            {
                return (
                    4 +  // sign
                    4 +  // ver
                    4 +  // count
                    4);  // flag
            }

            public void Serialize(Stream output)
            {
                output.WriteValueU32(0x50434B47, Endian.Big); // 50434B47
                output.WriteValueU32(2);
                output.WriteValueS32(this.Entries.Count);
                output.WriteValueU32(0xFFFFFFFF);

                var entries = this.Entries.OrderBy(e => e.Size).OrderBy(e => e.NameHash);

                foreach (var entry in entries)
                {
                    output.WriteValueU64(entry.NameHash);
                    output.WriteValueU32(entry.Offset);
                    output.WriteValueU32(entry.Unk1);
                    output.WriteValueU32(entry.Size);
                    output.WriteValueU32(entry.Size2);
                    output.WriteValueU16(entry.Heigth);
                    output.WriteValueU16(entry.Widgth);
                    output.WriteValueU32(entry.Unk2);
                }
            }

            public void Deserialize(Stream input)
            {
                uint sign = input.ReadValueU32(); // Always PCKG (0x50434B47)
                uint ver = input.ReadValueU32(); // 2
                uint count = input.ReadValueU32();
                uint flag = input.ReadValueU32(); // 0xFFFFFFFF

                this.Entries.Clear();
                for (uint i = 0; i < count; i++)
                {
                    var entry = new PckgEntry();
                    entry.NameHash = input.ReadValueU64();
                    entry.Offset = input.ReadValueU32();
                    entry.Unk1 = input.ReadValueU32();
                    entry.Size = input.ReadValueU32();
                    entry.Size2 = input.ReadValueU32();
                    entry.Heigth = input.ReadValueU16();
                    entry.Widgth = input.ReadValueU16();
                    entry.Unk2 = input.ReadValueU32();
                    this.Entries.Add(entry);
                }
            }
        }

        internal class PathEntry : PckgEntry
        {
            public string Path;
        }

        private void UnpackPckg(string inputPath, string outputPath)
        {
            var pckg = new PckgFile();
            using (var input = File.OpenRead(inputPath))
            {
                pckg.Deserialize(input);
            }

            Directory.CreateDirectory(outputPath);

            Stream data = File.OpenRead(inputPath);

            int percents = 0;
            int current = 0;
            int total = pckg.Entries.Count;

            foreach (var entry in pckg.Entries.OrderBy(e => e.Offset))
            {
                current++;

                long entryOffset = entry.Offset;

                // detect type
                string extension;
                {
                    var guess = new byte[64];
                    int read = 0;

                    if (entry.Size > 0)
                    {
                        data.Seek(entryOffset, SeekOrigin.Begin);
                        read = data.Read(guess, 0, (int)Math.Min(entry.Size, guess.Length));
                    }
                    extension = Helpers.ExtensionsDetect(guess, Math.Min(guess.Length, read));
                }

                // string name = "0x" + entry.NameHash.ToString("X16");
                string name = entry.NameHash.ToString("X16");
                name = Path.ChangeExtension(name, "." + extension);

                var entryPath = Path.Combine(outputPath, name);
                // Directory.CreateDirectory(Path.GetDirectoryName(entryPath));

                using (var output = File.Create(entryPath))
                {
                    if (entry.Size > 0)
                    {
                        data.Seek(entryOffset, SeekOrigin.Begin);
                        output.WriteFromStream(data, entry.Size);
                    }
                }

                // verbose
                if (InvokeRequired == true)
                {
                    Invoke((MethodInvoker)delegate
                    {
                        progressBar1.Value = percents;
                        resultLabel.Text = entryPath;
                    });
                }

                percents = ((current + 1) * 100) / total;
            }

            if (data != null)
            {
                data.Close();
            }

            if (InvokeRequired == true)
            {
                Invoke((MethodInvoker)delegate
                {
                    progressBar1.Value = 0;
                    resultLabel.Text = "Done";
                    buttonUnpack.Enabled = true;
                    buttonPack.Enabled = true;
                });
            }
        }

        private void PackPckg(string inputPath, string outputPath)
        {
            var entries = new List<PathEntry>();
            var pckg = new PckgFile();

            // Searching for archives...
            var inputPaths = new List<string>();
            inputPaths.AddRange(Directory.GetFiles(inputPath, "*", SearchOption.TopDirectoryOnly));

            foreach (var path in inputPaths)
            {
                string name = Path.GetFileNameWithoutExtension(path);
                ulong hash = 0;
                if (Helpers.IsHexString(name))
                {
                    hash = Helpers.HexLiteral2Unsigned(name);
                }
                else
                {
                    hash = FileFormats.Hashing.FNV64.Hash(name);
                }

                entries.Add(new PathEntry()
                {
                    NameHash = hash,
                    Offset = 0,
                    Unk1 = 0,
                    Size = 0,
                    Size2 = 0,
                    Widgth = 0,
                    Heigth = 0,
                    Unk2 = 0,
                    Path = path,
                });
            }


            uint headerSize = (uint)PckgFile.EstimateHeaderSize(entries.Count);
            long firstOffset = entries.Count * 32 + headerSize;
            long localOffset = firstOffset;

            Stream data = File.Create(outputPath);

            int percents = 0;
            int current = 0;
            int total = entries.Count;

            foreach (var entry in entries)
            {
                current++;

                // verbose
                if (InvokeRequired == true)
                {
                    Invoke((MethodInvoker)delegate
                    {
                        progressBar1.Value = percents;
                        resultLabel.Text = "Pack \"" + entry.Path + "\"";
                    });
                }

                using (var input = File.OpenRead(entry.Path))
                {
                    var length = (uint)input.Length;

                    data.Seek(localOffset, SeekOrigin.Begin);
                    data.WriteFromStream(input, length);

                    ushort height = 0;
                    ushort width = 0;
                    input.Seek(0L, SeekOrigin.Begin);
                    if (input.Length >= 16 && input.ReadValueU32() == 542327876)
                    {
                        input.Seek(0x0c, SeekOrigin.Begin);
                        height = input.ReadValueU16();
                        input.Seek(0x10, SeekOrigin.Begin);
                        width = input.ReadValueU16();
                    }

                    entry.Size = length;
                    entry.Size2 = length;
                    entry.Offset = (uint)localOffset;
                    entry.Widgth = width;
                    entry.Heigth = height;
                    pckg.Entries.Add(entry);

                    localOffset += length;
                }

                percents = ((current + 1) * 100) / total;
            }

            if (data != null)
            {
                data.Close();
            }

            using (var output = File.OpenWrite(outputPath))
            {
                pckg.Serialize(output);
            }

            if (InvokeRequired == true)
            {
                Invoke((MethodInvoker)delegate
                {
                    progressBar1.Value = 0;
                    resultLabel.Text = "Done \"" + outputPath + "\"";
                    buttonPack.Enabled = true;
                    buttonUnpack.Enabled = true;
                });
            }
        }

        private void buttonInput_Click(object sender, EventArgs e)
        {
            if (openPckgDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            textBoxInput.Text = openPckgDialog.FileName;
            string pathDir = Path.GetDirectoryName(textBoxInput.Text);

            // string pathUunpacked = "unpack_" + Path.GetFileNameWithoutExtension(textBoxInput.Text);
            string pathUunpacked = Path.GetFileNameWithoutExtension(textBoxInput.Text);

            string pathOutput = Path.Combine(pathDir, pathUunpacked);
            textBoxOutput.Text = pathOutput;
        }

        private void buttonOutput_Click(object sender, EventArgs e)
        {
            folderPckgDialog.Description = "Choose where to unpack...";

            if (!string.IsNullOrWhiteSpace(textBoxInput.Text))
            {
                folderPckgDialog.SelectedPath = Path.GetDirectoryName(textBoxInput.Text);
            }

            if (folderPckgDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            textBoxOutput.Text = folderPckgDialog.SelectedPath;
        }

        private void buttonUnpack_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxInput.Text) == true
                || string.IsNullOrWhiteSpace(textBoxOutput.Text) == true)
            {
                return;
            }

            string inputPath = Path.GetFullPath(textBoxInput.Text);
            string outputPath = Path.GetDirectoryName(textBoxOutput.Text.Replace("\\\\", "\\") + "\\");

            if (!File.Exists(inputPath))
            {
                Helpers.ShowError("The file " + inputPath + " does not exist");
                return;
            }

            var thread = new System.Threading.Thread(() => UnpackPckg(inputPath, outputPath));
            thread.Start();

            buttonPack.Enabled = false;
            buttonUnpack.Enabled = false;
        }

        private void buttonPack_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxInput.Text) == true
                || string.IsNullOrWhiteSpace(textBoxOutput.Text) == true)
            {
                return;
            }

            string inputPath = Path.GetDirectoryName(textBoxOutput.Text.Replace("\\\\", "\\") + "\\");

            if (!Directory.Exists(inputPath))
            {
                Helpers.ShowError("Choose a catalog for packaging. (Unpack Folder)");
                return;
            }

            string outputPath = Path.ChangeExtension(inputPath, "pckg");

            if (File.Exists(outputPath) && Helpers.ShowAsterisk("File '" + outputPath + "' exists and is about to be overwritten.\r\n\r\nDo you want to proceed?", "Output file exists") != DialogResult.Yes)
            {
                return;
            }

            /*if (File.Exists(outputPath))
            {
                if (Utility.Helper.ShowQuestion("The file exists, replace? \"" + outputPath + "\"") == DialogResult.No)
                {
                    return;
                }
            }*/

            System.Threading.Thread thread = new System.Threading.Thread(() => PackPckg(inputPath, outputPath));
            thread.Start();
            buttonPack.Enabled = false;
            buttonUnpack.Enabled = false;
        }

        private void Package_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.PCKGViewer_InputPckg = textBoxInput.Text;
            Properties.Settings.Default.PCKGViewer_OutputPckg = textBoxOutput.Text;
        }
    }
}
