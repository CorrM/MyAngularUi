using MAU.ReadyElement;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MAU.Example
{
	public class SetupComponent : MauComponent
	{

		#region [ Ui Elements ]

		protected MauMatInput processId;
		protected MauMatSelect unrealVersion;

		#endregion

		public override void InitElements()
		{
			//
			// ProcessId
			//
			processId = new MauMatInput(this, "ProcessId");
			processId.Click += ProcessId_Click;
			processId.Placeholder = "Game Process ID";

			/*
			//
			// UEVersion
			//
			unrealVersion = new MauMatSelect(this, "UEVersion");
			unrealVersion.Click += Btn_Click;
			unrealVersion.SelectionChange += Select_SelectionChange;
			unrealVersion.Options.AddRange(new[] { "CorrM-0", "CorrM-1", "CorrM-2" });
			unrealVersion.UpdateOptions();
			*/


			// Regester all MauElements
			RegisterComponent();
		}

		private void ProcessId_Click(MauElement element, Events.MauEventInfo eventInfo)
		{

		}

		private void Select_SelectionChange(MauElement element, Events.MauEventInfo eventInfo)
		{
			Console.WriteLine(unrealVersion.SelectedOption);
			unrealVersion.SetOption(unrealVersion.Options[0]);
		}

		#region [ Ui Events ]

		private static void Btn_Click(MauElement element, Events.MauEventInfo eventInfo)
		{
			MauMatSelect mauSelect = (MauMatSelect)element;
			Console.WriteLine($"mauSelect => Clicked");
		}

		#endregion
	}
}
