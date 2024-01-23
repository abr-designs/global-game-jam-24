using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    [SerializeField, Min(1)]
    private int spawnCount = 1;
    [SerializeField]
    private GameObject[] prefabs;

    [SerializeField]
    private TMP_Text countText;
    
    private int instanceCount;
    
    // Start is called before the first frame update
    private void Start()
    {
        countText.text = $"Count: 0";
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) == false)
            return;

        for (int i = 0; i < spawnCount; i++)
        {
            SpawnRandomObject();
        }
    }

    private void SpawnRandomObject()
    {
        var index = Random.Range(0, prefabs.Length);
        var random = prefabs[index];

        var instance = Instantiate(random, transform.position, Quaternion.Euler(RandomDegree(), RandomDegree(), RandomDegree()));
        
        instanceCount++;
        SetCountText(instanceCount);
    }
    
    private float RandomDegree()
    {
        return Random.Range(0f, 360f);
    }


    private void SetCountText(int count)
    {
        countText.text = $"Count: {count.ToString()}";
    }
}
