using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kraken.Network;
using Photon.Pun;
using UnityEngine.VFX;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Kraken
{
    public class StarfallAttack : MonoBehaviourPun
    {
        [SerializeField] private float _chargeTime;
        [SerializeField] private float _attackRadius;
        [SerializeField] private int _starCount;
        [SerializeField] private float _delayBetweenStars;
        [SerializeField] private float _telegraphRadius;
        [SerializeField] private BossSoundComponent _soundComponent;

        [SerializeField] private GameObject _starfallTelegraphPrefab;
        [SerializeField] private GameObject _starfallVFX;

        public void StartAttack(float chargeTime, float attackRadius, int starCount, float delayBetweenStars, float telegraphRadius, int damage)
        {
            _chargeTime = chargeTime;
            _attackRadius = attackRadius;
            _starCount = starCount;
            _delayBetweenStars = delayBetweenStars;
            _telegraphRadius = telegraphRadius;
            StartCoroutine(StarSpawn(chargeTime, telegraphRadius, damage));
        }

        public void StartAttack()
        {
            StartCoroutine(StarSpawn(_chargeTime, _telegraphRadius));
        }

        private IEnumerator StarSpawn(float chargeTime = 1f, float telegraphRadius = 1f, int damage = 0)
        {
            List<Vector3> spawnPoints = GetSpawnPoints();

            foreach(Vector3 s in spawnPoints)
            {
                Vector3 finalPos = s + transform.position;

                if (PhotonNetwork.IsMasterClient)
                {
                    photonView.RPC(nameof(RPC_ALL_Starfall), RpcTarget.All, finalPos, chargeTime, telegraphRadius, damage);
                }
                else
                {
                    RPC_ALL_Starfall(finalPos, chargeTime, telegraphRadius, damage);
                }

                yield return new WaitForSeconds(_delayBetweenStars);
            }
        }

        [PunRPC]
        private void RPC_ALL_Starfall(Vector3 pos, float chargeTime, float telegraphRadius, int damage)
        {
            RingTelegraph telegraph = Instantiate(_starfallTelegraphPrefab, pos, _starfallTelegraphPrefab.transform.rotation, this.transform).GetComponent<RingTelegraph>();
            telegraph.StartTelegraph(chargeTime, telegraphRadius, 0f, damage);
            telegraph._playSoundDelegate = _soundComponent.PlayStarfallHitSound;

            var star = Instantiate(_starfallVFX, telegraph.transform.position, telegraph.transform.rotation);
            var llc = star.GetComponent<LimitedLifetimeComponent>();
            llc.StartNewLifeTime(chargeTime + 1f);
            var vfx = star.GetComponent<VisualEffect>();
            vfx.SetFloat("Charge Radius", telegraphRadius);
            vfx.SetFloat("Charge Length", chargeTime - 0.5f);
            vfx.enabled = true;
        }

        private List<Vector3> GetSpawnPoints()
        {
            var spawnPoints = new List<Vector3>();
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
                    foreach (Vector3 point in spawnPoints)
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
                spawnPoints.Add(p);

            }
            return spawnPoints;
        }
#if UNITY_EDITOR
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
#endif
    }
}