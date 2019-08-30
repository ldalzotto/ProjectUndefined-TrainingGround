namespace CoreGame
{
    public static class OutlineComputeShaderConstants
    {
        public const string TEXTURE_WIDTH = "TextureWidth";
        public const string TEXTURE_HEIGHT = "TextureHeight";

        public const string PRE_EFFECT_TEXTURES = "_TmpRangeRenderArrayBuffer";
        public const string OUTLINE_TARGET_TEXTURE = "_RangeRenderBuffer";

        public const string RANGE_OUTLINE_KERNEL = "RangeOutline";
        public const string OUTLINE_KERNEL = "Outline";
    }
}
