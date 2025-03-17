using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace PaletteAnalyzerKey
{
    class GodMode
    {
        public static bool Enabled = false;
    }

    class GodModeModPlayer : ModPlayer
    {
        public override bool FreeDodge(Player.HurtInfo info)
        {
            if (GodMode.Enabled)
                return true;
            return base.FreeDodge(info);
        }

        public override void PreUpdate()
        {
            if (GodMode.Enabled)
            {
                Player.statLife = Player.statLifeMax2;
                Player.statMana = Player.statManaMax2;
                Player.wingTime = Player.wingTimeMax;
            }
        }
    }
    class Mods
    {
        private static void Teleport_XY(int X, int Y)
        {
            Player player = Main.player[0];
            Vector2 pos = new Vector2(X, Y);
            pos = new Vector2(pos.X * 16 + 8 - player.width / 2, pos.Y * 16 - player.height);
            player.Teleport(pos, 2, 0);
        }

    }
}
