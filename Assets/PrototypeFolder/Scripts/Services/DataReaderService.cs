using UnityEngine;
using UniRx;
using System.IO;
using System;
using System.Collections.Generic;

namespace GeoMovement
{
    public class DataReaderService
    {
        private Dictionary<(int, int), Box> _boxMap;
        private Vector2Int _mapSize;

        public Vector2Int GetSizeMap() => _mapSize;

        public Box GetBoxAtPosition(int x, int y) => _boxMap.TryGetValue((x, y), out var box) ? box : null;

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
            _boxMap = new Dictionary<(int, int), Box>();
            var lines = text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            var boxes = new List<Box>();
            int x = 0;
            int y = 0;

            for (y = 0; y < lines.Length; y++)
            {
                for (x = 0; x < lines[y].Length; x++)
                {
                    char c = lines[y][x];
                    var box = new Box
                    {
                        X = x,
                        Y = y,
                        BoxColor = c
                    };
                    boxes.Add(box);
                    _boxMap[(x, y)] = box;

                }
            }

            _mapSize = new Vector2Int(x, y);

            return boxes.ToArray();
        }
    }
}