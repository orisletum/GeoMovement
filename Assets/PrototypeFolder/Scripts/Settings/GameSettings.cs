using System.Collections.Generic;
using UnityEngine;

namespace GeoMovement
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Settings/Game Settings")]
    public class GameSettings : ScriptableObject
    {
        public List<KeyedObject<Color>> BoxColors = new List<KeyedObject<Color>>();
    }

    [System.Serializable]
    public class KeyedObject<T>
    {
        public string key;
        public T value;

        public KeyedObject(string key, T value)
        {
            this.key = key;
            this.value = value;
        }
    }
}