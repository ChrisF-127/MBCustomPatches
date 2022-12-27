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
			//  2 << ldloc.0 NULL [Label13]
			//  3    ldc.r4 1
			//  4    stfld System.Single <> c__DisplayClass108_0::equipmentWeightMultiplier
			//  5    ldarg.0 NULL
			//  6    ldfld TaleWorlds.Core.BasicCharacterObject ItemMenuVM::_character
			//  7    isinst TaleWorlds.CampaignSystem.CharacterObject
			//  8 << dup NULL
			//  9    stloc.1 NULL
			// 10    brfalse.s Label25
			// 11    ldloc.1 NULL
			for (int i = 0; i < list.Count - 9; i++)
			{
				if (   list[i + 0].opcode == OpCodes.Call
					&& list[i + 0].operand is MethodInfo mi && mi.DeclaringType == typeof(ItemMenuVM) && mi.Name == "CreateColoredProperty"
					&& list[i + 1].opcode == OpCodes.Pop
					&& list[i + 2].opcode == OpCodes.Ldloc_0
					// -- insert here --
					&& list[i + 3].opcode == OpCodes.Ldc_R4
					&& list[i + 4].opcode == OpCodes.Stfld
					// [...]
					&& list[i + 8].opcode == OpCodes.Dup
					&& list[i + 9].opcode == OpCodes.Stloc_1)
				{
					// insert lambda function
					void Insert(int index, OpCode opcode, object operand = null, params Label[] labels)
					{
						var instruction = new CodeInstruction(opcode, operand);
						if (labels?.Length > 0)
							instruction.labels.AddRange(labels);
						list.Insert(i++ + index, instruction);
					}

					// declare local
					var local12 = generator.DeclareLocal(typeof(string));

					// define jump labels
					var label16 = generator.DefineLabel();
					var label17 = generator.DefineLabel();
					var label18 = generator.DefineLabel();
					var label19 = generator.DefineLabel();
					var label20 = generator.DefineLabel();
					var label21 = generator.DefineLabel();
					var label22 = generator.DefineLabel();
					var label23 = generator.DefineLabel();
					var label24 = generator.DefineLabel();

					//    ++ br.s Label16
					Insert(2, OpCodes.Br_S, label16);
					//  2 << ldloc.0 NULL [Label13]
					//    >> ldarg.0 NULL [Label13]
					list[i + 2].opcode = OpCodes.Ldarg_0;

					//    ++ ldfld ItemVM ItemMenuVM::_targetItem
					Insert(3, OpCodes.Ldfld, AccessTools.Field(typeof(ItemMenuVM), "_targetItem"));
					//    ++ dup NULL
					Insert(3, OpCodes.Dup);
					//    ++ brtrue.s Label17
					Insert(3, OpCodes.Brtrue_S, label17);
					//    ++ pop NULL
					Insert(3, OpCodes.Pop);
					//    ++ ldnull NULL
					Insert(3, OpCodes.Ldnull);
					//    ++ br.s Label18
					Insert(3, OpCodes.Br_S, label18);
					//    ++ ldflda ItemRosterElement ItemVM::ItemRosterElement [Label17]
					Insert(3, OpCodes.Ldflda, AccessTools.Field(typeof(ItemVM), "ItemRosterElement"), label17);
					//    ++ call EquipmentElement ItemRosterElement::get_EquipmentElement()
					Insert(3, OpCodes.Call, AccessTools.PropertyGetter(typeof(ItemRosterElement), "EquipmentElement"));
					//    ++ stloc.s 5 (EquipmentElement)
					Insert(3, OpCodes.Stloc_S, 5);
					//    ++ ldloca.s 5 (EquipmentElement)
					Insert(3, OpCodes.Ldloca_S, 5);
					//    ++ call ItemObject EquipmentElement::get_Item()
					Insert(3, OpCodes.Call, AccessTools.PropertyGetter(typeof(EquipmentElement), "Item"));
					//    ++ dup NULL
					Insert(3, OpCodes.Dup);
					//    ++ brtrue.s Label19
					Insert(3, OpCodes.Brtrue_S, label19);
					//    ++ pop NULL
					Insert(3, OpCodes.Pop);
					//    ++ ldnull NULL
					Insert(3, OpCodes.Ldnull);
					//    ++ br.s Label20
					Insert(3, OpCodes.Br_S, label20);
					//    ++ call BasicCultureObject ItemObject::get_Culture() [Label19]
					Insert(3, OpCodes.Call, AccessTools.PropertyGetter(typeof(ItemObject), "Culture"), label19);
					//    ++ brfalse.s Label21 [Label18, Label20]
					Insert(3, OpCodes.Brfalse_S, label21, label18, label20);
					//    ++ ldarg.0 NULL
					Insert(3, OpCodes.Ldarg_0);
					//    ++ ldfld ItemVM ItemMenuVM::_targetItem
					Insert(3, OpCodes.Ldfld, AccessTools.Field(typeof(ItemMenuVM), "_targetItem"));
					//    ++ ldflda ItemRosterElement ItemVM::ItemRosterElement
					Insert(3, OpCodes.Ldflda, AccessTools.Field(typeof(ItemVM), "ItemRosterElement"));
					//    ++ call EquipmentElement ItemRosterElement::get_EquipmentElement()
					Insert(3, OpCodes.Call, AccessTools.PropertyGetter(typeof(ItemRosterElement), "EquipmentElement"));
					//    ++ stloc.s 5 (EquipmentElement)
					Insert(3, OpCodes.Stloc_S, 5);
					//    ++ ldloca.s 5 (EquipmentElement)
					Insert(3, OpCodes.Ldloca_S, 5);
					//    ++ call ItemObject EquipmentElement::get_Item()
					Insert(3, OpCodes.Call, AccessTools.PropertyGetter(typeof(EquipmentElement), "Item"));
					//    ++ callvirt BasicCultureObject ItemObject::get_Culture()
					Insert(3, OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(ItemObject), "Culture"));
					//    ++ callvirt TextObject BasicCultureObject::get_Name()
					Insert(3, OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(BasicCultureObject), "Name"));
					//    ++ dup NULL
					Insert(3, OpCodes.Dup);
					//    ++ brtrue.s Label22
					Insert(3, OpCodes.Brtrue_S, label22);
					//    ++ pop NULL
					Insert(3, OpCodes.Pop);
					//    ++ ldnull NULL
					Insert(3, OpCodes.Ldnull);
					//    ++ br.s Label23
					Insert(3, OpCodes.Br_S, label23);
					//    ++ callvirt virtual System.String System.Object::ToString() [Label22]
					Insert(3, OpCodes.Callvirt, AccessTools.Method(typeof(object), "ToString"), label22);
					//    ++ stloc.s 12 (System.String) [Label23]
					Insert(3, OpCodes.Stloc_S, local12, label23);
					//    ++ ldloc.s 12 (System.String)
					Insert(3, OpCodes.Ldloc_S, local12);
					//    ++ call static System.Boolean System.String::IsNullOrWhiteSpace(System.String value)
					Insert(3, OpCodes.Call, AccessTools.Method(typeof(string), "IsNullOrWhiteSpace"));
					//    ++ brfalse.s Label24
					Insert(3, OpCodes.Brtrue_S, label24);
					//    ++ ldarg.0 NULL
					Insert(3, OpCodes.Ldarg_0);
					//    ++ ldarg.0 NULL
					Insert(3, OpCodes.Ldarg_0);
					//    ++ call MBBindingList`1<ItemMenuTooltipPropertyVM> ItemMenuVM::get_TargetItemProperties()
					Insert(3, OpCodes.Call, AccessTools.PropertyGetter(typeof(ItemMenuVM), "TargetItemProperties"));
					//    ++ ldstr "Culture: "
					Insert(3, OpCodes.Ldstr, "Culture: ");
					//    ++ ldloc.s 12 (System.String)
					Insert(3, OpCodes.Ldloc_S, local12);
					//    ++ ldc.i4.0 NULL
					Insert(3, OpCodes.Ldc_I4_0);
					//    ++ ldnull NULL
					Insert(3, OpCodes.Ldnull);
					//    ++ call ItemMenuTooltipPropertyVM ItemMenuVM::CreateProperty(...)
					Insert(3, OpCodes.Call, AccessTools.Method(typeof(ItemMenuVM), "CreateProperty"));
					//    ++ pop NULL
					Insert(3, OpCodes.Pop);
					//    ++ ldloc.0 NULL [Label16, Label21, Label24]
					Insert(3, OpCodes.Ldloc_0, null, label16, label21, label24);


					//  8 << dup NULL
					//  9    stloc.1 NULL
					//    >> ldloc.1 NULL
					list[i + 8].opcode = OpCodes.Ldloc_1;
					(list[i + 9], list[i + 8]) = (list[i + 8], list[i + 9]);

					// notify that it's been applied and break
					applied = true;
					break;
				}
			}

			if (!applied)
				FileLog.Log($"{nameof(CustomPatches)}: failed to apply {nameof(Patch_ItemMenuVM_SetGeneralComponentTooltip)}");
			return list;
		}
	}

	//  0    call ItemMenuTooltipPropertyVM ItemMenuVM::CreateColoredProperty(...)
	//  1    pop NULL
	//  2 -- ldloc.0 NULL[Label13]
	//    ++ ldarg.0 NULL[Label13]
	//    ++ ldarg.0 NULL
	//    ++ ldfld TaleWorlds.Core.ViewModelCollection.ItemVM ItemMenuVM::_targetItem
	//    ++ call static System.Void CustomPatches.Patch_ItemMenuVM_SetGeneralComponentTooltip::CreateCultureProperty(...)
	//    ++ ldloc.0 NULL
	//  3    ldc.r4 1
	//  4    stfld System.Single <> c__DisplayClass108_0::equipmentWeightMultiplier
	//  5    ldarg.0 NULL
	//  6    ldfld TaleWorlds.Core.BasicCharacterObject ItemMenuVM::_character
	//  7    isinst TaleWorlds.CampaignSystem.CharacterObject
	//  8 -- dup NULL
	//  9    stloc.1 NULL
	//    ++ ldloc.1 NULL
	// 10    brfalse.s Label16
	// 11    ldloc.1 NULL
	//for (int i = 0; i < list.Count - 9; i++)
	//{
	//	if (   list[i + 0].opcode == OpCodes.Call 
	//		&& list[i + 0].operand is MethodInfo mi && mi.DeclaringType == typeof(ItemMenuVM) && mi.Name == "CreateColoredProperty"
	//		&& list[i + 1].opcode == OpCodes.Pop
	//		&& list[i + 2].opcode == OpCodes.Ldloc_0
	//		// -- insert here --
	//		&& list[i + 3].opcode == OpCodes.Ldc_R4
	//		&& list[i + 4].opcode == OpCodes.Stfld
	//		// [...]
	//		&& list[i + 8].opcode == OpCodes.Dup
	//		&& list[i + 9].opcode == OpCodes.Stloc_1)
	//	{
	//		// replace opcode
	//		list[i + 2].opcode = OpCodes.Ldarg_0;

	//		// insert new code
	//		list.Insert(i++ + 3, new CodeInstruction(OpCodes.Ldarg_0));
	//		list.Insert(i++ + 3, new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ItemMenuVM), "_targetItem")));
	//		list.Insert(i++ + 3, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Patch_ItemMenuVM_SetGeneralComponentTooltip), "CreateCultureProperty")));
	//		list.Insert(i++ + 3, new CodeInstruction(OpCodes.Ldloc_0));

	//		// replace opcode and swap places
	//		list[i + 8].opcode = OpCodes.Ldloc_1;
	//		(list[i + 9], list[i + 8]) = (list[i + 8], list[i + 9]);

	//		added = true;
	//		break;
	//	}
	//}

	// parameters:
	//	MBBindingList<ItemMenuTooltipPropertyVM> targetList,
	//	string definition,
	//	string value,
	//	int textHeight = 0,
	//	HintViewModel hint = null
	//		private static readonly MethodInfo CreatePropertyMethodInfo = AccessTools.Method(typeof(ItemMenuVM), "CreateProperty");
	//		public static void CreateCultureProperty(ItemMenuVM __instance, ItemVM _targetItem)
	//		{
	//#warning TODO implement method directly? & find out how comparing with player character works for culture
	//			try
	//			{
	//				if (Game.Current.IsDevelopmentMode)
	//					return;

	//				var culture = _targetItem?.ItemRosterElement.EquipmentElement.Item?.Culture?.Name?.ToString() ?? "";
	//				if (string.IsNullOrWhiteSpace(culture))
	//					return;

	//				CreatePropertyMethodInfo.Invoke(
	//					__instance,
	//					new object[]
	//					{
	//						__instance.TargetItemProperties,
	//						"Culture: ",
	//						culture,
	//						0,
	//						null,
	//					});
	//			}
	//			catch (Exception exc)
	//			{
	//				FileLog.Log($"{exc.GetType()}\n{exc.Message}");
	//				InformationManager.DisplayMessage(new InformationMessage($"{nameof(CustomPatches)}.{nameof(CreateCultureProperty)}: caused an exception: {exc.GetType()}", new Color(1f, 0f, 0f)));
	//			}
	//		}
}
