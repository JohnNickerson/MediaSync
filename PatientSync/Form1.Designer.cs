namespace PatientSync
{
    partial class Form1
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
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.SourceBox = new System.Windows.Forms.TextBox();
            this.BrowseSourceButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.SharedBox = new System.Windows.Forms.TextBox();
            this.BrowseSharedButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.SpaceBox = new System.Windows.Forms.TextBox();
            this.SpaceUnitSelect = new System.Windows.Forms.ComboBox();
            this.RunButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.OutputBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Source folder";
            this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // SourceBox
            // 
            this.SourceBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.SourceBox.Location = new System.Drawing.Point(15, 25);
            this.SourceBox.Name = "SourceBox";
            this.SourceBox.Size = new System.Drawing.Size(312, 20);
            this.SourceBox.TabIndex = 1;
            // 
            // BrowseSourceButton
            // 
            this.BrowseSourceButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BrowseSourceButton.Location = new System.Drawing.Point(333, 22);
            this.BrowseSourceButton.Name = "BrowseSourceButton";
            this.BrowseSourceButton.Size = new System.Drawing.Size(29, 23);
            this.BrowseSourceButton.TabIndex = 2;
            this.BrowseSourceButton.Text = "...";
            this.BrowseSourceButton.UseVisualStyleBackColor = true;
            this.BrowseSourceButton.Click += new System.EventHandler(this.BrowseSourceButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Shared folder";
            this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // SharedBox
            // 
            this.SharedBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.SharedBox.Location = new System.Drawing.Point(15, 64);
            this.SharedBox.Name = "SharedBox";
            this.SharedBox.Size = new System.Drawing.Size(312, 20);
            this.SharedBox.TabIndex = 4;
            // 
            // BrowseSharedButton
            // 
            this.BrowseSharedButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BrowseSharedButton.Location = new System.Drawing.Point(333, 64);
            this.BrowseSharedButton.Name = "BrowseSharedButton";
            this.BrowseSharedButton.Size = new System.Drawing.Size(29, 23);
            this.BrowseSharedButton.TabIndex = 5;
            this.BrowseSharedButton.Text = "...";
            this.BrowseSharedButton.UseVisualStyleBackColor = true;
            this.BrowseSharedButton.Click += new System.EventHandler(this.BrowseSharedButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 87);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(85, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Reserved space";
            this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // SpaceBox
            // 
            this.SpaceBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.SpaceBox.Location = new System.Drawing.Point(15, 103);
            this.SpaceBox.Name = "SpaceBox";
            this.SpaceBox.Size = new System.Drawing.Size(225, 20);
            this.SpaceBox.TabIndex = 7;
            this.SpaceBox.Text = "100";
            this.SpaceBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // SpaceUnitSelect
            // 
            this.SpaceUnitSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SpaceUnitSelect.FormattingEnabled = true;
            this.SpaceUnitSelect.Items.AddRange(new object[] {
            "B",
            "KB",
            "MB",
            "GB",
            "TB"});
            this.SpaceUnitSelect.Location = new System.Drawing.Point(246, 103);
            this.SpaceUnitSelect.Name = "SpaceUnitSelect";
            this.SpaceUnitSelect.Size = new System.Drawing.Size(81, 21);
            this.SpaceUnitSelect.TabIndex = 8;
            this.SpaceUnitSelect.Text = "MB";
            // 
            // RunButton
            // 
            this.RunButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.RunButton.Location = new System.Drawing.Point(287, 130);
            this.RunButton.Name = "RunButton";
            this.RunButton.Size = new System.Drawing.Size(75, 23);
            this.RunButton.TabIndex = 9;
            this.RunButton.Text = "Run";
            this.RunButton.UseVisualStyleBackColor = true;
            this.RunButton.Click += new System.EventHandler(this.RunButton_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 156);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Output";
            this.label4.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // OutputBox
            // 
            this.OutputBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.OutputBox.Location = new System.Drawing.Point(15, 173);
            this.OutputBox.Multiline = true;
            this.OutputBox.Name = "OutputBox";
            this.OutputBox.Size = new System.Drawing.Size(347, 193);
            this.OutputBox.TabIndex = 11;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(374, 378);
            this.Controls.Add(this.OutputBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.RunButton);
            this.Controls.Add(this.SpaceUnitSelect);
            this.Controls.Add(this.SpaceBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.SourceBox);
            this.Controls.Add(this.BrowseSourceButton);
            this.Controls.Add(this.BrowseSharedButton);
            this.Controls.Add(this.SharedBox);
            this.Controls.Add(this.label2);
            this.Name = "Form1";
            this.Text = "PatientSync";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox SourceBox;
        private System.Windows.Forms.Button BrowseSourceButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox SharedBox;
        private System.Windows.Forms.Button BrowseSharedButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox SpaceBox;
        private System.Windows.Forms.ComboBox SpaceUnitSelect;
        private System.Windows.Forms.Button RunButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox OutputBox;
    }
}

