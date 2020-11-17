using System;
using System.Drawing;
using System.Windows.Forms;

namespace TestCheck
{
	partial class FSettings : Form
	{
		#region public objects for initialisation
		public CSettings settings;
		#endregion

		private bool hasChanged;
		private DialogResult DlgRes = DialogResult.Cancel;

		public FSettings()
		{
			InitializeComponent();
		}

		private bool ValuesAreValid()
		{
			return F32.MaskCompleted && F37.MaskCompleted && F41.MaskCompleted && F42.MaskCompleted;
		}

		private void pbAccept_Click(object sender, EventArgs e)
		{
			// verify all fields
			if (ValuesAreValid())
			{
				// save settings
				settings.F32_BankID = F32.Text;
				settings.F37_IDC = F37.Text;
				settings.F41_TID = F41.Text;
				settings.F42_IsSIRET = F42_SIRET.Checked;
				settings.F42_IDCF = F42.Text;
				settings.AllowOfflineTransactions = offline.Checked;
				settings.AuthorisationThreshold = (int)seuil.Value * 100;
				settings.AllowModifyDecline = cbAllowModifyDeclined.Checked;
				settings.FNCICounter1 = (int)fnci1.Value;
				settings.FNCICounterN = (int)fnciN.Value;
				settings.FNCICounterX = (int)fnciX.Value;
				hasChanged = false;
				DlgRes = DialogResult.OK;
				Close();
			}
			else
				MessageBox.Show("Certaines données sont erronées, merci de les corriger ou d'annuler les saisies.", "ERREUR", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}


		private void FSettings_Load(object sender, EventArgs e)
		{
			toolTip1.SetToolTip(F32, "L'identifiant de la banque n'est requis que pour faire de la collecte chèque.\nSi vous ne connaissez pas votre identifiant vous pouvez le valoriser à 00000");
			toolTip1.SetToolTip(F37, "L'identifiant de centre n'est réellement pertinenent que si vous êtes en relation avec Verifiance,\nil faut alors reporter leur valeur dans cette donnée.\nDans tout autre cas vous pouvez laisser cette donnée inchangé ou la valoriser à votre idée.");
			toolTip1.SetToolTip(F42, "Saisissez l'identifiant qui vous a té communiqué,\nle système ajoutera tout seul les éventuels caractères obligatoires.");
			string s = "La modification de cette donnée ne sera effective qu'après avoir relancé le programme ou quand la journée aura changé.";

			F32.Text = settings.F32_BankID;
			F37.Text = settings.F37_IDC;
			F41.Text = settings.F41_TID;
			F42_SIRET.Checked = settings.F42_IsSIRET;
			F42_abonne.Checked = !settings.F42_IsSIRET;
			SetF42();
			F42.Text = settings.F42_IDCF;

			offline.Checked = settings.AllowOfflineTransactions;
			cbAllowModifyDeclined.Checked = settings.AllowModifyDecline;
			try
			{
				seuil.Value = settings.AuthorisationThreshold / 100;
			}
			catch (Exception) { }
			try
			{
				fnci1.Value = settings.FNCICounter1;
			}
			catch (Exception) { }
			try
			{
				fnciN.Value = settings.FNCICounterN;
			}
			catch (Exception) { }
			try
			{
				fnciX.Value = settings.FNCICounterX;
			}
			catch (Exception) { }

			hasChanged = false;
			SetButtons();
		}

		private void ValueChanged(object sender, EventArgs e)
		{
			hasChanged = true;
			if (sender is MaskedTextBox)
			{
				MaskedTextBox m = (MaskedTextBox)sender;
				if (string.IsNullOrEmpty(m.Text) || m.MaskCompleted)
					m.BackColor = SystemColors.Window;
				else
					m.BackColor = Color.LightYellow;
			}
			else if (sender is TextBox)
			{
			}
			SetButtons();
		}

		private void SetButtons()
		{
			pbAccept.Enabled = ValuesAreValid() && hasChanged;
		}

		private void pbCancel_Click(object sender, EventArgs e)
		{
			// arrived here close the form
			DlgRes = DialogResult.Cancel;
			Close();
		}

		private void FSettings_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (hasChanged)
			{
				DialogResult dlg = MessageBox.Show("Voulez-vous vraiment annuler vos saisies ?", "ANNULER", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
				if (e.Cancel = DialogResult.No == dlg)
					return;
			}
			DialogResult = DlgRes;
		}

		private void SetF42()
		{
			if (F42_SIRET.Checked)
			{
				F42.Text = string.Empty;
				F42.Mask = @"\000000000000000";
			}
			else if (F42_abonne.Checked)
			{
				F42.Text = string.Empty;
				F42.Mask = "1AAAAAAAAAA    ";
			}
		}

		private void F42Changed(object sender, EventArgs e)
		{
			SetF42();
		}

		private void F42_KeyDown(object sender, KeyEventArgs e)
		{
			if (Keys.Space == e.KeyCode)
			{
				e.SuppressKeyPress = true;
			}
		}


	}
}
