using System;
using System.Windows.Forms;

namespace TestCheck
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			JSonMainSettings json = new JSonMainSettings();
			Settings settings = json.ReadSettings();
			Config cfg = new Config();
			if (null != settings)
				cfg.Settings = settings;
			switch (cfg.DialogResult = cfg.ShowDialog())
			{
				case DialogResult.OK:
					Application.Run(new TestCheck(cfg.Settings));
					break;
			}
		}
	}
}
