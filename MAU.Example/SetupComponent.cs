using MAU.Core;
using MAU.Events;
using MAU.Helper.Enums;
using MAU.ReadyElement;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using MAU.Helper.Types;
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
		private MauMatButton targetLockBtn;

		#endregion

		public override void InitElements()
		{
			//
			// ProcessId
			//
			processId = new MauMatInput(this, "ProcessId");
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

			//
			// UEVersion
			//
			unrealVersion = new MauMatSelect(this, "UEVersion");
			unrealVersion.Options.AddRange(new[] { "CorrM-0", "CorrM-1", "CorrM-2" });

			//
			// UEConfig
			//
			unrealConfig = new MauMatSelect(this, "UEConfig");
			unrealConfig.SelectionChange += UnrealConfig_SelectionChange;
			unrealConfig.Options.AddRange(new[] { "CorrM-0", "CorrM-1", "CorrM-2" });

			//
			// UEConfig
			//
			targetLockBtn = new MauMatButton(this, "TargetLockBtn");
			targetLockBtn.Click += TargetLockBtn_Click;

			//
			// TargetInfo
			//
			targetInfoTbl = new MauTable(this, "TargetInfoTbl");
			targetInfoTbl.Content.Columns.AddRange(new []
			{
				"",
				""
			});
			targetInfoTbl.Content.Rows.AddRange(new[]
			{
				new[] { "Window Name", "Bad Name" },
				new[] { "Process ID", "0x00" },
				new[] { "Game Arch", "64" },
				new[] { "Exe Name", "UFT.exe" },
				new[] { "Modules Count", "12" },
				new[] { "Unreal Version", "4.24" },
				new[] { "Anti Cheat", "UnKnown" }
			});

			// Register all MauElements
			RegisterComponent();
		}

		#region [ Mau Events ]

		private void UnrealConfig_SelectionChange(MauElement element, MauEventInfo eventInfo)
		{
			
		}
		private void TargetLockBtn_Click(MauElement element, MauEventInfo eventInfo)
		{
			processId.Disabled = !processId.Disabled;
		}
		private void ProcessAutoFind_Click(MauElement element, MauEventInfo eventInfo)
		{
			Console.WriteLine(processAutoFind.Disabled);
		}

		#endregion
	}
}
