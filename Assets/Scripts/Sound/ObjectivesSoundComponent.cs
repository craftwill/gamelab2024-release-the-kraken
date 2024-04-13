using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class ObjectivesSoundComponent : MonoBehaviourPun
    {
        [SerializeField] private AK.Wwise.Event _territoryObjectiveSound;
        [SerializeField] private AK.Wwise.Event _minibossObjectiveSound;
        [SerializeField] private AK.Wwise.Event _bossObjectiveSound;

        private void Start()
        {
            AkSoundEngine.RegisterGameObj(gameObject);
        }

        private void OnDestroy()
        {
            AkSoundEngine.UnregisterGameObj(gameObject);
        }

        private void Update()
        {
            AkSoundEngine.SetObjectPosition(gameObject, transform);
        }

        public void PlayObjectiveSound(ObjectiveSO.Type type)
        {
            if (type == ObjectiveSO.Type.Territory)
            {
                photonView.RPC(nameof(RPC_All_PlayTerritoryObjectiveSound), RpcTarget.All);
            }
            else if (type == ObjectiveSO.Type.Miniboss)
            {
                photonView.RPC(nameof(RPC_All_PlayMinibossObjectiveSound), RpcTarget.All);
            }
            else if (type == ObjectiveSO.Type.Boss)
            {
                photonView.RPC(nameof(RPC_All_PlayBossObjectiveSound), RpcTarget.All);
            }
        }

        [PunRPC]
        private void RPC_All_PlayTerritoryObjectiveSound()
        {
            _territoryObjectiveSound.Post(gameObject);
        }

        [PunRPC]
        private void RPC_All_PlayMinibossObjectiveSound()
        {
            _minibossObjectiveSound.Post(gameObject);
        }

        [PunRPC]
        private void RPC_All_PlayBossObjectiveSound()
        {
            _bossObjectiveSound.Post(gameObject);
        }
    }
}