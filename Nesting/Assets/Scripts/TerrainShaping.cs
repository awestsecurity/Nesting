using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Attach this to an empty gameobject. It will do the work for you.
public class TerrainShaping : MonoBehaviour
{
	private int xScale, yScale;
	public float resolution = 1f;
	public GameObject fence;
	private float height = 0.5f;
	public float wallmargin = 3.5f;
	public Material material;
	
	private MeshFilter meshfilter;
	private MeshRenderer meshrenderer;
	private MeshCollider meshcollider;
	private Mesh mesh;
	
    // Start is called before the first frame update
    void Awake()
    {
		Random.InitState(BirdDetails.birdid);
		BirdDetails.mapx = Random.Range(115,240);
		BirdDetails.mapy = Random.Range(125,250);
		BirdDetails.mapz = Random.Range(0.75f,2f);
		xScale = BirdDetails.mapx;
		yScale = BirdDetails.mapy;
		height = BirdDetails.mapz;
		meshfilter = gameObject.AddComponent<MeshFilter>();
		meshrenderer = gameObject.AddComponent<MeshRenderer>();
		meshcollider = gameObject.AddComponent<MeshCollider>();
        meshfilter.mesh = FormMesh();
		meshrenderer.material = material;
		meshcollider.sharedMesh = meshfilter.mesh;
		CreateBoundries(wallmargin);
    }

	Mesh FormMesh() {
		Mesh tempMesh = new Mesh();
		tempMesh.name = "Terrain";
		Vector3[] verts = new Vector3[(xScale+1)*(yScale+1)];
		Vector2[] uv = new Vector2[verts.Length];
		int[] tris = new int[xScale * yScale * 6];
		
		//Create vertices with perlin noise heightmap
		float yf = 0;
		float perlinOffset = Random.value;
		for (int i = 0; yf <= yScale; yf += resolution) {
			for (float xf = 0; xf <= xScale; xf += resolution, i++) {
				float noisex = perlinOffset + xf / ((float)xScale * (resolution/3f));
				float noisey = perlinOffset + yf / ((float)yScale * (resolution/3f));
				float h = Mathf.PerlinNoise(noisex,noisey)*height - (height/2f);
				verts[i] = new Vector3(xf, h, yf);
				uv[i] = new Vector2(xf / xScale, yf / yScale);
			}
		}
		tempMesh.vertices = verts;
		tempMesh.uv = uv;
		
		//Sort our triangles
		for (int ti = 0, vi = 0, y = 0; y < yScale; y++, vi++) {
			for (int x = 0; x < xScale; x++, ti += 6, vi++) {
				tris[ti] = vi;
				tris[ti + 3] = tris[ti + 2] = vi + 1;
				tris[ti + 4] = tris[ti + 1] = vi + xScale + 1;
				tris[ti + 5] = vi + xScale + 2;
			}
		}
		tempMesh.triangles = tris;
		tempMesh.RecalculateNormals();
		return tempMesh;
	}

	// Create box colliders at the square edges - no falling off the map!
	void CreateBoundries(float margin = 1) {
		GameObject northwall = new GameObject();
		northwall.name = "Northwall";
		northwall.transform.parent = this.transform;
		for (int i=0; i <= ((xScale-(margin))/6.5f); i++) {
			GameObject go = Instantiate(fence, new Vector3(margin+(i*6.5f), -2.5f, yScale-margin), Quaternion.identity) as GameObject;
			go.transform.parent = northwall.transform;
		}
		
		GameObject southwall = new GameObject();
		southwall.name = "Southwall";
		southwall.transform.parent = this.transform;
		for (int i=0; i <= ((xScale-(margin))/6.5f); i++) {
			GameObject go = Instantiate(fence, new Vector3(margin+(i*6.5f), -2.5f, margin), Quaternion.identity) as GameObject;
			go.transform.parent = southwall.transform;
		}
		
		GameObject eastwall = new GameObject();
		eastwall.name = "Eastwall";
		eastwall.transform.parent = this.transform;
		for (int i=0; i <= ((yScale-(margin))/6.5f); i++) {
			GameObject go = Instantiate(fence, new Vector3(xScale-margin, -2.5f, margin+(i*6.5f)), Quaternion.Euler(0, 90, 0)) as GameObject;
			go.transform.parent = eastwall.transform;
		}	
		
		GameObject westwall = new GameObject();
		westwall.name = "Westwall";
		westwall.transform.parent = this.transform;
		for (int i=0; i <= ((yScale-(margin))/6.5f); i++) {
			GameObject go = Instantiate(fence, new Vector3(margin, -2.5f, margin+(i*6.5f)), Quaternion.Euler(0, 90, 0)) as GameObject;
			go.transform.parent = westwall.transform;
		}	
	}
}
