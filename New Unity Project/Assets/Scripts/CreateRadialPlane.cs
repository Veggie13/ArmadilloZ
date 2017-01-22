using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateRadialPlane : MonoBehaviour {

	MeshFilter mf;

	// Use this for initialization
	void Start () {

		Mesh mesh = new Mesh ();
		mf = GetComponent<MeshFilter> ();
		mf.mesh = mesh;
		int radius = 100;
		float rad60 = 1.047197f;
	    float edgesize = 0.25f;
		float triheight = edgesize * Mathf.Sin (rad60);
		int numvert = 0;
		int i;
		float amplitude = 0.1f;
		float kmultiplier = 10f;

		for (i = 0; i <= radius; i++)
			numvert = numvert + i;

		numvert = numvert * 6 + 1;
		int numtrianglepoints = radius * radius * 6 * 3; //Multiply by 3 since each triangle consists of 3 vertices

		Vector3[] vertices = new Vector3[numvert];
		int[] triangles = new int[numtrianglepoints];

		i = 0;

		for (int row = 0; row < (radius * 2 + 1); row++) 
		{
			for (int col = 0; col < ((radius * 2 + 1) - Mathf.Abs (radius - row)); col++) 
			{
				float x = (col + Mathf.Abs (radius - row) / 2f - radius) * edgesize;
				float z = (radius - row) * triheight;
				float s = Mathf.Sqrt (x * x + z * z);
				//float y = amplitude * Mathf.Cos (kmultiplier * s);
				vertices [i] = new Vector3 (x, 0, z);
				i++;

			}
		}

		i = 0;
		int tri = 0;

		for (int row = 0; row < radius; row++) 	//triangles for loop 1
		{										 
			for (int col = 0; col < (radius + row); col++)
			{
				triangles [tri] = i;
				triangles [tri+1] = i+1;
				triangles [tri+2] = i+row+radius+2;
				triangles [tri+3] = i;
				triangles [tri+4] = i+row+radius+2;
				triangles [tri+5] = i+row+radius+1;
				tri = tri + 6;
				i++;
			}
		triangles [tri] = i;					//tack on upper rows
		triangles [tri+1] = i+row+radius+2;
		triangles [tri+2] = i+row+radius+1;
		tri = tri + 3;
		i++;
		}

		for(int row = radius; row < radius*2; row++) 	//triangles for loop 2
		{										 
			for (int col = 0; col < (radius*2 - (row-radius) - 1); col++)
			{
				triangles [tri] = i;
				triangles [tri+1] = i+1;
				triangles [tri+2] = i+(3*radius)+1-row;
				triangles [tri+3] = i+1;
				triangles [tri+4] = i+1+(3*radius)+1-row;
				triangles [tri+5] = i+1+(3*radius)-row;
				tri = tri + 6;
				i++;
			}
			triangles [tri] = i;					//tack on bottom rows
			triangles [tri+1] = i+1;
			triangles [tri+2] = i+(3*radius)+1-row;
			tri = tri + 3;
			i++;
			i++;
		}

		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
		transform.gameObject.AddComponent<MeshCollider>();
		transform.GetComponent<MeshCollider>().sharedMesh = mesh;

	}

	void Update () {

//		Mesh mesh = mf.mesh;
//		int radius = 100;
//		float rad60 = 1.047197f;
//		float edgesize = 0.25f;
//		float triheight = edgesize * Mathf.Sin (rad60);
//		int numvert = 0;
//		int i;
//		float amplitude = 0.2f;
//		float kmultiplier = 5f;
//
//		for (i = 0; i <= radius; i++)
//			numvert = numvert + i;
//
//		numvert = numvert * 6 + 1;
//		int numtrianglepoints = radius * radius * 6 * 3; //Multiply by 3 since each triangle consists of 3 vertices
//
//		Vector3[] vertices = new Vector3[numvert];
//		int[] triangles = new int[numtrianglepoints];
//
//		i = 0;
//
//		for (int row = 0; row < (radius * 2 + 1); row++) 
//		{
//			for (int col = 0; col < ((radius * 2 + 1) - Mathf.Abs (radius - row)); col++) 
//			{
//				float x = (col + Mathf.Abs (radius - row) / 2f - radius) * edgesize;
//				float z = (radius - row) * triheight;
//				float s = Mathf.Sqrt (x * x + z * z);
//				float y = amplitude * Mathf.Cos (kmultiplier * s - (Time.fixedTime));
//				vertices [i] = new Vector3 (x, y, z);
//				i++;
//
//			}
//		}
//
//		mesh.vertices = vertices;
//		mesh.RecalculateNormals();
	//	GetComponent<MeshCollider> ().sharedMesh = null;
	//	GetComponent<MeshCollider> ().sharedMesh = mesh;
	}
}

