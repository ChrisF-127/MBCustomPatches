using HarmonyLib;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.Inventory;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace CustomPatches
{
	[HarmonyPatch(typeof(Crafting), "SwitchToPiece")]
	internal static class Patch_Crafting_SwitchToPiece
	{
		[HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var include = false;
			foreach (var instruction in instructions)
			{
				if (include)
					yield return instruction;
				else if (instruction.opcode == OpCodes.Callvirt
					&& instruction.operand is MethodInfo methodInfo
					&& methodInfo.Name == "SetScale")
					include = true;
			}
		}
	}

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

	[HarmonyPatch(typeof(DefaultSettlementEconomyModel), "GetDailyDemandForCategory")]
	internal static class Patch_DefaultSettlementEconomyModel_GetDailyDemandForCategory
	{
		[HarmonyPostfix]
		public static void Postfix(ref float __result)
		{
			__result *= CustomPatches.Settings.TradeDemandModifier;
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
}
