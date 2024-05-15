using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager m_instance;
    public static GameManager instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<GameManager>();
            }
            return m_instance;
        }
    }
    public bool isGameover { get; private set; } // 게임 오버 상태
    public List<GameObject> spawnObjectList;
    private int monsterCnt;

    private void Awake()
    {
        if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        FindObjectOfType<Player>().onDeadth += EndGame;
        spawnObjectList = new List<GameObject>();
        monsterCnt = 2;
    }

    public void DeleteList(GameObject spawnObject)
    {
        if (spawnObjectList == null)
            ClearGame();

        foreach (GameObject obj in spawnObjectList)
        {
            if (obj == spawnObject)
            {
                spawnObjectList.Remove(obj);
                if (spawnObjectList.Count < 0)
                {
                    ClearGame();
                }
            }
        }
    }

    public void DeleteCnt(int value)
    {
        monsterCnt -= value;
        if(monsterCnt <= 0)
        {
            ClearGame();
        }
    }

    private void EndGame()
    {
        isGameover = true;
        UIManager.instance.OverTextUpdate(isGameover);
    }
    private void ClearGame()
    {
        isGameover = true;
        UIManager.instance.ClearTextUpdate(isGameover);
    }
}
