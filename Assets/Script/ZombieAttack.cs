using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Zombie
{
    public int ComboIndex
    {
        set => comboIndex = value;
        get => comboIndex;
    }
    
    private enum AttackPattern
    {
        Attack =0,
        Guard = 1,
    }

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
        if (isFalled)
            return;
        if (isCritical2)
            return;

        /*여기서 공격할지 방어할지 택*/
        AttackPattern randomState = (AttackPattern)Random.Range(0, 2);

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
        //Debug.Log("ZombieGuard()" + Time.time);
    }

    /* animation clip event call*/
    private void End_Guard()
    {
        IsGuard = false;
        //Debug.Log("End_Guard() :" + Time.time);
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
