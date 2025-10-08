using UnityEngine;
using UniRx;
using System.IO;
using System;
using System.Collections.Generic;
using Zenject;
using System.Linq;

namespace GeoMovement
{
    public class DataReaderService
    {
        private GameSettings                    _gameSettings;
        private Dictionary<Vector2Int, Box>     _boxMap;
        private Vector2Int                      _mapSize;

        public Dictionary<Vector2Int, Box>      BoxMap=> _boxMap;
        
        [Inject]
        public void Construct(GameSettings gameSettings)
        {
            _gameSettings = gameSettings;
        }

        public Vector2Int GetSizeMap() => _mapSize;

        public Box GetBoxAtMapPosition(Vector2Int pos) => _boxMap.TryGetValue(pos, out var box) ? box : null;

        public IObservable<Box[]> ReadFile(string fileName)
        {
            return Observable.Create<Box[]>(observer =>
            {
                TextAsset textAsset = Resources.Load<TextAsset>(fileName);
                if (textAsset != null)
                {
                    var boxes = ParseText(textAsset.text);
                    observer.OnNext(boxes);
                    observer.OnCompleted();
                }
                else
                {
                    observer.OnError(new FileNotFoundException($"File {fileName} not found in Resources"));
                }

                return Disposable.Empty;
            });
        }

        private Box[] ParseText(string text)
        {
            _boxMap = new Dictionary<Vector2Int, Box>();
            var lines = text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            var boxes = new List<Box>();
            int x = 0;
            int y = 0;

            for (y = 0; y < lines.Length; y++)
            {
                for (x = 0; x < lines[y].Length; x++)
                {
                    char c = lines[y][x];
                    var color = _gameSettings.BoxColors.FirstOrDefault(x => x.Key[0] == c).Value;
                    var box = new Box
                    {
                        MapPosition = new Vector2Int(x, y),
                        BoxColor = color,
                        boxState = BoxState.Closed
                    };
                    boxes.Add(box);
                    _boxMap[new Vector2Int(x, y)] = box;

                }
            }
            _mapSize = new Vector2Int(x, y);

            return boxes.ToArray();
        }
    }
}