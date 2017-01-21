using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatePlaneMesh : MonoBehaviour {

	public float width = 50f;
	public float length = 50f;

	// Use this for initialization
	void Start () {

		MeshFilter mf = GetComponent<MeshFilter>();
		Mesh mesh = new Mesh ();
		mf.mesh = mesh;

		Vector3[] vertices = new Vector3[] 
		{
			new Vector3 (0, 0, 0),
			new Vector3 (width, 0, 0),
			new Vector3 (width, 0, length),
			new Vector3 (0, 0, length)
		};

		int[] tri = new int[]{ 2, 1, 0, 3, 2, 0 };

		Vector3[] normals = new Vector3[4];

		normals[0] = Vector3.up;
		normals[1] = Vector3.up;
		normals[2] = Vector3.up;
		normals[3] = Vector3.up;


		Vector2[] uv = new Vector2[] 
		{
			new Vector2(0,0),	
			new Vector2(0,1),
			new Vector2(1,1),
			new Vector2(1,0)
		};

		mesh.vertices = vertices;
		mesh.triangles = tri;
		mesh.uv = uv;
		mesh.normals = normals;

		transform.gameObject.AddComponent<MeshCollider>();
		transform.GetComponent<MeshCollider>().sharedMesh = mesh;


	}

}
