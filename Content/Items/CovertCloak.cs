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
        Item.height = 30;
        Item.width = 30;
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
    }

    public override void UpdateVanity(Player player)
    {
        player.GetModPlayer<CovertCloakPlayer>().Equipped = true;
    }
}

public class CovertCloakPlayer : ModPlayer
{
    public bool Equipped = false;

    public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
    {
        if (drawInfo.drawPlayer.GetModPlayer<CovertCloakPlayer>().Equipped)
        {
            drawInfo.helmetOffset += new Vector2(0, -2);
            drawInfo.drawPlayer.head = EquipLoader.GetEquipSlot(Mod, "CovertCloak", EquipType.Head);
            drawInfo.colorHead = Color.Transparent;
            drawInfo.drawsBackHairWithoutHeadgear = false;
            drawInfo.drawPlayer.face = -1;
        }
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