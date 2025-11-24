using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using TheBindingOfRarria.Common.Helpers;
using TheBindingOfRarria.Content.Projectiles;

namespace TheBindingOfRarria.Content.Items;

public class RubberCement : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;

    public override void SetDefaults()
    {
        Item.accessory = true;
        Item.height = 32;
        Item.width = 24;
        Item.rare = ItemRarityID.LightRed;
        Item.value = Item.sellPrice(0, 1, 12);
    }

    public override void UpdateAccessory(Player player, bool hideVisual) => player.GetModPlayer<CementosPlayer>().Cementos = true;
}

public class CementosPlayer : ModPlayer
{
    public bool Cementos = false;

    public override void ResetEffects() => Cementos = false;
}

public class QueenSlimeBagLoot : GlobalItem
{
    public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
    {
        if (item.type == ItemID.QueenSlimeBossBag)
        {
            IItemDropRule rule = new ItemDropWithConditionRule(ModContent.ItemType<RubberCement>(), 4, 1, 1, new Conditions.IsExpert());
            itemLoot.Add(rule);
        }
    }
}

public class QueenSlimeLootNPC : GlobalNPC
{
    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        if (npc.type == NPCID.QueenSlimeBoss)
            npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<RubberCement>(), 6));

        base.ModifyNPCLoot(npc, npcLoot);
    }
}

public class GlobalBouncyProjectile : GlobalProjectile
{
    public override bool InstancePerEntity => true;
    private bool Bouncy = false;

    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {

        if (!projectile.Transform(projectile.hostile || Main.netMode == NetmodeID.Server))
            return;

        else if (Main.player[projectile.owner].GetModPlayer<CementosPlayer>().Cementos && projectile.tileCollide && projectile.aiStyle != ProjAIStyleID.Bounce)
            Bouncy = true;
    }

    public override bool OnTileCollide(Projectile projectile, Vector2 oldVelocity)
    {
        if (Bouncy && projectile.tileCollide)
        {
                // If the projectile hits the left or right side of the tile, reverse the X velocity.
            if (Math.Abs(projectile.velocity.X - oldVelocity.X) > 0)
                projectile.velocity.X = -oldVelocity.X;

                // If the projectile hits the top or bottom side of the tile, reverse the Y velocity.
            if (Math.Abs(projectile.velocity.Y - oldVelocity.Y) > 0)
                projectile.velocity.X = -oldVelocity.X;

                // Borrowing code from EM, yessir.
            projectile.timeLeft = (int)(projectile.timeLeft * 0.8f);
            projectile.timeLeft -= 60;
            projectile.velocity *= 0.7f;

            return false;
        }

        return base.OnTileCollide(projectile, oldVelocity);
    }
}