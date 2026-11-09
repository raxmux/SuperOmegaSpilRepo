using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
 
    [SerializeField] List<GameObject> cameras = new List<GameObject>();

    public void switchCam()
    {
        cameras[0].GetComponent<Camera>().enabled = !cameras[0].GetComponent<Camera>().enabled;
        cameras[0].GetComponent<AudioListener>().enabled = !cameras[0].GetComponent<AudioListener>().enabled;
        cameras[1].GetComponent<Camera>().enabled = !cameras[1].GetComponent<Camera>().enabled; 
        cameras[1].GetComponent<AudioListener>().enabled = !cameras[1].GetComponent<AudioListener>().enabled;
    }
}
