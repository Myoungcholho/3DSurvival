using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntitiy : MonoBehaviour, IDamageable
{
    public float startingHealth = 100f;         //시작 HP
    public float health { get; protected set; }   //현재 HP
    public bool dead { get; protected set; }    //사망유무
    public event Action onDeadth;               //죽었을 때 실행할 델리게이트

    protected virtual void OnEnable()
    {
        health = startingHealth;
    }

    // 데미지 입을 시 처리
    public virtual void OnDamage(Vector3 hitPoint, Vector3 hitNormal, GameObject attacker, DoActionData doActionData)
    {
        health += doActionData.Power * -1.0f;

        #if MY_DEBUG
        Debug.Log("오브젝트 : " + gameObject.name +"남은 체력 : " + health);
        #endif

        if (health <= 0)
            Die();
    }

    // 회복 처리
    public virtual void RestoreHealth(float newHealth)
    {

    }

    // 사망 처리
    public virtual void Die()
    {
        if(onDeadth != null)
        {
            onDeadth();
        }
        dead = true;
    }
}
