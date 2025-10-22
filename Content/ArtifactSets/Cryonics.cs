using System;
using System.Collections.Generic;
using Terraria.Localization;
using TheBindingOfRarria.Common.Systems;
using TheBindingOfRarria.Content.Items;

namespace TheBindingOfRarria.Content.ArtifactSets;

public class Cryonics : IArtifactSet
{
    public List<int> Items => [
        ItemID.FrozenTurtleShell,
        ItemID.ArcticDivingGear,
        ItemType<MedicalIceBag>()];

    public LocalizedText Name => Language.GetOrRegister($"Mods.TheBindingOfRarria.ArtifactSets.Cryonics.Name");

    public Color NameColor => Color.SkyBlue;

    public List<Predicate<int>> Artifacts { get; set; }

    public int Count { get; set; }

    public List<string> AccessoryNames { get; set; }

    public void Effect(int who)
    {
        if (Main.player[who].TryGetModPlayer<CryonicsPlayer>(out var p))
            p.cryonics = true;
    }
}

public class CryonicsPlayer : ModPlayer
{
    public bool cryonics = false;

    public override void Load()
    {
        //ArtifactSetSystem.ArtifactSets.Add(new Cryonics());
    }

    public override void PreUpdateBuffs()
    {
        if (Player.wet && cryonics)
        {
            Player.iceBarrier = true;
            Player.iceBarrierFrame = (byte)Main.GlobalTimeWrappedHourly;
            Player.AddBuff(BuffID.IceBarrier, 2);
        }

        cryonics = false;
    }
}