using COMMON;

namespace TestCheck
	{
	public class Settings
		{
		public Configuration Gateway
			{
			get => _settings;
			set
				{
				switch (value)
					{
					case Configuration.TransaxLyra:
					case Configuration.FNCI:
					case Configuration.Libre:
						_settings = value;
						break;
					default:
						_settings = Configuration.Libre;
						break;
					}
				}
			}
		private Configuration _settings = Configuration.TransaxLyra;
		}
	public class JSonMainSettings: CJson<Settings> { public JSonMainSettings() : base(@".\main.settings.json") { } }
	}
