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
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Gibbed.IO;
using ColumnType = ResourceFormats.TableData.ColumnType;
using ResourceEntry = FileFormats.Archive.ResourceEntry;

namespace ResourceExplorer
{
    public partial class TableViewer : Form, IResourceViewer
    {
        private Dictionary<uint, string> ColumnNames = new Dictionary<uint, string>();

        public TableViewer()
        {
            InitializeComponent();
            _HintLabel.Text = "";

            // TODO: move this big list to an external file
            // I am lazy :effort:
            AddColumnName("0", "1", "2", "3", "4", "5", "6");
            AddColumnName("Id", "ID");
            AddColumnName("Top");
            AddColumnName("Max");
            AddColumnName("flag");
            AddColumnName("guid");
            AddColumnName("HP");
            AddColumnName("CO");
            AddColumnName("Type");
            AddColumnName("Left");
            AddColumnName("name");
            AddColumnName("Name");
            AddColumnName("Flags");
            AddColumnName("Speed");
            AddColumnName("Weight");
            AddColumnName("model");
            AddColumnName("scale");
            AddColumnName("speed");
            AddColumnName("Descr");
            AddColumnName("Group");
            AddColumnName("Notes");
            AddColumnName("Price");
            AddColumnName("Range");
            AddColumnName("Right");
            AddColumnName("Sound");
            AddColumnName("Value");
            AddColumnName("Water");
            AddColumnName("Bottom");
            AddColumnName("Default");
            AddColumnName("Defence");
            AddColumnName("Description");
            AddColumnName("impact");
            AddColumnName("Material");
            AddColumnName("Possibility");
            AddColumnName("Reaction");
            AddColumnName("Visibility");
            AddColumnName("restitution");
            AddColumnName("FadeIn");
            AddColumnName("FadeOut");
            AddColumnName("fragtex");
            AddColumnName("Impact");
            AddColumnName("SlipId");
            AddColumnName("TexCols");
            AddColumnName("TexEnd");
            AddColumnName("TexRows");
            AddColumnName("TexStart");
            AddColumnName("TypeName");
            AddColumnName("BlockId");
            AddColumnName("CrashId");
            AddColumnName("MaterialId");
            AddColumnName("RideId");
            AddColumnName("SlideId");
            AddColumnName("MaterialName");
            AddColumnName("CrossType");
            AddColumnName("affector");
            AddColumnName("MaxDistance");
            AddColumnName("BlockingTime");
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

        private void AddColumnName(params string[] names)
        {
            if (names != null)
            {
                foreach (var name in names)
                {
                    var hash = FileFormats.Hashing.FNV32.Hash(name);
                    ColumnNames.Add(hash, name);
                }
            }
        }

        private string GetColumnName(uint hash)
        {
            if (ColumnNames.ContainsKey(hash) == true)
            {
                return ColumnNames[hash];
            }

            return hash.ToString("X8");
        }

        public void LoadResourceEntry(ResourceEntry resourceEntry, string description, Endian endian)
        {
            var tables = new ResourceFormats.TableResource();
            using (var data = new MemoryStream(resourceEntry.Data, false))
            {
                tables.Deserialize(resourceEntry.Version, data, endian);
            }

            _TableComboBox.Items.Clear();
            foreach (var table in tables.Tables)
            {
                _TableComboBox.Items.Add(table);
            }
            if (_TableComboBox.Items.Count > 0)
            {
                _TableComboBox.SelectedIndex = 0;
            }
        }

        private void UpdateDisplay()
        {
            var table = _TableComboBox.SelectedItem as ResourceFormats.TableData;

            _DataGrid.Columns.Clear();
            _DataGrid.Rows.Clear();

            if (table == null)
            {
                return;
            }

            //System.Data.DataTable tb;

            // build columns
            {
                foreach (var def in table.Columns)
                {
                    var column = new DataGridViewTextBoxColumn()
                    {
                        ValueType = ResourceFormats.TableData.GetValueTypeForColumnType(def.Type),
                        Name = def.NameHash.ToString("X8"),
                        HeaderText = GetColumnName(def.NameHash),
                        Tag = def,
                    };

                    switch (def.Type)
                    {
                        case ColumnType.String8:
                        case ColumnType.String16:
                        case ColumnType.String32:
                        case ColumnType.String64:
                        case ColumnType.Hash64AndString32:
                        {
                            break;
                        }

                        case ColumnType.Flags32:
                        case ColumnType.Hash64:
                        {
                            break;
                        }

                        default:
                        {
                            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            break;
                        }
                    }

                    _DataGrid.Columns.Add(column);
                }

                var row = new DataGridViewRow();
                row.CreateCells(_DataGrid, table.Columns.Select(c => (object)c.Type).ToArray());
                row.ReadOnly = true;
                row.Frozen = true;
                row.DefaultCellStyle.BackColor = SystemColors.ControlLight;
                row.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                row.DefaultCellStyle.Format = "";
                _DataGrid.Rows.Add(row);
            }

            foreach (var row in table.Rows)
            {
                //var cells = new DataGridViewCell[row.Values.Count];
                _DataGrid.Rows.Add(row.Values.ToArray());
            }
        }

        private void OnSelectTable(object sender, EventArgs e)
        {
            UpdateDisplay();
        }

        private void OnLoadFromFile(object sender, EventArgs e)
        {
        }

        private void OnSaveToFile(object sender, EventArgs e)
        {
        }

        private void OnCellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
        }

        private void OnCellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            var column = _DataGrid.Columns[e.ColumnIndex];
            var def = column.Tag as ResourceFormats.TableData.Column;
            if (def == null)
            {
                _DataGrid.Rows[e.RowIndex].ErrorText = "null column definition?";
                return;
            }

            //this.dataGrid[e.ColumnIndex, e.RowIndex].
        }

        private void OnCurrentCellChanged(object sender, EventArgs e)
        {
            /*
            if (this.dataGrid.CurrentCell == null)
            {
                this.hintLabel.Text = "";
                return;
            }

            var column = this.dataGrid.Columns[this.dataGrid.CurrentCell.ColumnIndex];
            if (column == null)
            {
                this.hintLabel.Text = "";
                return;
            }

            var def = column.Tag as FileFormats.ResourceTypes.TableData.Column;
            if (def == null)
            {
                this.hintLabel.Text = "";
                return;
            }
            */
        }

        private void OnCopyHashes(object sender, EventArgs e)
        {
            var hashes = new List<uint>();

            foreach (ResourceFormats.TableData table in _TableComboBox.Items)
            {
                foreach (var hash in table.Columns.Select(c => c.NameHash))
                {
                    if (hashes.Contains(hash) == false)
                    {
                        hashes.Add(hash);
                    }
                }
            }

            hashes.Sort();

            var text = new StringBuilder();
            foreach (var hash in hashes)
            {
                text.AppendLine(hash.ToString("X8"));
            }

            Clipboard.SetText(text.ToString());
        }

        private void OnDataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Context == DataGridViewDataErrorContexts.Parsing)
            {
                e.ThrowException = false;
                _DataGrid.Rows[e.RowIndex].ErrorText = e.Exception.Message;
            }
        }
    }
}
