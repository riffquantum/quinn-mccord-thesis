using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatePlanes : MonoBehaviour {
    public Texture[] textures;
    public int currentTexture;
    new Renderer renderer;
	// Use this for initialization
	void Start () {
        renderer = GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {

            currentTexture++;
            currentTexture %= textures.Length;
            renderer.material.mainTexture = textures[currentTexture];
            Debug.Log(currentTexture);
        }

        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentTexture--;
            renderer.material.mainTexture = textures[currentTexture];
            Debug.Log(currentTexture);
            

        }
    }
}
