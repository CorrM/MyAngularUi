using MAU.Attributes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static MAU.Events.MauEventHandlers;

namespace MAU.ReadyElement
{
	public class MauMatSelect : MauElement
	{
		#region [ Events ]

		[MauEvent("selectionChange")]
		public event MauEventHandler SelectionChange;

		[MauEvent("valueChange")]
		protected event MauEventHandler ValueChange;

		#endregion

		#region [ Public Props ]

		[MauVariable]
		public List<string> Options { get; private set; }

		public string SelectedOption { get; private set; }

		#endregion

		public MauMatSelect(MauComponent parentComponent, string id) : base(parentComponent, id)
		{
			Options = new List<string>();
			ValueChange += MauMatSelect_ValueChange;
		}

		private void MauMatSelect_ValueChange(MauElement element, Events.MauEventInfo eventInfo)
		{
			SelectedOption = eventInfo.Data["value"].Value<string>();
		}

		public void UpdateOptions()
		{
			MauVariable.UpdateVar(this, nameof(Options));
		}
	}
}
