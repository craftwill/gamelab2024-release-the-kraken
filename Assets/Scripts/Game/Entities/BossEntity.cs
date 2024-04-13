using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kraken
{
    public class BossEntity : EnemyEntity
    {
        public override void TakeDamage(float dmgAmount)
        {
            if (Config.current.mustKillWithUltimate && _healthComponent.Health - dmgAmount <= 0f)
            {
                Debug.Log("Must kill with ultimate");
            }
            else
            {
                base.TakeDamage(dmgAmount);
            }
        }
    }
}