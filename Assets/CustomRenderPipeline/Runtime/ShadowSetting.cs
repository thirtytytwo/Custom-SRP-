using UnityEngine;

public enum TextureSize
{
    _256 = 256, _512 = 512, _1024 = 1024,
    _2048 = 2048, _4096 = 4096, _8192 = 8192
}

[System.Serializable]
public struct Driectional
{
    public TextureSize atlasSize;
}

[System.Serializable]
public class ShadowSetting
{
    [Min(0f)] public float maxDistance = 100f;
    public Driectional directional = new Driectional { atlasSize = TextureSize._1024 };
}
