Imports ELCDotNet
Imports COMMON

Public Class FWait

	Private Elc As ELC
	Private completed As Boolean = False

	Public Sub New(elc As ELC)
		' This call is required by the designer.
		InitializeComponent()
		' Add any initialization after the InitializeComponent() call.
		Me.Elc = elc
	End Sub

	Private Sub Wait_Load(sender As Object, e As EventArgs) Handles MyBase.Load
		Label1.Text = "Veuillez patienter"
		Label2.Text = String.Empty
	End Sub

	Protected Overrides Sub WndProc(ByRef m As Message)
		Select Case (m.Msg)
			Case Elc.WMStartTimer
				Label2.Text = "Démarrage du timer (si nécessaire)"

			Case Elc.WMProcessHasEndedEvent
				Label2.Text = "Fin du processus"
				pbCancel.Text = "Fermer"
				completed = True
		End Select
		MyBase.WndProc(m)
	End Sub

	Private Sub pbCancel_Click(sender As Object, e As EventArgs) Handles pbCancel.Click
		If completed Then
			DialogResult = DialogResult.OK
		Else
			Elc.CancelAsync()
			DialogResult = DialogResult.Cancel
		End If
	End Sub

End Class