using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class DynamicPlanet : MonoBehaviour
{
    public int minResolution = 2;
    public int maxResolution = 6;
    public Camera mainCamera;
    public float cubeDistance = 300.0f;
    public float maxSubdivisionDistance = 100.0f;
    public float planetRadius = 50.0f;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Mesh mesh;
    private float updateInterval = 1.0f;
    private float nextUpdateTime = 0.0f;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        mesh = CreateSubdividedCube(minResolution); // Start with minimum resolution
        meshFilter.mesh = mesh;

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void Update()
    {
        HandleCameraControls();

        if (Time.time >= nextUpdateTime)
        {
            nextUpdateTime = Time.time + updateInterval;
            UpdateMeshBasedOnDistance();
        }
    }

    void HandleCameraControls()
    {
        float moveSpeed = 10.0f;
        float rotationSpeed = 100.0f;

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        float moveForwardBackward = Input.GetAxis("ForwardBackward"); // Custom input for forward/backward

        Vector3 move = new Vector3(moveHorizontal, moveVertical, moveForwardBackward) * moveSpeed * Time.deltaTime;
        mainCamera.transform.Translate(move);

        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

        mainCamera.transform.Rotate(Vector3.up, mouseX);
        mainCamera.transform.Rotate(Vector3.right, -mouseY);
    }


    void UpdateMeshBasedOnDistance()
    {
        float distance = Vector3.Distance(mainCamera.transform.position, transform.position);
        int subdivisions = Mathf.RoundToInt(Mathf.Lerp(maxResolution, minResolution, distance / maxSubdivisionDistance));

        mesh = CreateSubdividedCube(subdivisions);
        MorphToSphere(mesh, planetRadius);
        meshFilter.mesh = mesh;
    }

    Mesh CreateSubdividedCube(int subdivisions)
    {
        Mesh mesh = new Mesh();

        // Define the initial vertices and triangles of a cube
        Vector3[] vertices = {
            new Vector3(-1, -1, -1), new Vector3(1, -1, -1), new Vector3(1, 1, -1), new Vector3(-1, 1, -1),
            new Vector3(-1, -1, 1), new Vector3(1, -1, 1), new Vector3(1, 1, 1), new Vector3(-1, 1, 1)
        };

        int[] triangles = {
            0, 2, 1, 0, 3, 2,  // Back
            4, 5, 6, 4, 6, 7,  // Front
            0, 1, 5, 0, 5, 4,  // Bottom
            2, 3, 7, 2, 7, 6,  // Top
            0, 4, 7, 0, 7, 3,  // Left
            1, 2, 6, 1, 6, 5   // Right
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

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

        var newVertices = new Vector3[oldVertices.Length + (oldTriangles.Length / 3) * 3 * 3];
        var newTriangles = new int[oldTriangles.Length * 4];

        int vIndex = 0;
        int tIndex = 0;

        for (int i = 0; i < oldTriangles.Length; i += 3)
        {
            Vector3 v1 = oldVertices[oldTriangles[i]];
            Vector3 v2 = oldVertices[oldTriangles[i + 1]];
            Vector3 v3 = oldVertices[oldTriangles[i + 2]];

            // Calculate midpoints of each edge
            Vector3 v12 = (v1 + v2).normalized;
            Vector3 v23 = (v2 + v3).normalized;
            Vector3 v31 = (v3 + v1).normalized;

            // Add new vertices
            newVertices[vIndex] = v1;
            newVertices[vIndex + 1] = v2;
            newVertices[vIndex + 2] = v3;
            newVertices[vIndex + 3] = v12;
            newVertices[vIndex + 4] = v23;
            newVertices[vIndex + 5] = v31;

            // Define new triangles
            newTriangles[tIndex] = vIndex;
            newTriangles[tIndex + 1] = vIndex + 3;
            newTriangles[tIndex + 2] = vIndex + 5;

            newTriangles[tIndex + 3] = vIndex + 3;
            newTriangles[tIndex + 4] = vIndex + 1;
            newTriangles[tIndex + 5] = vIndex + 4;

            newTriangles[tIndex + 6] = vIndex + 5;
            newTriangles[tIndex + 7] = vIndex + 4;
            newTriangles[tIndex + 8] = vIndex + 2;

            newTriangles[tIndex + 9] = vIndex + 3;
            newTriangles[tIndex + 10] = vIndex + 4;
            newTriangles[tIndex + 11] = vIndex + 5;

            vIndex += 6; // Move to the next set of vertices
            tIndex += 12; // Move to the next set of triangles
        }

        Mesh newMesh = new Mesh
        {
            vertices = newVertices,
            triangles = newTriangles
        };

        newMesh.RecalculateNormals();
        newMesh.RecalculateBounds();

        return newMesh;
    }


    void MorphToSphere(Mesh mesh, float radius)
    {
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

