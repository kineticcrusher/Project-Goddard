using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class DynamicPlanet : MonoBehaviour
{
    public int minResolution = 3; // Minimum number of subdivisions
    public int maxResolution = 6; // Maximum number of subdivisions
    public Camera mainCamera; // The camera used to determine visibility and distance
    public float cubeDistance = 300.0f; // Distance at which the planet appears as a cube
    public float maxSubdivisionDistance = 100.0f; // Distance at which maximum subdivision occurs
    public float planetRadius = 50.0f; // Radius of the planet
    public float updateInterval = 0.5f; // Time interval between mesh updates

    private MeshFilter meshFilter; // MeshFilter component of the planet
    private MeshRenderer meshRenderer; // MeshRenderer component of the planet
    private Mesh mesh; // The mesh of the planet
    private float nextUpdateTime = 0.0f; // Time for the next update

    void Start()
    {
        // Initialize the mesh filter and renderer
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        // Create an initial subdivided cube mesh
        mesh = CreateSubdividedCube(minResolution);
        meshFilter.mesh = mesh;

        // Use the main camera if no camera is specified
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void Update()
    {
        // Handle camera movement and rotation
        HandleCameraControls();

        // Update the mesh based on the distance to the camera at specified intervals
        if (Time.time >= nextUpdateTime)
        {
            nextUpdateTime = Time.time + updateInterval;
            UpdateMeshBasedOnDistance();
        }
    }

    void HandleCameraControls()
    {
        // Move the camera using WASD keys and rotate using the mouse
        float moveSpeed = 10.0f;
        float rotationSpeed = 100.0f;

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveForwardBackward = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(moveHorizontal, 0, moveForwardBackward) * moveSpeed * Time.deltaTime;
        mainCamera.transform.Translate(move);

        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

        mainCamera.transform.Rotate(Vector3.up, mouseX);
        mainCamera.transform.Rotate(Vector3.right, -mouseY);
    }

    void UpdateMeshBasedOnDistance()
    {
        // Determine the distance from the camera to the planet
        float distance = Vector3.Distance(mainCamera.transform.position, transform.position);
        // Calculate the number of subdivisions based on the distance
        int subdivisions = Mathf.RoundToInt(Mathf.Lerp(maxResolution, minResolution, distance / maxSubdivisionDistance));

        // Create a new subdivided cube mesh and morph it into a sphere
        mesh = CreateSubdividedCube(subdivisions);
        MorphToSphere(mesh, planetRadius);
        meshFilter.mesh = mesh;
    }

    Mesh CreateSubdividedCube(int subdivisions)
    {
        Mesh mesh = new Mesh();

        // Define the vertices of a cube
        Vector3[] vertices = {
            new Vector3(-1, -1, -1), new Vector3(1, -1, -1), new Vector3(1, 1, -1), new Vector3(-1, 1, -1),
            new Vector3(-1, -1, 1), new Vector3(1, -1, 1), new Vector3(1, 1, 1), new Vector3(-1, 1, 1)
        };

        // Define the triangles (faces) of a cube
        int[] triangles = {
            0, 2, 1, 0, 3, 2,  // Back
            4, 5, 6, 4, 6, 7,  // Front
            0, 1, 5, 0, 5, 4,  // Bottom
            2, 3, 7, 2, 7, 6,  // Top
            0, 4, 7, 0, 7, 3,  // Left
            1, 2, 6, 1, 6, 5   // Right
        };

        // Assign the vertices and triangles to the mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        // Subdivide the mesh the specified number of times
        for (int i = 0; i < subdivisions; i++)
        {
            mesh = Subdivide(mesh);
        }

        return mesh;
    }

    Mesh Subdivide(Mesh mesh)
    {
        var oldVertices = mesh.vertices;
        var oldTriangles = mesh.triangles;

        var newVertices = new List<Vector3>();
        var newTriangles = new List<int>();

        // Calculate the camera's view frustum planes
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);

        for (int i = 0; i < oldTriangles.Length; i += 3)
        {
            Vector3 v1 = oldVertices[oldTriangles[i]];
            Vector3 v2 = oldVertices[oldTriangles[i + 1]];
            Vector3 v3 = oldVertices[oldTriangles[i + 2]];

            // Subdivide only if the triangle is in view and facing the camera
            if (IsTriangleInView(v1, v2, v3, planes) && IsFacingCamera(v1, v2, v3))
            {
                // Calculate midpoints of the triangle's edges
                Vector3 v12 = (v1 + v2).normalized;
                Vector3 v23 = (v2 + v3).normalized;
                Vector3 v31 = (v3 + v1).normalized;

                int vIndex = newVertices.Count;

                // Add the vertices and triangles for the subdivided triangles
                newVertices.Add(v1);
                newVertices.Add(v2);
                newVertices.Add(v3);
                newVertices.Add(v12);
                newVertices.Add(v23);
                newVertices.Add(v31);

                newTriangles.Add(vIndex);
                newTriangles.Add(vIndex + 3);
                newTriangles.Add(vIndex + 5);

                newTriangles.Add(vIndex + 3);
                newTriangles.Add(vIndex + 1);
                newTriangles.Add(vIndex + 4);

                newTriangles.Add(vIndex + 5);
                newTriangles.Add(vIndex + 4);
                newTriangles.Add(vIndex + 2);

                newTriangles.Add(vIndex + 3);
                newTriangles.Add(vIndex + 4);
                newTriangles.Add(vIndex + 5);
            }
            else
            {
                int vIndex = newVertices.Count;

                // Add the vertices and triangle without subdivision
                newVertices.Add(v1);
                newVertices.Add(v2);
                newVertices.Add(v3);

                newTriangles.Add(vIndex);
                newTriangles.Add(vIndex + 1);
                newTriangles.Add(vIndex + 2);
            }
        }

        // Create a new mesh with the subdivided vertices and triangles
        Mesh newMesh = new Mesh
        {
            vertices = newVertices.ToArray(),
            triangles = newTriangles.ToArray()
        };

        newMesh.RecalculateNormals();
        newMesh.RecalculateBounds();

        return newMesh;
    }

    bool IsTriangleInView(Vector3 v1, Vector3 v2, Vector3 v3, Plane[] planes)
    {
        // Check if any of the triangle's vertices are within the camera's frustum
        if (GeometryUtility.TestPlanesAABB(planes, new Bounds(v1, Vector3.zero)) ||
            GeometryUtility.TestPlanesAABB(planes, new Bounds(v2, Vector3.zero)) ||
            GeometryUtility.TestPlanesAABB(planes, new Bounds(v3, Vector3.zero)))
        {
            return true;
        }

        return false;
    }

    bool IsFacingCamera(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        // Calculate the normal of the triangle
        Vector3 normal = Vector3.Cross(v2 - v1, v3 - v1).normalized;
        // Calculate the direction from the planet's center to the camera
        Vector3 toCamera = (mainCamera.transform.position - transform.position).normalized;

        // Check if the triangle is facing the camera
        return Vector3.Dot(normal, toCamera) > 0;
    }

    void MorphToSphere(Mesh mesh, float radius)
    {
        // Morph the vertices of the mesh to form a sphere
        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = vertices[i].normalized * radius;
        }
        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}
