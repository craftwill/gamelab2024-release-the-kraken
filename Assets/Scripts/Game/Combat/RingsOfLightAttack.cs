using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bytes;
using Kraken.Network;
using Photon.Pun;
using UnityEngine.VFX;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Kraken
{
    public class RingsOfLightAttack : MonoBehaviourPun
    {
        //left for designer to test values if needed
        [SerializeField] private float _ring1ChargeTime;
        [SerializeField] private float _ring2ChargeTime;
        [SerializeField] private float _ring1Radius;
        [SerializeField] private float _ring2Radius;
        [SerializeField] private BossSoundComponent _soundComponent;

        [SerializeField] private GameObject _ringOfLightTelegraphPrefab;
        [SerializeField] private GameObject _ringOfLightVFX;

        public void StartAttack(float ring1ChargeTime, float ring2ChargeTime, float ring1Radius, float ring2Radius, int damage)
        {
            photonView.RPC(nameof(RPC_ALL_RingsOfLight), RpcTarget.All, ring1ChargeTime, ring2ChargeTime, ring1Radius, ring2Radius, damage);
        }

        [PunRPC]
        private void RPC_ALL_RingsOfLight(float ring1ChargeTime, float ring2ChargeTime, float ring1Radius, float ring2Radius, int damage)
        {
            RingTelegraph telegraph = Instantiate(_ringOfLightTelegraphPrefab, this.transform.position, _ringOfLightTelegraphPrefab.transform.rotation, this.transform).GetComponent<RingTelegraph>();
            telegraph.StartTelegraph(ring1ChargeTime, ring1Radius, 0f, damage);
            telegraph._playSoundDelegate = _soundComponent.PlayRingsOfLightHitSound;

            var rol = Instantiate(_ringOfLightVFX, transform.position, telegraph.transform.rotation);
            var llc = rol.GetComponent<LimitedLifetimeComponent>();
            llc.StartNewLifeTime(ring1ChargeTime + 1f);
            var vfx = rol.GetComponent<VisualEffect>();
            vfx.SetFloat("Charge Radius", ring1Radius);
            vfx.SetFloat("Charge Length", ring1ChargeTime);
            vfx.SetFloat("Ring Start Radius", 0f);
            vfx.SetFloat("Ring End Radius", ring1Radius);
            vfx.enabled = true;

            StartCoroutine(SpawnSecondRing(ring1ChargeTime, ring2ChargeTime, ring2Radius, ring1Radius, damage));
        }

        private IEnumerator SpawnSecondRing(float ring1ChargeTime, float ring2ChargeTime, float ring2Radius, float ring1Radius, int damage)
        {
            yield return new WaitForSeconds(ring1ChargeTime);
            RingTelegraph telegraph2 = Instantiate(_ringOfLightTelegraphPrefab, this.transform.position, _ringOfLightTelegraphPrefab.transform.rotation, this.transform).GetComponent<RingTelegraph>();
            telegraph2.StartTelegraph(ring2ChargeTime, ring2Radius, ring1Radius, damage);
            telegraph2._playSoundDelegate = _soundComponent.PlayRingsOfLightHitSound;

            var rol = Instantiate(_ringOfLightVFX, transform.position, telegraph2.transform.rotation);
            var llc = rol.GetComponent<LimitedLifetimeComponent>();
            llc.StartNewLifeTime(ring2ChargeTime + 1f);
            var vfx = rol.GetComponent<VisualEffect>();
            vfx.SetFloat("Charge Radius", ring1Radius + ring2Radius);
            vfx.SetFloat("Charge Length", ring2ChargeTime);
            vfx.SetFloat("Ring Start Radius", ring1Radius);
            vfx.SetFloat("Ring End Radius", ring1Radius + ring2Radius);
            vfx.enabled = true;
        }

        //two functions method for designer testing as well
        public void StartAttack()
        {
            RingTelegraph telegraph = Instantiate(_ringOfLightTelegraphPrefab, this.transform.position, _ringOfLightTelegraphPrefab.transform.rotation, this.transform).GetComponent<RingTelegraph>();

            telegraph.StartTelegraph(_ring1ChargeTime, _ring1Radius);
            SpawnSecondRing(_ring1ChargeTime, _ring2ChargeTime, _ring2Radius, _ring1Radius, 0);
            Invoke(nameof(Ring2), _ring1ChargeTime);
        }
        private void Ring2()
        {
            RingTelegraph telegraph2 = Instantiate(_ringOfLightTelegraphPrefab, this.transform.position, _ringOfLightTelegraphPrefab.transform.rotation,this.transform).GetComponent<RingTelegraph>();
            telegraph2.StartTelegraph(_ring2ChargeTime, _ring2Radius, _ring1Radius);
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

