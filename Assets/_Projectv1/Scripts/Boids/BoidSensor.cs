using System.Collections.Generic;
using UnityEngine;
using Managers;

namespace Boids
{
    public class BoidSensor : MonoBehaviour
    {
        private Transform cachedTransform;

       
        private readonly List<BoidAgent> cachedNeighbors = new List<BoidAgent>();

        private void Awake()
        {
            cachedTransform = transform;
        }

       
        public IReadOnlyList<BoidAgent> GetNeighbors(float radius)
        {
            cachedNeighbors.Clear(); 
            if (GameManager.Instance == null) return cachedNeighbors;

            float sqrRadius = radius * radius;
            Vector3 currentPos = cachedTransform.position;

            foreach (var agent in GameManager.Instance.ActiveBoids)
            {
                if (agent == null || agent.gameObject == gameObject || agent.IsDestroyed) continue;

                float sqrDist = (agent.CachedTransform.position - currentPos).sqrMagnitude;
                if (sqrDist <= sqrRadius)
                {
                    cachedNeighbors.Add(agent);
                }
            }
            return cachedNeighbors;
        }

        public Transform GetNearestHunter(float radius)
        {
            if (GameManager.Instance == null || GameManager.Instance.Hunter == null) return null;

            float sqrRadius = radius * radius;
            Transform hunterTransform = GameManager.Instance.Hunter.CachedTransform;
            float sqrDist = (hunterTransform.position - cachedTransform.position).sqrMagnitude;

            if (sqrDist <= sqrRadius)
            {
                return hunterTransform;
            }
            return null;
        }

        public Transform GetNearestInterestObject(float radius)
        {
            if (GameManager.Instance == null || GameManager.Instance.Spawner == null) return null;

            float sqrRadius = radius * radius;
            Vector3 currentPos = cachedTransform.position;
            Transform nearest = null;
            float minSqrDist = float.MaxValue;

            foreach (var obj in GameManager.Instance.Spawner.ActiveObjects)
            {
                if (obj == null) continue;

                Transform objTransform = obj.CachedTransform;
                float sqrDist = (objTransform.position - currentPos).sqrMagnitude;

                if (sqrDist <= sqrRadius && sqrDist < minSqrDist)
                {
                    minSqrDist = sqrDist;
                    nearest = objTransform;
                }
            }
            return nearest;
        }
    }
}