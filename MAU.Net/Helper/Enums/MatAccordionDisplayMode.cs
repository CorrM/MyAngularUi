﻿using MAU.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAU.Helper.Enums
{
	public enum MatAccordionDisplayMode
	{
		[MauEnumMember("")]
		NoSet,

		[MauEnumMember("default")]
		Default,

		[MauEnumMember("flat")]
		Flat
	}
}
