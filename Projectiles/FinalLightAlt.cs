﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Revolutions.Projectiles
{
    public class FinalLightAlt : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("LightArrow");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.extraUpdates = 1;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.timeLeft = 200;
            projectile.alpha = 255;
            projectile.ignoreWater = true;
            projectile.aiStyle = 1;
            projectile.arrow = true;
            projectile.penetrate = 20;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -1;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Player player = Main.player[projectile.owner];
            if (Vector2.Distance(projectile.position, player.position) < 0.5f * Vector2.Distance(Vector2.Zero, new Vector2(Main.screenWidth, Main.screenHeight)))
            {
                projectile.damage = (int)(projectile.damage * 0.9f);
                Projectile.NewProjectile(player.Center, Vector2.Zero, mod.ProjectileType("FinalLight"), projectile.damage, 0, projectile.owner, -0.3f * projectile.penetrate + 10, target.whoAmI);
            }
        }
    }
}
