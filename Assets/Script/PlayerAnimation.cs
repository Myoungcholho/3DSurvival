using UnityEngine;

public partial class Player
{
    // �� ���� ���� �ڵ�
    #region Drawing
    private bool bDrawing = false;      // ���� �̴� ������.
    private bool bSheathing = false;    // ���� �ִ� ������.
    private bool bEquipped = false;     // ���� ��� �ִ� ��������.

    private void UpdateDrawing()
    {
        if (Input.GetButtonDown("Sword") == false)
            return;

        if (bDrawing == true)
            return;

        if (bSheathing == true)
            return;

        if (bGuarding)
            return;

        if (bEquipped == false)
        {
            bDrawing = true;
            animator.SetBool("IsEquip", bDrawing);

            return;
        }

        bSheathing = true;
        animator.SetBool("IsUnequip", bSheathing);
    }

    // �ִϸ��̼ǿ��� ���� �㸮�㿡 �����ٴ�� ȣ���.
    private void Begin_Equip()
    {
        swordDestination.transform.parent.DetachChildren();

        swordDestination.transform.position = Vector3.zero;
        swordDestination.transform.rotation = Quaternion.identity;
        swordDestination.transform.localScale = Vector3.one;

        swordDestination.transform.SetParent(handTransform, false);
    }

    private void End_Equip()
    {
        bEquipped = true; // ���� �����ߴ���
        animator.SetBool("bEquipped", bEquipped);

        bDrawing = false;
        animator.SetBool("IsEquip", false);
    }

    private void Begin_Unequip()
    {
        swordDestination.transform.parent.DetachChildren();
        swordDestination.transform.position = Vector3.zero;
        swordDestination.transform.rotation = Quaternion.identity;
        swordDestination.transform.localScale = Vector3.one;

        swordDestination.transform.SetParent(holsterTransform, false);
    }

    private void End_Unequip()
    {
        bEquipped = false;
        animator.SetBool("bEquipped", bEquipped);

        bSheathing = false;
        animator.SetBool("IsUnequip", false);
    }
    #endregion

    // �����ϴ� �ڵ�
    #region Attacking
    private bool bAttacking = false;
    private bool bEnable = false;
    private bool bExist = false;
    public int comboIndex;

    private void UpdateAttacking()
    {
        if (Input.GetButtonDown("Attack") == false)
            return;

        if (bDrawing || bSheathing)
            return;

        if (bGuarding)
            return;

        if (bEquipped == false)
            return;

        if(bEnable)
        {
            bExist = true;
            bEnable = false;
            return;
        }

        if (bAttacking == true)
            return;

        bAttacking = true;
        animator.SetBool("IsAttacking", true);
    }

    // �޺� ������ �̾
    private void Begin_Attack()
    {
        if (!bExist)
            return;

        comboIndex++;
        bExist = false;
        animator.SetTrigger("NextCombo");
    }

    // ������ ������ �� �ٽ� ���� �����ϰ� �ϱ� ���� ȣ��
    private void End_Attack()
    {
        bAttacking = false;
        animator.SetBool("IsAttacking", false);
        comboIndex = 0;
    }

    private void Begin_Combo()
    {
        bEnable = true;
    }


    private void End_Combo()
    {
        bEnable = false;
    }

    // �����ϸ� ��
    private void Begin_Collision()
    {
        sword.Begin_Collision();
    }

    // ������ ������ ��
    private void End_Collision()
    {
        sword.End_Collision();
    }
    #endregion

    // ���� �ڵ�
    #region Guard
    private bool bGuarding = false;
    private bool bBlocking = false;

    private void UpdateGuarding()
    {
        if (!Input.GetButton("Guard"))
        {
            bGuarding = false;
            animator.SetBool("bGuarding", bGuarding);
            return;
        }

        if (bHitting || bAttacking)
            return;

        // ���� ��� �ִ� ���¿����� �ǵ� ����
        if (!bEquipped)
            return;


        bGuarding = true;
        animator.SetBool("bGuarding", bGuarding);
    }

    private void Begin_Guard()
    {
        bBlocking = true;
    }

    private void End_Guard()
    {
        bBlocking = false;
    }

    #endregion
}