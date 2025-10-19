using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.Localization;
using TheBindingOfRarria.Content.ArtifactSets;
using TheBindingOfRarria.Content.Items;

namespace TheBindingOfRarria.Common.Systems;

public class ArtifactSetSystem : ModSystem
{
    public static List<ArtifactSet> ArtifactSets = [];
}

public class ArtifactSet(LocalizedText name, Color color, Action<int> effect, params Predicate<int>[] conditions)
{
    private readonly List<Predicate<int>> Artifacts = [.. conditions];

    public readonly LocalizedText Name = name;

    public readonly Color NameColor = color;

    private readonly Action<int> Effect = effect;

    public void Update(int who, params int[] items)
    {
        foreach (var item in Artifacts)
        {
            if (!items.Any(i => item(i)))
                return;
        }

        Effect.Invoke(who);
    }

    public bool Contains(int type)
    {
        return Artifacts.Any(art => art.Invoke(type));
    }
}

public class ArtifactSetPlayer : ModPlayer
{
    public List<ArtifactSet> Sets = ArtifactSetSystem.ArtifactSets;

    public List<int> Artifacts = [];

    public override void PostUpdateEquips()
    {
        foreach(var set in Sets)
        {
            set.Update(Player.whoAmI, [..Artifacts]);
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
                if (set.Contains(item.type))
                {
                    var shift = Main.keyState.IsKeyDown(Keys.LeftShift) || Main.keyState.IsKeyDown(Keys.RightShift) ? "Open" : "Closed";
                    var t = new TooltipLine(Mod, "ArtifactSet", $"[c/{set.NameColor.Hex4()}:" + Language.GetOrRegister($"Mods.{Mod.Name}.ArtifactSets.{set.Name}.Name").Value + ']' + Language.GetOrRegister($"Mods.{Mod.Name}.ArtifactSets.Info.{shift}").Value);

                    tooltips.Insert(1, t);
                }
        }
    }
}