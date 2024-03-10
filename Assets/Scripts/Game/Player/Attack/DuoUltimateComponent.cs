using Bytes;
using Kraken.Game;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;

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
        private GameObject[] _players = {};
        private static UltimateState _state = UltimateState.NotInUltimate;
        private static bool _otherPlayerWaiting = false;
        private bool _playersSeparated = false;
        private bool _canBeEnded = true;
        private static bool _ultimateAvailable = false;
        private static int _woolQuantity = 0;
        private static int _minWoolQuantity = 0;
        public static int _woolUsed = 0;

        private float _distanceTraveled = 0;
        private float _nextWoolUsage = 0;
        private Vector3 _previousPosition;

        private Coroutine _inputTimerCoroutine;

        private void Start()
        {
            _minWoolQuantity = Config.current.ultimateMinWool;
            EventManager.AddEventListener(EventNames.UpdateWoolQuantity, UpdateWoolQuantity);
            if (photonView.IsMine)
            {
                EventManager.AddEventListener(EventNames.UltimateRunning, HandleUltimateRunning);
            };
        }

        private void OnDestroy()
        {
            EventManager.RemoveEventListener(EventNames.UpdateWoolQuantity, UpdateWoolQuantity);
            if (photonView.IsMine)
            {
                EventManager.RemoveEventListener(EventNames.UltimateRunning, HandleUltimateRunning);
            };
        }

        private void Update()
        {
            if (!photonView.IsMine) return;
            if (_state == UltimateState.InUltimate)
            {
                //Calculate distance traveled
                Vector3 distanceVec = transform.position - _previousPosition;
                _distanceTraveled += distanceVec.magnitude;
                _previousPosition = transform.position;
                if (_distanceTraveled > _nextWoolUsage)
                {
                    photonView.RPC(nameof(RPC_Master_DispatchGainWool), RpcTarget.MasterClient, -1);
                    _nextWoolUsage += Config.current.ultimateDistancePerWool;
                    _woolUsed++;
                }

                if (!PhotonNetwork.IsMasterClient || !_canBeEnded) return;

                if (_woolQuantity == 0)
                {
                    photonView.RPC(nameof(Rpc_All_FinishDrawing), RpcTarget.All, true);
                }
                else if (GetDistanceBetweenPlayers() < Config.current.ultimateEndDistance)
                {
                    if (_playersSeparated)
                    {
                        _playersSeparated = false;
                        photonView.RPC(nameof(Rpc_All_FinishDrawing), RpcTarget.All, true);
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
            if (_state == UltimateState.InUltimate)
            {
                // Cancel the ultimate
                photonView.RPC(nameof(RPC_AllCancelUltimate), RpcTarget.All);
                return;
            }
            if (!_ultimateAvailable) return;

            _state = input ? UltimateState.WaitingForUltimate : UltimateState.NotInUltimate;
            if ((_otherPlayerWaiting || !Config.current.requireTwoPlayersForUltimate) && input && GetDistanceBetweenPlayers() < Config.current.ultimateStartMaxDistance)
            {
                photonView.RPC(nameof(RPC_All_StartDrawing), RpcTarget.All);
            }
            else if (input)
            {
                photonView.RPC(nameof(RPC_Others_WaitingForUltimate), RpcTarget.Others, input); ;
            }
        }

        [PunRPC]
        public void RPC_Master_DispatchGainWool(int quantity)
        {
            EventManager.Dispatch(EventNames.GainWool, new IntDataBytes(-1));
        }

        [PunRPC]
        public void RPC_Others_WaitingForUltimate(bool isWaiting)
        {
            if (_inputTimerCoroutine != null)
            {
                StopCoroutine(_inputTimerCoroutine);
                _inputTimerCoroutine = null;
            }
            _inputTimerCoroutine = StartCoroutine(UltimateTriggerTimer());
        }

        private IEnumerator UltimateTriggerTimer()
        {
            _otherPlayerWaiting = true;
            yield return new WaitForSeconds(Config.current.ultimateTriggerTimer);
            _otherPlayerWaiting = false;
        }

        private IEnumerator UltimateMinimumTimer()
        {
            _canBeEnded = false;
            yield return new WaitForSeconds(Config.current.ultimateMinimumDuration);
            _canBeEnded = true;
        }

        [PunRPC]
        public void RPC_All_StartDrawing()
        {
            EventManager.Dispatch(EventNames.UltimateRunning, new BoolDataBytes(true));
        }

        private void HandleUltimateRunning(BytesData data)
        {
            bool isRunning = ((BoolDataBytes)data).BoolValue;
            if (isRunning)
            {
                OnStartDrawing();
            }
        }

        private void OnStartDrawing()
        {
            _otherPlayerWaiting = false;
            _playersSeparated = false;
            _distanceTraveled = 0;
            _previousPosition = transform.position;
            _nextWoolUsage = Config.current.ultimateDistancePerWool;
            _woolUsed = 0;
            _state = UltimateState.InUltimate;
            StartCoroutine(UltimateMinimumTimer());
            if (_players.Length != 2) _players = GameObject.FindGameObjectsWithTag("Player");
            _players[0].transform.GetComponentInChildren<TrailRenderer>().AddPosition(_players[1].transform.position);
            foreach (GameObject player in _players)
            {
                player.transform.GetComponentInChildren<TrailRenderer>().emitting = true;
            }
        }

        IEnumerator UltimateCooldown()
        {
            _ultimateAvailable = false;
            EventManager.Dispatch(EventNames.UpdateUltimateUI, new FloatDataBytes(Config.current.ultimateCooldown));
            yield return new WaitForSeconds(Config.current.ultimateCooldown);
            _ultimateAvailable = true;
            EventManager.Dispatch(EventNames.UpdateUltimateUI, new FloatDataBytes(0));
        }

        private void UpdateWoolQuantity(BytesData data)
        {
            _woolQuantity = ((IntDataBytes)data).IntValue;
            if (_woolQuantity >= _minWoolQuantity)
            {
                _ultimateAvailable = true;
            }
            else
            {
                _ultimateAvailable = false;
            }
        }

        [PunRPC]
        public void Rpc_All_FinishDrawing(bool complete)
        {
            if (_players.Length != 2) _players = GameObject.FindGameObjectsWithTag("Player");
            List<Vector2> positions = new List<Vector2>();
            bool firstPlayer = true;
            foreach (GameObject player in _players)
            {
                TrailRenderer renderer = player.transform.GetComponentInChildren<TrailRenderer>();
                renderer.emitting = false;
                if (PhotonNetwork.IsMasterClient)
                {
                    Vector3[] rendererPositions = new Vector3[renderer.positionCount];
                    renderer.GetPositions(rendererPositions);
                    if (firstPlayer)
                    {
                        Array.Reverse(rendererPositions);
                    }
                    Vector2 pos2d = Vector2.zero;
                    foreach (Vector3 pos in rendererPositions)
                    {
                        pos2d.x = pos.x;
                        pos2d.y = pos.z;
                        positions.Add(pos2d);
                    }
                }
                renderer.Clear();
                firstPlayer = false;
            }
            if (PhotonNetwork.IsMasterClient)
            {
                UltimateGoOff(positions, complete);
            }

            _state = UltimateState.NotInUltimate;
            EventManager.Dispatch(EventNames.UltimateRunning, new BoolDataBytes(false));
        }

        [PunRPC]
        private void RPC_AllCancelUltimate()
        {
            if (_players.Length != 2) _players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in _players)
            {
                TrailRenderer renderer = player.transform.GetComponentInChildren<TrailRenderer>();
                renderer.emitting = false;
                renderer.Clear();
            }
            if (PhotonNetwork.IsMasterClient)
            {
                EventManager.Dispatch(EventNames.GainWool, new IntDataBytes(_woolUsed));
            }
            else
            {
                photonView.RPC(nameof(RPC_Master_RecoverSecondPlayerWool), RpcTarget.MasterClient, _woolUsed);
            }

            _state = UltimateState.NotInUltimate;
            EventManager.Dispatch(EventNames.UltimateRunning, new BoolDataBytes(false));
        }

        [PunRPC]
        private void RPC_Master_RecoverSecondPlayerWool(int quantity)
        {
            EventManager.Dispatch(EventNames.GainWool, new IntDataBytes(quantity));
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

        private void UltimateGoOff(List<Vector2> positions, bool complete)
        {
            float playerDistance = Vector3.Distance(_players[0].transform.position, _players[1].transform.position);
            int damage = 0;
            if (complete)
            {
                damage = Config.current.ultimateDamage;
            }
            else if (playerDistance > Config.current.ultimateMinDamageDistance)
            {
                damage = Config.current.ultimateMinDamage;
            }
            else
            {
                damage = (int) Mathf.Lerp(Config.current.ultimateMinDamage, Config.current.ultimateDamage, playerDistance / Config.current.ultimateMinDamageDistance);
            }
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            HealthComponent enemyHealthComponent;
            int enemiesAffected = 0;
            Vector2 enemyPos2d = Vector2.zero;
            foreach (GameObject enemy in enemies)
            {
                enemyPos2d.x = enemy.transform.position.x;
                enemyPos2d.y = enemy.transform.position.z;
                if (IsEnemyInPolygon(positions, enemyPos2d))
                {
                    enemiesAffected++;
                    enemyHealthComponent = enemy.GetComponent<HealthComponent>();
                    if (enemyHealthComponent != null)
                    {
                        enemyHealthComponent.TakeDamage(damage);
                    }
                }
            }
            Debug.Log(enemiesAffected + " enemies have taken " + damage + " damage by the ultimate");
        }

        /*private Vector3 TempGetUltimateCenter(List<Vector3> positions)
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
        }*/

        // Shamelessly stolen from https://codereview.stackexchange.com/a/108903
        private bool IsEnemyInPolygon(List<Vector2> positions, Vector2 enemyPos)
        {
            int polygonLength = positions.Count, i = 0;
            bool inside = false;
            // x, y for tested point.
            float pointX = enemyPos.x, pointY = enemyPos.y;
            // start / end point for the current polygon segment.
            float startX, startY, endX, endY;
            Vector2 endPoint = positions[polygonLength - 1];
            endX = endPoint.x;
            endY = endPoint.y;
            while (i < polygonLength)
            {
                startX = endX; startY = endY;
                endPoint = positions[i++];
                endX = endPoint.x; endY = endPoint.y;
                //
                inside ^= (endY > pointY ^ startY > pointY) /* ? pointY inside [startY;endY] segment ? */
                          && /* if so, test if it is under the segment */
                          ((pointX - endX) < (pointY - endY) * (startX - endX) / (startY - endY));
            }
            return inside;
        }
    }
}