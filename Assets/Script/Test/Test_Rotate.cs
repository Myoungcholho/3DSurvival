using Unity.VisualScripting;
using UnityEngine;

public class Test_Rotate : MonoBehaviour
{
    public GameObject target;    


    void Update()
    {
        transform.Rotate(0, 25f * Time.deltaTime, 0);


        Vector3 targetPos = target.transform.position;
        Vector3 position = transform.position;

        Vector3 direction = targetPos - position;
        //transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
        Debug.DrawLine(position, position + transform.forward * direction.magnitude, Color.red);
    }

    private void OnGUI()
    {
        GUI.color = Color.red;
        GUILayout.Label($"Euler Angle : {transform.eulerAngles}");
        
    }
}
