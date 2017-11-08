using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;


public class DB_Script : MonoBehaviour {

	// Use this for initialization
	void Start () {
        string conn = "URI=file:" + Application.dataPath + "/Susan_overnight.LEVER"; //Path to database.
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        Debug.Log("opening database");
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();
        string sqlQuery = "SELECT cellID,verts,edges,normals, faces " + "FROM tblCells where cellID == 1 ";
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


            Debug.Log("cellID= " + cellID + " verts =" + verts + "  edges =" + edges + " normals =" + normals + " faces=" + faces);
            break;
        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
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
        //Debug.Log(result);
        return result;
         
    }

}
