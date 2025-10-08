using DG.Tweening;
using System.Linq;
using UniRx;
using UnityEngine;
using Zenject;

namespace GeoMovement
{
    public class MovePlayerService
    {
        public ReactiveProperty<Box>    CurrentPlayerBox = new ReactiveProperty<Box>();
        private DataReaderService       _dataReader;
        private SpawnBoxService         _spawnBoxService;
        private PlayerView              _player;
        private Vector2Int              _mapSize;
        private bool                    _isJumped = false;

        [Inject]
        public void Construct( PlayerView player, DataReaderService dataReader, SpawnBoxService spawnBoxService)
        {
            _player = player;
            _dataReader = dataReader;
            _mapSize = _dataReader.GetSizeMap();
            _spawnBoxService = spawnBoxService;

            Observable.EveryUpdate()
            .Where(_ => Input.anyKeyDown)
            .Subscribe(_ =>
            {
                if(!_isJumped)
                Update();
            });

            CurrentPlayerBox.Value = _spawnBoxService.GetRandomStartBox();
            LoadPlayerPosition(CurrentPlayerBox.Value.MapPosition);
        }

        public void LoadPlayerPosition(Vector2Int pos) => _player.transform.position = new Vector3(pos.x, 1, pos.y);
       
        private void Update()
        {

            if (Input.GetKeyDown(KeyCode.W)) MovePlayer(Vector2Int.up);
            else if (Input.GetKeyDown(KeyCode.S)) MovePlayer(Vector2Int.down);
            else if (Input.GetKeyDown(KeyCode.A)) MovePlayer(Vector2Int.left);
            else if (Input.GetKeyDown(KeyCode.D)) MovePlayer(Vector2Int.right);
        }

        public void MovePlayer(Vector2Int direction)
        {
            _isJumped = true;
            Vector2Int mapPosPlusDirection = CurrentPlayerBox.Value.MapPosition + direction;

            Vector3 newPosition = new Vector3(
                Mathf.Repeat(mapPosPlusDirection.x, _mapSize.x),
                1,
                Mathf.Repeat(mapPosPlusDirection.y, _mapSize.y)
            );
            var playerView = _player.GetComponent<PlayerView>();


            _player.transform.DOJump(newPosition, 2, 1, 0.5f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                var newPos = new Vector2Int((int)newPosition.x, (int)newPosition.z);
                var b = _dataReader.GetBoxAtMapPosition(newPos);
                CurrentPlayerBox.Value = b;
                _isJumped = false;
            });
        }
    }
}