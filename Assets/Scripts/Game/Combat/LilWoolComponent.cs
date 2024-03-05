using Bytes;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class LilWoolComponent : MonoBehaviourPun
    {
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private Collider _collider;
        private GameObject[] _players = { };
        private GameObject _destinationPlayer = null;
        private Vector3 _velocity = Vector3.up;
        private bool _ragdolling = true;
        private bool _destroyed = false;
        private void Start()
        {
            float maxHorizontalVelocity = Config.current.maxWoolHorizontalVelocity;
            float maxAngularVelocity = Config.current.maxWoolAngularVelocity;

            _velocity *= Random.Range(Config.current.minWoolVerticalVelocity, Config.current.maxWoolVerticalVelocity);
            _velocity += new Vector3(Random.Range(-maxHorizontalVelocity, maxHorizontalVelocity), 0, Random.Range(-maxHorizontalVelocity, maxHorizontalVelocity));
            float angularX = Random.Range(-Config.current.maxWoolAngularVelocity, Config.current.maxWoolAngularVelocity);
            float angularY = Random.Range(-Config.current.maxWoolAngularVelocity, Config.current.maxWoolAngularVelocity);
            float angularZ = Random.Range(-Config.current.maxWoolAngularVelocity, Config.current.maxWoolAngularVelocity);
            _rb.velocity = _velocity;
            _rb.angularVelocity = new Vector3(angularX, angularY, angularZ);
            StartCoroutine(RagdollTimer());
        }

        private void Update()
        {
            if (_players.Length != 2) RefreshPlayerList();
            if (_ragdolling) return;

            Vector3 direction = (_destinationPlayer.transform.position - transform.position).normalized;
            _rb.AddForce(_rb.mass * Config.current.woolAcceleration * direction);
        }

        private void RefreshPlayerList()
        {
            _players = GameObject.FindGameObjectsWithTag("Player");
        }

        private GameObject FindClosestPlayer()
        {
            GameObject closest = null;
            float distance = float.MaxValue;
            foreach (GameObject player in _players)
            {
                float playerDistance = Vector3.Distance(player.transform.position, transform.position);
                if (playerDistance < distance)
                {
                    closest = player;
                    distance = playerDistance;
                }
            }
            return closest;
        }

        private IEnumerator RagdollTimer()
        {
            yield return new WaitForSeconds(Config.current.woolRagdollDuration);
            _ragdolling = false;
            _collider­.enabled = false;
            _rb.useGravity = false;
            _rb.drag = Config.current.woolDrag;
            _destinationPlayer = FindClosestPlayer();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == 11 && !_destroyed) // Player
            {
                _destroyed = true;
                EventManager.Dispatch(EventNames.GainWool, new IntDataBytes(1));
                Destroy(gameObject);
            }
        }
    }

}