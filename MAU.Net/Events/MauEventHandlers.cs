using MAU.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAU.Events
{
	public class MauEventHandlers
	{
		public delegate void MauEventHandler(MauComponent component, MauEventInfo eventInfo);
	}
}
