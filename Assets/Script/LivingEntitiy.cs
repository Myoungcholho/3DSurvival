using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntitiy : MonoBehaviour, IDamageable
{
    public float startingHealth = 100f;         //���� HP
    public float health { get; protected set; }   //���� HP
    public bool dead { get; protected set; }    //�������
    public event Action onDeadth;               //�׾��� �� ������ ��������Ʈ

    protected virtual void OnEnable()
    {
        health = startingHealth;
    }

    // ������ ���� �� ó��
    public virtual void OnDamage(Vector3 hitPoint, Vector3 hitNormal, GameObject attacker, DoActionData doActionData)
    {
        health += doActionData.Power * -1.0f;

        #if MY_DEBUG
        Debug.Log("������Ʈ : " + gameObject.name +"���� ü�� : " + health);
        #endif

        if (health <= 0)
            Die();
    }

    // ȸ�� ó��
    public virtual void RestoreHealth(float newHealth)
    {

    }

    // ��� ó��
    public virtual void Die()
    {
        if(onDeadth != null)
        {
            onDeadth();
        }
        dead = true;
    }
}
