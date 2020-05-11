using MAU.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAU.Helper.Enums
{
	public enum ThemePalette
	{
		[MauEnumMember("")]
		NotSet,

		[MauEnumMember("primary")]
		Primary,

		[MauEnumMember("accent")]
		Accent,

		[MauEnumMember("warn")]
		Warn
	}
}
