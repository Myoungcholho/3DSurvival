using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntitiy : MonoBehaviour, IDamageable
{
    public float startingHealth = 100f;         //시작 HP
    public float health { get; private set; }   //현재 HP
    public bool dead { get; protected set; }    //사망유무
    public event Action onDeadth;               //죽었을 때 실행할 델리게이트

    public virtual void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        
    }

    public virtual void RestoreHealth(float newHealth)
    {

    }

    public virtual void Die()
    {
        if(onDeadth != null)
        {
            onDeadth();
        }
        dead = true;
    }
}
