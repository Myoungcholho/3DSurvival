using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : LivingEntitiy
{
    public LayerMask whatIsTarget; // 추적 대상 레이어
    private LivingEntitiy targetEntity; // 추적 대상
    private NavMeshAgent navMeshAgent;    
    private Animator animator;
    public float damage = 20f;
    public float timeBetAttack = 0.5f;
    private float lastAttackTime;

    public bool IsWalking
    {
        get => isWalking; 
        set => isWalking = value;
    }
    private bool isWalking;

    [SerializeField]
    private float radius = 5f;

    // 타격 대상 유무 true/false
    public bool hasTarget
    {
        get
        {
            if(targetEntity != null && !targetEntity.dead)
            {
                return true;
            }
            return false;
        }
    }
    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        StartCoroutine(UpdatePath());
    }

    void Update()
    {
        animator.SetBool("HasTarget", IsWalking);
    }

    // Nav Update
    private IEnumerator UpdatePath()
    {
        while(!dead)
        {
            if(hasTarget)
            {
                navMeshAgent.isStopped = false;
                // 목적지 셋팅, 적의 위치로
                navMeshAgent.SetDestination(targetEntity.transform.position);
            }
            else
            {
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

    // 몬스터가 피해를 입었다면
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if(!dead)
        {

        }

        base.OnDamage(damage, hitPoint, hitNormal);
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

        navMeshAgent.isStopped = true;
        navMeshAgent.enabled = false;
    }

    // 공격 범위에 들어왔다면
    private void OnTriggerStay(Collider other)
    {
        if(!dead && Time.time >= lastAttackTime + timeBetAttack)
        {
            LivingEntitiy attackTarget = other.GetComponent<LivingEntitiy>();

            if(attackTarget != null && attackTarget == targetEntity)
            {
                lastAttackTime = Time.time;
                Vector3 hitPoint = other.ClosestPoint(transform.position);
                Vector3 hitNoral = transform.position - other.transform.position;
                attackTarget.OnDamage(damage, hitPoint, hitNoral);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
