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

        private ObjectiveInstance currentObjective;

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

            currentObjective = GetNextObjective();
            UpdateObjectiveUI();
        }

        private void HandleNextObjectives(BytesData data)
        {
            Debug.Log("Next objective!");

            currentObjective = GetNextObjective();
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
            ObjectiveSO objectiveSO = currentObjective.objectiveSO;
            photonView.RPC(nameof(RPC_All_UpdateObjectiveUI), RpcTarget.All, objectiveSO.name);
        }

        private void RPC_All_UpdateObjectiveUI(string objectiveName) 
        {
            EventManager.Dispatch(EventNames.UpdateObjectiveUI, new UpdateObjectiveUIData(objectiveName));
        }

        private ObjectiveInstance GetNextObjective()
        {
            return new ObjectiveInstance(_allObjectives[0]);
        }
    }
}