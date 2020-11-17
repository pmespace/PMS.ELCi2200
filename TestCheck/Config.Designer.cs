namespace TestCheck
	{
	partial class Config
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
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.pbAccept = new System.Windows.Forms.Button();
			this.pbCancel = new System.Windows.Forms.Button();
			this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
			this.rbFree = new System.Windows.Forms.RadioButton();
			this.rbTransaxLyra = new System.Windows.Forms.RadioButton();
			this.rbFNCI = new System.Windows.Forms.RadioButton();
			this.tableLayoutPanel1.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.tableLayoutPanel3.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 0);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(343, 79);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel2.AutoSize = true;
			this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel2.ColumnCount = 3;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel2.Controls.Add(this.pbAccept, 1, 0);
			this.tableLayoutPanel2.Controls.Add(this.pbCancel, 2, 0);
			this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 47);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 1;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.Size = new System.Drawing.Size(337, 29);
			this.tableLayoutPanel2.TabIndex = 0;
			// 
			// pbAccept
			// 
			this.pbAccept.AutoSize = true;
			this.pbAccept.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.pbAccept.Location = new System.Drawing.Point(226, 3);
			this.pbAccept.Name = "pbAccept";
			this.pbAccept.Size = new System.Drawing.Size(49, 23);
			this.pbAccept.TabIndex = 0;
			this.pbAccept.Text = "Valider";
			this.pbAccept.UseVisualStyleBackColor = true;
			this.pbAccept.Click += new System.EventHandler(this.pbAccept_Click);
			// 
			// pbCancel
			// 
			this.pbCancel.AutoSize = true;
			this.pbCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.pbCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.pbCancel.Location = new System.Drawing.Point(281, 3);
			this.pbCancel.Name = "pbCancel";
			this.pbCancel.Size = new System.Drawing.Size(53, 23);
			this.pbCancel.TabIndex = 1;
			this.pbCancel.Text = "Annuler";
			this.pbCancel.UseVisualStyleBackColor = true;
			this.pbCancel.Click += new System.EventHandler(this.pbCancel_Click);
			// 
			// tableLayoutPanel3
			// 
			this.tableLayoutPanel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel3.AutoSize = true;
			this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel3.ColumnCount = 3;
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.tableLayoutPanel3.Controls.Add(this.rbFree, 2, 0);
			this.tableLayoutPanel3.Controls.Add(this.rbTransaxLyra, 0, 0);
			this.tableLayoutPanel3.Controls.Add(this.rbFNCI, 1, 0);
			this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
			this.tableLayoutPanel3.Name = "tableLayoutPanel3";
			this.tableLayoutPanel3.RowCount = 2;
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel3.Size = new System.Drawing.Size(337, 38);
			this.tableLayoutPanel3.TabIndex = 1;
			// 
			// rbFree
			// 
			this.rbFree.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.rbFree.AutoSize = true;
			this.rbFree.Location = new System.Drawing.Point(227, 3);
			this.rbFree.Name = "rbFree";
			this.rbFree.Size = new System.Drawing.Size(107, 17);
			this.rbFree.TabIndex = 2;
			this.rbFree.TabStop = true;
			this.rbFree.Text = "Libre";
			this.rbFree.UseVisualStyleBackColor = true;
			this.rbFree.CheckedChanged += new System.EventHandler(this.checkedChanged);
			// 
			// rbTransaxLyra
			// 
			this.rbTransaxLyra.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.rbTransaxLyra.AutoSize = true;
			this.rbTransaxLyra.Location = new System.Drawing.Point(3, 3);
			this.rbTransaxLyra.Name = "rbTransaxLyra";
			this.rbTransaxLyra.Size = new System.Drawing.Size(106, 17);
			this.rbTransaxLyra.TabIndex = 0;
			this.rbTransaxLyra.Text = "Transax/Lyra";
			this.rbTransaxLyra.UseVisualStyleBackColor = true;
			this.rbTransaxLyra.CheckedChanged += new System.EventHandler(this.checkedChanged);
			// 
			// rbFNCI
			// 
			this.rbFNCI.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.rbFNCI.AutoSize = true;
			this.rbFNCI.Location = new System.Drawing.Point(115, 3);
			this.rbFNCI.Name = "rbFNCI";
			this.rbFNCI.Size = new System.Drawing.Size(106, 17);
			this.rbFNCI.TabIndex = 1;
			this.rbFNCI.TabStop = true;
			this.rbFNCI.Text = "FNCI";
			this.rbFNCI.UseVisualStyleBackColor = true;
			this.rbFNCI.CheckedChanged += new System.EventHandler(this.checkedChanged);
			// 
			// Config
			// 
			this.AcceptButton = this.pbAccept;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.pbCancel;
			this.ClientSize = new System.Drawing.Size(367, 103);
			this.ControlBox = false;
			this.Controls.Add(this.tableLayoutPanel1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Config";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Choisissez votre configuration";
			this.Load += new System.EventHandler(this.Config_Load);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tableLayoutPanel2.PerformLayout();
			this.tableLayoutPanel3.ResumeLayout(false);
			this.tableLayoutPanel3.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

			}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.Button pbAccept;
		private System.Windows.Forms.Button pbCancel;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
		private System.Windows.Forms.RadioButton rbFree;
		private System.Windows.Forms.RadioButton rbTransaxLyra;
		private System.Windows.Forms.RadioButton rbFNCI;
		}
	}