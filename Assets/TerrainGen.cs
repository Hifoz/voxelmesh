using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TerrainGen : MonoBehaviour {
    private Mesh mesh;
    private List<Vector3> vertices;
    private List<int> triangles;


    private enum direction{ // xp = x plus, ym = y minus, etc..
        xp, xm, yp, ym, zp, zm
    }

	void Start () {
        GetComponent<MeshFilter>().mesh = this.mesh = new Mesh();
        mesh.name = "3DTerrainMesh";
        vertices = new List<Vector3>();
        triangles = new List<int>();
	}

    // For testing the generation really simply
    public void testGenerator() {
        int[,,] mapdata = new int[,,] {
            {{1,0,1}, {0,1,0}, {1,0,1}},
            {{0,1,0}, {1,0,1}, {0,1,0}},
            {{1,0,1}, {0,1,0}, {1,0,1}}
        };

        Clear();
        generateMesh(mapdata);
        ReCalculate();
    }


    public void generateMesh(int[,,] pointdata) {
        for (int x = 0; x < pointdata.GetLength(0); x++) {
            for (int y = 0; y < pointdata.GetLength(1); y++) {
                for (int z = 0; z < pointdata.GetLength(2); z++) {
                    generateFace(direction.xp, new int[] { x, y, z }, pointdata[x, y, z], x < pointdata.GetLength(0) - 1 ? pointdata[x + 1, y, z] : 0);
                    generateFace(direction.yp, new int[] { x, y, z }, pointdata[x, y, z], y < pointdata.GetLength(1) - 1 ? pointdata[x, y + 1, z] : 0);
                    generateFace(direction.zp, new int[] { x, y, z }, pointdata[x, y, z], z < pointdata.GetLength(2) - 1 ? pointdata[x, y, z + 1] : 0);
                    generateFace(direction.xm, new int[] { x, y, z }, pointdata[x, y, z], x > 0 ? pointdata[x - 1, y, z] : 0);
                    generateFace(direction.ym, new int[] { x, y, z }, pointdata[x, y, z], y > 0 ? pointdata[x, y - 1, z] : 0);
                    generateFace(direction.zm, new int[] { x, y, z }, pointdata[x, y, z], z > 0 ? pointdata[x, y, z - 1] : 0);
                }
            }
        }
    }

    private void generateFace(direction facedir, int[] position, int value, int othervalue) {
        if (value == 0 || othervalue != 0)
            return;

        int vi = vertices.Count;

        Vector3[] offsets = getPositionOffset(facedir);

        vertices.Add(offsets[0] + new Vector3(position[0], position[1], position[2]));
        vertices.Add(offsets[1] + new Vector3(position[0], position[1], position[2]));
        vertices.Add(offsets[2] + new Vector3(position[0], position[1], position[2]));
        vertices.Add(offsets[3] + new Vector3(position[0], position[1], position[2]));

        triangles.AddRange(new int[] {
            vi, vi + 1, vi + 2,
            vi + 2, vi + 1, vi + 3
        });

    }

    // Returns a list with how the 4 points for a face is offset from the center point
    private Vector3[] getPositionOffset(direction facedir) {
        Vector3[] offsets = new Vector3[4];
        switch (facedir) {
            case direction.xp:
                offsets[0] = new Vector3(0.5f, -0.5f, -0.5f);
                offsets[1] = new Vector3(0.5f, 0.5f, -0.5f);
                offsets[2] = new Vector3(0.5f, -0.5f, 0.5f);
                offsets[3] = new Vector3(0.5f, 0.5f, 0.5f);
                break;
            case direction.xm:
                offsets[0] = new Vector3(-0.5f, -0.5f, -0.5f);
                offsets[1] = new Vector3(-0.5f, -0.5f, 0.5f);
                offsets[2] = new Vector3(-0.5f, 0.5f, -0.5f);
                offsets[3] = new Vector3(-0.5f, 0.5f, 0.5f);
                break;
            case direction.yp:
                offsets[0] = new Vector3(-0.5f, 0.5f, -0.5f);
                offsets[1] = new Vector3(-0.5f, 0.5f, 0.5f);
                offsets[2] = new Vector3(0.5f, 0.5f, -0.5f);
                offsets[3] = new Vector3(0.5f, 0.5f, 0.5f);
                break;
            case direction.ym:
                offsets[0] = new Vector3(-0.5f, -0.5f, -0.5f);
                offsets[1] = new Vector3(0.5f, -0.5f, -0.5f);
                offsets[2] = new Vector3(-0.5f, -0.5f, 0.5f);
                offsets[3] = new Vector3(0.5f, -0.5f, 0.5f);
                break;
            case direction.zp:
                offsets[0] = new Vector3(-0.5f, -0.5f, 0.5f);
                offsets[1] = new Vector3(0.5f, -0.5f, 0.5f);
                offsets[2] = new Vector3(-0.5f, 0.5f, 0.5f);
                offsets[3] = new Vector3(0.5f, 0.5f, 0.5f);
                break;
            case direction.zm:
                offsets[0] = new Vector3(-0.5f, -0.5f, -0.5f);
                offsets[1] = new Vector3(-0.5f, 0.5f, -0.5f);
                offsets[2] = new Vector3(0.5f, -0.5f, -0.5f);
                offsets[3] = new Vector3(0.5f, 0.5f, -0.5f);
                break;
        }

        return offsets;
    }

    // Recalculates mesh data
    private void ReCalculate() {
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }

    public void Clear() {
        mesh.Clear();
        vertices.Clear();
        triangles.Clear();
    }
}
