﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;

namespace Aequus.Content.ExporterQuests
{
    public interface IPlacementData
    {
        List<Point> ScanRoom(NPC townNPC);

        public static Point GetStartingPoint(NPC townNPC)
        {
            var home = new Point(townNPC.homeTileX, townNPC.homeTileY);
            home.Y--;
            while (home.Y > 10 && !Main.tile[home].IsSolid())
            {
                home.Y--;
            }
            home.Y++;
            return home;
        }
    }
}