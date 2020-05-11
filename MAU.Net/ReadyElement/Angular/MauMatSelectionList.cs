using System.Collections.Generic;
using MAU.Attributes;
using MAU.Core;
using MAU.Helper.Enums;
using MAU.Helper.Types;
using static MAU.Attributes.MauProperty;
using static MAU.Events.MauEventHandlers;
using static MAU.Attributes.MauMethod;

namespace MAU.ReadyElement.Angular
{
	public class MauMatSelectionList : MauElement
	{
		#region [ Mau Events ]

		[MauEvent("selectionChange")]
		public event MauEventHandler SelectionChange;

		#endregion

		#region [ Mau Variable ]

		[MauVariable]
		public MauDataList<string> Options { get; private set; }

		#endregion

		#region [ Mau Properties ]

		[MauProperty("color", MauPropertyType.ComponentProperty)]
		public ThemePalette Color { get; set; }

		[MauProperty("disableRipple", MauPropertyType.ComponentProperty)]
		public bool DisableRipple { get; set; }

		[MauProperty("disabled", MauPropertyType.ComponentProperty)]
		public bool Disabled { get; set; }

		[MauProperty("multiple", MauPropertyType.ComponentProperty)]
		public bool Multiple { get; set; }

		#endregion

		#region [ Mau Methods ]

		[MauMethod("deselectAll", MauMethodType.ComponentMethod)]
		public void DeselectAll() { }

		[MauMethod("focus", MauMethodType.ComponentMethod)]
		public void Focus() { }

		[MauMethod("selectAll", MauMethodType.ComponentMethod)]
		public void SelectAll() { }

		#endregion

		public MauMatSelectionList(MauComponent parentComponent, string mauId) : base(parentComponent, mauId)
		{
			Options = new MauDataList<string>(this, nameof(Options));
		}

		#region [ Options Controlling ]

		//public bool SelectOption(string newOption)
		//{
		//	if (!Options.Contains(newOption))
		//		return false;

		//	SelectedOption = newOption;
		//	return true;
		//}
		//public bool SelectOption(int newOptionIndex)
		//{
		//	if (Options.Count >= newOptionIndex)
		//		return false;

		//	SelectedOption = Options[newOptionIndex];
		//	return true;
		//}

		#endregion
	}
}
