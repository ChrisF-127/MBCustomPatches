using HarmonyLib;
using SandBox.GameComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace CustomPatches
{
#if false
	[HarmonyPatch(typeof(SandboxAgentApplyDamageModel))]
	internal static class Patch_SandboxAgentApplyDamageModel_DecideMissileWeaponFlags
	{
		[HarmonyPostfix]
		[HarmonyPatch("DecideMissileWeaponFlags")]
		internal static void DecideMissileWeaponFlags(Agent attackerAgent, MissionWeapon missileWeapon, ref WeaponFlags missileWeaponFlags)
		{
			//Helper.Message($"DecideMissileWeaponFlags {attackerAgent?.Name} {missileWeapon} {missileWeaponFlags}", false, Colors.White);
#warning TODO missile pierce
			//missileWeaponFlags |= WeaponFlags.MultiplePenetration;
		}
	}
#endif

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
