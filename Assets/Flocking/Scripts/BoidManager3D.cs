using System.Collections.Generic;
using UnityEngine;

public class BoidManager3D : MonoBehaviour
{
    public int boidCount = 10;
    public float spawnRadius = 10.0f;
    public GameObject boidPrefab;

    private List<Boid3D> boidList;

    public Transform target;

    private void Start()
    {
        boidList = new List<Boid3D>();
        for (int i = 0; i < boidCount; i++)
        {
            Vector3 spawnPosition = transform.position + Random.insideUnitSphere * spawnRadius;
            Boid3D boid = Instantiate(boidPrefab, spawnPosition, Quaternion.identity).GetComponent<Boid3D>();
            boid.SetTarget(target);
            boidList.Add(boid);
        }
    }
    private void Update()
    {
        foreach (Boid3D boid in boidList)
        {
            if (boid.GetTarget() == target)
                return;

            boid.SetTarget(target);
        }   
    }

}
