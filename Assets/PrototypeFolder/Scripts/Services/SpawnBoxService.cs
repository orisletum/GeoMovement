using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using Zenject;

namespace GeoMovement
{
    public class SpawnBoxService
    {
        private Box                     _currentCenterBox;
        private List<BoxView>           _boxesList = new List<BoxView>();
        private Box[]                   _allBoxes;
        private BoxView.Factory         _boxFactory;
        private DataReaderService       _dataReader;
        private GameSettings            _gameSettings;
        private string                  _fileName = "level_data";
        private Vector2Int              _mapSize;

        [Inject]
        public void Construct(GameSettings gameSettings, BoxView.Factory boxFactory, DataReaderService dataReader)
        {
            _gameSettings = gameSettings;
            _boxFactory = boxFactory;
            _dataReader = dataReader;

            _dataReader.ReadFile(_fileName)
                .Subscribe(
                    boxes => OnBoxesLoaded(boxes),
                    error => Debug.LogError($"Error loading boxes: {error}"),
                    () => Debug.Log("Boxes loaded")
                );

            Observable.EveryUpdate()
                .Where(_ => Input.anyKeyDown)
                .ThrottleFirst(TimeSpan.FromSeconds(0.1f))
                .Subscribe(_ =>
                {
                    Update();
                });

        }
        private void OnBoxesLoaded(Box[] boxes)
        {
            _allBoxes = boxes;
            if (_allBoxes.Length > 0)
            {
                var randomBox = UnityEngine.Random.Range(0, _allBoxes.Length);
                _currentCenterBox = _allBoxes[randomBox];
                _mapSize = _dataReader.GetSizeMap();
                Debug.Log(_mapSize);
                ShowBoxGroupAtPosition(_currentCenterBox.X, _currentCenterBox.Y);
                
            }
        }

        private void Update()
        {
            if      (Input.GetKeyDown(KeyCode.W))   MoveCenter(0, 1);
            else if (Input.GetKeyDown(KeyCode.S))   MoveCenter(0, -1);
            else if (Input.GetKeyDown(KeyCode.A))   MoveCenter(-1, 0);
            else if (Input.GetKeyDown(KeyCode.D))   MoveCenter(1, 0);
        }

        private void MoveCenter(int dx, int dy)
        {
            int newX = _currentCenterBox.X + dx;
            int newY = _currentCenterBox.Y + dy;

            newX = UpdatePosition(newX, _mapSize.x);
            newY = UpdatePosition(newY, _mapSize.y);

            Box newBox = _dataReader.GetBoxAtPosition(newX, newY);
            if (newBox != null)
            {
                _currentCenterBox = newBox;
                ShowBoxGroupAtPosition(newX, newY);
            }
        }

        public int UpdatePosition(int value, int maxSize)
        {
            if (value < 0) value += maxSize;
            else if (value >= maxSize) value -= maxSize;
            return value;
        }

        private void ShowBoxGroupAtPosition(int x, int y)
        {
            ClearDisplayedBoxes();

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    int posX = x + dx;
                    int posY = y + dy;
                    posX = UpdatePosition(posX, _mapSize.x);
                    posY = UpdatePosition(posY, _mapSize.y);
                 
                    Box box = _dataReader.GetBoxAtPosition(posX, posY);
                    if (box != null)
                    {
                        CreateBoxObject(box, posX, posY);
                    }
                }
            }
        }

        private void CreateBoxObject(Box box, int x, int y)
        {
            var color = _gameSettings.BoxColors.FirstOrDefault(x => x.key[0] == box.BoxColor);

            var settings = new BoxView.Settings
            {
                Position = new Vector3(box.X, 0, box.Y),
                Id = box.Y * 10 + box.X,
                Color = color.value

            };

            var newBox = _boxFactory.Create(settings);
            _boxesList.Add(newBox);
        }

        private void ClearDisplayedBoxes()
        {
            foreach (var boxObj in _boxesList) 
                boxObj.DespawnBox();
            _boxesList.Clear();
        }
    }
}