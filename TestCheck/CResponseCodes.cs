using System;
using System.Collections.Generic;

namespace TestCheck
{
	public enum Decisions
	{
		OK,
		KO,
		Referral,
	}
	public class CResponseCodes: Dictionary<string, string>
	{
		#region constructor
		public CResponseCodes() : base(StringComparer.CurrentCultureIgnoreCase) { }
		#endregion
	}
}
