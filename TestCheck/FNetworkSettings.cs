using System;
using System.Drawing;
using System.Windows.Forms;
using COMMON;

namespace TestCheck
{
	partial class FNetworkSettings : Form
	{
		#region public objects for initialisation
		public CNetworkSettings settings;
		#endregion

		private bool modified = false;
		private DialogResult DlgRes = DialogResult.Cancel;

		public FNetworkSettings()
		{
			InitializeComponent();
		}

		private void hasChanged(object sender, EventArgs e)
		{
			modified = true;
			if (sender is MaskedTextBox)
			{
				MaskedTextBox m = (MaskedTextBox)sender;
				if (!string.IsNullOrEmpty(m.Text) || m.MaskCompleted)
					m.BackColor = SystemColors.Window;
				else
					m.BackColor = Color.LightYellow;
			}
			else if (sender is TextBox)
			{
				TextBox m = (TextBox)sender;
				if (!string.IsNullOrEmpty(m.Text))
					m.BackColor = SystemColors.Window;
				else
					m.BackColor = Color.LightYellow;
			}
			SetButtons();
		}

		private void SetButtons()
		{
			pbAccept.Enabled = modified && ValuesAreValid();
		}

		private bool ValuesAreValid()
		{
			return !string.IsNullOrEmpty(IP1.Text) && !string.IsNullOrEmpty(IP2.Text);
		}

		private void SetPortValue(NumericUpDown port, uint value)
		{
			try
			{
				port.Value = value;
			}
			catch (Exception)
			{
				if (port.Maximum < value)
					port.Value = port.Maximum;
				else
					port.Value = port.Minimum;
			}
		}
		private void FNetworkSettings_Load(object sender, EventArgs e)
		{
			send.Minimum = CNetworkSettings.TIMEOUT_MIN;
			send.Maximum = CNetworkSettings.TIMEOUT_MAX;
			receive.Minimum = send.Minimum;
			receive.Maximum = send.Maximum;

			server1.Text = settings.ServerName1;
			IP1.Text = settings.ServerIPAddress1;
			SetPortValue(port1, settings.PortNumber1);
			server2.Text = settings.ServerName2;
			IP2.Text = settings.ServerIPAddress2;
			SetPortValue(port2, settings.PortNumber2);
			send.Value = settings.SendTimeout;
			receive.Value = settings.ReceiveTimeout;
			modified = false;
			SetButtons();
		}

		private void pbCancel_Click(object sender, EventArgs e)
		{
			// arrived here close the form
			DlgRes = DialogResult.Cancel;
			Close();
		}

		private void pbAccept_Click(object sender, EventArgs e)
		{
			// verify all fields
			if (ValuesAreValid())
			{
				// save settings
				settings.ServerName1 = server1.Text;
				settings.ServerIPAddress1 = IP1.Text;
				settings.PortNumber1 = (uint)port1.Value;
				settings.ServerName2 = server2.Text;
				settings.ServerIPAddress2 = IP2.Text;
				settings.PortNumber2 = (uint)port2.Value;
				settings.SendTimeout = (int)send.Value;
				settings.ReceiveTimeout = (int)receive.Value;
				modified = false;
				DlgRes = DialogResult.OK;
				Close();
			}
			else
				MessageBox.Show("Certaines données sont erronées, merci de les corriger ou d'annuler les saisies.", "ERREUR", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void FNetworkSettings_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (modified)
			{
				DialogResult dlg = MessageBox.Show("Voulez-vous vraiment annuler vos saisies ?", "ANNULER", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
				if (e.Cancel = DialogResult.No == dlg)
					return;
			}
			DialogResult = DlgRes;
		}
	}
}
