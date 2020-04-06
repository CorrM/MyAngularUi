using MAU.Core;
using MAU.ReadyElement;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MAU.Example
{
	public class SetupComponent : MauComponent
	{

		/// <summary>
		/// ToDo: Change logic to get prop value when call `MauProperty` property
		/// </summary>

		#region [ Ui Elements ]

		protected MauMatInput processId;
		protected MauMatButton processAutoFind;
		protected MauMatSelect unrealVersion;

		#endregion

		public override void InitElements()
		{
			//
			// ProcessId
			//
			processId = new MauMatInput(this, "ProcessId");
			processId.InputChange += ProcessId_InputChange;
			processId.Placeholder = "Game Process ID";

			//
			// ProcessAutoFind
			//
			processAutoFind = new MauMatButton(this, "ProcessAutoFind");

			//
			// UEVersion
			//
			unrealVersion = new MauMatSelect(this, "UEVersion");
			unrealVersion.Click += Btn_Click;
			unrealVersion.OpenedChange += UnrealVersion_OpenedChange;
			unrealVersion.SelectionChange += unrealVersion_SelectionChange;
			unrealVersion.Options.AddRange(new[] { "CorrM-0", "CorrM-1", "CorrM-2" });
			unrealVersion.UpdateOptions();

			// Regester all MauElements
			RegisterComponent();
		}

		private void UnrealVersion_OpenedChange(MauElement element, Events.MauEventInfo eventInfo)
		{
			Console.WriteLine(unrealVersion.PanelOpen);
		}

		private void ProcessId_InputChange(MauElement element, Events.MauEventInfo eventInfo)
		{
			unrealVersion.Toggle();
		}

		private void unrealVersion_SelectionChange(MauElement element, Events.MauEventInfo eventInfo)
		{
			Console.WriteLine(unrealVersion.SelectedOption);
		}

		#region [ Ui Events ]

		private void Btn_Click(MauElement element, Events.MauEventInfo eventInfo)
		{
			MauMatSelect mauSelect = (MauMatSelect)element;
			Console.WriteLine($"mauSelect => Clicked");
		}

		#endregion
	}
}
