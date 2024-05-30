using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public partial class Zombie : LivingEntitiy
{
    public LayerMask whatIsTarget; // 추적 대상 레이어
    private LivingEntitiy targetEntity; // 추적 대상   
    public Animator animator;
    public float damage = 20f;
    public Slider healthSlider;    // 적 hp 슬라이더

    
    public bool outOfRange;        // 범위 밖으로 나갔을 때 딜레이
    private HandAttack handWeapon;  // 기본 손 무기


    public bool IsWalking
    {
        get => isWalking; 
        set => isWalking = value;
    }

    [SerializeField]
    private float radius = 5f;
    private float limitDistance = 0.5f;
    private float rotationSpeed = 100f;
    private float walkSpeed = 2f;
    

    //피격시 메테리얼 변경
    private List<Material> materialList;
    private List<Color> originColorList;
    private new Rigidbody rigidbody;
    private AudioSource audioSource;


    /*Zombie State Header 모아둠 나중에 사용자 프로퍼티로 묶자*/
    [SerializeField]
    [Header("-----Zombie State-----")]
    private bool hitting;           // 맞는 중인지 아닌지 판단
    [SerializeField]
    private bool isWalking;         // 걷는 중인지
    [SerializeField]
    private bool isCritical;        // 치명적인 상태라면
    [SerializeField]
    private bool isFalled;          // 자빠져있는 상태라면
    [SerializeField]
    private bool IsAttack;          // 공격 중이라면
    [SerializeField]
    private bool IsGuard;           // 방어 중이라면
    [SerializeField]
    private bool bEnable = false;   // 좀비 콤보공격 가능
    [SerializeField]
    private bool bExist = false;    // 좀비 다음 콤보공격 실행
    [SerializeField]
    private int defenseCount = 0;   // 방어 횟수
    [SerializeField]
    private int comboIndex;         // 현재 콤보 인덱스

    // 타격 대상 유무 true/false
    public bool hasTarget
    {
        get
        {
            if(targetEntity != null && !targetEntity.dead)
            {
                // 타겟이 있고 거리가 가까워지면 때릴 때 움직이면 안되므로
                if (Vector3.Distance(transform.position, targetEntity.transform.position) < limitDistance)
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
        audioSource = GetComponent<AudioSource>();
        handWeapon = GameObject.Find("AttackZone").GetComponent<HandAttack>();
        rigidbody = GetComponent<Rigidbody>();
        materialList = new List<Material>();
        originColorList = new List<Color>();
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach(Renderer renderer in renderers) 
        {
            materialList.Add(renderer.material);
            originColorList.Add(renderer.material.color);
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        healthSlider.gameObject.SetActive(true);
        healthSlider.maxValue = startingHealth;
        healthSlider.value = health;
    }

    private void Start()
    {
        StartCoroutine(UpdatePath());
        StartCoroutine(RandomPositionGenerator());
    }

    void Update()
    {
        /*if (GameManager.instance.isGameover)
            return;*/
        TrackPlayer();
        MoveToRandomPosition();
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
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius, whatIsTarget);
            if(colliders.Length == 0)
            {
                targetEntity = null;
                // 여기부터 작업, 외곽 바깥으로 플레이어 나가면 잠시 대기할 수 있도록 애니메이션 설정
                isWalking = false;
                animator.SetBool("IsWalking", false);
            }
            else
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    Player livingEntitiy = colliders[i].GetComponent<Player>();
                    if (livingEntitiy != null && !livingEntitiy.dead)
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
        if (IsGuard)
            return;

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
        // 회전 다 했으면 쫓아가기
        if (angleDifference < 5f)
        {
            transform.transform.position = Vector3.MoveTowards(pos, targetPos, walkSpeed * Time.deltaTime);
            IsWalking = true;
            animator.SetBool("IsWalking", IsWalking);
        }
    }

    // 몬스터가 피해를 입었다면
    public override void OnDamage(Vector3 hitPoint, Vector3 hitNormal, GameObject attacker, DoActionData data)
    {
        if (IsGuard)
        {
            data.StopFrame = 20;
            SucessGuard();
        }

        // Animation Delay Code..
        FrameComponent.Instance.Delay(data.StopFrame);

        if (data.HitParticle != null)
        {
            GameObject obj = Instantiate(data.HitParticle, transform, false);
            obj.transform.localPosition = data.HitParticlePositionOffset;
            obj.transform.localScale = data.HitParticleScaleOffset;
        }

        if (IsGuard)
            return;

        if (isCritical)
        {
            data.Power = ConstValue.FalledDamage;
        }

        // 데미지 처리
        base.OnDamage(hitPoint, hitNormal,attacker,data);
        healthSlider.value = health;

        foreach(Material material in materialList)
        {
            material.color = Color.red;
        }
        Invoke("RestoreColor", 0.5f);


        if (data.HitAudioClip != null)
            audioSource.PlayOneShot(data.HitAudioClip);

        if (!dead)
        {
            transform.LookAt(attacker.transform, Vector3.up);

            if (!isFalled)
            {
                if (isCritical)
                {
                    animator.SetTrigger("GuardDown");
                }
                else
                    animator.SetTrigger("Damaged");
            }
            hitting = true;
            End_Attack();                   // 공격 중에 맞을 수 있으니 false로 변경
            rigidbody.isKinematic = false;
            float launch = rigidbody.drag * data.Distance * 100.0f;
            Debug.Log("zombie.cs , 밀리는 힘 launch" + launch);
            rigidbody.AddForce(-transform.forward * launch *10);
            StartCoroutine(Change_IsKinemetics(50));


            return;
        }

        animator.SetTrigger("IsDead");
        Collider collider = GetComponent<Collider>();
        collider.enabled = false;
        Destroy(gameObject, 5.0f);
    }
    private void SucessGuard()
    {
        ++defenseCount;

        if(defenseCount < ConstValue.GuardBreakCnt)
            animator.SetTrigger("GuardHitting");
        else
        {
            defenseCount = 0;
            IsGuard = false;
            isCritical = true;
        }

        

    }
    /*애니메이션 event호출 메서드*/
    private void Begin_Falled()
    {
        isFalled = true;
        IsGuard = false;
    }
    private void End_Falled()
    {
        isFalled = false;
        isCritical = false;
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

    // 몬스터가 죽었다면
    public override void Die()
    {
        base.Die();

        Collider[] colliders = GetComponents<Collider>();
        for(int i=0; i< colliders.Length; i++) 
        {
            colliders[i].enabled = false;
        }

        /*삭제 방식*/
        //GameManager.instance.DeleteList(gameObject);
        GameManager.instance.DeleteCnt(1);
    }

    private void RestoreColor()
    {
        for (int i = 0; i < materialList.Count; i++)
            materialList[i].color = originColorList[i];
    }
    private IEnumerator Change_IsKinemetics(int frame)
    {
        for(int i=0; i<frame;++i)
            yield return new WaitForFixedUpdate();

        rigidbody.isKinematic = true;
    }


    private void OnDrawGizmos()
    {
        if (Selection.activeGameObject != gameObject)
            return;

        /*적 반경을 탐지하는 구 Draw*/
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
        /*이동할 좌표 경로를 그리는 선 Draw*/
        Gizmos.color = Color.red;
        Debug.DrawLine(transform.position, destPos, Color.red);
    }
}