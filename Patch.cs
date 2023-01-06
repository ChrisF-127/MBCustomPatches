using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.ViewModelCollection.Inventory;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Order;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order;

namespace CustomPatches
{

	//[HarmonyPatch(typeof(MissionOrderVM))]
	//internal static class Patch
	//{
	//	[HarmonyPrefix]
	//	[HarmonyPatch("ApplySelectedOrder")]
	//	internal static void Method(MissionOrderVM __instance)
	//	{
	//		Helper.Message($"ApplySelectedOrder {__instance.LastSelectedOrderItem?.TooltipText}");
	//	}
	//}

	//[HarmonyPatch(typeof(MBAgentVisuals))]
	//internal static class PatchMBAgentVisuals
	//{
	//	[HarmonyPostfix]
	//	[HarmonyPatch("SetContourColor")]
	//	internal static void SetContourColor(MBAgentVisuals __instance)
	//	{
	//		Helper.Message($"SetContourColor {__instance.GetEntity()?.Name}");
	//	}
	//}


	//[HarmonyPatch(typeof(OrderTroopPlacer))]
	//internal static class Patch2
	//{
	//	[HarmonyPostfix]
	//	[HarmonyPatch("UpdateFormationDrawingForMovementOrder")]
	//	internal static void Method(bool giveOrder)
	//	{
	//		if (giveOrder)
	//			Helper.Message($"UpdateFormationDrawingForMovementOrder");
	//	}
	//}
}
