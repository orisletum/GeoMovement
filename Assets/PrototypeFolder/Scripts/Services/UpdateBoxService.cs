using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using Zenject;

namespace GeoMovement
{
    public class UpdateBoxService
    {
        private SpawnBoxService         _spawnBoxService;
        private MovePlayerService       _movePlayerService;
        private DataReaderService       _dataReader;
        private Vector2Int              _mapSize;
        private List<BoxView>           _viewList = new List<BoxView>();

        [Inject]
        public void Construct(SpawnBoxService spawnBoxService, MovePlayerService movePlayerService, DataReaderService dataReader)
        {
            _spawnBoxService = spawnBoxService;
            _movePlayerService = movePlayerService;
            _dataReader = dataReader;
            _mapSize = _dataReader.GetSizeMap();

            _movePlayerService.CurrentPlayerBox
            .ObserveEveryValueChanged(x =>x.Value)
            .Subscribe(playerBox => 
            {
                UpdateVisibility(playerBox);
            });
        }
        public void UpdateVisibility(Box playerBox)
        {

            foreach (var view in _viewList)
            {
                if(view.Box.boxState == BoxState.Selected)
                view.Box.boxState = BoxState.Opened;
            }
            _viewList.Clear();

            var playerPos = playerBox.MapPosition;

            for (int x = -1; x <= 1; x++)
                for (int y = -1; y <= 1; y++)
                {
                    Vector2Int pos = new Vector2Int(
                        (int)Mathf.Repeat(playerPos.x + x, _mapSize.x),
                        (int)Mathf.Repeat(playerPos.y + y, _mapSize.y)
                    );

                    if (_dataReader.BoxMap.TryGetValue(pos, out Box b))
                    {
                        b.boxState = BoxState.Selected;
                        var view = _spawnBoxService.BoxesDictionary.FirstOrDefault(box => box.Key == b).Value;
                        _viewList.Add(view);
                        if(view.Box.boxState != BoxState.Selected)
                        view.Box.boxState = BoxState.Selected;
                    }
                }

            _spawnBoxService.BoxesDictionary.Values.ToList().ForEach(view => {
                switch (view.Box.boxState)
                {
                    case BoxState.Closed:
                        view.SetAlpha(0);
                        break;
                    case BoxState.Opened:
                        view.SetAlpha(0.35f);
                        break;
                    case BoxState.Selected:
                        view.SetAlpha(1);
                        break;
                    default:
                        break;
                }
            });
        }
    }
}