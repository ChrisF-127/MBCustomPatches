using HarmonyLib;
using SandBox.ViewModelCollection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection.Inventory;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Engine;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.ExtraWidgets;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.ViewModelCollection.Scoreboard;
using TaleWorlds.TwoDimension;
using TaleWorlds.CampaignSystem.Settlements;

namespace SyUtilityPatches
{
	public static class HarmonyPatches
	{
		private static Harmony Harmony { get; set; }

		public static void Initialize()
		{
			Harmony = new Harmony("sy.utilitypatches");
			PatchAgentMeshCrashPrevention();
		}

		#region CROSSHAIR OPACITY
		private static bool _patchedCrosshairOpacity = false;
		public static void PatchCrosshairOpacity(bool apply)
		{
			if (apply && !_patchedCrosshairOpacity)
			{
				Harmony.Patch(
					AccessTools.Method(typeof(CrosshairWidget), "OnUpdate"),
					postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(CrosshairWidget_OnUpdate_Postfix)));
				_patchedCrosshairOpacity = true;
			}
			else if (!apply && _patchedCrosshairOpacity)
			{
				Harmony.Unpatch(
					AccessTools.Method(typeof(CrosshairWidget), "OnUpdate"),
					AccessTools.Method(typeof(HarmonyPatches), nameof(CrosshairWidget_OnUpdate_Postfix)));
				_patchedCrosshairOpacity = false;
			}
		}
		private static void CrosshairWidget_OnUpdate_Postfix(CrosshairWidget __instance)
		{
			foreach (var ic in __instance.Children)
			{
				if (ic is ValueBasedVisibilityWidget v)
				{
					foreach (var vc in v.Children)
					{
						if (vc is BrushWidget bw)
							bw.SetAlpha(SyUtilityPatches.Settings.CrosshairOpacity);
					}
				}
			}
		}
		#endregion

		#region UNLOCK ALL SMITHING PARTS
		private static bool _patchedUnlockAllParts = false;
		public static void PatchUnlockAllParts(bool apply)
		{
			if (apply && !_patchedUnlockAllParts)
			{
				Harmony.Patch(
					AccessTools.Method(typeof(CraftingCampaignBehavior), "IsOpened"),
					postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(CraftingCampaignBehavior_IsOpened_Postfix)));
				_patchedUnlockAllParts = true;
			}
			else if (!apply && _patchedUnlockAllParts)
			{
				Harmony.Unpatch(
					AccessTools.Method(typeof(CraftingCampaignBehavior), "IsOpened"),
					AccessTools.Method(typeof(HarmonyPatches), nameof(CraftingCampaignBehavior_IsOpened_Postfix)));
				_patchedUnlockAllParts = false;
			}
		}
		private static void CraftingCampaignBehavior_IsOpened_Postfix(ref bool __result)
		{
			__result = true;
		}
		#endregion

		#region ALLOW ALL QUALITY
		private static bool _patchedAllowAllQuality = false;
		public static void PatchAllowAllQuality(bool apply)
		{
			if (apply && !_patchedAllowAllQuality)
			{
				Harmony.Patch(
					AccessTools.Method(typeof(DefaultSmithingModel), "AdjustQualityRegardingDesignTier"),
					prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(DefaultSmithingModel_AdjustQualityRegardingDesignTier_Prefix)));
				_patchedAllowAllQuality = true;
			}
			else if (!apply && _patchedAllowAllQuality)
			{
				Harmony.Unpatch(
					AccessTools.Method(typeof(DefaultSmithingModel), "AdjustQualityRegardingDesignTier"),
					AccessTools.Method(typeof(HarmonyPatches), nameof(DefaultSmithingModel_AdjustQualityRegardingDesignTier_Prefix)));
				_patchedAllowAllQuality = false;
			}
		}
		private static bool DefaultSmithingModel_AdjustQualityRegardingDesignTier_Prefix(ref ItemQuality __result, ItemQuality weaponQuality)
		{
			__result = weaponQuality;
			return false; // do nothing & skip original method
		}
		#endregion

		#region ALWAYS WAR
		private static bool _patchedAlwaysWar = false;
		public static void PatchAlwaysWar(bool apply)
		{
			if (apply && !_patchedAlwaysWar)
			{
				Harmony.Patch(
					AccessTools.Method(typeof(DeclareWarDecision), "DetermineSupport"),
					postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(DeclareWarDecision_DetermineSupport_Postfix)));
				Harmony.Patch(
					AccessTools.Method(typeof(KingdomDecisionProposalBehavior), "ConsiderWar"),
					postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(KingdomDecisionProposalBehavior_ConsiderWar_Postfix)));
				_patchedAlwaysWar = true;
			}
			else if (!apply && _patchedAlwaysWar)
			{
				Harmony.Unpatch(
					AccessTools.Method(typeof(DeclareWarDecision), "DetermineSupport"),
					AccessTools.Method(typeof(HarmonyPatches), nameof(DeclareWarDecision_DetermineSupport_Postfix)));
				Harmony.Unpatch(
					AccessTools.Method(typeof(KingdomDecisionProposalBehavior), "ConsiderWar"),
					AccessTools.Method(typeof(HarmonyPatches), nameof(KingdomDecisionProposalBehavior_ConsiderWar_Postfix)));
				_patchedAlwaysWar = false;
			}
		}
		private static void DeclareWarDecision_DetermineSupport_Postfix(Clan clan, ref float __result)
		{
			__result = 100f;
		}
		private static void KingdomDecisionProposalBehavior_ConsiderWar_Postfix(Clan clan, Kingdom kingdom, ref bool __result)
		{
			__result = true;
		}
		#endregion

		#region PARTY SPEED
		private static bool _patchedPartySpeed = false;
		public static void PatchPartySpeed(bool apply)
		{
			if (apply && !_patchedPartySpeed)
			{
				Harmony.Patch(
					AccessTools.Method(typeof(DefaultPartySpeedCalculatingModel), "CalculateBaseSpeedForParty"),
					postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(DefaultPartySpeedCalculatingModel_CalculateBaseSpeedForParty_Postfix)));
				_patchedPartySpeed = true;
			}
			else if (!apply && _patchedPartySpeed)
			{
				Harmony.Unpatch(
					AccessTools.Method(typeof(DefaultPartySpeedCalculatingModel), "CalculateBaseSpeedForParty"),
					AccessTools.Method(typeof(HarmonyPatches), nameof(DefaultPartySpeedCalculatingModel_CalculateBaseSpeedForParty_Postfix)));
				_patchedPartySpeed = false;
			}
		}
		private static void DefaultPartySpeedCalculatingModel_CalculateBaseSpeedForParty_Postfix(ref float __result)
		{
			__result *= SyUtilityPatches.Settings.PartySpeedModifier;
		}
		#endregion

		#region MINIMUM THRUST MOMENTUM
		private static bool _patchedMinimumThrustMomentum = false;
		public static void PatchMinimumThrustMomentum(bool apply)
		{
			if (apply && !_patchedMinimumThrustMomentum)
			{
				Harmony.Patch(
					AccessTools.Method(typeof(Mission), "MeleeHitCallback"),
					prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(Mission_MeleeHitCallback_Prefix)));
				_patchedMinimumThrustMomentum = true;
			}
			else if (!apply && _patchedMinimumThrustMomentum)
			{
				Harmony.Unpatch(
					AccessTools.Method(typeof(Mission), "MeleeHitCallback"),
					AccessTools.Method(typeof(HarmonyPatches), nameof(Mission_MeleeHitCallback_Prefix)));
			}
		}
		private static void Mission_MeleeHitCallback_Prefix(AttackCollisionData collisionData, ref float inOutMomentumRemaining)
		{
			if (collisionData.StrikeType == (int)StrikeType.Thrust && inOutMomentumRemaining < SyUtilityPatches.Settings.MinimumThrustMomentum)
				inOutMomentumRemaining = SyUtilityPatches.Settings.MinimumThrustMomentum;
		}
		#endregion

		#region SHOW CULTURE IN TOOLTIP
		private static bool _patchedShowCultureInTooltip = false;
		public static void PatchShowCultureInTooltip(bool apply)
		{
			if (apply && !_patchedShowCultureInTooltip)
			{
				Harmony.Patch(
					AccessTools.Method(typeof(ItemMenuVM), "SetGeneralComponentTooltip"),
					transpiler: new HarmonyMethod(typeof(HarmonyPatches), nameof(ItemMenuVM_SetGeneralComponentTooltip_Transpiler)));
				_patchedShowCultureInTooltip = true;
			}
			else if (!apply && _patchedShowCultureInTooltip)
			{
				Harmony.Unpatch(
					AccessTools.Method(typeof(ItemMenuVM), "SetGeneralComponentTooltip"),
					AccessTools.Method(typeof(HarmonyPatches), nameof(ItemMenuVM_SetGeneralComponentTooltip_Transpiler)));
				_patchedShowCultureInTooltip = false;
			}
		}
		private static IEnumerable<CodeInstruction> ItemMenuVM_SetGeneralComponentTooltip_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
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
					list.Insert(i++ + 3, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HarmonyPatches), nameof(CreateCultureProperty))));
					list.Insert(i++ + 3, new CodeInstruction(OpCodes.Ldloc_0));

					// replace opcode and swap places
					list[i + 8].opcode = OpCodes.Ldloc_1;
					(list[i + 9], list[i + 8]) = (list[i + 8], list[i + 9]);

					applied = true;
					break;
				}
			}

			if (!applied)
				SyUtilityPatches.Message($"{nameof(SyUtilityPatches)}: failed to apply {nameof(ItemMenuVM_SetGeneralComponentTooltip_Transpiler)}");
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
		internal static void CreateCultureProperty(ItemMenuVM __instance, ItemVM _targetItem, BasicCharacterObject _character)
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
				SyUtilityPatches.Message($"{nameof(SyUtilityPatches)}.{nameof(CreateCultureProperty)}: caused an exception: {exc.GetType()}: {exc.Message}");
			}
		}
		#endregion

		#region COMBAT BALANCE BAR / SCOREBOARD
		#region COMBAT BALANCE BAR
		private static bool _patchedCombatBalanceShowAllTroops;
		public static void PatchCombatBalanceShowAllTroops(bool apply)
		{
			if (apply && !_patchedCombatBalanceShowAllTroops && !_patchedScoreboardShowAllTroops)
			{
				Harmony.Patch(
					AccessTools.Method(typeof(SPScoreboardSideVM), "RefreshPower"),
					prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(SPScoreboardSideVM_RefreshPower_Prefix)));
				_patchedCombatBalanceShowAllTroops = true;
			} 
			else if (!apply && _patchedCombatBalanceShowAllTroops)
			{
				Harmony.Unpatch(
					AccessTools.Method(typeof(SPScoreboardSideVM), "RefreshPower"),
					AccessTools.Method(typeof(HarmonyPatches), nameof(SPScoreboardSideVM_RefreshPower_Prefix)));
				_patchedCombatBalanceShowAllTroops = false;
			}
		}

		private static readonly PropertyInfo _sbSide_InitialPowerPI = AccessTools.Property(typeof(SPScoreboardSideVM), "InitialPower");
		private static readonly PropertyInfo _sbSide_CurrentPowerPI = AccessTools.Property(typeof(SPScoreboardSideVM), "CurrentPower");
		private static string _battleResultArmy_AttackerText = null;
		private static string BattleResultArmy_AttackerText
		{
			get
			{
				if (_battleResultArmy_AttackerText == null)
					_battleResultArmy_AttackerText = GameTexts.FindText("str_battle_result_army", "attacker").Value;
				return _battleResultArmy_AttackerText;
			}
		}
		private static bool SPScoreboardSideVM_RefreshPower_Prefix(
			SPScoreboardSideVM __instance,
			TextObject ____nameTextObject)
		{
			if (Mission.Current?.HasMissionBehavior<BattleReinforcementsSpawnController>() == true)
			{
				var mapEventSide = ____nameTextObject.Value == BattleResultArmy_AttackerText ? 
					MobileParty.MainParty?.MapEvent?.AttackerSide : 
					MobileParty.MainParty?.MapEvent?.DefenderSide;
				if (mapEventSide != null)
				{
					var initialPower = __instance.InitialPower;
					if (initialPower == 0f)
					{
						var descriptors = new List<UniqueTroopDescriptor>();
						mapEventSide.GetAllTroops(ref descriptors);
						foreach (var desc in descriptors)
						{
							var troop = mapEventSide.GetAllocatedTroop(desc) ?? mapEventSide.GetReadyTroop(desc);
							initialPower += troop.GetPower();
						}
						_sbSide_InitialPowerPI.SetValue(__instance, initialPower);
					}

					var currentPower = initialPower;
					foreach (var party in __instance.Parties)
					{
						foreach (var member in party.Members)
						{
							var score = member.Score;
							currentPower -= (score.Dead + score.Routed + score.Wounded) * member.Character.GetPower();
						}
					}
					_sbSide_CurrentPowerPI.SetValue(__instance, Mathf.Max(currentPower, 0));

					//CustomPatches.Message($"{mapEventSide.MissionSide} {initialPower} {currentPower}", false, Colors.Cyan);
					return false; // skip
				}
			}
			return true; // don't skip
		}
		#endregion

		#region SCOREBOARD
		private static bool _patchedScoreboardShowAllTroops;
		public static void PatchScoreboardShowAllTroops(bool apply)
		{
			if (apply && !_patchedScoreboardShowAllTroops && !_patchedCombatBalanceShowAllTroops)
			{
				Harmony.Patch(
					AccessTools.Method(typeof(SPScoreboardVM), "Initialize"),
					postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(SPScoreboardVM_Initialize_Postfix)));
				Harmony.Patch(
					AccessTools.Method(typeof(BattleObserverMissionLogic), "OnAgentBuild"),
					transpiler: new HarmonyMethod(typeof(HarmonyPatches), nameof(BattleObserverMissionLogic_OnAgentBuild_Transpiler)));
				_patchedScoreboardShowAllTroops = true;
			}
			else if (_patchedScoreboardShowAllTroops)
			{
				Harmony.Unpatch(
					AccessTools.Method(typeof(SPScoreboardVM), "Initialize"),
					AccessTools.Method(typeof(HarmonyPatches), nameof(SPScoreboardVM_Initialize_Postfix)));
				Harmony.Unpatch(
					AccessTools.Method(typeof(BattleObserverMissionLogic), "OnAgentBuild"),
					AccessTools.Method(typeof(HarmonyPatches), nameof(BattleObserverMissionLogic_OnAgentBuild_Transpiler)));
				_patchedScoreboardShowAllTroops = false;
			}
		}
		// Add all troops to the Scoreboard upon initialization
		private static void SPScoreboardVM_Initialize_Postfix(SPScoreboardVM __instance)
		{
			if (PlayerEncounter.Battle != null 
				&& Mission.Current?.HasMissionBehavior<BattleReinforcementsSpawnController>() == true
				&& MobileParty.MainParty?.MapEvent?.AttackerSide != null
				&& MobileParty.MainParty?.MapEvent?.DefenderSide != null)
			{
				addAllTroops(BattleSideEnum.Attacker);
				addAllTroops(BattleSideEnum.Defender);
			}
			void addAllTroops(BattleSideEnum side)
			{
				var mapEventSide = side == BattleSideEnum.Attacker ? MobileParty.MainParty.MapEvent.AttackerSide : MobileParty.MainParty.MapEvent.DefenderSide;
				foreach (var party in mapEventSide.Parties)
				{
					foreach (var troop in party.Party.MemberRoster.GetTroopRoster())
						__instance.TroopNumberChanged(side, party.Party, troop.Character, troop.Number - troop.WoundedNumber);
				}
			}
		}
		// Have to disable spawning units being added to the Scoreboard, otherwise they are added twice
		private static IEnumerable<CodeInstruction> BattleObserverMissionLogic_OnAgentBuild_Transpiler(IEnumerable<CodeInstruction> codeInstructions)
		{
			var list = codeInstructions.ToList();
			var patched = false;

			//SyUtilityPatches.Message($"mmmmmmmmmm BEFORE\n{string.Join("\n", list)}", false, Colors.Cyan);

			for (int i = 0; i < list.Count; i++)
			{
				// -- callvirt abstract virtual System.Void TaleWorlds.Core.IBattleObserver::TroopNumberChanged(TaleWorlds.Core.BattleSideEnum side, TaleWorlds.Core.IBattleCombatant battleCombatant, TaleWorlds.Core.BasicCharacterObject character, System.Int32 number, System.Int32 numberKilled, System.Int32 numberWounded, System.Int32 numberRouted, System.Int32 killCount, System.Int32 numberReadyToUpgrade)
				// ++ call static System.Void SyUtilityPatches.HarmonyPatches::BattleObserverMissionLogic_OnAgentBuild_TroopNumberChanged(TaleWorlds.Core.IBattleObserver BattleObserver, TaleWorlds.MountAndBlade.Agent agent, TaleWorlds.Core.BattleSideEnum side, TaleWorlds.Core.IBattleCombatant battleCombatant, TaleWorlds.Core.BasicCharacterObject character, System.Int32 number, System.Int32 numberKilled, System.Int32 numberWounded, System.Int32 numberRouted, System.Int32 killCount, System.Int32 numberReadyToUpgrade)
				if (list[i].opcode == OpCodes.Callvirt && list[i].operand is MethodInfo mi && mi.Name == "TroopNumberChanged")
				{
					list[i] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HarmonyPatches), nameof(BattleObserverMissionLogic_OnAgentBuild_TroopNumberChanged)));
					patched = true;
					break;
				}
			}

			if (!patched)
				SyUtilityPatches.Message($"{nameof(SyUtilityPatches)}: failed to apply {nameof(BattleObserverMissionLogic_OnAgentBuild_Transpiler)}", false);

			//SyUtilityPatches.Message($"mmmmmmmmmm AFTER\n{string.Join("\n", list)}", false, Colors.Cyan);

			return list;
		}
		private static void BattleObserverMissionLogic_OnAgentBuild_TroopNumberChanged(IBattleObserver BattleObserver, BattleSideEnum side, IBattleCombatant battleCombatant, BasicCharacterObject character, 
			int number = 0, int numberKilled = 0, int numberWounded = 0, int numberRouted = 0, int killCount = 0, int numberReadyToUpgrade = 0)
		{
			if (!(PlayerEncounter.Battle != null
				&& Mission.Current?.HasMissionBehavior<BattleReinforcementsSpawnController>() == true
				&& MobileParty.MainParty?.MapEvent?.AttackerSide != null
				&& MobileParty.MainParty?.MapEvent?.DefenderSide != null))
				BattleObserver.TroopNumberChanged(side, battleCombatant, character, number, numberKilled, numberWounded, numberRouted, killCount, numberReadyToUpgrade);
		}
		#endregion
		#endregion

		#region COMPANION LIMIT
		private static bool _patchedCompanionLimit = false;
		public static void PatchCompanionLimit(bool apply)
		{
			if (apply && !_patchedCompanionLimit)
			{
				Harmony.Patch(
					AccessTools.PropertyGetter(typeof(CompanionsCampaignBehavior), "_desiredTotalCompanionCount"),
					postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(CompanionsCampaignBehavior_get_desiredTotalCompanionCount_Postfix)));
				Harmony.Patch(
					AccessTools.Method(typeof(CompanionsCampaignBehavior), "OnNewGameCreated"),
					transpiler: new HarmonyMethod(typeof(HarmonyPatches), nameof(CompanionsCampaignBehavior_OnNewGameCreated_Transpiler)));
				_patchedCompanionLimit = true;
			}
			else if (!apply && _patchedCompanionLimit)
			{
				Harmony.Unpatch(
					AccessTools.PropertyGetter(typeof(CompanionsCampaignBehavior), "_desiredTotalCompanionCount"),
					AccessTools.Method(typeof(HarmonyPatches), nameof(CompanionsCampaignBehavior_get_desiredTotalCompanionCount_Postfix)));
				Harmony.Unpatch(
					AccessTools.Method(typeof(CompanionsCampaignBehavior), "OnNewGameCreated"),
					AccessTools.Method(typeof(HarmonyPatches), nameof(CompanionsCampaignBehavior_OnNewGameCreated_Transpiler)));
				_patchedCompanionLimit = false;
			}
		}
		private static void CompanionsCampaignBehavior_get_desiredTotalCompanionCount_Postfix(ref float __result)
		{
			//FileLog.Log($"Companion count: {__result} * {SyUtilityPatches.Settings.WandererSpawnLimitModifier} = {__result * SyUtilityPatches.Settings.WandererSpawnLimitModifier}");
			__result *= SyUtilityPatches.Settings.WandererSpawnLimitModifier;
		}
		private static IEnumerable<CodeInstruction> CompanionsCampaignBehavior_OnNewGameCreated_Transpiler(IEnumerable<CodeInstruction> codeInstructions)
		{
			var list = codeInstructions.ToList();
			var patched = false;

			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].opcode == OpCodes.Callvirt && list[i].operand is MethodInfo mi && mi.Name == "get_Item")
				{
					list.Insert(i++, new CodeInstruction(OpCodes.Ldloc_0));
					list.Insert(i++, new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(List<Town>), nameof(List<Town>.Count))));
					list.Insert(i++, new CodeInstruction(OpCodes.Rem));

					patched = true;
					break;
				}
			}

			if (!patched)
				SyUtilityPatches.Message($"{nameof(SyUtilityPatches)}: failed to apply {nameof(CompanionsCampaignBehavior_OnNewGameCreated_Transpiler)}", false);

			return list;
		}
		#endregion

		#region MISSING AGENT MESH CRASH PREVENTION
		public static void PatchAgentMeshCrashPrevention()
		{
			Harmony.Patch(
				AccessTools.Method(typeof(MissionAgentLabelView), "SetHighlightForAgents"),
				transpiler: new HarmonyMethod(typeof(HarmonyPatches), nameof(MissionAgentLabelView_SetHighlightForAgents_Transpiler)));

			// find lambda method
			var type = typeof(MissionAgentLabelView).GetNestedTypes(AccessTools.all).FirstOrDefault(t => t.Name.Contains("c__DisplayClass38_0"));
			var method = type.GetMethods(AccessTools.all).FirstOrDefault(m => m.Name.Contains("b__0"));
			Harmony.Patch(
				method,
				transpiler: new HarmonyMethod(typeof(HarmonyPatches), nameof(MissionAgentLabelView_SetHighlightForAgents_b__0_Transpiler)));
		}
		private static IEnumerable<CodeInstruction> MissionAgentLabelView_SetHighlightForAgents_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
		{
			var list = new List<CodeInstruction>(instructions);

			//FileLog.Log($"mmmmmmmmmmmmmmmmmmmmmmmmm BEFORE");
			//foreach (var instruction in list)
			//	FileLog.Log($"{instruction}");

			Label? prevLabel = null, thisLabel = null;
			for (int i = 0; i < list.Count; i++)
			{
				//ldloc.s 4 (TaleWorlds.MountAndBlade.Agent)
				//ldarg.0 NULL
				//ldfld System.Collections.Generic.Dictionary`2<TaleWorlds.MountAndBlade.Agent, TaleWorlds.Engine.MetaMesh> CustomPatches.Test::_agentMeshes
				//ldloc.s 4 (TaleWorlds.MountAndBlade.Agent)
				//callvirt virtual TaleWorlds.Engine.MetaMesh System.Collections.Generic.Dictionary`2<TaleWorlds.MountAndBlade.Agent, TaleWorlds.Engine.MetaMesh>::get_Item(TaleWorlds.MountAndBlade.Agent key)
				if (list[i].opcode == OpCodes.Br_S && list[i].operand is Label l)
				{
					prevLabel = l;
				}
				else if (list[i].opcode == OpCodes.Ldloc_S && list[i].operand is LocalBuilder lb
					&& list[i + 1].opcode == OpCodes.Ldarg_0
					&& list[i + 2].opcode == OpCodes.Ldfld && list[i + 2].operand is FieldInfo fi && fi.Name == "_agentMeshes"
					&& list[i + 3].opcode == OpCodes.Ldloc_S && list[i + 3].operand == lb
					&& list[i + 4].opcode == OpCodes.Callvirt && list[i + 4].operand is MethodInfo mi && mi.Name == "get_Item")
				{
					//ldfld System.Collections.Generic.Dictionary`2<TaleWorlds.MountAndBlade.Agent, TaleWorlds.Engine.MetaMesh> CustomPatches.Test::_agentMeshes
					//ldloc.s 4 (TaleWorlds.MountAndBlade.Agent)
					//callvirt virtual System.Boolean System.Collections.Generic.Dictionary`2<TaleWorlds.MountAndBlade.Agent, TaleWorlds.Engine.MetaMesh>::ContainsKey(TaleWorlds.MountAndBlade.Agent key)
					//brfalse.s Label4
					//ldarg.0 NULL
					thisLabel = generator.DefineLabel();
					list.Insert(i++, new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(MissionAgentLabelView), "_agentMeshes")));
					list.Insert(i++, new CodeInstruction(OpCodes.Ldloc_S, lb));
					list.Insert(i++, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HarmonyPatches), nameof(AgentMeshes_CheckContainsKey))));
					list.Insert(i++, new CodeInstruction(OpCodes.Brfalse_S, thisLabel));
					list.Insert(i++, new CodeInstruction(OpCodes.Ldarg_0));
				}
				else if (prevLabel is Label p
					&& thisLabel is Label t
					&& list[i].labels.Contains(p))
				{
					list[i].labels.Add(t);

					prevLabel = null;
					thisLabel = null;
				}
			}

			//FileLog.Log($"mmmmmmmmmmmmmmmmmmmmmmmmm AFTER");
			//foreach (var instruction in list)
			//	FileLog.Log($"{instruction}");

			return list;
		}
		private static IEnumerable<CodeInstruction> MissionAgentLabelView_SetHighlightForAgents_b__0_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
		{
			var list = new List<CodeInstruction>(instructions);

			//FileLog.Log($"mmmmmmmmmmmmmmmmmmmmmmmmm BEFORE");
			//foreach (var instruction in list)
			//	FileLog.Log($"{instruction}");

			//ldfld System.Collections.Generic.Dictionary`2<TaleWorlds.MountAndBlade.Agent, TaleWorlds.Engine.MetaMesh> CustomPatches.Test::_agentMeshes
			//ldarg.1 NULL
			//callvirt virtual System.Boolean System.Collections.Generic.Dictionary`2<TaleWorlds.MountAndBlade.Agent, TaleWorlds.Engine.MetaMesh>::ContainsKey(TaleWorlds.MountAndBlade.Agent key)
			//brfalse.s Label0
			//ldarg.0 NULL
			//ldfld CustomPatches.Test CustomPatches.<>c__DisplayClass8_0::<>4__this
			var label = generator.DefineLabel();
			int i = 2;
			list.Insert(i++, new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(MissionAgentLabelView), "_agentMeshes")));
			list.Insert(i++, new CodeInstruction(OpCodes.Ldarg_1));
			list.Insert(i++, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HarmonyPatches), nameof(AgentMeshes_CheckContainsKey))));
			list.Insert(i++, new CodeInstruction(OpCodes.Brfalse_S, label));
			list.Insert(i++, new CodeInstruction(OpCodes.Ldarg_0));
			list.Insert(i++, list[1]); // ldfld
			list.Last().labels.Add(label);

			//FileLog.Log($"mmmmmmmmmmmmmmmmmmmmmmmmm AFTER");
			//foreach (var instruction in list)
			//	FileLog.Log($"{instruction}");

			return list;
		}
		private static bool AgentMeshes_CheckContainsKey(Dictionary<Agent, MetaMesh> agentMeshes, Agent key)
		{
			var contains = agentMeshes?.ContainsKey(key) == true;
			if (!contains)
				FileLog.Log($"WARNING: AgentMesh not found: {key?.Name}");
			return contains;
		}
		#endregion
	}
}
