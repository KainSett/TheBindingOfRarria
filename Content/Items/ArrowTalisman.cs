
namespace TheBindingOfRarria.Content.Items;

public class ArrowTalisman : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;

    public override void SetStaticDefaults()
    {
        ItemID.Sets.ShimmerTransformToItem[ItemID.BoneArrow] = Type;
        ItemID.Sets.ShimmerTransformToItem[ItemID.BowStatue] = Type;
    }

    public override void SetDefaults()
    {
        Item.width = 34;
        Item.height = 36;
        Item.accessory = true;
        Item.rare = ItemRarityID.Gray;
        Item.value = Item.sellPrice(0, 0, 10);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<ArrowTalismanPlayer>().Yus = true;
    }
}

public class ArrowTalismanPlayer : ModPlayer
{
    public bool Yus = false;

    public override void ResetEffects()
    {
        Yus = false;
    }
}

public class TalismanArrowProj : GlobalProjectile
{
    public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
    {
        return entity.friendly && entity.aiStyle == ProjAIStyleID.Arrow;
    }

    public override bool InstancePerEntity => true;

    public Vector2 velocity = Vector2.Zero;

    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        var owner = Main.player[projectile.owner];
        if (!owner.HeldItem.IsAir && owner.HeldItem != null && owner.ChooseAmmo(owner.HeldItem) != null && owner.ChooseAmmo(owner.HeldItem).ammo == AmmoID.Arrow && owner.TryGetModPlayer<ArrowTalismanPlayer>(out var p) && p.Yus)
        {
            velocity = projectile.velocity * 1.2f;
            projectile.damage = (int)(1.2f * projectile.damage);
        }
    }

    public override void PostAI(Projectile projectile)
    {
        if (velocity != Vector2.Zero)
            projectile.velocity = velocity;
    }
}