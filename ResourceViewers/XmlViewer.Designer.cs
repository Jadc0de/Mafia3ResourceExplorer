namespace ResourceExplorer
{
    partial class XmlViewer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.ToolStrip _ToolStrip;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XmlViewer));
            System.Windows.Forms.ToolStripSeparator _ToolStripSeparator1;
            this._SaveButton = new System.Windows.Forms.ToolStripButton();
            this._LoadFromFileButton = new System.Windows.Forms.ToolStripButton();
            this._SaveToFileButton = new System.Windows.Forms.ToolStripButton();
            this._SaveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this._OpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this._Scintilla = new ScintillaNET.Scintilla();
            _ToolStrip = new System.Windows.Forms.ToolStrip();
            _ToolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            _ToolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // _ToolStrip
            // 
            _ToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._SaveButton,
            _ToolStripSeparator1,
            this._LoadFromFileButton,
            this._SaveToFileButton});
            _ToolStrip.Location = new System.Drawing.Point(0, 0);
            _ToolStrip.Name = "_ToolStrip";
            _ToolStrip.Size = new System.Drawing.Size(531, 25);
            _ToolStrip.TabIndex = 1;
            _ToolStrip.Text = "toolStrip1";
            // 
            // _SaveButton
            // 
            this._SaveButton.Image = ((System.Drawing.Image)(resources.GetObject("_SaveButton.Image")));
            this._SaveButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._SaveButton.Name = "_SaveButton";
            this._SaveButton.Size = new System.Drawing.Size(51, 22);
            this._SaveButton.Text = "Save";
            this._SaveButton.Click += new System.EventHandler(this.OnSave);
            // 
            // _ToolStripSeparator1
            // 
            _ToolStripSeparator1.Name = "_ToolStripSeparator1";
            _ToolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // _LoadFromFileButton
            // 
            this._LoadFromFileButton.Image = ((System.Drawing.Image)(resources.GetObject("_LoadFromFileButton.Image")));
            this._LoadFromFileButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._LoadFromFileButton.Name = "_LoadFromFileButton";
            this._LoadFromFileButton.Size = new System.Drawing.Size(105, 22);
            this._LoadFromFileButton.Text = "Load From File";
            this._LoadFromFileButton.Click += new System.EventHandler(this.OnLoadFromFile);
            // 
            // _SaveToFileButton
            // 
            this._SaveToFileButton.Image = ((System.Drawing.Image)(resources.GetObject("_SaveToFileButton.Image")));
            this._SaveToFileButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._SaveToFileButton.Name = "_SaveToFileButton";
            this._SaveToFileButton.Size = new System.Drawing.Size(89, 22);
            this._SaveToFileButton.Text = "Save To File";
            this._SaveToFileButton.Click += new System.EventHandler(this.OnSaveToFile);
            // 
            // _SaveFileDialog
            // 
            this._SaveFileDialog.DefaultExt = "xml";
            this._SaveFileDialog.Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*";
            // 
            // _OpenFileDialog
            // 
            this._OpenFileDialog.DefaultExt = "xml";
            this._OpenFileDialog.Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*";
            // 
            // _Scintilla
            // 
            this._Scintilla.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._Scintilla.Location = new System.Drawing.Point(0, 28);
            this._Scintilla.Name = "_Scintilla";
            this._Scintilla.Size = new System.Drawing.Size(531, 319);
            this._Scintilla.TabIndex = 2;
            this._Scintilla.Text = "scintilla1";
            // 
            // XmlViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(531, 347);
            this.Controls.Add(this._Scintilla);
            this.Controls.Add(_ToolStrip);
            this.KeyPreview = true;
            this.Name = "XmlViewer";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "XML";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form_KeyDown);
            _ToolStrip.ResumeLayout(false);
            _ToolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.SaveFileDialog _SaveFileDialog;
        private System.Windows.Forms.ToolStripButton _SaveButton;
        private System.Windows.Forms.OpenFileDialog _OpenFileDialog;
        private ScintillaNET.Scintilla _Scintilla;
        private System.Windows.Forms.ToolStripButton _LoadFromFileButton;
        private System.Windows.Forms.ToolStripButton _SaveToFileButton;
    }
}