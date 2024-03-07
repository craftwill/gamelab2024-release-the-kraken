using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Bytes;

namespace Kraken
{
    public class RingOfLightsAttack : MonoBehaviour
    {
        //left as serializable for designer to tweek values if needed
        [SerializeField] private float _ring1ChargeTime;
        [SerializeField] private float _ring2ChargeTime;
        [SerializeField] private float _ring1Radius;
        [SerializeField] private float _ring2Radius;

        [SerializeField] private GameObject _ringOfLightTelegraphPrefab;

        public void StartAttack()
        {
            RingTelegraph telegraph = Instantiate(_ringOfLightTelegraphPrefab, this.transform.position, _ringOfLightTelegraphPrefab.transform.rotation).GetComponent<RingTelegraph>();

            telegraph.StartTelegraph(_ring1ChargeTime, _ring1Radius);
            Invoke(nameof(Ring2), _ring1ChargeTime);

            //Animate.Delay(_ring1ChargeTime, () =>
            //{
            //    StarfallTelegraph telegraph2 = Instantiate(_ringOfLightTelegraphPrefab, this.transform.position, _ringOfLightTelegraphPrefab.transform.rotation).GetComponent<StarfallTelegraph>();
            //    telegraph2.StartTelegraph(_ring2ChargeTime, _ring2Radius, _ring1Radius);
            //}, true);
        }

        private void Ring2()
        {
            RingTelegraph telegraph2 = Instantiate(_ringOfLightTelegraphPrefab, this.transform.position, _ringOfLightTelegraphPrefab.transform.rotation).GetComponent<RingTelegraph>();
            telegraph2.StartTelegraph(_ring2ChargeTime, _ring2Radius, _ring1Radius);
        }

        [CustomEditor(typeof(RingOfLightsAttack))]
        public class CustomButton : Editor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                var script = (RingOfLightsAttack)target;
                if (GUILayout.Button("Trigger attack"))
                {
                    if (Application.isPlaying)
                        script.StartAttack();
                }
            }
        }
    }
}

