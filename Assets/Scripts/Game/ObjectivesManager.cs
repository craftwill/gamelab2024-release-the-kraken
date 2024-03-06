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
            public Zone spawnLocation;
            public MinimapHighlightComponent minimapHighlight;
        }
        [SerializeField] private List<ObjectiveWithLocation> _allObjectives;
        [SerializeField] private ObjectiveWithLocation _bossObjective;

        private ObjectiveInstance currentObjective = null;
        private int objectiveIndex = 0;
        private int minibossAlives = 0;

        private void Start()
        {
            if (!_isMaster) return;

            // Temporary basic shuffle
            if (Config.current.randomizeObjectives) _allObjectives.MMShuffle();
            
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
            if (objectiveIndex >= _allObjectives.Count) {
                return TrySpawnBossObjective();
            }

            var objective = _allObjectives[objectiveIndex].objective;
            var zone = _allObjectives[objectiveIndex].spawnLocation;
            var minimapHighlight = _allObjectives[objectiveIndex].minimapHighlight;
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