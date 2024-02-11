using CustomPatches;
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

namespace CustomPatches
{
	public class CustomPatches : MBSubModuleBase
	{
		public static MCMSettings Settings { get; private set; }
		public static HarmonyPatches HarmonyPatches { get; private set; }

		private bool isInitialized = false;

		protected override void OnBeforeInitialModuleScreenSetAsRoot()
		{
			base.OnBeforeInitialModuleScreenSetAsRoot();

			if (isInitialized)
				return;
			try
			{
				HarmonyPatches = new HarmonyPatches();
				Settings = GlobalSettings<MCMSettings>.Instance ?? throw new Exception("Settings is null");
				Settings.ApplySettings();
				isInitialized = true;
			}
			catch (Exception)
			{
				Message($"{nameof(CustomPatches)}: Initializing Settings failed!");
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
