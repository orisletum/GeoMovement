using UniRx;
using UnityEngine;
using Zenject;
namespace GeoMovement
{
    public class CameraFollow : MonoBehaviour
    {
        [Inject] private PlayerView     _player;
        private Vector3                 _offset = new Vector3(0, 0, -7);
        private void Start()
        {
            Observable.EveryUpdate()
            .Where(_ => _player != null)
            .Subscribe(_ =>
            {
                transform.position = new Vector3(
                    _player.transform.position.x, 
                    transform.transform.position.y,
                    _player.transform.position.z) + _offset;
            });
        }
    }
}