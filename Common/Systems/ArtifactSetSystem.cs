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

    public string Name { get; }

    public Color NameColor { get; }

    public void Update(int who)
    {
        if (Artifacts is null || Artifacts.Count <= 0)
            SetConditions();

        if (!Main.player[who].TryGetModPlayer<ArtifactSetPlayer>(out var p) || !Main.player[who].TryGetModPlayer<ModAccessorySlotPlayer>(out var plr))
            return;

        foreach (var item in Artifacts)
        {
            if (!CheckEquipped(who, item, out var Item))
                continue;
            else
            {
                AccessoryNames[Artifacts.IndexOf(item)] = Item.Name;

                Count += 1;
            }
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
        AccessoryNames = [];
        foreach (var item in Items)
        {
            var tree = Array.FindAll(Main.recipe, (r => r.HasIngredient(item)));

            List<int> i = [];
            foreach (var it in tree)
                i.Add(it.createItem.type);

            if (tree is null || i is null || i.Count <= 0)
            {
                Artifacts.Add(c => c == item);
            }

            else
            {
                Artifacts.Add(c => i.Contains(c) || c == item);
            }

            AccessoryNames.Add(new Item(item).Name);
        }
    }

    public bool Contains(int type)
    {
        return Artifacts.Any(art => art(type));
    }

    public bool CheckEquipped(int who, Predicate<int> predicate, out Item item)
    {
        var p = Main.player[who].GetModPlayer<ArtifactSetPlayer>();

        var modded = Main.player[who].GetModPlayer<ModAccessorySlotPlayer>().exAccessorySlot.FirstOrDefault(i => predicate(i.type));
        var vanilla = Main.player[who].armor.FirstOrDefault(i => predicate(i.type));
        item = vanilla is null ? modded : vanilla;

        return p.Equipped.Any(i => predicate(i));
    }

    public List<string> AccessoryNames { get; set; }

    public int Count { get; set; }
}

public class ArtifactSetPlayer : ModPlayer
{
    public HashSet<IArtifactSet> Sets = [];

    public List<int> Equipped = [];

    public override void PostUpdateEquips()
    {
        foreach (var set in Sets)
            set.Update(Player.whoAmI);
    }

    public override void PreUpdate()
    {
        foreach (var set in Sets)
           set.Count = 0;

        Equipped.Clear();
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
    public override void UpdateAccessory(Item item, Player player, bool hideVisual)
    {
        player.GetModPlayer<ArtifactSetPlayer>().Equipped.Add(item.type);
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        if (!tooltips.Any(t => t.Name.Contains("Social")) && Main.LocalPlayer.TryGetModPlayer<ArtifactSetPlayer>(out var p))
        {
            foreach (var set in p.Sets)
                if (set.Contains(item.type))
                {
                    var shift = Language.GetTextValue($"Mods.TheBindingOfRarria.ArtifactSets.Info.Closed");
                    var name = $"[c/{set.NameColor.Hex3()}:" + Language.GetTextValue($"Mods.TheBindingOfRarria.ArtifactSets.{set.Name}.Name");
                    if (Main.keyState.IsKeyDown(Keys.LeftShift) || Main.keyState.IsKeyDown(Keys.RightShift))
                    {
                        shift = string.Format(Language.GetTextValue($"Mods.TheBindingOfRarria.ArtifactSets.Info.Equipped"), set.Count, set.Artifacts.Count);

                        var line1 = name + shift;

                        var tooltip = new TooltipLine(Mod, "ArtifactSetEffect", Language.GetTextValue($"Mods.TheBindingOfRarria.ArtifactSets.{set.Name}.Effect"));
                        tooltip.OverrideColor = set.Count >= set.Artifacts.Count ? Color.White : Color.Gray;

                        var n = new TooltipLine(Mod, "ArtifactSet", line1);

                        var index = tooltips.LastIndexOf(tooltips.LastOrDefault(t => t.Name != "ArtifactSetBonus" && t.Name.Contains("Artifact")));

                        tooltips.Insert(index == -1 ? 1 : index + 1, tooltip);
                        tooltips.Insert(index == -1 ? 1 : index + 1, n);
                        for (int i = (index == -1 ? 0 : index) + 3; i < tooltips.Count; i++)
                            tooltips[i].Hide();

                        tooltip = new TooltipLine(Mod, "ArtifactSetInfoConsists", /*"\n" + */Language.GetTextValue($"Mods.TheBindingOfRarria.ArtifactSets.Info.Consists"));
                        tooltips.Add(tooltip);

                        var names = set.AccessoryNames;
                        foreach (var na in names)
                        {
                            var nam = new TooltipLine(Mod, "ArtifactNames", na);
                            if (!set.CheckEquipped(Main.myPlayer, set.Artifacts[names.IndexOf(na)], out var Item))
                                nam.OverrideColor = Color.Gray;

                                tooltips.Add(nam);
                        }

                        continue;
                    }
                    var text = name + shift;

                    var t = new TooltipLine(Mod, "ArtifactSet", text);

                    tooltips.Insert(1, t);
                }
        }
    }
}