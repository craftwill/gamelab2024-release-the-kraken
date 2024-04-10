using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitedLifetimeComponent : MonoBehaviour
{
    [SerializeField] private float _lifetime;

    private void Start()
    {
        StartCoroutine(LifetimeCoroutine());
    }

    private IEnumerator LifetimeCoroutine()
    {
        yield return new WaitForSeconds(_lifetime);
        Destroy(gameObject);
    }
}
