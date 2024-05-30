using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public partial class Zombie : LivingEntitiy
{
    public LayerMask whatIsTarget; // ���� ��� ���̾�
    private LivingEntitiy targetEntity; // ���� ���   
    public Animator animator;
    public float damage = 20f;
    public Slider healthSlider;    // �� hp �����̴�

    
    public bool outOfRange;        // ���� ������ ������ �� ������
    private HandAttack handWeapon;  // �⺻ �� ����


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
    

    //�ǰݽ� ���׸��� ����
    private List<Material> materialList;
    private List<Color> originColorList;
    private new Rigidbody rigidbody;
    private AudioSource audioSource;


    /*Zombie State Header ��Ƶ� ���߿� ����� ������Ƽ�� ����*/
    [SerializeField]
    [Header("-----Zombie State-----")]
    private bool hitting;           // �´� ������ �ƴ��� �Ǵ�
    [SerializeField]
    private bool isWalking;         // �ȴ� ������
    [SerializeField]
    private bool isCritical;        // ġ������ ���¶��
    [SerializeField]
    private bool isFalled;          // �ں����ִ� ���¶��
    [SerializeField]
    private bool IsAttack;          // ���� ���̶��
    [SerializeField]
    private bool IsGuard;           // ��� ���̶��
    [SerializeField]
    private bool bEnable = false;   // ���� �޺����� ����
    [SerializeField]
    private bool bExist = false;    // ���� ���� �޺����� ����
    [SerializeField]
    private int defenseCount = 0;   // ��� Ƚ��
    [SerializeField]
    private int comboIndex;         // ���� �޺� �ε���

    // Ÿ�� ��� ���� true/false
    public bool hasTarget
    {
        get
        {
            if(targetEntity != null && !targetEntity.dead)
            {
                // Ÿ���� �ְ� �Ÿ��� ��������� ���� �� �����̸� �ȵǹǷ�
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
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius, whatIsTarget);
            if(colliders.Length == 0)
            {
                targetEntity = null;
                // ������� �۾�, �ܰ� �ٱ����� �÷��̾� ������ ��� ����� �� �ֵ��� �ִϸ��̼� ����
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
        // ȸ�� �� ������ �Ѿư���
        if (angleDifference < 5f)
        {
            transform.transform.position = Vector3.MoveTowards(pos, targetPos, walkSpeed * Time.deltaTime);
            IsWalking = true;
            animator.SetBool("IsWalking", IsWalking);
        }
    }

    // ���Ͱ� ���ظ� �Ծ��ٸ�
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

        // ������ ó��
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
            End_Attack();                   // ���� �߿� ���� �� ������ false�� ����
            rigidbody.isKinematic = false;
            float launch = rigidbody.drag * data.Distance * 100.0f;
            Debug.Log("zombie.cs , �и��� �� launch" + launch);
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
    /*�ִϸ��̼� eventȣ�� �޼���*/
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

    // ���Ͱ� �׾��ٸ�
    public override void Die()
    {
        base.Die();

        Collider[] colliders = GetComponents<Collider>();
        for(int i=0; i< colliders.Length; i++) 
        {
            colliders[i].enabled = false;
        }

        /*���� ���*/
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

        /*�� �ݰ��� Ž���ϴ� �� Draw*/
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
        /*�̵��� ��ǥ ��θ� �׸��� �� Draw*/
        Gizmos.color = Color.red;
        Debug.DrawLine(transform.position, destPos, Color.red);
    }
}