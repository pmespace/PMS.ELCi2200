namespace TestCheck
{
	partial class FNetworkSettings
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
			this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
			this.receive = new System.Windows.Forms.NumericUpDown();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.send = new System.Windows.Forms.NumericUpDown();
			this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
			this.port1 = new System.Windows.Forms.NumericUpDown();
			this.server1 = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.IP1 = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
			this.server2 = new System.Windows.Forms.TextBox();
			this.port2 = new System.Windows.Forms.NumericUpDown();
			this.label7 = new System.Windows.Forms.Label();
			this.IP2 = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.pbAccept = new System.Windows.Forms.Button();
			this.pbCancel = new System.Windows.Forms.Button();
			this.tableLayoutPanel1.SuspendLayout();
			this.tableLayoutPanel3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.receive)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.send)).BeginInit();
			this.tableLayoutPanel6.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.port1)).BeginInit();
			this.tableLayoutPanel4.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.port2)).BeginInit();
			this.tableLayoutPanel2.SuspendLayout();
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
			this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(551, 266);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// tableLayoutPanel3
			// 
			this.tableLayoutPanel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
				| System.Windows.Forms.AnchorStyles.Left)
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel3.AutoSize = true;
			this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel3.ColumnCount = 2;
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel3.Controls.Add(this.receive, 1, 3);
			this.tableLayoutPanel3.Controls.Add(this.label6, 0, 3);
			this.tableLayoutPanel3.Controls.Add(this.label5, 0, 2);
			this.tableLayoutPanel3.Controls.Add(this.label1, 0, 1);
			this.tableLayoutPanel3.Controls.Add(this.label2, 0, 0);
			this.tableLayoutPanel3.Controls.Add(this.send, 1, 2);
			this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel6, 1, 0);
			this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel4, 1, 1);
			this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
			this.tableLayoutPanel3.Name = "tableLayoutPanel3";
			this.tableLayoutPanel3.RowCount = 5;
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel3.Size = new System.Drawing.Size(545, 225);
			this.tableLayoutPanel3.TabIndex = 0;
			// 
			// receive
			// 
			this.receive.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.receive.Location = new System.Drawing.Point(173, 197);
			this.receive.Maximum = new decimal(new int[] {
				300,
				0,
				0,
				0});
			this.receive.Name = "receive";
			this.receive.Size = new System.Drawing.Size(123, 20);
			this.receive.TabIndex = 3;
			this.receive.Value = new decimal(new int[] {
				30,
				0,
				0,
				0});
			this.receive.ValueChanged += new System.EventHandler(this.hasChanged);
			// 
			// label6
			// 
			this.label6.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(27, 200);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(140, 13);
			this.label6.TabIndex = 8;
			this.label6.Text = "En réception (en secondes):";
			// 
			// label5
			// 
			this.label5.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(3, 174);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(164, 13);
			this.label5.TabIndex = 7;
			this.label5.Text = "Timer en émission (en secondes):";
			// 
			// label1
			// 
			this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(65, 119);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(102, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Serveur secondaire:";
			// 
			// label2
			// 
			this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(81, 35);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(86, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "Serveur primaire:";
			// 
			// send
			// 
			this.send.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.send.Location = new System.Drawing.Point(173, 171);
			this.send.Maximum = new decimal(new int[] {
				300,
				0,
				0,
				0});
			this.send.Name = "send";
			this.send.Size = new System.Drawing.Size(123, 20);
			this.send.TabIndex = 2;
			this.send.Value = new decimal(new int[] {
				30,
				0,
				0,
				0});
			this.send.ValueChanged += new System.EventHandler(this.hasChanged);
			// 
			// tableLayoutPanel6
			// 
			this.tableLayoutPanel6.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
				| System.Windows.Forms.AnchorStyles.Left)
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel6.AutoSize = true;
			this.tableLayoutPanel6.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel6.ColumnCount = 2;
			this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel6.Controls.Add(this.port1, 1, 2);
			this.tableLayoutPanel6.Controls.Add(this.server1, 1, 0);
			this.tableLayoutPanel6.Controls.Add(this.label8, 0, 0);
			this.tableLayoutPanel6.Controls.Add(this.IP1, 1, 1);
			this.tableLayoutPanel6.Controls.Add(this.label3, 0, 2);
			this.tableLayoutPanel6.Controls.Add(this.label4, 0, 1);
			this.tableLayoutPanel6.Location = new System.Drawing.Point(173, 3);
			this.tableLayoutPanel6.Name = "tableLayoutPanel6";
			this.tableLayoutPanel6.RowCount = 3;
			this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel6.Size = new System.Drawing.Size(369, 78);
			this.tableLayoutPanel6.TabIndex = 0;
			// 
			// port1
			// 
			this.port1.Location = new System.Drawing.Point(126, 55);
			this.port1.Maximum = new decimal(new int[] {
				65535,
				0,
				0,
				0});
			this.port1.Minimum = new decimal(new int[] {
				1,
				0,
				0,
				0});
			this.port1.Name = "port1";
			this.port1.Size = new System.Drawing.Size(114, 20);
			this.port1.TabIndex = 2;
			this.port1.Value = new decimal(new int[] {
				1,
				0,
				0,
				0});
			// 
			// server1
			// 
			this.server1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.server1.Location = new System.Drawing.Point(126, 3);
			this.server1.Name = "server1";
			this.server1.Size = new System.Drawing.Size(240, 20);
			this.server1.TabIndex = 0;
			this.server1.TextChanged += new System.EventHandler(this.hasChanged);
			// 
			// label8
			// 
			this.label8.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(35, 6);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(85, 13);
			this.label8.TabIndex = 4;
			this.label8.Text = "Nom du serveur:";
			// 
			// IP1
			// 
			this.IP1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.IP1.Location = new System.Drawing.Point(126, 29);
			this.IP1.Name = "IP1";
			this.IP1.Size = new System.Drawing.Size(240, 20);
			this.IP1.TabIndex = 1;
			this.IP1.TextChanged += new System.EventHandler(this.hasChanged);
			// 
			// label3
			// 
			this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(50, 58);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(70, 13);
			this.label3.TabIndex = 1;
			this.label3.Text = "Port à utiliser:";
			// 
			// label4
			// 
			this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(3, 32);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(117, 13);
			this.label4.TabIndex = 3;
			this.label4.Text = "IP ou URL à contacter:";
			// 
			// tableLayoutPanel4
			// 
			this.tableLayoutPanel4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
				| System.Windows.Forms.AnchorStyles.Left)
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel4.AutoSize = true;
			this.tableLayoutPanel4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel4.ColumnCount = 2;
			this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel4.Controls.Add(this.server2, 1, 0);
			this.tableLayoutPanel4.Controls.Add(this.port2, 1, 2);
			this.tableLayoutPanel4.Controls.Add(this.label7, 0, 0);
			this.tableLayoutPanel4.Controls.Add(this.IP2, 1, 1);
			this.tableLayoutPanel4.Controls.Add(this.label9, 0, 2);
			this.tableLayoutPanel4.Controls.Add(this.label10, 0, 1);
			this.tableLayoutPanel4.Location = new System.Drawing.Point(173, 87);
			this.tableLayoutPanel4.Name = "tableLayoutPanel4";
			this.tableLayoutPanel4.RowCount = 3;
			this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel4.Size = new System.Drawing.Size(369, 78);
			this.tableLayoutPanel4.TabIndex = 1;
			// 
			// server2
			// 
			this.server2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.server2.Location = new System.Drawing.Point(126, 3);
			this.server2.Name = "server2";
			this.server2.Size = new System.Drawing.Size(240, 20);
			this.server2.TabIndex = 0;
			this.server2.TextChanged += new System.EventHandler(this.hasChanged);
			// 
			// port2
			// 
			this.port2.Location = new System.Drawing.Point(126, 55);
			this.port2.Maximum = new decimal(new int[] {
				65535,
				0,
				0,
				0});
			this.port2.Minimum = new decimal(new int[] {
				1,
				0,
				0,
				0});
			this.port2.Name = "port2";
			this.port2.Size = new System.Drawing.Size(114, 20);
			this.port2.TabIndex = 2;
			this.port2.Value = new decimal(new int[] {
				1,
				0,
				0,
				0});
			// 
			// label7
			// 
			this.label7.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(35, 6);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(85, 13);
			this.label7.TabIndex = 4;
			this.label7.Text = "Nom du serveur:";
			// 
			// IP2
			// 
			this.IP2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.IP2.Location = new System.Drawing.Point(126, 29);
			this.IP2.Name = "IP2";
			this.IP2.Size = new System.Drawing.Size(240, 20);
			this.IP2.TabIndex = 1;
			this.IP2.TextChanged += new System.EventHandler(this.hasChanged);
			// 
			// label9
			// 
			this.label9.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(50, 58);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(70, 13);
			this.label9.TabIndex = 1;
			this.label9.Text = "Port à utiliser:";
			// 
			// label10
			// 
			this.label10.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(3, 32);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(117, 13);
			this.label10.TabIndex = 3;
			this.label10.Text = "IP ou URL à contacter:";
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
			this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 234);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 1;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.Size = new System.Drawing.Size(545, 29);
			this.tableLayoutPanel2.TabIndex = 1;
			// 
			// pbAccept
			// 
			this.pbAccept.Location = new System.Drawing.Point(386, 3);
			this.pbAccept.Name = "pbAccept";
			this.pbAccept.Size = new System.Drawing.Size(75, 23);
			this.pbAccept.TabIndex = 0;
			this.pbAccept.Text = "&Valider";
			this.pbAccept.UseVisualStyleBackColor = true;
			this.pbAccept.Click += new System.EventHandler(this.pbAccept_Click);
			// 
			// pbCancel
			// 
			this.pbCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.pbCancel.Location = new System.Drawing.Point(467, 3);
			this.pbCancel.Name = "pbCancel";
			this.pbCancel.Size = new System.Drawing.Size(75, 23);
			this.pbCancel.TabIndex = 1;
			this.pbCancel.Text = "&Annuler";
			this.pbCancel.UseVisualStyleBackColor = true;
			this.pbCancel.Click += new System.EventHandler(this.pbCancel_Click);
			// 
			// FNetworkSettings
			// 
			this.AcceptButton = this.pbAccept;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.pbCancel;
			this.ClientSize = new System.Drawing.Size(575, 290);
			this.Controls.Add(this.tableLayoutPanel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FNetworkSettings";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Paramètres réseau";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FNetworkSettings_FormClosing);
			this.Load += new System.EventHandler(this.FNetworkSettings_Load);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.tableLayoutPanel3.ResumeLayout(false);
			this.tableLayoutPanel3.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.receive)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.send)).EndInit();
			this.tableLayoutPanel6.ResumeLayout(false);
			this.tableLayoutPanel6.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.port1)).EndInit();
			this.tableLayoutPanel4.ResumeLayout(false);
			this.tableLayoutPanel4.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.port2)).EndInit();
			this.tableLayoutPanel2.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.Button pbAccept;
		private System.Windows.Forms.Button pbCancel;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox IP1;
		private System.Windows.Forms.NumericUpDown receive;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.NumericUpDown send;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
		private System.Windows.Forms.TextBox server1;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
		private System.Windows.Forms.TextBox server2;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox IP2;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.NumericUpDown port1;
		private System.Windows.Forms.NumericUpDown port2;
	}
}