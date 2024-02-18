using SyUtilityPatches;
using HarmonyLib;
using MCM.Abstractions.Base.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SyUtilityPatches
{
	public class SyUtilityPatches : MBSubModuleBase
	{
		public static MCMSettings Settings { get; private set; } = null;
		public static HarmonyPatches HarmonyPatches { get; private set; } = null;

		private bool isInitialized = false;

		protected override void OnBeforeInitialModuleScreenSetAsRoot()
		{
			base.OnBeforeInitialModuleScreenSetAsRoot();

			if (isInitialized)
				return;
			try
			{
				Settings = GlobalSettings<MCMSettings>.Instance ?? throw new Exception("Settings is null");
				HarmonyPatches = new HarmonyPatches();
				Settings.ApplySettings();
				isInitialized = true;
			}
			catch (Exception exc)
			{
				Message($"{nameof(SyUtilityPatches)}: Initializing Settings failed: {exc.GetType()}: {exc.Message}\n{exc.StackTrace}", false);
			}
		}

		internal static void Message(string s, bool stacktrace = true, Color? color = null, bool log = true)
		{
			try
			{
				if (log)
					FileLog.Log(s + (stacktrace ? $"\n{Environment.StackTrace}" : ""));

				InformationManager.DisplayMessage(new InformationMessage(s, color ?? new Color(1f, 0f, 0f)));
			}
			catch
			{
			}
		}
	}
}
