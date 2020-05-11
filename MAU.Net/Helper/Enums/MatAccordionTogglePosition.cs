using MAU.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAU.Helper.Enums
{
	public enum MatAccordionTogglePosition
	{
		[MauEnumMember("")]
		NotSet,

		[MauEnumMember("before")]
		Before,

		[MauEnumMember("after")]
		After
	}
}
