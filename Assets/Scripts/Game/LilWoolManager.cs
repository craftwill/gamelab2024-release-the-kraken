using Bytes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class LilWoolManager : KrakenNetworkedManager
    {
        private int _woolQuantity = 0;
        private void Start()
        {
            if (!_isMaster) return;

            EventManager.AddEventListener(EventNames.GainWool, GainWool);
            _woolQuantity = 0;
        }

        private void OnDestroy()
        {
            EventManager.RemoveEventListener(EventNames.GainWool, GainWool);
        }

        private void GainWool(BytesData data)
        {
            int quantityToAdd = ((IntDataBytes)data).IntValue;
            _woolQuantity += quantityToAdd;
            OnQuantityUpdate();
        }

        private void OnQuantityUpdate()
        {
            EventManager.Dispatch(EventNames.UpdateWoolQuantity, new IntDataBytes(_woolQuantity));
            if ( _woolQuantity <= 0 )
            {
                _woolQuantity = 0;
            }
        }
    }

}