using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Zombie
{
    public int ComboIndex
    {
        get => comboIndex;
    }
    
    private enum AttackPattern
    {
        Attack =0,
        Guard = 1,
    }


    // 공격 범위에 들어왔다면
    // Player의 BoxCollider 만 지금 체크 중
    private float patternTime;
    private float patternDelay =1f;
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == this)
            return;
        if (dead)
            return;
        if (IsGuard)
            return;

        if(bEnable)
        {
            bEnable = false;
            bExist = true;
            return;
        }

        if (IsAttack)
            return;

/*        if (!(Time.time >= patternDelay + patternTime))
            return;*/

        /*여기서 공격할지 방어할지 택*/
        AttackPattern randomState = (AttackPattern)Random.Range(1, 2);
        //patternTime = Time.time;

        switch (randomState)
        {
            case AttackPattern.Attack:
            {
                ZombieAttack(other);
            }
            break;
            case AttackPattern.Guard:
            {
                ZombieGuard();
            }
            break;
        }
    }

    private void ZombieAttack(Collider other)
    {
        LivingEntitiy attackTarget = other.GetComponent<LivingEntitiy>();
        // 타겟이 있고, 목표하는 타겟이 맞다면 공격
        if (attackTarget != null && attackTarget == targetEntity)
        {
            animator.SetTrigger("Attacking");
            IsAttack = true;

            IsWalking = false;
        }
    }
    private void ZombieGuard()
    {
        IsGuard = true;
        animator.SetBool("IsGuard", IsGuard);
    }

    /* animation clip event call*/
    private void End_Guard()
    {
        IsGuard = false;
        defenseCount = 0;
        animator.SetBool("IsGuard", IsGuard);
    }

    private void Next_Combo()
    {
        if (!bExist)
            return;

        bExist = false;
        animator.SetTrigger("NextCombo");
        ++comboIndex;
    }
    private void Begin_Enable()
    {
        bEnable = true;
    }
    private void End_Enable()
    {
        bEnable = false;
    }
    private void End_Attack()
    {
        IsAttack = false;
        comboIndex = 0;
    }

}
