using MAU.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAU.Helper.Enums
{
	public enum CheckBoxLabelPosition
	{
		[MauEnumMember("")]
		NotSet,

		[MauEnumMember("before")]
		Before,

		[MauEnumMember("after")]
		After
	}
}
