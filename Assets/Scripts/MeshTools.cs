using UnityEngine;

// quick and easy storage for all the stuff unity meshes use :)
public struct MeshInfo {
    public Vector3[] verts; public int[] tris; public Vector2[] uv;

    public MeshInfo (Vector3[] verts, int[] tris, Vector2[] uv) {
        this.verts = verts; this.tris = tris; this.uv = uv;
    }
}

public static class MeshTools {
    public static MeshInfo GeneratePlaneMesh(int resolution, Vector3 normal) {
        Vector3[] verts = new Vector3[(resolution + 1) * (resolution + 1)];
        Vector2[] uv = new Vector2[verts.Length];

        // get the two perpendicular axes from the normal (local space xyz)
        Vector3 perpend1 = new(normal.y, normal.z, normal.x);
        Vector3 perpend2 = Vector3.Cross(normal, perpend1);

        // create grid of appropriate density specified by resolution
        for (int i = 0, x = 0; x <= resolution; ++x) {
            for (int z = 0; z <= resolution; ++z) {
                float xFractionOfUnit = 1f / resolution * x;
                float zFractionOfUnit = 1f / resolution * z;
                float xOffsetToCentered = xFractionOfUnit - 0.5f;
                float zOffsetToCentered = zFractionOfUnit - 0.5f;

                // move along local x and z based on position in grid space
                verts[i] = (normal / 2) + (perpend1 * xOffsetToCentered) + (perpend2 * zOffsetToCentered);
                // set uvs such that left edge is 0 and right edge is 1
                uv[i] = new(xFractionOfUnit, zFractionOfUnit);

                ++i;
            }
        }

        // create triangles (specified by vert index in clockwise order) by groups of 3
        int[] tris = new int[resolution * resolution * 6];
        int trianglesOffset = 0;
        int vertexOffset = 0;
        for (int x = 0; x < resolution; ++x) {
            for (int z = 0; z < resolution; ++z) {
                tris[trianglesOffset] = vertexOffset;
                tris[trianglesOffset + 1] = vertexOffset + resolution + 1;
                tris[trianglesOffset + 2] = vertexOffset + 1;
                tris[trianglesOffset + 3] = vertexOffset + 1;
                tris[trianglesOffset + 4] = vertexOffset + resolution + 1;
                tris[trianglesOffset + 5] = vertexOffset + resolution + 2;

                ++vertexOffset;
                trianglesOffset += 6;
            }

            ++vertexOffset;
        }

        return new MeshInfo(verts, tris, uv);
    }

    // sets all points to be equal distance from the center (which is the definition of a sphere)
    public static Vector3 CubePointToSphere(Vector3 point) {
        return point.normalized;
    }

    // not 100% sure how this works, but thanks to Seb Lague and http://mathproofs.blogspot.com/2005/07/mapping-cube-to-sphere.html
    public static Vector3 CubePointToSphereEvenDist(Vector3 point) {
        float x2 = point.x * point.x;
	    float y2 = point.y * point.y;
	    float z2 = point.z * point.z;
        float y = point.y * Mathf.Sqrt(1 - (z2 / 2) - (x2 / 2) + ((z2 * x2) / 3));
	    float x = point.x * Mathf.Sqrt(1 - (y2 / 2) - (z2 / 2) + ((y2 * z2) / 3));
        float z = point.z * Mathf.Sqrt(1 - (x2 / 2) - (y2 / 2) + ((x2 * y2) / 3));
	    return new Vector3(x, y, z).normalized;
    }
}