using System.Collections;
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
        velocity = Random.insideUnitCircle * maxSpeed; // Velocidad inicial aleatoria
        acceleration = Vector3.zero;
    }

    void Update()
    {
        Vector3 sep = Separation();
        Vector3 ali = Alignment();
        Vector3 coh = Cohesion();

        // Si hay un objetivo, agregar la fuerza de Seek hacia el objetivo
        if (target != null)
        {
            Vector3 seekTarget = Seek(target.position);
            acceleration += seekTarget;
        }

        // Aplicar las fuerzas de flocking
        acceleration += sep;
        acceleration += ali;
        acceleration += coh;

        // Actualizar velocidad y posición
        velocity += acceleration * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        transform.position += velocity * Time.deltaTime;

        // Resetear la aceleración
        acceleration = Vector3.zero;

        // Alinear la dirección del Boid con su movimiento
        if (velocity != Vector3.zero)
            transform.forward = velocity.normalized;
    }

    // Separación: Mantener distancia con otros boids
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

    // Alineación: Moverse en la misma dirección que los vecinos
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

    // Cohesión: Moverse hacia el centro del grupo
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

    // Función de búsqueda de vecinos cercanos
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

    // Función Seek: Moverse hacia un objetivo
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
