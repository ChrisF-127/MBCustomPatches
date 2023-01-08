using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.ExtraWidgets;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission;

namespace CustomPatches
{
	[HarmonyPatch(typeof(CraftingCampaignBehavior), "IsOpened")]
	internal static class Patch_CraftingCampaignBehavior_IsOpened
	{
		[HarmonyPostfix]
		public static void Postfix(ref bool __result)
		{
			if (CustomPatches.Settings.UnlockAllParts)
				__result = true;
		}
	}

	[HarmonyPatch(typeof(DefaultPartySpeedCalculatingModel), "CalculateBaseSpeedForParty")]
	internal static class Patch_DefaultPartySpeedCalculatingModel_CalculateBaseSpeedForParty
	{
		[HarmonyPostfix]
		public static void Postfix(ref float __result)
		{
			__result *= CustomPatches.Settings.PartySpeedModifier;
		}
	}

	[HarmonyPatch(typeof(CrosshairWidget), "OnUpdate")]
	internal static class Patch_CrosshairWidget_OnUpdate
	{
		[HarmonyPostfix]
		internal static void Postfix(CrosshairWidget __instance)
		{
			foreach (var ic in __instance.Children)
			{
				if (ic is ValueBasedVisibilityWidget v)
				{
					foreach (var vc in v.Children)
					{
						if (vc is BrushWidget bw)
							bw.AlphaFactor = CustomPatches.Settings.CrosshairOpacity;
					}
				}
			}
		}
	}
}
