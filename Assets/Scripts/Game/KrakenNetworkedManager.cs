
using Photon.Pun;

namespace Kraken
{
    public class KrakenNetworkedManager : MonoBehaviourPun
    {
        protected bool _isMaster;

        protected virtual void Awake()
        {
            _isMaster = PhotonNetwork.IsMasterClient;
        }
    }
}
