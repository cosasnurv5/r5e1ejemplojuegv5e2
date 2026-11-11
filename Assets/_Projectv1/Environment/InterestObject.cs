using UnityEngine;

namespace Environment
{
    public class InterestObject : MonoBehaviour
    {
        [Header("Configuración de la Durabilidad.")]
        public float maxDurability = 100f;
        public float currentDurability;

        public Transform CachedTransform { get; private set; }
        private ObjectSpawner spawner;

        private void Awake()
        {
            CachedTransform = transform;
            currentDurability = maxDurability;
        }

        public void Initialize(ObjectSpawner spawnerReference)
        {
            spawner = spawnerReference;
        }

        public void TakeDamage(float amount)
        {
            currentDurability -= amount;
            if (currentDurability <= 0)
            {
                currentDurability = 0;
                DestroyObject();
            }
        }

        private void DestroyObject()
        {
            if (spawner != null)
            {
                spawner.UnregisterObject(this);
            }
            Destroy(gameObject);
        }
    }
}