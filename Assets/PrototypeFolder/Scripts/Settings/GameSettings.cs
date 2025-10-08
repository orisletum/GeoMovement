using System.Collections.Generic;
using UnityEngine;

namespace GeoMovement
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Settings/Game Settings")]
    public class GameSettings : ScriptableObject
    {
        private string                      _fileName = "level_data";

        public string                       FileName => _fileName;
        public List<KeyedObject<Color>>     BoxColors = new List<KeyedObject<Color>>();
    }

    [System.Serializable]
    public class KeyedObject<T>
    {
        public string   Key;
        public T        Value;

        public KeyedObject(string key, T value)
        {
            Key = key;
            Value = value;
        }
    }
}