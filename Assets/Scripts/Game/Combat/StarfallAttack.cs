using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Kraken.Network;
using Photon.Pun;

namespace Kraken
{
    public class StarfallAttack : MonoBehaviour
    {
        //left for designer to test values if needed
        [SerializeField] private float _chargeTime;
        [SerializeField] private float _attackRadius;
        [SerializeField] private int _starCount;
        [SerializeField] private float _delayBetweenStars;
        [SerializeField] private float _telegraphRadius;

        [SerializeField] private GameObject _starfallTelegraphPrefab;

        public void StartAttack(float chargeTime, float attackRadius, int starCount, float delayBetweenStars, float telegraphRadius, int damage)
        {
            _chargeTime = chargeTime;
            _attackRadius = attackRadius;
            _starCount = starCount;
            _delayBetweenStars = delayBetweenStars;
            _telegraphRadius = telegraphRadius;
            StartCoroutine(StarSpawn(damage));
        }

        public void StartAttack()
        {
            StartCoroutine(StarSpawn());
        }

        private IEnumerator StarSpawn(int damage = 0)
        {
            List<Vector3> spawnPoints = new List<Vector3>();
            float minDistance = 0.2f * _attackRadius;
            
            int maxAttempts = 20;

            for (int i = 0; i < _starCount; ++i)
            {
                Vector3 p = transform.position;
                
                //rejection sampling
                bool valid = false;
                for (int j = 0; j < maxAttempts; ++j)
                {
                    Vector2 rPos = Random.insideUnitCircle * _attackRadius;
                    p = new Vector3(rPos.x, 0, rPos.y);
                    
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

                RingTelegraph telegraph;

                if (PhotonNetwork.IsMasterClient)
                {
                    telegraph = NetworkUtils.Instantiate(_starfallTelegraphPrefab.name, p + transform.position, _starfallTelegraphPrefab.transform.rotation).GetComponent<RingTelegraph>();
                }
                else
                {
                    telegraph = Instantiate(_starfallTelegraphPrefab, p + transform.position, _starfallTelegraphPrefab.transform.rotation).GetComponent<RingTelegraph>();
                }

                telegraph.StartTelegraph(_chargeTime, _telegraphRadius, 0f, damage);

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