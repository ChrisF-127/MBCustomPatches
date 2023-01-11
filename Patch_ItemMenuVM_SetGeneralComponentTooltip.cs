using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.Inventory;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace CustomPatches
{
	[HarmonyPatch(typeof(ItemMenuVM), "SetGeneralComponentTooltip")]
	internal static class Patch_ItemMenuVM_SetGeneralComponentTooltip
	{
		[HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
		{
			var applied = false;
			var list = new List<CodeInstruction>(instructions);

			//  0    call ItemMenuTooltipPropertyVM ItemMenuVM::CreateColoredProperty(...)
			//  1    pop NULL
			//  2 -- ldloc.0 NULL[Label13]
			//    ++ ldarg.0 NULL[Label13]
			//    ++ ldarg.0 NULL
			//    ++ ldfld ItemVM ItemMenuVM::_targetItem
			//    ++ call static Void Patch_ItemMenuVM_SetGeneralComponentTooltip::CreateCultureProperty(...)
			//    ++ ldloc.0 NULL
			//  3    ldc.r4 1
			//  4    stfld Single <> c__DisplayClass108_0::equipmentWeightMultiplier
			//  5    ldarg.0 NULL
			//  6    ldfld BasicCharacterObject ItemMenuVM::_character
			//  7    isinst CharacterObject
			//  8 -- dup NULL
			//  9    stloc.1 NULL
			//    ++ ldloc.1 NULL
			// 10    brfalse.s Label16
			// 11    ldloc.1 NULL
			for (int i = 0; i < list.Count - 9; i++)
			{
				if (list[i + 0].opcode == OpCodes.Call
					&& list[i + 0].operand is MethodInfo mi && mi == CreateColoredPropertyMethodInfo
					&& list[i + 1].opcode == OpCodes.Pop
					&& list[i + 2].opcode == OpCodes.Ldloc_0
					// -- insert here --
					&& list[i + 3].opcode == OpCodes.Ldc_R4
					&& list[i + 4].opcode == OpCodes.Stfld
					// [...]
					&& list[i + 8].opcode == OpCodes.Dup
					&& list[i + 9].opcode == OpCodes.Stloc_1)
				{
					// replace opcode
					list[i + 2].opcode = OpCodes.Ldarg_0;

					// insert new code
					list.Insert(i++ + 3, new CodeInstruction(OpCodes.Ldarg_0));
					list.Insert(i++ + 3, new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ItemMenuVM), "_targetItem")));
					list.Insert(i++ + 3, new CodeInstruction(OpCodes.Ldarg_0));
					list.Insert(i++ + 3, new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ItemMenuVM), "_character")));
					list.Insert(i++ + 3, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Patch_ItemMenuVM_SetGeneralComponentTooltip), "CreateCultureProperty")));
					list.Insert(i++ + 3, new CodeInstruction(OpCodes.Ldloc_0));

					// replace opcode and swap places
					list[i + 8].opcode = OpCodes.Ldloc_1;
					(list[i + 9], list[i + 8]) = (list[i + 8], list[i + 9]);

					applied = true;
					break;
				}
			}

			if (!applied)
				CustomPatches.Message($"{nameof(CustomPatches)}: failed to apply {nameof(Patch_ItemMenuVM_SetGeneralComponentTooltip)}");
			return list;
		}

		// parameters:
		//	MBBindingList<ItemMenuTooltipPropertyVM> targetList,
		//	string definition,
		//	string value,
		//	Color color,
		//	int textHeight = 0,
		//	HintViewModel hint = null,
		//	TooltipProperty.TooltipPropertyFlags propertyFlags = TooltipProperty.TooltipPropertyFlags.None
		private static readonly MethodInfo CreateColoredPropertyMethodInfo = AccessTools.Method(typeof(ItemMenuVM), "CreateColoredProperty");
		public static void CreateCultureProperty(ItemMenuVM __instance, ItemVM _targetItem, BasicCharacterObject _character)
		{
			try
			{
				if (Game.Current.IsDevelopmentMode)
					return;

				var culture = _targetItem?.ItemRosterElement.EquipmentElement.Item?.Culture;
				var cultureName = culture?.Name?.ToString();
				if (string.IsNullOrWhiteSpace(cultureName))
					return;

				var characterCulture = _character is CharacterObject co ? co.Culture : _character?.Culture;
				var color = characterCulture != null ? characterCulture == culture ? UIColors.PositiveIndicator : UIColors.NegativeIndicator : new Color(0f, 0f, 0f, 0f);

				CreateColoredPropertyMethodInfo.Invoke(
					__instance,
					new object[]
					{
						__instance.TargetItemProperties,
						"Culture: ",
						cultureName,
						color,
						0,
						null,
						TooltipProperty.TooltipPropertyFlags.None,
					});
			}
			catch (Exception exc)
			{
				CustomPatches.Message($"{nameof(CustomPatches)}.{nameof(CreateCultureProperty)}: caused an exception: {exc.GetType()}: {exc.Message}");
			}
		}
	}
}
