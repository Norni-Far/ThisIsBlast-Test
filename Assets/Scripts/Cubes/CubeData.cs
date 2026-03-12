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
        Yellow,
        Purple,
        Orange,
        Pink,
        Brown,
        Turquoise
    }

    public CubeColor CubeType;
}