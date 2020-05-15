using MAU.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAU.Helper.Enums
{
	public enum MatTabHeaderPosition
	{
		[MauEnumMember("")]
		NotSet,

		[MauEnumMember("above")]
		Above,

		[MauEnumMember("below")]
		Below
	}
}
