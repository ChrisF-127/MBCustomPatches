using HarmonyLib;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace SyUtilityPatches
{
	public class MCMSettings : AttributeGlobalSettings<MCMSettings>
	{
		public override string Id => "SyUtilityPatches";
		public override string DisplayName => "Syrus Utility Patches";
		public override string FolderName => "SyUtilityPatches";
		public override string FormatType => "json";

		private HarmonyPatches HarmonyPatches => SyUtilityPatches.HarmonyPatches;

		private float _crosshairOpacity = 1f;
		[SettingPropertyFloatingInteger(
			"Crosshair Opacity",
			0f,
			1f,
			"0.00",
			RequireRestart = false,
			HintText = " [Native: 1.00]",
			Order = 0)]
		[SettingPropertyGroup(
			"Settings",
			GroupOrder = 0)]
		public float CrosshairOpacity
		{ 
			get => _crosshairOpacity;
			set
			{
				_crosshairOpacity = value;
				HarmonyPatches?.PatchCrosshairOpacity(_crosshairOpacity != 1f);
			}
		}

		private bool _unlockAllParts = false;
		[SettingPropertyBool(
			"Smithing: Unlock all parts",
			RequireRestart = false,
			HintText = "Unlocks all smithing parts as long as the setting is enabled\nDisabling it will lock all parts that have not previously been unlocked",
			Order = 1)]
		[SettingPropertyGroup(
			"Settings",
			GroupOrder = 0)]
		public bool UnlockAllParts
		{
			get => _unlockAllParts;
			set
			{
				_unlockAllParts = value;
				HarmonyPatches?.PatchUnlockAllParts(_unlockAllParts);
			}
		}

		private bool _allowAllQuality = false;
		[SettingPropertyBool(
			"Smithing: Disable quality limited by part tier",
			RequireRestart = false,
			HintText = "Smithing quality is limited by the average tier of parts used (None = 1), an average of >=4.5 may be Legendary, >=3.5 Masterwork and below Fine. Enabling this setting removes these limits",
			Order = 2)]
		[SettingPropertyGroup(
			"Settings",
			GroupOrder = 0)]
		public bool AllowAllQuality
		{
			get => _allowAllQuality;
			set
			{
				_allowAllQuality = value;
				HarmonyPatches?.PatchAllowAllQuality(_allowAllQuality);
			}
		}

		private bool _alwaysWar = false;
		[SettingPropertyBool(
			"Always War",
			RequireRestart = false,
			HintText = "Every war proposal should be accepted",
			Order = 3)]
		[SettingPropertyGroup(
			"Settings",
			GroupOrder = 0)]
		public bool AlwaysWar
		{
			get => _alwaysWar;
			set
			{
				_alwaysWar = value;
				HarmonyPatches?.PatchAlwaysWar(_alwaysWar);
			}
		}

		private float _partySpeedModifier = 1f;
		[SettingPropertyFloatingInteger(
			"Party Speed Modifier",
			0.01f,
			100f,
			"0.00",
			RequireRestart = false,
			HintText = "Modifies the movement speed of all parties [Native: 1.00]",
			Order = 4)]
		[SettingPropertyGroup(
			"Settings",
			GroupOrder = 0)]
		public float PartySpeedModifier
		{
			get => _partySpeedModifier;
			set
			{
				_partySpeedModifier = value;
				HarmonyPatches?.PatchPartySpeed(_partySpeedModifier != 1f);
			}
		}

		private float _minimumThrustMomentum = 0f;
		[SettingPropertyFloatingInteger(
			"Minimum Thrust Momentum",
			0f,
			1f,
			"0.00",
			RequireRestart = false,
			HintText = "Just an attempt to make spears/polearms useable at close range [Native: 0.00]",
			Order = 5)]
		[SettingPropertyGroup(
			"Settings",
			GroupOrder = 0)]
		public float MinimumThrustMomentum
		{
			get => _minimumThrustMomentum;
			set
			{
				_minimumThrustMomentum = value;
				HarmonyPatches?.PatchMinimumThrustMomentum(_minimumThrustMomentum != 0f);
			}
		}

		private bool _showCultureInTooltip = false;
		[SettingPropertyBool(
			"Show Culture in Tooltip",
			RequireRestart = false,
			HintText = "Shows culture in equipment tooltip",
			Order = 6)]
		[SettingPropertyGroup(
			"Settings",
			GroupOrder = 0)]
		public bool ShowCultureInTooltip
		{
			get => _showCultureInTooltip;
			set
			{
				_showCultureInTooltip = value;
				HarmonyPatches?.PatchShowCultureInTooltip(_showCultureInTooltip);
			}
		}

		private bool _combatBalanceShowAllTroops = false;
		[SettingPropertyBool(
			"Consider all troops for combat balance",
			RequireRestart = false,
			HintText = 
			"Takes into account all troops for each side for the combat balance bar at the top of the screen" +
			"\nExclusive with 'Show all troops in scoreboard'!",
			Order = 7)]
		[SettingPropertyGroup(
			"Settings",
			GroupOrder = 0)]
		public bool CombatBalanceShowAllTroops
		{
			get => _combatBalanceShowAllTroops;
			set
			{
				if (value)
					ScoreboardShowAllTroops = false;
				_combatBalanceShowAllTroops = value;
				HarmonyPatches?.PatchCombatBalanceShowAllTroops(_combatBalanceShowAllTroops);
				OnPropertyChanged(nameof(CombatBalanceShowAllTroops));
			}
		}
		private bool _scoreboardShowAllTroops = false;
		[SettingPropertyBool(
			"Show all troops in scoreboard",
			RequireRestart = false,
			HintText = 
			"Show all troops for each side in the scoreboard, instead of only spawned troops" +
			"\nExclusive with 'Consider all troops for combat balance'!",
			Order = 8)]
		[SettingPropertyGroup(
			"Settings",
			GroupOrder = 0)]
		public bool ScoreboardShowAllTroops
		{
			get => _scoreboardShowAllTroops;
			set
			{
				if (value)
					CombatBalanceShowAllTroops = false;
				_scoreboardShowAllTroops = value;
				HarmonyPatches?.PatchScoreboardShowAllTroops(_scoreboardShowAllTroops);
				OnPropertyChanged(nameof(ScoreboardShowAllTroops));
			}
		}

		[SettingPropertyButton(
			"Add all equipment to inventory",
			RequireRestart = false,
			HintText = "This was implemented for testing purposes; it will flood your inventory with every possible equipment item!",
			Content = "ADD EQUIPMENT",
			Order = 9)]
		[SettingPropertyGroup(
			"Settings",
			GroupOrder = 0)]
		public Action AllEquipmentButton { get; set; } = () =>
		{
			InformationManager.ShowInquiry(
				new InquiryData(
					new TextObject("Adding equipment...").ToString(),
					new TextObject("Add all available, valid equipment to inventory?").ToString(),
					true, true,
					new TextObject("Yes").ToString(),
					new TextObject("Cancel").ToString(),
					action,
					() => { }));
			void action()
			{
				var mainParty = Campaign.Current?.MainParty;
				if (mainParty != null)
				{
					int count = 0;
					foreach (var item in Items.All)
					{
						if (item != null
							&& !item.IsAnimal 
							&& !item.IsFood 
							&& !item.IsTradeGood 
							//&& item.Culture == mainParty.LeaderHero.Culture 
							//&& !mainParty.ItemRoster.Any(ir => ir.EquipmentElement.Item == item)
							)
						{
							SyUtilityPatches.Message($"Adding equipment: '{item.Name}'", false, Colors.White);
							mainParty.ItemRoster.Add(new ItemRosterElement(item, 1));
							count++;
						}
					}
					InformationManager.DisplayMessage(new InformationMessage($"Done; {count} items added to player inventory", Colors.Cyan));
				}
				else
					InformationManager.DisplayMessage(new InformationMessage("Could not add equipment, player party not found", Colors.Red));
			}
		};

		public void ApplySettings()
		{
			HarmonyPatches.PatchCrosshairOpacity(CrosshairOpacity != 1f);
			HarmonyPatches.PatchUnlockAllParts(UnlockAllParts);
			HarmonyPatches.PatchAllowAllQuality(AllowAllQuality);
			HarmonyPatches.PatchAlwaysWar(AlwaysWar);
			HarmonyPatches.PatchPartySpeed(PartySpeedModifier != 1f);
			HarmonyPatches.PatchMinimumThrustMomentum(MinimumThrustMomentum != 0f);
			HarmonyPatches.PatchShowCultureInTooltip(ShowCultureInTooltip);
			HarmonyPatches.PatchCombatBalanceShowAllTroops(CombatBalanceShowAllTroops);
			HarmonyPatches.PatchScoreboardShowAllTroops(ScoreboardShowAllTroops);
		}
	}
}
