using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonDontDestroy : MonoBehaviour
{
    private static SingletonDontDestroy thisInstance;

    private void Awake()
    {
        if(thisInstance != null)
        {
            Destroy(gameObject);
            return;
        }

        thisInstance = this;
        DontDestroyOnLoad(this);
    }
}
