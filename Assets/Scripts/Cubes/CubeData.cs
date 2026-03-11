using System;
using NaughtyAttributes;

[Serializable]
public class CubeData
{
    public enum CubeColor
    {
        Green,
        Red,
        Blue,
        Yellow
    }

    public CubeColor CubeType;
}