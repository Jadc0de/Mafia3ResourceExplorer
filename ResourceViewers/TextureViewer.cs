using System;
using System.IO;
using System.Windows.Forms;
using Gibbed.IO;
using System.Drawing;
using FileFormats.Archive;

namespace ResourceExplorer
{
    public partial class TextureViewer : Form, IResourceViewer
    {
        private ResourceEntry _ResourceEntry;
        private Endian _Endian;
        private string _Description;
        private ResourceFormats.TextureResource _Resource;
        private Bitmap _Bitmap;

        public TextureViewer()
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
            var resource = new ResourceFormats.TextureResource();
            using (var data = new MemoryStream(resourceEntry.Data, false))
            {
                resource.Deserialize(resourceEntry.Version, data, endian);
            }

            _ResourceEntry = resourceEntry;
            _Endian = endian;
            _Description = description;
            _Resource = resource;

            LoadImage();

            _PreviewImageBox.Zoom = 100;
            _PreviewImageBox.AutoScrollPosition = new Point(0, 0);

            _GridButton.Checked = Properties.Settings.Default.TextureViewer_Grid;

            RefreshPictureBox();
        }

        private void OnSave(object sender, EventArgs e)
        {
            using (var data = new MemoryStream())
            {
                _Resource.Serialize(_ResourceEntry.Version, data, _Endian);
                data.Flush();
                _ResourceEntry.Data = data.ToArray();
            }

            System.Media.SystemSounds.Asterisk.Play();
        }

        private void LoadImage()
        {
            try
            {
                if (_Bitmap != null)
                {
                    _Bitmap.Dispose();
                }
                _Bitmap = DdsFile.DdsFile.Load(new MemoryStream(_Resource.Data, false));
            }
            catch
            {
                _Bitmap = null;
            }
        }
    
        private void RefreshPictureBox()
        {
            Extensions.TextureUtilities.Format format = Extensions.TextureUtilities.Format.UNKNOWN;

            try
            {
                format = Extensions.TextureUtilities.TextureInfo(_Resource.Data);
            }
            catch (IOException)
            { }

            _PreviewImageBox.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

            if (_Bitmap == null)
            {
                _PreviewImageBox.Image = null;
            }
            else
            {
                _PreviewImageBox.Image = _Bitmap;
                _PreviewImageBox.Refresh();

                _StatusLabel.Text = string.Format("{0} x {1} {2}", _Bitmap.Width, _Bitmap.Height, format.ToString("g"));

                ScaleImageToFit();
            }
        }

        private void ScaleImageToFit()
        {
            if (_PreviewImageBox.Image == null)
            {
                return;
            }

            if (_ZoomButton.Checked == true)
            {
                _PreviewImageBox.SizeMode = ImageGlass.ImageBoxSizeMode.Fit;
            }
            else
            {
                _PreviewImageBox.SizeMode = ImageGlass.ImageBoxSizeMode.Normal;
            }

            if (_GridButton.Checked == true)
            {
                _PreviewImageBox.GridDisplayMode = ImageGlass.ImageBoxGridDisplayMode.Client;

                Properties.Settings.Default.TextureViewer_Grid = true;
                Properties.Settings.Default.Save();
            }
            else
            {
                _PreviewImageBox.GridDisplayMode = ImageGlass.ImageBoxGridDisplayMode.None;

                Properties.Settings.Default.TextureViewer_Grid = false;
                Properties.Settings.Default.Save();
            }

            _PreviewImageBox.Refresh();
        }

        private void OnGrid(object sender, EventArgs e)
        {
            ScaleImageToFit();
        }

        private void OnZoom(object sender, EventArgs e)
        {
            //this.UpdatePreview();
            RefreshPictureBox();
        }

        private void OnToggleAlpha(object sender, EventArgs e)
        {
            RefreshPictureBox();
        }

        private void OnSaveToFile(object sender, EventArgs e)
        {
            _SaveFileDialog.FileName = PathHelper.GetFileName(_Description, ".dds");
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

            LoadImage();

            RefreshPictureBox();
        }

        private void _PreviewImageBox_ZoomChanged(object sender, EventArgs e)
        {
            if (_PreviewImageBox.Image == null)
            {
                return;
            }

            _ZoomButton.Text = "Zoom " + _PreviewImageBox.Zoom.ToString();
        }
    }
}
