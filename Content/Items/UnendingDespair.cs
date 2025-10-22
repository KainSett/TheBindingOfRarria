using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using TheBindingOfRarria.Content.Projectiles;

namespace TheBindingOfRarria.Content.Items;

public class UnendingDespair : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;

    public override void SetDefaults()
    {
        Item.accessory = true;
        Item.width = 28;
        Item.height = 30;
        Item.rare = ItemRarityID.Expert;
        Item.value = Item.sellPrice(0, 7);
        Item.expert = true;
    }


    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        var p = player.GetModPlayer<LifeSuckerPlayer>();
        p.Sucker = true;

        p.counter++;

        if (p.counter >= 300)
        {
                // Make this use either an array or hashset please.
                // No, ms. List<T> hater.
            List<int> victims = [];

            for (int i = 0; i < 4; i++)
            {
                float dist = 325 * 325;
                victims.Add(-1);

                foreach (var t in Main.ActiveNPCs)
                {
                    if (!victims.Contains(t.whoAmI) && !t.friendly && t.lifeMax > 5 && t.CanBeChasedBy() && !t.immortal && !t.CountsAsACritter && t.Center.DistanceSQ(player.Center) < dist)
                    {
                        dist = t.Center.DistanceSQ(player.Center);
                        
                        victims[i] = t.whoAmI;
                    }
                }
                if (victims.Count == i + 1 && victims[i] != -1)
                {
                    p.counter = 0;
                    Projectile.NewProjectile(player.GetSource_Accessory(Item, "Unending Despair sucking"), Main.npc[victims[i]].Center, Main.npc[victims[i]].Center - player.Center, ModContent.ProjectileType<LifeSucker>(), player.statLifeMax2 / 10, 0, player.whoAmI, victims[i]);
                }
            }
        }
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        int index = tooltips.FindIndex(t => t.Name == "Tooltip0");
        if (index != -1)
        {
            string text = string.Format(Language.GetTextValue("Mods.TheBindingOfRarria.Items.UnendingDespair.Tooltip"), $"{Main.LocalPlayer.statLifeMax2 / 10}");

            text = text[..text.LastIndexOf($"\n")];
            text = text[..text.LastIndexOf($"\n")];
            tooltips[index].Text = text;
        }
    }
}

public class DespairLootNPC : GlobalNPC
{
    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        if (npc.type == NPCID.DungeonSpirit)
            npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsExpert(), ModContent.ItemType<UnendingDespair>(), 20));

        base.ModifyNPCLoot(npc, npcLoot);
    }
}

// ???????????????????????????
// It's so that I could have every lifesucker proj pool off their heal here in one place.
public class LifeSuckerPlayer : ModPlayer
{
    public int counter = 0;

    public int heal = 0;

    public bool Sucker = false;

    public override void ResetEffects()
    {
        if (!Sucker)
            heal = 0;

        Sucker = false;
    }

    public override void PostUpdate()
    {
        if (heal != 0)
        {
            Player.Heal(heal);
            heal = 0;
        }
    }
}