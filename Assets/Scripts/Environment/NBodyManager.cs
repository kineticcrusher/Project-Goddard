using UnityEngine;

public class NBodyManager : MonoBehaviour {
    public const double gravitationalConstant = 0.001f; //real world value is 0.00000000006674;
    NBodyInstance[] nBodies;

    void Awake() {
        nBodies = FindObjectsOfType<NBodyInstance>();
    }

    /* updating velocities and THEN positions in that order helps to prevent instability caused by things like
     * the three-body problem.
     */
    void FixedUpdate() {
        for (int i = 0; i < nBodies.Length; ++i) {
            nBodies[i].CalculateGravitationalForce(nBodies, gravitationalConstant);
        }

        for (int i = 0; i < nBodies.Length; ++i) {
            nBodies[i].UpdatePosition();
        }
    }
}

//PLEASE APPROVE :(