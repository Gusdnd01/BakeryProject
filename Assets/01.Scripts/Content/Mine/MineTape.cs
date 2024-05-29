using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineTape : MonoBehaviour
{
    private Animator _animator;


    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void LockTape(bool isLock)
    {
        _animator.SetBool("isUnLock", !isLock);
    }

}
