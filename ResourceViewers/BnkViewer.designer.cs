namespace ResourceExplorer
{
    partial class BnkViewer
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
            Properties.Settings.Default.BnkViewer_Volume = _AudioPlaybackPanel.Volume;
            Properties.Settings.Default.BnkViewer_Loop = _AudioPlaybackPanel.Loop;

            if (_DIDXData != null)
            {
                _DIDXData.Close();
                _DIDXData.Dispose();
            }

            try
            {
                if (System.IO.File.Exists(_PathCache))
                {
                    System.IO.File.Delete(_PathCache);
                }
            }
            catch { }

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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ToolStrip _ToolStrip;
            System.Windows.Forms.ToolStripButton _SaveButton;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BnkViewer));
            System.Windows.Forms.ToolStripSeparator _ToolStripSeparator1;
            System.Windows.Forms.ToolStripButton _LoadFromFileButton;
            this._SaveToFileButton = new System.Windows.Forms.ToolStripButton();
            this._SaveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this._OpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this._ContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.playToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.replaceWemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.extractAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.convertAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._Container = new System.Windows.Forms.SplitContainer();
            this._EntryListView = new System.Windows.Forms.ListView();
            this.columnNum = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnFile = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.textLog = new System.Windows.Forms.TextBox();
            this._OpenFileWem = new System.Windows.Forms.OpenFileDialog();
            _ToolStrip = new System.Windows.Forms.ToolStrip();
            _SaveButton = new System.Windows.Forms.ToolStripButton();
            _ToolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            _LoadFromFileButton = new System.Windows.Forms.ToolStripButton();
            _ToolStrip.SuspendLayout();
            this._ContextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._Container)).BeginInit();
            this._Container.Panel1.SuspendLayout();
            this._Container.Panel2.SuspendLayout();
            this._Container.SuspendLayout();
            this.SuspendLayout();
            // 
            // _ToolStrip
            // 
            _ToolStrip.BackColor = System.Drawing.Color.Transparent;
            _ToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            _SaveButton,
            _ToolStripSeparator1,
            _LoadFromFileButton,
            this._SaveToFileButton});
            _ToolStrip.Location = new System.Drawing.Point(0, 0);
            _ToolStrip.Name = "_ToolStrip";
            _ToolStrip.Size = new System.Drawing.Size(600, 25);
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
            // _SaveFileDialog
            // 
            this._SaveFileDialog.DefaultExt = "bnk";
            this._SaveFileDialog.Filter = "Wwise SoundBank (*.bnk)|*.bnk|All Files (*.*)|*.*";
            // 
            // _OpenFileDialog
            // 
            this._OpenFileDialog.DefaultExt = "bnk";
            this._OpenFileDialog.Filter = "Wwise SoundBank (*.bnk)|*.bnk|All Files (*.*)|*.*";
            // 
            // _ContextMenu
            // 
            this._ContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.playToolStripMenuItem,
            this.toolStripSeparator1,
            this.replaceWemToolStripMenuItem,
            this.toolStripSeparator2,
            this.extractAllToolStripMenuItem,
            this.convertAllToolStripMenuItem});
            this._ContextMenu.Name = "_ContextMenu";
            this._ContextMenu.Size = new System.Drawing.Size(159, 104);
            this._ContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.OnOpenContextMenu);
            // 
            // playToolStripMenuItem
            // 
            this.playToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("playToolStripMenuItem.Image")));
            this.playToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.White;
            this.playToolStripMenuItem.Name = "playToolStripMenuItem";
            this.playToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.playToolStripMenuItem.Text = "Play";
            this.playToolStripMenuItem.Click += new System.EventHandler(this.OnContextMenuPlay);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(155, 6);
            // 
            // replaceWemToolStripMenuItem
            // 
            this.replaceWemToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("replaceWemToolStripMenuItem.Image")));
            this.replaceWemToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.White;
            this.replaceWemToolStripMenuItem.Name = "replaceWemToolStripMenuItem";
            this.replaceWemToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
            this.replaceWemToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.replaceWemToolStripMenuItem.Text = "Replace";
            this.replaceWemToolStripMenuItem.Click += new System.EventHandler(this.OnContextMenuReplaceWem);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(155, 6);
            // 
            // extractAllToolStripMenuItem
            // 
            this.extractAllToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("extractAllToolStripMenuItem.Image")));
            this.extractAllToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.White;
            this.extractAllToolStripMenuItem.Name = "extractAllToolStripMenuItem";
            this.extractAllToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.extractAllToolStripMenuItem.Text = "Extract All";
            this.extractAllToolStripMenuItem.Click += new System.EventHandler(this.OnContextMenuExtractAll);
            // 
            // convertAllToolStripMenuItem
            // 
            this.convertAllToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("convertAllToolStripMenuItem.Image")));
            this.convertAllToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.White;
            this.convertAllToolStripMenuItem.Name = "convertAllToolStripMenuItem";
            this.convertAllToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.convertAllToolStripMenuItem.Text = "Convert All";
            this.convertAllToolStripMenuItem.Click += new System.EventHandler(this.OnContextMenuConvertAll);
            // 
            // _Container
            // 
            this._Container.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._Container.Dock = System.Windows.Forms.DockStyle.Fill;
            this._Container.Location = new System.Drawing.Point(0, 25);
            this._Container.Name = "_Container";
            // 
            // _Container.Panel1
            // 
            this._Container.Panel1.Controls.Add(this._EntryListView);
            // 
            // _Container.Panel2
            // 
            this._Container.Panel2.Controls.Add(this.textLog);
            this._Container.Size = new System.Drawing.Size(600, 300);
            this._Container.SplitterDistance = 217;
            this._Container.TabIndex = 8;
            // 
            // _EntryListView
            // 
            this._EntryListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._EntryListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnNum,
            this.columnFile,
            this.columnSize});
            this._EntryListView.ContextMenuStrip = this._ContextMenu;
            this._EntryListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._EntryListView.FullRowSelect = true;
            this._EntryListView.GridLines = true;
            this._EntryListView.HideSelection = false;
            this._EntryListView.Location = new System.Drawing.Point(0, 0);
            this._EntryListView.MultiSelect = false;
            this._EntryListView.Name = "_EntryListView";
            this._EntryListView.Size = new System.Drawing.Size(215, 298);
            this._EntryListView.TabIndex = 8;
            this._EntryListView.UseCompatibleStateImageBehavior = false;
            this._EntryListView.View = System.Windows.Forms.View.Details;
            this._EntryListView.DoubleClick += new System.EventHandler(this.OnEntryListView_DoubleClick);
            this._EntryListView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnEntryListView_KeyDown);
            this._EntryListView.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.OnEntryListView_PreviewKeyDown);
            // 
            // columnNum
            // 
            this.columnNum.Text = "№";
            this.columnNum.Width = 39;
            // 
            // columnFile
            // 
            this.columnFile.Text = "File";
            this.columnFile.Width = 90;
            // 
            // columnSize
            // 
            this.columnSize.Text = "Size";
            this.columnSize.Width = 79;
            // 
            // textLog
            // 
            this.textLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textLog.BackColor = System.Drawing.SystemColors.Control;
            this.textLog.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textLog.Location = new System.Drawing.Point(3, 201);
            this.textLog.Multiline = true;
            this.textLog.Name = "textLog";
            this.textLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textLog.Size = new System.Drawing.Size(372, 95);
            this.textLog.TabIndex = 0;
            this.textLog.WordWrap = false;
            // 
            // _OpenFileWem
            // 
            this._OpenFileWem.DefaultExt = "wem";
            this._OpenFileWem.Filter = "Wwise Encoded Media (*.wem)|*.wem|All Files (*.*)|*.*";
            // 
            // BnkViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 325);
            this.Controls.Add(this._Container);
            this.Controls.Add(_ToolStrip);
            this.Name = "BnkViewer";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "BnkEditor";
            _ToolStrip.ResumeLayout(false);
            _ToolStrip.PerformLayout();
            this._ContextMenu.ResumeLayout(false);
            this._Container.Panel1.ResumeLayout(false);
            this._Container.Panel2.ResumeLayout(false);
            this._Container.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._Container)).EndInit();
            this._Container.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStripButton _SaveToFileButton;
        private System.Windows.Forms.SaveFileDialog _SaveFileDialog;
        private System.Windows.Forms.OpenFileDialog _OpenFileDialog;
        private System.Windows.Forms.SplitContainer _Container;
        private System.Windows.Forms.TextBox textLog;
        private System.Windows.Forms.ContextMenuStrip _ContextMenu;
        private System.Windows.Forms.ToolStripMenuItem replaceWemToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog _OpenFileWem;
        private System.Windows.Forms.ToolStripMenuItem playToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem extractAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem convertAllToolStripMenuItem;
        private System.Windows.Forms.ListView _EntryListView;
        private System.Windows.Forms.ColumnHeader columnFile;
        private System.Windows.Forms.ColumnHeader columnSize;
        private System.Windows.Forms.ColumnHeader columnNum;
    }
}