using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using System;
using Bytes;

namespace Kraken
{
    [CreateAssetMenu(fileName = "Attack", menuName = "Kraken/Create Attack")]
    public class AttackSO : ScriptableObject
    {
        [Tooltip("Name of the attack, currently only for debugging purposes")]
        public string attackName;
        [Tooltip("Next attack if you combo")]
        public AttackSO nextAttack;
        [Tooltip("The hitbox of the attack, insert a gameobject with a collider component and the \"DealDamage\" tag, see Assets/Prefab/Collider for an example")]
        public GameObject colliderGameObject;
        [Range(0.0f, 1.0f), Tooltip("How long until the hitbox comes out")]
        public float timeBeforeHitboxDuration;
        [Range(0.0f, 1.0f), Tooltip("How long does the hitbox stays out")]
        public float hitboxDuration;
        [Range(0.0f, 10.0f), Tooltip("How long the until you can attack again")]
        public float attackLockLength;
        [Range(0.0f, 10.0f), Tooltip("How long after the animation ended can you still queue the next attack")]
        public float animDoneBufferTimer;
        public int damage;
        public int comboStep = 1;
    }
}