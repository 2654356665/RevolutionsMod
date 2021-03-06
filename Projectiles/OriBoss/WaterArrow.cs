﻿using Microsoft.Xna.Framework;
using Revolutions.Utils;
using Terraria;
using Terraria.ModLoader;

namespace Revolutions.Projectiles.OriBoss
{
    public class WaterArrow : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ultimate Star");
        }
        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.extraUpdates = 1;
            projectile.hostile = true;
            projectile.timeLeft = 600;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.scale = 1f;
            projectile.alpha = 255;
            projectile.aiStyle = -1;
        }
        public override void AI()
        {
            if (projectile.position.X > Main.screenPosition.X && projectile.position.Y > Main.screenPosition.Y && projectile.position.X < Main.screenPosition.X + Main.screenWidth && projectile.position.Y < Main.screenPosition.Y + Main.screenHeight)
            {
                for (int i = 0; i < 6; i++)
                {
                    Dust d = Dust.NewDustDirect(projectile.Center, 17, 17, MyDustId.Water);
                    d.noGravity = true;
                    Dust e = Dust.NewDustDirect(projectile.Center, 17, 17, MyDustId.BlueCircle);
                    e.noGravity = true;
                }
                Dust f = Dust.NewDustDirect(projectile.Center, 15, 15, MyDustId.BlueWhiteBubble, 0, 0, 0, default(Color), 0.5f);
                f.noGravity = true;
            }
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust d = Dust.NewDustDirect(projectile.Center, 17, 17, MyDustId.Water);
                d.noGravity = true;
                d.velocity *= 2;
                Dust e = Dust.NewDustDirect(projectile.Center, 17, 17, MyDustId.BlueCircle);
                e.noGravity = true;
                e.velocity *= 2;
            }
            Dust f = Dust.NewDustDirect(projectile.Center, 15, 15, MyDustId.BlueWhiteBubble, 0, 0, 0, default(Color), 0.5f);
            f.noGravity = true;
            f.velocity *= 2;
        }
    }
}
