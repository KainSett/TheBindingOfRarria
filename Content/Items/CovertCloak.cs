
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.GameContent.Achievements;
using Terraria.GameInput;
using Terraria.Graphics.Renderers;
using Terraria.ModLoader.Default;
using TheBindingOfRarria.Common.Registries;
using TheBindingOfRarria.Common.Systems;
using TheBindingOfRarria.Content.NPCs;
using static Terraria.Player;

namespace TheBindingOfRarria.Content.Items;

[AutoloadEquip(EquipType.Back, EquipType.Front)]
public class CovertCloak : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;

    public override void Load()
    {
        EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Head}", EquipType.Head, this);
    }

    public override void SetDefaults()
    {
        Item.height = 64;
        Item.width = 32;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 3);
        Item.rare = ItemRarityID.Green;

        if (Main.netMode != NetmodeID.Server)
        {
            int equipSlotHead = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
            ArmorIDs.Head.Sets.DrawHead[equipSlotHead] = false;
        }
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<CovertCloakPlayer>().Equipped = true;
        player.aggro -= 500;
    }

    public override void UpdateVanity(Player player)
    {
        player.GetModPlayer<CovertCloakPlayer>().Equipped = true;
    }

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        spriteBatch.Draw(TextureAssets.Item[Type].Value, position, frame, drawColor, 0, origin, scale * 1.4f, SpriteEffects.None, 0);

        return false;
    }

    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
    {
        Main.GetItemDrawFrame(Type, out var texture, out var frame);
        var origin = frame.Size() / 2;
        var position = Item.Bottom - Main.screenPosition - new Vector2(0, origin.Y * 0.8f);

        spriteBatch.Draw(texture, position, frame, lightColor, rotation, origin, scale, SpriteEffects.None, 0);

        return false;
    }
}

public class CovertCloakPlayer : ModPlayer
{
    public bool Equipped = false;

    public override void ResetEffects()
    {
        Equipped = false;
    }

    public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
    {
        if (drawInfo.drawPlayer.GetModPlayer<CovertCloakPlayer>().Equipped || drawInfo.drawPlayer.front == EquipLoader.GetEquipSlot(Mod, "CovertCloak", EquipType.Front))
        {
            drawInfo.drawPlayer.head = EquipLoader.GetEquipSlot(Mod, "CovertCloak", EquipType.Head);
            drawInfo.colorHead = Color.Transparent;
            drawInfo.drawsBackHairWithoutHeadgear = false;
            drawInfo.drawPlayer.face = -1;
        }
    }

    public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
    {
        if (Equipped && Effects.Glitch is not null && Effects.Glitch.Value is not null)
        {
            var _glitchIntensity = 0.2f * Main.GameZoomTarget;
            PlayerRenderTarget.DrawEffect = Effects.Glitch.Value;
            PlayerRenderTarget.DrawAction = () =>
            {
                PlayerRenderTarget.DrawEffect.Parameters["intensity"].SetValue(_glitchIntensity);
                PlayerRenderTarget.DrawEffect.Parameters["textureSize"].SetValue(PlayerRenderTarget.Target.Size() / Main.GameZoomTarget);
                PlayerRenderTarget.DrawEffect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly);

                Main.graphics.GraphicsDevice.Textures[1] = Textures.Noise[0].Value;
            };
        }
        else
            return;
    }

    public override void FrameEffects()
    {
        if (Equipped)
        {
            Player.head = EquipLoader.GetEquipSlot(Mod, "CovertCloak", EquipType.Head);
            Player.face = -1;
        }
    }

}