using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using COMMON;
using CHPN;
using ELCDotNet;

namespace TestCheck
{
	public partial class TestCheck : Form
	{
		private string path = @".\";
		private const string SETTINGS = @"testcheck.settings";
		private const string NETWORK_SETTINGS = @"testcheck.settings.network";
		private const string TRANSAX_SETTINGS = @".transax";
		private const string LYRA_SETTINGS = @".lyra";
		private const string FNCI_SETTINGS = @".fnci";
		private const string EXTENSION = @".json";
		private const string NEXO_SETTINGS = @"testcheck.settings.nexo" + EXTENSION;
		private int CheckIndex = 0;

		private string settingsFile = SETTINGS;
		private string networkSettingsFile = NETWORK_SETTINGS;

		private ELC Elc = new ELC();
		private CCheck Check = null;
		private CRecord theRecord = null;
		private Settings currentSettings = null;

		private const string CRLF = "\r\n";
		private TypeDePieceIdentite NoIDCard;

		public TestCheck(Settings settings)
		{
			InitializeComponent();
			ChangeConfig(settings);
		}

		private void SetConfig()
		{
			Config cfg = new Config();
			cfg.Settings = this.currentSettings;
			switch (cfg.DialogResult = cfg.ShowDialog())
			{
				case DialogResult.OK:
					ChangeConfig(cfg.Settings);
					break;
			}
		}

		private void ChangeConfig(Settings settings)
		{
			path = (string)Registry.GetValue(@"HKEY_CURRENT_USER\Software\CHECK\TestCheck", "Settings", @".\");
			if (string.IsNullOrEmpty(path))
				path = @".\";
			this.currentSettings = settings;
			string s = "TestCheck";
			settingsFile = SETTINGS;
			networkSettingsFile = NETWORK_SETTINGS;
			rbGarantie.Checked = true;
			rbGarantie.Enabled = rbFNCI.Enabled = true;
			switch (settings.Gateway)
			{
				case Configuration.TransaxLyra:
					settingsFile += TRANSAX_SETTINGS;
					networkSettingsFile += LYRA_SETTINGS;
					s += TRANSAX_SETTINGS + LYRA_SETTINGS;
					rbFNCI.Enabled = true;
					rbGarantie.Checked = rbGarantie.Enabled = true;
					break;
				case Configuration.FNCI:
					settingsFile += FNCI_SETTINGS;
					networkSettingsFile += FNCI_SETTINGS;
					s += FNCI_SETTINGS;
					rbGarantie.Enabled = false;
					rbFNCI.Checked = rbFNCI.Enabled = true;
					break;
			}
			Text = s;
			settingsFile += EXTENSION;
			settingsFile = path + settingsFile;
			networkSettingsFile += EXTENSION;
			networkSettingsFile = path + networkSettingsFile;
			Check = new CCheck();
			Check.OpenSettings(settingsFile, networkSettingsFile);
			Text = "CHECK CHPN NEXO - " + Check.SettingsFileName + " - " + Check.NetworkSettingsFileName;
			rbTest.Checked = true;

			F43_POS5.Items.Add(Check.Chpn.TypeDePieceIdentiteAsString(TypeDePieceIdentite.F43_POS5_CARTE_IDENTITE_NATIONALE));
			F43_POS5.Items.Add(Check.Chpn.TypeDePieceIdentiteAsString(TypeDePieceIdentite.F43_POS5_PERMIS_CONDUIRE_NATIONAL));
			F43_POS5.Items.Add(Check.Chpn.TypeDePieceIdentiteAsString(TypeDePieceIdentite.F43_POS5_PASSEPORT_NATIONAL));
			F43_POS5.Items.Add(Check.Chpn.TypeDePieceIdentiteAsString(TypeDePieceIdentite.F43_POS5_CARTE_RESIDENT));
			F43_POS5.Items.Add(Check.Chpn.TypeDePieceIdentiteAsString(TypeDePieceIdentite.F43_POS5_CARTE_SEJOUR));
			F43_POS5.Items.Add(Check.Chpn.TypeDePieceIdentiteAsString(TypeDePieceIdentite.F43_POS5_CARTE_IDENTITE_EUROPEENE));
			F43_POS5.Items.Add(Check.Chpn.TypeDePieceIdentiteAsString(TypeDePieceIdentite.F43_POS5_PERMIS_CONDUIRE_EUROPEEN));
			F43_POS5.Items.Add(Check.Chpn.TypeDePieceIdentiteAsString(TypeDePieceIdentite.F43_POS5_PASSEPORT_EUROPEEN));
			F43_POS5.Items.Add(Check.Chpn.TypeDePieceIdentiteAsString(TypeDePieceIdentite.F43_POS5_AUTRE));
			NoIDCard = (TypeDePieceIdentite)F43_POS5.Items.Add("AUCUNE");
			F43_POS5.SelectedIndex = (int)NoIDCard;

			F2.Text = Check.Settings.F2;
			F4.Text = Check.Settings.F4;
			F43_POS1.Text = Check.Settings.F43_POS1TO4;
			F43_POS5.SelectedIndex = (int)(TypeDePieceIdentite.F43_POS5_BEGIN <= Check.Settings.F43_POS5 && TypeDePieceIdentite.F43_POS5_END >= Check.Settings.F43_POS5 ? Check.Settings.F43_POS5 - 1 : NoIDCard);
			F43_POS6.Text = Check.Settings.F43_POS6TO9;
			F43_POS10.Checked = (TypeDeCheque.F43_POS10_CHEQUE_PERSONNEL == Check.Settings.F43_POS10 ? true : false);
			F2.Text = Check.Settings.F2;

			SetButtons();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			SetButtons();
		}

		private void Reset()
		{
			theRecord = null;
			log.Clear();
			cbcom.Text = F43.Text = F38.Text = F40.Text = F40Ex.Text = F39.Text = F39Ex.Text = F44_MSG.Text = F44_SIGNATURE.Text = F44_COMPTEUR.Text = string.Empty;
		}

		private void SetButtons()
		{
			pbReadCheck.Enabled = pbWriteCheck.Enabled = Elc.Opened;
			pbSendIPDU.Enabled = (0 != CMisc.StrToLong(F4.Text)) && (0 == F43_POS1.Text.Length || F43_POS1.MaskCompleted) && (0 == F43_POS6.Text.Length || F43_POS6.MaskCompleted);
		}

		private void sendIPDU_Click(object sender, EventArgs e)
		{
			Reset();
			// pas obligatoire
			Check.Chpn.F02_Input = F2.Text;
			Check.Chpn.F43_InputGuaranteeBirthDate = (F43_POS1.Enabled ? F43_POS1.Text : string.Empty);
			Check.Chpn.F43_InputGuaranteeIDType = (TypeDePieceIdentite)((int)TypeDePieceIdentite.F43_POS5_BEGIN <= F43_POS5.SelectedIndex + 1 && (int)TypeDePieceIdentite.F43_POS5_END >= F43_POS5.SelectedIndex + 1 ? F43_POS5.SelectedIndex + 1 : (int)TypeDePieceIdentite.F43_POS5_NONE);
			Check.Chpn.F43_InputGuaranteeIDTypeDate = (F43_POS6.Enabled ? F43_POS6.Text : string.Empty);
			Check.Chpn.F43_InputGuaranteeCheckType = (F43_POS10.Enabled ? (F43_POS10.Checked ? TypeDeCheque.F43_POS10_CHEQUE_PERSONNEL : TypeDeCheque.F43_POS10_CHEQUE_SOCIETE) : TypeDeCheque.F43_POS10_NONE);

			// OBLIGATOIRE
			Check.Chpn.F04_InputAmount = uint.Parse(F4.Text);
			if (string.IsNullOrEmpty(cmc7.Text))
				Check.Chpn.F35_InputCMC7 = (rbTest.Checked ? "D0000001D800000000909F000000000000B" : "D1943749D075000004908F089000075641B");
			else
				Check.Chpn.F35_InputCMC7 = cmc7.Text;

			Check.ServiceToUse = (rbGarantie.Checked ? Service.GarantieCheque : Service.InterrogationFNCI);
			theRecord = Check.ProcessMessage();
			if (null != theRecord)
			{
				if (AuthorisationType.offline != theRecord.authorisationType)
				{
					if (null != theRecord.Request)
					{
						log.Text = Check.Chpn.DescribeMessage("REQUEST", theRecord.Request);
						if (null != theRecord.Reply)
						{
							log.Text += CRLF + Check.Chpn.DescribeMessage("REPLY", theRecord.Reply);
							cbcom.Text = Encoding.UTF8.GetString(theRecord.Reply.GetPI(0x01).Data);
							cbcomdescription.Text = Check.Chpn.CBCOMResultAsString(theRecord.Reply.GetPI(0x01));
							F11.Text = Check.Chpn.F11_OutputTransactionID.ToString();
							F07.Text = Check.Chpn.F07_Output;
							F43.Text = Check.Chpn.F43_OutputGuaranteeMessage;
							F38.Text = Check.Chpn.F38_OutputGuaranteeSignature;
							F40.Text = Check.Chpn.F40_OutputGuaranteeResponseCode;
							F40Ex.Text = Check.Chpn.F40_OutputGuaranteeResponseCodeAsString;
							F39.Text = Check.Chpn.F39_OutputFnciResponseCode;
							F39Ex.Text = Check.Chpn.F39_OutputFnciResponseCodeAsString;
							F44_MSG.Text = Check.Chpn.F44_OutputFnciMessage;
							F44_SIGNATURE.Text = Check.Chpn.F44_OutputFnciSignature;
							F44_COMPTEUR.Text = Check.Chpn.F44_OutputFnciCounterIntraDay.ToString();
						}
						else
							log.Text += CRLF + "**********" + CRLF + "Aucune donnée reçue. Veuillez consulter le fichier de log " + Check.LogFileName + ".";
					}
					else
						log.Text += CRLF + "**********" + CRLF + "Impossible d'envoyer les données. Veuillez consulter le fichier de log " + Check.LogFileName + ".";
				}
				else
					log.Text += CRLF + "Transaction traitée offline";
			}
			else
				log.Text += CRLF + "**********" + CRLF + "Erreur lors du traitement de la transaction. Veuillez consulter le fichier de log " + Check.LogFileName + ".";
		}

		private void hasChanged(object sender, EventArgs e)
		{
			if (sender is MaskedTextBox)
			{
				MaskedTextBox m = (MaskedTextBox)sender;
				if (string.IsNullOrEmpty(m.Text) || m.MaskCompleted)
					m.BackColor = SystemColors.Window;
				else
					m.BackColor = Color.LightYellow;
			}
			SetButtons();
		}

		private void pbSettings_Click(object sender, EventArgs e)
		{
			Check.DisplaySettings();
		}

		private void pbNetworkSettings_Click(object sender, EventArgs e)
		{
			Check.DisplayNetworkSettings();
		}

		private void F43_POS5_SelectedIndexChanged(object sender, EventArgs e)
		{
			F43_POS1.Enabled = F43_POS6.Enabled = F43_POS10.Enabled = (F43_POS5.SelectedIndex != (int)NoIDCard);
		}

		private void button1_Click(object sender, EventArgs e)
		{
			SetConfig();
		}

		private void pbReadCheck_Click(object sender, EventArgs e)
		{
			string raw = null, chpn = null;
			bool docinside = false;
			if (Elc.Read(ref raw, ref chpn, ref docinside, 15))
			{
			cmc7.Text = chpn;
			}
			log.Text = "Resultat: " + Elc.LastAsyncResult.ToString() + " - Raw: " + raw + " - CHPN: " + chpn + " - Doc dedans: " + docinside.ToString();
		}

		private void rbGarantie_CheckedChanged(object sender, EventArgs e)
		{

		}

		private void rbTest_CheckedChanged(object sender, EventArgs e)
		{
			cmc7.Text = rbTest.Checked ? "D0000001D800000000909F000000000000B" : "D1943749D075000004908F089000075641B";
		}

		private void pbWriteCheck_Click(object sender, EventArgs e)
		{
			ObjectToPrint o = new ObjectToPrint()
			{
				MerchantName = "Mon commerçant",
				MerchantAddress = "mon adresse",
				Amount = uint.Parse(F4.Text),
				Index = CheckIndex++,
				ResponseCode = "RC",
				Seal = "mon sceau",
				Type = PrintType.garantie
			};
			string printed = null;
			Elc.Write(o, ref printed, 15);
			log.Text = "Resultat: " + Elc.LastAsyncResult.ToString() + " - Printed: " + printed;
		}

		private void pbOpen_Click(object sender, EventArgs e)
		{
			Elc.OpenWithDrivers("testcheck.elcdrivers.json");
			SetButtons();
		}
	}
}