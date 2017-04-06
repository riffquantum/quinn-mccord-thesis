using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System.IO;

public class InstantiatePlanes : MonoBehaviour {

    
    public Transform zPlane;
    public float zValue = 0.000981f;

    //public string folderName;
    //public string imageSequenceName;
    //private string baseName;

    public Material newMaterial;
    public Texture[] newTexture;
    

    /*
    //get number of images in folder
    public static long DirCount(DirectoryInfo d)
    {
        long i = 0;
        // Add file sizes.
        FileInfo[] fis = d.GetFiles();
        foreach (FileInfo fi in fis)
        {
            if (fi.Extension.Equals(".tif", StringComparison.OrdinalIgnoreCase))
                i++;
        }
        return i;
        

    }*/

    private void Awake()
    {
        //newMaterial = GetComponent<Material>();
        //baseName = folderName + "/" + imageSequenceName;
    }
    
    // Use this for initialization
    void Start () {
        for (int i= 0; i<= 24; i++)
        {
            float newzValue = zValue*i;
            //Debug.Log(newzValue);
            Texture newMat = newMaterial.mainTexture = newTexture[i];
            Instantiate(zPlane, new Vector3(0,newzValue,0), Quaternion.identity);
            

        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
