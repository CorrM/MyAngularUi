﻿using MAU.ReadyElement;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MAU.Example
{
	public class SetupComponent : MauComponent
	{

		#region [ Ui Elements ]

		protected MauMatSelect select;

		#endregion

		public override void InitElements()
		{
			// FirstSelect
			select = new MauMatSelect(this, "FirstSelect");
			select.Click += Btn_Click;
			select.SelectionChange += Select_SelectionChange;
			select.Options.AddRange(new[] { "CorrM-0", "CorrM-1", "CorrM-2" });
			select.UpdateOptions();

			// Regester all MauElements
			RegisterComponent();
		}

		private void Select_SelectionChange(MauElement element, Events.MauEventInfo eventInfo)
		{
			
		}

		#region [ Ui Events ]

		private static void Btn_Click(MauElement element, Events.MauEventInfo eventInfo)
		{
			MauMatSelect mauSelect = (MauMatSelect)element;
			Console.WriteLine(mauSelect.SelectedOption);
		}

		#endregion
	}
}