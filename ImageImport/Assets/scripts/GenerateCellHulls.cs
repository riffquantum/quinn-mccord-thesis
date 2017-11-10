using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(MeshFilter))]
public class GenerateCellHulls : MonoBehaviour {
    public DB_Script myDBScript;
    public float size = 1;
    private MeshFilter filter;
    
	// Use this for initialization
	void Start () {
        filter = GetComponent<MeshFilter>();
        filter.mesh = GenerateMesh();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    Mesh GenerateMesh()
    {
        //for cellID = 1 
        Mesh mesh = new Mesh();
        for (int i = 0; i < myDBScript.verticesFloats.Length; i += 3){
            mesh.SetVertices(new List<Vector3>()
        {

                //for each set of DB_Script vertices, 244 vertices for cellID = 1
                
                new Vector3(-size * 0.5f, 0, -size * 0.5f),
                new Vector3(size * 0.5f, 0, -size * 0.5f),
                new Vector3(size * 0.5f, 0, size * 0.5f),
                new Vector3(-size * 0.5f, 0, size * 0.5f)

        });
        }// end for loop

        /*
        mesh.SetTriangles(new List<int>()
        {
            3,1,0,
            3,2,1,
        },0);

        mesh.SetNormals(new List<Vector3>()
        {
            Vector3.down,
            Vector3.down,
            Vector3.down,
            Vector3.down,
        });
        */
        return mesh;
    }
    
}
