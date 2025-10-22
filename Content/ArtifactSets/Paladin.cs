using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.Localization;
using TheBindingOfRarria.Common.Systems;
using TheBindingOfRarria.Content.Items;

namespace TheBindingOfRarria.Content.ArtifactSets;

public class Paladin : IArtifactSet
{
    public List<int> Items => [
        ItemID.PaladinsShield,
        ItemID.CrossNecklace,
        ItemType<CursedChain>()];

    public LocalizedText Name => Language.GetOrRegister($"Mods.TheBindingOfRarria.ArtifactSets.Paladin.Name");

    public Color NameColor => Color.PaleGoldenrod;

    public List<Predicate<int>> Artifacts { get; set; }

    public int Count { get; set; }

    public List<string> AccessoryNames { get; set; }

    public void Effect(int who)
    {
        if (Main.player[who].TryGetModPlayer<PaladinPlayer>(out var p))
            p.paladin = true;
    }
}

public class PaladinPlayer : ModPlayer
{
    public bool paladin = false;

    public override void Load()
    {
        On_Player.UpdateLifeRegen += PaladinRegen;

        ArtifactSetSystem.ArtifactSets.Add(new Paladin());
    }

    private static void PaladinRegen(On_Player.orig_UpdateLifeRegen orig, Player self)
    {
        if (self.immune && self.TryGetModPlayer<PaladinPlayer>(out var p) && p.paladin) 
        { 
            self.lifeRegen = (int)(self.lifeRegen * 1.5f);

            p.paladin = false;
        }

        else orig(self);
    }
}