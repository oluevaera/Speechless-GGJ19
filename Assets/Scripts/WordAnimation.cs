using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WordAnimation : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro text;

    [SerializeField]
    private float lifeTime;
    [SerializeField]
    private AnimationCurve fade;
    [SerializeField]
    private float minSpeed = 0.75f;
    [SerializeField]
    private float maxSpeed = 1f;

    private float progress = 0;
    private float speed;

    private void Start() 
    {
        speed = Random.Range(minSpeed, maxSpeed);
    }

    void Update()
    {
        progress += Time.deltaTime;

        if (progress > lifeTime)
        {
            Destroy(gameObject);
        }

        // Update text colour
        var col = text.color;
        col.a = fade.Evaluate(progress / lifeTime);
        text.color = col;

        // Update position
        transform.position += transform.TransformDirection(new Vector3(speed * Time.deltaTime, 0, 0));
    }
}
