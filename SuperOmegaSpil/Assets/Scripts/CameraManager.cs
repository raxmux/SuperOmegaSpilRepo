using System;
using System.Collections;
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
    [SerializeField] ScreenTrans screenTrans;

    public void switchCam()
    {
        screenTrans.Fade();
        StartCoroutine(switchCamEnumerator());
    }

    public IEnumerator switchCamEnumerator()
    {
        yield return new WaitForSeconds(0.3f);
        cameras[0].GetComponent<Camera>().enabled = !cameras[0].GetComponent<Camera>().enabled;
        cameras[0].GetComponent<AudioListener>().enabled = !cameras[0].GetComponent<AudioListener>().enabled;
        cameras[1].GetComponent<Camera>().enabled = !cameras[1].GetComponent<Camera>().enabled;
        cameras[1].GetComponent<AudioListener>().enabled = !cameras[1].GetComponent<AudioListener>().enabled;
    }
}
