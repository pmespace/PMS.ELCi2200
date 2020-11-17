Imports System.Runtime.InteropServices
Imports ELCDotNet
Imports COMMON

Public Class mainForm

	Private pelc As IntPtr
	Private Elc As New ELC()
	Private Const JSON_FILE_NAME As String = "elcmanager.settings.json"
	Private Const WM_START_TIMER = Win32.WM_USER + 5
	Private Const WM_PROCESS_HAS_ENDED = Win32.WM_USER + 6
	Private wait As FWait

	Private Sub mainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
		For i As PrintType = PrintType._begin + 1 To PrintType._end - 1
			cboxType.Items.Add(i)
		Next
		cboxType.SelectedIndex = 0
		ReadSettings()
		SetOpenButton()
	End Sub

	Private Sub SetUD(up As NumericUpDown, value As Integer, def As Integer)
		Try
			up.Value = value
		Catch ex As Exception
			up.Value = def
		End Try
	End Sub

	Private Sub ReadSettings()
		Dim json As New CJson(Of Settings)
		json.FileName = JSON_FILE_NAME
		Dim settings As Settings = json.ReadSettings
		If Not IsNothing(settings) Then
			SetUD(udAmount, settings.Amount, 100)
			SetUD(udReadTimer, settings.TimerRead, 5)
			SetUD(udWriteTimer, settings.TimerWrite, 5)
			SetUD(udWriteExTimer, settings.TimerWriteEx, 5)
			SetUD(udIndex, settings.Index, 0)
			efWrite.Text = settings.Text
			efName.Text = settings.MerchantName
			efLocation.Text = settings.MerchantAddress
			efSeal.Text = settings.Seal
			efRC.Text = settings.ResponseCode
			Try
				cboxType.SelectedItem = settings.Type
			Catch ex As Exception
				cboxType.SelectedIndex = 0
			End Try
			cbGenerateLog.Checked = settings.GenerateLog
		End If
	End Sub

	Private Sub WriteSettings()
		Dim settings As New Settings
		settings.Amount = udAmount.Value
		settings.TimerRead = udReadTimer.Value
		settings.TimerWrite = udWriteTimer.Value
		settings.TimerWriteEx = udWriteExTimer.Value
		settings.Index = udIndex.Value
		settings.Text = efWrite.Text
		settings.MerchantName = efName.Text
		settings.MerchantAddress = efLocation.Text
		settings.Seal = efSeal.Text
		settings.ResponseCode = efRC.Text
		settings.Type = cboxType.SelectedItem
		settings.GenerateLog = cbGenerateLog.Checked

		Dim json As New CJson(Of Settings)
		json.FileName = JSON_FILE_NAME
		json.WriteSettings(settings)
	End Sub

	Private Sub SetOpenButton()
		If Elc.Opened Then
			pbOpen.Text = "Fermer l'ELC"
			efPort.Text = "Connecté à COM" & Elc.Port.ToString("0")
		Else
			pbOpen.Text = "Ouvrir l'ELC"
			efPort.Text = "Fermé"
		End If
		TableLayoutPanelFunctions.Enabled = Elc.Opened
	End Sub

	Private Sub pbOpen_Click(sender As Object, e As EventArgs) Handles pbOpen.Click
		panelMain.Enabled = False
		If Elc.Opened Then
			Elc.Close()
		Else
			Elc.OpenWithDrivers("elcmanager.drivers.json", cbGenerateLog.Checked)
		End If
		RAZResults()
		SetOpenButton()
		panelMain.Enabled = True
	End Sub

	Private Sub pbState_Click(sender As Object, e As EventArgs) Handles pbState.Click
		panelMain.Enabled = False
		RAZResults()
		Dim docIsInside As Boolean
		stateres.Text = Elc.Status(docIsInside).ToString
		cbStateCheckInside.Checked = docIsInside
		panelMain.Enabled = True
	End Sub

	Private Sub pbRead_Click(sender As Object, e As EventArgs) Handles pbRead.Click, pbReadAsync.Click
		panelMain.Enabled = False
		RAZResults()
		Dim docIsInside As Boolean
		Dim raw As String = Nothing, chpn As String = Nothing
		cbReadTimeout.Checked = False
		cbReadCancelled.Checked = False
		Dim f As ELCi2200.ELCResult
		'call requested function
		If sender Is pbRead Then
			'synchronous processing
			Elc.Read(raw, chpn, docIsInside, udReadTimer.Value)
			f = Elc.LastAsyncResult
		Else
			'asynchronous processing
			PrepareAsync()
			If Elc.ReadAsync(udReadTimer.Value) Then
				If DialogResult.OK = wait.ShowDialog() Then
					f = Elc.ReadAsyncResult(raw, chpn, docIsInside)
				Else
					f = Elc.LastAsyncResult
					MsgBox("L'opération n'est pas complète: " & f.ToString)
				End If
			End If
			wait = Nothing
		End If
		'display result
		readres.Text = (ELCi2200.ELCResult.completed = f).ToString
		If ELCi2200.ELCResult.completed = f Then
			cbReadCheckInside.Checked = docIsInside
			cbReadTimeout.Checked = Elc.Timeout
			cbReadCancelled.Checked = Elc.Cancelled
			efRaw.Text = raw
			efCHPN.Text = chpn
		End If
		panelMain.Enabled = True
	End Sub

	Private Sub pbWrite_Click(sender As Object, e As EventArgs) Handles pbWrite.Click
		panelMain.Enabled = False
		RAZResults()
		cbWriteExTimeout.Checked = False
		cbWriteExCancelled.Checked = False
		Dim s As String = efWrite.Text
		Dim f As ELCi2200.ELCResult
		If sender Is pbWrite Then
			'asynchronous processing
			Elc.WriteEx(s, udWriteExTimer.Value)
			f = Elc.LastAsyncResult
		Else
			'asynchronous processing
			PrepareAsync()
			If Elc.WriteAsyncEx(s, udWriteExTimer.Value) Then
				If DialogResult.OK = wait.ShowDialog() Then
					f = Elc.WriteAsyncResult()
				Else
					f = Elc.LastAsyncResult
					MsgBox("L'opération n'est pas complète: " & f.ToString)
				End If
			End If
			wait = Nothing
		End If
		writeexres.Text = (ELCi2200.ELCResult.completed = f).ToString
		If ELCi2200.ELCResult.completed = f Then
			Label15.Text = s
		Else
			cbWriteExTimeout.Checked = Elc.Timeout
			cbWriteExCancelled.Checked = Elc.Cancelled
		End If
		panelMain.Enabled = True
	End Sub

	Private Sub RAZResults()
		stateres.Text = String.Empty
		cbStateCheckInside.Checked = False
		abortres.Text = String.Empty
		cbAbortCheckEjected.Checked = False
		readres.Text = String.Empty
		cbReadCheckInside.Checked = False
		efRaw.Text = String.Empty
		efCHPN.Text = String.Empty
		efRaw.Text = String.Empty
		writeres.Text = String.Empty
		writeexres.Text = String.Empty
		Label14.Text = String.Empty
		Label15.Text = String.Empty
	End Sub

	Private Sub pbAbort_Click(sender As Object, e As EventArgs) Handles pbAbort.Click
		panelMain.Enabled = False
		RAZResults()
		Dim docHasBeenEjected As Boolean
		abortres.Text = Elc.Abort(docHasBeenEjected).ToString
		cbAbortCheckEjected.Checked = docHasBeenEjected
		panelMain.Enabled = True
	End Sub

	Private Sub pbWriteCheck_Click(sender As Object, e As EventArgs) Handles pbWriteCheck.Click
		panelMain.Enabled = False
		RAZResults()
		Dim o As New ObjectToPrint With
			{
			.Amount = udAmount.Value,
			.MerchantName = efName.Text,
			.MerchantAddress = efLocation.Text,
			.Index = udIndex.Value,
			.Type = cboxType.SelectedItem,
			.ResponseCode = efRC.Text,
			.Seal = efSeal.Text
			}
		cbWriteTimeout.Checked = False
		cbWriteCancelled.Checked = False
		Dim printed As String = Nothing
		Dim f As ELCi2200.ELCResult
		If sender Is pbWriteCheck Then
			'asynchronous processing
			Elc.Write(o, printed, udWriteTimer.Value)
			f = Elc.LastAsyncResult
		Else
			'asynchronous processing
			PrepareAsync()
			If Elc.WriteAsync(o, printed, udWriteTimer.Value) Then
				If DialogResult.OK = wait.ShowDialog() Then
					f = Elc.WriteAsyncResult()
				Else
					f = Elc.LastAsyncResult
					MsgBox("L'opération n'est pas complète: " & f.ToString)
				End If
			End If
			wait = Nothing
		End If
		writeres.Text = (ELCi2200.ELCResult.completed = f).ToString
		If ELCi2200.ELCResult.completed = f Then
			Label14.Text = printed
		Else
			cbWriteTimeout.Checked = Elc.Timeout
			cbWriteCancelled.Checked = Elc.Cancelled
		End If
		panelMain.Enabled = True
	End Sub

	Private Sub cboxType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboxType.SelectedIndexChanged
		Select Case cboxType.SelectedItem
			Case PrintType.garantie, PrintType.fnci
				panelRC.Enabled = True
			Case Else
				panelRC.Enabled = False
		End Select
	End Sub

	Private Sub mainForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
		WriteSettings()
	End Sub

	Private Sub PrepareAsync()
		wait = New FWait(Elc)
		Elc.WindowToWarn = wait.Handle
	End Sub

End Class