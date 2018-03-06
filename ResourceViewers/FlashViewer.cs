using System;
using System.IO;
using System.Windows.Forms;
using Be.Windows.Forms;
using Gibbed.IO;
using ResourceEntry = FileFormats.Archive.ResourceEntry;

namespace ResourceExplorer
{
    public partial class FlashViewer : Form, IResourceViewer
    {
        private ResourceEntry _ResourceEntry;
        private Endian _Endian;
        private string _Description;
        private ResourceFormats.FlashResource _Resource;

        public FlashViewer()
        {
            InitializeComponent();
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

        public void LoadResourceEntry(ResourceEntry resourceEntry, string description, Endian endian)
        {
            var resource = new ResourceFormats.FlashResource();
            using (var data = new MemoryStream(resourceEntry.Data, false))
            {
                resource.Deserialize(resourceEntry.Version, data, endian);
            }

            _ResourceEntry = resourceEntry;
            _Endian = endian;
            _Description = description;
            _Resource = resource;

            _HexBox.ByteProvider = new DynamicByteProvider(resource.Data);
        }

        private void OnSave(object sender, EventArgs e)
        {
            using (var data = new MemoryStream())
            {
                _Resource.Serialize(_ResourceEntry.Version, data, _Endian);
                data.Flush();
                _ResourceEntry.Data = data.ToArray();
            }

            // Update
            var resource = new ResourceFormats.FlashResource();
            using (var data = new MemoryStream(_ResourceEntry.Data, false))
            {
                resource.Deserialize(_ResourceEntry.Version, data, _Endian);
            }
            _Resource = resource;
            _HexBox.ByteProvider = new DynamicByteProvider(resource.Data);
        }

        private void OnSaveToFile(object sender, EventArgs e)
        {
            _SaveFileDialog.FileName = PathHelper.GetFileName(_Description, ".gfx");
            if (_SaveFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            File.WriteAllBytes(_SaveFileDialog.FileName, _Resource.Data);
        }

        private void OnLoadFromFile(object sender, EventArgs e)
        {
            if (_OpenFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            _Resource.Data = File.ReadAllBytes(_OpenFileDialog.FileName);
            _HexBox.ByteProvider = new DynamicByteProvider(_Resource.Data);
        }
    }
}
