
using System;
using System.IO;
using System.Linq;
using Terraria.Audio;
using Terraria.ModLoader.IO;
using TheBindingOfRarria.Common.Helpers;
using TheBindingOfRarria.Content.Projectiles;

namespace TheBindingOfRarria.Content.Items;

public class HandBallista : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;

    public override void SetDefaults()
    {
        Item.height = 100;
        Item.width = 100;
        Item.damage = 135;
        Item.DamageType = DamageClass.Ranged;
        Item.noUseGraphic = true;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.noMelee = true;
        Item.useTime = 75;
        Item.useAnimation = 75;
        Item.value = Item.sellPrice(gold: 1, silver: 30);
        Item.useAmmo = AmmoID.Arrow;
        Item.shootsEveryUse = true;
        Item.shootSpeed = 9;
        Item.shoot = ProjectileID.WoodenArrowFriendly;
        Item.rare = ItemRarityID.Expert;
        Item.expert = true;
        Item.knockBack = 5f;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        if (type == ProjectileID.WoodenArrowFriendly)
            type = ModContent.ProjectileType<BallistaBolt>();
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Projectile.NewProjectile(source, position, velocity, type, damage, knockback);

        return false;
    }

    public override bool CanShoot(Player player)
    {
        return false;
    }

    public override void HoldItem(Player player)
    {
        if (Main.myPlayer != player.whoAmI)
            return;

        if (player.ItemAnimationActive)
        {
            var p = Main.projectile.FirstOrDefault(t => t.active && t.owner == player.whoAmI && t.type == ModContent.ProjectileType<HandBallistaProj>());
            if (p is null)
                return;

            player.direction = p.spriteDirection;
        }
        if (player.itemAnimation == (int)(75 / 3f) && player.whoAmI == Main.myPlayer)
        {
            var shoot = player.PickAmmo(Item, out var proj, out var speed, out var damage, out var knockBack, out var ammo);
            if (!shoot)
                return;

            var p = Main.projectile.FirstOrDefault(t => t.active && t.owner == player.whoAmI && t.type == ModContent.ProjectileType<HandBallistaProj>());
            if (p is null)
                return;

            var pos = player.Center + player.Center.DirectionTo(p.Center + new Vector2(0, 3)) * 70;
            var vel = player.Center.DirectionTo(p.Center + new Vector2(0, 5)) * Item.shootSpeed * speed;

            ModifyShootStats(player, ref pos, ref vel, ref proj, ref damage, ref knockBack);
            Shoot(player, new EntitySource_ItemUse_WithAmmo(player, Item, ammo), pos, vel, proj, damage, knockBack);

            player.velocity -= vel / 20;

            SoundEngine.PlaySound(SoundID.DD2_BallistaTowerShot);
        }
    }

    public override void UseAnimation(Player player)
    {
        if (Main.myPlayer != player.whoAmI)
            return;

        var mousePos = Main.MouseWorld;
        var proj = Projectile.NewProjectileDirect(player.GetSource_ItemUse_WithPotentialAmmo(Item, AmmoID.Arrow), player.Center + player.Center.DirectionTo(mousePos) * 15 + new Vector2(0, 5), player.Center.DirectionTo(mousePos), ModContent.ProjectileType<HandBallistaProj>(), 0, 0);
        
    }

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        var texture = TextureAssets.Item[Type].Value;
        frame.Height /= 4;
        spriteBatch.Draw(texture, position + new Vector2(0, frame.Height * 6 * scale), frame, drawColor, 0, origin, scale * 4, SpriteEffects.None, 0);

        return false;
    }

    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
    {
        var texture = TextureAssets.Item[Type].Value;
        var frame = texture.Frame(1, 4, 0, 0, 0, -2);
        spriteBatch.Draw(texture, Item.Bottom - Main.screenPosition - new Vector2(0, frame.Height / 2), frame, lightColor.MultiplyRGBA(alphaColor), rotation, frame.Size() / 2, scale, SpriteEffects.None, 0);

        return false;
    }
}

public class BallistaItemNPCShop : GlobalNPC
{
    public override void ModifyActiveShop(NPC npc, string shopName, Item[] items)
    {
        // tavernkeep
        if (npc.type == NPCID.DD2Bartender && Main.expertMode)
        {
            var index = Array.FindIndex(items, e => e == null);

            if (index != -1)
                items[index] = new Item(ModContent.ItemType<HandBallista>())
                {
                    shopCustomPrice = 10,
                    shopSpecialCurrency = CustomCurrencyID.DefenderMedals

                };

        }
    }
}