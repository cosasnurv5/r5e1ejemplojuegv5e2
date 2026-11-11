using UnityEngine;
using FSM;
using Steering;
using Boids;
using Hunter.States;
using Environment;
using Managers;

namespace Hunter
{
    public class HunterNPC : SteeringAgent
    {
        [Header("Requerimientos.")]
        public float TBA = 3f;
        public float RangeAttackRadius = 8f;
        public float MeleeAttackRadius = 2f;

        [Header("Energía.")]
        public float maxEnergy = 100f;
        public float Energy = 100f;
        public float energyD = 10f;
        public float energyR = 20f;

        [Header("Configuración.")]
        public float visionRadius = 10f;
        public WaypointPath path;
        public ObjectSpawner spawner;
        public float mapLimitX = 18f;
        public float mapLimitZ = 18f;

        [Header("DebgR.")]
        public bool showDebugVisuals = true;
        [SerializeField] private Renderer hunterRenderer;

        public FiniteStateMachine FSM { get; private set; }
        public HunterPatrolState PatrolState { get; private set; }
        public HunterAttackState AttackState { get; private set; }
        public HunterGatherState GatherState { get; private set; }
        public HunterRecoveryState RecoveryState { get; private set; }

        public float TBATimer { get; set; }
        public BoidAgent CurrentTarget { get; set; }
        public BoidAgent DestroyedTarget { get; set; }

        protected override void Awake()
        {
            base.Awake(); 

           
            if (GameManager.Instance != null)
            {
                GameManager.Instance.Hunter = this;
            }

            FSM = new FiniteStateMachine();

            PatrolState = new HunterPatrolState(this);
            AttackState = new HunterAttackState(this);
            GatherState = new HunterGatherState(this);
            RecoveryState = new HunterRecoveryState(this);

            if (hunterRenderer == null)
                hunterRenderer = GetComponentInChildren<Renderer>();
        }

        private void Start()
        {
            TBATimer = 0f;
            FSM.ChangeState(PatrolState);
        }

        protected override void Update()
        {
            if (TBATimer > 0)
            {
                TBATimer -= Time.deltaTime;
            }

            FSM.Update();
            base.Update();
        }

        public bool IsTargetOutOfBounds(Vector3 position)
        {
            return Mathf.Abs(position.x) > mapLimitX || Mathf.Abs(position.z) > mapLimitZ;
        }

        public BoidAgent FindValidBoidInVision()
        {
            if (GameManager.Instance == null) return null;

            BoidAgent bestTarget = null;
            float visionRadiusSqr = visionRadius * visionRadius;
            float closestDistanceSqr = Mathf.Infinity;
            Vector3 currentPos = CachedTransform.position;

            foreach (var boid in GameManager.Instance.ActiveBoids)
            {
                if (boid == null || boid.IsDestroyed || !boid.gameObject.activeInHierarchy) continue;

                Vector3 boidPos = boid.CachedTransform.position;

                if (IsTargetOutOfBounds(boidPos)) continue;

                float dSqr = (boidPos - currentPos).sqrMagnitude;

                if (dSqr <= visionRadiusSqr && dSqr < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqr;
                    bestTarget = boid;
                }
            }

            if (bestTarget != null && CurrentTarget != bestTarget)
            {
                Debug.Log($"El HunterNPC, detecto el Boid {bestTarget.name}");
            }

            return bestTarget;
        }

        public BoidAgent FindDestroyedBoidInVision()
        {
            if (GameManager.Instance == null) return null;

            float visionRadiusSqr = visionRadius * visionRadius;
            Vector3 currentPos = CachedTransform.position;

            foreach (var boid in GameManager.Instance.ActiveBoids)
            {
                if (boid == null || !boid.IsDestroyed) continue;

                Vector3 boidPos = boid.CachedTransform.position;
                float dSqr = (boidPos - currentPos).sqrMagnitude;

                if (dSqr <= visionRadiusSqr)
                {
                    return boid;
                }
            }
            return null;
        }

        public void Stop()
        {
            Velocity = Vector3.zero;
        }

        private void OnDrawGizmos()
        {
            if (!showDebugVisuals) return;

            
            Vector3 currentPos = Application.isPlaying ? CachedTransform.position : transform.position;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(currentPos, visionRadius);

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(currentPos, RangeAttackRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(currentPos, MeleeAttackRadius);

            if (CurrentTarget != null)
            {
                Gizmos.color = Color.green;
                Vector3 targetPos = Application.isPlaying ? CurrentTarget.CachedTransform.position : CurrentTarget.transform.position;
                Gizmos.DrawLine(currentPos, targetPos);
            }
        }
    }
}