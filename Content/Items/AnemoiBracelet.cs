using System;
using Terraria;
using Terraria.ModLoader;

namespace TheBindingOfRarria.Content.Items
{
    public class AnemoiBracelet : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 24;
            Item.accessory = true;
        }
    }
    public class NatureDodgePlayer : ModPlayer
    {
        public bool HasBracelet = false;
        public override void PreUpdate()
        {
            HasBracelet = false;
        }
        public override void UpdateEquips()
        {
            if (Array.FindIndex(Player.armor, item => item.type == ModContent.ItemType<AnemoiBracelet>() && !item.social) != -1)
                HasBracelet = true;
        }
        public override bool FreeDodge(Player.HurtInfo info)
        {
            if (!HasBracelet)
                return base.FreeDodge(info);

            var rand = new Random();
            var chance = Math.Min(0.07 + Player.moveSpeed / 10, 0.21);
            return rand.NextDouble() < chance;
        }
    }
}