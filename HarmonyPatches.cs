using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.ExtraWidgets;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission;
using TaleWorlds.MountAndBlade.View.MissionViews;

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
						{
							bw.SetAlpha(CustomPatches.Settings.CrosshairOpacity);
						}
					}
				}
			}
		}
	}

	[HarmonyPatch(typeof(MissionAgentLabelView), "SetHighlightForAgents")]
	internal static class Patch_MissionAgentLabelView_SetHighlightForAgents
	{
		[HarmonyTranspiler]
		internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
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
					list.Insert(i++, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Patch_MissionAgentLabelView_SetHighlightForAgents), "CheckContainsKey")));
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

		internal static bool CheckContainsKey(Dictionary<Agent, MetaMesh> agentMeshes, Agent key)
		{
			var contains = agentMeshes?.ContainsKey(key) == true;
			if (!contains)
				FileLog.Log($"Not found: {key?.Name}");
			return contains;
		}
	}

	[HarmonyPatch]
	internal static class Patch_MissionAgentLabelView_SetHighlightForAgents_b__0
	{
		[HarmonyTargetMethod]
		internal static MethodBase TargetMethod()
		{
			// find lambda method
			var type = typeof(MissionAgentLabelView).GetNestedTypes(AccessTools.all).FirstOrDefault(t => t.Name.Contains("c__DisplayClass38_0"));
			var method = type.GetMethods(AccessTools.all).FirstOrDefault(m => m.Name.Contains("b__0"));
			return method;
		}

		[HarmonyTranspiler]
		internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
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
			list.Insert(i++, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Patch_MissionAgentLabelView_SetHighlightForAgents), "CheckContainsKey")));
			list.Insert(i++, new CodeInstruction(OpCodes.Brfalse_S, label));
			list.Insert(i++, new CodeInstruction(OpCodes.Ldarg_0));
			list.Insert(i++, list[1]); // ldfld
			list.Last().labels.Add(label);

			//FileLog.Log($"mmmmmmmmmmmmmmmmmmmmmmmmm AFTER");
			//foreach (var instruction in list)
			//	FileLog.Log($"{instruction}");

			return list;
		}
	}

#if false
	[HarmonyPatch]
	internal static class Patch_Test_SetHighlightForAgents
	{
		[HarmonyTargetMethod]
		internal static MethodBase TargetMethod()
		{
			// find <>c__DisplayClass38_0::<>9__0
			var type = typeof(Test).GetNestedTypes(AccessTools.all).FirstOrDefault(t => t.Name.Contains("c__DisplayClass"));
			var method = type.GetMethods(AccessTools.all).FirstOrDefault(m => m.Name.Contains("__0"));
			return method;
		}

		[HarmonyTranspiler]
		internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var list = new List<CodeInstruction>(instructions);

			FileLog.Log($"mmmmmmmmmmmmmmmmmmmmmmmmm TEST");
			foreach (var instruction in list)
				FileLog.Log($"{instruction}");

			return list;
		}
	}
#endif

#if false
	[HarmonyPatch(typeof(Test), "SetHighlightForAgents")]
	internal static class Patch_Test_SetHighlightForAgents
	{
		[HarmonyTranspiler]
		internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var list = new List<CodeInstruction>(instructions);

			FileLog.Log($"mmmmmmmmmmmmmmmmmmmmmmmmm TEST");
			foreach (var instruction in list)
				FileLog.Log($"{instruction}");

			return list;
		}
	}
#endif

#if false
	internal class Test : MissionAgentLabelView
	{
		private OrderController PlayerOrderController { get; }
		private SiegeWeaponController PlayerSiegeWeaponController { get; }
		private readonly Dictionary<Agent, MetaMesh> _agentMeshes;

		private void UpdateSelectionVisibility(Agent agent, MetaMesh mesh, bool? visibility = null)
		{ }

		private void SetHighlightForAgents(bool highlight, bool useSiegeMachineUsers, bool useAllTeamAgents)
		{
			if (PlayerOrderController == null)
			{
				bool flag = base.Mission.PlayerTeam == null;
				Debug.Print($"PlayerOrderController is null and playerTeamIsNull: {flag}", 0, Debug.DebugColor.White, 17179869184uL);
			}
			if (useSiegeMachineUsers)
			{
				foreach (TaleWorlds.MountAndBlade.SiegeWeapon selectedWeapon in PlayerSiegeWeaponController.SelectedWeapons)
				{
					foreach (Agent user in selectedWeapon.Users)
					{
						if (_agentMeshes.ContainsKey(user)) // <--
							UpdateSelectionVisibility(user, _agentMeshes[user], highlight);
					}
				}
				return;
			}
			if (useAllTeamAgents)
			{
				if (PlayerOrderController.Owner == null)
				{
					return;
				}
				{
					foreach (Agent activeAgent in PlayerOrderController.Owner.Team.ActiveAgents)
					{
						if (_agentMeshes.ContainsKey(activeAgent)) // <--
							UpdateSelectionVisibility(activeAgent, _agentMeshes[activeAgent], highlight);
					}
					return;
				}
			}
			foreach (Formation selectedFormation in PlayerOrderController.SelectedFormations)
			{
				selectedFormation.ApplyActionOnEachUnit(delegate (Agent agent)
				{
					//if (_agentMeshes.ContainsKey(agent)) // <--
						UpdateSelectionVisibility(agent, _agentMeshes[agent], highlight);
				});
			}
		}
	}
#endif

#if false
	[HarmonyPatch(typeof(FightTournamentGame), "CanNpcJoinTournament")]
	internal static class Patch_FightTournamentGame_CanNpcJoinTournament
	{
		[HarmonyPostfix]
		internal static void Postfix(FightTournamentGame __instance, ref bool __result, Hero hero, List<CharacterObject> participantCharacters, bool considerSkills)
		{
			if (hero != null)
			{
				FileLog.Log(
					$"Hero {hero.Name} -> {__result}" +
					$"\n\t!IsWounded {!hero.IsWounded}" +
					$"\n\t!IsNoncombatant {!hero.IsNoncombatant}" +
					$"\n\t!Contains {!participantCharacters.Contains(hero.CharacterObject)}" +
					$"\n\t!MainHero {hero != Hero.MainHero}" +
					$"\n\tOfAge {hero.Age >= Campaign.Current.Models.AgeModel.HeroComesOfAge}" +
					$"\n\tIsLord {hero.IsLord}" +
					$"\n\tIsWanderer {hero.IsWanderer}" +
					$"\n\t-- CanBeAParticipant {__instance.CanBeAParticipant(hero.CharacterObject, considerSkills)}" +
					$"\n\t\tIsHero {hero.CharacterObject.IsHero}" +
					$"\n\t\tTier {hero.CharacterObject.Tier}" +
					$"\n\t\tConsiderSkills {considerSkills}" +
					$"\n\t\tOneHanded {hero.CharacterObject.HeroObject.GetSkillValue(DefaultSkills.OneHanded)}" +
					$"\n\t\tTwoHanded {hero.CharacterObject.HeroObject.GetSkillValue(DefaultSkills.TwoHanded)}" +
					$"\n");
			}
		}
	}
#endif
}
