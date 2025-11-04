using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour
{
    [SerializeField] private Transform treePrefab;
    
    public HashSet<int> Init(float z)
    {
        //place the obstacle at the location provided
        transform.position = new Vector3(0, 0, z);

        //we always have obstacles outside the area
        HashSet<int> location = new() { -6, 6 };

        //populate with some obstacles 
        int numTrees = Random.Range(1, 5);
        for (int i = 0; i < numTrees; i++)
        {
            //create new tree object
            Transform tree = Instantiate(treePrefab, transform);
            
            //put tree at random location
            int xPos = Random.Range(-5, 6);
            tree.position = new Vector3(xPos, 0.1f, z);

            //record the location in our HashSet
            location.Add(xPos);

        }
        return location;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
