﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Revolutions.Utils;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Revolutions.Projectiles.RareWeapon
{
    public class Meteower_Meteor : PowerProj
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Meteor");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 11;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.timeLeft = 30;
            projectile.tileCollide = true;
            projectile.alpha = 255;
            projectile.ignoreWater = true;
            projectile.aiStyle = -1;
            projectile.scale = 0.6f;
            projectile.light = 0.35f;
            projectile.penetrate = 2000;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -1;
        }
        float a, b, m, n;
        int t;
        bool x = false;
        public override void AI()
        {
            t++;
            for (int j = 19; j > 0; j--)
            {
                HyperOldPositon[j] = HyperOldPositon[j - 1];
            }
            HyperOldPositon[0] = projectile.position;
            projectile.alpha = 255;
            if (t < 3)
            {
                //e.scale = 0;
                projectile.alpha = 255;
            }
            if (x == false)
            {
                x = true;
                m = projectile.velocity.X;
                n = projectile.velocity.Y;
                a = projectile.ai[0] - projectile.velocity.X;
                b = projectile.ai[1] - projectile.velocity.Y;
            }
            projectile.velocity.X = t / 15.45f * a + m;
            projectile.velocity.Y = t / 15.45f * b + n;
            //实际上可以用Helper.GetCloser()重写，但是我懒得这么做
        }
        public override void Kill(int timeLeft)
        {
            Color color = Main.player[projectile.owner].GetModPlayer<RevolutionsPlayer>().starFlareColor;
            color = Helper.GetCloserColor(Helper.GetCloserColor(color, Helper.ToGreyColor(color), -2, 1), Color.Black, 4, 7); 
            if (timeLeft == 0)
            {
                Dust f = Dust.NewDustDirect(projectile.position, 0, 0,
                mod.DustType("hyperbola2"), 0, 0, 0, new Color(233, 233, 255), 1.65f);
                f.noGravity = true;
                Dust d = Dust.NewDustDirect(projectile.position - 12 * Helper.ToUnitVector(projectile.position - HyperOldPositon[30]), 24, 24, MyDustId.WhiteTrans, 0, 0, 100, Color.White, 0.8f);
                d.noGravity = true;
                d.velocity *= 2;
                Dust d2 = Dust.NewDustDirect(projectile.position - 12 * Helper.ToUnitVector(projectile.position - HyperOldPositon[30]), 24, 24, MyDustId.WhiteTrans, 0, 0, 100, Color.White, 0.8f);
                d2.noGravity = true;
                d2.velocity *= 2;
                Dust e = Dust.NewDustDirect(projectile.position - 12 * Helper.ToUnitVector(projectile.position - HyperOldPositon[30]), 24, 24, MyDustId.WhiteTrans, 0, 0, 100, color, 0.8f);
                e.noGravity = true;
                e.velocity *= 2;
                Dust e2 = Dust.NewDustDirect(projectile.position - 12 * Helper.ToUnitVector(projectile.position - HyperOldPositon[30]), 24, 24, MyDustId.WhiteTrans, 0, 0, 100, color, 0.8f);
                e2.noGravity = true;
                e2.velocity *= 2;
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    Dust d = Dust.NewDustDirect(projectile.position, 4, 4, MyDustId.WhiteTrans, 0, 0, 100, Color.White, 0.8f);
                    d.noGravity = true;
                    Dust e = Dust.NewDustDirect(projectile.position, 4, 4, MyDustId.WhiteTrans, 0, 0, 100, color, 0.8f);
                    e.noGravity = true;
                }
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Color color = Main.player[projectile.owner].GetModPlayer<RevolutionsPlayer>().starFlareColor;
            color = Helper.GetCloserColor(Helper.GetCloserColor(color, Helper.ToGreyColor(color), -2, 1), Color.Black, 4, 7); 
            projectile.damage = 0;
            if (projectile.timeLeft > 3) projectile.timeLeft = 3;
            Vector2 pos = 2 * target.Center - projectile.position;
            pos.Y += (target.Center.Y - pos.Y) * 2;
            for (int i = 0; i < 7; i++)
            {
                Dust d = Dust.NewDustDirect(pos, 4, 4, MyDustId.WhiteTrans, 0.4f * projectile.velocity.X, 0.4f * projectile.velocity.Y, 100, Color.White, 0.8f);
                d.noGravity = true;
                Dust e = Dust.NewDustDirect(pos, 4, 4, MyDustId.WhiteTrans, 0.4f * projectile.velocity.X, 0.4f * projectile.velocity.Y, 100, color, 0.8f);
                e.noGravity = true;
            }
        }
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            int critfix = 1;
            if (crit) critfix = 2;
            if (target.defense <= projectile.damage * critfix)
            {
                if (target.defense <= 19) damage += target.defense / 2 * critfix;
                if (target.defense > 19) damage += 8 * critfix;
            }
            //无视16防御
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width, projectile.height);
            for (int k = 0; k < 10; k++)
            {
                if (HyperOldPositon[k] == Vector2.Zero) break;
                Vector2 drawPosition = 0.35f * projectile.oldPos[k] + 0.65f * projectile.position;
                drawPosition += new Vector2(0f, projectile.gfxOffY) - Main.screenPosition;
                float sizeFix = 2;
                sizeFix /= 1 + k;
                sizeFix -= 1;
                float sizeFix2 = 11;
                sizeFix2 /= 1 + k;
                sizeFix2 -= 1;
                Color color = Main.player[projectile.owner].GetModPlayer<RevolutionsPlayer>().starFlareColor;
                color = new Color(color.R, color.G, color.B, (int)(255 * sizeFix)) * sizeFix2 * 0.9f;
                spriteBatch.Draw(Main.projectileTexture[ModContent.ProjectileType<RareWeapon.MeteowerHelper>()], drawPosition, null, color, projectile.rotation, drawOrigin, projectile.scale * 0.5f, SpriteEffects.None, 0f);
            }
            return true;
        }
    }
}
