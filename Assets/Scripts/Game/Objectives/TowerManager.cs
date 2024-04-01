using Bytes;
using Newtonsoft.Json;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Kraken
{
    public class TowerManager : KrakenNetworkedManager
    {
        public static string GetFilePath()
        {
            return Path.Combine(Application.persistentDataPath, "towers.json");
        }

        public static TowerManager Instance { get; private set; }

        public int TowersBuiltThisRound { get; private set; } = 0;
        public Dictionary<int, int> TowerData { get; private set; } = new Dictionary<int, int>();
        [SerializeField] private List<Tower> _towers;

        protected override void Awake()
        {
            base.Awake();
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        private void Start()
        {
            if (!PhotonNetwork.IsMasterClient)
                return;

            string filePath = GetFilePath();
            if (File.Exists(filePath))
            {
                string jsonData = File.ReadAllText(filePath);
                TowerData = JsonConvert.DeserializeObject<Dictionary<int, int>>(jsonData);

                for(int i = 0; i < _towers.Count; ++i)
                {
                    if (TowerData.TryGetValue(_towers[i].Id, out int val))
                    {
                        if (val == (int)Tower.TowerState.Waiting)
                        {
                            _towers[i].SetNewTowerState(Tower.TowerState.Active);
                        }
                        if (val == (int)Tower.TowerState.Active)
                        {
                            _towers[i].SetNewTowerState(Tower.TowerState.Inactive);
                        }
                    }
                    else
                    {
                        _towers[i].SetNewTowerState(Tower.TowerState.Inactive);
                    }
                }
            }
        }

        public void TowerBuilt()
        {
            EventManager.Dispatch(EventNames.GainWool, new IntDataBytes(-Config.current.towerWoolCost));
            photonView.RPC(nameof(RPC_All_IncrementTowerBuilt), RpcTarget.All);
        }

        [PunRPC]
        private void RPC_All_IncrementTowerBuilt()
        {
            TowersBuiltThisRound++;
        }

        public void WriteFile()
        {
            File.WriteAllText(GetFilePath(), JsonConvert.SerializeObject(TowerData));
        }
    }
}