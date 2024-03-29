using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

using Bytes;
using MoreMountains.Tools;

namespace Kraken
{
    public class ObjectivesManager : KrakenNetworkedManager
    {
        [System.Serializable]
        public struct ObjectiveWithLocation
        {
            public ObjectiveSO objective;
            public List<Zone> spawnLocation;
            public List<MinimapHighlightComponent> minimapHighlight;
        }
        [SerializeField] private List<ObjectiveWithLocation> _allSpawnObjectives;
        [SerializeField] private List<ObjectiveWithLocation> _allMinibossObjectives;
        [SerializeField] private ObjectiveWithLocation _bossObjective;

        private List<ObjectiveWithLocation> _loadedObjectives;
        private ObjectiveInstance currentObjective = null;
        private int objectiveIndex = 0;
        private int minibossAlives = 0;

        private void Start()
        {
            if (!_isMaster) return;

            // Temporary basic shuffle
            if (Config.current.randomizeObjectives)
            {
                _allSpawnObjectives.MMShuffle();
                _allMinibossObjectives.MMShuffle();
            }

            _loadedObjectives = new List<ObjectiveWithLocation>();
            _loadedObjectives.AddRange(_allSpawnObjectives.GetRange(0, 0));
            _loadedObjectives.AddRange(_allMinibossObjectives.GetRange(0, 0));
            if (Config.current.randomizeObjectives) _loadedObjectives.MMShuffle();

            EventManager.AddEventListener(EventNames.StartObjectives, HandleStartObjectives);
            EventManager.AddEventListener(EventNames.NextObjective, HandleNextObjectives);
            EventManager.AddEventListener(EventNames.StopObjectives, HandleStopObjectives);
            EventManager.AddEventListener(EventNames.MinibossCountChange, HandleMinibossCount);
        }

        private void OnDestroy()
        {
            if (!_isMaster) return;

            EventManager.RemoveEventListener(EventNames.StartObjectives, HandleStartObjectives);
            EventManager.RemoveEventListener(EventNames.NextObjective, HandleNextObjectives);
            EventManager.RemoveEventListener(EventNames.StopObjectives, HandleStopObjectives);
            EventManager.RemoveEventListener(EventNames.MinibossCountChange, HandleMinibossCount);
        }

        private void HandleMinibossCount(BytesData data)
        {
            int count = (data as IntDataBytes).IntValue;
            minibossAlives += count;

            if (minibossAlives == 0 && currentObjective == null)
                HandleNextObjectives(null);
        }

        private void HandleStartObjectives(BytesData data) 
        {
            Debug.Log("Start objectives!");

            HandleNextObjectives(data);
        }

        private void HandleNextObjectives(BytesData data)
        {
            Debug.Log("Next objective!");

            currentObjective = GetNextObjective();

            if (currentObjective is null) return;

            currentObjective.TriggerObjective();
            
            ObjectiveInstance cur = currentObjective;
            int time = cur.objectiveSO.objectiveTimer;
            //will either end by the time or if something triggers the next objective to start
            Animate.Repeat(1f, () =>
            {
                UpdateObjectiveUI(time);

                if (time == 0)
                {
                    cur.EndObjective(true);
                }
                time--;

                return cur == currentObjective;

            }, cur.objectiveSO.objectiveTimer, true);
        }

        private void HandleStopObjectives(BytesData data)
        {
            Debug.Log("Stop objectives!");
            currentObjective?.EndObjective(false);
            currentObjective = null;
            UpdateObjectiveUI(-1);
        }

        private void UpdateObjectiveUI(int timeLeft)
        {
            if(currentObjective == null)
            {
                photonView.RPC(nameof(RPC_All_UpdateObjectiveUI), RpcTarget.All, "", 0);
                return;
            }

            ObjectiveSO objectiveSO = currentObjective.objectiveSO;

            photonView.RPC(nameof(RPC_All_UpdateObjectiveUI), RpcTarget.All, objectiveSO.objectiveName, timeLeft);
        }

        [PunRPC]
        private void RPC_All_UpdateObjectiveUI(string objectiveName, int objectiveTimer) 
        {
            EventManager.Dispatch(EventNames.UpdateObjectiveUI, new UpdateObjectiveUIData(objectiveName, objectiveTimer));
        }

        private ObjectiveInstance GetNextObjective()
        {
            if (objectiveIndex >= _loadedObjectives.Count) {
                return TrySpawnBossObjective();
            }

            var objective = _loadedObjectives[objectiveIndex].objective;
            var zone = _loadedObjectives[objectiveIndex].spawnLocation;
            var minimapHighlight = _loadedObjectives[objectiveIndex].minimapHighlight;
            var nextObjective = new ObjectiveInstance(objective, zone, minimapHighlight);
            objectiveIndex++;
            return nextObjective;
        }

        private ObjectiveInstance TrySpawnBossObjective()
        {
            if (minibossAlives == 0) return new ObjectiveInstance(_bossObjective.objective, _bossObjective.spawnLocation, _bossObjective.minimapHighlight);

            return null;
        }
    }
}