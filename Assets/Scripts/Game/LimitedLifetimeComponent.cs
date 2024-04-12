using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitedLifetimeComponent : MonoBehaviour
{
    [SerializeField] private float _lifetime;

    private void Start()
    {
        if (!Mathf.Approximately(_lifetime, 0f))
            StartCoroutine(LifetimeCoroutine());

    }

    public void StartNewLifeTime(float lifetime)
    {
        _lifetime = lifetime;
        StartCoroutine(LifetimeCoroutine());
    }

    private IEnumerator LifetimeCoroutine()
    {
        yield return new WaitForSeconds(_lifetime);
        Destroy(gameObject);
    }
}
