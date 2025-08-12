using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackBase : MonoBehaviour
{
    //public Sprite bulletSprite;
    [SerializeField]
    int hp;
    [SerializeField]
    float cooldown;
    [SerializeField]
    bool parryable;

    protected abstract void StartAttack();

    private void Start()
    {
        StartAttack();
    }
}

