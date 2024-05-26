using System.Collections;
using UnityEngine;

public class FrameComponent : MonoBehaviour
{
    private static FrameComponent instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }


    public static FrameComponent Instance => instance;


    private Animator[] animators;

    private void Start()
    {
        animators = FindObjectsOfType<Animator>();
    }

    public void Delay(int frame)
    {
        StartCoroutine(Start_Delay(frame));
    }

    private IEnumerator Start_Delay(int frame)
    {
        foreach (Animator animator in animators)
        {
            if (animator != null)
                animator.speed = 0.0f;
        }


        for (int i = 0; i < frame; i++)
            yield return new WaitForFixedUpdate();


        foreach (Animator animator in animators)
        {
            if (animator != null)
                animator.speed = 1.0f;
        }
    }
}