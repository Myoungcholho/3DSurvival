using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Zombie
{
    private bool IsAttack;
    public int comboIndex;
    // ���� ������ ���Դٸ�
    // Player�� BoxCollider �� ���� üũ ��
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == this)
            return;


        if(!dead && !IsAttack)
        {
            LivingEntitiy attackTarget = other.GetComponent<LivingEntitiy>();

            if (IsAttack)
                return;

            // Ÿ���� �ְ�, ��ǥ�ϴ� Ÿ���� �´ٸ� ����
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
        comboIndex = 0;
    }

}
