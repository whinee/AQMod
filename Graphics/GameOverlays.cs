﻿using Aequus.Common.Utilities.Drawing;
using Aequus.Graphics.RenderTargets;
using Aequus.Graphics.Tiles;
using Aequus.Particles;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Aequus.Graphics
{
    public class GameOverlays : ILoadable
    {
        public void Load(Mod mod)
        {
            On.Terraria.Graphics.Effects.OverlayManager.Draw += OverlayManager_Draw;
        }

        private void OverlayManager_Draw(On.Terraria.Graphics.Effects.OverlayManager.orig_Draw orig, OverlayManager self, SpriteBatch spriteBatch, RenderLayers layer, bool beginSpriteBatch)
        {
            if (layer == RenderLayers.ForegroundWater)
            {
                if (beginSpriteBatch)
                    spriteBatch.Begin_World(shader: false);

                SpecialTileRenderer.Render(TileRenderLayer.PostDrawLiquids);
                ParticleSystem.GetLayer(ParticleLayer.AboveLiquid).Draw(spriteBatch);
                if (RadonMossFogRenderer.Instance.IsReady)
                {
                    spriteBatch.End();
                    RadonMossFogRenderer.Instance.DrawOntoScreen(spriteBatch);
                    spriteBatch.Begin_World(shader: false);
                }

                if (beginSpriteBatch)
                    spriteBatch.End();
            }
            orig(self, spriteBatch, layer, beginSpriteBatch);
        }

        public void Unload()
        {
        }
    }
}