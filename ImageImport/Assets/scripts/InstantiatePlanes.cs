﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text.RegularExpressions;

public class InstantiatePlanes : MonoBehaviour {

    
    public GameObject zPlaneObj;
    public Transform c1Plane;
    public Transform c2Plane;
	public Text UIfeedbackText;
    
    public Shader myShader;
    public Color[] channelColor;
    public Texture[, ,] textures;
    
    public float zScaleFactor = 100f;
	public int numPlanesPerTex = 10;
    public float xyScaleFactor = 120f;

    public int frameCounter = 1;
    
    public string folderName;
	public string baseName = "Susan_overnight";
	private string imgSuffix = "_c{0:d2}_t{1:d4}_z{2:d4}";

	private int maxChannel = 0;
	private int maxTime = 0;
	private int maxZ = 0;
    private int xDim = 0;
    private int yDim = 0;
    private float xPixSize = 0;
    private float yPixSize = 0;
    private float zPixSize = 0;


    private IEnumerator textureSettingCoroutine = null;

	public List<GameObject> allPlanes = new List<GameObject> ();

	private string getREGEXValueFromText(string txtContents, string matchstring) {
		Regex regChan = new Regex (matchstring, RegexOptions.IgnoreCase);
		Match matChan = regChan.Match (txtContents);
		Debug.Log (matChan.Groups [1].Value);
		return matChan.Groups[1].Value;
	}

    private int getIntValueFromText(string txtContents, string matchstring) {
        return int.Parse(getREGEXValueFromText(txtContents, matchstring));
    }

    private float getFloatValueFromText(string txtContents, string matchstring)
    {
        return float.Parse(getREGEXValueFromText(txtContents, matchstring));
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

		maxChannel = getIntValueFromText(txtContents, "NumberOfChannels:(\\d+)");
		maxTime = getIntValueFromText(txtContents, "NumberOfFrames:(\\d+)");
		maxZ = getIntValueFromText(txtContents, "ZDimension:(\\d+)");
        xDim = getIntValueFromText(txtContents, "XDimension:(\\d+)");
        yDim = getIntValueFromText(txtContents, "YDimension:(\\d+)");
        xPixSize = getFloatValueFromText(txtContents, "XPixelPhysicalSize:([\\d\\.]+)");
        yPixSize = getFloatValueFromText(txtContents, "YPixelPhysicalSize:([\\d\\.]+)");
        zPixSize = getFloatValueFromText(txtContents, "ZPixelPhysicalSize:([\\d\\.]+)");


        // Get max number of channels, time-values, and z-values from that file
        Debug.Log ("Read from file: channels " + maxChannel);
		Debug.Log ("Read from file: frames " + maxTime);
		Debug.Log ("Read from file: Z " + maxZ);
        Debug.Log("Read from file: xy dimensions " + xDim + " " + yDim);
        Debug.Log("Read from file: xyz pixel size " + xPixSize + " " + yPixSize + " " + zPixSize);

        yield return null;
		// Initialize the textures data structure to that size
		textures = new Texture[maxChannel, maxTime, maxZ];

		yield return StartCoroutine (LoadTextures (maxChannel, maxTime, maxZ));
	}

	private IEnumerator LoadTextures(int maxChannel, int maxTime, int maxZ)
    {
		int totalTexToLoad = maxTime * maxChannel * maxZ;
		int loadedNum = 0;
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
					loadedNum++;
					// texture loaded, wait then check if it's one that should be displayed ASAP
                    //yield return new WaitForSeconds(0.01f);
					yield return null;
                }
				if (UIfeedbackText != null) {
					UIfeedbackText.text = "Loaded " + loadedNum + " of " + totalTexToLoad + " textures";
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
        float zSpacing = zPixSize * zScaleFactor;
        // 10x10 default plane:
        float xPlaneScale = xPixSize * xyScaleFactor * xDim / 10f;
        float yPlaneScale = yPixSize * xyScaleFactor * yDim / 10f;
        
        /*// size and position the particle system:
        ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
        ParticleSystem.ShapeModule shap = ps.shape;
        shap.box = new Vector3(xPlaneScale * 10f, zSpacing * maxZ, yPlaneScale * 10f);
        ps.gameObject.transform.localPosition = new Vector3(0f, zSpacing * maxZ / 2f, 0f);
        */
        for (int chindex = 1; chindex <= maxChannel; chindex++)
		{
			GameObject channelObject = new GameObject ("Channel " + chindex);
			channelObject.transform.SetParent (gameObject.transform);
			for (int zindex = 1; zindex <= maxZ; zindex++)
			{
				GameObject zObject = new GameObject ("ZLevel " + zindex);
				zObject.transform.SetParent (channelObject.transform);
				Texture newTexture = textures [chindex - 1, frameCounter - 1, zindex - 1];
				while (newTexture == null) {
					Debug.Log ("CreatePlanes has to wait for texture loading");
					yield return new WaitForSeconds (0.01f);
					newTexture = textures [chindex - 1, frameCounter - 1, zindex - 1];
				}
				// positioning of planes:
				float newbasezValue = zSpacing * (zindex - 1) + (chindex - 1) * ((zSpacing / numPlanesPerTex) / 2);
				for (int i = 0; i < numPlanesPerTex; i++) {
					float newzValue = newbasezValue + (i * (zSpacing / numPlanesPerTex));
					GameObject instantiatedPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
					instantiatedPlane.name = "Plane ch " + chindex + " z " + zindex + " i " + i;
					allPlanes.Add (instantiatedPlane.gameObject);
					instantiatedPlane.transform.position = new Vector3(0, newzValue, 0);
					instantiatedPlane.transform.localScale = new Vector3(xPlaneScale, 0, yPlaneScale);
					Renderer rend = instantiatedPlane.GetComponent<Renderer>();
					rend.material.SetTexture("_MainTex", newTexture);
					rend.material.shader = myShader;
                    rend.material.color = channelColor[chindex];
					instantiatedPlane.transform.SetParent (zObject.transform);
                    
                    // test adding a static particle system in the same plane
                    

				}
				yield return null;
			}
		}   
		Debug.Log ("Created " + allPlanes.Count + " planes");
	}

    private IEnumerator SetTexturesToPlanes(int newFrameCounter)
    {
		while (maxZ == 0 || allPlanes.Count != (maxZ * maxChannel * numPlanesPerTex)) {
			Debug.Log ("Not all planes have been created yet");
			yield return new WaitForSeconds (0.01f);
		}
		List<GameObject>.Enumerator planeEnum = allPlanes.GetEnumerator();

        for (int chindex = 1; chindex <= maxChannel; chindex++)
        {
            for (int zindex = 1; zindex <= maxZ; zindex++)
            {
				Texture newTexture = textures[chindex - 1, newFrameCounter - 1, zindex - 1];
				if (newTexture == null) {
					Debug.Log ("SetTexturesToPlanes has to wait for texture loading (ch " + chindex + " z " + zindex + ")");
					while (newTexture == null) {
						yield return new WaitForSeconds (0.01f);
						newTexture = textures [chindex - 1, newFrameCounter - 1, zindex - 1];
					}
				}
				for (int i = 0; i < numPlanesPerTex; i++)
                {					
					//Debug.Log("chindex = " + chindex + ", zindex = " + zindex + ", numPlanesPerTex = " + i + ", frameCounter = " + newFrameCounter);
					// move the enum one forward, trusting that it contains exactly the right number of planes:
					planeEnum.MoveNext ();
					Renderer rend = planeEnum.Current.GetComponent<Renderer> ();
					rend.material.SetTexture("_MainTex", newTexture);
                }
            }
        }        
		yield return null;
		textureSettingCoroutine = null;
    }
    
    void Start () {
		
		StartCoroutine(ReadTexturesFromFolder());

		StartCoroutine(CreatePlanes());

    }
	
	// Update is called once per frame
	void Update () {
		if (maxZ == 0) {
			return;
		}
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {     
            frameCounter++;
            if (frameCounter > maxTime)
            {
                frameCounter = 1;
            }
			StartNewTextureCoroutine (frameCounter);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            frameCounter--;
            if (frameCounter < 1)
            {
                frameCounter = maxTime;
            }
			StartNewTextureCoroutine (frameCounter);
        }
    }
    void plusFC()
    {
        frameCounter++;
        if (frameCounter > maxTime)
        {
            frameCounter = 1;
        }
        StartNewTextureCoroutine(frameCounter);
    }
    void minusFC()
    {
        frameCounter--;
        if (frameCounter < 1)
        {
            frameCounter = maxTime;
        }
        StartNewTextureCoroutine(frameCounter);
    }

	void StartNewTextureCoroutine(int newFrameCounter) {
		Debug.Log("Setting frameCounter to " + frameCounter);
		if (textureSettingCoroutine != null) {
			Debug.Log ("Stopping existing texture setting coroutine");
			StopCoroutine (textureSettingCoroutine);
		}
		textureSettingCoroutine = SetTexturesToPlanes(frameCounter);
		StartCoroutine (textureSettingCoroutine);
	}
    
}
