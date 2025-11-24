using Terraria.Localization;
using TheBindingOfRarria.Content.Buffs;
using TheBindingOfRarria.Content.Projectiles;

namespace TheBindingOfRarria.Content.Items;

public class MagmaBell : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;

    public override void SetDefaults()
    {
        Item.width = 36;
        Item.height = 46;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 4);
        Item.rare = ItemRarityID.Orange;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<MagmaPlayer>().protection = Item;
        player.buffImmune[BuffID.Burning] = true;
        player.buffImmune[BuffID.OnFire] = true;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.Bell)
            .AddIngredient(ItemID.MagmaStone)
            .AddIngredient(ModContent.ItemType<PaleOre>(), 10)
            .AddTile(TileID.TinkerersWorkbench)
            .Register();
    }
}
public class MagmaPlayer : ModPlayer
{
    public Item protection = null;

    public int counter = 0;

    public override void ResetEffects()
    {
        protection = null;
    }

    public override void PostUpdateEquips()
    {
        counter = (int)MathHelper.Max(0, counter - 1);
    }

    public override bool FreeDodge(Player.HurtInfo info)
    {
        if (protection != null && counter <= 0 && info.Dodgeable && info.Damage <= 35)
        {
            Player.immune = true;
            int time = Player.longInvince ? 100 : 50;
            Player.SetImmuneTimeForAllTypes(time);
            counter = 420;
            Projectile.NewProjectile(Player.GetSource_Accessory_OnHurt(protection, info.DamageSource), Player.Center, Vector2.Zero, ModContent.ProjectileType<MagmaShield>(), 0, 0, Player.whoAmI);
            return true;
        }
        else
            return base.FreeDodge(info);
    }

    public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
    {
        if (attempt.veryrare && attempt.inLava && attempt.CanFishInLava && Main.rand.NextBool(5))
        {
            npcSpawn = -1;
            sonar.Color = Color.Orange;
            sonar.Text = Language.GetTextValue("Mods.TheBindingOfRarria.Items.MagmaBell.DisplayName");
            itemDrop = ModContent.ItemType<MagmaBell>();
        }
    }
}