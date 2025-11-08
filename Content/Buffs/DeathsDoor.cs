using TheBindingOfRarria.Content.Items;

namespace TheBindingOfRarria.Content.Buffs;

public class DeathsDoor : ModBuff
{
    public override string Texture => ContentPath + "Buffs/" + Name;

    public override void Update(Player player, ref int buffIndex)
    {
        for (int i = 0; i < player.buffImmune.Length; i++)
            player.buffImmune[i] = true;

        player.buffImmune[Type] = false;

        player.statLife = player.GetModPlayer<DeathsDoorPlayer>().life;
    }
}