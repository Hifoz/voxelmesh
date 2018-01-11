using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// A Voxel Mesh generator 
/// </summary>
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class VoxelMeshGen2 : MonoBehaviour {
	private List<Vector3> vertices;
	private List<int> triangles;
	private Mesh mesh;
	private int[,,] pointmap;

	private Dictionary<Vector3, int> indicies;

	public enum FaceDirection {
		xp, xm, yp, ym, zp, zm
	}



	void Awake() {
		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		vertices = new List<Vector3>();
		triangles = new List<int>();
		indicies = new Dictionary<Vector3, int>();
	}

	void Start() {
		int[,,] points = new int[,,]{
			{ {1,0,1}, {0,1,0}, {1,0,1},},
			{ {0,1,0}, {1,0,1}, {0,1,0},},
			{ {1,0,1}, {0,1,0}, {1,0,1},}
		};


		
		GenerateCubeMesh(points);

		Debug.Log("2nd Number of vertices:" + vertices.Count);
	}


	/// <summary>
	/// Generates a mesh of cubes
	/// </summary>
	/// <param name="pointmap">data used to build cubes</param>
	public void GenerateCubeMesh(int[,,] pointmap) {
		this.pointmap = pointmap;
		for (int x = 0; x < pointmap.GetLength(0); x++) {
			for (int y = 0; y < pointmap.GetLength(1); y++) {
				for (int z = 0; z < pointmap.GetLength(2); z++) {
					if (pointmap[x, y, z] == 1)
						GenerateCube(new Vector3(x, y, z));
				}
			}
		}


		Recalculate();
	}



	/// <summary>
	/// Generates the mesh data for a cube
	/// </summary>
	/// <param name="cubePos">point position of the cube</param>
	public void GenerateCube(Vector3 cubePos) {
		if (cubePos.x == pointmap.GetLength(0) - 1 || pointmap[(int)cubePos.x + 1, (int)cubePos.y, (int)cubePos.z] == 0) GenerateCubeFace(FaceDirection.xp, cubePos);
		if (cubePos.y == pointmap.GetLength(1) - 1 || pointmap[(int)cubePos.x, (int)cubePos.y + 1, (int)cubePos.z] == 0) GenerateCubeFace(FaceDirection.yp, cubePos);
		if (cubePos.z == pointmap.GetLength(2) - 1 || pointmap[(int)cubePos.x, (int)cubePos.y, (int)cubePos.z + 1] == 0) GenerateCubeFace(FaceDirection.zp, cubePos);

		if (cubePos.x == 0 || pointmap[(int)cubePos.x - 1, (int)cubePos.y, (int)cubePos.z] == 0) GenerateCubeFace(FaceDirection.xm, cubePos);
		if (cubePos.y == 0 || pointmap[(int)cubePos.x, (int)cubePos.y - 1, (int)cubePos.z] == 0) GenerateCubeFace(FaceDirection.ym, cubePos);
		if (cubePos.z == 0 || pointmap[(int)cubePos.x, (int)cubePos.y, (int)cubePos.z - 1] == 0) GenerateCubeFace(FaceDirection.zm, cubePos);
	}

	/// <summary>
	/// Generates the mesh data for a face of a cube
	/// </summary>
	/// <param name="dir">direction of face</param>
	/// <param name="pointPos">point position of the cube</param>
	public void GenerateCubeFace(FaceDirection dir, Vector3 pointPos) {
		int vertIndex = vertices.Count;
		List<Vector3> triangleParts = new List<Vector3>();
		switch (dir) {
			case FaceDirection.xp:
				triangleParts.AddRange(new Vector3[]{pointPos + new Vector3(0.5f, -0.5f, -0.5f),
													 pointPos + new Vector3(0.5f,  0.5f, -0.5f),
													 pointPos + new Vector3(0.5f, -0.5f,  0.5f),
													 pointPos + new Vector3(0.5f,  0.5f,  0.5f)});
				break;
			case FaceDirection.xm:
				triangleParts.AddRange(new Vector3[]{pointPos + new Vector3(-0.5f, -0.5f, -0.5f),
													 pointPos + new Vector3(-0.5f, -0.5f,  0.5f),
													 pointPos + new Vector3(-0.5f,  0.5f, -0.5f),
													 pointPos + new Vector3(-0.5f,  0.5f,  0.5f)});
				break;
			case FaceDirection.yp:
				triangleParts.AddRange(new Vector3[]{pointPos + new Vector3(-0.5f, 0.5f, -0.5f),
													 pointPos + new Vector3(-0.5f, 0.5f,  0.5f),
													 pointPos + new Vector3(0.5f,  0.5f, -0.5f),
													 pointPos + new Vector3(0.5f,  0.5f,  0.5f)});
				break;
			case FaceDirection.ym:
				triangleParts.AddRange(new Vector3[]{pointPos + new Vector3(-0.5f, -0.5f, -0.5f),
													 pointPos + new Vector3(0.5f,  -0.5f, -0.5f),
													 pointPos + new Vector3(-0.5f, -0.5f,  0.5f),
													 pointPos + new Vector3(0.5f,  -0.5f,  0.5f)});
				break;
			case FaceDirection.zp:
				triangleParts.AddRange(new Vector3[]{pointPos + new Vector3(-0.5f, -0.5f, 0.5f),
													 pointPos + new Vector3(0.5f,  -0.5f, 0.5f),
													 pointPos + new Vector3(-0.5f,  0.5f, 0.5f),
													 pointPos + new Vector3(0.5f,   0.5f, 0.5f)});
				break;
			case FaceDirection.zm:
				triangleParts.AddRange(new Vector3[]{pointPos + new Vector3(-0.5f, -0.5f, -0.5f),
											 		 pointPos + new Vector3(-0.5f,  0.5f, -0.5f),
													 pointPos + new Vector3(0.5f,  -0.5f, -0.5f),
													 pointPos + new Vector3(0.5f,   0.5f, -0.5f)});
				break;
		}


		foreach(int idx in new int[] {0, 1, 2, 2, 1, 3 })
			triangles.Add(GetVertex(triangleParts[idx]));
	}


	private int GetVertex(Vector3 pos) {
		if (indicies.ContainsKey(pos)) {
			return indicies[pos];
		}
		else {
			int newI = vertices.Count;
			vertices.Add(pos);
			indicies.Add(pos, newI);
			return newI;
		}
	}



	/// <summary>
	/// Clears the mesh
	/// </summary>
	private void ClearMesh() {
		vertices.Clear();
		triangles.Clear();
		mesh.Clear();
		Recalculate();
	}


	/// <summary>
	/// Recalculates the mesh
	/// </summary>
	private void Recalculate() {
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.RecalculateNormals();
	}

}
