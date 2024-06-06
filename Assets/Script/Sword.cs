using System;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    [SerializeField]
    DoActionData[] doActionDatas;

    private new Collider collider;
    private GameObject rootObject;
    private List<GameObject> hittedList;

    private void Awake()
    {
        collider = GetComponent<Collider>();
        rootObject = transform.root.gameObject;
        hittedList = new List<GameObject>();
    }

    private void Start()
    {
        End_Collision();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (rootObject == other.gameObject)
            return;

        if (hittedList.Contains(other.gameObject))
            return;

        hittedList.Add(other.gameObject);

        LivingEntitiy entitiy = other.GetComponent<LivingEntitiy>();
        if(entitiy != null) 
        {
            Player player = rootObject.GetComponent<Player>();
            entitiy.OnDamage(Vector3.zero, Vector3.zero,rootObject, doActionDatas[player.comboIndex]);
            
        }
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