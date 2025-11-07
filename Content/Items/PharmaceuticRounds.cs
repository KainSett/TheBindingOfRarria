using System;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TheBindingOfRarria.Content.Projectiles;

namespace TheBindingOfRarria.Content.Items;

public class PharmaceuticRounds : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 99;
    }

    public override void SetDefaults()
    {
        Item.damage = 9; 
        Item.DamageType = DamageClass.Ranged;
        Item.width = 32;
        Item.height = 30;
        Item.maxStack = Item.CommonMaxStack;
        Item.consumable = true;
        Item.knockBack = 2f;
        Item.value = 30;
        Item.expert = true;
        Item.shoot = ModContent.ProjectileType<PharmaceuticBullet>(); 
        Item.shootSpeed = 4f; 
        Item.ammo = AmmoID.Bullet; 
    }
}

public partial class NurseItemsShop : GlobalNPC
{
    public override void ModifyActiveShop(NPC npc, string shopName, Item[] items)
    {
        if (npc.type == NPCID.ArmsDealer && Main.expertMode && Main.npc.Any(target => target.type == NPCID.Nurse && target.Center.DistanceSQ(npc.Center) <= 800 * 800))
        {
            var index = Array.FindIndex(items, e => e == null);

            if (index != -1)
                items[index] = new Item(ModContent.ItemType<PharmaceuticRounds>());

        }

        AddBloodBag(npc, ref items);
        //AddHealLocket(npc, ref items);
    }
}