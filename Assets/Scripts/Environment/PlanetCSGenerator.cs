using UnityEngine;

public class PlanetCSGenerator : MonoBehaviour {
    public Material planetMaterial;
    public float planetRadius = 1f;
    [Range(2, 255)]
    public int planetFaceResolution = 5;

    public bool useEvenVertDistribution = false;
    
    Vector3[] cubeFaces = new Vector3[6] {
        Vector3.up,
        Vector3.down,
        Vector3.left,
        Vector3.right,
        Vector3.forward,
        Vector3.back
    };
    MeshInfo[] faceMeshes;

    void Update() {
        if (Input.GetKeyDown(KeyCode.G)) {
            faceMeshes = new MeshInfo[6];
            foreach (Transform t in transform) {
                Destroy(t.gameObject);
            }
            
            for (int i = 0; i < 6; ++i) {
                GeneratePlaneObject(planetFaceResolution, cubeFaces[i]);
            }

            Debug.Log("Planet generated in " + Time.deltaTime + "s");
        }
    }

    MeshInfo GeneratePlaneObject(int resolution, Vector3 normal) {
        MeshInfo data = MeshTools.GeneratePlaneMesh(resolution, normal);
        for (int i = 0; i < data.verts.Length; ++i) {
            if (!useEvenVertDistribution) data.verts[i] = MeshTools.CubePointToSphere(data.verts[i]) * planetRadius;
            else data.verts[i] = MeshTools.CubePointToSphereEvenDist(data.verts[i]) * planetRadius;
        }

        GameObject planeObject = new("Planet Face"); planeObject.transform.parent = transform;
        MeshFilter filt = planeObject.AddComponent<MeshFilter>();
        filt.mesh.vertices = data.verts; filt.mesh.triangles = data.tris; filt.mesh.uv = data.uv;
        filt.mesh.RecalculateNormals();
        planeObject.AddComponent<MeshRenderer>().material = planetMaterial;

        return data;
    }
}