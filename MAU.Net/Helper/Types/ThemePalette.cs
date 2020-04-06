using MAU.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAU.Helper.Types
{
	public enum ThemePalette
	{
		[MauEnumMember(0)]
		NoSet,

		[MauEnumMember("primary")]
		Primary,

		[MauEnumMember("accent")]
		Accent,

		[MauEnumMember("warn")]
		Warn
	}
}
