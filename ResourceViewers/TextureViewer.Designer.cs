namespace ResourceExplorer
{
    partial class TextureViewer
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
            System.Windows.Forms.ToolStripButton _SaveButton;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TextureViewer));
            System.Windows.Forms.ToolStripSeparator _ToolStripSeparator1;
            System.Windows.Forms.ToolStripButton _LoadFromFileButton;
            System.Windows.Forms.ToolStripSeparator _ToolStripSeparator2;
            ImageGlass.DefaultGifAnimator defaultGifAnimator1 = new ImageGlass.DefaultGifAnimator();
            this._SaveToFileButton = new System.Windows.Forms.ToolStripButton();
            this._ZoomButton = new System.Windows.Forms.ToolStripButton();
            this._ToolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this._GridButton = new System.Windows.Forms.ToolStripButton();
            this._SaveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this._OpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this._Status = new System.Windows.Forms.StatusStrip();
            this._StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this._PreviewImageBox = new ImageGlass.ImageBox();
            _ToolStrip = new System.Windows.Forms.ToolStrip();
            _SaveButton = new System.Windows.Forms.ToolStripButton();
            _ToolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            _LoadFromFileButton = new System.Windows.Forms.ToolStripButton();
            _ToolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            _ToolStrip.SuspendLayout();
            this._Status.SuspendLayout();
            this.SuspendLayout();
            // 
            // _ToolStrip
            // 
            _ToolStrip.BackColor = System.Drawing.Color.Transparent;
            _ToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            _SaveButton,
            _ToolStripSeparator1,
            _LoadFromFileButton,
            this._SaveToFileButton,
            _ToolStripSeparator2,
            this._ZoomButton,
            this._ToolStripSeparator3,
            this._GridButton});
            _ToolStrip.Location = new System.Drawing.Point(0, 0);
            _ToolStrip.Name = "_ToolStrip";
            _ToolStrip.Size = new System.Drawing.Size(619, 25);
            _ToolStrip.TabIndex = 5;
            _ToolStrip.Text = "toolStrip1";
            // 
            // _SaveButton
            // 
            _SaveButton.Image = ((System.Drawing.Image)(resources.GetObject("_SaveButton.Image")));
            _SaveButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            _SaveButton.Name = "_SaveButton";
            _SaveButton.Size = new System.Drawing.Size(51, 22);
            _SaveButton.Text = "Save";
            _SaveButton.Click += new System.EventHandler(this.OnSave);
            // 
            // _ToolStripSeparator1
            // 
            _ToolStripSeparator1.Name = "_ToolStripSeparator1";
            _ToolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // _LoadFromFileButton
            // 
            _LoadFromFileButton.Image = ((System.Drawing.Image)(resources.GetObject("_LoadFromFileButton.Image")));
            _LoadFromFileButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            _LoadFromFileButton.Name = "_LoadFromFileButton";
            _LoadFromFileButton.Size = new System.Drawing.Size(105, 22);
            _LoadFromFileButton.Text = "Load From File";
            _LoadFromFileButton.Click += new System.EventHandler(this.OnLoadFromFile);
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
            // _ToolStripSeparator2
            // 
            _ToolStripSeparator2.Name = "_ToolStripSeparator2";
            _ToolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // _ZoomButton
            // 
            this._ZoomButton.Checked = true;
            this._ZoomButton.CheckOnClick = true;
            this._ZoomButton.CheckState = System.Windows.Forms.CheckState.Checked;
            this._ZoomButton.Image = ((System.Drawing.Image)(resources.GetObject("_ZoomButton.Image")));
            this._ZoomButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._ZoomButton.Name = "_ZoomButton";
            this._ZoomButton.Size = new System.Drawing.Size(59, 22);
            this._ZoomButton.Text = "Zoom";
            this._ZoomButton.Click += new System.EventHandler(this.OnZoom);
            // 
            // _ToolStripSeparator3
            // 
            this._ToolStripSeparator3.Name = "_ToolStripSeparator3";
            this._ToolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // _GridButton
            // 
            this._GridButton.CheckOnClick = true;
            this._GridButton.Image = ((System.Drawing.Image)(resources.GetObject("_GridButton.Image")));
            this._GridButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._GridButton.Name = "_GridButton";
            this._GridButton.Size = new System.Drawing.Size(49, 22);
            this._GridButton.Text = "Grid";
            this._GridButton.Click += new System.EventHandler(this.OnGrid);
            // 
            // _SaveFileDialog
            // 
            this._SaveFileDialog.DefaultExt = "dds";
            this._SaveFileDialog.Filter = "DDS Textures (*.dds)|*.dds|All Files (*.*)|*.*";
            // 
            // _OpenFileDialog
            // 
            this._OpenFileDialog.DefaultExt = "dds";
            this._OpenFileDialog.Filter = "DDS Textures (*.dds)|*.dds|All Files (*.*)|*.*";
            // 
            // _Status
            // 
            this._Status.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._StatusLabel});
            this._Status.Location = new System.Drawing.Point(0, 345);
            this._Status.Name = "_Status";
            this._Status.Size = new System.Drawing.Size(619, 22);
            this._Status.SizingGrip = false;
            this._Status.TabIndex = 6;
            // 
            // _StatusLabel
            // 
            this._StatusLabel.Name = "_StatusLabel";
            this._StatusLabel.Size = new System.Drawing.Size(16, 17);
            this._StatusLabel.Text = "...";
            // 
            // _PreviewImageBox
            // 
            this._PreviewImageBox.Animator = defaultGifAnimator1;
            this._PreviewImageBox.BackColor = System.Drawing.Color.DimGray;
            this._PreviewImageBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._PreviewImageBox.GridDisplayMode = ImageGlass.ImageBoxGridDisplayMode.None;
            this._PreviewImageBox.Location = new System.Drawing.Point(0, 25);
            this._PreviewImageBox.Name = "_PreviewImageBox";
            this._PreviewImageBox.Size = new System.Drawing.Size(619, 320);
            this._PreviewImageBox.TabIndex = 7;
            this._PreviewImageBox.ZoomChanged += new System.EventHandler(this._PreviewImageBox_ZoomChanged);
            // 
            // TextureViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(619, 367);
            this.Controls.Add(this._PreviewImageBox);
            this.Controls.Add(this._Status);
            this.Controls.Add(_ToolStrip);
            this.Name = "TextureViewer";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Texture";
            _ToolStrip.ResumeLayout(false);
            _ToolStrip.PerformLayout();
            this._Status.ResumeLayout(false);
            this._Status.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStripButton _SaveToFileButton;
        private System.Windows.Forms.SaveFileDialog _SaveFileDialog;
        private System.Windows.Forms.OpenFileDialog _OpenFileDialog;
        private System.Windows.Forms.ToolStripButton _ZoomButton;
        private System.Windows.Forms.StatusStrip _Status;
        private ImageGlass.ImageBox _PreviewImageBox;
        private System.Windows.Forms.ToolStripStatusLabel _StatusLabel;
        private System.Windows.Forms.ToolStripSeparator _ToolStripSeparator3;
        private System.Windows.Forms.ToolStripButton _GridButton;
    }
}