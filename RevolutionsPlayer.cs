﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Revolutions.Items.Accessory;
using Revolutions.Items.Armor;
using Revolutions.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;

namespace Revolutions
{
    public class RevolutionsPlayer : ModPlayer
    {
        public Vector2[] pastPosition { get; set; } = new Vector2[601];
        public Vector2[] pastCenter { get; set; } = new Vector2[601];
        public Vector2[] pastSpeed { get; set; } = new Vector2[601];
        public int[] pastLife { get; set; } = new int[601];
        public int[] pastMana { get; set; } = new int[601];
        public int[] starFlare { get; set; } = new int[601];
        public int[] corePower { get; set; } = new int[601];
        public float sanity { get; set; } = 100;
        public int difficulty { get; set; } = 0;
        public int maxStarFlare { get; set; } = 500;
        public int aPTimer { get; set; } = 0;
        public int dPTimer { get; set; } = 0;
        public int wPTimer { get; set; } = 0;
        public int sPTimer { get; set; } = 0;
        public int channeltime = 0;
        public float[] extraSpeed = new float[2];
        public int extraSpdTimer = 0;
        public Vector2 instantmove = Vector2.Zero;
        public Color starFlareColor { get; set; } = Color.White;
        public int starFlareColorType { get; set; } = 0;
        public bool starFlareStatus { get; set; } = false;
        public int saviourStatus { get; set; } = 1;
        public bool evolutionary { get; set; } = false;
        public bool saviourexist { get; set; } = false;
        public static int timer = 0;
        public int justDmgcounter { get; set; } = 0;
        public int sLifeStealcounter { get; set; } = 0;
        public int talkActive { get; set; } = 0;
        public string nowSaying { get; set; } = "";
        public static NPC nowBoss = null;
        public static int nowBossLife = 0;
        public static int nowBossLifeTrue = 0;
        public static int nowBossLifeMax = 0;
        public static bool lastHthBarStatus = false;
        public static int HthBarTimer = 0;
        public string spname { get; set; } = "none";
        public bool lightning { get; set; } = false;
        public bool lightningproj { get; set; } = false;
        public int hitcounter { get; set; } = 0;
        public int bossFightTimer { get; set; } = 0;
        public int hitBonuscounter { get; set; } = 0;
        public static Color customStarFlareColor = Color.White;
        public static List<StringTimerInt> npctalk = new List<StringTimerInt>();
        public static int logoTimer = 0;
        public static float drawcircler = 0;
        public static int drawcircletype = 0;
        public static float screenR = 0;
        public override void OnEnterWorld(Player player)
        {
            screenR = new Vector2(Main.screenWidth, Main.screenHeight).Length() * Main.UIScale / 2;
            logoTimer += 90;
            for (int i = 0; i < 601; i++)
            {
                pastPosition[i] = player.position;
                pastLife[i] = player.statLife;
                pastMana[i] = player.statMana;
                pastCenter[i] = player.Center;
                pastSpeed[i] = player.velocity;
                if (i != 0) starFlare[i] = 0;
                corePower[i] = 0;
            }
            lightning = false;
            Helper.GetSetting();
        }
        public override TagCompound Save()
        {
            logoTimer = 0;
            drawcircler = 0;
            TagCompound tag = new TagCompound();
            tag.Add("sf", starFlare[0]);
            tag.Add("sfmax", maxStarFlare);
            tag.Add("sfcolor", customStarFlareColor);
            tag.Add("sfcolortype", starFlareColorType);
            return tag;
        }
        public override void Load(TagCompound tag)
        {
            if (tag.ContainsKey("sf") && tag.ContainsKey("sfmax") && tag.ContainsKey("sfcolor") && tag.ContainsKey("sfcolortype"))
            {
                starFlare[0] = tag.GetAsInt("sf");
                maxStarFlare = tag.GetAsInt("sfmax");
                customStarFlareColor = tag.Get<Color>("sfcolor");
                starFlareColorType = tag.GetAsInt("sfcolortype");
                //Helper.Array2Setting(tag.Get<bool[]>("setting"));
            }
            base.Load(tag);
        }
        List<Point> justOpenDoors = new List<Point>();
        public override void PreUpdate()
        {
            if (!starFlareStatus) starFlareColor = new Color(126, 171, 243);
            else starFlareColor = Helper.SFCtypeToColor(starFlareColorType);
            if (Revolutions.Settings.spcolor) spname = Helper.Name2Specialname(player.name);
            else spname = "none";
            //过去的属性
            for (int j = 600; j > 0; j--)
            {
                pastPosition[j] = pastPosition[j - 1];
                pastCenter[j] = pastCenter[j - 1];
                pastSpeed[j] = pastSpeed[j - 1];
                pastLife[j] = pastLife[j - 1];
                pastMana[j] = pastMana[j - 1];
                starFlare[j] = starFlare[j - 1];
                corePower[j] = corePower[j - 1];
            }
            for (int i = 0; i < npctalk.Count; i++)
            {
                if (npctalk[i].timer > 0) npctalk[i].timer--;
                else npctalk.RemoveAt(i);
            }
            pastPosition[0] = player.position;
            pastCenter[0] = player.Center;
            pastSpeed[0] = player.velocity;
            pastLife[0] = player.statLife;
            pastMana[0] = player.statMana;
            if (talkActive > 0) talkActive--;
            //周期性事件
            if (timer == 60)
            {
                timer = 0;
                justDmgcounter = 0;
                sLifeStealcounter = 0;
            }
            else timer++;
            //周期性事件
            if (timer % 20 == 0 && timer != 0)
            {
                if (starFlare[0] + 1 > maxStarFlare) starFlare[0] = maxStarFlare;
                else starFlare[0] += 1;
            }
            if (timer == 30 || timer == 60)
            {
                if (hitcounter == 0 && nowBoss != null && difficulty < 100) difficulty++;
            }
            if (nowBoss == null) difficulty = 50;
            //自动开关门
            if (Revolutions.Settings.autodoor)
            {
                for (int i = 0; i < justOpenDoors.Count; i++)
                {
                    var door = justOpenDoors.ToArray()[i];
                    if (Vector2.Distance(door.ToVector2(), Helper.ToTilesPos(player.Center).ToVector2()) >= 3)
                    {
                        WorldGen.CloseDoor(door.X, door.Y);
                        justOpenDoors.RemoveAt(i);
                    }
                }
                if (Main.tile[Helper.ToTilesPos(player.Center).X + 1, Helper.ToTilesPos(player.Center).Y].type == TileID.ClosedDoor)
                {
                    if (!WorldGen.OpenDoor(Helper.ToTilesPos(player.Center).X + 1, Helper.ToTilesPos(player.Center).Y, player.direction))
                        WorldGen.OpenDoor(Helper.ToTilesPos(player.Center).X + 1, Helper.ToTilesPos(player.Center).Y, -player.direction);
                    justOpenDoors.Add(new Point(Helper.ToTilesPos(player.Center).X + 1, Helper.ToTilesPos(player.Center).Y));
                }
                if (Main.tile[Helper.ToTilesPos(player.Center).X - 1, Helper.ToTilesPos(player.Center).Y].type == TileID.ClosedDoor)
                {
                    if (!WorldGen.OpenDoor(Helper.ToTilesPos(player.Center).X - 1, Helper.ToTilesPos(player.Center).Y, player.direction))
                        WorldGen.OpenDoor(Helper.ToTilesPos(player.Center).X - 1, Helper.ToTilesPos(player.Center).Y, -player.direction);
                    justOpenDoors.Add(new Point(Helper.ToTilesPos(player.Center).X - 1, Helper.ToTilesPos(player.Center).Y));
                }
            }
        }
        public override void ResetEffects()
        {
            saviourexist = false;
            evolutionary = false;
            hitcounter = 0;
            hitBonuscounter = 0;
            drawcircler = 0;
            drawcircletype = 0;
        }
        int timerw = 0;
        //W按下的计时器
        int timers = 0;
        int counterw = 0;
        //计时器的计数器，为了实现功能
        int counters = 0;
        int cd;
        public override void SetControls()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.A))
                aPTimer++;
            else
                aPTimer = 0;
            if (Keyboard.GetState().IsKeyDown(Keys.D))
                dPTimer++;
            else
                dPTimer = 0;
            if (Keyboard.GetState().IsKeyDown(Keys.W))
                wPTimer++;
            else
                wPTimer = 0;
            if (Keyboard.GetState().IsKeyDown(Keys.S))
                sPTimer++;
            else
                sPTimer = 0;
            if (cd > 0) cd--;
            if (wPTimer == 1 || wPTimer == 2) timerw++;
            else timerw = 0;
            if (timerw > 1) counterw += 12;
            if (sPTimer == 1 || sPTimer == 2) timers++;
            else timers = 0;
            if (timers > 1) counters += 12;
            if (counterw > 12 && cd == 0 && saviourexist)
            {
                counterw = 0;
                cd = 10;
                saviourStatus *= -1;
                new Talk(0, Language.GetTextValue("Mods.Revolutions.saviourstatus" + saviourStatus.ToString()), 180, player);
            }
            if (counters > 12 && cd == 0 && saviourexist)
            {
                counters = 0;
                cd = 10;
                saviourStatus *= -1;
                new Talk(0, Language.GetTextValue("Mods.Revolutions.saviourstatus" + saviourStatus.ToString()), 180, player);
            }
            if (counterw > 0) counterw--;
            if (counters > 0) counters--;
        }
        public override void DrawEffects(PlayerDrawInfo drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            base.DrawEffects(drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
        }
        public static int timer2 = 0;
        public static int timer3 = 0;
        public override void PostUpdate()
        {
            nowBoss = null;
            nowBossLifeMax = 0;
            if (timer2 == 10 || timer2 == 0) nowBossLife = 0;
            nowBossLifeTrue = 0;
            foreach (NPC npc in Main.npc)
            {
                if (npc.boss && npc.active)
                {
                    if (nowBoss == null) nowBoss = npc;
                    else if (npc.lifeMax > nowBoss.lifeMax && npc.type != 134 && npc.type != 135 && npc.type != 136) nowBoss = npc;
                }
                if (npc.active && npc.type == 125)
                {
                    nowBossLifeMax += npc.lifeMax;
                    nowBossLifeTrue += npc.life;
                    if (timer2 == 10 || timer2 == 0)
                    {
                        nowBossLife += npc.life;
                    }
                }
                if (nowBoss == null && npc.type == 13) nowBoss = npc;
                //增加boss组件生命值
                if (nowBoss != null && nowBoss.active && npc.active)
                {
                    //石巨人
                    if (nowBoss.type == 245 && (npc.type == 246 || npc.type == 247 || npc.type == 248)) { nowBossLifeMax += npc.lifeMax; nowBossLifeTrue += npc.life; if (timer2 == 10 || timer2 == 0) { nowBossLife += npc.life; } }
                    //骷髅王
                    if (nowBoss.type == 35 && npc.type == 36) { nowBossLifeMax += npc.lifeMax; nowBossLifeTrue += npc.life; if (timer2 == 10 || timer2 == 0) { nowBossLife += npc.life; } }
                    //机械骷髅王
                    if (nowBoss.type == 127 && (npc.type == 128 || npc.type == 129 || npc.type == 130 || npc.type == 131)) { nowBossLifeMax += npc.lifeMax; nowBossLifeTrue += npc.life; if (timer2 == 10 || timer2 == 0) { nowBossLife += npc.life; } }
                    //克苏鲁之脑
                    if (nowBoss.type == 266 && npc.type == 267) { nowBossLifeMax += npc.lifeMax; nowBossLifeTrue += npc.life; if (timer2 == 10 || timer2 == 0) { nowBossLife += npc.life; } }
                    //世界吞噬者
                    if (nowBoss.type == 13 && (npc.type == 13 || npc.type == 14 || npc.type == 15)) { nowBossLifeMax += npc.lifeMax; nowBossLifeTrue += npc.life; if (timer2 == 10 || timer2 == 0) { nowBossLife += npc.life; } }
                    //Moon Lord
                    if (nowBoss.type == 398 && (npc.type == 396 || npc.type == 397)) { nowBossLifeMax += npc.lifeMax; if (npc.ai[0] != -2) { nowBossLifeTrue += npc.life; if (timer2 == 10 || timer2 == 0) { nowBossLife += npc.life; } } }
                }
            }
            if (timer2 == 10) { timer2 = 0; timer3 = 1; }
            if (nowBoss != null)
            {
                timer2++;
                if (nowBoss.type != 125 && nowBoss.type != 13)
                {
                    nowBossLifeTrue += nowBoss.life;
                    nowBossLifeMax += nowBoss.lifeMax;
                }
            }
            else { timer2 = 0; nowBossLife = 0; timer3 = 0; }
            if (timer2 == 1 && timer3 == 1 && nowBoss.type != 125) nowBossLife += nowBoss.life;
            Random rd = new Random();
            if (nowBoss != null)
            {
                if (nowBoss.type == 398 && nowBoss.ai[0] == 2) { nowBossLife = 0; nowBossLifeTrue = 0; }
                if (ModLoader.GetMod("Eternalresolve") != null && ModLoader.GetMod("Eternalresolve").NPCType("Omidy") == nowBoss.type && nowBoss.life == 100000) { nowBossLife = 0; nowBossLifeTrue = 0; }
                if (bossFightTimer < 6000 && timer % 5 == 0 && timer != 0) bossFightTimer++;
            }
            else
            {
                bossFightTimer = 0;
            }
        }
        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            if (nowBoss != null && hitBonuscounter < 3 && bossFightTimer < 6000)
            {
                bossFightTimer++;
                hitBonuscounter++;
            }
            Random rd = new Random();
            int a = rd.Next(0, 60 / player.HeldItem.useTime * 20);
            if (a == 1) new Talk(0, Language.GetTextValue("Mods.Revolutions.Talk.GoForTheEyes"), 180, player);
            if (a == 2) new Talk(0, Language.GetTextValue("Mods.Revolutions.Talk.CritalAttacks"), 180, player);
            if (nowBoss != null && nowBossLifeMax > 39999)
            {
                if (a == 3) new Talk(0, Language.GetTextValue("Mods.Revolutions.Talk.NewHistory01"), 180, player);
                if (a == 4) new Talk(0, Language.GetTextValue("Mods.Revolutions.Talk.NewHistory02"), 180, player);
                if (a == 5) new Talk(0, Language.GetTextValue("Mods.Revolutions.Talk.NewHistory03"), 180, player);
            }
            if (a == 6 && (proj.ranged || proj.magic)) new Talk(0, Language.GetTextValue("Mods.Revolutions.Talk.Attack03"), 180, player);
            if (a == 7) new Talk(0, Language.GetTextValue("Mods.Revolutions.Talk.Attack05"), 180, player);
            if (a == 8 && proj.melee) new Talk(0, Language.GetTextValue("Mods.Revolutions.Talk.Attack04"), 180, player);
            if (a == 6 && proj.minion) new Talk(0, Language.GetTextValue("Mods.Revolutions.Talk.Attack06"), 180, player);
            if (proj.type == ProjectileID.FallingStar && nowBoss != null && nowBoss.type == NPCID.SkeletronHead && a == 1 && timer % 2 == 0) new Talk(0, Language.GetTextValue("Mods.Revolutions.Talk.StarCannonSP"), 180, player);
            float lsfix = 1f;
            if (target.type == NPCID.TargetDummy) lsfix = 1 / damage;
            if (sLifeStealcounter < 21 && saviourStatus == 1)
            {
                if (player.armor[0].type == ItemType<SaviourRanged>() && player.armor[1].type == ItemType<SaviourBreastplate>() && player.armor[2].type == ItemType<SaviourLeggings>() && proj.ranged)
                {
                    Helper.LifeSteal(player, damage, 0.04f * lsfix);
                }
                if (player.armor[0].type == ItemType<SaviourMagic>() && player.armor[1].type == ItemType<SaviourBreastplate>() && player.armor[2].type == ItemType<SaviourLeggings>() && proj.magic)
                {
                    Helper.LifeSteal(player, damage, 0.04f * lsfix);
                }
                if (player.armor[0].type == ItemType<SaviourMelee>() && player.armor[1].type == ItemType<SaviourBreastplate>() && player.armor[2].type == ItemType<SaviourLeggings>() && proj.melee)
                {
                    Helper.LifeSteal(player, damage, 0.04f * lsfix);
                }
                if (player.armor[0].type == ItemType<SaviourSummon>() && player.armor[1].type == ItemType<SaviourBreastplate>() && player.armor[2].type == ItemType<SaviourLeggings>() && proj.minion)
                {
                    Helper.LifeSteal(player, damage, 0.04f * lsfix);
                }
                sLifeStealcounter++;
            }
            else if (saviourStatus == -1 && proj.type != mod.ProjectileType("JustDamage"))
            {
                if (justDmgcounter < 21)
                {
                    if (player.armor[0].type == ItemType<SaviourRanged>() && player.armor[1].type == ItemType<SaviourBreastplate>() && player.armor[2].type == ItemType<SaviourLeggings>() && proj.ranged)
                    {
                        Projectile.NewProjectile(target.Center, Vector2.Zero, mod.ProjectileType("JustDamage"), (int)((target.lifeMax - target.life) * 0.0025f), 0, player.whoAmI); ;
                    }
                    if (player.armor[0].type == ItemType<SaviourMagic>() && player.armor[1].type == ItemType<SaviourBreastplate>() && player.armor[2].type == ItemType<SaviourLeggings>() && proj.magic)
                    {
                        Projectile.NewProjectile(target.Center, Vector2.Zero, mod.ProjectileType("JustDamage"), (int)((target.lifeMax - target.life) * 0.0025f), 0, player.whoAmI); ;
                    }
                    if (player.armor[0].type == ItemType<SaviourMelee>() && player.armor[1].type == ItemType<SaviourBreastplate>() && player.armor[2].type == ItemType<SaviourLeggings>() && proj.melee)
                    {
                        Projectile.NewProjectile(target.Center, Vector2.Zero, mod.ProjectileType("JustDamage"), (int)((target.lifeMax - target.life) * 0.0025f), 0, player.whoAmI); ;
                    }
                    if (player.armor[0].type == ItemType<SaviourSummon>() && player.armor[1].type == ItemType<SaviourBreastplate>() && player.armor[2].type == ItemType<SaviourLeggings>() && proj.minion)
                    {
                        Projectile.NewProjectile(target.Center, Vector2.Zero, mod.ProjectileType("JustDamage"), (int)((target.lifeMax - target.life) * 0.0025f), 0, player.whoAmI); ;
                    }
                }
            }
        }

        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            if (nowBoss != null && hitBonuscounter < 3 && bossFightTimer < 600)
            {
                bossFightTimer++;
                hitBonuscounter++;
            }
            Random rd = new Random();
            int a = rd.Next(0, 60 / player.HeldItem.useTime * 20);
            if (a == 1) new Talk(0, Language.GetTextValue("Mods.Revolutions.Talk.GoForTheEyes"), 180, player);
            if (a == 2) new Talk(0, Language.GetTextValue("Mods.Revolutions.Talk.CritalAttacks"), 180, player);
            if (nowBoss != null && nowBossLifeMax > 39999)
            {
                if (a == 3) new Talk(0, Language.GetTextValue("Mods.Revolutions.Talk.NewHistory01"), 180, player);
                if (a == 4) new Talk(0, Language.GetTextValue("Mods.Revolutions.Talk.NewHistory02"), 180, player);
                if (a == 5) new Talk(0, Language.GetTextValue("Mods.Revolutions.Talk.NewHistory03"), 180, player);
            }
            if (a == 8) new Talk(0, Language.GetTextValue("Mods.Revolutions.Talk.Attack04"), 180, player);
            if (sLifeStealcounter < 21 && saviourStatus == 1 && target.type != NPCID.TargetDummy)
            {
                if (player.armor[0].type == ItemType<SaviourMelee>() && player.armor[1].type == ItemType<SaviourBreastplate>() && player.armor[2].type == ItemType<SaviourLeggings>())
                {
                    Helper.LifeSteal(player, damage, 0.08f);
                }
                sLifeStealcounter++;
            }
            else if (saviourStatus == -1 && justDmgcounter < 21)
            {
                if (player.armor[0].type == ItemType<SaviourMelee>() && player.armor[1].type == ItemType<SaviourBreastplate>() && player.armor[2].type == ItemType<SaviourLeggings>())
                {
                    Projectile.NewProjectile(target.Center, Vector2.Zero, mod.ProjectileType("JustDamage"), (int)((target.lifeMax - target.life) * 0.0025f), 0, player.whoAmI); ;
                    justDmgcounter++;
                }
            }
        }
        public override void OnRespawn(Player player)
        {
            Random rd = new Random();
            int a = rd.Next(0, 7);
            if (a == 1) new Talk(0, Language.GetTextValue("Mods.Revolutions.Talk.Respawn01"), 180, player);
            if (a == 2) new Talk(0, Language.GetTextValue("Mods.Revolutions.Talk.Respawn02"), 180, player);
            if (a == 3) new Talk(0, Language.GetTextValue("Mods.Revolutions.Talk.Respawn03"), 180, player);
            if (a == 4) new Talk(0, Language.GetTextValue("Mods.Revolutions.Talk.Respawn04"), 180, player);
            if (a == 5) new Talk(0, Language.GetTextValue("Mods.Revolutions.Talk.Respawn05"), 180, player);
            if (a == 6) new Talk(0, Language.GetTextValue("Mods.Revolutions.Talk.Respawn06"), 180, player);
        }
        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            Random rd = new Random();
            int a = rd.Next(0, 5);
            if (a == 1) new Talk(0, Language.GetTextValue("Mods.Revolutions.Talk.Die01"), 180, player);
            if (a == 2) new Talk(0, Language.GetTextValue("Mods.Revolutions.Talk.Die02"), 180, player);
            if (a == 3) new Talk(0, Language.GetTextValue("Mods.Revolutions.Talk.Die03"), 180, player);
            if (a == 4) new Talk(0, Language.GetTextValue("Mods.Revolutions.Talk.Die07"), 180, player);
            if (nowBoss != null)
            {
                if (a == 1) new Talk(0, Language.GetTextValue("Mods.Revolutions.Talk.Die04"), 180, player);
                if (a == 2) new Talk(0, Language.GetTextValue("Mods.Revolutions.Talk.Die05"), 180, player);
                if (a == 4) new Talk(0, Language.GetTextValue("Mods.Revolutions.Talk.Die06"), 180, player);
            }
        }
        public override void OnHitByNPC(NPC npc, int damage, bool crit)
        {
            if (nowBoss != null && difficulty > 0 && hitcounter < 3)
            {
                difficulty--;
                hitcounter++;
            }
        }
        public override void OnHitByProjectile(Projectile proj, int damage, bool crit)
        {
            if (nowBoss != null && difficulty > 0 && hitcounter < 3)
            {
                difficulty--;
                hitcounter++;
            }
        }
        public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
        {
            if (Revolutions.Settings.extraAI)
            {
                if (nowBoss != null && nowBoss.type != NPCID.Plantera) damage = (int)(damage * ((difficulty * difficulty / 13334f) + 0.75f));
                if (nowBoss != null && nowBoss.type == NPCID.Plantera) damage = (int)(damage * ((difficulty * difficulty / 50000f) + 1f));
            }
        }
        public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
        {
            if (Revolutions.Settings.extraAI)
            {
                if (nowBoss != null && nowBoss.type != NPCID.Plantera) damage = (int)(damage * ((difficulty * difficulty / 13334f) + 0.75f));
                if (nowBoss != null && nowBoss.type == NPCID.Plantera) damage = (int)(damage * ((difficulty * difficulty / 50000f) + 1f));
            }
        }
        public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            if (Revolutions.Settings.extraAI && nowBoss != null) damage = (int)(damage * ((bossFightTimer * bossFightTimer / 36000000f) + 1f));
        }
        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (Revolutions.Settings.extraAI && nowBoss != null) damage = (int)(damage * ((bossFightTimer * bossFightTimer / 36000000f) + 1f));
        }
    }
}

