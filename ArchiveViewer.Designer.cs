namespace ResourceExplorer
{
    partial class ArchiveViewer
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ImageList _TypeImageList;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ArchiveViewer));
            System.Windows.Forms.ToolStrip _ToolStrip;
            System.Windows.Forms.ToolStripSeparator _ToolStripSeparator1;
            System.Windows.Forms.StatusStrip _StatusStrip;
            this._OpenArchiveButton = new System.Windows.Forms.ToolStripButton();
            this._SaveArchiveButton = new System.Windows.Forms.ToolStripButton();
            this._OptionsButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsCompression = new System.Windows.Forms.ToolStripMenuItem();
            this.tsNone = new System.Windows.Forms.ToolStripMenuItem();
            this.tsBest = new System.Windows.Forms.ToolStripMenuItem();
            this.tsOne = new System.Windows.Forms.ToolStripMenuItem();
            this.tsOpenSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.tsHexMode = new System.Windows.Forms.ToolStripMenuItem();
            this._StripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.tsPackage = new System.Windows.Forms.ToolStripMenuItem();
            this.tsFnvHash = new System.Windows.Forms.ToolStripMenuItem();
            this._HelpButton = new System.Windows.Forms.ToolStripButton();
            this._HintLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this._EntryTreeView = new System.Windows.Forms.TreeView();
            this._ContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._ContextMenuExtract = new System.Windows.Forms.ToolStripMenuItem();
            this._SaveArchiveDialog = new System.Windows.Forms.SaveFileDialog();
            this._OpenArchiveDialog = new System.Windows.Forms.OpenFileDialog();
            this._SplitContainer = new System.Windows.Forms.SplitContainer();
            _TypeImageList = new System.Windows.Forms.ImageList(this.components);
            _ToolStrip = new System.Windows.Forms.ToolStrip();
            _ToolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            _StatusStrip = new System.Windows.Forms.StatusStrip();
            _ToolStrip.SuspendLayout();
            _StatusStrip.SuspendLayout();
            this._ContextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._SplitContainer)).BeginInit();
            this._SplitContainer.Panel1.SuspendLayout();
            this._SplitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // _TypeImageList
            // 
            _TypeImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("_TypeImageList.ImageStream")));
            _TypeImageList.TransparentColor = System.Drawing.Color.Transparent;
            _TypeImageList.Images.SetKeyName(0, "Unknown");
            _TypeImageList.Images.SetKeyName(1, "SDS");
            _TypeImageList.Images.SetKeyName(2, "MemFile");
            _TypeImageList.Images.SetKeyName(3, "XML");
            _TypeImageList.Images.SetKeyName(4, "SystemObjectDatabase");
            _TypeImageList.Images.SetKeyName(5, "Script");
            _TypeImageList.Images.SetKeyName(6, "Texture");
            _TypeImageList.Images.SetKeyName(7, "Sound");
            _TypeImageList.Images.SetKeyName(8, "Cutscene");
            _TypeImageList.Images.SetKeyName(9, "Generic");
            _TypeImageList.Images.SetKeyName(10, "hkAnimation");
            // 
            // _ToolStrip
            // 
            _ToolStrip.BackColor = System.Drawing.Color.Transparent;
            _ToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._OpenArchiveButton,
            this._SaveArchiveButton,
            _ToolStripSeparator1,
            this._OptionsButton,
            this._HelpButton});
            _ToolStrip.Location = new System.Drawing.Point(0, 0);
            _ToolStrip.Name = "_ToolStrip";
            _ToolStrip.Size = new System.Drawing.Size(934, 25);
            _ToolStrip.TabIndex = 1;
            _ToolStrip.Text = "toolStrip1";
            // 
            // _OpenArchiveButton
            // 
            this._OpenArchiveButton.Image = ((System.Drawing.Image)(resources.GetObject("_OpenArchiveButton.Image")));
            this._OpenArchiveButton.ImageTransparentColor = System.Drawing.Color.White;
            this._OpenArchiveButton.Name = "_OpenArchiveButton";
            this._OpenArchiveButton.Size = new System.Drawing.Size(99, 22);
            this._OpenArchiveButton.Text = "Open Archive";
            this._OpenArchiveButton.Click += new System.EventHandler(this.OnOpen);
            // 
            // _SaveArchiveButton
            // 
            this._SaveArchiveButton.Image = ((System.Drawing.Image)(resources.GetObject("_SaveArchiveButton.Image")));
            this._SaveArchiveButton.ImageTransparentColor = System.Drawing.Color.White;
            this._SaveArchiveButton.Name = "_SaveArchiveButton";
            this._SaveArchiveButton.Size = new System.Drawing.Size(94, 22);
            this._SaveArchiveButton.Text = "Save Archive";
            this._SaveArchiveButton.Click += new System.EventHandler(this.OnSaveArchive);
            // 
            // _ToolStripSeparator1
            // 
            _ToolStripSeparator1.Name = "_ToolStripSeparator1";
            _ToolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // _OptionsButton
            // 
            this._OptionsButton.AutoToolTip = false;
            this._OptionsButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsCompression,
            this.tsOpenSelected,
            this.tsHexMode,
            this._StripSeparator,
            this.tsPackage,
            this.tsFnvHash});
            this._OptionsButton.Image = ((System.Drawing.Image)(resources.GetObject("_OptionsButton.Image")));
            this._OptionsButton.ImageTransparentColor = System.Drawing.Color.White;
            this._OptionsButton.Name = "_OptionsButton";
            this._OptionsButton.Size = new System.Drawing.Size(78, 22);
            this._OptionsButton.Text = "Options";
            // 
            // tsCompression
            // 
            this.tsCompression.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsNone,
            this.tsBest,
            this.tsOne});
            this.tsCompression.Name = "tsCompression";
            this.tsCompression.Size = new System.Drawing.Size(149, 22);
            this.tsCompression.Text = "Compression";
            // 
            // tsNone
            // 
            this.tsNone.Checked = true;
            this.tsNone.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsNone.Name = "tsNone";
            this.tsNone.Size = new System.Drawing.Size(169, 22);
            this.tsNone.Text = "No Compression";
            this.tsNone.Click += new System.EventHandler(this.OnCheckNone);
            // 
            // tsBest
            // 
            this.tsBest.Name = "tsBest";
            this.tsBest.Size = new System.Drawing.Size(169, 22);
            this.tsBest.Text = "Best Compression";
            this.tsBest.Click += new System.EventHandler(this.OnCheckBest);
            // 
            // tsOne
            // 
            this.tsOne.CheckOnClick = true;
            this.tsOne.Name = "tsOne";
            this.tsOne.Size = new System.Drawing.Size(169, 22);
            this.tsOne.Text = "One Block";
            this.tsOne.Click += new System.EventHandler(this.OnCheckOne);
            // 
            // tsOpenSelected
            // 
            this.tsOpenSelected.CheckOnClick = true;
            this.tsOpenSelected.Name = "tsOpenSelected";
            this.tsOpenSelected.Size = new System.Drawing.Size(149, 22);
            this.tsOpenSelected.Text = "Open selected";
            // 
            // tsHexMode
            // 
            this.tsHexMode.CheckOnClick = true;
            this.tsHexMode.Name = "tsHexMode";
            this.tsHexMode.Size = new System.Drawing.Size(149, 22);
            this.tsHexMode.Text = "Hex View";
            this.tsHexMode.Click += new System.EventHandler(this.OnCheckHex);
            // 
            // _StripSeparator
            // 
            this._StripSeparator.Name = "_StripSeparator";
            this._StripSeparator.Size = new System.Drawing.Size(146, 6);
            // 
            // tsPackage
            // 
            this.tsPackage.Name = "tsPackage";
            this.tsPackage.Size = new System.Drawing.Size(149, 22);
            this.tsPackage.Text = "Package";
            this.tsPackage.Click += new System.EventHandler(this.OnPackageShow);
            // 
            // tsFnvHash
            // 
            this.tsFnvHash.Name = "tsFnvHash";
            this.tsFnvHash.ShortcutKeyDisplayString = "";
            this.tsFnvHash.Size = new System.Drawing.Size(149, 22);
            this.tsFnvHash.Text = "Hash";
            this.tsFnvHash.Click += new System.EventHandler(this.OnFnvShow);
            // 
            // _HelpButton
            // 
            this._HelpButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this._HelpButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._HelpButton.Image = ((System.Drawing.Image)(resources.GetObject("_HelpButton.Image")));
            this._HelpButton.ImageTransparentColor = System.Drawing.Color.White;
            this._HelpButton.Name = "_HelpButton";
            this._HelpButton.Size = new System.Drawing.Size(23, 22);
            this._HelpButton.Text = "About";
            this._HelpButton.Click += new System.EventHandler(this.OnAboutShow);
            // 
            // _StatusStrip
            // 
            _StatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._HintLabel});
            _StatusStrip.Location = new System.Drawing.Point(0, 417);
            _StatusStrip.Name = "_StatusStrip";
            _StatusStrip.Size = new System.Drawing.Size(934, 22);
            _StatusStrip.TabIndex = 2;
            _StatusStrip.Text = "statusStrip1";
            // 
            // _HintLabel
            // 
            this._HintLabel.Name = "_HintLabel";
            this._HintLabel.Size = new System.Drawing.Size(30, 17);
            this._HintLabel.Text = "Hint";
            // 
            // _EntryTreeView
            // 
            this._EntryTreeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._EntryTreeView.ContextMenuStrip = this._ContextMenu;
            this._EntryTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._EntryTreeView.HideSelection = false;
            this._EntryTreeView.ImageIndex = 0;
            this._EntryTreeView.ImageList = _TypeImageList;
            this._EntryTreeView.Location = new System.Drawing.Point(0, 0);
            this._EntryTreeView.Name = "_EntryTreeView";
            this._EntryTreeView.SelectedImageIndex = 0;
            this._EntryTreeView.Size = new System.Drawing.Size(287, 390);
            this._EntryTreeView.TabIndex = 0;
            this._EntryTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.OnSelectEntry);
            this._EntryTreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.OnOpenEntry1);
            // 
            // _ContextMenu
            // 
            this._ContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._ContextMenuExtract});
            this._ContextMenu.Name = "_ContextMenu";
            this._ContextMenu.Size = new System.Drawing.Size(110, 26);
            this._ContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.OnOpenContextMenu);
            // 
            // _ContextMenuExtract
            // 
            this._ContextMenuExtract.Name = "_ContextMenuExtract";
            this._ContextMenuExtract.Size = new System.Drawing.Size(109, 22);
            this._ContextMenuExtract.Text = "Extract";
            this._ContextMenuExtract.Click += new System.EventHandler(this.OnContextMenuExtract);
            // 
            // _SaveArchiveDialog
            // 
            this._SaveArchiveDialog.DefaultExt = "sds";
            this._SaveArchiveDialog.Filter = "Mafia III SDS archives (*.sds)|*.sds|All Files (*.*)|*.*";
            // 
            // _OpenArchiveDialog
            // 
            this._OpenArchiveDialog.DefaultExt = "sds";
            this._OpenArchiveDialog.Filter = "Mafia III SDS archives (*.sds)|*.sds|All Files (*.*)|*.*";
            // 
            // _SplitContainer
            // 
            this._SplitContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._SplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this._SplitContainer.Location = new System.Drawing.Point(0, 25);
            this._SplitContainer.Name = "_SplitContainer";
            // 
            // _SplitContainer.Panel1
            // 
            this._SplitContainer.Panel1.Controls.Add(this._EntryTreeView);
            this._SplitContainer.Size = new System.Drawing.Size(934, 392);
            this._SplitContainer.SplitterDistance = 289;
            this._SplitContainer.TabIndex = 4;
            // 
            // ArchiveViewer
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(934, 439);
            this.Controls.Add(this._SplitContainer);
            this.Controls.Add(_StatusStrip);
            this.Controls.Add(_ToolStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ArchiveViewer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Mafia III Resource Explorer";
            _ToolStrip.ResumeLayout(false);
            _ToolStrip.PerformLayout();
            _StatusStrip.ResumeLayout(false);
            _StatusStrip.PerformLayout();
            this._ContextMenu.ResumeLayout(false);
            this._SplitContainer.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._SplitContainer)).EndInit();
            this._SplitContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView _EntryTreeView;
        private System.Windows.Forms.ToolStripStatusLabel _HintLabel;
        private System.Windows.Forms.SaveFileDialog _SaveArchiveDialog;
        private System.Windows.Forms.OpenFileDialog _OpenArchiveDialog;
        private System.Windows.Forms.SplitContainer _SplitContainer;
        private System.Windows.Forms.ToolStripDropDownButton _OptionsButton;
        private System.Windows.Forms.ToolStripMenuItem tsCompression;
        private System.Windows.Forms.ToolStripMenuItem tsNone;
        private System.Windows.Forms.ToolStripMenuItem tsBest;
        private System.Windows.Forms.ToolStripMenuItem tsOne;
        private System.Windows.Forms.ToolStripMenuItem tsOpenSelected;
        private System.Windows.Forms.ToolStripButton _OpenArchiveButton;
        private System.Windows.Forms.ToolStripSeparator _StripSeparator;
        private System.Windows.Forms.ToolStripMenuItem tsPackage;
        private System.Windows.Forms.ToolStripMenuItem tsHexMode;
        private System.Windows.Forms.ToolStripMenuItem tsFnvHash;
        private System.Windows.Forms.ToolStripButton _HelpButton;
        private System.Windows.Forms.ContextMenuStrip _ContextMenu;
        private System.Windows.Forms.ToolStripMenuItem _ContextMenuExtract;
        private System.Windows.Forms.ToolStripButton _SaveArchiveButton;
    }
}