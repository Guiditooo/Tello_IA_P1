using System.Collections.Generic;
using UnityEngine;

public class Boid2D : MonoBehaviour
{
    public Vector2 velocity;
    public float maxSpeed = 5.0f;
    public float maxForce = 0.5f;
    public float neighborRadius = 3.0f;
    public float separationRadius = 1.5f;

    private Vector2 acceleration;
    private Transform target;

    void Start()
    {
        velocity = Random.insideUnitCircle * maxSpeed; // Velocidad inicial aleatoria
        acceleration = Vector2.zero;
    }

    void Update()
    {
        // Calcular las fuerzas del flocking
        Vector2 sep = Separation();
        Vector2 ali = Alignment();
        Vector2 coh = Cohesion();

        // Si hay un objetivo, agregar la fuerza de Seek hacia el objetivo
        if (target != null)
        {
            Vector2 seekTarget = Seek(target.position);
            acceleration += seekTarget;
        }

        // Aplicar las fuerzas
        acceleration += sep;
        acceleration += ali;
        acceleration += coh;

        // Actualizar velocidad y posición
        velocity += acceleration * Time.deltaTime;
        velocity = Vector2.ClampMagnitude(velocity, maxSpeed);
        transform.position += (Vector3)(velocity * Time.deltaTime);

        // Resetear la aceleración
        acceleration = Vector2.zero;

        // Alinear la dirección del Boid2D con su movimiento
        if (velocity != Vector2.zero)
            transform.up = velocity.normalized;
    }

    // Separación: Mantener distancia con otros boids
    Vector2 Separation()
    {
        Vector2 force = Vector2.zero;
        int count = 0;

        foreach (Boid2D other in FindNeighbors())
        {
            float distance = Vector2.Distance(transform.position, other.transform.position);
            if (distance > 0 && distance < separationRadius)
            {
                Vector2 diff = (Vector2)transform.position - (Vector2)other.transform.position;
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
    Vector2 Alignment()
    {
        Vector2 avgVelocity = Vector2.zero;
        int count = 0;

        foreach (Boid2D other in FindNeighbors())
        {
            avgVelocity += other.velocity;
            count++;
        }

        if (count > 0)
        {
            avgVelocity /= count;
            Vector2 steer = avgVelocity - velocity;
            return Vector2.ClampMagnitude(steer, maxForce);
        }

        return Vector2.zero;
    }

    // Cohesión: Moverse hacia el centro del grupo
    Vector2 Cohesion()
    {
        Vector2 centerOfMass = Vector2.zero;
        int count = 0;

        foreach (Boid2D other in FindNeighbors())
        {
            centerOfMass += (Vector2)other.transform.position;
            count++;
        }

        if (count > 0)
        {
            centerOfMass /= count;
            return Seek(centerOfMass);
        }

        return Vector2.zero;
    }

    // Función de búsqueda de vecinos cercanos
    List<Boid2D> FindNeighbors()
    {
        List<Boid2D> neighbors = new List<Boid2D>();
        foreach (Boid2D other in FindObjectsOfType<Boid2D>())
        {
            if (other != this && Vector2.Distance(transform.position, other.transform.position) < neighborRadius)
            {
                neighbors.Add(other);
            }
        }
        return neighbors;
    }

    // Función Seek: Moverse hacia un objetivo
    Vector2 Seek(Vector2 target)
    {
        Vector2 desired = target - (Vector2)transform.position;
        desired = Vector2.ClampMagnitude(desired, maxSpeed);
        Vector2 steer = desired - velocity;
        return Vector2.ClampMagnitude(steer, maxForce);
    }

    public void SetTarget(Transform newTarget) => target = newTarget;
    public Transform GetTarget() => target;
}
