using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public partial class Zombie : LivingEntitiy
{
    public LayerMask whatIsTarget; // ���� ��� ���̾�
    private LivingEntitiy targetEntity; // ���� ���   
    public Animator animator;
    public float damage = 20f;

    private bool hitting;           // �´� ������ �ƴ��� �Ǵ�
    public bool outOfRange;        // ���� ������ ������ �� ������
    private HandAttack handWeapon;  // �⺻ �� ����

    public bool IsWalking
    {
        get => isWalking; 
        set => isWalking = value;
    }
    private bool isWalking;

    [SerializeField]
    private float radius = 5f;

    private float rotationSpeed = 100f;
    private float walkSpeed = 2f;

    // Ÿ�� ��� ���� true/false
    public bool hasTarget
    {
        get
        {
            if(targetEntity != null && !targetEntity.dead)
            {
                if (Vector3.Distance(transform.position, targetEntity.transform.position) > radius)
                {
                    StopCoroutine(ChangeBoolAfterDelay());
                    StartCoroutine(ChangeBoolAfterDelay());
                    return false;
                }
                else
                    return true;
            }
            return false;
        }
    }
    private void Awake()
    {
        animator = GetComponent<Animator>();
        handWeapon = GameObject.Find("AttackZone").GetComponent<HandAttack>();
    }

    private void Start()
    {
        health = startingHealth;
        StartCoroutine(UpdatePath());
    }

    void Update()
    {
        if (GameManager.instance.isGameover)
            return;

        TrackPlayer();
    }

    // ������ 5��
    private IEnumerator ChangeBoolAfterDelay()
    {
        outOfRange = true;
        IsWalking = false;
        yield return new WaitForSeconds(5f);
        outOfRange = false;
        IsWalking = true;
    }


    // Ÿ���� ���ٸ� Ÿ���� ã�� ���
    private IEnumerator UpdatePath()
    {
        while(!dead)
        {
            if(!hasTarget)
            {
                targetEntity = null;
                Collider[] colliders = Physics.OverlapSphere(transform.position, radius, whatIsTarget);
                for(int i=0; i<colliders.Length; i++) 
                {
                    LivingEntitiy livingEntitiy = colliders[i].GetComponent<LivingEntitiy>();
                    if(livingEntitiy != null && !livingEntitiy.dead)
                    {
                        targetEntity = livingEntitiy;

                        break;
                    }
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }

    private void TrackPlayer()
    {
        if (dead)
            return;

        if (!hasTarget)
            return;

        if (hitting)
            return;

        if (IsAttack)
            return;

        Vector3 targetPos = targetEntity.transform.position;
        Vector3 pos = transform.position;
        Vector3 direction = targetPos - pos;

        Quaternion angle = Quaternion.LookRotation(direction.normalized, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, angle, rotationSpeed *Time.deltaTime);

        float angleDifference = Quaternion.Angle(transform.rotation, angle);
        // ȸ�� �� ������ �Ѿư���
        if (angleDifference < 5f)
        {
            transform.transform.position = Vector3.MoveTowards(pos, targetPos, walkSpeed * Time.deltaTime);
            IsWalking = true;
            animator.SetBool("IsWalking", IsWalking);
        }
    }

    // ���Ͱ� ���ظ� �Ծ��ٸ�
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        base.OnDamage(damage, hitPoint, hitNormal);
        if(!dead)
        {
            animator.SetTrigger("Damaged");
            hitting = true;
            return;
        }
        animator.SetTrigger("IsDead");
        Collider collider = GetComponent<Collider>();
        collider.enabled = false;
        Destroy(gameObject, 5.0f);
    }

    private void End_Hit()
    {
        hitting = false;
    }

    private void Begin_Collision()
    {
        handWeapon.Begin_Collision();
    }
    private void End_Collision()
    {
        handWeapon.End_Collision();
    }


    // ���Ͱ� �׾��ٸ�
    public override void Die()
    {
        base.Die();

        Collider[] colliders = GetComponents<Collider>();
        for(int i=0; i< colliders.Length; i++) 
        {
            colliders[i].enabled = false;
        }
    }



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}