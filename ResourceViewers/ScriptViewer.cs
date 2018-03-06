/* Copyright (c) 2017 Rick (rick 'at' gibbed 'dot' us)
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would
 *    be appreciated but is not required.
 * 
 * 2. Altered source versions must be plainly marked as such, and must not
 *    be misrepresented as being the original software.
 * 
 * 3. This notice may not be removed or altered from any source
 *    distribution.
 */

using System;
using System.IO;
using System.Windows.Forms;
using Be.Windows.Forms;
using Gibbed.IO;
using ResourceEntry = FileFormats.Archive.ResourceEntry;

namespace ResourceExplorer
{
    public partial class ScriptViewer : Form, IResourceViewer
    {
        private ResourceEntry _ResourceEntry;
        private Endian _Endian;
        private string _Description;
        private ResourceFormats.ScriptResource _Resource;

        public ScriptViewer()
        {
            InitializeComponent();

            // this._HexBox.ReadOnly = true;
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
            var resource = new ResourceFormats.ScriptResource();
            using (var data = new MemoryStream(resourceEntry.Data, false))
            {
                resource.Deserialize(resourceEntry.Version, data, endian);
            }

            _ResourceEntry = resourceEntry;
            _Endian = endian;
            _Description = description;
            _Resource = resource;

            // ReSharper disable LocalizableElement
            Text += ": " + resource.Path;
            // ReSharper restore LocalizableElement
            _ScriptComboBox.Items.Clear();
            foreach (var script in resource.Scripts)
            {
                _ScriptComboBox.Items.Add(script);
            }
            if (_ScriptComboBox.Items.Count > 0)
            {
                _ScriptComboBox.SelectedIndex = 0;
            }
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

        private void OnSelectScript(object sender, EventArgs e)
        {
            var script = _ScriptComboBox.SelectedItem as ResourceFormats.ScriptData;
            if (script == null)
            {
                return;
            }
            _HexBox.ByteProvider = new DynamicByteProvider(script.Data);
        }

        private void OnLoadFromFile(object sender, EventArgs e)
        {
            var script = _ScriptComboBox.SelectedItem as ResourceFormats.ScriptData;
            if (script == null)
            {
                return;
            }

            if (_OpenFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            script.Data = File.ReadAllBytes(_OpenFileDialog.FileName);
            _HexBox.ByteProvider = new DynamicByteProvider(script.Data);
        }

        private void OnSaveToFile(object sender, EventArgs e)
        {
            var script = _ScriptComboBox.SelectedItem as ResourceFormats.ScriptData;
            if (script == null)
            {
                return;
            }

            // string name = Path.GetFileName(_ScriptComboBox.SelectedItem.ToString());

            _SaveFileDialog.FileName = PathHelper.GetFileName(_ScriptComboBox.SelectedItem.ToString());

            if (_SaveFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            File.WriteAllBytes(_SaveFileDialog.FileName, script.Data);
        }
    }
}
