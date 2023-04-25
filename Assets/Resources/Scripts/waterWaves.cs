using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// wave structures and functions ///////////////////////
struct Wave
{
	public float freq;                     // 2*PI / wavelength
	public float amp;                      // amplitude
	public float phase;                        // speed * 2*PI / wavelength
	public Vector2 dir;

	public Wave(float initFreq, float initAmp, float initPhase, Vector2 initDir)
    {
		freq = initFreq;
		amp = initAmp;
		phase = initPhase;
		dir = initDir;
    }
};


public class waterWaves : MonoBehaviour
{
    MeshFilter waterMesh;


	[SerializeField] List<Vector3> meshVertices;
	[SerializeField] List<int> triangles;

	[Header("Water Plane Properties")]
	[SerializeField] Vector2 planeSize;
	[SerializeField] int planeResolution;


    // Start is called before the first frame update
    void Start()
    {
        waterMesh = gameObject.GetComponent<MeshFilter>();
		generateCustomePlane();
		setMesh();

	}

    // Update is called once per frame
    void FixedUpdate()
    {
		updateWaves();
	}



	float evaluateWave(Wave w, Vector4 pos, float t)
	{
		return w.amp * Mathf.Sin(Vector3.Dot(w.dir, pos) * w.freq + t * w.phase);
	}

	/// <summary>
	/// Update the waves mesh using sine waves.
	/// </summary>
	void updateWaves()
	{
		///////// TWEAKABLE PARAMETERS //////////////////
		Mesh updatedWaterMesh = waterMesh.mesh;
		Vector3[] meshVertices = new Vector3[updatedWaterMesh.vertices.Length];
		meshVertices = updatedWaterMesh.vertices;

		float WaveAmp = 1.01f;
		float WaveFreq = 0.3f;

		const int NWAVES = 4;
		Wave[] wave = new Wave[NWAVES]
		{
		new Wave(WaveFreq, WaveAmp, 0.5f, new Vector2(0.0f, 0.6f)) ,
		new Wave(WaveFreq * 2f, WaveAmp*0.5f, 1.3f, new Vector2(0.7f, 0.0f)),
		new Wave(WaveFreq, WaveAmp, 0.5f, new Vector2(0.1f, 0.2f)),
		new Wave(WaveFreq* 4f, WaveAmp * 0.5f, 1.3f, new Vector2(0.5f, 0.1f))
		};

		for (int i = 0; i < waterMesh.mesh.vertexCount; i++)
        {
			Vector4 Po = new Vector4(meshVertices[i][0], meshVertices[i][1], meshVertices[i][2], 1.0f);

			// sum waves	
			Po.y = 0.0f;
			// Compute y displacement and derivative for the waves defined above
			// Add Code Here (Compute y displacement and derivative)

			for (int j = 0; j < NWAVES; j++)
			{
				Po.y += evaluateWave(wave[j], Po, Time.time / 2);
			}
			meshVertices[i].y = Po.y;
		}
		
		updatedWaterMesh.vertices = meshVertices;


		waterMesh.mesh = updatedWaterMesh;
	}


	void generateCustomePlane()
    {
		meshVertices = new List<Vector3>();
		float xPerStep = planeSize.x / (planeResolution - 1);
		float yPerStep = planeSize.y / (planeResolution - 1);

		for (int y = 0; y < planeResolution; y++)
		{
			for (int x = 0; x < planeResolution; x++)
			{
				Vector3 vertex = new Vector3(x * xPerStep, 0, y * yPerStep);
				meshVertices.Add(vertex);
			}
		}

		for (int y = 0; y < planeResolution - 1; y++)
		{
			for (int x = 0; x < planeResolution - 1; x++)
			{
				int i = y * planeResolution + x;

				triangles.Add(i);
				triangles.Add(i + planeResolution + 1);
				triangles.Add(i + planeResolution);

				triangles.Add(i);
				triangles.Add(i + 1);
				triangles.Add(i + planeResolution + 1);
			}
		}


		for (int i = 0; i < triangles.Count; i += 3)
		{
			int temp = triangles[i + 1];
			triangles[i + 1] = triangles[i + 2];
			triangles[i + 2] = temp;
		}
	}


	void setMesh()
    {
		waterMesh.mesh.Clear();
		waterMesh.mesh.vertices = meshVertices.ToArray();
		waterMesh.mesh.triangles = triangles.ToArray();
		waterMesh.mesh.RecalculateNormals();



	}


}
