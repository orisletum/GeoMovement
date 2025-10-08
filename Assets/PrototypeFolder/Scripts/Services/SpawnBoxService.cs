using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

namespace GeoMovement
{
    public class SpawnBoxService
    {
        private Dictionary<Box, BoxView>    _boxesDictionary = new Dictionary<Box, BoxView>();
        private Box[]                       _allBoxes;
        private BoxView.Factory             _boxFactory;
        private DataReaderService           _dataReader;
        private GameSettings                _gameSettings;
        private Vector2Int                  _mapSize;

        public Vector2Int                   MapSize => _mapSize;
        public Dictionary<Box, BoxView>     BoxesDictionary => _boxesDictionary;

        [Inject]
        public void Construct(BoxView.Factory boxFactory, DataReaderService dataReader, GameSettings gameSettings)
        {
            _boxFactory = boxFactory;
            _dataReader = dataReader;
            _gameSettings = gameSettings;

            _dataReader.ReadFile(_gameSettings.FileName)
            .Subscribe(
                boxes => OnBoxesLoaded(boxes),
                error => Debug.LogError($"Error loading boxes: {error}"),
                () => Debug.Log("Boxes loaded")
            );
        }

        private void OnBoxesLoaded(Box[] boxes)
        {
            _allBoxes = boxes;
            if (_allBoxes.Length > 0)
            {
                _mapSize = _dataReader.GetSizeMap();
                Debug.Log(_mapSize);
                ShowField();

            }
        }

        public Box GetRandomStartBox() => _allBoxes[Random.Range(0, _allBoxes.Length)];
        
        private void ShowField()
        {
            for (int x = 0; x < _mapSize.x; x++)
            {
                for (int y = 0; y < _mapSize.y; y++)
                {
                    var pos = new Vector2Int(x, y);
                    Box box = _dataReader.GetBoxAtMapPosition(pos);
                    if (box != null)
                    {
                        CreateBoxView(box);
                    }
                }
            }
        }

        public void CreateBoxView(Box box)
        {
            var settings = new BoxView.Settings
            {
                Box = box
            };

            var newBox = _boxFactory.Create(settings);
            _boxesDictionary.Add(box, newBox);
        }
    }
}