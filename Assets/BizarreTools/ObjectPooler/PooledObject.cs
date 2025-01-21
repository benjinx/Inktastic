using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PooledObject : MonoBehaviour
{
    public UnityEvent onObjSpawned;

    public void OnObjectSpawned()
    {
        onObjSpawned?.Invoke();
    }
}
