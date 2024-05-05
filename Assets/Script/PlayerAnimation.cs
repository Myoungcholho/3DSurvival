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

    //
    private void End_Equip()
    {
        bEquipped = true; // ���� �����ߴ���

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

        bSheathing = false;
        animator.SetBool("IsUnequip", false);
    }
    #endregion

    // �����ϴ� �ڵ�
    #region Attacking
    private bool bAttacking = false;

    private void UpdateAttacking()
    {
        if (Input.GetButtonDown("Attack") == false)
            return;

        if (bDrawing || bSheathing)
            return;

        if (bEquipped == false)
            return;

        if (bAttacking == true)
            return;


        bAttacking = true;
        animator.SetBool("IsAttacking", true);
        //animator.SetTrigger("Attacking");
    }

    // ������ ������ �� �ٽ� ���� �����ϰ� �ϱ� ���� ȣ��
    private void End_Attack()
    {
        bAttacking = false;
        animator.SetBool("IsAttacking", false);
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
}