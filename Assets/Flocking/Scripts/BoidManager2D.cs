using System.Collections.Generic;
using UnityEngine;

public class BoidManager2D : MonoBehaviour
{
    public int boidCount = 10;
    public float spawnRadius = 10.0f;
    public GameObject boidPrefab;

    private List<Boid2D> boidList;

    public Transform target;

    private void Start()
    {
        boidList = new List<Boid2D>();
        for (int i = 0; i < boidCount; i++)
        {
            Vector2 spawnPosition = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;
            Boid2D boid = Instantiate(boidPrefab, spawnPosition, Quaternion.identity).GetComponent<Boid2D>();
            boid.SetTarget(target);
            boidList.Add(boid);
        }
    }
    private void Update()
    {
        foreach (Boid2D boid in boidList)
        {
            if (boid.GetTarget() == target)
                return;

            boid.SetTarget(target);
        }   
    }

}
