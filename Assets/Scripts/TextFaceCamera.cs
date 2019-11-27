using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextFaceCamera : MonoBehaviour
{
    private Camera m_camera;

    void Start()
    {
        m_camera = Camera.main;
    }
    
    void LateUpdate()
    {
        transform.LookAt(transform.position + m_camera.transform.rotation * Vector3.forward, m_camera.transform.rotation * Vector3.up);
    }
}
