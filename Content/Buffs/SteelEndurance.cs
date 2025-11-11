using ReLogic.Graphics;
using Terraria.UI.Chat;
using TheBindingOfRarria.Content.Items;

namespace TheBindingOfRarria.Content.Buffs;
/*
public class SteelEndurance : ModBuff
{
    public override string Texture => ContentPath + "Buffs/" + Name;

    public override void Update(Player player, ref int buffIndex) => player.endurance += player.GetModPlayer<SteelPlayer>().Stacks * 0.03f;

    public override void PostDraw(SpriteBatch spriteBatch, int buffIndex, BuffDrawParams drawParams)
    {
        DynamicSpriteFont font = FontAssets.MouseText.Value;
        string text = $"{Main.LocalPlayer.GetModPlayer<SteelPlayer>().Stacks}";

        Vector2 position = drawParams.Position + drawParams.Texture.Size();

        Vector2 origin = drawParams.Texture.Size() * 0.5f;

        var color = Color.White;
        color.A = 150;

        // You can use ChatManager::DrawColorCodedStringWithShadow.
        ChatManager.DrawColorCodedStringWithShadow(spriteBatch, font, text, position, color, 0f, origin, new Vector2(0.7f));
        //spriteBatch.DrawString(font, text, position, color, 0, origin + new Vector2(6f * (Main.LocalPlayer.GetModPlayer<GeneThiefPlayer>().genePool / 50), 4.5f), 0.8f, SpriteEffects.None, 0);
        //spriteBatch.DrawString(font, text, position, Color.White, 0, origin + new Vector2(6.5f * (Main.LocalPlayer.GetModPlayer<GeneThiefPlayer>().genePool / 50), 5), 0.7f, SpriteEffects.None, 0);
    }
}*/