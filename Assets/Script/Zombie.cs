using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : LivingEntitiy
{
    public LayerMask whatIsTarget; // ���� ��� ���̾�
    private LivingEntitiy targetEntity; // ���� ���
    private NavMeshAgent navMeshAgent;    
    private Animator animator;
    public float damage = 20f;
    public float timeBetAttack = 0.5f;
    private float lastAttackTime;

    
    private bool hasTarget
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
        animator.SetBool("HasTarget", hasTarget);
        if(targetEntity != null ) 
        {
            Debug.Log("���� ��� :" + targetEntity.name);
        }
    }

    private IEnumerator UpdatePath()
    {
        while(!dead)
        {
            if(hasTarget)
            {
                navMeshAgent.isStopped = false;
                navMeshAgent.SetDestination(targetEntity.transform.position);
            }
            else
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, 20f, whatIsTarget);
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

    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if(!dead)
        {

        }

        base.OnDamage(damage, hitPoint, hitNormal);
    }

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
}
