using DG.Tweening;
using UnityEngine;
using Zenject;

namespace GeoMovement
{
    public class BoxView : MonoBehaviour, IPoolable<BoxView.Settings, IMemoryPool>
    {
        private MeshRenderer                    _meshRenderer;
        private MaterialPropertyBlock           _propertyBlock;
        private IMemoryPool                     _pool;
        private Vector3                         _position;
        private Color                           _color;
        private float                           _scaleFactor = 1f;
        private Box                             _box;

        public Box                              Box => _box;

        public class Settings
        {
            public Box  Box;
        }

        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _propertyBlock = new MaterialPropertyBlock();
        }

        public void OnSpawned(Settings settings, IMemoryPool pool)
        {
            _box = settings.Box;
            _pool = pool;
            _position = new Vector3(_box.MapPosition.x, 0, _box.MapPosition.y);
            _color = _box.BoxColor;
          
            SetNewPosition(_position);
            SetNewColor(_color);
            gameObject.SetActive(true);
        }

        public void SetNewPosition(Vector3 pos) => transform.position = pos * _scaleFactor;

        public void SetNewColor(Color color)
        {
          
            _propertyBlock.SetColor("_BaseColor", color);
            _meshRenderer.SetPropertyBlock(_propertyBlock);
        }

        public void SetAlpha(float alpha)
        {
            DOTween.To(() => _color.a, a => _color.a = a, alpha, 1f).OnUpdate(() =>
            {
                _propertyBlock.SetColor("_BaseColor", _color);
                _meshRenderer.SetPropertyBlock(_propertyBlock);
            });
        }

        public void OnDespawned() => gameObject.SetActive(false);

        public void DespawnBox() => _pool?.Despawn(this);

        private void OnDisable() => _pool = null;

        public class Factory : PlaceholderFactory<Settings, BoxView> { }
    }
}