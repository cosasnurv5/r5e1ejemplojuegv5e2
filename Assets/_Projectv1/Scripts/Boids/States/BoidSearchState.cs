using UnityEngine;
using FSM;
using Steering;

namespace Boids.States
{
    public class BoidSearchState : IState
    {
        private readonly BoidAgent boid;

        public BoidSearchState(BoidAgent boid) => this.boid = boid;

        public void Enter() { }

        public void Update()
        {
            if (boid.HitsReceived >= 2)
            {
                boid.FSM.ChangeState(boid.DestroydState);
                return;
            }

            if (boid.Sensor.GetNearestHunter(boid.visionRadius) != null)
            {
                boid.FSM.ChangeState(boid.EvadState);
                return;
            }

            if (boid.CurrentInterestTarget == null)
            {
                boid.FSM.ChangeState(boid.FlockngState);
                return;
            }

           
            float sqrDist = (boid.CachedTransform.position - boid.CurrentInterestTarget.position).sqrMagnitude;
            float sqrInteractDist = boid.interactDistance * boid.interactDistance;

            if (sqrDist <= sqrInteractDist)
            {
                boid.FSM.ChangeState(boid.DamageState);
                return;
            }

            Vector3 arriveForce = SteeringBehaviors.Arrive(boid, boid.CurrentInterestTarget.position, 2f) * boid.arriveWeight;
            boid.AddForce(arriveForce);
        }

        public void Exit() { }
    }
}