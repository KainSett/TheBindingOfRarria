using System.Linq;
using Terraria.Localization;
using TheBindingOfRarria.Common.Systems;
using TheBindingOfRarria.Content.Items;

namespace TheBindingOfRarria.Content.ArtifactSets;

public class PaladinPlayer : ModPlayer
{
    public bool Paladin = false;

    //public int stored = 0;

    public override void Load()
    {
        On_Player.UpdateLifeRegen += PaladinRegen;

        ArtifactSetSystem.ArtifactSets.Add(new ArtifactSet(Language.GetOrRegister($"Mods.{Mod.Name}.ArtifactSets.Paladin.Name"),
            Color.PaleGoldenrod,
            (plr) =>
            {
                if (Main.player[plr].TryGetModPlayer<PaladinPlayer>(out var p))
                    p.Paladin = true;
            },
        i => i == ItemID.PaladinsShield || Main.recipe.Any(r => r.HasIngredient(i) && r.HasResult(ItemID.PaladinsShield)),
        i => i == ItemID.CrossNecklace || Main.recipe.Any(r => r.HasIngredient(i) && r.HasResult(ItemID.CrossNecklace)),
        i => i == ItemType<CursedChain>() || Main.recipe.Any(r => r.HasIngredient(i) && r.HasResult(ItemType<CursedChain>()))
        ));
    }

    private static void PaladinRegen(On_Player.orig_UpdateLifeRegen orig, Player self)
    {
        if (self.immune && self.TryGetModPlayer<PaladinPlayer>(out var p) && p.Paladin) 
        { 
            self.lifeRegen = (int)(self.lifeRegen * 1.5f);

            p.Paladin = false;
        }

        else orig(self);
    }
}