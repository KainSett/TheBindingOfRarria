namespace TheBindingOfRarria.Content.Items;

public class ReaperTalisman : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;

    public override void SetStaticDefaults()
    {
        ItemID.Sets.ShimmerTransformToItem[ItemID.ReaperBanner] = Type;
        ItemID.Sets.ShimmerTransformToItem[ItemID.ReaperStatue] = Type;
    }

    public override void SetDefaults()
    {
        Item.width = 30;
        Item.height = 34;
        Item.accessory = true;
        Item.rare = ItemRarityID.Gray;
        Item.value = Item.sellPrice(0, 0, 10);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<ReaperTalismanPlayer>().DarkHarvest = true;
    }
}

public class ReaperTalismanPlayer : ModPlayer
{
    public bool DarkHarvest = false;

    public override void ResetEffects()
    {
        DarkHarvest = false;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        if (DarkHarvest && target.life < target.lifeMax / 2)
        {
            modifiers.ArmorPenetration += 6;
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (DarkHarvest && Player.lifeSteal > 0 && target.canGhostHeal && target.life + hit.Damage < target.lifeMax / 2)
        {
            Player.Heal(hit.Damage / 33);
            Player.lifeSteal -= hit.Damage / 33;
        }
    }
}