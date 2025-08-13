using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackBase : MonoBehaviour
{
    //public Sprite bulletSprite;
    [SerializeField]
    public int hp;
    [SerializeField]
    public float cooldown;
    [SerializeField]
    public bool parryable;

    protected abstract void StartAttack();

    private void Start()
    {
        StartAttack();
    }
}

