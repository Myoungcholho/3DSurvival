using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public partial class Player : LivingEntitiy
{
    [SerializeField]
    private float walkSpeed = 2.0f;
    [SerializeField]
    private float runSpeed = 4.0f;
    private float speed;
    private float horizontal;
    private float vertical;
    private bool bRun;
    Vector3 direction;

    [SerializeField]
    private GameObject swordPrefab;
    private GameObject swordDestination; // ���� Instantiate ����
    private Sword sword; // ������ ��ũ��Ʈ
    private Transform holsterTransform;
    private Transform handTransform;

    private Animator animator;
    private bool bHitting;

    [SerializeField]
    private GameObject shieldPrefab;
    private GameObject shieldDestination;   // ���� Instantiate ����
    private Transform shieldTransform;

    // �и��� ���� Effect ����
    [SerializeField]
    DoActionData parringData;

    // �÷��̾� �ݶ��̴�
    private new Collider collider;

    // UI HP bar
    public Slider healthBar;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        collider = GetComponent<Collider>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        healthBar.gameObject.SetActive(true);
        healthBar.maxValue = startingHealth;
        healthBar.value = health;
    }

    private void Start()
    {
        holsterTransform = transform.FindChildByName("Holster");
        handTransform = transform.FindChildByName("HolderWeapon");
        shieldTransform = transform.FindChildByName("ShieldAttachPoint");
        if (swordPrefab != null)
        {
            swordDestination = Instantiate<GameObject>(swordPrefab, holsterTransform);
            sword = swordDestination.GetComponent<Sword>();
        }
        if(shieldPrefab != null)
        {
            shieldDestination = Instantiate<GameObject>(shieldPrefab, shieldTransform);
        }
    }

    private void Update()
    {
        if (GameManager.instance.isGameover)
            return;

        if (bHitting)
            return;
        if (bParring)
            return;

        UpdateMoving();
        UpdateDrawing();
        UpdateAttacking();
        UpdateGuarding();
    }

    private void UpdateMoving()
    {
        if (bAttacking)
            return;
        if (bGuarding)
            return;

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        bool bRun = Input.GetButton("Run");
        speed = bRun ? runSpeed : walkSpeed;
        direction = (Vector3.forward * vertical) + (Vector3.right * horizontal);
        direction = direction.normalized * speed;
        transform.Translate(direction * Time.deltaTime);
        animator.SetFloat("SpeedX", direction.x);
        animator.SetFloat("SpeedZ", direction.z);
    }

    // �÷��̾� �ǰ� �� ȣ��
    public override void OnDamage(Vector3 hitPoint,Vector3 hitNormal, GameObject attacker, DoActionData data)
    {
        Debug.Log("OnDamage " + this.gameObject.name);

        if(bParryExist)
        {
            // �������
            // �и� ���� �� �� ���� �ִϸ��̼� ���·� �̵��� �� �ְԲ�
            // �� ���� �ִϸ��̼� ���� Critical ���·� �� ��
            Debug.Log("�и� ����");
            ReflectDamage(attacker);
            return;
        }
        FrameComponent.Instance.Delay(data.StopFrame);
        Debug.Log("Delay : " + data.StopFrame);

        // ���� �и� Ÿ�̹� �� �������� �޾Ҵٸ�

        if (data.HitParticle != null)
        {
            GameObject obj = Instantiate(data.HitParticle, transform, false);
            obj.transform.localPosition = data.HitParticlePositionOffset;
            obj.transform.localScale = data.HitParticleScaleOffset;
        }

        if (bBlocking)
        {
            Debug.Log("Player OnDamage ����-> ������ ��ȿ");
            return;
        }

        base.OnDamage(hitPoint, hitNormal, attacker, data);


        healthBar.value = health;
        if (dead)
            return;

        animator.SetTrigger("Hitting");
        bHitting = true;
        bParring = false;
    }

    private void ReflectDamage(GameObject attacker)
    {
        LivingEntitiy living = attacker.GetComponent<LivingEntitiy>();
        if(living == null)
        {
            Debug.Log("ReflectDamage() null");
            return;
        }
        living.OnDamage(Vector3.zero, Vector3.zero, this.gameObject, parringData);
    }

    private void End_Hitting()
    {
        bHitting = false;
    }

    public override void Die()
    {
        base.Die();
        animator.SetTrigger("IsDead");
        collider.enabled = false;
    }
}
