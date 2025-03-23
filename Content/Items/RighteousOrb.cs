using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Graphics;
using Terraria.UI;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework.Input;

namespace TheBindingOfRarria.Content.Items
{
    public class RighteousOrb : ModItem
    {
        private static ModKeybind righteousCrackKey;
        private static int lastFiredTime = 0; 

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;

            Item.maxStack = 1;

            Item.value = Item.buyPrice(gold: 1, silver: 50);
            Item.rare = ItemRarityID.Orange;

            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            
            player.moveSpeed += 0.5f;
            player.wingTimeMax = (int)(player.wingTimeMax * 2f);

            
            if (righteousCrackKey.JustPressed)
            {
               
                if (Main.time - lastFiredTime >= 300)
                {
                    FireProjectile(player);
                    lastFiredTime = (int)Main.time; 
                }
            }
        }

        
        public override void Load()
        {
            righteousCrackKey = KeybindLoader.RegisterKeybind(Mod, "Righteous Crack", Keys.C); 
        }


        private void FireProjectile(Player player)
        {
            
            int proj = Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.position.X, player.position.Y, 0f, 0f, ModContent.ProjectileType<Projectiles.RighteousCrack>(), 20, 1f, player.whoAmI);

          
            Main.projectile[proj].velocity = Vector2.Zero;
        }
    }
}