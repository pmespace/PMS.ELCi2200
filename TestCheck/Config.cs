using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestCheck
	{
	public partial class Config: Form
		{
		internal Settings Settings { get; set; }

		internal Config()
			{
			InitializeComponent();
			Settings = new Settings();
			}

		private void pbCancel_Click(object sender, EventArgs e)
			{
			DialogResult = DialogResult.Cancel;
			}

		private void pbAccept_Click(object sender, EventArgs e)
			{
			JSonMainSettings json = new JSonMainSettings();
			json.WriteSettings(Settings);
			DialogResult = DialogResult.OK;
			}

		private void Config_Load(object sender, EventArgs e)
			{
			switch (Settings.Gateway)
				{
				case Configuration.TransaxLyra:
					rbTransaxLyra.Checked = true;
					break;
				case Configuration.FNCI:
					rbFNCI.Checked = true;
					break;
				default:
					rbFree.Checked = true;
					break;
				}
			}

		private void checkedChanged(object sender, EventArgs e)
			{
			if (rbTransaxLyra.Checked)
				Settings.Gateway = Configuration.TransaxLyra;
			else if (rbFNCI.Checked)
				Settings.Gateway = Configuration.FNCI;
			else if (rbFree.Checked)
				Settings.Gateway = Configuration.Libre;
			}
		}
	}
