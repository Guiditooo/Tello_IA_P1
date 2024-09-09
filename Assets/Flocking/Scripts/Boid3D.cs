using System.Collections.Generic;
using UnityEngine;

public class Boid3D : MonoBehaviour
{
    public Vector3 velocity;
    public float maxSpeed = 5.0f;
    public float maxForce = 0.5f;
    public float neighborRadius = 3.0f;
    public float separationRadius = 1.5f;

    private Vector3 acceleration;
    private Transform target;

    void Start()
    {
        velocity = Random.insideUnitCircle * maxSpeed;
        acceleration = Vector3.zero;
    }

    void Update()
    {
        Vector3 sep = Separation();
        Vector3 ali = Alignment();
        Vector3 coh = Cohesion();

        if (target != null)
        {
            Vector3 seekTarget = Seek(target.position);
            acceleration += seekTarget;
        }

        acceleration += sep;
        acceleration += ali;
        acceleration += coh;

        velocity += acceleration * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        transform.position += velocity * Time.deltaTime;

        acceleration = Vector3.zero;

        if (velocity != Vector3.zero)
            transform.forward = velocity.normalized;
    }

    // Separation: mantiene distancia de otros boids
    Vector3 Separation() 
    {
        Vector3 force = Vector3.zero;
        int count = 0;

        foreach (Boid3D other in FindNeighbors())
        {
            float distance = Vector3.Distance(transform.position, other.transform.position);
            if (distance > 0 && distance < separationRadius)
            {
                Vector3 diff = transform.position - other.transform.position;
                diff.Normalize();
                diff /= distance;
                force += diff;
                count++;
            }
        }

        if (count > 0)
        {
            force /= count;
        }

        return force;
    }

    // Alignment: Hace que su rotación sea igual a la rotación promedio de los vecinos
    Vector3 Alignment()
    {
        Vector3 avgVelocity = Vector3.zero;
        int count = 0;

        foreach (Boid3D other in FindNeighbors())
        {
            avgVelocity += other.velocity;
            count++;
        }

        if (count > 0)
        {
            avgVelocity /= count;
            Vector3 steer = avgVelocity - velocity;
            return Vector3.ClampMagnitude(steer, maxForce);
        }

        return Vector3.zero;
    }

    // Cohesion: Se intenta mover al centro de la formación
    Vector3 Cohesion()
    {
        Vector3 centerOfMass = Vector3.zero;
        int count = 0;

        foreach (Boid3D other in FindNeighbors())
        {
            centerOfMass += other.transform.position;
            count++;
        }

        if (count > 0)
        {
            centerOfMass /= count;
            return Seek(centerOfMass);
        }

        return Vector3.zero;
    }

    List<Boid3D> FindNeighbors()
    {
        List<Boid3D> neighbors = new List<Boid3D>();
        foreach (Boid3D other in FindObjectsOfType<Boid3D>())
        {
            if (other != this && Vector3.Distance(transform.position, other.transform.position) < neighborRadius)
            {
                neighbors.Add(other);
            }
        }
        return neighbors;
    }

    Vector3 Seek(Vector3 target)
    {
        Vector3 desired = target - transform.position;
        desired = Vector3.ClampMagnitude(desired, maxSpeed);
        Vector3 steer = desired - velocity;
        return Vector3.ClampMagnitude(steer, maxForce);
    }

    public void SetTarget(Transform newTarget) => target = newTarget;
    public Transform GetTarget() => target;
}
