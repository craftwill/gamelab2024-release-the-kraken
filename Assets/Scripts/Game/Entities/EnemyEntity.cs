using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

using Bytes;

namespace Kraken
{
    public class EnemyEntity : Entity
    {
        protected override void Awake()
        {
            base.Awake();
        }

        protected override void HandleTakeDamage(float dmgAmount)
        {
            base.HandleTakeDamage(dmgAmount);

            _entityAnimationComponent.PlayHurtAnim();
        }

        protected override void HandleDie()
        {
            base.HandleDie();

            photonView.RPC(nameof(RPC_All_Die), RpcTarget.All);
        }

        // send flying enemy with physics when it dies
        [PunRPC]
        private void RPC_All_Die() 
        {
            GetComponent<PhotonTransformView>().enabled = false;
            GetComponent<NavMeshAgent>().enabled = false;

            SphereCollider colAdded = gameObject.AddComponent<SphereCollider>();
            colAdded.radius = 0.2f;

            Rigidbody rgAdded = gameObject.AddComponent<Rigidbody>();
            Vector3 closestPlayerPos = GetClosestPlayer().Item1.transform.position;
            Vector3 dirToSend = -(closestPlayerPos - this.transform.position).normalized;
            Vector3 verticalForce = new Vector3(0f, Random.Range(1f, 10f), 0f);
            rgAdded.AddForce(dirToSend * 35f + verticalForce, ForceMode.Impulse);
            rgAdded.AddTorque(new Vector3(Random.Range(3f, 8f), Random.Range(3f, 8f), Random.Range(3f, 8f)), ForceMode.Impulse);

            if (!PhotonNetwork.IsMasterClient) return;

            Animate.Delay(1.5f, () => 
            {
                if (this == null) return;
                PhotonNetwork.Destroy(photonView);
            }, true);
        }
    }
}
