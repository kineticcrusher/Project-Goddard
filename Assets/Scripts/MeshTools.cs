using UnityEngine;

public static class MeshTools {
    public static (Vector3[], int[]) GeneratePlaneMesh(int dimensions, int resolution) {
        int lod = dimensions * resolution;
        Vector3[] verts = new Vector3[lod * lod];
        int[] tris = new int[lod * lod * 6];

        // create all the vertices at the correct amount of detail
        for (int i = 0, x = 0; x < lod; ++x) {
            for (int z = 0; z < lod; ++z) {
                verts[i] = new((float)x / resolution, 0, (float)z / resolution);
                ++i;
            }
        }

        // generate trianges between sets of three verts with wrapping from row to row and column to column
        short vertOffset = 0;
        short triOffset = 0;
        for (int x = 0; x < lod - 1; ++x) {
            for (int z = 0; z < lod - 1; ++z) {
                tris[triOffset] = vertOffset; 
                tris[triOffset + 1] = vertOffset + 1;
                tris[triOffset + 2] = vertOffset + lod;
                tris[triOffset + 3] = vertOffset + 1; 
                tris[triOffset + 4] = vertOffset + lod + 1;
                tris[triOffset + 5] = vertOffset + lod;

                ++vertOffset;
                triOffset += 6;
            }

            ++vertOffset;
        }
        
        return (verts, tris);
    }
}