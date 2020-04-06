using MAU.Core;
using MAU.Events;
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

		#region [ Mau Elements ]

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
			processAutoFind.Click += ProcessAutoFind_Click;

			//
			// UEVersion
			//
			unrealVersion = new MauMatSelect(this, "UEVersion");
			unrealVersion.OpenedChange += UnrealVersion_OpenedChange;
			unrealVersion.SelectionChange += UnrealVersion_SelectionChange;
			unrealVersion.Options.AddRange(new[] { "CorrM-0", "CorrM-1", "CorrM-2" });
			unrealVersion.UpdateOptions();

			// Regester all MauElements
			RegisterComponent();

			processAutoFind.Test();
		}

		private void ProcessAutoFind_Click(MauElement element, MauEventInfo eventInfo)
		{
			Console.WriteLine(processAutoFind.Disabled);
			processAutoFind.Color = Helper.Types.ThemePalette.Warn;
		}

		#region [ Mau Events ]

		private void UnrealVersion_OpenedChange(MauElement element, MauEventInfo eventInfo)
		{
			Console.WriteLine(unrealVersion.PanelOpen);
		}

		private void ProcessId_InputChange(MauElement element, MauEventInfo eventInfo)
		{
			unrealVersion.Toggle();
		}

		private void UnrealVersion_SelectionChange(MauElement element, MauEventInfo eventInfo)
		{
			Console.WriteLine(unrealVersion.SelectedOption);
		}

		#endregion
	}
}
