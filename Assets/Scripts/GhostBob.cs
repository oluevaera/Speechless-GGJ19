using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostBob : MonoBehaviour
{
    private float startY;

    void Start()
    {
        startY = transform.localPosition.y;
    }

    void Update()
    {
        Vector3 pos = transform.localPosition;
        pos.y = startY + ((Mathf.Sin(Time.time * 4f) + 1f) * 0.1f);
        transform.localPosition = pos;
    }
}
