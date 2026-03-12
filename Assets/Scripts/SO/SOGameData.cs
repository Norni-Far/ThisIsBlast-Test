namespace Blast.Core
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "GameDataSO")]
    public class SOGameData : ScriptableObject
    {
        public List<CubeColorsData> CubeColorsData;
    }

    [Serializable]
    public class CubeColorsData
    {
        public CubeData.CubeColor CubeColor;
        public Material Material;
    }
}
