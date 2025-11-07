using Terraria.GameContent.ItemDropRules;
using TheBindingOfRarria.Content.Projectiles;

namespace TheBindingOfRarria.Content.Items;

public class DragonFire : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;

    public override void SetDefaults()
    {
        Item.accessory = true;
        Item.width = 40;
        Item.height = 40;
        Item.rare = ItemRarityID.Yellow;
        Item.value = Item.sellPrice(0, 0, 7, 20);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<DragonAuraPlayer>().dragon = true;
        player.maxTurrets += 2;
    }
}

public class DragonLootNPC : GlobalNPC
{
    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        if (npc.type == NPCID.DD2Betsy)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DragonFire>(), 5));
        }
        base.ModifyNPCLoot(npc, npcLoot);
    }
}

public class DragonAuraPlayer : ModPlayer
{
    public bool dragon = false;

    public override void ResetEffects() => dragon = false;

    public int counter = 0;

    public override void PostUpdate()
    {
        counter = (counter + 1) % 60;
    }
}

public class DragonAuraTurrets : GlobalProjectile
{
    public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
    {
        return entity.sentry;
    }

    public override void AI(Projectile projectile)
    {
        if (Main.player[projectile.owner].TryGetModPlayer<DragonAuraPlayer>(out var p) && p.dragon && p.counter == 0 && Main.myPlayer == projectile.owner)
            Projectile.NewProjectile(projectile.GetProjectileSource_FromThis(), projectile.Center, Vector2.Zero, ModContent.ProjectileType<DragonAura>(), 40, 2);
    }
}