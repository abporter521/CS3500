namespace SpreadsheetGUI
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
            this.label1 = new System.Windows.Forms.Label();
            this.formulaField = new System.Windows.Forms.TextBox();
            this.fileStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.additionalFeaturesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rEADMEDocumentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fAQsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.whereIsTheHorizontalScrollBarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.enterButton = new System.Windows.Forms.Button();
            this.savedBox = new System.Windows.Forms.TextBox();
            this.cellSelected = new System.Windows.Forms.TextBox();
            this.value = new System.Windows.Forms.Label();
            this.valueBox = new System.Windows.Forms.TextBox();
            this.saveFileWindow = new System.Windows.Forms.SaveFileDialog();
            this.openFile = new System.Windows.Forms.OpenFileDialog();
            this.spreadsheetUI = new SS.SpreadsheetPanel();
            this.fileStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Monotype Corsiva", 10.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(53, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 22);
            this.label1.TabIndex = 1;
            this.label1.Text = "Cell Formula:";
            // 
            // formulaField
            // 
            this.formulaField.Location = new System.Drawing.Point(164, 33);
            this.formulaField.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.formulaField.Name = "formulaField";
            this.formulaField.Size = new System.Drawing.Size(374, 24);
            this.formulaField.TabIndex = 2;
            // 
            // fileStrip
            // 
            this.fileStrip.BackColor = System.Drawing.Color.DarkGreen;
            this.fileStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.fileStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.fileStrip.Location = new System.Drawing.Point(0, 0);
            this.fileStrip.Name = "fileStrip";
            this.fileStrip.Size = new System.Drawing.Size(913, 28);
            this.fileStrip.TabIndex = 4;
            this.fileStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.openToolStripMenuItem,
            this.closeToolStripMenuItem,
            this.closeToolStripMenuItem1});
            this.fileToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(46, 24);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
            this.saveToolStripMenuItem.Text = "New";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
            this.closeToolStripMenuItem.Text = "Save";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem1
            // 
            this.closeToolStripMenuItem1.Name = "closeToolStripMenuItem1";
            this.closeToolStripMenuItem1.Size = new System.Drawing.Size(181, 26);
            this.closeToolStripMenuItem1.Text = "Close";
            this.closeToolStripMenuItem1.Click += new System.EventHandler(this.closeToolStripMenuItem1_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.additionalFeaturesToolStripMenuItem,
            this.rEADMEDocumentToolStripMenuItem,
            this.fAQsToolStripMenuItem});
            this.helpToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(55, 24);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // additionalFeaturesToolStripMenuItem
            // 
            this.additionalFeaturesToolStripMenuItem.Name = "additionalFeaturesToolStripMenuItem";
            this.additionalFeaturesToolStripMenuItem.Size = new System.Drawing.Size(222, 26);
            this.additionalFeaturesToolStripMenuItem.Text = "Additional Features";
            // 
            // rEADMEDocumentToolStripMenuItem
            // 
            this.rEADMEDocumentToolStripMenuItem.Name = "rEADMEDocumentToolStripMenuItem";
            this.rEADMEDocumentToolStripMenuItem.Size = new System.Drawing.Size(222, 26);
            this.rEADMEDocumentToolStripMenuItem.Text = "README document";
            this.rEADMEDocumentToolStripMenuItem.Click += new System.EventHandler(this.rEADMEDocumentToolStripMenuItem_Click);
            // 
            // fAQsToolStripMenuItem
            // 
            this.fAQsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.whereIsTheHorizontalScrollBarToolStripMenuItem});
            this.fAQsToolStripMenuItem.Name = "fAQsToolStripMenuItem";
            this.fAQsToolStripMenuItem.Size = new System.Drawing.Size(222, 26);
            this.fAQsToolStripMenuItem.Text = "FAQs";
            this.fAQsToolStripMenuItem.Click += new System.EventHandler(this.fAQsToolStripMenuItem_Click);
            // 
            // whereIsTheHorizontalScrollBarToolStripMenuItem
            // 
            this.whereIsTheHorizontalScrollBarToolStripMenuItem.Name = "whereIsTheHorizontalScrollBarToolStripMenuItem";
            this.whereIsTheHorizontalScrollBarToolStripMenuItem.Size = new System.Drawing.Size(317, 26);
            this.whereIsTheHorizontalScrollBarToolStripMenuItem.Text = "Where is the horizontal scroll bar?";
            // 
            // enterButton
            // 
            this.enterButton.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.enterButton.Font = new System.Drawing.Font("Microsoft YaHei UI", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.enterButton.Location = new System.Drawing.Point(544, 29);
            this.enterButton.Name = "enterButton";
            this.enterButton.Size = new System.Drawing.Size(75, 28);
            this.enterButton.TabIndex = 5;
            this.enterButton.Text = "Enter";
            this.enterButton.UseVisualStyleBackColor = false;
            this.enterButton.Click += new System.EventHandler(this.enterButton_Click);
            // 
            // savedBox
            // 
            this.savedBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.savedBox.BackColor = System.Drawing.Color.DarkGreen;
            this.savedBox.Font = new System.Drawing.Font("MS Reference Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.savedBox.ForeColor = System.Drawing.Color.White;
            this.savedBox.Location = new System.Drawing.Point(3, 425);
            this.savedBox.Name = "savedBox";
            this.savedBox.ReadOnly = true;
            this.savedBox.Size = new System.Drawing.Size(898, 23);
            this.savedBox.TabIndex = 6;
            // 
            // cellSelected
            // 
            this.cellSelected.Font = new System.Drawing.Font("Microsoft YaHei UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cellSelected.Location = new System.Drawing.Point(8, 31);
            this.cellSelected.Name = "cellSelected";
            this.cellSelected.ReadOnly = true;
            this.cellSelected.Size = new System.Drawing.Size(39, 24);
            this.cellSelected.TabIndex = 7;
            this.cellSelected.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // value
            // 
            this.value.AutoSize = true;
            this.value.Font = new System.Drawing.Font("Monotype Corsiva", 10.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.value.Location = new System.Drawing.Point(732, 35);
            this.value.Name = "value";
            this.value.Size = new System.Drawing.Size(56, 22);
            this.value.TabIndex = 8;
            this.value.Text = "Value:";
            // 
            // valueBox
            // 
            this.valueBox.Location = new System.Drawing.Point(794, 33);
            this.valueBox.Name = "valueBox";
            this.valueBox.ReadOnly = true;
            this.valueBox.Size = new System.Drawing.Size(64, 24);
            this.valueBox.TabIndex = 9;
            // 
            // saveFileWindow
            // 
            this.saveFileWindow.DefaultExt = "sprd";
            this.saveFileWindow.FileName = "Spreadsheet1";
            this.saveFileWindow.Filter = "Spreadsheet(.sprd)|*.sprd";
            this.saveFileWindow.Title = "Save File";
            // 
            // openFile
            // 
            this.openFile.FileName = "mySpreadsheet1";
            this.openFile.Filter = "Spreadsheet (.sprd)|*.sprd|All Files|*.*";
            this.openFile.FileOk += new System.ComponentModel.CancelEventHandler(this.openFile_FileOk);
            // 
            // spreadsheetUI
            // 
            this.spreadsheetUI.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.spreadsheetUI.Location = new System.Drawing.Point(0, 59);
            this.spreadsheetUI.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.spreadsheetUI.Name = "spreadsheetUI";
            this.spreadsheetUI.Size = new System.Drawing.Size(901, 359);
            this.spreadsheetUI.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(913, 446);
            this.Controls.Add(this.valueBox);
            this.Controls.Add(this.value);
            this.Controls.Add(this.cellSelected);
            this.Controls.Add(this.savedBox);
            this.Controls.Add(this.enterButton);
            this.Controls.Add(this.fileStrip);
            this.Controls.Add(this.formulaField);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.spreadsheetUI);
            this.Font = new System.Drawing.Font("Microsoft YaHei UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainMenuStrip = this.fileStrip;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.fileStrip.ResumeLayout(false);
            this.fileStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SS.SpreadsheetPanel spreadsheetUI;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox formulaField;
        private System.Windows.Forms.MenuStrip fileStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.Button enterButton;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem1;
        private System.Windows.Forms.TextBox savedBox;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem additionalFeaturesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rEADMEDocumentToolStripMenuItem;
        private System.Windows.Forms.TextBox cellSelected;
        private System.Windows.Forms.Label value;
        private System.Windows.Forms.TextBox valueBox;
        private System.Windows.Forms.SaveFileDialog saveFileWindow;
        private System.Windows.Forms.OpenFileDialog openFile;
        private System.Windows.Forms.ToolStripMenuItem fAQsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem whereIsTheHorizontalScrollBarToolStripMenuItem;
    }
}

