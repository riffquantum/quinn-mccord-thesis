using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageSequenceSingleTexture : MonoBehaviour 
    {
    //texture object that will output animation, gameobject's material, frames
    private Texture texture;
    private Material goMaterial;
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
        goMaterial = GetComponent<Material>();
        
        baseName = folderName + "/" + imageSequenceName;
    }
    
	// Use this for initialization
	void Start ()
    {
        texture = (Texture)Resources.Load(baseName + "0", typeof(Texture));
	}
	
	// Update is called once per frame
	void Update () {
        //start 'playloop' method as coroutine with 0.04 delay
        StartCoroutine("PlayLoop", 0.04f);
        //set materials texcture to current value of frameCount
        goMaterial.mainTexture = texture; 
	}

    IEnumerator PlayLoop(float delay)
    {
        //wait for the time defined at delay param
        yield return new WaitForSeconds(delay);
        //advance one frame
        frameCounter = (++frameCounter) % numberofFrames;
        //load current frame
        texture = (Texture)Resources.Load(baseName + frameCounter.ToString("D5"), typeof(Texture));
        //stop coroutine
        StopCoroutine("PlayLoop");
    }
}
