using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageSequenceSingleTexture : MonoBehaviour 
    {

    private InstantiatePlanes myInstantiator;
    //texture object that will output animation, gameobject's material, frames
    private Texture texture;
    public Material newMaterial;
    private int frameCounter = 0;
    

    //
    public string folderName;
    public string imageSequenceName;
    //number of frames in animation - the t value from images
    public int numberofFrames;
    //base name of files in sequence
    private string baseName;


    private void Awake()
    {
        newMaterial = GetComponent<Material>();
        
        baseName = folderName + "/" + imageSequenceName;
    }
    
	// Use this for initialization
	void Start ()
    {
        texture = (Texture)Resources.Load(baseName + "0", typeof(Texture));
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            //if (frameCounter == numberofFrames)
           // {
                //frameCounter = 0;
                frameCounter++;
                Debug.Log(frameCounter);
            //}
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {

           // if (frameCounter == 0)
           // {
              //  frameCounter = numberofFrames;
                frameCounter--;
                Debug.Log(frameCounter);
           // }
            
        }

        //start 'playloop' method as coroutine with 0.04 delay
        StartCoroutine("PlayLoop");
        //set materials texcture to current value of frameCount
        newMaterial.mainTexture = this.texture; 
	}

    IEnumerator PlayLoop(float delay)
    {
        //wait for the time defined at delay param
        yield return new WaitForSeconds(delay);
        //advance one frame
        frameCounter = (++frameCounter) % numberofFrames;
        //load current frame
        texture = (Texture)Resources.Load(baseName + frameCounter.ToString("D5"), typeof(Texture));
		newMaterial.mainTexture = this.texture; 
        
    }
}
