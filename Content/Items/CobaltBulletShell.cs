using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TheBindingOfRarria.Content.Buffs.Debuffs;

namespace TheBindingOfRarria.Content.Items;

public class CobaltBulletShell : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;

    public override void SetDefaults()
    {
        Item.accessory = true;
        Item.width = 18;
        Item.height = 36;
        Item.rare = ItemRarityID.LightRed;
        Item.value = Item.sellPrice(0, 0, 22, 8);
    }

    public override void UpdateAccessory(Player player, bool hideVisual) => player.GetModPlayer<FerromagneticPlayer>().Ferromagnetic = true;
    
    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.CobaltBar)
            .AddIngredient(ItemID.EmptyBullet)
            .AddTile(TileID.Anvils)
            .Register();
    }
}

public class FerromagneticPlayer : ModPlayer
{
    public bool Ferromagnetic = false;

    public override void ResetEffects() => Ferromagnetic = false;
    
    public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (Ferromagnetic && BulletGlobalProjectile.Bullet.Contains(proj.type))
            target.AddBuff(ModContent.BuffType<MagneticField>(), 240);
    }
}

public class BulletGlobalProjectile : GlobalProjectile
{
    public static HashSet<int> Bullet = [];

    public override bool InstancePerEntity => true;

    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        Player owner = Main.player[projectile.owner];

        if (!Bullet.Contains(projectile.type) && projectile.aiStyle == ProjAIStyleID.Arrow && projectile.friendly && !owner.HeldItem.IsAir && owner.HeldItem != null && owner.ChooseAmmo(owner.HeldItem) != null && owner.ChooseAmmo(owner.HeldItem).ammo == AmmoID.Bullet)
            Bullet.Add(projectile.type);
        
    }
}