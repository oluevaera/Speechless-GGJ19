using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Attack : MonoBehaviour 
{
    [SerializeField]
    private List<string> attackWords;
    [Header("Word Settings")]
    [SerializeField]
    private GameObject wordPrefab;
    // How long to wait between word spawns
    [SerializeField]
    private float minWordGap;
    [SerializeField]
    private float maxWordGap;
    [SerializeField]
    private Vector3 arc = new Vector3(0, 30, 15);
    [SerializeField]
    private float range = 3f;
    [SerializeField]
    private List<Mesh> guides = new List<Mesh>();

    private float wordCooldown = 0;
    private bool attacking = false;

    public void StartAttack()
    {
        attacking = true;
    }

    public void StopAttack()
    {
        attacking = false;
    }

    private void Update() 
    {
        // Words
        if (wordCooldown > 0)
        {
            wordCooldown -= Time.deltaTime;
        }
        if (wordCooldown <= 0)
        {
            wordCooldown = Random.Range(minWordGap, maxWordGap);

            var rot = transform.rotation.eulerAngles;
            rot.x += Random.Range(-arc.x, arc.x);
            rot.y += Random.Range(-arc.y, arc.y);
            rot.z += Random.Range(-arc.z, arc.z);
            var go = Instantiate(wordPrefab, transform.position, Quaternion.Euler(rot));
            var text = go.GetComponent<TextMeshPro>();
            text.text = attackWords[Random.Range(0, attackWords.Count)];
        }
    }
}