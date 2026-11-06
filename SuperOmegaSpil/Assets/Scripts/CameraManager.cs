using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    new List<Camera> cameras = new List<Camera>();

    public void switchCam()
    {
        foreach (Camera cam in cameras)
        {
            if (cam.enabled == true)
            {
                cam.enabled = false;
            }
            else
            {
                cam.enabled = true;
            }
        }
    }
}
