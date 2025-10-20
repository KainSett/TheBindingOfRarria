using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Localization;
using TheBindingOfRarria.Common.Systems;
using TheBindingOfRarria.Content.ArtifactSets;

namespace TheBindingOfRarria.Common.Helpers;

public static partial class Helper
{
    public static void InsertArtifactSetBonusTooltip(this List<TooltipLine> tooltips, int type)
    {
        if (!Main.LocalPlayer.TryGetModPlayer<ArtifactSetPlayer>(out var p) || !p.Sets.Any(l => l.Key.Contains(type)))
            return;

        string name = "";
        foreach (var set in p.Sets)
        {
            if (set.Key.Contains(type) && set.Value >= set.Key.Count)
                name = set.Key.Name.Value;
        }

        if (name == "")
            return;

        var text = Language.GetTextValue($"Mods.TheBindingOfRarria.ArtifactSets.{name}.Additional");
        var line = new TooltipLine("TheBindingOfRarria/ArtifactSetBonus", text)
        {
            OverrideColor = p.Sets.FirstOrDefault(s => s.Key.Contains(type)).Key.NameColor
        };


        var index = tooltips.FindLastIndex(t => t.Name.Contains("Tooltip"));

        if (index != -1)
            tooltips.Insert(index, line);
    }
}
