using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntitiy : MonoBehaviour, IDamageable
{
    public float startingHealth = 100f;         //���� HP
    public float health { get; private set; }   //���� HP
    public bool dead { get; protected set; }    //�������
    public event Action onDeadth;               //�׾��� �� ������ ��������Ʈ

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
