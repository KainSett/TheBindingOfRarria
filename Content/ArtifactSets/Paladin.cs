using System.Linq;
using Terraria.Localization;
using TheBindingOfRarria.Common.Systems;
using TheBindingOfRarria.Content.Items;

namespace TheBindingOfRarria.Content.ArtifactSets;

public class Paladin : ModPlayer
{
    public bool paladin = false;

    public override void Load()
    {
        On_Player.UpdateLifeRegen += PaladinRegen;

        ArtifactSetSystem.ArtifactSets.Add(new ArtifactSet(Language.GetOrRegister($"Mods.{Mod.Name}.ArtifactSets.{Name}.Name"),
            Color.PaleGoldenrod,
            (plr) =>
            {
                if (Main.player[plr].TryGetModPlayer<Paladin>(out var p))
                    p.paladin = true;
            },
        i => i == ItemID.PaladinsShield || Main.recipe.Any(r => r.HasIngredient(ItemID.PaladinsShield) && r.HasResult(i)),
        i => i == ItemID.CrossNecklace || Main.recipe.Any(r => r.HasIngredient(ItemID.CrossNecklace) && r.HasResult(i)),
        i => i == ItemType<CursedChain>() || Main.recipe.Any(r => r.HasIngredient(ItemType<CursedChain>()) && r.HasResult(i))
        ));
    }

    private static void PaladinRegen(On_Player.orig_UpdateLifeRegen orig, Player self)
    {
        if (self.immune && self.TryGetModPlayer<Paladin>(out var p) && p.paladin) 
        { 
            self.lifeRegen = (int)(self.lifeRegen * 1.5f);

            p.paladin = false;
        }

        else orig(self);
    }
}