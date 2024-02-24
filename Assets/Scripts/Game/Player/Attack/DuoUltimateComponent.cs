using Kraken.Game;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class DuoUltimateComponent : MonoBehaviourPun
    {
        enum UltimateState
        {
            NotInUltimate,
            WaitingForUltimate,
            InUltimate
        }
        [SerializeField] private TrailRenderer _trailRenderer;
        private GameObject[] _players = { };
        private UltimateState _state = UltimateState.NotInUltimate;
        private bool _otherPlayerWaiting = false;
        private bool _playersSeparated = false;

        private void Update()
        {
            if (!PhotonNetwork.IsMasterClient) return;
            if (_state == UltimateState.InUltimate)
            {
                if (GetDistanceBetweenPlayers() < Config.current.ultimateEndDistance)
                {
                    if (_playersSeparated)
                    {
                        photonView.RPC(nameof(Rpc_All_FinishDrawing), RpcTarget.All);
                    }
                }
                else
                {
                    _playersSeparated = true;
                }
            }
        }

        public void OnDuoUltimateInput(bool input)
        {
            if (_state == UltimateState.InUltimate) return;

            _state = input ? UltimateState.WaitingForUltimate : UltimateState.NotInUltimate;
            if ((_otherPlayerWaiting || !Config.current.requireTwoPlayersForUltimate) && input && GetDistanceBetweenPlayers() < Config.current.ultimateStartMaxDistance)
            {
                photonView.RPC(nameof(RPC_All_StartDrawing), RpcTarget.All);
            }
            else
            {
                photonView.RPC(nameof(RPC_Others_WaitingForUltimate), RpcTarget.Others, input); ;
            }
        }

        [PunRPC]
        public void RPC_Others_WaitingForUltimate(bool isWaiting)
        {
            _otherPlayerWaiting = isWaiting;
        }

        public void OnDuoUltimateReleased()
        {
            if (_state == UltimateState.WaitingForUltimate)
            {
                _state = UltimateState.NotInUltimate;
            }
        }

        [PunRPC]
        public void RPC_All_StartDrawing()
        {
            _state = UltimateState.InUltimate;
            //_otherPlayerWaiting = false;
            _playersSeparated = false;
            _players[0].transform.GetComponentInChildren<TrailRenderer>().AddPosition(_players[1].transform.position);
            _players[1].transform.GetComponentInChildren<TrailRenderer>().AddPosition(_players[0].transform.position);
            foreach (GameObject player in _players)
            {
                player.transform.GetComponentInChildren<TrailRenderer>().emitting = true;
            }
        }

        [PunRPC]
        public void Rpc_All_FinishDrawing()
        {
            List<Vector3> positions = new List<Vector3>();
            foreach (GameObject player in _players)
            {
                TrailRenderer renderer = player.transform.GetComponentInChildren<TrailRenderer>();
                renderer.emitting = false;
                Vector3[] rendererPositions = new Vector3[renderer.positionCount];
                renderer.GetPositions(rendererPositions);
                positions.AddRange(rendererPositions);
                renderer.Clear();
            }
            if (PhotonNetwork.IsMasterClient)
            {
                UltimateGoOff(positions);
            }

            _state = UltimateState.NotInUltimate;
        }

        private float GetDistanceBetweenPlayers()
        {
            if (_players.Length == 2)
            {
                return Vector3.Distance(_players[0].transform.position, _players[1].transform.position);
            }
            else
            {
                _players = GameObject.FindGameObjectsWithTag("Player");
                return 0f;
            }
        }

        private void UltimateGoOff(List<Vector3> positions)
        {
            Vector3 lassoCenter = TempGetUltimateCenter(positions);
            float furthest = TempGetFurthestPositionDistance(positions, lassoCenter);
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            HealthComponent enemyHealthComponent;
            int enemiesAffected = 0;
            foreach (GameObject enemy in enemies)
            {
                if (Vector3.Distance(enemy.transform.position, lassoCenter) < furthest)
                {
                    enemiesAffected++;
                    enemyHealthComponent = enemy.GetComponent<HealthComponent>();
                    if (enemyHealthComponent != null)
                    {
                        enemyHealthComponent.TakeDamage(Config.current.ultimateDamage);
                    }
                }
            }
            Debug.Log("Affecting " + enemiesAffected + " enemies within " + furthest + " units around " + lassoCenter);
        }

        private Vector3 TempGetUltimateCenter(List<Vector3> positions)
        {
            Vector3 center = Vector3.zero;
            foreach (Vector3 position in positions)
            {
                center += position;
            }
            return center / positions.Count;
        }

        private float TempGetFurthestPositionDistance(List<Vector3> positions, Vector3 center)
        {
            float furthestDistance = 0;
            foreach (Vector3 position in positions)
            {
                float distance = Vector3.Distance(center, position);
                if (distance > furthestDistance)
                {
                    furthestDistance = distance;
                }
            }
            return furthestDistance;
        }
    }
}