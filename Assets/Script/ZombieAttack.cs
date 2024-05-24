using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Zombie
{
    private bool IsAttack;
    //���� ���� �޺�
    private int comboIndex;
    public int ComboIndex
    {
        get => comboIndex;
    }
    private bool bEnable = false;
    private bool bExist = false;

    // ���� ������ ���Դٸ�
    // Player�� BoxCollider �� ���� üũ ��
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == this)
            return;
        if (dead)
            return;

        if(bEnable)
        {
            bEnable = false;
            bExist = true;
            return;
        }

        if (IsAttack)
            return;


        LivingEntitiy attackTarget = other.GetComponent<LivingEntitiy>();
        // Ÿ���� �ְ�, ��ǥ�ϴ� Ÿ���� �´ٸ� ����
        if (attackTarget != null && attackTarget == targetEntity)
        {
            animator.SetTrigger("Attacking");
            IsAttack = true;

            IsWalking = false;
        }


    }

    /* animation clip event call*/
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
