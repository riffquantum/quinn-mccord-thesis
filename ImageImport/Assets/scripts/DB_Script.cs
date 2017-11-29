using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;


public class DB_Script : MonoBehaviour {
    public int frameCounter = 1;
    public GameObject HullsParent;
    public GameObject HullPrefab;
    private IDbConnection dbconn;

    public void CreateHullObjects(int frameCounter) {
        // first get rid of previous hull objects:
        GameObject[] HullsToDelete;
        HullsToDelete = GameObject.FindGameObjectsWithTag("Hull");
        foreach (GameObject child in HullsToDelete)
        {
            
                Destroy(child);
            
        }

        this.frameCounter = frameCounter;
        QueryDBForHulls();
    }

    private void CreateSingleHull(int hullNr, List<Vector3> vertices, List<Vector3> normals, int[] faceInts) {
        Mesh mesh = new Mesh();
        mesh.SetVertices(vertices);
        mesh.SetNormals(normals);
        mesh.SetTriangles(faceInts, 0);
        // create new game object with parent HullsParent for the mesh:
        GameObject newGO = Instantiate(HullPrefab, HullsParent.transform);
        newGO.name = "Hull" + hullNr;
        newGO.tag = "Hull";
        newGO.layer = 5;
        newGO.GetComponent<MeshFilter>().mesh = mesh;
        
    }

    void Start() {
        string conn = "URI=file:" + Application.dataPath + "/Susan_overnight.LEVER"; //Path to database.
        dbconn = (IDbConnection)new SqliteConnection(conn);
        Debug.Log("opening database");
        dbconn.Open(); //Open connection to the database.
        CreateHullObjects(frameCounter);
    }

    void OnApplicationQuit() {
        dbconn.Close();
        dbconn = null;
    }

    void QueryDBForHulls() { 
        IDbCommand dbcmd = dbconn.CreateCommand();
        string sqlQuery = "SELECT cellID,verts,edges,normals,faces FROM tblCells where channel == 1 AND time == " + frameCounter;
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {
            int cellID = reader.GetInt32(0); 
            byte[] verts = GetBytes(reader, 1);
            byte[] edges = GetBytes(reader, 2); 
            byte[] normals = GetBytes(reader, 3);
            byte[] faces = GetBytes(reader, 4);
            // edges should be a list of integers, two each for one edge:
            Debug.Log("Length of edges: " + edges.Length);
            int[] edgeInts = BytesToInts(edges);
            //Debug.Log("edgeInts: len " + edgeInts.Length + " min " + edgeInts.Min() + " max " + edgeInts.Max());
            //verts should be a list of floats, three for x,y,z coordinates
            Debug.Log("Length of verts: " + verts.Length);
            List<Vector3> vertFloats = FloatsToVector3s(BytesToFloats(verts));
            //Debug.Log("vertFloats: len " + vertFloats.Length + " min " + vertFloats.Min() + " max " + vertFloats.Max());
            //faces should be a list of ints, three for each triangle
            Debug.Log("Length of faces: " + faces.Length);
            int[] faceInts = BytesToInts(faces);
            Debug.Log("faceInts: len " + faceInts.Length + " min " + faceInts.Min() + " max " + edgeInts.Max());
            //normals???
            List<Vector3> normalFloats = FloatsToVector3s(BytesToFloats(normals));

            Debug.Log("cellID= " + cellID + " verts =" + verts + "  edges =" + edges + " normals =" + normals + " faces=" + faces);
            CreateSingleHull(cellID, vertFloats, normalFloats, faceInts);
        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
    }

    private int[] BytesToInts(byte[] bytes)
    {
        int STEPSIZE = 4;
        int[] result = new int[bytes.Length / STEPSIZE];
        for (int i = 0; i < bytes.Length; i += STEPSIZE)
        {
            result[i / STEPSIZE] = BitConverter.ToInt32(bytes, i);
        }
        return result;
    }

    private float[] BytesToFloats(byte[] bytes)
    {
        int STEPSIZE = 4;
        float[] result = new float[bytes.Length / STEPSIZE];
        for (int i = 0; i < bytes.Length; i += STEPSIZE)
        {
            result[i / STEPSIZE] = BitConverter.ToSingle(bytes, i);
        }
        return result;
    }

    private List<Vector3> FloatsToVector3s(float[] floats)
    {
        int STEPSIZE = 3;
        List<Vector3> result = new List<Vector3>(floats.Length / STEPSIZE);
        for (int i = 0; i < floats.Length; i += STEPSIZE)
        {
            result.Add(new Vector3(floats[i], floats[i + 1], floats[i + 2]));
        }
        return result;
    }


    /// <summary>
    /// Reads all available bytes from reader
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="ordinal"></param>
    /// <returns></returns>
    private byte[] GetBytes(IDataReader reader, int ordinal)
    {
        byte[] result = null;

        if (!reader.IsDBNull(ordinal))
        {
            long size = reader.GetBytes(ordinal, 0, null, 0, 0); //get the length of data 
            result = new byte[size];
            int bufferSize = 1024;
            long bytesRead = 0;
            int curPos = 0;
            while (bytesRead < size)
            {
                bytesRead += reader.GetBytes(ordinal, curPos, result, curPos, bufferSize);
                curPos += bufferSize;
            }
        }
      
        return result;
         
    }

}
