using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A Voxel Mesh generator 
/// </summary>
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class VoxelMeshGen : MonoBehaviour {
	private List<Vector3> vertices;
	private List<int> triangles;
	private List<Color> colors;
    private Mesh mesh;
	private int[,,] pointmap;

	public enum FaceDirection {
		xp, xm, yp, ym, zp, zm
	}



	void Awake() {
		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		mesh.MarkDynamic();
		vertices = new List<Vector3>();
		triangles = new List<int>();
		colors = new List<Color>();
    }

	void Start() {
		//int[,,] points = new int[,,]{ { {1,0,1}, {0,1,0}, {1,0,1},}, { {0,1,0}, {1,0,1}, {0,1,0},}, { {1,0,1}, {0,1,0}, {1,0,1},} }; // 3d "checkerboard" of cubes

		int[,,] points2 = new int[16, 21, 16]; // Maximum amount of cubes that can be contained in a single mesh in worst case scenario (3d "checkerboard") is 16*16*21

		for (int i = 0; i < 16; i++) {
            for (int j = 0; j < 21; j++) {
                for (int k = 0; k < 16; k++) {
                    points2[i, j, k] = Random.Range(0, 10000) % 3;
				}
			}
		}


		GenerateMesh(points2);

		Debug.Log("Number of vertices:" + vertices.Count);
	}


	private void Update() {
		if (Input.GetKey(KeyCode.U))
			Regenerate();

	}

	public void Regenerate() {
		ClearMesh();
		GenerateMesh(this.pointmap);
	}



	/// <summary>
	/// Generates a mesh of cubes
	/// </summary>
	/// <param name="pointmap">data used to build cubes</param>
	public void GenerateMesh(int[,,] pointmap) {
		this.pointmap = pointmap;
		for (int x = 0; x < pointmap.GetLength(0); x++) {
			for (int y = 0; y < pointmap.GetLength(1); y++) {
				for (int z = 0; z < pointmap.GetLength(2); z++) {
					if (pointmap[x, y, z] != 0)
						GenerateCube(new Vector3(x, y, z), pointmap[x,y,z]);
				}
			}
		}


		Recalculate();
	}



    /// <summary>
    /// Generates the mesh data for a cube
    /// </summary>
    /// <param name="cubePos">point position of the cube</param>
    public void GenerateCube(Vector3 cubePos, int cubetype) {
		if (cubePos.x == pointmap.GetLength(0) - 1 || pointmap[(int)cubePos.x + 1, (int)cubePos.y, (int)cubePos.z] == 0) GenerateCubeFace(FaceDirection.xp, cubePos, cubetype);
		if (cubePos.y == pointmap.GetLength(1) - 1 || pointmap[(int)cubePos.x, (int)cubePos.y + 1, (int)cubePos.z] == 0) GenerateCubeFace(FaceDirection.yp, cubePos, cubetype);
		if (cubePos.z == pointmap.GetLength(2) - 1 || pointmap[(int)cubePos.x, (int)cubePos.y, (int)cubePos.z + 1] == 0) GenerateCubeFace(FaceDirection.zp, cubePos, cubetype);
		if (cubePos.x == 0 || pointmap[(int)cubePos.x - 1, (int)cubePos.y, (int)cubePos.z] == 0) GenerateCubeFace(FaceDirection.xm, cubePos, cubetype);
		if (cubePos.y == 0 || pointmap[(int)cubePos.x, (int)cubePos.y - 1, (int)cubePos.z] == 0) GenerateCubeFace(FaceDirection.ym, cubePos, cubetype);
		if (cubePos.z == 0 || pointmap[(int)cubePos.x, (int)cubePos.y, (int)cubePos.z - 1] == 0) GenerateCubeFace(FaceDirection.zm, cubePos, cubetype);
	}

	/// <summary>
	/// Generates the mesh data for a face of a cube
	/// </summary>
	/// <param name="dir">direction of face</param>
	/// <param name="pointPos">point position of the cube</param>
	public void GenerateCubeFace(FaceDirection dir, Vector3 pointPos, int cubetype) {
		int vertIndex = vertices.Count;
		switch (dir) {
			case FaceDirection.xp:
				vertices.AddRange(new Vector3[]{pointPos + new Vector3(0.5f, -0.5f, -0.5f),
												pointPos + new Vector3(0.5f,  0.5f, -0.5f),
												pointPos + new Vector3(0.5f, -0.5f,  0.5f),
												pointPos + new Vector3(0.5f,  0.5f,  0.5f)});
				break;
			case FaceDirection.xm:
				vertices.AddRange(new Vector3[]{pointPos + new Vector3(-0.5f, -0.5f, -0.5f),
												pointPos + new Vector3(-0.5f, -0.5f,  0.5f),
												pointPos + new Vector3(-0.5f,  0.5f, -0.5f),
												pointPos + new Vector3(-0.5f,  0.5f,  0.5f)});
				break;
			case FaceDirection.yp:
				vertices.AddRange(new Vector3[]{pointPos + new Vector3(-0.5f, 0.5f, -0.5f),
												pointPos + new Vector3(-0.5f, 0.5f,  0.5f),
												pointPos + new Vector3(0.5f,  0.5f, -0.5f),
												pointPos + new Vector3(0.5f,  0.5f,  0.5f)});
				break;
			case FaceDirection.ym:
				vertices.AddRange(new Vector3[]{pointPos + new Vector3(-0.5f, -0.5f, -0.5f),
												pointPos + new Vector3(0.5f,  -0.5f, -0.5f),
												pointPos + new Vector3(-0.5f, -0.5f,  0.5f),
												pointPos + new Vector3(0.5f,  -0.5f,  0.5f)});
				break;
			case FaceDirection.zp:
				vertices.AddRange(new Vector3[]{pointPos + new Vector3(-0.5f, -0.5f, 0.5f),
												pointPos + new Vector3(0.5f,  -0.5f, 0.5f),
												pointPos + new Vector3(-0.5f,  0.5f, 0.5f),
												pointPos + new Vector3(0.5f,   0.5f, 0.5f)});
				break;
			case FaceDirection.zm:
				vertices.AddRange(new Vector3[]{pointPos + new Vector3(-0.5f, -0.5f, -0.5f),
												pointPos + new Vector3(-0.5f,  0.5f, -0.5f),
												pointPos + new Vector3(0.5f,  -0.5f, -0.5f),
												pointPos + new Vector3(0.5f,   0.5f, -0.5f)});
				break;
		}
		triangles.AddRange(new int[] { vertIndex, vertIndex + 1, vertIndex + 2 });
		triangles.AddRange(new int[] { vertIndex + 2, vertIndex + 1, vertIndex + 3 });

        if (cubetype == 1)
            colors.AddRange(Enumerable.Repeat(Color.green, 4).ToArray());
        else if (cubetype == 2)
            colors.AddRange(Enumerable.Repeat(Color.red, 4).ToArray());

	}




	/// <summary>
	/// Clears the mesh
	/// </summary>
	private void ClearMesh() {
		vertices.Clear();
		triangles.Clear();
        colors.Clear();
		mesh.Clear();
	}


	/// <summary>
	/// Recalculates the mesh
	/// </summary>
	private void Recalculate() {
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
        mesh.colors = colors.ToArray();
		mesh.RecalculateNormals();
	}

}
