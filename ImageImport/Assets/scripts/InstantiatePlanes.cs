using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;

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
	private string imgSuffix = "_c{0:d2}_t{1:d4}_z{2:d4}";
	public TextAsset txtFile;


	/*public void HandleTextFile{
		
		static void ReadString(){
			string path = "Assets/Resources/Susan_overnight/Susan_overnight.txt";
			//Read text
			StreamReader reader = new StreamReader(path);
			Debug.Log (reader.ReadToEnd ());
			reader.Close ();
		}
	}
	*/

	public IEnumerator ReadTexturesFromFolder()
    {
		yield return null;
		Debug.Log (txtFile);

		// Read .txt file
		//TODO read whole contents of text file
		string filename = folderName + "/" + baseName;
		Debug.Log (filename);
		TextAsset txtAssets = Resources.Load<TextAsset>(filename);
		string txtContents = txtAssets.text;
		Regex r = new Regex("NumberOfChannels:(\\d+)", RegexOptions.IgnoreCase);
		Match m = r.Match(txtContents);
		Debug.Log (m.Groups[1].Value);
		int maxChannel = int.Parse(m.Groups[1].Value);
		// same for maxtime maxz


		// Get max number of channels, time-values, and z-values from that file
		Debug.Log("Read from file: channels " + maxChannel);
		//int maxChannel = 2; //numChan
		int maxTime = 50; //numTime
		int maxZ = 25; //numZ
		yield return null;
		// Initialize the textures data structure to that size
		this.textures = new Texture[maxChannel,maxTime,maxZ];

		// Go through files and read in textures
		for (int chindex = 1; chindex <= maxChannel; chindex++)
		{
			for (int timeindex = 1; timeindex <= maxTime; timeindex++)
			{
				for (int zindex = 1; zindex <= maxZ; zindex++)
				{
					//construct the filename:
					string currentTexFilename = folderName + "/" + baseName + string.Format(imgSuffix, chindex, timeindex, zindex);
					Debug.Log("Trying to load texture " + currentTexFilename);
					//load Resource Texture And assign it
					Texture newtexture = Resources.Load<Texture>(currentTexFilename);
					Debug.Log ("Loaded " + newtexture);
					textures[chindex - 1, timeindex - 1, zindex - 1] = newtexture;
					yield return new WaitForSeconds(0.1f);
					// now check if this texture is one that should be displayed right now: is time=0???
					// if so, set it on the right material/planes
				}
			}
		}


		// Then you'll be able to access any texture with three indices:
		//newTex = textures[channelnum, timenum, znum]

    }

    private void Awake()
    {
        //newMaterial = GetComponent<Material>();
        //baseName = folderName + "/" + imageSequenceName;
    }
    
    // Use this for initialization
    void Start () {
		//HandleTextFile ();
		StartCoroutine(ReadTexturesFromFolder());
		/*
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

           

        }*/
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
