using System.Collections.Generic;
using UnityEngine;

public class HandAttack : MonoBehaviour
{
    [SerializeField]
    DoActionData[] doActionDatas;

    private new Collider collider;
    private GameObject rootObject;
    private List<GameObject> hittedList;
    [SerializeField]
    private float damage = 20f;
    //private Zombie zombie;

    //���� ���� �޺�
    public int comboIndex;

    private void Awake()
    {
        collider = GetComponent<Collider>();
        rootObject = transform.root.gameObject;
        hittedList = new List<GameObject>();
        //zombie = transform.root.gameObject.GetComponent<Zombie>();
    }

    private void Start()
    {
        End_Collision();
    }

    private void OnTriggerStay(Collider other)
    {
        if (rootObject == other.gameObject)
            return;

        if (hittedList.Contains(other.gameObject))
            return;

        hittedList.Add(other.gameObject);
        
        LivingEntitiy entitiy = other.GetComponent<LivingEntitiy>();
        Vector3 hitPoint = other.ClosestPoint(transform.position);          //�ٸ� ������Ʈ�� ���� ����� ������ ã��.
        Vector3 hitNormal = transform.position - other.transform.position;
        entitiy?.OnDamage(damage, hitPoint, hitNormal,rootObject, doActionDatas[0]);
    }

    public void Begin_Collision()
    {
        collider.enabled = true;
    }

    public void End_Collision()
    {
        collider.enabled = false;
        hittedList.Clear();
    }
}
