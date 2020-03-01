using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Revolutions.Utils;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Revolutions.Items.Weapon
{
    public class ThunderBlade : ModItem
    {
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            item.damage = 145;
            item.melee = true;
            item.width = 40;
            item.crit = 14;
            item.height = 20;
            item.useTime = 9;
            item.useAnimation = 9;
            item.useStyle = 1;
            item.noMelee = false;
            item.knockBack = 10;
            item.value = Item.sellPrice(0, 36, 0, 0); ;
            item.rare = 11;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
        }
        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            Projectile.NewProjectile(target.Center + new Vector2((600 + Helper.EntroptPool[damage]), -800 + Helper.EntroptPool[damage + 100]), Vector2.Zero, mod.ProjectileType("Thunder"), damage, 0, player.whoAmI, target.Center.X, target.Center.Y);
        }
        public override void UseStyle(Player player)
        {
            /*Action<GameTime> action = new Action<GameTime>(draw);
            Main.OnPostDraw += action;*/
        }
        public void draw(object ob)
        {
            
        }
        
    }
}