using Bytes;
using Kraken.Game;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class PlayerHealingComponent : MonoBehaviourPun
    {
        [SerializeField] private PlayerControlsComponent _playerControls;
        [SerializeField] private PlayerSoundComponent _soundComponent;
        private GameObject[] _players = { };
        private HealthComponent _componentToHeal;
        private LilWoolManager _lilWoolManager;
        private bool _isHealing = false;

        private void Start()
        {
            _lilWoolManager = Object.FindObjectOfType<LilWoolManager>();
        }

        private void Update()
        {
            if (_players.Length < 2)
            {
                _players = GameObject.FindGameObjectsWithTag("Player");
            }
            else if (_componentToHeal == null)
            {
                // Get other player's health component
                if (GameObject.ReferenceEquals(gameObject, _players[0]))
                {
                    _componentToHeal = _players[1].GetComponent<HealthComponent>();
                }
                else
                {
                    _componentToHeal = _players[0].GetComponent<HealthComponent>();                    
                }
            }

            if (_isHealing)
            {
                transform.LookAt(_componentToHeal.transform);
                if (GetDistanceBetweenPlayers() > Config.current.healingMaxDistance || _componentToHeal.Health == _componentToHeal.MaxHealth)
                {
                    _isHealing = false;
                }
            }
        }

        public void OnHealingInput(bool pressed)
        {

            if (pressed)
            {
                if (GetDistanceBetweenPlayers() <= Config.current.healingMaxDistance && _componentToHeal.Health < _componentToHeal.MaxHealth && _lilWoolManager._woolQuantity > 0)
                {
                    _isHealing = true;
                    StartCoroutine(HealCoroutine());
                }
            }
            else
            {
                _isHealing = false;
            }
        }

        private IEnumerator HealCoroutine()
        {
            int healingDone = 0;
            _playerControls.SetControlsEnabled(false);
            photonView.RPC(nameof(_soundComponent.RPC_All_PlayHealingSound), RpcTarget.All);
            while (_isHealing)
            {
                if (healingDone++ % Config.current.healingHpPerWool == 0)
                {
                    if (_lilWoolManager._woolQuantity == 0)
                    {
                        _isHealing = false;
                    }
                    photonView.RPC(nameof(RPC_Master_UseWool), RpcTarget.MasterClient);
                }
                _componentToHeal?.GetHealed(1f);
                yield return new WaitForSeconds(Config.current.healingRate);
            }
            _playerControls.SetControlsEnabled(true);
            photonView.RPC(nameof(_soundComponent.RPC_All_StopHealingSound), RpcTarget.All);
        }

        [PunRPC]
        private void RPC_Master_UseWool()
        {
            EventManager.Dispatch(EventNames.GainWool, new IntDataBytes(-1));
        }

        private float GetDistanceBetweenPlayers()
        {
            if (_players.Length == 2)
            {
                return Vector3.Distance(_players[0].transform.position, _players[1].transform.position);
            }
            else
            {
                return float.MaxValue;
            }
        }
    }
}
