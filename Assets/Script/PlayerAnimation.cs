using UnityEngine;

public partial class Player
{
    // 검 장착 해제 코드
    #region Drawing
    private bool bDrawing = false;      // 검을 뽑는 중인지.
    private bool bSheathing = false;    // 검을 넣는 중인지.
    private bool bEquipped = false;     // 검을 들고 있는 상태인지.

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

    // 애니메이션에서 손을 허리춤에 가져다댈대 호출됨.
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
        bEquipped = true; // 검을 장착했는지
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

    // 공격하는 코드
    #region Attacking
    [SerializeField]
    [Header("-----Player State-----")]
    private bool bAttacking = false;
    [SerializeField]
    private bool bEnable = false;
    [SerializeField]
    private bool bExist = false;
    [SerializeField]
    private bool bParryExist = false;           // 패링 중 순간에 잠깐 켜지고 꺼질 것임
    [SerializeField]
    private bool bParring = false;
    public int comboIndex;

    private void UpdateAttacking()
    {
        if (Input.GetButtonDown("Attack") == false)
            return;

        if (bDrawing || bSheathing)
            return;

        if (bParring)
            return;

        if (bEquipped == false)
            return;

        if (bGuarding)
        {
            GuardAttack();
            return;
        }


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

    private void GuardAttack()
    {
        animator.SetTrigger("Parring");
    }

    // 방패 패링 애니메이션 시작 시 호출
    private void Begin_Parring()
    {
        bParring = true;
        bBlocking = false;
    }
    private void End_Parring()
    {
        bParring = false;
        bBlocking = true;
    }
    private void Begin_GuardAttack()
    {
        // bool 값만 변경해두고
        // 데미지 들어올 때 만약 여기가 true였다면
        // 좀비에게 기절모션이 걸릴 수 있도록하자.
        bParryExist = true;
    }
    private void End_GuardAttack() 
    {
        animator.ResetTrigger("Parring");
        bParryExist = false;
    }

    // 콤보 공격을 이어감
    private void Begin_Attack()
    {
        if (!bExist)
            return;

        comboIndex++;
        bExist = false;
        animator.SetTrigger("NextCombo");
    }

    // 공격이 끝났을 때 다시 공격 가능하게 하기 위해 호출
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

    // 공격하면 켜
    private void Begin_Collision()
    {
        sword.Begin_Collision();
    }

    // 공격이 끝나면 꺼
    private void End_Collision()
    {
        sword.End_Collision();
    }
    #endregion

    // 가드 코드
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

        // 검을 들고 있는 상태에서만 실드 가능
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