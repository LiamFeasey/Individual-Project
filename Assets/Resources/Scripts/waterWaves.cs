using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;



/// <summary>
/// The wave struct that stores the details about different waves.
/// Will be used later on when evaluating waves, gives better control over the wave shapes, sizes, and frequencies.
/// </summary>
struct Wave
{
	public float freq;      // 2*PI / wavelength
	public float amp;       // amplitude
	public float phase;		// speed * 2*PI / wavelength
	public Vector2 dir;		// Direction

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
	/// <summary>
	/// The mesh for the water that will have the waves algorithm applied to it.
	/// </summary>
    MeshFilter waterMesh;

	[Tooltip("The coordinates of the vertices for the generated water mesh")]
	[SerializeField] List<Vector3> meshVertices;

	[Tooltip("The indices that make up the triangles for the generated water mesh")]
	[SerializeField] List<int> triangles;

	[Header("Water Plane Properties")]

	[Tooltip("The dimensions of the water plane, changes how big the mesh will be. " +
        "The larger this is the worse the resolution will be as vertices will spread out")]
	[SerializeField] Vector2 planeSize;

	[Tooltip("The resolution of the mesh vertices," +
        " a larger number means more vertices will be used to increae accuracy and smoothness of the water and waves.")]
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
        Vector3[] meshVertices = waterMesh.mesh.vertices;
        float time = Time.time;
        Thread calcWaves = new Thread(() => updateWaves(meshVertices, time));
        calcWaves.Start();
        calcWaves.Join();
        waterMesh.mesh.vertices = meshVertices;
    }



	/// <summary>
	/// Eveluates the wave using a sin wave and returns the modified y level for the provided vertex
	/// </summary>
	/// <param name="w">The Wave being evaluated</param>
	/// <param name="pos">The position the current vertex is at</param>
	/// <param name="t">The current time</param>
	/// <returns>The calculated Y value for this vertex in the wave</returns>
	float evaluateWave(Wave w, Vector4 pos, float t)
	{
		return w.amp * Mathf.Sin(Vector3.Dot(w.dir, pos) * w.freq + t * w.phase);
	}

	/// <summary>
	/// Iterates through every vertex in the water mesh and evaluates their Y position using the provided wave formula(s).
	/// The vertices and time are passed in instead of just referencing the values stored in the class because this function
	/// will be called on a different thread for performance reasons.
	/// </summary>
	/// <param name="inVertices">The array of Vertices that represents the current state of the water mesh</param>
	/// <param name="time">The current time of the program</param>
	public void updateWaves(Vector3[] inVertices, float time)
	{
		///////// TWEAKABLE PARAMETERS //////////////////
		Vector3[] meshVertices = new Vector3[inVertices.Length];
		meshVertices = inVertices;

		float WaveAmp = 0.3f;
		float WaveFreq = 0.1f;

		const int NWAVES = 4;
		Wave[] wave = new Wave[NWAVES]
		{
		new Wave(WaveFreq, WaveAmp, 0.5f, new Vector2(0.0f, 0.6f)) ,
		new Wave(WaveFreq * 2f, WaveAmp*0.5f, 1.3f, new Vector2(0.7f, 0.0f)),
		new Wave(WaveFreq, WaveAmp * 2.0f, 0.5f, new Vector2(0.1f, 0.2f)),
		new Wave(WaveFreq* 4f, WaveAmp * 0.5f, 1.3f, new Vector2(0.5f, 0.1f))
		};

		for (int i = 0; i < meshVertices.Length; i++)
        {
			Vector4 Po = new Vector4(meshVertices[i][0], meshVertices[i][1], meshVertices[i][2], 1.0f);

			// sum waves	
			Po.y = 0.0f;
			// Compute y displacement for the waves

			for (int j = 0; j < NWAVES; j++)
			{
				Po.y += evaluateWave(wave[j], Po, time / 2);
			}
			meshVertices[i].y = Po.y;
		}

		inVertices = meshVertices;
	}

	/// <summary>
	/// Generates a plane mesh based on the parameters stored in the waterWaves class.
	/// Such as resolution and dimensions.
	/// </summary>
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

	/// <summary>
	/// Updates the water mesh's vertices and triangles with the new lists.
	/// </summary>
	void setMesh()
    {
		waterMesh.mesh.Clear();
		waterMesh.mesh.vertices = meshVertices.ToArray();
		waterMesh.mesh.triangles = triangles.ToArray();
		waterMesh.mesh.RecalculateNormals();
	}
}