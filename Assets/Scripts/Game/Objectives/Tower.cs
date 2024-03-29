using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Kraken
{
    public class Tower : MonoBehaviourPun
    {
        public static string GetFilePath()
        {
            return Path.Combine(Application.persistentDataPath, "towers.json");
        }

        private enum TowerState
        {
            Inactive = 0,
            Waiting = 1,
            Active = 2
        }

        [SerializeField] private int _id;
        private List<PlayerTowerInteractComponent> _playersInRange = new List<PlayerTowerInteractComponent>();
        private TowerState _towerState;

        private void OnTriggerEnter(Collider other)
        {
            var towerInteract = other.GetComponent<PlayerTowerInteractComponent>();

            if (towerInteract)
            {
                towerInteract.SetNewTowerInRange(this);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var towerInteract = other.GetComponent<PlayerTowerInteractComponent>();

            if (towerInteract)
            {
                towerInteract.SetNewTowerInRange(null);
            }
        }

        public void PlayerTryBuild(PlayerTowerInteractComponent player)
        {
            //circular reference baby
            if (!_playersInRange.Contains(player))
                _playersInRange.Add(player);
            


            if (_playersInRange.Count == 2)
            {
                if(_towerState == TowerState.Inactive)
                {
                    _towerState = TowerState.Waiting;
                }
            }
        }

        public void PlayerCancelBuild(PlayerTowerInteractComponent player)
        {
            _playersInRange.Remove(player);
        }

        private void Start()
        {
            string filePath = GetFilePath();
            if (File.Exists(filePath))
            {
                string jsonData = File.ReadAllText(filePath);
                var dataList = JsonUtility.FromJson<Dictionary<int, int>>(jsonData);

                if (dataList.TryGetValue(_id, out int val))
                {
                    if(val == (int)TowerState.Waiting)
                    {
                        _towerState = TowerState.Active;
                    }
                    if(val == (int)TowerState.Active)
                    {
                        _towerState = TowerState.Inactive;
                    }
                }
                else
                {
                    _towerState = TowerState.Inactive;
                }

                Debug.Log(_towerState);
                dataList.Add(_id, (int)_towerState);
                File.WriteAllText(filePath, JsonUtility.ToJson(jsonData));
            }
            
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}