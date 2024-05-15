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
    public Text gameClearText;
    
    public void OverTextUpdate(bool value)
    {
        gameOverText.gameObject.SetActive(value);
    }

    public void ClearTextUpdate(bool value)
    {
        gameClearText.gameObject.SetActive(value);
    }
}