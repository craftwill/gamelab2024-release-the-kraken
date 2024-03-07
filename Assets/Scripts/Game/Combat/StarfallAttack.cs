using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Kraken.Network;

namespace Kraken
{
    public class StarfallAttack : MonoBehaviour
    {
        //left as serializable for designer to tweek values if needed
        [SerializeField] private float _chargeTime;
        [SerializeField] private float _attackRadius;
        [SerializeField] private int _starCount;
        [SerializeField] private float _delayBetweenStars;
        [SerializeField] private float _telegraphRadius;

        [SerializeField] private GameObject _starfallTelegraphPrefab;

        [SerializeField, Range(0f,1f)] private float test;
        public void StartAttack(float chargeTime, float attackRadius, int starCount, float delayBetweenStars, float telegraphRadius)
        {
            _chargeTime = chargeTime;
            _attackRadius = attackRadius;
            _starCount = starCount;
            _delayBetweenStars = delayBetweenStars;
            _telegraphRadius = telegraphRadius;
            StartCoroutine(StarSpawn());
        }

        public void StartAttack()
        {
            StartCoroutine(StarSpawn());
        }

        private IEnumerator StarSpawn()
        {
            List<Vector3> spawnPoints = new List<Vector3>();
            float minDistance = 0.3f * _attackRadius;
            
            int maxAttempts = 20;

            for (int i = 0; i < _starCount; ++i)
            {
                Vector3 p = transform.position;
                
                //rejection sampling
                bool valid = false;
                for (int j = 0; j < maxAttempts; ++j)
                {
                    Vector2 rPos = Random.insideUnitCircle * _attackRadius;
                    p = new Vector3(rPos.x, transform.position.y, rPos.y);
                    
                    bool tooClose = false;
                    foreach(Vector3 point in spawnPoints)
                    {
                        var t = Vector3.Distance(p, point);
                        if (t < minDistance)
                        {
                            tooClose = true;
                            break;
                        }
                    }

                    if (!tooClose)
                    {
                        valid = true;
                        break;
                    }
                }

                if (!valid)
                {
                    p = transform.position;
                }
                spawnPoints.Add(p);

                RingTelegraph telegraph = Instantiate(_starfallTelegraphPrefab, p, _starfallTelegraphPrefab.transform.rotation).GetComponent<RingTelegraph>();
                telegraph.StartTelegraph(_chargeTime, _telegraphRadius);

                yield return new WaitForSeconds(_delayBetweenStars);
            }
        }

        [CustomEditor(typeof(StarfallAttack))]
        public class CustomButton : Editor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                var script = (StarfallAttack)target;
                if (GUILayout.Button("Trigger attack"))
                {
                    if (Application.isPlaying)
                        script.StartAttack();
                }
            }
        }
    }
}