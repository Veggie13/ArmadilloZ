using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateRadialPlane : MonoBehaviour {

	MeshFilter mf;
    const int radius = 50;
    const float rad60 = 1.047197f;
    const float edgesize = 0.5f;
    float triheight = edgesize * Mathf.Sin(rad60);
    int numvert = 0;
    Vector2[] planePoints;

    // Use this for initialization
    void Start () {

		Mesh mesh = new Mesh ();
		mf = GetComponent<MeshFilter> ();
		mf.mesh = mesh;

        int i;
        for (i = 0; i <= radius; i++)
			numvert = numvert + i;

		numvert = numvert * 6 + 1;
		int numtrianglepoints = radius * radius * 6 * 3; //Multiply by 3 since each triangle consists of 3 vertices

		var vertices = new Vector3[numvert];
        planePoints = new Vector2[numvert];
		int[] triangles = new int[numtrianglepoints];

		i = 0;

		for (int row = 0; row < (radius * 2 + 1); row++) 
		{
			for (int col = 0; col < ((radius * 2 + 1) - Mathf.Abs (radius - row)); col++) 
			{
				float x = (col + Mathf.Abs (radius - row) / 2f - radius) * edgesize;
				float z = (radius - row) * triheight;
                planePoints[i] = new Vector2(x, z);
				vertices [i] = new Vector3 (x, WaveField.GetAmplitude(x, z), z);
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
		//transform.gameObject.AddComponent<MeshCollider>();
		transform.GetComponent<MeshCollider>().sharedMesh = mesh;

        WaveField.forceMgr.Objects.Add(new WaveField.Movable()
        {
            Body = GameObject.Find("Sphere").GetComponent<Rigidbody>(),
            DragCoefficient = 0.7f,
            StopSpeed = 1f
        });
    }

	void Update () {

        Mesh mesh = mf.mesh;
        var vertices = new Vector3[mesh.vertexCount];
        for (int i = 0; i < vertices.Length; i++) 
		{
			float x = planePoints[i].x;
			float z = planePoints[i].y;
			vertices [i] = new Vector3 (x, WaveField.GetAmplitude(x, z), z);
		}

        mesh.vertices = vertices;
		mesh.RecalculateNormals();
        //GetComponent<MeshCollider> ().sharedMesh = null;
        //GetComponent<MeshCollider> ().sharedMesh = mesh;

        if (Input.GetMouseButtonUp(0))
        {
            var clickPos = Input.mousePosition;
            var ray = Camera.main.ScreenPointToRay(clickPos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                WaveField.forceMgr.Regions.Add(new WaveField.RadialWaveForceRegion()
                {
                    AnnularRegion = new WaveField.Annulus2()
                    {
                        Center = new Vector2(hit.point.x, hit.point.z),
                        InnerRadius = 2f,
                        OuterRadius = 4f,
                        MinArc = Mathf.PI,
                        MaxArc = Mathf.PI * (2f - 0.0f)
                    },
                    Center = new Vector2(hit.point.x, hit.point.z),
                    Speed = 5f,
                    UnitAmplitude = 2.4f,
                    UnitWaveAmplitude = 1f,
                    Wavelength = 0.5f
                });
                //WaveField.forceMgr.Regions.Add(new WaveField.RadialWaveForceRegion()
                //{
                //    AnnularRegion = new WaveField.Annulus2()
                //    {
                //        Center = new Vector2(hit.point.x, hit.point.z),
                //        InnerRadius = 2f,
                //        OuterRadius = 4f,
                //        MinArc = Mathf.PI * (2f - 0.3f),
                //        MaxArc = Mathf.PI * (2f - 0.2f)
                //    },
                //    Center = new Vector2(hit.point.x, hit.point.z),
                //    Speed = 5f,
                //    UnitAmplitude = 2.4f,
                //    UnitWaveAmplitude = 1f,
                //    Wavelength = 0.5f
                //});
            }
        }
    }

    void FixedUpdate()
    {
        WaveField.forceMgr.Tick(Time.fixedDeltaTime);
    }
}

