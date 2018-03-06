using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using ScintillaNET;
using Gibbed.IO;
using ResourceEntry = FileFormats.Archive.ResourceEntry;

namespace ResourceExplorer
{
    public partial class XmlViewer : Form, IResourceViewer
    {
        private ResourceEntry _ResourceEntry;
        private Endian _Endian;
        private string _Description;
        private ResourceFormats.XmlResource _Resource;
        private int maxLineNumberCharLength;

#if DEBUG
        private ulong hash_input = 0;
#endif

        public XmlViewer()
        {
            InitializeComponent();

            InitScintilla(_Scintilla);
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }

        public static Color IntToColor(int rgb)
        {
            return Color.FromArgb(255, (byte)(rgb >> 16), (byte)(rgb >> 8), (byte)rgb);
        }

        public void InitScintilla(Scintilla scintilla)
        {
            // INITIAL VIEW CONFIG
            scintilla.WrapMode = WrapMode.None;
            scintilla.IndentationGuides = IndentView.LookBoth;

            scintilla.StyleResetDefault();
            scintilla.Styles[Style.Default].Font = "Courier New";
            scintilla.Styles[Style.Default].SizeF = 9.75f;
            scintilla.StyleClearAll();

            //scintilla.Margins[0].Type = MarginType.Number;
            scintilla.Margins[0].Width = 20; // Show line numbers.

            scintilla.Lexer = Lexer.Xml;

            // DEFAULT
            scintilla.Styles[0].ForeColor = Color.FromArgb(0x000000);
            scintilla.Styles[0].BackColor = Color.FromArgb(0xFFFFFF);
            scintilla.Styles[0].Bold = true;
            // TAG
            scintilla.Styles[1].ForeColor = Color.FromArgb(0x0000FF);
            scintilla.Styles[1].BackColor = Color.FromArgb(0xFFFFFF);
            // TAGUNKNOWN
            scintilla.Styles[2].ForeColor = Color.FromArgb(0x0000FF);
            scintilla.Styles[2].BackColor = Color.FromArgb(0xFFFFFF);
            // ATTRIBUTE
            scintilla.Styles[3].ForeColor = Color.FromArgb(0xFF0000);
            scintilla.Styles[3].BackColor = Color.FromArgb(0xFFFFFF);
            // ATTRIBUTEUNKNOWN
            scintilla.Styles[4].ForeColor = Color.FromArgb(0xFF0000);
            scintilla.Styles[4].BackColor = Color.FromArgb(0xFFFFFF);
            // NUMBER
            scintilla.Styles[5].ForeColor = Color.FromArgb(0xFF0000);
            scintilla.Styles[5].BackColor = Color.FromArgb(0xFFFFFF);
            // DOUBLESTRING
            scintilla.Styles[6].ForeColor = Color.FromArgb(0x8000FF);
            scintilla.Styles[6].BackColor = Color.FromArgb(0xFFFFFF);
            //scintilla.Styles[6].Bold = true;
            // SINGLESTRING
            scintilla.Styles[7].ForeColor = Color.FromArgb(0x8000FF);
            scintilla.Styles[7].BackColor = Color.FromArgb(0xFFFFFF);
            //scintilla.Styles[7].Bold = true;
            // COMMENT
            scintilla.Styles[9].ForeColor = Color.FromArgb(0x008000);
            scintilla.Styles[9].BackColor = Color.FromArgb(0xFFFFFF);
            // ENTITY
            scintilla.Styles[10].ForeColor = Color.FromArgb(0x000000);
            scintilla.Styles[10].BackColor = Color.FromArgb(0xFEFDE0);
            //scintilla.Styles[10].Italic = true;
            // TAGEND
            scintilla.Styles[11].ForeColor = Color.FromArgb(0x0000FF);
            scintilla.Styles[11].BackColor = Color.FromArgb(0xFFFFFF);
            // XMLSTART
            scintilla.Styles[12].ForeColor = Color.FromArgb(0xFF0000);
            scintilla.Styles[12].BackColor = Color.FromArgb(0xFFFF00);
            // XMLEND
            scintilla.Styles[13].ForeColor = Color.FromArgb(0xFF0000);
            scintilla.Styles[13].BackColor = Color.FromArgb(0xFFFF00);
            // SGMLDEFAULT
            scintilla.Styles[21].ForeColor = Color.FromArgb(0x000000);
            scintilla.Styles[21].BackColor = Color.FromArgb(0xA6CAF0);
            // CDATA
            scintilla.Styles[17].ForeColor = Color.FromArgb(0xFF8000);
            scintilla.Styles[17].BackColor = Color.FromArgb(0xFFFFFF);

            // Enable folding
            scintilla.SetProperty("fold", "1");
            scintilla.SetProperty("fold.compact", "1");
            scintilla.SetProperty("fold.html", "1");

            // Configure a margin to display folding symbols
            scintilla.Margins[2].Type = MarginType.Symbol;
            scintilla.Margins[2].Mask = Marker.MaskFolders;
            scintilla.Margins[2].Sensitive = true;
            scintilla.Margins[2].Width = 20;

            // Set colors for all folding markers
            /*for (int i = 25; i <= 31; i++)
            {
                scintilla.Markers[i].SetForeColor(SystemColors.ControlLightLight);
                scintilla.Markers[i].SetBackColor(SystemColors.ControlDark);
            }*/

            // Reset folder markers
            for (int i = Marker.FolderEnd; i <= Marker.FolderOpen; i++)
            {
                scintilla.Markers[i].SetForeColor(SystemColors.ControlLightLight);
                scintilla.Markers[i].SetBackColor(SystemColors.ControlDark);
            }

            // Style the folder markers
            scintilla.Markers[Marker.Folder].Symbol = MarkerSymbol.BoxPlus;
            scintilla.Markers[Marker.Folder].SetBackColor(SystemColors.ControlText);
            scintilla.Markers[Marker.FolderOpen].Symbol = MarkerSymbol.BoxMinus;
            scintilla.Markers[Marker.FolderEnd].Symbol = MarkerSymbol.BoxPlusConnected;
            scintilla.Markers[Marker.FolderEnd].SetBackColor(SystemColors.ControlText);
            scintilla.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
            scintilla.Markers[Marker.FolderOpenMid].Symbol = MarkerSymbol.BoxMinusConnected;
            scintilla.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
            scintilla.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

            // Enable automatic folding
            scintilla.AutomaticFold = AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change;

            scintilla.ClearCmdKey(Keys.Control | Keys.F);
            scintilla.ClearCmdKey(Keys.Control | Keys.G);
            scintilla.ClearCmdKey(Keys.Control | Keys.S);
            scintilla.ClearCmdKey(Keys.Control | Keys.H);
            scintilla.ClearCmdKey(Keys.Control | Keys.B);

            scintilla.FoldAll(FoldAction.Expand);
            scintilla.FoldAll(FoldAction.Contract);

            scintilla.Text = string.Empty;
            scintilla.EmptyUndoBuffer();

            scintilla.TextChanged += (this.OnTextChanged);
        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            // Did the number of characters in the line number display change?
            // i.e. nnn VS nn, or nnnn VS nn, etc...
            var maxLineNumberCharLength = _Scintilla.Lines.Count.ToString().Length;
            if (maxLineNumberCharLength == this.maxLineNumberCharLength)
                return;

            // Calculate the width required to display the last line number
            // and include some padding for good measure.
            const int padding = 2;
            _Scintilla.Margins[0].Width = _Scintilla.TextWidth(Style.LineNumber, new string('9', maxLineNumberCharLength + 1)) + padding;
            this.maxLineNumberCharLength = maxLineNumberCharLength;
        }

        public void LoadResourceEntry(ResourceEntry resourceEntry, string description, Endian endian)
        {
            _SaveButton.Enabled = true;
            _LoadFromFileButton.Enabled = true;
            _SaveToFileButton.Enabled = true;

#if (DEBUG == FALSE)
            try
            {
#endif

#if DEBUG
                using (var file = File.Create("input_xml.bin"))
                {
                    file.WriteBytes(resourceEntry.Data);
                }
                hash_input = FileFormats.Hashing.FNV64.Hash(resourceEntry.Data, 0, resourceEntry.Data.Length);
#endif
                var resource = new ResourceFormats.XmlResource();
                using (var data = new MemoryStream(resourceEntry.Data, false))
                {
                    resource.Deserialize(resourceEntry.Version, data, endian);
                }

                _ResourceEntry = resourceEntry;
                _Endian = endian;
                _Description = description;
                _Resource = resource;

                _Scintilla.Text = resource.Content;
                _Scintilla.EmptyUndoBuffer();

#if (DEBUG == FALSE)
            }
            catch
            {
                _SaveButton.Enabled = false;
                _LoadFromFileButton.Enabled = false;
                _SaveToFileButton.Enabled = false;
                _Scintilla.Text = "";
            }
#endif
        }

        private void OnSave(object sender, EventArgs e)
        {
            _Resource.Content = _Scintilla.Text;
            using (var data = new MemoryStream())
            {
                try
                {
                    _Resource.Serialize(_ResourceEntry.Version, data, _Endian);
                    data.Flush();
                    _ResourceEntry.Data = data.ToArray();
                }
                catch (Exception ex)
                {
                    Helpers.ShowError(ex);
                }
            }

#if DEBUG
            using (var file = File.Create("output_xml.bin"))
            {
                file.WriteBytes(_ResourceEntry.Data);
            }

            ulong hash_output = FileFormats.Hashing.FNV64.Hash(_ResourceEntry.Data, 0, _ResourceEntry.Data.Length);
            if (hash_output != hash_input)
            {
                Helpers.ShowInformation("hash_output != hash_input");
            }
#endif
            System.Media.SystemSounds.Asterisk.Play();
        }

        private void OnSaveToFile(object sender, EventArgs e)
        {
            _SaveFileDialog.FileName = PathHelper.GetFileName(_Resource.Name, ".xml");
            if (_SaveFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            File.WriteAllText(_SaveFileDialog.FileName, _Scintilla.Text);
        }

        private void OnLoadFromFile(object sender, EventArgs e)
        {
            if (_OpenFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            _Scintilla.Text = File.ReadAllText(_OpenFileDialog.FileName);
            _Scintilla.EmptyUndoBuffer();
        }
    }
}
