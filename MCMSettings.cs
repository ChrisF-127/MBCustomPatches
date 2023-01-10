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

namespace CustomPatches
{
	public class MCMSettings : AttributeGlobalSettings<MCMSettings>
	{
		public override string Id => "CustomPatches";
		public override string DisplayName => "Custom Patches";
		public override string FolderName => "CustomPatches";
		public override string FormatType => "json";

		[SettingPropertyBool(
			"Unlock all smithing parts",
			RequireRestart = false,
			HintText = "Unlock all smithing parts",
			Order = 0)]
		[SettingPropertyGroup(
			"Settings",
			GroupOrder = 0)]
		public bool UnlockAllParts { get; set; } = false;

		[SettingPropertyButton(
			"Give me Everything",
			RequireRestart = false,
			HintText = "HALP!",
			Content = "HIT ME!",
			Order = 1)]
		[SettingPropertyGroup(
			"Settings",
			GroupOrder = 0)]
		public Action AllEquipmentButton { get; set; } = () => 
		{
			var mainParty = Campaign.Current?.MainParty;
			if (mainParty != null)
			{
				foreach (var item in Items.All)
				{
					if (!item.IsAnimal && !item.IsFood && !item.IsTradeGood && item.Culture == mainParty.LeaderHero.Culture /*&& !mainParty.ItemRoster.Any(ir => ir.EquipmentElement.Item == item)*/)
					{
						FileLog.Log($"Adding '{item.Name}'");
						mainParty.ItemRoster.Add(new ItemRosterElement(item, 1));
					}
				}
				InformationManager.DisplayMessage(new InformationMessage($"Done!", new Color(0f, 1f, 0f)));
			}
			else
				InformationManager.DisplayMessage(new InformationMessage("Game not running", new Color(1f, 0f, 0f)));
		};

		[SettingPropertyFloatingInteger(
			"Party Speed Modifier",
			0.01f,
			100f,
			"0.00",
			RequireRestart = false,
			HintText = " [Native: 1.00]",
			Order = 2)]
		[SettingPropertyGroup(
			"Settings",
			GroupOrder = 0)]
		public float PartySpeedModifier { get; set; } = 1f;

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
		public float CrosshairOpacity { get; set; } = 1f;



		#region MISSILE PIERCE
		[SettingPropertyBool(
			"Only player missiles pierce",
			RequireRestart = false,
			HintText = " [Native: false]",
			Order = 0)]
		[SettingPropertyGroup(
			"General",
			GroupOrder = 0)]
		public bool OnlyPlayerMissilesPierce { get; set; } = false;

		[SettingPropertyBool(
			"Pierce on killing blow only",
			RequireRestart = false,
			HintText = " [Native: false]",
			Order = 1)]
		[SettingPropertyGroup(
			"General",
			GroupOrder = 0)]
		public bool PierceOnKillingBlowOnly { get; set; } = false;

		[SettingPropertyBool(
			"Piercing Arrows",
			RequireRestart = false,
			HintText = " [Native: false]",
			IsToggle = true,
			Order = 0)]
		[SettingPropertyGroup(
			"Piercing Arrows",
			GroupOrder = 1)]
		public bool EnableArrowPierce { get; set; } = false;
		[SettingPropertyFloatingInteger(
			"Damage retained per Hit",
			0f,
			1f,
			"0%",
			RequireRestart = false,
			HintText = " [Native: 25%]",
			Order = 1)]
		[SettingPropertyGroup(
			"Piercing Arrows",
			GroupOrder = 1)]
		public float ArrowPierceDamageRetained { get; set; } = 0.25f;
		[SettingPropertyFloatingInteger(
			"Damage ratio required to pierce through Armor",
			0f,
			1f,
			"0%",
			RequireRestart = false,
			HintText = " [Native: 80%]",
			Order = 2)]
		[SettingPropertyGroup(
			"Piercing Arrows",
			GroupOrder = 1)]
		public float ArrowPierceRatioRequiredThroughArmor { get; set; } = 0.80f;

		[SettingPropertyBool(
			"Piercing Bolts",
			RequireRestart = false,
			HintText = " [Native: false]",
			IsToggle = true,
			Order = 0)]
		[SettingPropertyGroup(
			"Piercing Bolts",
			GroupOrder = 2)]
		public bool EnableBoltPierce { get; set; } = false;
		[SettingPropertyFloatingInteger(
			"Damage retained per Hit",
			0f,
			1f,
			"0%",
			RequireRestart = false,
			HintText = " [Native: 33%]",
			Order = 1)]
		[SettingPropertyGroup(
			"Piercing Bolts",
			GroupOrder = 2)]
		public float BoltPierceDamageRetained { get; set; } = 0.33f;
		[SettingPropertyFloatingInteger(
			"Damage ratio required to pierce through Armor",
			0f,
			1f,
			"0%",
			RequireRestart = false,
			HintText = " [Native: 75%]",
			Order = 2)]
		[SettingPropertyGroup(
			"Piercing Bolts",
			GroupOrder = 2)]
		public float BoltPierceRatioRequiredThroughArmor { get; set; } = 0.75f;

		[SettingPropertyBool(
			"Piercing Javelins",
			RequireRestart = false,
			HintText = " [Native: false]",
			IsToggle = true,
			Order = 0)]
		[SettingPropertyGroup(
			"Piercing Javelins",
			GroupOrder = 3)]
		public bool EnableJavelinPierce { get; set; } = false;
		[SettingPropertyFloatingInteger(
			"Damage retained per Hit",
			0f,
			1f,
			"0%",
			RequireRestart = false,
			HintText = " [Native: 20%]",
			Order = 1)]
		[SettingPropertyGroup(
			"Piercing Javelins",
			GroupOrder = 3)]
		public float JavelinPierceDamageRetained { get; set; } = 0.20f;
		[SettingPropertyFloatingInteger(
			"Damage ratio required to pierce through Armor",
			0f,
			1f,
			"0%",
			RequireRestart = false,
			HintText = " [Native: 67%]",
			Order = 2)]
		[SettingPropertyGroup(
			"Piercing Javelins",
			GroupOrder = 3)]
		public float JavelinPierceRatioRequiredThroughArmor { get; set; } = 0.67f;
		#endregion
	}
}
