namespace ABETProjectGUI
{
    partial class NewForm
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
            this.playerNumLabel = new System.Windows.Forms.Label();
            this.playerNumTextBox = new System.Windows.Forms.TextBox();
            this.playerListBox = new System.Windows.Forms.ListBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // playerNumLabel
            // 
            this.playerNumLabel.AutoSize = true;
            this.playerNumLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.playerNumLabel.Location = new System.Drawing.Point(44, 63);
            this.playerNumLabel.Name = "playerNumLabel";
            this.playerNumLabel.Size = new System.Drawing.Size(69, 20);
            this.playerNumLabel.TabIndex = 0;
            this.playerNumLabel.Text = "Player #:";
            // 
            // playerNumTextBox
            // 
            this.playerNumTextBox.Location = new System.Drawing.Point(119, 63);
            this.playerNumTextBox.Name = "playerNumTextBox";
            this.playerNumTextBox.Size = new System.Drawing.Size(100, 20);
            this.playerNumTextBox.TabIndex = 1;
            this.playerNumTextBox.TextChanged += new System.EventHandler(this.playerNumTextBox_TextChanged);
            // 
            // playerListBox
            // 
            this.playerListBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.playerListBox.FormattingEnabled = true;
            this.playerListBox.ItemHeight = 20;
            this.playerListBox.Location = new System.Drawing.Point(48, 105);
            this.playerListBox.Name = "playerListBox";
            this.playerListBox.Size = new System.Drawing.Size(686, 284);
            this.playerListBox.TabIndex = 2;
            this.playerListBox.SelectedIndexChanged += new System.EventHandler(this.playerListBox_SelectedIndexChanged);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(256, 53);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(70, 20);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click_1);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(256, 79);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(70, 20);
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click_1);
            // 
            // NewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.playerListBox);
            this.Controls.Add(this.playerNumTextBox);
            this.Controls.Add(this.playerNumLabel);
            this.Name = "NewForm";
            this.Text = "New Player";
            this.Load += new System.EventHandler(this.NewForm_Load_1);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label playerNumLabel;
        private System.Windows.Forms.TextBox playerNumTextBox;
        private System.Windows.Forms.ListBox playerListBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
    }
}