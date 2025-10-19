using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using TheBindingOfRarria.Common.Config;
using TheBindingOfRarria.Common.Helpers;
using TheBindingOfRarria.Content.Buffs;

namespace TheBindingOfRarria.Content.Items;

public class YorghsRing : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;
    public override void SetDefaults()
    {
        Item.accessory = true;
        Item.width = 30;
        Item.height = 28;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.sellPrice(0, 0, 1, 12);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        var p = player.GetModPlayer<ParryAccPlayer>();

        p.CanParry = true;

        player.shield_parry_cooldown = (int)MathHelper.Max(0, player.shield_parry_cooldown - 1);
        if (p.parrying)
        {
            //player.TryTogglingShield(true);
            //player.shieldRaised = true;
            player.shieldParryTimeLeft++;
            if (player.shieldParryTimeLeft >= 20) 
            {
                player.shieldParryTimeLeft = 0;
                p.parrying = false; 
            }
        }
        if (p.cooldown == 1)
        {
            SoundEngine.PlaySound(25, player.Center);
            for (int i = 0; i < 10; i++)
            {
                int num = Dust.NewDust(player.Center + new Vector2(player.direction * 6 + ((player.direction == -1) ? (-10) : 0), -14f), 10, 16, DustID.ManaRegeneration, 0f, 0f, 255, new Color(255, 100, 0, 127), (float)Main.rand.Next(10, 16) * 0.1f);
                Main.dust[num].noLight = true;
                Main.dust[num].noGravity = true;
                Main.dust[num].velocity *= 0.5f;
            }
        }
    }
    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        var key = KeybindSystem.AbsorbingKey.GetAssignedKeys().FirstOrDefault();
        if (key == "" || key == null)
            key = "N";

        var text = string.Format(Language.GetTextValue($"Mods.TheBindingOfRarria.Items.{Name}.Tooltip"), key);


        var index = tooltips.FindIndex(line => line.Name == "Tooltip0");
        if (index != -1)
        {
            text = text.Remove(text.LastIndexOf($"\n"));
            text = text.Remove(text.LastIndexOf($"\n"));
            tooltips[index].Text = text;
        }
    }
}

public class ParryAccPlayer : ModPlayer
{
    public bool CanParry = false;

    public bool parrying = false;

    public int cooldown = 0;

    public override void PostUpdate()
    {
        cooldown = cooldown > 0 ? cooldown - 1 : cooldown;
    }

    public override void ResetEffects()
    {
        CanParry = false;
    }

    public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
    {
        if (CanParry && Player.shieldParryTimeLeft > 0)
        {
            if (MathF.Sign(proj.Center.X - proj.velocity.X - Player.Center.X) != Player.direction)
                return;

            modifiers.Cancel();
            proj.GetReflected();
        }
    }

    public override void ProcessTriggers(TriggersSet triggersSet)
    {
        if (CanParry && cooldown == 0 && (KeybindSystem.ParryKey.Current || (KeybindSystem.ParryKey.GetAssignedKeys().FirstOrDefault() == null && Main.keyState.IsKeyDown(Keys.N))))
        {
            parrying = true;
            cooldown = 300;
        }
    }

    public override void Load()
    {
        On_PlayerDrawLayers.DrawPlayer_25_Shield += On_PlayerDrawLayers_DrawPlayer_25_Shield;
    }

    private void On_PlayerDrawLayers_DrawPlayer_25_Shield(On_PlayerDrawLayers.orig_DrawPlayer_25_Shield orig, ref PlayerDrawSet drawinfo)
    {
        if (!drawinfo.drawPlayer.GetModPlayer<ParryAccPlayer>().parrying)
            orig(ref drawinfo);
        else
        {
            var old = drawinfo.drawPlayer.shieldRaised;
            drawinfo.drawPlayer.shieldRaised = true;
            orig(ref drawinfo);
            drawinfo.drawPlayer.shieldRaised = old;
        }
    }
}