using System.Collections.Generic;
using UnityEngine;

public class HandAttack : MonoBehaviour
{
    [SerializeField]
    DoActionData[] doActionDatas;

    private new Collider collider;
    private GameObject rootObject;
    private Zombie zombie;
    private List<GameObject> hittedList;

    private void Awake()
    {
        collider = GetComponent<Collider>();
        rootObject = transform.root.gameObject;
        hittedList = new List<GameObject>();
        zombie = transform.root.gameObject.GetComponent<Zombie>();
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
        Vector3 hitPoint = other.ClosestPoint(transform.position);          //다른 오브젝트의 가장 가까운 지점을 찾음.
        Vector3 hitNormal = transform.position - other.transform.position;
        entitiy?.OnDamage(hitPoint, hitNormal,rootObject, doActionDatas[zombie.ComboIndex]);
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
