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

        Vector3 perpend1 = new(normal.y, normal.z, normal.x);
        Vector3 perpend2 = Vector3.Cross(normal, perpend1);

        for (int i = 0, x = 0; x <= resolution; ++x) {
            for (int z = 0; z <= resolution; ++z) {
                float xFractionOfUnit = 1f / resolution * x;
                float zFractionOfUnit = 1f / resolution * z;
                float xOffsetToCentered = xFractionOfUnit - 0.5f;
                float zOffsetToCentered = zFractionOfUnit - 0.5f;

                verts[i] = (normal / 2) + (perpend1 * xOffsetToCentered) + (perpend2 * zOffsetToCentered);
                uv[i] = new(xFractionOfUnit, zFractionOfUnit);

                ++i;
            }
        }

        int[] tris = new int[resolution * resolution * 6];
        short trianglesOffset = 0;
        short vertexOffset = 0;
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
}