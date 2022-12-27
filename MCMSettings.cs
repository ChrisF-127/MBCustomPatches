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
			"Trade Demand Modifier",
			0.01f,
			100f,
			"0.00",
			RequireRestart = false,
			HintText = " [Native: 1.00]",
			Order = 3)]
		[SettingPropertyGroup(
			"Settings",
			GroupOrder = 0)]
		public float TradeDemandModifier { get; set; } = 1f;
	}
}
