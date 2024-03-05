using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class ConeTelegraph : MonoBehaviourPun
    {
        private float _range;
        private float _angle;
        private LayerMask _obstructingLayer;
        private LayerMask _playerLayer;
        private int _triangleResolution;
        private Material _materialInner;
        private Material _materialOuter;

        [SerializeField] private MeshRenderer _meshRendererInner;
        [SerializeField] private MeshRenderer _meshRendererOuter;
        [SerializeField] private MeshFilter _meshFilterInner;
        [SerializeField] private MeshFilter _meshFilterOuter;
        [SerializeField] private InflictDamageComponent _inflictDamageComponent;//could probably avoid using this component

        private Mesh _coneMeshInner;
        private Mesh _coneMeshOuter;

        public void InitSettings(float range, float angle, LayerMask obstructingLayer, LayerMask playerLayer, Material materialInner, Material materialOuter)
        {
            _range = range;
            _angle = angle;
            _obstructingLayer = obstructingLayer;
            _playerLayer = playerLayer;
            _triangleResolution = (int)angle;
            _materialInner = materialInner;
            _materialOuter = materialOuter;

            _meshRendererInner.material = _materialInner;
            _meshRendererOuter.material = _materialOuter;

            _coneMeshInner = new Mesh();
            _coneMeshOuter = new Mesh();

            _angle *= Mathf.Deg2Rad;
        }

        public void DrawCone(float chargeRatio, bool triggerPlayer = false)
        {
            int[] trianglesInner = new int[(_triangleResolution - 1) * 3];
            int[] trianglesOuter = new int[(_triangleResolution - 1) * 6];

            Vector3[] verticesInner = new Vector3[_triangleResolution + 1];
            Vector3[] verticesOuter = new Vector3[_triangleResolution * 2];

            verticesInner[0] = Vector3.zero; //start position

            float currentangle = -_angle / 2;
            float angleIncrement = _angle / (_triangleResolution - 1);
            float sine, cosine;

            var playerHits = new List<Kraken.Game.DetectDamageComponent>();

            for (int i = 0; i < _triangleResolution; i++)
            {
                sine = Mathf.Sin(currentangle);
                cosine = Mathf.Cos(currentangle);

                Vector3 raycastDirection = (transform.forward * cosine) + (transform.right * sine);
                Vector3 vertForward = (Vector3.forward * cosine) + (Vector3.right * sine);

                //Bypassing ontriggerenter from detectdamagecomponent, currently a player can block the attack if the other one is standing behind. It's not a bug but a feature
                if(triggerPlayer && PhotonNetwork.IsMasterClient && Physics.Raycast(transform.position,raycastDirection, out RaycastHit playerHit, _range, _playerLayer))
                {
                    var damageComponent = playerHit.transform.GetComponent<Kraken.Game.DetectDamageComponent>();
                    if (damageComponent && !playerHits.Contains(damageComponent))
                    {
                        playerHits.Add(damageComponent);
                        damageComponent.TakeDamageFromOtherSource(_inflictDamageComponent.Damage);
                    }
                }

                if (Physics.Raycast(transform.position, raycastDirection, out RaycastHit hit, _range, _obstructingLayer))
                {
                    verticesInner[i + 1] = vertForward * Mathf.Min(chargeRatio * _range, hit.distance);
                    verticesOuter[2 * i] = vertForward * Mathf.Min(chargeRatio * _range, hit.distance);
                    verticesOuter[(2 * i) + 1] = vertForward * Mathf.Min(_range, hit.distance);
                }
                else
                {
                    verticesInner[i + 1] = vertForward * chargeRatio * _range;
                    verticesOuter[2 * i] = vertForward * chargeRatio * _range;
                    verticesOuter[(2 * i) + 1] = vertForward * _range;
                }

                currentangle += angleIncrement;
            }

            for (int i = 0, j = 0; i < trianglesInner.Length; i += 3, j++)
            {
                trianglesInner[i] = 0;
                trianglesInner[i + 1] = j + 1;
                trianglesInner[i + 2] = j + 2;
            }

            for (int i = 0, j = 0; i < (_triangleResolution - 1); i++, j += 2)
            {
                int triangleIndex = i * 6;

                trianglesOuter[triangleIndex] = j;
                trianglesOuter[triangleIndex + 1] = j + 2;
                trianglesOuter[triangleIndex + 2] = j + 1;

                trianglesOuter[triangleIndex + 3] = j + 1;
                trianglesOuter[triangleIndex + 4] = j + 2;
                trianglesOuter[triangleIndex + 5] = j + 3;
            }

            _coneMeshInner.Clear();
            _coneMeshOuter.Clear();

            _coneMeshInner.vertices = verticesInner;
            _coneMeshOuter.vertices = verticesOuter;

            _coneMeshInner.triangles = trianglesInner;
            _coneMeshOuter.triangles = trianglesOuter;

            _meshFilterInner.mesh = _coneMeshInner;
            _meshFilterOuter.mesh = _coneMeshOuter;
        }
    }
}