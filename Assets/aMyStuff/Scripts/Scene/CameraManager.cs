using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering;
using System.Security.Cryptography;
using static UnityEngine.UI.Image;

public class CameraManager : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCam;
    public Camera mainCam;
    public bool usingVirtualCam = true;
    public bool edgeScrolling = false;
    public bool dragScrolling = true;
    public int dragScrollSpeed = 25;
    public int edgeScrollSpeed = 10;
    public int maxZoomDistance = 40;

    private CinemachineComponentBase camBody;
    private Vector3 lastMousePos;

    void Start()
    {
        camBody = virtualCam.GetCinemachineComponent(CinemachineCore.Stage.Body);
    }

    void Update()
    {
        // Lock/Unlock camera to player character
        if (Input.GetKeyDown(KeyCode.Space))
        {
            usingVirtualCam = !usingVirtualCam;

            if (usingVirtualCam)
            {
                virtualCam.gameObject.SetActive(true);
            }
            else
            {
                virtualCam.gameObject.SetActive(false);
                mainCam.gameObject.SetActive(true);
            }
        }

        // Zoom in
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (usingVirtualCam)
            {
                if (camBody is CinemachineFramingTransposer)
                {
                    (camBody as CinemachineFramingTransposer).m_CameraDistance = Mathf.Clamp((camBody as CinemachineFramingTransposer).m_CameraDistance - 1, 10, maxZoomDistance);
                }
            }
            else
            {
                float dist = Vector3.Distance(mainCam.transform.position, virtualCam.Follow.transform.position);
                if (dist > 11) mainCam.transform.position += mainCam.transform.forward;
            }
        }

        // Zoom out
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (usingVirtualCam)
            {
                if (camBody is CinemachineFramingTransposer)
                {
                    (camBody as CinemachineFramingTransposer).m_CameraDistance = Mathf.Clamp((camBody as CinemachineFramingTransposer).m_CameraDistance + 1, 10, maxZoomDistance);
                }
            }
            else
            {
                float dist = Vector3.Distance(mainCam.transform.position, virtualCam.Follow.transform.position);
                if (dist < 19) mainCam.transform.position -= mainCam.transform.forward;
            }
        }

        if (!usingVirtualCam)
        {
            // Move cam when left click dragging
            if (dragScrolling)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    lastMousePos = Input.mousePosition;
                }

                if (Input.GetMouseButton(0))
                {
                    var delta = lastMousePos - Input.mousePosition;

                    if (delta.x < 0)
                    {
                        mainCam.transform.position -= Vector3.left * Time.deltaTime * dragScrollSpeed;
                    }
                    else if (delta.x > 0)
                    {
                        mainCam.transform.position -= Vector3.right * Time.deltaTime * dragScrollSpeed;
                    }

                    if (delta.y < 0)
                    {
                        mainCam.transform.position -= Vector3.back * Time.deltaTime * dragScrollSpeed;
                    }
                    else if (delta.y > 0)
                    {
                        mainCam.transform.position -= Vector3.forward * Time.deltaTime * dragScrollSpeed;
                    }

                    lastMousePos = Input.mousePosition;
                }
            }

            if (edgeScrolling)
            {
                // Move cam when mouse near edge of screen and not dragging
                if (!Input.GetMouseButton(0))
                {
                    float x = Input.mousePosition.x;
                    float y = Input.mousePosition.y;

                    if (x < 10)
                    {
                        mainCam.transform.position -= Vector3.left * Time.deltaTime * edgeScrollSpeed;
                    }
                    else if (x > Screen.width - 10)
                    {
                        mainCam.transform.position -= Vector3.right * Time.deltaTime * edgeScrollSpeed;
                    }
                    if (y < 10)
                    {
                        mainCam.transform.position -= Vector3.back * Time.deltaTime * edgeScrollSpeed;
                    }
                    if (y > Screen.height - 10)
                    {
                        mainCam.transform.position -= Vector3.forward * Time.deltaTime * edgeScrollSpeed;
                    }
                }
            }
        }
    }
}
