using UnityEngine;

public class NBodyInstance : MonoBehaviour {
    public float mass;
    public Vector3 initialVelocity;
    public bool isAffected = true; public bool isAffector = true;
    Vector3 velocity;

    void Awake() {
        velocity = initialVelocity;
    }

    // implements newton's universal gravitational formula (F = G * m1*m2 / r^2)
    public void CalculateGravitationalForce(NBodyInstance[] otherNBodies, double gravitationalConstant) {
        if (isAffected)
            foreach (NBodyInstance affector in otherNBodies)
                if (affector != this && affector.isAffector) {
                    float distance = (affector.transform.position - transform.position).sqrMagnitude;
                    Vector3 direction = (affector.transform.position - transform.position).normalized;
                    double force = gravitationalConstant * ((mass * affector.mass) / distance);

                    velocity += direction * ((float)force / mass);
                }
    }

    public void UpdatePosition() {
        transform.position += velocity;
    }
}