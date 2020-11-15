using System.Collections.Generic;

namespace ELCDotNet
{
	/// <summary>
	/// Description of a driver that can be used to open the ELC
	/// </summary>
	public class USBDriver
	{
		public string Description { get; set; }
		public string DriverName { get; set; }
	}
	public class USBDrivers: List<USBDriver> { }
}
