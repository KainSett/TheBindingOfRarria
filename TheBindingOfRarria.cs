using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using TheBindingOfRarria.Common.Systems;
using TheBindingOfRarria.Content.Items;
using TheBindingOfRarria.Content.Projectiles;
using static Terraria.WorldGen;
using static TheBindingOfRarria.Common.Helpers.Helper;

namespace TheBindingOfRarria;

public class TheBindingOfRarria : Mod
{
    public static List<string> ItemPages = new ();
    public static List<string> RecipePages = new();

    public static string recipeTemplate = "-->{{recipes/register\r\n|result=#name|amount=1\r\n|station=none}}<!--";
    public static string material = "\r\n|material|amount";
    public static string dropBox = "{{mod sub-page}}<!--DO NOT REMOVE THIS LINE! It is required for Mod sub-pages to work properly.-->\r\n{{infobox wrapper\r\n|{{item infobox\r\n{{drop infobox\r\n|}}";
    public static string drop = "\r\n| source | 1 | {{difficulty|chance}}";
    public static string itemPage = "{{mod sub-page}}<!--DO NOT REMOVE THIS LINE! It is required for Mod sub-pages to work properly.-->\r\n{{item infobox\r\n| type = Accessory\r\n| sell = {{value|p|g|s|c}}\r\n| stack = 1\r\n| rare = 0\r\n| tooltip = firstLine<br>\"flavor\"\r\n}}\r\n\r\n'''name''' is a [[Hardmode]] {{+|Accessories|accessory}} \r\n\r\n\r\n== Crafting ==\r\n=== Recipe ===\r\n{{recipes|result=#name}}\r\n\r\n\r\n== Notes ==\r\n{{*}} This item\r\n\r\n\r\n== Trivia ==\r\n* This item";

    public static readonly BlendState Subtractive = new()
    {
        ColorBlendFunction = BlendFunction.ReverseSubtract,
        ColorDestinationBlend = Blend.One,
        ColorSourceBlend = Blend.SourceAlpha,
        AlphaBlendFunction = BlendFunction.ReverseSubtract,
        AlphaDestinationBlend = Blend.One,
        AlphaSourceBlend = Blend.SourceAlpha
    };

    public enum PacketTypes : int
    {
        ProjectileReflect,
        EntitySlow,
        DustSpawn,
        PlayerScale,
        Default
    }

    public enum State
    {
        Default,
        Slow,
        Fast
    }

    public static void GetWikiItemAndRecipePages(IEnumerable<ModItem> items)
    {
        foreach (var item in items)
        {
            var name = item.DisplayName.Value;
            var pulledRecipe = Array.Find(Main.recipe, r => r.createItem.type == item.Type);
            if (pulledRecipe != null)
            {
                //recipes here

                var stations = "";
                foreach (var st in pulledRecipe.requiredTile)
                {
                    var n = TileID.Search.GetName(st);
                    for (int i = 0; i < n.Length - 2; i++)
                    {
                        if (!char.IsWhiteSpace(n[i]) && !char.IsWhiteSpace(n[i + 1]) && char.IsUpper(n[i + 1]) && char.IsLower(n[i]))
                        {
                            n = n[..(i + 1)] + " " + n[(i + 1)..];
                            i++;
                        }
                    }
                    stations += "\r\n|station=" + n;
                }

                var recipe = recipeTemplate.Replace("name", name).Replace("\r\n|station=none", stations);

                var mat = "";
                foreach (var m in pulledRecipe.requiredItem)
                {
                    mat += material.Replace("material", m.Name).Replace("amount", m.stack.ToString());
                }
                recipe = recipe[..recipe.IndexOf('}')] + mat + recipe[recipe.IndexOf('}')..];

                recipe = recipe.Replace("\"", "");
                RecipePages.Add(recipe);
            }
            /*else 
            {
                // drops here

                var start = dropBox[..dropBox.IndexOf('|')];
                var box = dropBox[dropBox.LastIndexOf("{{")..];

                for (int t = 0; t < NPCID.Count - 1; t++)
                {
                    var drops = Main.ItemDropsDB.GetRulesForNPCID(t);

                    if (drops != null)
                    {
                        foreach (var d in drops)
                        {
                            var r = d.ChainedRules.FirstOrDefault();
                            var mast = d.ChainedRules.Find(cond => cond.RuleToChain == new Conditions.IsMasterMode());
                            var exp = d.ChainedRules.Find(cond => cond.RuleToChain == new Conditions.IsExpert());

                            r = mast ?? exp ?? r;
                            if (r == null)
                                continue;

                            var dif = mast != null ? "master|" : exp != null ? "expert|" : "";

                            var n = NPCID.Search.GetName(t);
                            for (int i = 0; i < n.Length - 2; i++)
                            {
                                if (!char.IsWhiteSpace(n[i]) && !char.IsWhiteSpace(n[i + 1]) && char.IsUpper(n[i + 1]) && char.IsLower(n[i]))
                                {
                                    n = n[..(i + 1)] + " " + n[(i + 1)..];
                                    i++;
                                }
                            }

                            var text = drop.Replace("source", n).Replace("difficulty|", dif);
                            box = box.Replace("\r\n|", text);
                        }
                    }
                }

            itemPage = itemPage.Replace(itemPage[..itemPage.IndexOf("\r\n{{")], start).Replace("'''", box + "'''");
            }*/

            var value = item.Item.value;
            var c = value % 100;
            value = (value - c) / 100;
            var s = value % 100;
            value = (value - s) / 100;
            var g = value % 100;
            value = (value - g) / 100;
            var p = value % 100;
            value = (value - p) / 100;

            var tooltip = item.Tooltip.Value;
            if (tooltip == "" || !tooltip.Contains('\n'))
                continue;

            var page = itemPage.Replace("name", name).Replace("p|g|s|c", $"{p}|{g}|{s}|{c}").Replace("rare = 0", $"rare = {item.Item.rare.ToString()}").Replace("firstLine", tooltip[..tooltip.LastIndexOf("\n")].Replace("\n", "<br>\n")).Replace("flavor", tooltip[tooltip.LastIndexOf("\n")..]);

            page = page.Replace("\"", "");

            ItemPages.Add(page);
        }
    }

    public override void PostAddRecipes()
    {
        //GetWikiItemAndRecipePages(GetContent<ModItem>());
    }

    public override void HandlePacket(BinaryReader reader, int whoAmI)
    {
        var type = reader.ReadInt32();
        if (type == (int)PacketTypes.ProjectileReflect)
        {
            int id = reader.ReadInt32();

            foreach (var proj in Main.ActiveProjectiles)
            {
                if (proj.identity == id)
                    proj.GetReflected();
            }
            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = GetPacket();
                packet.Write(type);
                packet.Write(id);
                packet.Send();
            }
        }
        else if (type == (int)PacketTypes.EntitySlow)
        {
            int slow = reader.ReadInt32();
            int duration = reader.ReadInt32();
            bool entityType = reader.ReadBoolean();
            int id = reader.ReadInt32();

            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = GetPacket();
                packet.Write(type);
                packet.Write(slow);
                packet.Write(duration);
                packet.Write(entityType);
                packet.Write(id);
                packet.Send();
            }

            // guh
            if (entityType)
            {
                foreach (Projectile p in Main.ActiveProjectiles)
                    if (p.identity == id)
                        p.GetGlobalProjectile<SlowedGlobalProjectile>().Slowed = ((State)slow, duration);
            }
            else
                foreach (NPC n in Main.ActiveNPCs)
                {
                    if (n.whoAmI != id)
                        continue;

                    n.GetGlobalNPC<SlowedGlobalNPC>().Slowed = ((State)slow, duration);

                    if (n.realLife != -1)
                    {
                        Main.npc[n.realLife].GetGlobalNPC<SlowedGlobalNPC>().Slowed = ((State)slow, duration);

                        var npcs = Array.FindAll(Main.npc, npc => npc.realLife == n.realLife && npc.active);
                        if (npcs is null || npcs.Length < 1)
                            break;

                        foreach (NPC npc in npcs)
                        {
                            npc.GetGlobalNPC<SlowedGlobalNPC>().Slowed = ((State)slow, duration);
                        }
                    }

                    break;
                }
            return;
        }
        else if (type == (int)PacketTypes.DustSpawn)
        {
            Vector2 position = reader.ReadVector2();
            Vector2 direction = reader.ReadVector2();
            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = GetPacket();
                packet.Write(type);
                packet.WriteVector2(position);
                packet.WriteVector2(direction);
                packet.Send();
            }
            else
            {
                NatureDodgePlayer natureplayer = Main.LocalPlayer.GetModPlayer<NatureDodgePlayer>();

                natureplayer.blocked = true;
                natureplayer.position = position;
                natureplayer.direction = direction;
            }
            return;
        }
        else if (type == (int)PacketTypes.PlayerScale)
        {
            var who = (int)reader.ReadByte();
            var scale = reader.ReadSingle();

            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = GetPacket();
                packet.Write(type);
                packet.Write((byte)who);
                packet.Write(scale);
                packet.Send();
            }

            var player = Main.player[who];
            ResizedPlayerUtils.SetScale(player, scale, false);
        }
    }
}
