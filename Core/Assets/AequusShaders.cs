﻿using ReLogic.Content;

namespace AequusRemake.Core.Assets;

public sealed partial class AequusShaders : AssetManager<Effect> {
    public static readonly string EffectPath = "Effects/{0}";

    public static readonly RequestCache<Effect> Gamestar = New("Gamestar");
    public static readonly RequestCache<Effect> SeaFirefly = New("SeaFireflies");
    public static readonly RequestCache<Effect> FadeToCenter = New("FadeToCenter");
    public static readonly RequestCache<Effect> UVVertexShader = New("UVVertexShader");
    public static readonly RequestCache<Effect> BubbleMerge = New("BubbleMerge");
    public static readonly RequestCache<Effect> Multiply = New("Multiply");
    public static readonly RequestCache<Effect> LuminentMultiply = New("LuminentMultiply");
    public static readonly RequestCache<Effect> VertexShader = New("VertexShader");
    public static readonly RequestCache<Effect> LegacyMiscEffects = New("LegacyMiscEffects");
    public static readonly RequestCache<Effect> LegacyVertexShader = New("LegacyVertexShader");

    private static RequestCache<Effect> New(string name) {
        return new RequestCache<Effect>(string.Format(EffectPath, name));
    }

    public static Asset<Effect> Get(string name) {
        return ModContent.Request<Effect>(string.Format($"AequusRemake/{EffectPath}", name));
    }
}