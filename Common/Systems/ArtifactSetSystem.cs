using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Terraria.Localization;
using Terraria.UI;
using TheBindingOfRarria.Content.ArtifactSets;
using TheBindingOfRarria.Content.Items;

namespace TheBindingOfRarria.Common.Systems;

public class ArtifactSetSystem : ModSystem
{
    public static List<ArtifactSet> ArtifactSets = [];

    public override void Unload()
    {
        ArtifactSets.Clear();
    }
}

public class ArtifactSet(LocalizedText name, Color color, Action<int> effect, params Predicate<int>[] conditions)
{
    private readonly List<Predicate<int>> Artifacts = [.. conditions];

    public readonly LocalizedText Name = name;

    public readonly Color NameColor = color;

    private readonly Action<int> Effect = effect;

    public void Update(int who, params int[] items)
    {
        if (!Main.player[who].TryGetModPlayer<ArtifactSetPlayer>(out var p))
            return;

        foreach (var item in Artifacts)
        {
            if (!items.Any(i => item(i)))
                continue;
            else 
                p.Sets[this] += 1;
        }
        if (p.Sets[this] < 3)
            return;

        Effect.Invoke(who);
    }

    public bool Contains(int type)
    {
        return Artifacts.Any(art => art.Invoke(type));
    }

    public int Count => Artifacts.Count;
}

public class ArtifactSetPlayer : ModPlayer
{
    public Dictionary<ArtifactSet, int> Sets = [];

    public List<int> Artifacts = [];

    public override void PostUpdateEquips()
    {
        foreach(var set in Sets)
        {
            set.Key.Update(Player.whoAmI, [..Artifacts]);
        }
    }

    public override void PreUpdate()
    {
        Artifacts.Clear();
        foreach (var set in Sets)
            Sets[set.Key] = 0;
    }

    public override void Initialize()
    {
        foreach (var set in ArtifactSetSystem.ArtifactSets)
        {
            Sets.Add(set, 0);
        }
    }
}

public class Artifact : GlobalItem
{
    public override void UpdateAccessory(Item item, Player player, bool hideVisual)
    {
        if (player.TryGetModPlayer<ArtifactSetPlayer>(out var p))
        {
            p.Artifacts.Add(item.type);
        }
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        if (Main.LocalPlayer.TryGetModPlayer<ArtifactSetPlayer>(out var p))
        {
            foreach (var set in p.Sets)
                if (set.Key.Contains(item.type))
                {
                    var shift = Language.GetTextValue($"Mods.TheBindingOfRarria.ArtifactSets.Info.Closed");
                    var name = $"[c/{set.Key.NameColor.Hex3()}:" + Language.GetTextValue($"Mods.TheBindingOfRarria.ArtifactSets.{set.Key.Name}.Name") + ']';
                    if (Main.keyState.IsKeyDown(Keys.LeftShift) || Main.keyState.IsKeyDown(Keys.RightShift))
                    {
                        shift = $" ({set.Value}/{set.Key.Count})";

                        var line1 = name + shift;

                        var tooltip = new TooltipLine(Mod, "ArtifactSetEffect", Language.GetTextValue($"Mods.TheBindingOfRarria.ArtifactSets.{set.Key.Name}.Effect"));
                        var n = new TooltipLine(Mod, "ArtifactSet", line1);
                        tooltips.Insert(1, tooltip);
                        tooltips.Insert(1, n);
                        for (int i = 3; i < tooltips.Count; i++)
                            tooltips[i].Hide();
                        return;
                    }
                    var text = name + shift;

                    var t = new TooltipLine(Mod, "ArtifactSet", text);

                    tooltips.Insert(1, t);
                }
        }
    }
}