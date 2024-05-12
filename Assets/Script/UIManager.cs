using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager m_instance;
    public static UIManager instance
    {
        get
        {
            if(m_instance == null)
            {
                m_instance = FindObjectOfType<UIManager>();
                return m_instance;
            }
            else { return m_instance; }
        }
    }
    public Text gameOverText;
    
    public void SetTextActive(bool value)
    {
        gameOverText.gameObject.SetActive(value);
    }
}