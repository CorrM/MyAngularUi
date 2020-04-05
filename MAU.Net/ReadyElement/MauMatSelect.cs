using MAU.Attributes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static MAU.Attributes.MauProperty;
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

		[MauProperty("value", MauPropertyType.ComponentProperty)]
		public string SelectedOption { get; private set; }

		[MauProperty("disabled", MauPropertyType.ComponentProperty)]
		public bool Disabled { get; set; }

		#endregion

		public MauMatSelect(MauComponent parentComponent, string mauId) : base(parentComponent, mauId)
		{
			Options = new List<string>();
		}

		#region [ MatSelect Options ]

		public void UpdateOptions()
		{
			MauVariable.UpdateVar(this, nameof(Options));
		}
		public bool SetOption(string newOption)
		{
			if (!Options.Contains(newOption))
				return false;

			SelectedOption = newOption;
			return true;
		}
		public bool SetOption(int newOptionIndex)
		{
			if (Options.Count >= newOptionIndex)
				return false;

			SelectedOption = Options[newOptionIndex];
			return true;
		}

		#endregion

	}
}
