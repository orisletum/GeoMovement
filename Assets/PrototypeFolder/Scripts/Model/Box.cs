using System;
using UnityEngine;

namespace GeoMovement
{
    public enum BoxState
    {
        Closed,
        Opened,
        Selected
    }
    [Serializable]
    public class Box
    {
        public BoxState     boxState;
        public Vector2Int   MapPosition;
        public Color        BoxColor;
    }
}