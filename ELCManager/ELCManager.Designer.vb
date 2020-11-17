<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class mainForm
	Inherits System.Windows.Forms.Form

	'Form overrides dispose to clean up the component list.
	<System.Diagnostics.DebuggerNonUserCode()> _
	Protected Overrides Sub Dispose(ByVal disposing As Boolean)
		Try
			If disposing AndAlso components IsNot Nothing Then
				components.Dispose()
			End If
		Finally
			MyBase.Dispose(disposing)
		End Try
	End Sub

	'Required by the Windows Form Designer
	Private components As System.ComponentModel.IContainer

	'NOTE: The following procedure is required by the Windows Form Designer
	'It can be modified using the Windows Form Designer.  
	'Do not modify it using the code editor.
	<System.Diagnostics.DebuggerStepThrough()> _
	Private Sub InitializeComponent()
		Me.panelMain = New System.Windows.Forms.TableLayoutPanel()
		Me.pbOpen = New System.Windows.Forms.Button()
		Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
		Me.efPort = New System.Windows.Forms.TextBox()
		Me.TableLayoutPanelFunctions = New System.Windows.Forms.TableLayoutPanel()
		Me.TableLayoutPanel7 = New System.Windows.Forms.TableLayoutPanel()
		Me.TableLayoutPanel10 = New System.Windows.Forms.TableLayoutPanel()
		Me.cbWriteTimeout = New System.Windows.Forms.CheckBox()
		Me.cbWriteCancelled = New System.Windows.Forms.CheckBox()
		Me.Label13 = New System.Windows.Forms.Label()
		Me.udWriteTimer = New System.Windows.Forms.NumericUpDown()
		Me.Label14 = New System.Windows.Forms.Label()
		Me.efName = New System.Windows.Forms.TextBox()
		Me.Label2 = New System.Windows.Forms.Label()
		Me.Label3 = New System.Windows.Forms.Label()
		Me.efLocation = New System.Windows.Forms.TextBox()
		Me.Label7 = New System.Windows.Forms.Label()
		Me.udAmount = New System.Windows.Forms.NumericUpDown()
		Me.Label8 = New System.Windows.Forms.Label()
		Me.udIndex = New System.Windows.Forms.NumericUpDown()
		Me.Label9 = New System.Windows.Forms.Label()
		Me.cboxType = New System.Windows.Forms.ComboBox()
		Me.panelRC = New System.Windows.Forms.TableLayoutPanel()
		Me.efRC = New System.Windows.Forms.TextBox()
		Me.Label11 = New System.Windows.Forms.Label()
		Me.Label10 = New System.Windows.Forms.Label()
		Me.efSeal = New System.Windows.Forms.TextBox()
		Me.writeres = New System.Windows.Forms.TextBox()
		Me.TableLayoutPanel6 = New System.Windows.Forms.TableLayoutPanel()
		Me.cbAbortCheckEjected = New System.Windows.Forms.CheckBox()
		Me.abortres = New System.Windows.Forms.TextBox()
		Me.writeexres = New System.Windows.Forms.TextBox()
		Me.readres = New System.Windows.Forms.TextBox()
		Me.pbAbort = New System.Windows.Forms.Button()
		Me.TableLayoutPanel5 = New System.Windows.Forms.TableLayoutPanel()
		Me.TableLayoutPanel9 = New System.Windows.Forms.TableLayoutPanel()
		Me.cbWriteExTimeout = New System.Windows.Forms.CheckBox()
		Me.cbWriteExCancelled = New System.Windows.Forms.CheckBox()
		Me.udWriteExTimer = New System.Windows.Forms.NumericUpDown()
		Me.Label12 = New System.Windows.Forms.Label()
		Me.Label15 = New System.Windows.Forms.Label()
		Me.efWrite = New System.Windows.Forms.TextBox()
		Me.Label4 = New System.Windows.Forms.Label()
		Me.TableLayoutPanel3 = New System.Windows.Forms.TableLayoutPanel()
		Me.cbReadCheckInside = New System.Windows.Forms.CheckBox()
		Me.efRaw = New System.Windows.Forms.TextBox()
		Me.Label6 = New System.Windows.Forms.Label()
		Me.efCHPN = New System.Windows.Forms.TextBox()
		Me.Label5 = New System.Windows.Forms.Label()
		Me.TableLayoutPanel8 = New System.Windows.Forms.TableLayoutPanel()
		Me.cbReadTimeout = New System.Windows.Forms.CheckBox()
		Me.cbReadCancelled = New System.Windows.Forms.CheckBox()
		Me.Label1 = New System.Windows.Forms.Label()
		Me.udReadTimer = New System.Windows.Forms.NumericUpDown()
		Me.pbState = New System.Windows.Forms.Button()
		Me.TableLayoutPanel4 = New System.Windows.Forms.TableLayoutPanel()
		Me.cbStateCheckInside = New System.Windows.Forms.CheckBox()
		Me.stateres = New System.Windows.Forms.TextBox()
		Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
		Me.pbReadAsync = New System.Windows.Forms.Button()
		Me.pbRead = New System.Windows.Forms.Button()
		Me.TableLayoutPanel11 = New System.Windows.Forms.TableLayoutPanel()
		Me.pbWriteAsync = New System.Windows.Forms.Button()
		Me.pbWrite = New System.Windows.Forms.Button()
		Me.TableLayoutPanel12 = New System.Windows.Forms.TableLayoutPanel()
		Me.pbWriteCheck = New System.Windows.Forms.Button()
		Me.pbWriteCheckAsync = New System.Windows.Forms.Button()
		Me.cbGenerateLog = New System.Windows.Forms.CheckBox()
		Me.panelMain.SuspendLayout()
		Me.TableLayoutPanel2.SuspendLayout()
		Me.TableLayoutPanelFunctions.SuspendLayout()
		Me.TableLayoutPanel7.SuspendLayout()
		Me.TableLayoutPanel10.SuspendLayout()
		CType(Me.udWriteTimer, System.ComponentModel.ISupportInitialize).BeginInit()
		CType(Me.udAmount, System.ComponentModel.ISupportInitialize).BeginInit()
		CType(Me.udIndex, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.panelRC.SuspendLayout()
		Me.TableLayoutPanel6.SuspendLayout()
		Me.TableLayoutPanel5.SuspendLayout()
		Me.TableLayoutPanel9.SuspendLayout()
		CType(Me.udWriteExTimer, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.TableLayoutPanel3.SuspendLayout()
		Me.TableLayoutPanel8.SuspendLayout()
		CType(Me.udReadTimer, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.TableLayoutPanel4.SuspendLayout()
		Me.TableLayoutPanel1.SuspendLayout()
		Me.TableLayoutPanel11.SuspendLayout()
		Me.TableLayoutPanel12.SuspendLayout()
		Me.SuspendLayout()
		'
		'panelMain
		'
		Me.panelMain.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
			Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.panelMain.ColumnCount = 2
		Me.panelMain.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
		Me.panelMain.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
		Me.panelMain.Controls.Add(Me.pbOpen, 0, 0)
		Me.panelMain.Controls.Add(Me.TableLayoutPanel2, 1, 0)
		Me.panelMain.Controls.Add(Me.TableLayoutPanelFunctions, 0, 1)
		Me.panelMain.Location = New System.Drawing.Point(12, 12)
		Me.panelMain.Name = "panelMain"
		Me.panelMain.RowCount = 2
		Me.panelMain.RowStyles.Add(New System.Windows.Forms.RowStyle())
		Me.panelMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
		Me.panelMain.Size = New System.Drawing.Size(949, 325)
		Me.panelMain.TabIndex = 0
		'
		'pbOpen
		'
		Me.pbOpen.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.pbOpen.AutoSize = True
		Me.pbOpen.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.pbOpen.Location = New System.Drawing.Point(3, 4)
		Me.pbOpen.Name = "pbOpen"
		Me.pbOpen.Size = New System.Drawing.Size(43, 23)
		Me.pbOpen.TabIndex = 0
		Me.pbOpen.Text = "Open"
		Me.pbOpen.UseVisualStyleBackColor = True
		'
		'TableLayoutPanel2
		'
		Me.TableLayoutPanel2.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
			Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.TableLayoutPanel2.AutoSize = True
		Me.TableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.TableLayoutPanel2.ColumnCount = 2
		Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
		Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
		Me.TableLayoutPanel2.Controls.Add(Me.efPort, 0, 0)
		Me.TableLayoutPanel2.Controls.Add(Me.cbGenerateLog, 1, 0)
		Me.TableLayoutPanel2.Location = New System.Drawing.Point(52, 3)
		Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
		Me.TableLayoutPanel2.RowCount = 1
		Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
		Me.TableLayoutPanel2.Size = New System.Drawing.Size(894, 26)
		Me.TableLayoutPanel2.TabIndex = 1
		'
		'efPort
		'
		Me.efPort.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.efPort.Location = New System.Drawing.Point(3, 3)
		Me.efPort.Name = "efPort"
		Me.efPort.ReadOnly = True
		Me.efPort.Size = New System.Drawing.Size(770, 20)
		Me.efPort.TabIndex = 0
		'
		'TableLayoutPanelFunctions
		'
		Me.TableLayoutPanelFunctions.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
			Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.TableLayoutPanelFunctions.AutoSize = True
		Me.TableLayoutPanelFunctions.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.TableLayoutPanelFunctions.ColumnCount = 3
		Me.panelMain.SetColumnSpan(Me.TableLayoutPanelFunctions, 2)
		Me.TableLayoutPanelFunctions.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
		Me.TableLayoutPanelFunctions.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 45.0!))
		Me.TableLayoutPanelFunctions.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
		Me.TableLayoutPanelFunctions.Controls.Add(Me.TableLayoutPanel7, 2, 3)
		Me.TableLayoutPanelFunctions.Controls.Add(Me.writeres, 1, 3)
		Me.TableLayoutPanelFunctions.Controls.Add(Me.TableLayoutPanel6, 2, 4)
		Me.TableLayoutPanelFunctions.Controls.Add(Me.abortres, 1, 4)
		Me.TableLayoutPanelFunctions.Controls.Add(Me.writeexres, 1, 2)
		Me.TableLayoutPanelFunctions.Controls.Add(Me.readres, 1, 1)
		Me.TableLayoutPanelFunctions.Controls.Add(Me.pbAbort, 0, 4)
		Me.TableLayoutPanelFunctions.Controls.Add(Me.TableLayoutPanel5, 2, 2)
		Me.TableLayoutPanelFunctions.Controls.Add(Me.TableLayoutPanel3, 2, 1)
		Me.TableLayoutPanelFunctions.Controls.Add(Me.pbState, 0, 0)
		Me.TableLayoutPanelFunctions.Controls.Add(Me.TableLayoutPanel4, 2, 0)
		Me.TableLayoutPanelFunctions.Controls.Add(Me.stateres, 1, 0)
		Me.TableLayoutPanelFunctions.Controls.Add(Me.TableLayoutPanel1, 0, 1)
		Me.TableLayoutPanelFunctions.Controls.Add(Me.TableLayoutPanel11, 0, 2)
		Me.TableLayoutPanelFunctions.Controls.Add(Me.TableLayoutPanel12, 0, 3)
		Me.TableLayoutPanelFunctions.Location = New System.Drawing.Point(3, 35)
		Me.TableLayoutPanelFunctions.Name = "TableLayoutPanelFunctions"
		Me.TableLayoutPanelFunctions.RowCount = 6
		Me.TableLayoutPanelFunctions.RowStyles.Add(New System.Windows.Forms.RowStyle())
		Me.TableLayoutPanelFunctions.RowStyles.Add(New System.Windows.Forms.RowStyle())
		Me.TableLayoutPanelFunctions.RowStyles.Add(New System.Windows.Forms.RowStyle())
		Me.TableLayoutPanelFunctions.RowStyles.Add(New System.Windows.Forms.RowStyle())
		Me.TableLayoutPanelFunctions.RowStyles.Add(New System.Windows.Forms.RowStyle())
		Me.TableLayoutPanelFunctions.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
		Me.TableLayoutPanelFunctions.Size = New System.Drawing.Size(943, 287)
		Me.TableLayoutPanelFunctions.TabIndex = 2
		'
		'TableLayoutPanel7
		'
		Me.TableLayoutPanel7.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
			Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.TableLayoutPanel7.AutoSize = True
		Me.TableLayoutPanel7.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.TableLayoutPanel7.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
		Me.TableLayoutPanel7.ColumnCount = 6
		Me.TableLayoutPanel7.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
		Me.TableLayoutPanel7.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40.0!))
		Me.TableLayoutPanel7.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
		Me.TableLayoutPanel7.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40.0!))
		Me.TableLayoutPanel7.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
		Me.TableLayoutPanel7.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20.0!))
		Me.TableLayoutPanel7.Controls.Add(Me.TableLayoutPanel10, 0, 2)
		Me.TableLayoutPanel7.Controls.Add(Me.efName, 0, 0)
		Me.TableLayoutPanel7.Controls.Add(Me.Label2, 0, 0)
		Me.TableLayoutPanel7.Controls.Add(Me.Label3, 2, 0)
		Me.TableLayoutPanel7.Controls.Add(Me.efLocation, 3, 0)
		Me.TableLayoutPanel7.Controls.Add(Me.Label7, 4, 0)
		Me.TableLayoutPanel7.Controls.Add(Me.udAmount, 5, 0)
		Me.TableLayoutPanel7.Controls.Add(Me.Label8, 4, 1)
		Me.TableLayoutPanel7.Controls.Add(Me.udIndex, 5, 1)
		Me.TableLayoutPanel7.Controls.Add(Me.panelRC, 0, 1)
		Me.TableLayoutPanel7.Location = New System.Drawing.Point(189, 160)
		Me.TableLayoutPanel7.Name = "TableLayoutPanel7"
		Me.TableLayoutPanel7.RowCount = 3
		Me.TableLayoutPanel7.RowStyles.Add(New System.Windows.Forms.RowStyle())
		Me.TableLayoutPanel7.RowStyles.Add(New System.Windows.Forms.RowStyle())
		Me.TableLayoutPanel7.RowStyles.Add(New System.Windows.Forms.RowStyle())
		Me.TableLayoutPanel7.Size = New System.Drawing.Size(751, 91)
		Me.TableLayoutPanel7.TabIndex = 11
		'
		'TableLayoutPanel10
		'
		Me.TableLayoutPanel10.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
			Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.TableLayoutPanel10.AutoSize = True
		Me.TableLayoutPanel10.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.TableLayoutPanel10.ColumnCount = 5
		Me.TableLayoutPanel7.SetColumnSpan(Me.TableLayoutPanel10, 6)
		Me.TableLayoutPanel10.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
		Me.TableLayoutPanel10.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
		Me.TableLayoutPanel10.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
		Me.TableLayoutPanel10.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
		Me.TableLayoutPanel10.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
		Me.TableLayoutPanel10.Controls.Add(Me.cbWriteTimeout, 0, 0)
		Me.TableLayoutPanel10.Controls.Add(Me.cbWriteCancelled, 1, 0)
		Me.TableLayoutPanel10.Controls.Add(Me.Label13, 3, 0)
		Me.TableLayoutPanel10.Controls.Add(Me.udWriteTimer, 4, 0)
		Me.TableLayoutPanel10.Controls.Add(Me.Label14, 2, 0)
		Me.TableLayoutPanel10.Location = New System.Drawing.Point(3, 62)
		Me.TableLayoutPanel10.Name = "TableLayoutPanel10"
		Me.TableLayoutPanel10.RowCount = 1
		Me.TableLayoutPanel10.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
		Me.TableLayoutPanel10.Size = New System.Drawing.Size(745, 26)
		Me.TableLayoutPanel10.TabIndex = 9
		'
		'cbWriteTimeout
		'
		Me.cbWriteTimeout.AutoSize = True
		Me.cbWriteTimeout.Enabled = False
		Me.cbWriteTimeout.Location = New System.Drawing.Point(3, 3)
		Me.cbWriteTimeout.Name = "cbWriteTimeout"
		Me.cbWriteTimeout.Size = New System.Drawing.Size(64, 17)
		Me.cbWriteTimeout.TabIndex = 0
		Me.cbWriteTimeout.Text = "Timeout"
		Me.cbWriteTimeout.UseVisualStyleBackColor = True
		'
		'cbWriteCancelled
		'
		Me.cbWriteCancelled.AutoSize = True
		Me.cbWriteCancelled.Enabled = False
		Me.cbWriteCancelled.Location = New System.Drawing.Point(73, 3)
		Me.cbWriteCancelled.Name = "cbWriteCancelled"
		Me.cbWriteCancelled.Size = New System.Drawing.Size(65, 17)
		Me.cbWriteCancelled.TabIndex = 1
		Me.cbWriteCancelled.Text = "Annulée"
		Me.cbWriteCancelled.UseVisualStyleBackColor = True
		'
		'Label13
		'
		Me.Label13.Anchor = System.Windows.Forms.AnchorStyles.Right
		Me.Label13.AutoSize = True
		Me.Label13.Location = New System.Drawing.Point(655, 6)
		Me.Label13.Name = "Label13"
		Me.Label13.Size = New System.Drawing.Size(36, 13)
		Me.Label13.TabIndex = 4
		Me.Label13.Text = "Timer:"
		'
		'udWriteTimer
		'
		Me.udWriteTimer.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.udWriteTimer.Location = New System.Drawing.Point(697, 3)
		Me.udWriteTimer.Maximum = New Decimal(New Integer() {30, 0, 0, 0})
		Me.udWriteTimer.Minimum = New Decimal(New Integer() {5, 0, 0, 0})
		Me.udWriteTimer.Name = "udWriteTimer"
		Me.udWriteTimer.Size = New System.Drawing.Size(45, 20)
		Me.udWriteTimer.TabIndex = 5
		Me.udWriteTimer.Value = New Decimal(New Integer() {5, 0, 0, 0})
		'
		'Label14
		'
		Me.Label14.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.Label14.AutoSize = True
		Me.Label14.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Label14.ForeColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
		Me.Label14.Location = New System.Drawing.Point(144, 7)
		Me.Label14.Name = "Label14"
		Me.Label14.Size = New System.Drawing.Size(505, 12)
		Me.Label14.TabIndex = 6
		Me.Label14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		'
		'efName
		'
		Me.efName.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.efName.BackColor = System.Drawing.Color.Yellow
		Me.efName.ForeColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
		Me.efName.Location = New System.Drawing.Point(3, 3)
		Me.efName.Name = "efName"
		Me.efName.Size = New System.Drawing.Size(158, 20)
		Me.efName.TabIndex = 0
		'
		'Label2
		'
		Me.Label2.Anchor = System.Windows.Forms.AnchorStyles.Right
		Me.Label2.AutoSize = True
		Me.Label2.Location = New System.Drawing.Point(187, 6)
		Me.Label2.Name = "Label2"
		Me.Label2.Size = New System.Drawing.Size(108, 13)
		Me.Label2.TabIndex = 0
		Me.Label2.Text = "Nom du commerçant:"
		'
		'Label3
		'
		Me.Label3.Anchor = System.Windows.Forms.AnchorStyles.Right
		Me.Label3.AutoSize = True
		Me.Label3.Location = New System.Drawing.Point(301, 6)
		Me.Label3.Name = "Label3"
		Me.Label3.Size = New System.Drawing.Size(124, 13)
		Me.Label3.TabIndex = 2
		Me.Label3.Text = "Adresse du commerçant:"
		'
		'efLocation
		'
		Me.efLocation.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.efLocation.BackColor = System.Drawing.Color.Yellow
		Me.efLocation.ForeColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
		Me.efLocation.Location = New System.Drawing.Point(431, 3)
		Me.efLocation.Name = "efLocation"
		Me.efLocation.Size = New System.Drawing.Size(128, 20)
		Me.efLocation.TabIndex = 1
		'
		'Label7
		'
		Me.Label7.Anchor = System.Windows.Forms.AnchorStyles.Right
		Me.Label7.AutoSize = True
		Me.Label7.Location = New System.Drawing.Point(565, 6)
		Me.Label7.Name = "Label7"
		Me.Label7.Size = New System.Drawing.Size(115, 13)
		Me.Label7.TabIndex = 4
		Me.Label7.Text = "Montant (en centimes):"
		'
		'udAmount
		'
		Me.udAmount.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.udAmount.BackColor = System.Drawing.Color.Yellow
		Me.udAmount.ForeColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
		Me.udAmount.Location = New System.Drawing.Point(686, 3)
		Me.udAmount.Maximum = New Decimal(New Integer() {10000000, 0, 0, 0})
		Me.udAmount.Name = "udAmount"
		Me.udAmount.Size = New System.Drawing.Size(62, 20)
		Me.udAmount.TabIndex = 2
		'
		'Label8
		'
		Me.Label8.Anchor = System.Windows.Forms.AnchorStyles.Right
		Me.Label8.AutoSize = True
		Me.Label8.Location = New System.Drawing.Point(590, 36)
		Me.Label8.Name = "Label8"
		Me.Label8.Size = New System.Drawing.Size(90, 13)
		Me.Label8.TabIndex = 6
		Me.Label8.Text = "Index du chèque:"
		'
		'udIndex
		'
		Me.udIndex.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.udIndex.Location = New System.Drawing.Point(686, 32)
		Me.udIndex.Name = "udIndex"
		Me.udIndex.Size = New System.Drawing.Size(62, 20)
		Me.udIndex.TabIndex = 5
		'
		'Label9
		'
		Me.Label9.Anchor = System.Windows.Forms.AnchorStyles.Right
		Me.Label9.AutoSize = True
		Me.Label9.Location = New System.Drawing.Point(3, 7)
		Me.Label9.Name = "Label9"
		Me.Label9.Size = New System.Drawing.Size(34, 13)
		Me.Label9.TabIndex = 8
		Me.Label9.Text = "Type:"
		'
		'cboxType
		'
		Me.cboxType.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.cboxType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me.cboxType.FormattingEnabled = True
		Me.cboxType.Location = New System.Drawing.Point(43, 3)
		Me.cboxType.Name = "cboxType"
		Me.cboxType.Size = New System.Drawing.Size(95, 21)
		Me.cboxType.TabIndex = 0
		'
		'panelRC
		'
		Me.panelRC.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
			Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.panelRC.AutoSize = True
		Me.panelRC.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.panelRC.ColumnCount = 6
		Me.TableLayoutPanel7.SetColumnSpan(Me.panelRC, 4)
		Me.panelRC.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
		Me.panelRC.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 23.80952!))
		Me.panelRC.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
		Me.panelRC.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 38.09524!))
		Me.panelRC.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
		Me.panelRC.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 38.09524!))
		Me.panelRC.Controls.Add(Me.efRC, 5, 0)
		Me.panelRC.Controls.Add(Me.Label11, 4, 0)
		Me.panelRC.Controls.Add(Me.Label10, 2, 0)
		Me.panelRC.Controls.Add(Me.efSeal, 3, 0)
		Me.panelRC.Controls.Add(Me.cboxType, 1, 0)
		Me.panelRC.Controls.Add(Me.Label9, 0, 0)
		Me.panelRC.Location = New System.Drawing.Point(3, 29)
		Me.panelRC.Name = "panelRC"
		Me.panelRC.RowCount = 1
		Me.panelRC.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
		Me.panelRC.Size = New System.Drawing.Size(556, 27)
		Me.panelRC.TabIndex = 4
		'
		'efRC
		'
		Me.efRC.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.efRC.Location = New System.Drawing.Point(395, 3)
		Me.efRC.Name = "efRC"
		Me.efRC.Size = New System.Drawing.Size(158, 20)
		Me.efRC.TabIndex = 2
		'
		'Label11
		'
		Me.Label11.Anchor = System.Windows.Forms.AnchorStyles.Right
		Me.Label11.AutoSize = True
		Me.Label11.Location = New System.Drawing.Point(354, 7)
		Me.Label11.Name = "Label11"
		Me.Label11.Size = New System.Drawing.Size(35, 13)
		Me.Label11.TabIndex = 2
		Me.Label11.Text = "Code:"
		'
		'Label10
		'
		Me.Label10.Anchor = System.Windows.Forms.AnchorStyles.Right
		Me.Label10.AutoSize = True
		Me.Label10.Location = New System.Drawing.Point(144, 7)
		Me.Label10.Name = "Label10"
		Me.Label10.Size = New System.Drawing.Size(41, 13)
		Me.Label10.TabIndex = 0
		Me.Label10.Text = "Sceau:"
		'
		'efSeal
		'
		Me.efSeal.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.efSeal.Location = New System.Drawing.Point(191, 3)
		Me.efSeal.Name = "efSeal"
		Me.efSeal.Size = New System.Drawing.Size(157, 20)
		Me.efSeal.TabIndex = 1
		'
		'writeres
		'
		Me.writeres.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.writeres.Location = New System.Drawing.Point(144, 195)
		Me.writeres.Name = "writeres"
		Me.writeres.Size = New System.Drawing.Size(39, 20)
		Me.writeres.TabIndex = 10
		'
		'TableLayoutPanel6
		'
		Me.TableLayoutPanel6.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
			Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.TableLayoutPanel6.AutoSize = True
		Me.TableLayoutPanel6.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.TableLayoutPanel6.ColumnCount = 2
		Me.TableLayoutPanel6.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
		Me.TableLayoutPanel6.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
		Me.TableLayoutPanel6.Controls.Add(Me.cbAbortCheckEjected, 0, 0)
		Me.TableLayoutPanel6.Location = New System.Drawing.Point(189, 257)
		Me.TableLayoutPanel6.Name = "TableLayoutPanel6"
		Me.TableLayoutPanel6.RowCount = 1
		Me.TableLayoutPanel6.RowStyles.Add(New System.Windows.Forms.RowStyle())
		Me.TableLayoutPanel6.Size = New System.Drawing.Size(751, 23)
		Me.TableLayoutPanel6.TabIndex = 14
		'
		'cbAbortCheckEjected
		'
		Me.cbAbortCheckEjected.Anchor = System.Windows.Forms.AnchorStyles.Left
		Me.cbAbortCheckEjected.AutoSize = True
		Me.cbAbortCheckEjected.Enabled = False
		Me.cbAbortCheckEjected.Location = New System.Drawing.Point(3, 3)
		Me.cbAbortCheckEjected.Name = "cbAbortCheckEjected"
		Me.cbAbortCheckEjected.Size = New System.Drawing.Size(95, 17)
		Me.cbAbortCheckEjected.TabIndex = 0
		Me.cbAbortCheckEjected.Text = "Chèque éjecté"
		Me.cbAbortCheckEjected.UseVisualStyleBackColor = True
		'
		'abortres
		'
		Me.abortres.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.abortres.Location = New System.Drawing.Point(144, 258)
		Me.abortres.Name = "abortres"
		Me.abortres.Size = New System.Drawing.Size(39, 20)
		Me.abortres.TabIndex = 13
		'
		'writeexres
		'
		Me.writeexres.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.writeexres.Location = New System.Drawing.Point(144, 115)
		Me.writeexres.Name = "writeexres"
		Me.writeexres.Size = New System.Drawing.Size(39, 20)
		Me.writeexres.TabIndex = 7
		'
		'readres
		'
		Me.readres.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.readres.Location = New System.Drawing.Point(144, 51)
		Me.readres.Name = "readres"
		Me.readres.Size = New System.Drawing.Size(39, 20)
		Me.readres.TabIndex = 4
		'
		'pbAbort
		'
		Me.pbAbort.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.pbAbort.AutoSize = True
		Me.pbAbort.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.pbAbort.Location = New System.Drawing.Point(3, 257)
		Me.pbAbort.Name = "pbAbort"
		Me.pbAbort.Size = New System.Drawing.Size(135, 23)
		Me.pbAbort.TabIndex = 12
		Me.pbAbort.Text = "Annulation/Ejection"
		Me.pbAbort.UseVisualStyleBackColor = True
		'
		'TableLayoutPanel5
		'
		Me.TableLayoutPanel5.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
			Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.TableLayoutPanel5.AutoSize = True
		Me.TableLayoutPanel5.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.TableLayoutPanel5.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
		Me.TableLayoutPanel5.ColumnCount = 4
		Me.TableLayoutPanel5.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
		Me.TableLayoutPanel5.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
		Me.TableLayoutPanel5.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
		Me.TableLayoutPanel5.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
		Me.TableLayoutPanel5.Controls.Add(Me.TableLayoutPanel9, 0, 1)
		Me.TableLayoutPanel5.Controls.Add(Me.efWrite, 0, 0)
		Me.TableLayoutPanel5.Controls.Add(Me.Label4, 0, 0)
		Me.TableLayoutPanel5.Location = New System.Drawing.Point(189, 96)
		Me.TableLayoutPanel5.Name = "TableLayoutPanel5"
		Me.TableLayoutPanel5.RowCount = 2
		Me.TableLayoutPanel5.RowStyles.Add(New System.Windows.Forms.RowStyle())
		Me.TableLayoutPanel5.RowStyles.Add(New System.Windows.Forms.RowStyle())
		Me.TableLayoutPanel5.Size = New System.Drawing.Size(751, 58)
		Me.TableLayoutPanel5.TabIndex = 8
		'
		'TableLayoutPanel9
		'
		Me.TableLayoutPanel9.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
			Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.TableLayoutPanel9.AutoSize = True
		Me.TableLayoutPanel9.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.TableLayoutPanel9.ColumnCount = 5
		Me.TableLayoutPanel5.SetColumnSpan(Me.TableLayoutPanel9, 2)
		Me.TableLayoutPanel9.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
		Me.TableLayoutPanel9.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
		Me.TableLayoutPanel9.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
		Me.TableLayoutPanel9.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
		Me.TableLayoutPanel9.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
		Me.TableLayoutPanel9.Controls.Add(Me.cbWriteExTimeout, 0, 0)
		Me.TableLayoutPanel9.Controls.Add(Me.cbWriteExCancelled, 1, 0)
		Me.TableLayoutPanel9.Controls.Add(Me.udWriteExTimer, 4, 0)
		Me.TableLayoutPanel9.Controls.Add(Me.Label12, 3, 0)
		Me.TableLayoutPanel9.Controls.Add(Me.Label15, 2, 0)
		Me.TableLayoutPanel9.Location = New System.Drawing.Point(3, 29)
		Me.TableLayoutPanel9.Name = "TableLayoutPanel9"
		Me.TableLayoutPanel9.RowCount = 1
		Me.TableLayoutPanel9.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
		Me.TableLayoutPanel9.Size = New System.Drawing.Size(745, 26)
		Me.TableLayoutPanel9.TabIndex = 4
		'
		'cbWriteExTimeout
		'
		Me.cbWriteExTimeout.AutoSize = True
		Me.cbWriteExTimeout.Enabled = False
		Me.cbWriteExTimeout.Location = New System.Drawing.Point(3, 3)
		Me.cbWriteExTimeout.Name = "cbWriteExTimeout"
		Me.cbWriteExTimeout.Size = New System.Drawing.Size(64, 17)
		Me.cbWriteExTimeout.TabIndex = 0
		Me.cbWriteExTimeout.Text = "Timeout"
		Me.cbWriteExTimeout.UseVisualStyleBackColor = True
		'
		'cbWriteExCancelled
		'
		Me.cbWriteExCancelled.AutoSize = True
		Me.cbWriteExCancelled.Enabled = False
		Me.cbWriteExCancelled.Location = New System.Drawing.Point(73, 3)
		Me.cbWriteExCancelled.Name = "cbWriteExCancelled"
		Me.cbWriteExCancelled.Size = New System.Drawing.Size(65, 17)
		Me.cbWriteExCancelled.TabIndex = 1
		Me.cbWriteExCancelled.Text = "Annulée"
		Me.cbWriteExCancelled.UseVisualStyleBackColor = True
		'
		'udWriteExTimer
		'
		Me.udWriteExTimer.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.udWriteExTimer.Location = New System.Drawing.Point(697, 3)
		Me.udWriteExTimer.Maximum = New Decimal(New Integer() {30, 0, 0, 0})
		Me.udWriteExTimer.Minimum = New Decimal(New Integer() {5, 0, 0, 0})
		Me.udWriteExTimer.Name = "udWriteExTimer"
		Me.udWriteExTimer.Size = New System.Drawing.Size(45, 20)
		Me.udWriteExTimer.TabIndex = 4
		Me.udWriteExTimer.Value = New Decimal(New Integer() {5, 0, 0, 0})
		'
		'Label12
		'
		Me.Label12.Anchor = System.Windows.Forms.AnchorStyles.Right
		Me.Label12.AutoSize = True
		Me.Label12.Location = New System.Drawing.Point(655, 6)
		Me.Label12.Name = "Label12"
		Me.Label12.Size = New System.Drawing.Size(36, 13)
		Me.Label12.TabIndex = 3
		Me.Label12.Text = "Timer:"
		'
		'Label15
		'
		Me.Label15.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.Label15.AutoSize = True
		Me.Label15.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Label15.ForeColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
		Me.Label15.Location = New System.Drawing.Point(144, 7)
		Me.Label15.Name = "Label15"
		Me.Label15.Size = New System.Drawing.Size(505, 12)
		Me.Label15.TabIndex = 7
		Me.Label15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		'
		'efWrite
		'
		Me.efWrite.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.efWrite.Location = New System.Drawing.Point(96, 3)
		Me.efWrite.Name = "efWrite"
		Me.efWrite.Size = New System.Drawing.Size(652, 20)
		Me.efWrite.TabIndex = 0
		'
		'Label4
		'
		Me.Label4.Anchor = System.Windows.Forms.AnchorStyles.Right
		Me.Label4.AutoSize = True
		Me.Label4.Location = New System.Drawing.Point(3, 6)
		Me.Label4.Name = "Label4"
		Me.Label4.Size = New System.Drawing.Size(87, 13)
		Me.Label4.TabIndex = 0
		Me.Label4.Text = "Texte à imprimer:"
		'
		'TableLayoutPanel3
		'
		Me.TableLayoutPanel3.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
			Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.TableLayoutPanel3.AutoSize = True
		Me.TableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.TableLayoutPanel3.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
		Me.TableLayoutPanel3.ColumnCount = 5
		Me.TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
		Me.TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
		Me.TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
		Me.TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
		Me.TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
		Me.TableLayoutPanel3.Controls.Add(Me.cbReadCheckInside, 0, 0)
		Me.TableLayoutPanel3.Controls.Add(Me.efRaw, 1, 0)
		Me.TableLayoutPanel3.Controls.Add(Me.Label6, 1, 0)
		Me.TableLayoutPanel3.Controls.Add(Me.efCHPN, 4, 0)
		Me.TableLayoutPanel3.Controls.Add(Me.Label5, 3, 0)
		Me.TableLayoutPanel3.Controls.Add(Me.TableLayoutPanel8, 0, 1)
		Me.TableLayoutPanel3.Location = New System.Drawing.Point(189, 32)
		Me.TableLayoutPanel3.Name = "TableLayoutPanel3"
		Me.TableLayoutPanel3.RowCount = 2
		Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle())
		Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle())
		Me.TableLayoutPanel3.Size = New System.Drawing.Size(751, 58)
		Me.TableLayoutPanel3.TabIndex = 5
		'
		'cbReadCheckInside
		'
		Me.cbReadCheckInside.Anchor = System.Windows.Forms.AnchorStyles.Left
		Me.cbReadCheckInside.AutoSize = True
		Me.cbReadCheckInside.Enabled = False
		Me.cbReadCheckInside.Location = New System.Drawing.Point(3, 4)
		Me.cbReadCheckInside.Name = "cbReadCheckInside"
		Me.cbReadCheckInside.Size = New System.Drawing.Size(175, 17)
		Me.cbReadCheckInside.TabIndex = 0
		Me.cbReadCheckInside.Text = "Chèque toujours dans le lecteur"
		Me.cbReadCheckInside.UseVisualStyleBackColor = True
		'
		'efRaw
		'
		Me.efRaw.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.efRaw.Location = New System.Drawing.Point(222, 3)
		Me.efRaw.Name = "efRaw"
		Me.efRaw.ReadOnly = True
		Me.efRaw.Size = New System.Drawing.Size(237, 20)
		Me.efRaw.TabIndex = 1
		'
		'Label6
		'
		Me.Label6.Anchor = System.Windows.Forms.AnchorStyles.Right
		Me.Label6.AutoSize = True
		Me.Label6.Location = New System.Drawing.Point(184, 6)
		Me.Label6.Name = "Label6"
		Me.Label6.Size = New System.Drawing.Size(32, 13)
		Me.Label6.TabIndex = 0
		Me.Label6.Text = "Raw:"
		'
		'efCHPN
		'
		Me.efCHPN.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.efCHPN.Location = New System.Drawing.Point(511, 3)
		Me.efCHPN.Name = "efCHPN"
		Me.efCHPN.ReadOnly = True
		Me.efCHPN.Size = New System.Drawing.Size(237, 20)
		Me.efCHPN.TabIndex = 2
		'
		'Label5
		'
		Me.Label5.Anchor = System.Windows.Forms.AnchorStyles.Right
		Me.Label5.AutoSize = True
		Me.Label5.Location = New System.Drawing.Point(465, 6)
		Me.Label5.Name = "Label5"
		Me.Label5.Size = New System.Drawing.Size(40, 13)
		Me.Label5.TabIndex = 2
		Me.Label5.Text = "CHPN:"
		'
		'TableLayoutPanel8
		'
		Me.TableLayoutPanel8.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
			Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.TableLayoutPanel8.AutoSize = True
		Me.TableLayoutPanel8.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.TableLayoutPanel8.ColumnCount = 5
		Me.TableLayoutPanel3.SetColumnSpan(Me.TableLayoutPanel8, 5)
		Me.TableLayoutPanel8.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
		Me.TableLayoutPanel8.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
		Me.TableLayoutPanel8.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
		Me.TableLayoutPanel8.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
		Me.TableLayoutPanel8.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
		Me.TableLayoutPanel8.Controls.Add(Me.cbReadTimeout, 0, 0)
		Me.TableLayoutPanel8.Controls.Add(Me.cbReadCancelled, 1, 0)
		Me.TableLayoutPanel8.Controls.Add(Me.Label1, 3, 0)
		Me.TableLayoutPanel8.Controls.Add(Me.udReadTimer, 4, 0)
		Me.TableLayoutPanel8.Location = New System.Drawing.Point(3, 29)
		Me.TableLayoutPanel8.Name = "TableLayoutPanel8"
		Me.TableLayoutPanel8.RowCount = 1
		Me.TableLayoutPanel8.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
		Me.TableLayoutPanel8.Size = New System.Drawing.Size(745, 26)
		Me.TableLayoutPanel8.TabIndex = 3
		'
		'cbReadTimeout
		'
		Me.cbReadTimeout.AutoSize = True
		Me.cbReadTimeout.Enabled = False
		Me.cbReadTimeout.Location = New System.Drawing.Point(3, 3)
		Me.cbReadTimeout.Name = "cbReadTimeout"
		Me.cbReadTimeout.Size = New System.Drawing.Size(64, 17)
		Me.cbReadTimeout.TabIndex = 0
		Me.cbReadTimeout.Text = "Timeout"
		Me.cbReadTimeout.UseVisualStyleBackColor = True
		'
		'cbReadCancelled
		'
		Me.cbReadCancelled.AutoSize = True
		Me.cbReadCancelled.Enabled = False
		Me.cbReadCancelled.Location = New System.Drawing.Point(73, 3)
		Me.cbReadCancelled.Name = "cbReadCancelled"
		Me.cbReadCancelled.Size = New System.Drawing.Size(65, 17)
		Me.cbReadCancelled.TabIndex = 1
		Me.cbReadCancelled.Text = "Annulée"
		Me.cbReadCancelled.UseVisualStyleBackColor = True
		'
		'Label1
		'
		Me.Label1.Anchor = System.Windows.Forms.AnchorStyles.Right
		Me.Label1.AutoSize = True
		Me.Label1.Location = New System.Drawing.Point(655, 6)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New System.Drawing.Size(36, 13)
		Me.Label1.TabIndex = 2
		Me.Label1.Text = "Timer:"
		'
		'udReadTimer
		'
		Me.udReadTimer.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.udReadTimer.Location = New System.Drawing.Point(697, 3)
		Me.udReadTimer.Maximum = New Decimal(New Integer() {30, 0, 0, 0})
		Me.udReadTimer.Minimum = New Decimal(New Integer() {5, 0, 0, 0})
		Me.udReadTimer.Name = "udReadTimer"
		Me.udReadTimer.Size = New System.Drawing.Size(45, 20)
		Me.udReadTimer.TabIndex = 3
		Me.udReadTimer.Value = New Decimal(New Integer() {5, 0, 0, 0})
		'
		'pbState
		'
		Me.pbState.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.pbState.AutoSize = True
		Me.pbState.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.pbState.Location = New System.Drawing.Point(3, 3)
		Me.pbState.Name = "pbState"
		Me.pbState.Size = New System.Drawing.Size(135, 23)
		Me.pbState.TabIndex = 0
		Me.pbState.Text = "Etat du lecteur"
		Me.pbState.UseVisualStyleBackColor = True
		'
		'TableLayoutPanel4
		'
		Me.TableLayoutPanel4.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
			Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.TableLayoutPanel4.AutoSize = True
		Me.TableLayoutPanel4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.TableLayoutPanel4.ColumnCount = 2
		Me.TableLayoutPanel4.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
		Me.TableLayoutPanel4.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
		Me.TableLayoutPanel4.Controls.Add(Me.cbStateCheckInside, 0, 0)
		Me.TableLayoutPanel4.Location = New System.Drawing.Point(189, 3)
		Me.TableLayoutPanel4.Name = "TableLayoutPanel4"
		Me.TableLayoutPanel4.RowCount = 1
		Me.TableLayoutPanel4.RowStyles.Add(New System.Windows.Forms.RowStyle())
		Me.TableLayoutPanel4.Size = New System.Drawing.Size(751, 23)
		Me.TableLayoutPanel4.TabIndex = 2
		'
		'cbStateCheckInside
		'
		Me.cbStateCheckInside.Anchor = System.Windows.Forms.AnchorStyles.Left
		Me.cbStateCheckInside.AutoSize = True
		Me.cbStateCheckInside.Enabled = False
		Me.cbStateCheckInside.Location = New System.Drawing.Point(3, 3)
		Me.cbStateCheckInside.Name = "cbStateCheckInside"
		Me.cbStateCheckInside.Size = New System.Drawing.Size(207, 17)
		Me.cbStateCheckInside.TabIndex = 0
		Me.cbStateCheckInside.Text = "Chèque devant le lecteur prêt à être lu"
		Me.cbStateCheckInside.UseVisualStyleBackColor = True
		'
		'stateres
		'
		Me.stateres.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.stateres.Location = New System.Drawing.Point(144, 4)
		Me.stateres.Name = "stateres"
		Me.stateres.Size = New System.Drawing.Size(39, 20)
		Me.stateres.TabIndex = 1
		'
		'TableLayoutPanel1
		'
		Me.TableLayoutPanel1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
			Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.TableLayoutPanel1.AutoSize = True
		Me.TableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.TableLayoutPanel1.ColumnCount = 1
		Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
		Me.TableLayoutPanel1.Controls.Add(Me.pbReadAsync, 0, 1)
		Me.TableLayoutPanel1.Controls.Add(Me.pbRead, 0, 0)
		Me.TableLayoutPanel1.Location = New System.Drawing.Point(3, 32)
		Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
		Me.TableLayoutPanel1.RowCount = 2
		Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
		Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
		Me.TableLayoutPanel1.Size = New System.Drawing.Size(135, 58)
		Me.TableLayoutPanel1.TabIndex = 3
		'
		'pbReadAsync
		'
		Me.pbReadAsync.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.pbReadAsync.AutoSize = True
		Me.pbReadAsync.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.pbReadAsync.Location = New System.Drawing.Point(3, 32)
		Me.pbReadAsync.Name = "pbReadAsync"
		Me.pbReadAsync.Size = New System.Drawing.Size(129, 23)
		Me.pbReadAsync.TabIndex = 1
		Me.pbReadAsync.Text = "Lecture asynchrone"
		Me.pbReadAsync.UseVisualStyleBackColor = True
		'
		'pbRead
		'
		Me.pbRead.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.pbRead.AutoSize = True
		Me.pbRead.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.pbRead.Location = New System.Drawing.Point(3, 3)
		Me.pbRead.Name = "pbRead"
		Me.pbRead.Size = New System.Drawing.Size(129, 23)
		Me.pbRead.TabIndex = 0
		Me.pbRead.Text = "Lecture d'un chèque"
		Me.pbRead.UseVisualStyleBackColor = True
		'
		'TableLayoutPanel11
		'
		Me.TableLayoutPanel11.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
			Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.TableLayoutPanel11.AutoSize = True
		Me.TableLayoutPanel11.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.TableLayoutPanel11.ColumnCount = 1
		Me.TableLayoutPanel11.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
		Me.TableLayoutPanel11.Controls.Add(Me.pbWriteAsync, 0, 1)
		Me.TableLayoutPanel11.Controls.Add(Me.pbWrite, 0, 0)
		Me.TableLayoutPanel11.Location = New System.Drawing.Point(3, 96)
		Me.TableLayoutPanel11.Name = "TableLayoutPanel11"
		Me.TableLayoutPanel11.RowCount = 2
		Me.TableLayoutPanel11.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
		Me.TableLayoutPanel11.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
		Me.TableLayoutPanel11.Size = New System.Drawing.Size(135, 58)
		Me.TableLayoutPanel11.TabIndex = 6
		'
		'pbWriteAsync
		'
		Me.pbWriteAsync.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.pbWriteAsync.AutoSize = True
		Me.pbWriteAsync.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.pbWriteAsync.Location = New System.Drawing.Point(3, 32)
		Me.pbWriteAsync.Name = "pbWriteAsync"
		Me.pbWriteAsync.Size = New System.Drawing.Size(129, 23)
		Me.pbWriteAsync.TabIndex = 1
		Me.pbWriteAsync.Text = "Impression asynchrone"
		Me.pbWriteAsync.UseVisualStyleBackColor = True
		'
		'pbWrite
		'
		Me.pbWrite.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.pbWrite.AutoSize = True
		Me.pbWrite.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.pbWrite.Location = New System.Drawing.Point(3, 3)
		Me.pbWrite.Name = "pbWrite"
		Me.pbWrite.Size = New System.Drawing.Size(129, 23)
		Me.pbWrite.TabIndex = 0
		Me.pbWrite.Text = "Impression"
		Me.pbWrite.UseVisualStyleBackColor = True
		'
		'TableLayoutPanel12
		'
		Me.TableLayoutPanel12.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
			Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.TableLayoutPanel12.AutoSize = True
		Me.TableLayoutPanel12.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.TableLayoutPanel12.ColumnCount = 1
		Me.TableLayoutPanel12.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
		Me.TableLayoutPanel12.Controls.Add(Me.pbWriteCheck, 0, 0)
		Me.TableLayoutPanel12.Controls.Add(Me.pbWriteCheckAsync, 0, 1)
		Me.TableLayoutPanel12.Location = New System.Drawing.Point(3, 160)
		Me.TableLayoutPanel12.Name = "TableLayoutPanel12"
		Me.TableLayoutPanel12.RowCount = 2
		Me.TableLayoutPanel12.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
		Me.TableLayoutPanel12.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
		Me.TableLayoutPanel12.Size = New System.Drawing.Size(135, 91)
		Me.TableLayoutPanel12.TabIndex = 9
		'
		'pbWriteCheck
		'
		Me.pbWriteCheck.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.pbWriteCheck.AutoSize = True
		Me.pbWriteCheck.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.pbWriteCheck.Location = New System.Drawing.Point(3, 11)
		Me.pbWriteCheck.Name = "pbWriteCheck"
		Me.pbWriteCheck.Size = New System.Drawing.Size(129, 23)
		Me.pbWriteCheck.TabIndex = 0
		Me.pbWriteCheck.Text = "Impression d'un chèque"
		Me.pbWriteCheck.UseVisualStyleBackColor = True
		'
		'pbWriteCheckAsync
		'
		Me.pbWriteCheckAsync.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.pbWriteCheckAsync.AutoSize = True
		Me.pbWriteCheckAsync.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.pbWriteCheckAsync.Location = New System.Drawing.Point(3, 56)
		Me.pbWriteCheckAsync.Name = "pbWriteCheckAsync"
		Me.pbWriteCheckAsync.Size = New System.Drawing.Size(129, 23)
		Me.pbWriteCheckAsync.TabIndex = 1
		Me.pbWriteCheckAsync.Text = "Impression asynchrone"
		Me.pbWriteCheckAsync.UseVisualStyleBackColor = True
		'
		'cbGenerateLog
		'
		Me.cbGenerateLog.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.cbGenerateLog.AutoSize = True
		Me.cbGenerateLog.Location = New System.Drawing.Point(779, 4)
		Me.cbGenerateLog.Name = "cbGenerateLog"
		Me.cbGenerateLog.Size = New System.Drawing.Size(112, 17)
		Me.cbGenerateLog.TabIndex = 1
		Me.cbGenerateLog.Text = "Générer fichier log"
		Me.cbGenerateLog.UseVisualStyleBackColor = True
		'
		'mainForm
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.AutoSize = True
		Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.ClientSize = New System.Drawing.Size(973, 349)
		Me.Controls.Add(Me.panelMain)
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
		Me.MaximizeBox = False
		Me.Name = "mainForm"
		Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
		Me.Text = "ELC Manager"
		Me.panelMain.ResumeLayout(False)
		Me.panelMain.PerformLayout()
		Me.TableLayoutPanel2.ResumeLayout(False)
		Me.TableLayoutPanel2.PerformLayout()
		Me.TableLayoutPanelFunctions.ResumeLayout(False)
		Me.TableLayoutPanelFunctions.PerformLayout()
		Me.TableLayoutPanel7.ResumeLayout(False)
		Me.TableLayoutPanel7.PerformLayout()
		Me.TableLayoutPanel10.ResumeLayout(False)
		Me.TableLayoutPanel10.PerformLayout()
		CType(Me.udWriteTimer, System.ComponentModel.ISupportInitialize).EndInit()
		CType(Me.udAmount, System.ComponentModel.ISupportInitialize).EndInit()
		CType(Me.udIndex, System.ComponentModel.ISupportInitialize).EndInit()
		Me.panelRC.ResumeLayout(False)
		Me.panelRC.PerformLayout()
		Me.TableLayoutPanel6.ResumeLayout(False)
		Me.TableLayoutPanel6.PerformLayout()
		Me.TableLayoutPanel5.ResumeLayout(False)
		Me.TableLayoutPanel5.PerformLayout()
		Me.TableLayoutPanel9.ResumeLayout(False)
		Me.TableLayoutPanel9.PerformLayout()
		CType(Me.udWriteExTimer, System.ComponentModel.ISupportInitialize).EndInit()
		Me.TableLayoutPanel3.ResumeLayout(False)
		Me.TableLayoutPanel3.PerformLayout()
		Me.TableLayoutPanel8.ResumeLayout(False)
		Me.TableLayoutPanel8.PerformLayout()
		CType(Me.udReadTimer, System.ComponentModel.ISupportInitialize).EndInit()
		Me.TableLayoutPanel4.ResumeLayout(False)
		Me.TableLayoutPanel4.PerformLayout()
		Me.TableLayoutPanel1.ResumeLayout(False)
		Me.TableLayoutPanel1.PerformLayout()
		Me.TableLayoutPanel11.ResumeLayout(False)
		Me.TableLayoutPanel11.PerformLayout()
		Me.TableLayoutPanel12.ResumeLayout(False)
		Me.TableLayoutPanel12.PerformLayout()
		Me.ResumeLayout(False)

	End Sub

	Friend WithEvents panelMain As TableLayoutPanel
	Friend WithEvents pbOpen As Button
	Friend WithEvents TableLayoutPanel2 As TableLayoutPanel
	Friend WithEvents efPort As TextBox
	Friend WithEvents TableLayoutPanelFunctions As TableLayoutPanel
	Friend WithEvents pbState As Button
	Friend WithEvents TableLayoutPanel4 As TableLayoutPanel
	Friend WithEvents TableLayoutPanel5 As TableLayoutPanel
	Friend WithEvents Label4 As Label
	Friend WithEvents TableLayoutPanel3 As TableLayoutPanel
	Friend WithEvents pbRead As Button
	Friend WithEvents pbWrite As Button
	Friend WithEvents efWrite As TextBox
	Friend WithEvents efRaw As TextBox
	Friend WithEvents Label6 As Label
	Friend WithEvents efCHPN As TextBox
	Friend WithEvents Label5 As Label
	Friend WithEvents pbAbort As Button
	Friend WithEvents abortres As TextBox
	Friend WithEvents writeexres As TextBox
	Friend WithEvents readres As TextBox
	Friend WithEvents stateres As TextBox
	Friend WithEvents cbStateCheckInside As CheckBox
	Friend WithEvents cbReadCheckInside As CheckBox
	Friend WithEvents TableLayoutPanel6 As TableLayoutPanel
	Friend WithEvents cbAbortCheckEjected As CheckBox
	Friend WithEvents TableLayoutPanel7 As TableLayoutPanel
	Friend WithEvents efName As TextBox
	Friend WithEvents Label2 As Label
	Friend WithEvents Label3 As Label
	Friend WithEvents efLocation As TextBox
	Friend WithEvents Label7 As Label
	Friend WithEvents udAmount As NumericUpDown
	Friend WithEvents Label8 As Label
	Friend WithEvents udIndex As NumericUpDown
	Friend WithEvents writeres As TextBox
	Friend WithEvents pbWriteCheck As Button
	Friend WithEvents Label9 As Label
	Friend WithEvents cboxType As ComboBox
	Friend WithEvents panelRC As TableLayoutPanel
	Friend WithEvents efRC As TextBox
	Friend WithEvents Label11 As Label
	Friend WithEvents Label10 As Label
	Friend WithEvents efSeal As TextBox
	Friend WithEvents TableLayoutPanel10 As TableLayoutPanel
	Friend WithEvents cbWriteTimeout As CheckBox
	Friend WithEvents cbWriteCancelled As CheckBox
	Friend WithEvents TableLayoutPanel9 As TableLayoutPanel
	Friend WithEvents cbWriteExTimeout As CheckBox
	Friend WithEvents cbWriteExCancelled As CheckBox
	Friend WithEvents TableLayoutPanel8 As TableLayoutPanel
	Friend WithEvents cbReadTimeout As CheckBox
	Friend WithEvents cbReadCancelled As CheckBox
	Friend WithEvents Label13 As Label
	Friend WithEvents udWriteTimer As NumericUpDown
	Friend WithEvents udWriteExTimer As NumericUpDown
	Friend WithEvents Label12 As Label
	Friend WithEvents Label1 As Label
	Friend WithEvents udReadTimer As NumericUpDown
	Friend WithEvents Label14 As Label
	Friend WithEvents Label15 As Label
	Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
	Friend WithEvents pbReadAsync As Button
	Friend WithEvents TableLayoutPanel11 As TableLayoutPanel
	Friend WithEvents pbWriteAsync As Button
	Friend WithEvents TableLayoutPanel12 As TableLayoutPanel
	Friend WithEvents pbWriteCheckAsync As Button
	Friend WithEvents cbGenerateLog As CheckBox
End Class
