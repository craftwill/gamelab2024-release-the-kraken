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

            float woolQuantityPercentage = _woolQuantity / Config.current.maxWoolQuantity;

            // If wool quantity > 50%, show cast totem animation
            bool showTotemAnim = woolQuantityPercentage > 0.5f;
            EventManager.Dispatch(EventNames.UpdatePlayerTotemAbilityUI, new BoolDataBytes(showTotemAnim));

            // If wool at 100%, show cast ultimate animation
            bool showUltAnim = woolQuantityPercentage >= 1f;
            EventManager.Dispatch(EventNames.UpdateUltimateUI, new BoolDataBytes(showUltAnim));

            // Update wool gauge
            EventManager.Dispatch(EventNames.UpdateWoolQuantity, new IntDataBytes(_woolQuantity));
        }
    }
}