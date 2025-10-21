using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Terraria.Localization;
using Terraria.ModLoader.Default;
using Terraria.UI;
using TheBindingOfRarria.Content.ArtifactSets;
using TheBindingOfRarria.Content.Items;

namespace TheBindingOfRarria.Common.Systems;

public class ArtifactSetSystem : ModSystem
{
    public static List<IArtifactSet> ArtifactSets = [];

    public override void Unload()
    {
        ArtifactSets.Clear();
    }
}


public interface IArtifactSet
{
    public List<int> Items { get; }

    public List<Predicate<int>> Artifacts { get; set; }

    public LocalizedText Name { get; }

    public Color NameColor { get; }

    public void Update(int who)
    {
        if (Artifacts is null || Artifacts.Count <= 0)
            SetConditions();

        if (!Main.player[who].TryGetModPlayer<ArtifactSetPlayer>(out var p) || !Main.player[who].TryGetModPlayer<ModAccessorySlotPlayer>(out var plr))
            return;

        foreach (var item in Artifacts)
        {
            if (!plr.exAccessorySlot.Any(i => item(i.type)) && !Main.player[who].armor.Any(i => item(i.type)))
                continue;
            else 
                Count += 1;
        }
        if (Count < Artifacts.Count)
            return;

        Effect(who);
    }

    public void Effect(int who)
    {

    }

    public void SetConditions()
    {
        Artifacts = [];
        List<int> i = [];
        foreach (var item in Items)
        {
            var tree = Array.FindAll(Main.recipe, (r => r.HasIngredient(item)));

            i.Clear();
            foreach (var it in tree)
                i.Add(it.createItem.type);

            if (tree is null || i is null || i.Count <= 0)
            {
                Artifacts.Add(c => c == item);
            }

            else Artifacts.Add(c => i.Contains(c) || c == item);
        }
    }

    public bool Contains(int type)
    {
        return Artifacts.Any(art => art(type));
    }

    public int Count { get; set; }
}

public class ArtifactSetPlayer : ModPlayer
{
    public HashSet<IArtifactSet> Sets = [];

    public override void PostUpdateEquips()
    {
        foreach (var set in Sets)
            set.Update(Player.whoAmI);
    }

    public override void PreUpdate()
    {
        foreach (var set in Sets)
           set.Count = 0;
    }

    public override void Initialize()
    {
        Sets.Clear();
        foreach (var set in ArtifactSetSystem.ArtifactSets)
        {
            Sets.Add(set);
        }
    }
}

public class Artifact : GlobalItem
{
    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        if (Main.LocalPlayer.TryGetModPlayer<ArtifactSetPlayer>(out var p))
        {
            foreach (var set in p.Sets)
                if (set.Contains(item.type))
                {
                    var shift = Language.GetTextValue($"Mods.TheBindingOfRarria.ArtifactSets.Info.Closed");
                    var name = $"[c/{set.NameColor.Hex3()}:" + Language.GetTextValue($"Mods.TheBindingOfRarria.ArtifactSets.{set.Name}.Name") + ']';
                    if (Main.keyState.IsKeyDown(Keys.LeftShift) || Main.keyState.IsKeyDown(Keys.RightShift))
                    {
                        shift = $" ({set.Count}/{set.Artifacts.Count})";

                        var line1 = name + shift;

                        var tooltip = new TooltipLine(Mod, "ArtifactSetEffect", Language.GetTextValue($"Mods.TheBindingOfRarria.ArtifactSets.{set.Name}.Effect"));
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