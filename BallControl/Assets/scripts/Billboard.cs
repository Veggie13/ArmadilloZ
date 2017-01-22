using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private void LateUpdate()
    {
        if (Camera.main != null)
        {
            transform.LookAt(Camera.main.transform.position, Camera.main.transform.up);
            //transform.Rotate(Vector3.up, 180);
        }
    }
}
