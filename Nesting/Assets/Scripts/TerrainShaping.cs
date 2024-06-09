using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Attach this to an empty gameobject. It will do the work for you.
public class TerrainShaping : MonoBehaviour
{
	private int xScale, yScale;
	private float resolution = 0.88f;
	public GameObject fence;
	private float height = 1.75f;
	public float wallmargin = 3.5f;
	public Material material;
	[Range(-0.2f,0.2f)]
	public float colliderOffset;
	
	private int chunkDivisions = 2;
	private MeshFilter meshfilter;
	private MeshRenderer meshrenderer;
	private MeshCollider meshcollider;
	private Mesh mesh;
	private GameObject ground;
	private GameObject offsetGround;
	
    // Start is called before the first frame update
    void Awake()
    {
		Random.InitState(BirdDetails.birdid);
		BirdDetails.mapx = Random.Range(60,120);
		BirdDetails.mapy = Random.Range(66,125);
		BirdDetails.mapz = Random.Range(0.75f,5.5f);
		//Extra width is needed to accomodate the hexagons sides
		BirdDetails.hexBuffer = Mathf.RoundToInt( BirdDetails.mapy / 3.33f ) * 2;
		xScale = BirdDetails.mapx + BirdDetails.hexBuffer;
		yScale = BirdDetails.mapy;
		height = BirdDetails.mapz;
		ground = new GameObject();
		ground.name = "Ground";
		ground.transform.parent = this.transform;
		ground.layer = 8;
		
		meshfilter = ground.AddComponent<MeshFilter>();
		meshrenderer = ground.AddComponent<MeshRenderer>();
		//meshcollider = ground.AddComponent<MeshCollider>();
		meshfilter.mesh = FormMesh();
		meshrenderer.material = material;
		//meshcollider.sharedMesh = meshfilter.mesh;
		ground.transform.position = new Vector3( -BirdDetails.hexBuffer, 0, 0);		
		//Move the ground collider from the ground render
		if (colliderOffset != 0) {
			offsetGround = new GameObject();
			MeshFilter offsetMeshfilter = offsetGround.AddComponent<MeshFilter>();
			offsetMeshfilter.mesh = meshfilter.mesh;
			MeshCollider offsetMeshcollider = offsetGround.AddComponent<MeshCollider>();
			offsetMeshcollider.sharedMesh = meshfilter.mesh;
			offsetGround.transform.parent = this.transform;
			offsetGround.transform.position = ground.transform.position + new Vector3(0,colliderOffset,0);
			offsetGround.layer = 8;
			offsetGround.name = "Ground Collider";
		} else {
			meshcollider = ground.AddComponent<MeshCollider>();
			meshcollider.sharedMesh = meshfilter.mesh;
		}
		CreateBoundries(wallmargin);
		if (Debug.isDebugBuild) {
			Debug.Log($"Ground created. x:{xScale}, y:{yScale}, height:{height}");
		}
    }

	Mesh FormMesh() {
		Mesh tempMesh = new Mesh();
		tempMesh.name = "Terrain";
		int truexScale = xScale+1;
		int trueyScale = yScale+1;
		Vector3[] verts = new Vector3[truexScale*trueyScale];
		Vector2[] uv = new Vector2[verts.Length];
		int[] tris = new int[xScale * yScale * 6];
		
		//Create vertices with perlin noise heightmap
		float yf = 0;
		float perlinOffset = Random.value;
		for (int i = 0; yf <= yScale*2; yf += 2) {
			for (float xf = 0; xf <= xScale*2; xf += 2, i++) {
				float noisex = perlinOffset + xf / ((float)xScale * (resolution/3f));
				float noisey = perlinOffset + yf / ((float)yScale * (resolution/3f));
				float h = Mathf.PerlinNoise(noisex,noisey)*height - (height/2f);
				verts[i] = new Vector3(xf, h, yf);
				//Debug.Log(verts[i]);
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

	// Create colliders at the edges - no falling off the map!
	void CreateBoundries(float margin = 1) {
		int ylimit = Mathf.CeilToInt( yScale * 2 - margin) ;
		int yrepeat = Mathf.CeilToInt( ( ylimit ) / 6.5f);
		int xlimit = Mathf.CeilToInt(BirdDetails.mapx * 2 - margin);
		float angleDistMod = 1.65f;
		
		GameObject northwall = new GameObject();
		northwall.name = "Northwall";
		northwall.transform.parent = this.transform;
		for (int i=0; i <= (xlimit/6.5f); i++) {
			GameObject go = Instantiate(fence, new Vector3(margin+(i*6.5f), -2.5f, ylimit), Quaternion.identity) as GameObject;
			go.transform.parent = northwall.transform;
		}
		
		GameObject southwall = new GameObject();
		southwall.name = "Southwall";
		southwall.transform.parent = this.transform;
		for (int i=0; i <= (xlimit/6.5f); i++) {
			GameObject go = Instantiate(fence, new Vector3(margin+(i*6.5f), -2.5f, margin), Quaternion.identity) as GameObject;
			go.transform.parent = southwall.transform;
		}
		
		//EASRSIDE WALLS
		GameObject seastwall = new GameObject();
		Transform seastwallT = seastwall.transform;
		seastwallT.position = new Vector3(xlimit,0,margin);
		seastwall.name = "SEastwall";
		seastwallT.parent = this.transform;
		for (int i=0; i <= yrepeat/angleDistMod; i++) {
			GameObject go = Instantiate(fence, new Vector3(xlimit, -2.5f, margin+(i*6.5f)), Quaternion.Euler(0, 90, 0)) as GameObject;
			go.transform.parent = seastwallT;
		}	
		seastwallT.eulerAngles = new Vector3(0, 30, 0);
		
		GameObject neastwall = new GameObject();
		Transform neastwallT = neastwall.transform;
		neastwallT.position = new Vector3(xlimit,0, ylimit);
		neastwall.name = "NEastwall";
		neastwallT.parent = this.transform;
		for (int i=0; i <= yrepeat/angleDistMod; i++) {
			GameObject go = Instantiate(fence, new Vector3(xlimit, -2.5f, ylimit-(i*6.5f)), Quaternion.Euler(0, 90, 0)) as GameObject;
			go.transform.parent = neastwallT;
		}	
		neastwallT.eulerAngles = new Vector3(0, 330, 0);
		
		//WESTSIDE WALLS
		GameObject swestwall = new GameObject();
		Transform swestwallT = swestwall.transform;
		swestwallT.position = new Vector3(margin,0,margin);
		swestwall.name = "SWestwall";
		swestwallT.parent = this.transform;
		for (int i=0; i <= yrepeat/angleDistMod; i++) {
			GameObject go = Instantiate(fence, new Vector3(margin, -2.5f, margin+(i*6.5f)), Quaternion.Euler(0, 90, 0)) as GameObject;
			go.transform.parent = swestwallT;
		}	
		swestwallT.eulerAngles = new Vector3(0, 330, 0);
		
		GameObject nwestwall = new GameObject();
		Transform nwestwallT = nwestwall.transform;
		nwestwallT.position = new Vector3(margin,0,ylimit);
		nwestwall.name = "NWestwall";
		nwestwallT.parent = this.transform;
		for (int i=0; i <= yrepeat/angleDistMod; i++) {
			GameObject go = Instantiate(fence, new Vector3(margin, -2.5f, ylimit-(i*6.5f)), Quaternion.Euler(0, 90, 0)) as GameObject;
			go.transform.parent = nwestwallT;
		}	
		nwestwallT.eulerAngles = new Vector3(0, 30, 0);
	}
}
