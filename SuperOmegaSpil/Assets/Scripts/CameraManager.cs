using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            Debug.LogWarning("FUCK GUYS NOOO theres more than one instance of CameraManager!!!");
        }
    }

    [SerializeField] List<GameObject> cameras = new List<GameObject>();

    public void switchCam()
    {
        cameras[0].GetComponent<Camera>().enabled = !cameras[0].GetComponent<Camera>().enabled;
        cameras[0].GetComponent<AudioListener>().enabled = !cameras[0].GetComponent<AudioListener>().enabled;
        cameras[1].GetComponent<Camera>().enabled = !cameras[1].GetComponent<Camera>().enabled; 
        cameras[1].GetComponent<AudioListener>().enabled = !cameras[1].GetComponent<AudioListener>().enabled;
    }
}
