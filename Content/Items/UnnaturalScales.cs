
namespace TheBindingOfRarria.Content.Items;

public class UnnaturalScales : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;

    public override void Load()
    {
        // All code below runs only if we're not loading on a server
        if (Main.netMode != NetmodeID.Server)
        {
            // Add equip textures
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Head}", EquipType.Head, this);
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Body}", EquipType.Body, this);
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Legs}", EquipType.Legs, this);
        }
    }

    public override void SetDefaults()
    {
        Item.width = 20;
        Item.height = 20;
        Item.vanity = true;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 1);
        Item.rare = ItemRarityID.Expert;
        Item.expert = true;

        if (Main.netMode != NetmodeID.Server)
        {
            int equipSlotHead = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
            int equipSlotBody = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);
            int equipSlotLegs = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);
            ArmorIDs.Head.Sets.DrawHead[equipSlotHead] = false;
            ArmorIDs.Body.Sets.HidesTopSkin[equipSlotBody] = true;
            ArmorIDs.Body.Sets.HidesArms[equipSlotBody] = true;
            ArmorIDs.Legs.Sets.HidesBottomSkin[equipSlotLegs] = true;
        }
    }

    public override void UpdateEquip(Player player)
    {
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<RekSaiArmorPlayer>().Equipped = true;
        for (int i = 0; i < 3; i++)
        {
            Item.NewItem(player.armor[i].GetSource_DropAsItem(), player.Center, player.armor[i]);
            player.armor[i].TurnToAir();
        }
    }

    public override void UpdateVanity(Player player)
    {
        player.GetModPlayer<RekSaiArmorPlayer>().Equipped = true;
    }
}

public class RekSaiArmorPlayer : ModPlayer
{
    public bool Equipped = false;

    public override void PreUpdate()
    {
        Equipped = false;
    }

    public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
    {
        if (drawInfo.drawPlayer.GetModPlayer<RekSaiArmorPlayer>().Equipped)
        {
            drawInfo.drawPlayer.legs = EquipLoader.GetEquipSlot(Mod, "UnnaturalScales", EquipType.Legs);
            drawInfo.drawPlayer.body = EquipLoader.GetEquipSlot(Mod, "UnnaturalScales", EquipType.Body);
            drawInfo.drawPlayer.head = EquipLoader.GetEquipSlot(Mod, "UnnaturalScales", EquipType.Head);
            drawInfo.colorLegs = Color.Transparent;
            drawInfo.colorPants = Color.Transparent;
            drawInfo.colorBodySkin = Color.Transparent;
            drawInfo.hidesBottomSkin = true;
            drawInfo.hidesTopSkin = true;
            drawInfo.drawPlayer.face = -1;
            drawInfo.legsOffset += new Vector2(-6, 0) * drawInfo.drawPlayer.direction;
        } 
    }

    public override void FrameEffects()
    {
        if (Equipped)
        {
            Player.legs = EquipLoader.GetEquipSlot(Mod, "UnnaturalScales", EquipType.Legs);
            Player.body = EquipLoader.GetEquipSlot(Mod, "UnnaturalScales", EquipType.Body);
            Player.head = EquipLoader.GetEquipSlot(Mod, "UnnaturalScales", EquipType.Head);
            Player.face = -1;
        }
    }
}