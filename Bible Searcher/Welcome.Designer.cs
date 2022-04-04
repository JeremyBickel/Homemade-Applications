namespace Bible_Searcher
{
    partial class Welcome
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
            this.lbBooks = new System.Windows.Forms.ListBox();
            this.rtbBible = new System.Windows.Forms.RichTextBox();
            this.rtbStrongs = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // lbBooks
            // 
            this.lbBooks.FormattingEnabled = true;
            this.lbBooks.ItemHeight = 15;
            this.lbBooks.Location = new System.Drawing.Point(12, 12);
            this.lbBooks.Name = "lbBooks";
            this.lbBooks.Size = new System.Drawing.Size(120, 514);
            this.lbBooks.TabIndex = 0;
            // 
            // rtbBible
            // 
            this.rtbBible.Location = new System.Drawing.Point(138, 12);
            this.rtbBible.Name = "rtbBible";
            this.rtbBible.Size = new System.Drawing.Size(535, 799);
            this.rtbBible.TabIndex = 1;
            this.rtbBible.Text = "";
            // 
            // rtbStrongs
            // 
            this.rtbStrongs.Location = new System.Drawing.Point(679, 12);
            this.rtbStrongs.Name = "rtbStrongs";
            this.rtbStrongs.Size = new System.Drawing.Size(333, 799);
            this.rtbStrongs.TabIndex = 2;
            this.rtbStrongs.Text = "";
            // 
            // Welcome
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1024, 830);
            this.Controls.Add(this.rtbStrongs);
            this.Controls.Add(this.rtbBible);
            this.Controls.Add(this.lbBooks);
            this.Name = "Welcome";
            this.Text = "Welcome to Bible Searcher";
            this.ResumeLayout(false);

        }

        #endregion

        private ListBox lbBooks;
        private RichTextBox rtbBible;
        private RichTextBox rtbStrongs;
    }
}