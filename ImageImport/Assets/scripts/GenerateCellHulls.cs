using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(MeshFilter))]
public class GenerateCellHulls : MonoBehaviour {
    public float size = 1;
    private MeshFilter filter;
    public DBScript myBDScript;

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
        Mesh mesh;
        mesh.SetVertices(new List<Vector3>()
        {
            //for each cellID
                //for length of myDBScript vertices
                new Vector3(vertX,vertY,vertZ),
        });
        return mesh;
    }
}
