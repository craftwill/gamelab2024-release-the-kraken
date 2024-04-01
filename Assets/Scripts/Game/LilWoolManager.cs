using Bytes;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class LilWoolManager : KrakenNetworkedManager
    {
        public int _woolQuantity { get; private set; } = 0;
        private void Start()
        {
            if (!_isMaster) return;

            _woolQuantity = Config.current.initialWoolQuantity;

            EventManager.AddEventListener(EventNames.GainWool, GainWool);
        }

        private void OnDestroy()
        {
            EventManager.RemoveEventListener(EventNames.GainWool, GainWool);
        }

        private void GainWool(BytesData data)
        {
            int quantityToAdd = ((IntDataBytes)data).IntValue;
            photonView.RPC(nameof(RPC_All_GainWool), RpcTarget.All, quantityToAdd);

        }

        [PunRPC]
        private void RPC_All_GainWool(int quantity)
        {
            _woolQuantity += quantity;
            OnQuantityUpdate();
        }

        private void OnQuantityUpdate()
        {
            if (_woolQuantity <= 0)
            {
                _woolQuantity = 0;
            }
            else if (_woolQuantity > Config.current.maxWoolQuantity)
            {
                _woolQuantity = Config.current.maxWoolQuantity;
            }
            EventManager.Dispatch(EventNames.UpdateWoolQuantity, new IntDataBytes(_woolQuantity));
        }
    }
}