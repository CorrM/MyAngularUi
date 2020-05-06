using MAU.Core;
using MAU.Events;
using MAU.Helper.Enums;
using MAU.ReadyElement;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MAU.ReadyElement.Angular;

namespace MAU.Example
{
	public class SetupComponent : MauComponent
	{
		#region [ Mau Elements ]

		private MauMatInput processId;
		private MauMatButton processAutoFind;
		private MauMatSlideToggle useKernel;
		private MauMatSelect unrealVersion;
		private MauMatSelect unrealConfig;
		private MauTable targetInfoTbl;

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
			// UseKernel
			//
			useKernel = new MauMatSlideToggle(this, "UseKernel");
			useKernel.Change += UseKernel_Change;

			//
			// UEVersion
			//
			unrealVersion = new MauMatSelect(this, "UEVersion");
			unrealVersion.SelectionChange += UnrealVersion_SelectionChange;
			unrealVersion.Options.AddRange(new[] { "CorrM-0", "CorrM-1", "CorrM-2" });
			unrealVersion.UpdateOptions();

			//
			// UEConfig
			//
			unrealConfig = new MauMatSelect(this, "UEConfig");
			unrealConfig.SelectionChange += UnrealConfig_SelectionChange;
			unrealConfig.Options.AddRange(new[] { "CorrM-0", "CorrM-1", "CorrM-2" });
			unrealConfig.UpdateOptions();

			//
			// TargetInfo
			//
			targetInfoTbl = new MauTable(this, "TargetInfoTbl");
			//targetInfoTbl.Options.AddRange(new[] { "CorrM-0", "CorrM-1", "CorrM-2" });
			//targetInfoTbl.UpdateOptions();

			// Register all MauElements
			RegisterComponent();
		}


		#region [ Mau Events ]

		private void UnrealConfig_SelectionChange(MauElement element, MauEventInfo eventInfo)
		{
			
		}
		private void UseKernel_Change(MauElement element, MauEventInfo eventInfo)
		{
			Console.WriteLine(useKernel.Checked);
		}
		private void ProcessAutoFind_Click(MauElement element, MauEventInfo eventInfo)
		{
			Console.WriteLine(processAutoFind.Disabled);
		}
		private void ProcessId_InputChange(MauElement element, MauEventInfo eventInfo)
		{

		}
		private void UnrealVersion_SelectionChange(MauElement element, MauEventInfo eventInfo)
		{
			Console.WriteLine(unrealVersion.SelectedOption);
		}

		#endregion
	}
}
