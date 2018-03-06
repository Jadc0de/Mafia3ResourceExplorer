namespace ManagedBass
{
    partial class PlayerControl
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
            Bass.Free();
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelInfo = new System.Windows.Forms.Label();
            this.picSpectrum = new System.Windows.Forms.PictureBox();
            this.buttonPlay = new System.Windows.Forms.Button();
            this._VolumeTrack = new System.Windows.Forms.TrackBar();
            this._PositionTrack = new System.Windows.Forms.TrackBar();
            this._CheckLoop = new System.Windows.Forms.CheckBox();
            this._PanTrack = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picSpectrum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._VolumeTrack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._PositionTrack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._PanTrack)).BeginInit();
            this.SuspendLayout();
            // 
            // labelInfo
            // 
            this.labelInfo.AutoSize = true;
            this.labelInfo.Location = new System.Drawing.Point(150, 16);
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Size = new System.Drawing.Size(144, 13);
            this.labelInfo.TabIndex = 3;
            this.labelInfo.Text = "00:00:00.000 / 00:00:00.000";
            // 
            // picSpectrum
            // 
            this.picSpectrum.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.picSpectrum.BackColor = System.Drawing.Color.Black;
            this.picSpectrum.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picSpectrum.Location = new System.Drawing.Point(10, 152);
            this.picSpectrum.Name = "picSpectrum";
            this.picSpectrum.Size = new System.Drawing.Size(341, 89);
            this.picSpectrum.TabIndex = 1;
            this.picSpectrum.TabStop = false;
            this.picSpectrum.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBoxSpectrum_MouseDown);
            // 
            // buttonPlay
            // 
            this.buttonPlay.Location = new System.Drawing.Point(10, 10);
            this.buttonPlay.Name = "buttonPlay";
            this.buttonPlay.Size = new System.Drawing.Size(46, 24);
            this.buttonPlay.TabIndex = 1;
            this.buttonPlay.Text = "Play";
            this.buttonPlay.UseVisualStyleBackColor = true;
            this.buttonPlay.Click += new System.EventHandler(this.buttonPlay_Click);
            // 
            // _VolumeTrack
            // 
            this._VolumeTrack.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._VolumeTrack.AutoSize = false;
            this._VolumeTrack.Location = new System.Drawing.Point(68, 82);
            this._VolumeTrack.Maximum = 100;
            this._VolumeTrack.Name = "_VolumeTrack";
            this._VolumeTrack.Size = new System.Drawing.Size(291, 26);
            this._VolumeTrack.TabIndex = 5;
            this._VolumeTrack.TickStyle = System.Windows.Forms.TickStyle.None;
            this._VolumeTrack.Value = 100;
            this._VolumeTrack.Scroll += new System.EventHandler(this.OnVolumeTrack_Scroll);
            // 
            // _PositionTrack
            // 
            this._PositionTrack.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._PositionTrack.AutoSize = false;
            this._PositionTrack.Location = new System.Drawing.Point(68, 50);
            this._PositionTrack.Maximum = 100;
            this._PositionTrack.Name = "_PositionTrack";
            this._PositionTrack.Size = new System.Drawing.Size(291, 26);
            this._PositionTrack.TabIndex = 4;
            this._PositionTrack.TickStyle = System.Windows.Forms.TickStyle.None;
            this._PositionTrack.Scroll += new System.EventHandler(this.OnPositionTrack_Scroll);
            // 
            // _CheckLoop
            // 
            this._CheckLoop.AutoSize = true;
            this._CheckLoop.Location = new System.Drawing.Point(77, 15);
            this._CheckLoop.Name = "_CheckLoop";
            this._CheckLoop.Size = new System.Drawing.Size(50, 17);
            this._CheckLoop.TabIndex = 2;
            this._CheckLoop.Text = "Loop";
            this._CheckLoop.UseVisualStyleBackColor = true;
            this._CheckLoop.CheckedChanged += new System.EventHandler(this._CheckLoop_CheckedChanged);
            // 
            // _PanTrack
            // 
            this._PanTrack.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._PanTrack.AutoSize = false;
            this._PanTrack.Location = new System.Drawing.Point(68, 114);
            this._PanTrack.Maximum = 100;
            this._PanTrack.Minimum = -100;
            this._PanTrack.Name = "_PanTrack";
            this._PanTrack.Size = new System.Drawing.Size(291, 26);
            this._PanTrack.TabIndex = 6;
            this._PanTrack.TickStyle = System.Windows.Forms.TickStyle.None;
            this._PanTrack.Scroll += new System.EventHandler(this.OnPanTrack_Scroll);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Position";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 82);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Volume";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 114);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Balance";
            // 
            // PlayerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._PanTrack);
            this.Controls.Add(this._CheckLoop);
            this.Controls.Add(this._PositionTrack);
            this.Controls.Add(this._VolumeTrack);
            this.Controls.Add(this.labelInfo);
            this.Controls.Add(this.buttonPlay);
            this.Controls.Add(this.picSpectrum);
            this.Name = "PlayerControl";
            this.Size = new System.Drawing.Size(362, 253);
            this.Load += new System.EventHandler(this.OnLoad);
            ((System.ComponentModel.ISupportInitialize)(this.picSpectrum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._VolumeTrack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._PositionTrack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._PanTrack)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox picSpectrum;
        private System.Windows.Forms.Label labelInfo;
        private System.Windows.Forms.Button buttonPlay;
        private System.Windows.Forms.TrackBar _VolumeTrack;
        private System.Windows.Forms.TrackBar _PositionTrack;
        private System.Windows.Forms.CheckBox _CheckLoop;
        private System.Windows.Forms.TrackBar _PanTrack;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}
