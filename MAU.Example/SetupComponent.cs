using MAU.Core;
using MAU.Events;
using MAU.ReadyElement;
using System;
using MAU.ReadyElement.Angular;
using Newtonsoft.Json.Linq;

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
		private MauMatSelectionList finderGObjectsList;

		#endregion

		protected override void InitElements()
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
			// TargetLockBtn
			//
			targetLockBtn = new MauMatButton(this, "TargetLockBtn");
			targetLockBtn.Click += TargetLockBtn_Click;

			//
			// TargetInfo
			//
			targetInfoTbl = new MauTable(this, "TargetInfoTbl");
			targetInfoTbl.Content.Columns.AddRange(new[]
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

			//
			// FinderGObjectsList
			//
			finderGObjectsList = new MauMatSelectionList(this, "FinderGObjectsList");
			finderGObjectsList.Click += FinderGObjectsList_Click;
			finderGObjectsList.SelectionChange += FinderGObjectsList_SelectionChange;
			finderGObjectsList.Options.AddRange(new[] { "CorrM0", "CorrM1" });
		}

		private void FinderGObjectsList_SelectionChange(MauElement element, MauEventInfo eventInfo)
		{
			string gg = eventInfo.Data["option"]!["_value"]!.Value<string>();
		}

		private void FinderGObjectsList_Click(MauElement element, MauEventInfo eventInfo)
		{

		}

		#region [ Mau Events ]

		private void ProcessId_InputChange(MauElement element, MauEventInfo eventInfo)
		{
			if (processId.Value.StartsWith("0x"))
				processId.SetStyle("color", "red");
			else
				processId.RemoveStyle("color");
		}
		private void UnrealConfig_SelectionChange(MauElement element, MauEventInfo eventInfo)
		{

		}
		private void TargetLockBtn_Click(MauElement element, MauEventInfo eventInfo)
		{
			unrealVersion.Open();
		}
		private void ProcessAutoFind_Click(MauElement element, MauEventInfo eventInfo)
		{
			Console.WriteLine(processAutoFind.Disabled);
		}

		#endregion

	}
}
