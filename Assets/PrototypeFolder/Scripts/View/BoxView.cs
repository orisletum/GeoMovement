using UnityEngine;
using Zenject;

namespace GeoMovement
{
    public class BoxView : MonoBehaviour, IPoolable<BoxView.Settings, IMemoryPool>
    {
        private IMemoryPool _pool;
        private int _id;
        private Vector3 _position;
        private Color _color;
        private MeshRenderer _meshRenderer;
        private MaterialPropertyBlock _propertyBlock;

        public class Settings
        {
            public int Id;
            public Vector3 Position;
            public Color Color;
        }

        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _propertyBlock = new MaterialPropertyBlock();
        }

        public void OnSpawned(Settings settings, IMemoryPool pool)
        {
            _pool = pool;
            _id = settings.Id;
            _position = settings.Position;
            _color = settings.Color;
            SetNewPosition(_position);
            SetNewColor(_color);
            gameObject.SetActive(true);
        }

        public void SetNewPosition(Vector3 pos) => transform.position = pos;

        public void SetNewColor(Color color)
        {
            _propertyBlock.SetColor("_BaseColor", color);
            _meshRenderer.SetPropertyBlock(_propertyBlock);
        }

        public void OnDespawned() => gameObject.SetActive(false);

        public void DespawnBox() => _pool?.Despawn(this);

        private void OnDisable() => _pool = null;

        public class Factory : PlaceholderFactory<Settings, BoxView> { }
    }

}