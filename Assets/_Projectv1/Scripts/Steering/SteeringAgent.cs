using UnityEngine;

namespace Steering
{
    public class SteeringAgent : MonoBehaviour
    {
        [Header("Configuración.")]
        public float maxSpeed = 5f;
        public float maxForce = 10f;
        public float mass = 1f;

        public Vector3 Velocity { get; protected set; }
        public Vector3 SteeringForce { get; private set; }

       
        private Transform cachedTransform;
        public Transform CachedTransform
        {
            get
            {
                if (cachedTransform == null)
                {
                    cachedTransform = transform;
                }
                return cachedTransform;
            }
            private set => cachedTransform = value;
        }

        protected virtual void Awake()
        {
            CachedTransform = transform;
        }

        protected virtual void Update()
        {
            SteeringForce = Vector3.ClampMagnitude(SteeringForce, maxForce);
            Vector3 acceleration = SteeringForce / mass;

            Velocity = Vector3.ClampMagnitude(Velocity + acceleration * Time.deltaTime, maxSpeed);

            Vector3 pos = CachedTransform.position;
            pos += Velocity * Time.deltaTime;
            CachedTransform.position = pos;

            if (Velocity.sqrMagnitude > 0.001f)
            {
                CachedTransform.forward = Velocity.normalized;
            }

            SteeringForce = Vector3.zero;
        }

        public void AddForce(Vector3 force)
        {
            SteeringForce += force;
        }
    }
}