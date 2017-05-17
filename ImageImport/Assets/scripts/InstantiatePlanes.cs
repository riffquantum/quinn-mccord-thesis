using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System.IO;

public class InstantiatePlanes : MonoBehaviour {

    private ImageSequenceSingleTexture mySequence;
    public GameObject zPlaneObj;
    public Transform c1Plane;
    public Transform c2Plane;

    public Texture[, ,] textures;
    public int currentTexture;
    public float zValue;


    //int frameCounter;
    //int numberofFrames = 25;


    public Material[] c1Material;
    //public Texture[] c1Texture;
    public Material[] c2Material;
    //public Texture[] c2Texture;

    public string folderName;
	public string baseName = "Susan_overnight";
	private string imgSuffix = "_c{0:d2}_t{1:d4}_z{2:d4}.tif";

    
	public void ReadTexturesFromFolder()
    {
		// Read .txt file
		//TODO read whole contents of text file
		string filename = folderName + baseName + ".txt";


		// Get max number of channels, time-values, and z-values from that file
		int maxChannel = 2;
		int maxTime = 50;
		int maxZ = 25;

		// Initialize the textures data structure to that size
		this.textures = new Texture[maxChannel,maxTime,maxZ];

		// Go through files and read in textures
		for channels
			for times
				for Zvalues
					//construct the filename:
					string currentTexFilename = folderName + baseName + String.Format(imgSuffix, chnum, timenum, znum);
					load Resource Texture And assign it
					textures[channelnum, timenum, znum] = newtexture;
		
		// Then you'll be able to access any texture with three indices:
		//newTex = textures[channelnum, timenum, znum]

        // Add file sizes.
        /*FileInfo[] fis = d.GetFiles();
        foreach (FileInfo fi in fis)
        {
            if (fi.Extension.Equals(".tif", StringComparison.OrdinalIgnoreCase))
                //i++;
        }
        */


    }

    private void Awake()
    {
        //newMaterial = GetComponent<Material>();
        //baseName = folderName + "/" + imageSequenceName;
    }
    
    // Use this for initialization
    void Start () {

		// ReadTexturesFromFolder();

        for (int i= 0; i< c1Material.Length; i++)
        {
            float newzValue = zValue * i;
            

            //Instantiate(c1Plane, new Vector3(0, newzValue, 0), Quaternion.identity);


            //Instantiate(c2Plane, new Vector3(0, newzValue, 0), Quaternion.identity);

            for (int j = 0; j <= 19; j++) 
            {
                float dupZVal = newzValue + j;
                GameObject c1plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                c1plane.transform.position = new Vector3(0, dupZVal, 0);
                c1plane.transform.localScale = new Vector3(1, 0, 1);
                Renderer rend = c1plane.GetComponent<Renderer>();
                rend.material = new Material(c1Material[i]);


                GameObject c2plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                c2plane.transform.position = new Vector3(0, dupZVal, 0);
                c2plane.transform.localScale = new Vector3(1, 0, 1);
                Renderer c2rend = c2plane.GetComponent<Renderer>();
                c2rend.material = new Material(c2Material[i]);

              

            }

           

        }
	}
	
	// Update is called once per frame
	void Update () {

       /*
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {

            currentTexture++;
            currentTexture %= textures.Length;
            
            rend.material.mainTexture = textures[currentTexture];
            Debug.Log(currentTexture);
        }

        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {





        }
        */

    }

    
}
