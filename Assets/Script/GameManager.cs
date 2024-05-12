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
    public bool isGameover { get; private set; } // ���� ���� ����
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
    }


    private void EndGame()
    {
        isGameover = true;
        UIManager.instance.SetTextActive(isGameover);
    }
}
