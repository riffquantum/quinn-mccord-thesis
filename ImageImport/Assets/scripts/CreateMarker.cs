using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRTK;

public class CreateMarker : MonoBehaviour {

    public GameObject MitosisMarker;
    public Transform TargetParent; //the Interaction ovject
    public static int frame = InstantiatePlanes.frameCounter;

	/* Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("p")) {
            frame = InstantiatePlanes.frameCounter;
            PlaceMarker(frame, new Vector3(0, 0, 0));
            }  
	}
    */
    public void PlaceMarker(int frameNumber, Vector3 pos)
    {
        //update frame from instantiate planes script
        frame = InstantiatePlanes.frameCounter;
        //instantiate new marker 
        GameObject newMarker = Instantiate(MitosisMarker, pos, Quaternion.identity, TargetParent); 
        //access tooltip comopnent and set display text to current frame
        VRTK_ObjectTooltip textTooltip = newMarker.GetComponent<VRTK_ObjectTooltip>();
        textTooltip.displayText = frameNumber.ToString();
        
        Debug.Log(frameNumber + " from createmarker script ");
    }
}
