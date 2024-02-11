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

namespace CustomPatches
{
	public class MCMSettings : AttributeGlobalSettings<MCMSettings>
	{
		public override string Id => "CustomPatches";
		public override string DisplayName => "Custom Patches";
		public override string FolderName => "CustomPatches";
		public override string FormatType => "json";

		public void ApplySettings()
		{
			CrosshairOpacity = CrosshairOpacity;
			UnlockAllParts = UnlockAllParts;
			AlwaysWar = AlwaysWar;
			PartySpeedModifier = PartySpeedModifier;
			ShowCultureInTooltip = ShowCultureInTooltip;
			CombatBalanceShowAllTroops = CombatBalanceShowAllTroops;
			ScoreboardShowAllTroops = ScoreboardShowAllTroops;
		}

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
				CustomPatches.HarmonyPatches.PatchCrosshairOpacity(_crosshairOpacity != 1f);
			}
		}

		private bool _unlockAllParts = false;
		[SettingPropertyBool(
			"Unlock all smithing parts",
			RequireRestart = false,
			HintText = "Unlock all smithing parts",
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
				CustomPatches.HarmonyPatches.PatchUnlockAllParts(_unlockAllParts);
			}
		}

		private bool _alwaysWar = false;
		[SettingPropertyBool(
			"Always War",
			RequireRestart = false,
			HintText = "Every war proposal should be accepted",
			Order = 2)]
		[SettingPropertyGroup(
			"Settings",
			GroupOrder = 0)]
		public bool AlwaysWar
		{
			get => _alwaysWar;
			set
			{
				_alwaysWar = value;
				CustomPatches.HarmonyPatches.PatchAlwaysWar(_alwaysWar);
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
			Order = 3)]
		[SettingPropertyGroup(
			"Settings",
			GroupOrder = 0)]
		public float PartySpeedModifier
		{
			get => _partySpeedModifier;
			set
			{
				_partySpeedModifier = value;
				CustomPatches.HarmonyPatches.PatchPartySpeed(_partySpeedModifier != 1f);
			}
		}

		private bool _showCultureInTooltip = false;
		[SettingPropertyBool(
			"Show Culture in Tooltip",
			RequireRestart = false,
			HintText = "Shows culture in equipment tooltip",
			Order = 4)]
		[SettingPropertyGroup(
			"Settings",
			GroupOrder = 0)]
		public bool ShowCultureInTooltip
		{
			get => _showCultureInTooltip;
			set
			{
				_showCultureInTooltip = value;
				CustomPatches.HarmonyPatches.PatchShowCultureInTooltip(_showCultureInTooltip);
			}
		}

		private bool _combatBalanceShowAllTroops = false;
		[SettingPropertyBool(
			"Consider all troops for combat balance",
			RequireRestart = false,
			HintText = 
			"Takes into account all troops for each side for the combat balance bar at the top of the screen" +
			"\nExclusive with 'Show all troops in scoreboard'!",
			Order = 5)]
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
				CustomPatches.HarmonyPatches.PatchCombatBalanceShowAllTroops(_combatBalanceShowAllTroops);
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
			Order = 6)]
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
				CustomPatches.HarmonyPatches.PatchScoreboardShowAllTroops(_scoreboardShowAllTroops);
				OnPropertyChanged(nameof(ScoreboardShowAllTroops));
			}
		}

		[SettingPropertyButton(
			"Add all equipment to inventory",
			RequireRestart = false,
			HintText = "This was implemented for testing purposes; it will flood your inventory with every possible equipment item!",
			Content = "ADD EQUIPMENT",
			Order = 7)]
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
							CustomPatches.Message($"Adding equipment: '{item.Name}'", false, Colors.White);
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
	}
}
