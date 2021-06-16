using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestBehaviour : MonoBehaviour
{
    private WebCamTexture mCamera;
    public GameObject cubeObj, cameraPlane;
    private bool isRotating, GPSEnabled;
    public Text debugText;

    private void Start()
    {
        isRotating = false;
        GPSEnabled = false;

        mCamera = new WebCamTexture();
        cameraPlane.GetComponent<Renderer>().material.mainTexture = mCamera;
        mCamera.Stop();
    }

    void Update()
    {
#       if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                if (hit.collider.CompareTag("DisplayObject"))
                {
                    toggleRotate();
                }
            }
        }
        #endif

        #if UNITY_ANDROID
        if ((Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Began))
        {
            Ray raycast = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit raycastHit;
            if (Physics.Raycast(raycast, out raycastHit))
            {
                if (raycastHit.collider.CompareTag("DisplayObject"))
                {
                    toggleRotate();
                }
            }
        }
        #endif

        if (isRotating)
        {
            float getLastRot = cubeObj.transform.localRotation.y;
            getLastRot = getLastRot + 5;
            cubeObj.transform.Rotate(0, getLastRot, 0);
        }
    }

    public void toggleRotate()
    {
        switch (isRotating)
        {
            case true:
                isRotating = false;
                break;

            case false:
                isRotating = true;
                break;
        }
    }

    public void toggleCamera()
    {
        switch (mCamera.isPlaying)
        {
            case true:
                mCamera.Stop();
                break;

            case false:
                mCamera.Play();
                break;
        }
    }

    public void toggleGPS()
    {
        switch (GPSEnabled)
        { 
            case false:
                GPSEnabled = true;
                StartCoroutine(StartGPS());
                break;

            case true:
                GPSEnabled = false;
                StopCoroutine(StartGPS());
                debugText.text = "GPS Disabled";
                break;
        }
    }

    IEnumerator StartGPS()
    {
        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser)
            yield break;

        // Start service before querying location
        Input.location.Start();

        // Wait until service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            debugText.text = "Timed out";
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            debugText.text = "Unable to determine device location";
            yield break;
        }
        else
        {
            // Access granted and location value could be retrieved
            debugText.text = "Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp;
        }
    }
}
