namespace TipCalculator
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.totalbill = new System.Windows.Forms.Label();
            this.billBox = new System.Windows.Forms.TextBox();
            this.tipAmt = new System.Windows.Forms.TextBox();
            this.tipPercentage = new System.Windows.Forms.Label();
            this.percentageAmt = new System.Windows.Forms.TextBox();
            this.finishedBill = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.totalWTip = new System.Windows.Forms.Label();
            this.bill = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // totalbill
            // 
            this.totalbill.AccessibleName = "";
            this.totalbill.AutoSize = true;
            this.totalbill.Location = new System.Drawing.Point(236, -153);
            this.totalbill.Name = "totalbill";
            this.totalbill.Size = new System.Drawing.Size(105, 20);
            this.totalbill.TabIndex = 0;
            this.totalbill.Text = "Enter Total Bill";
            // 
            // billBox
            // 
            this.billBox.Location = new System.Drawing.Point(380, 102);
            this.billBox.Name = "billBox";
            this.billBox.Size = new System.Drawing.Size(187, 27);
            this.billBox.TabIndex = 1;
            this.billBox.TextChanged += new System.EventHandler(this.billBox_TextChanged);
            // 
            // tipAmt
            // 
            this.tipAmt.Location = new System.Drawing.Point(381, 231);
            this.tipAmt.Name = "tipAmt";
            this.tipAmt.Size = new System.Drawing.Size(187, 27);
            this.tipAmt.TabIndex = 2;
            // 
            // tipPercentage
            // 
            this.tipPercentage.AutoSize = true;
            this.tipPercentage.Font = new System.Drawing.Font("Showcard Gothic", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.tipPercentage.Location = new System.Drawing.Point(211, 168);
            this.tipPercentage.Name = "tipPercentage";
            this.tipPercentage.Size = new System.Drawing.Size(143, 21);
            this.tipPercentage.TabIndex = 4;
            this.tipPercentage.Text = "Tip Percentage";
            // 
            // percentageAmt
            // 
            this.percentageAmt.Location = new System.Drawing.Point(380, 168);
            this.percentageAmt.Name = "percentageAmt";
            this.percentageAmt.Size = new System.Drawing.Size(185, 27);
            this.percentageAmt.TabIndex = 5;
            this.percentageAmt.TextChanged += new System.EventHandler(this.billBox_TextChanged);
            // 
            // finishedBill
            // 
            this.finishedBill.Location = new System.Drawing.Point(382, 284);
            this.finishedBill.Name = "finishedBill";
            this.finishedBill.Size = new System.Drawing.Size(186, 27);
            this.finishedBill.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Showcard Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(211, 238);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 26);
            this.label1.TabIndex = 7;
            this.label1.Text = "Tip";
            // 
            // totalWTip
            // 
            this.totalWTip.AutoSize = true;
            this.totalWTip.Font = new System.Drawing.Font("Showcard Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.totalWTip.Location = new System.Drawing.Point(211, 291);
            this.totalWTip.Name = "totalWTip";
            this.totalWTip.Size = new System.Drawing.Size(122, 26);
            this.totalWTip.TabIndex = 8;
            this.totalWTip.Text = "Total Bill";
            // 
            // bill
            // 
            this.bill.AutoSize = true;
            this.bill.Font = new System.Drawing.Font("Showcard Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.bill.Location = new System.Drawing.Point(211, 106);
            this.bill.Name = "bill";
            this.bill.Size = new System.Drawing.Size(54, 26);
            this.bill.TabIndex = 9;
            this.bill.Text = "Bill";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.bill);
            this.Controls.Add(this.totalWTip);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.finishedBill);
            this.Controls.Add(this.percentageAmt);
            this.Controls.Add(this.tipPercentage);
            this.Controls.Add(this.tipAmt);
            this.Controls.Add(this.billBox);
            this.Controls.Add(this.totalbill);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label totalbill;
        private System.Windows.Forms.TextBox billBox;
        private System.Windows.Forms.TextBox tipAmt;
        private System.Windows.Forms.Label tipPercentage;
        private System.Windows.Forms.TextBox percentageAmt;
        private System.Windows.Forms.TextBox finishedBill;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label totalWTip;
        private System.Windows.Forms.Label bill;
    }
}

