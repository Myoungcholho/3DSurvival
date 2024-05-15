using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject spawnObject;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(UpdateSpwan());
    }

    private IEnumerator UpdateSpwan()
    {
        while(!GameManager.instance.isGameover)
        {
            GameObject obj = Instantiate<GameObject>(spawnObject, transform.position, transform.rotation);
            GameManager.instance.spawnObjectList.Add(obj);

            yield return new WaitForSeconds(20);
        }
    }
}
