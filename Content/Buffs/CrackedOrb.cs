using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Graphics;
using Terraria.UI;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TheBindingOfRarria.Content.Buffs
{
    public class CrackedOrb : ModBuff
    {
       
        public void SetDefaults()
        {
            // DisplayName.SetDefault("Cracked Orb");
            // Description.SetDefault("Increases movement speed and wing time.");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            
            player.moveSpeed += 0.3f;
            player.wingTimeMax = (int)(player.wingTimeMax * 1.5f);
        }
    }
}
