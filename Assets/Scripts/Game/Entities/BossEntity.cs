using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class BossEntity : EnemyEntity
    {
        public override void TakeDamage(float dmgAmount)
        {
            if (_healthComponent.Health - dmgAmount <= 0f)
            {
                Debug.Log("Must kill with ultimate");
            }
            else
            {
                base.TakeDamage(dmgAmount);
            }
        }
        public void TakeUltimateDamage(float dmgAmount)
        {
            _healthComponent.TakeDamage(dmgAmount);
        }
        
    }
}