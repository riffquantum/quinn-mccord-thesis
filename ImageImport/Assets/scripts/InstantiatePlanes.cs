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
    public Color[] channelColor;
    public Texture[, ,] textures;
    public Texture currentTexture;
    public float zSpacing = 0.9f;
	public int numPlanesPerTex = 10;


    public int frameCounter = 1;
    
    public string folderName;
	public string baseName = "Susan_overnight";
	private string imgSuffix = "_c{0:d2}_t{1:d4}_z{2:d4}";

	private int maxChannel = 0;
	private int maxTime = 0;
	private int maxZ = 0;

	private int getREGEXValueFromText(string txtContents, string matchstring) {
		Regex regChan = new Regex (matchstring, RegexOptions.IgnoreCase);
		Match matChan = regChan.Match (txtContents);
		Debug.Log (matChan.Groups [1].Value);
		return int.Parse (matChan.Groups [1].Value);
	}

	public IEnumerator ReadTexturesFromFolder()
	{
		yield return null;		
		// Read .txt file
		//TODO read whole contents of text file
		string filename = folderName + "/" + baseName;
		Debug.Log (filename);
		TextAsset txtAssets = Resources.Load<TextAsset> (filename);
		string txtContents = txtAssets.text;

		maxChannel = getREGEXValueFromText (txtContents, "NumberOfChannels:(\\d+)");
		maxTime = getREGEXValueFromText (txtContents, "NumberOfFrames:(\\d+)");
		maxZ = getREGEXValueFromText (txtContents, "ZDimension:(\\d+)");

        

        // Get max number of channels, time-values, and z-values from that file
        Debug.Log ("Read from file: channels " + maxChannel);
		Debug.Log ("Read from file: frames " + maxTime);
		Debug.Log ("Read from file: Z " + maxZ);
        
		yield return null;
		// Initialize the textures data structure to that size
		textures = new Texture[maxChannel, maxTime, maxZ];

		yield return StartCoroutine (LoadTextures (maxChannel, maxTime, maxZ));
	}

	private IEnumerator LoadTextures(int maxChannel, int maxTime, int maxZ)
    {
		// Go through files and read in textures
		for (int timeindex = 1; timeindex <= maxTime; timeindex++)
		{
			for (int chindex = 1; chindex <= maxChannel; chindex++)
			{
                for (int zindex = 1; zindex <= maxZ; zindex++)
				{
					//construct the filename:
					string currentTexFilename = folderName + "/" + baseName + string.Format(imgSuffix, chindex, timeindex, zindex);
					//Debug.Log("Trying to load texture " + currentTexFilename);
					//load Resource Texture And assign it
					Texture newtexture = Resources.Load<Texture>(currentTexFilename);
					//Debug.Log ("Loaded " + newtexture);
					textures[chindex - 1, timeindex - 1, zindex - 1] = newtexture;
					// texture loaded, wait then check if it's one that should be displayed ASAP
                    yield return new WaitForSeconds(0.01f);
                }
			}            
        }
        // Then you'll be able to access any texture with three indices:
        //Texture newTex = textures[channelnum, timenum, znum];

    }
		

	private IEnumerator CreatePlanes()
    {
		while (maxZ == 0) {
			yield return new WaitForSeconds (0.1f);
		}
		for (int chindex = 1; chindex <= maxChannel; chindex++)
		{
			for (int zindex = 1; zindex <= maxZ; zindex++)
			{
				Texture newTexture = textures [chindex - 1, frameCounter - 1, zindex - 1];
				while (newTexture == null) {
					Debug.Log ("CreatePlanes has to wait for texture loading");
					yield return new WaitForSeconds (0.01f);
				}
				// positioning of planes:
				float newbasezValue = zSpacing * (zindex - 1) + (chindex - 1) * ((zSpacing / numPlanesPerTex) / 2);
				for (int i = 0; i < numPlanesPerTex; i++) {
					float newzValue = newbasezValue + (i * (zSpacing / numPlanesPerTex));
					GameObject c1plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
					c1plane.transform.position = new Vector3(0, newzValue, 0);
					c1plane.transform.localScale = new Vector3(512, 0, 512);
					Renderer rend = c1plane.GetComponent<Renderer>();
					rend.material.SetTexture("_MainTex", newTexture);
					rend.material.shader = myShader;
					
                    rend.material.color = channelColor[chindex];
                    


					yield return null;
				}
			}
		}            
	}

    private IEnumerator SetTexturesToPlanes(int frameCounter)
    {

        int thisFrameCounter = frameCounter;
        for (int chindex = 1; chindex <= maxChannel; chindex++)
        {
            for (int zindex = 1; zindex <= maxZ; zindex++)
            {
               
                for (int i = 0; i < numPlanesPerTex; i++)
                {
                    Texture newTexture = textures[chindex - 1, thisFrameCounter-1, zindex - 1];

                    Renderer rend = GetComponent<Renderer>();
                    rend.material.SetTexture("_MainTex", newTexture);
                    
                    while (newTexture == null)
                    {
                        Debug.Log("SetTexturesToPlanes has to wait for texture loading");
                        
                        yield return new WaitForSeconds(0.01f);
                    }

                }
            }
            
        }
        
    }

	void Start () {
		
		StartCoroutine(ReadTexturesFromFolder());

		StartCoroutine(CreatePlanes());

    }
	
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            
            frameCounter++;
            if (frameCounter > maxTime)
            {
                frameCounter = 1;
            }
            Debug.Log(frameCounter);
			StartCoroutine(SetTexturesToPlanes(frameCounter));
            Debug.Log("running set texture co routine");
        }

        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            frameCounter--;
            if (frameCounter < 1)
            {
                frameCounter = maxTime;
            }
            Debug.Log(frameCounter);
			StartCoroutine(SetTexturesToPlanes(frameCounter));
        }


    }

    
}
