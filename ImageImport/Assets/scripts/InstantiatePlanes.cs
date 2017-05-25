using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;

public class InstantiatePlanes : MonoBehaviour {

    
    public GameObject zPlaneObj;
    public Transform c1Plane;
    public Transform c2Plane;


    public Shader myShader;
    public Color myColor;
    public Color myColor2;
    public Texture[, ,] textures;
    public Texture currentTexture;
    public float zValue;


    int frameCounter = 0;
    
    public string folderName;
	public string baseName = "Susan_overnight";
	private string imgSuffix = "_c{0:d2}_t{1:d4}_z{2:d4}";
	


	

	public IEnumerator ReadTexturesFromFolder()
    {
		yield return null;
		
		// Read .txt file
		//TODO read whole contents of text file
		string filename = folderName + "/" + baseName;
		Debug.Log (filename);
		TextAsset txtAssets = Resources.Load<TextAsset>(filename);
		string txtContents = txtAssets.text;

        Regex regChan = new Regex("NumberOfChannels:(\\d+)", RegexOptions.IgnoreCase);
		Match matChan = regChan.Match(txtContents);
		Debug.Log (matChan.Groups[1].Value);
		int maxChannel = int.Parse(matChan.Groups[1].Value);
        
        Regex regTime = new Regex("NumberOfFrames:(\\d+)", RegexOptions.IgnoreCase);
        Match matTime = regTime.Match(txtContents);
        Debug.Log(matChan.Groups[1].Value);
        int maxTime = int.Parse(matTime.Groups[1].Value);

        Regex regZ = new Regex("ZDimension:(\\d+)", RegexOptions.IgnoreCase);
        Match matZ = regZ.Match(txtContents);
        Debug.Log(matZ.Groups[1].Value);
        int maxZ = int.Parse(matZ.Groups[1].Value);

        // Get max number of channels, time-values, and z-values from that file
        Debug.Log("Read from file: channels " + maxChannel);
        Debug.Log("Read from file: frames " + maxTime);
        Debug.Log("Read from file: Z " + maxZ);
        
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
                    
                    yield return new WaitForSeconds(0.01f);
                    // now check if this texture is one that should be displayed right now: is time=0???
                    // if so, set it on the right material/planes


                    if (chindex == 1)
                    {
                        GameObject c1plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                        c1plane.transform.position = new Vector3(0, 0, 0);
                        c1plane.transform.localScale = new Vector3(512, 0, 512);
                        Renderer rend = c1plane.GetComponent<Renderer>();
                        rend.material.SetTexture("_MainTex", textures[chindex - 1, timeindex - 1, zindex - 1]);
                        rend.material.shader = myShader;
                        rend.material.color = myColor;
                    }else if (chindex == 2)
                    {
                        GameObject c2plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                        c2plane.transform.position = new Vector3(0, 0, 0);
                        c2plane.transform.localScale = new Vector3(512, 0, 512);
                        Renderer rend = c2plane.GetComponent<Renderer>();
                        rend.material.SetTexture("_MainTex", newtexture);
                        rend.material.shader = myShader;
                        rend.material.color = myColor2;
                    }
                    /*
                    if (frameCounter == 0 && timeindex == 1)
                    {
                        currentTexture = textures[0, 0, 0];
                    }
                    */
                }
			}
            
        }
        
        // Then you'll be able to access any texture with three indices:
        //newTex = textures[channelnum, timenum, znum]

    }

    public void LoadPlanes()
    {
        for (int i = 0; i < 25; i++)
        {
            float newzValue = zValue * i;

            for (int j = 0; j <= 19; j++)
            {
                float dupZVal = newzValue + j;
                GameObject c1plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                c1plane.transform.position = new Vector3(0, dupZVal, 0);
                c1plane.transform.localScale = new Vector3(512, 0, 512);
                Renderer rend = GetComponent<Renderer>();
                


                GameObject c2plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                c2plane.transform.position = new Vector3(0, dupZVal, 0);
                c2plane.transform.localScale = new Vector3(512, 0, 512);
                Renderer c2rend = GetComponent<Renderer>();
                



            }



        }

    }
    // Use this for initialization
    void Start () {
		
		StartCoroutine(ReadTexturesFromFolder());

        //LoadPlanes();

           

        }
	
	
	// Update is called once per frame
	void Update () {


        
        
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {

            frameCounter++;
            Debug.Log(frameCounter);
        }

        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            frameCounter--;
            Debug.Log(frameCounter);
        }


    }

    
}
