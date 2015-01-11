namespace SachsenCoder.Anita.WinFormsUi
{
    partial class MainWindow
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtRawContent = new System.Windows.Forms.TextBox();
            this.btnFetchData = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.lstResults = new System.Windows.Forms.ListBox();
            this.lblSelectedName = new System.Windows.Forms.Label();
            this.txtBaseFolder = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSelectFolder = new System.Windows.Forms.Button();
            this.lblProgressInfo = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtRawContent
            // 
            this.txtRawContent.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRawContent.Location = new System.Drawing.Point(12, 391);
            this.txtRawContent.MaxLength = 999999;
            this.txtRawContent.Multiline = true;
            this.txtRawContent.Name = "txtRawContent";
            this.txtRawContent.ReadOnly = true;
            this.txtRawContent.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtRawContent.Size = new System.Drawing.Size(736, 201);
            this.txtRawContent.TabIndex = 0;
            // 
            // btnFetchData
            // 
            this.btnFetchData.Font = new System.Drawing.Font("Consolas", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFetchData.Location = new System.Drawing.Point(754, 608);
            this.btnFetchData.Name = "btnFetchData";
            this.btnFetchData.Size = new System.Drawing.Size(198, 72);
            this.btnFetchData.TabIndex = 1;
            this.btnFetchData.Text = "Fetch Data";
            this.btnFetchData.UseVisualStyleBackColor = true;
            this.btnFetchData.Click += new System.EventHandler(this.btnFetchData_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSearch.Location = new System.Drawing.Point(754, 16);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(198, 26);
            this.txtSearch.TabIndex = 2;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // lstResults
            // 
            this.lstResults.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstResults.FormattingEnabled = true;
            this.lstResults.ItemHeight = 18;
            this.lstResults.Location = new System.Drawing.Point(754, 48);
            this.lstResults.Name = "lstResults";
            this.lstResults.Size = new System.Drawing.Size(198, 526);
            this.lstResults.TabIndex = 3;
            this.lstResults.SelectedIndexChanged += new System.EventHandler(this.lstResults_SelectedIndexChanged);
            // 
            // lblSelectedName
            // 
            this.lblSelectedName.AutoSize = true;
            this.lblSelectedName.Font = new System.Drawing.Font("Consolas", 18F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSelectedName.Location = new System.Drawing.Point(9, 9);
            this.lblSelectedName.Name = "lblSelectedName";
            this.lblSelectedName.Size = new System.Drawing.Size(90, 28);
            this.lblSelectedName.TabIndex = 5;
            this.lblSelectedName.Text = "label1";
            // 
            // txtBaseFolder
            // 
            this.txtBaseFolder.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBaseFolder.Location = new System.Drawing.Point(68, 608);
            this.txtBaseFolder.Name = "txtBaseFolder";
            this.txtBaseFolder.Size = new System.Drawing.Size(615, 26);
            this.txtBaseFolder.TabIndex = 6;
            this.txtBaseFolder.TextChanged += new System.EventHandler(this.txtBasePath_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(8, 611);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 19);
            this.label1.TabIndex = 7;
            this.label1.Text = "Base:";
            // 
            // btnSelectFolder
            // 
            this.btnSelectFolder.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSelectFolder.Location = new System.Drawing.Point(689, 609);
            this.btnSelectFolder.Name = "btnSelectFolder";
            this.btnSelectFolder.Size = new System.Drawing.Size(59, 25);
            this.btnSelectFolder.TabIndex = 8;
            this.btnSelectFolder.Text = "...";
            this.btnSelectFolder.UseVisualStyleBackColor = true;
            this.btnSelectFolder.Click += new System.EventHandler(this.btnSelectFolder_Click);
            // 
            // lblProgressInfo
            // 
            this.lblProgressInfo.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProgressInfo.Location = new System.Drawing.Point(11, 48);
            this.lblProgressInfo.Name = "lblProgressInfo";
            this.lblProgressInfo.Size = new System.Drawing.Size(737, 106);
            this.lblProgressInfo.TabIndex = 9;
            this.lblProgressInfo.Text = "label2";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(617, 15);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(964, 692);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lblProgressInfo);
            this.Controls.Add(this.btnSelectFolder);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtBaseFolder);
            this.Controls.Add(this.lblSelectedName);
            this.Controls.Add(this.lstResults);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.btnFetchData);
            this.Controls.Add(this.txtRawContent);
            this.Name = "MainWindow";
            this.Text = "Anita (R1.5)";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtRawContent;
        private System.Windows.Forms.Button btnFetchData;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.ListBox lstResults;
        private System.Windows.Forms.Label lblSelectedName;
        private System.Windows.Forms.TextBox txtBaseFolder;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSelectFolder;
        private System.Windows.Forms.Label lblProgressInfo;
        private System.Windows.Forms.Button btnCancel;
    }
}

