namespace ResourceExplorer
{
    partial class HashViewer
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
            this.textBoxSrc = new System.Windows.Forms.TextBox();
            this._ButtonCalculate = new System.Windows.Forms.Button();
            this.textBox32 = new System.Windows.Forms.TextBox();
            this.textBox64 = new System.Windows.Forms.TextBox();
            this._CheckSwap = new System.Windows.Forms.CheckBox();
            this._CheckHex = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBoxSrc
            // 
            this.textBoxSrc.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSrc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxSrc.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBoxSrc.Location = new System.Drawing.Point(93, 12);
            this.textBoxSrc.Multiline = true;
            this.textBoxSrc.Name = "textBoxSrc";
            this.textBoxSrc.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxSrc.Size = new System.Drawing.Size(333, 76);
            this.textBoxSrc.TabIndex = 1;
            this.textBoxSrc.Text = "texture.dds";
            // 
            // _ButtonCalculate
            // 
            this._ButtonCalculate.Location = new System.Drawing.Point(11, 12);
            this._ButtonCalculate.Name = "_ButtonCalculate";
            this._ButtonCalculate.Size = new System.Drawing.Size(76, 23);
            this._ButtonCalculate.TabIndex = 0;
            this._ButtonCalculate.Text = "Calculate";
            this._ButtonCalculate.UseVisualStyleBackColor = true;
            this._ButtonCalculate.Click += new System.EventHandler(this.OnButtonCalculate);
            // 
            // textBox32
            // 
            this.textBox32.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox32.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox32.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBox32.Location = new System.Drawing.Point(93, 94);
            this.textBox32.Name = "textBox32";
            this.textBox32.Size = new System.Drawing.Size(333, 22);
            this.textBox32.TabIndex = 2;
            // 
            // textBox64
            // 
            this.textBox64.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox64.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox64.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBox64.Location = new System.Drawing.Point(93, 122);
            this.textBox64.Name = "textBox64";
            this.textBox64.Size = new System.Drawing.Size(333, 22);
            this.textBox64.TabIndex = 3;
            // 
            // _CheckSwap
            // 
            this._CheckSwap.AutoSize = true;
            this._CheckSwap.Checked = true;
            this._CheckSwap.CheckState = System.Windows.Forms.CheckState.Checked;
            this._CheckSwap.Location = new System.Drawing.Point(12, 70);
            this._CheckSwap.Name = "_CheckSwap";
            this._CheckSwap.Size = new System.Drawing.Size(73, 17);
            this._CheckSwap.TabIndex = 4;
            this._CheckSwap.Text = "Swap Out";
            this._CheckSwap.UseVisualStyleBackColor = true;
            // 
            // _CheckHex
            // 
            this._CheckHex.AutoSize = true;
            this._CheckHex.Location = new System.Drawing.Point(12, 45);
            this._CheckHex.Name = "_CheckHex";
            this._CheckHex.Size = new System.Drawing.Size(75, 17);
            this._CheckHex.TabIndex = 5;
            this._CheckHex.Text = "Hex String";
            this._CheckHex.UseVisualStyleBackColor = true;
            this._CheckHex.CheckedChanged += new System.EventHandler(this.OnCheckHexChecked);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 97);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Result Hash 32";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 125);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Result Hash 64";
            // 
            // FnvHash
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 156);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._CheckHex);
            this.Controls.Add(this._CheckSwap);
            this.Controls.Add(this.textBox64);
            this.Controls.Add(this.textBox32);
            this.Controls.Add(this._ButtonCalculate);
            this.Controls.Add(this.textBoxSrc);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new System.Drawing.Size(450, 190);
            this.Name = "FnvHash";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FNV Hash";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxSrc;
        private System.Windows.Forms.Button _ButtonCalculate;
        private System.Windows.Forms.TextBox textBox32;
        private System.Windows.Forms.TextBox textBox64;
        private System.Windows.Forms.CheckBox _CheckSwap;
        private System.Windows.Forms.CheckBox _CheckHex;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}