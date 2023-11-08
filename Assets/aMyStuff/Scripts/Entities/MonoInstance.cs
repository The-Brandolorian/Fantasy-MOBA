using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Allows to call coroutines from scriptable objects via this class.
public class MonoInstance : MonoBehaviour
{
    public static MonoInstance instance;

    private void Start()
    {
        MonoInstance.instance = this;
    }
}
