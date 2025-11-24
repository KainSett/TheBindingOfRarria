using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace TheBindingOfRarria.Content.Items;

public class TheLastFinger : ModItem
{
    public override string Texture => ContentPath + "Items/" + Name;

    public override void SetDefaults()
    {
        Item.height = 30;
        Item.width = 26;
        Item.accessory = true;
        Item.rare = ItemRarityID.Expert;
        Item.value = Item.sellPrice(0, 1, 12);
        Item.expert = true;
    }

    private int counter = 0;

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.statManaMax2 += 100;
        player.aggro -= 500;

        counter--;

        if (counter <= 0 && Main.myPlayer == player.whoAmI)
        {
            float distance = 400f * 400;
            Vector2 position = Vector2.Zero;

            foreach (var target in Main.ActiveNPCs)
            {
                var dist = target.Center.DistanceSQ(player.Center);
                if (distance > dist && !target.friendly && target.lifeMax > 5 && target.CanBeChasedBy() && !target.immortal && !target.CountsAsACritter)
                {
                    distance = dist;
                    position = target.Center;
                }
            }

            if (position != Vector2.Zero)
            {
                counter = 10;
                counter += (int)player.Center.Distance(position) / 32 - 6;
                Vector2 velocity = new Vector2(11f, 11f).RotatedByRandom(TwoPi);
                Projectile.NewProjectile(player.GetSource_Accessory(Item, "Thukuna accessory"), position - velocity * 9, velocity, ProjectileID.Muramasa, 30, 1, player.whoAmI, 0, 0, 0);
            }
        }
    }
}

    // Before my tweaks you were INSTANCING A GLOBAL PROJECTILE, PER FUCKING ITEM.
public class MuraProj : GlobalProjectile
{
    public override bool InstancePerEntity => true;

    public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => entity.type == ProjectileID.Muramasa;

    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        if (source != null && source.Context != null && (source.Context == "Thukuna accessory" || source.Context == "SlasherHalberd"))
        {
            projectile.penetrate = -1;
            projectile.timeLeft = 30;
            projectile.scale *= 1.3f;
            SpawnUpdate = true;
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                SpawnUpdate = false;
                Slash = true;
            }
        }
    }

    private bool SpawnUpdate = false;
    private bool Slash = false;

    public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter)
    {
        bitWriter.WriteBit(SpawnUpdate);

        if (SpawnUpdate)
        {
            binaryWriter.Write(projectile.penetrate);
            binaryWriter.Write(projectile.timeLeft);
            binaryWriter.Write(projectile.scale);
        }

        SpawnUpdate = false;
    }

    public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader)
    {
        SpawnUpdate = bitReader.ReadBit();

        if (SpawnUpdate)
        {
            projectile.penetrate = binaryReader.ReadInt32();
            projectile.timeLeft = binaryReader.ReadInt32();
            projectile.scale = binaryReader.ReadSingle();

            SpawnUpdate = false;
            Slash = true;
        }
    }

    public override bool PreDraw(Projectile projectile, ref Color lightColor)
    {
        if (Slash)
        {
            projectile.scale = 2.6f;
            lightColor.B = 120;
            lightColor.G = 120;
            lightColor.R = 240;

            Texture2D texture = TextureAssets.Projectile[projectile.type].Value;

            Color color = lightColor * (250 * (1f / 255f));

            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, texture.Bounds, color with { A = 0 }, projectile.rotation, texture.Size() * 0.5f, projectile.scale, SpriteEffects.None, 0);

            return false;
        }

        return base.PreDraw(projectile, ref lightColor);
    }
}

public class CrateLootThukunaFinger : GlobalItem
{
    public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
    {
        if (item.type == ItemID.CrimsonFishingCrateHard)
        {
            IItemDropRule rule = ItemDropRule.ByCondition(new Conditions.IsExpert(), ModContent.ItemType<TheLastFinger>(), 6);
            itemLoot.Add(rule);
        }
    }
}

public class ThukunaLootNPC : GlobalNPC
{
    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        if (npc.type == NPCID.BigMimicCorruption)
            npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsExpert(), ModContent.ItemType<TheLastFinger>(), 6));
        base.ModifyNPCLoot(npc, npcLoot);
    }
}