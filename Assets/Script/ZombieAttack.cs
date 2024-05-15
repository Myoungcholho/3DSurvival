using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Zombie
{
    private bool IsAttack;

    // 공격 범위에 들어왔다면
    // Player의 BoxCollider 만 지금 체크 중
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == this)
            return;

        if(!dead && !IsAttack)
        {
            LivingEntitiy attackTarget = other.GetComponent<LivingEntitiy>();

            // 타겟이 있고, 목표하는 타겟이 맞다면 공격
            if(attackTarget != null && attackTarget == targetEntity)
            {
                animator.SetTrigger("Attacking");
                IsAttack = true;
                IsWalking = false;
            }
        }
    }

    private void End_Attack()
    {
        IsAttack = false;
    }

}
