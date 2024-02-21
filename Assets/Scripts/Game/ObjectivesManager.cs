using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

using Bytes;

namespace Kraken
{
    public class ObjectivesManager : KrakenNetworkedManager
    {
        [SerializeField] private List<ObjectiveSO> _allObjectives;

        private ObjectiveInstance currentObjective = null;
        private static int objectiveIndex = 0;
        private void Start()
        {
            if (!_isMaster) return;

            EventManager.AddEventListener(EventNames.StartObjectives, HandleStartObjectives);
            EventManager.AddEventListener(EventNames.NextObjective, HandleNextObjectives);
            EventManager.AddEventListener(EventNames.StopObjectives, HandleStopObjectives);
        }

        private void OnDestroy()
        {
            if (!_isMaster) return;

            EventManager.RemoveEventListener(EventNames.StartObjectives, HandleStartObjectives);
            EventManager.RemoveEventListener(EventNames.NextObjective, HandleNextObjectives);
            EventManager.RemoveEventListener(EventNames.StopObjectives, HandleStopObjectives);
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
            currentObjective?.TriggerObjective();
            
            UpdateObjectiveUI();
        }

        private void HandleStopObjectives(BytesData data)
        {
            Debug.Log("Stop objectives!");

            currentObjective = null;
            UpdateObjectiveUI();
        }

        private void UpdateObjectiveUI()
        {
            if(currentObjective == null)
            {
                photonView.RPC(nameof(RPC_All_UpdateObjectiveUI), RpcTarget.All, "");
                return;
            }

            ObjectiveSO objectiveSO = currentObjective.objectiveSO;
            photonView.RPC(nameof(RPC_All_UpdateObjectiveUI), RpcTarget.All, objectiveSO.name);
        }

        [PunRPC]
        private void RPC_All_UpdateObjectiveUI(string objectiveName) 
        {
            EventManager.Dispatch(EventNames.UpdateObjectiveUI, new UpdateObjectiveUIData(objectiveName));
        }

        private ObjectiveInstance GetNextObjective()
        {
            if (objectiveIndex >= _allObjectives.Count) {

                EventManager.Dispatch(EventNames.PlayerWin, null);
                return null;
            } 

            var nextObjective = new ObjectiveInstance(_allObjectives[objectiveIndex]);
            objectiveIndex++;
            return nextObjective;
        }
    }
}