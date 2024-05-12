using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : LivingEntitiy
{
    public LayerMask whatIsTarget; // 추적 대상 레이어
    private LivingEntitiy targetEntity; // 추적 대상   
    private Animator animator;
    public float damage = 20f;
    public float timeBetAttack = 0.5f;
    private float lastAttackTime;
    private bool hitting;           // 맞는 중인지 아닌지 판단
    public bool outOfRange;        // 범위 밖으로 나갔을 때 딜레이


    public bool IsWalking
    {
        get => isWalking; 
        set => isWalking = value;
    }
    private bool isWalking;

    [SerializeField]
    private float radius = 5f;

    private float rotationSpeed = 50f;
    private float walkSpeed = 2f;

    // 타격 대상 유무 true/false
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
    }

    private void Start()
    {
        health = startingHealth;
        StartCoroutine(UpdatePath());
    }

    void Update()
    {
        trackPlayer();

        animator.SetBool("IsWalking", IsWalking);
    }

    // 딜레이 5초
    private IEnumerator ChangeBoolAfterDelay()
    {
        outOfRange = true;
        IsWalking = false;
        yield return new WaitForSeconds(5f);
        outOfRange = false;
        IsWalking = true;
    }


    // 타겟이 없다면 타겟을 찾아 등록
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
            yield return new WaitForSeconds(0.25f);
        }
    }

    private void trackPlayer()
    {
        if (dead)
            return;

        if (!hasTarget)
            return;

        if (hitting)
            return;

        Vector3 targetPos = targetEntity.transform.position;
        Vector3 pos = transform.position;
        Vector3 direction = targetPos - pos;

        Quaternion angle = Quaternion.LookRotation(direction.normalized, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, angle, rotationSpeed *Time.deltaTime);

        float angleDifference = Quaternion.Angle(transform.rotation, angle);
        // 회전 다 했으면 쫓아가기
        if (angleDifference < 5f)
        {
            transform.transform.position = Vector3.MoveTowards(pos, targetPos, walkSpeed * Time.deltaTime);
            IsWalking = true;
        }
    }

    // 몬스터가 피해를 입었다면
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

    // 몬스터가 죽었다면
    public override void Die()
    {
        base.Die();

        Collider[] colliders = GetComponents<Collider>();
        for(int i=0; i< colliders.Length; i++) 
        {
            colliders[i].enabled = false;
        }
    }

    // 공격 범위에 들어왔다면
    private void OnTriggerStay(Collider other)
    {
        /*if(!dead && Time.time >= lastAttackTime + timeBetAttack)
        {
            LivingEntitiy attackTarget = other.GetComponent<LivingEntitiy>();

            if(attackTarget != null && attackTarget == targetEntity)
            {
                lastAttackTime = Time.time;
                Vector3 hitPoint = other.ClosestPoint(transform.position);
                Vector3 hitNoral = transform.position - other.transform.position;
                attackTarget.OnDamage(damage, hitPoint, hitNoral);
            }
        }*/
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
