using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPivot : MonoBehaviour {
    public Transform PivotPointLocation;
    enum Controller { LTouch, RTouch };
    
    public Vector3 LControllerPosition;
    public Vector3 RControllerPosition;

    
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        LControllerPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
        RControllerPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
    }

    public void LeftSetPivotLocation()
    {
        if (OVRInput.Axis1D.SecondaryHandTrigger == 0)
        {
            PivotPointLocation.transform.position = LControllerPosition;
            Debug.Log(PivotPointLocation.transform.position);
        }
        
    }
    public void RightSetPivotLocation()
    {
        if (OVRInput.Axis1D.PrimaryHandTrigger == 0)
        {
            PivotPointLocation.transform.position = RControllerPosition;
            Debug.Log(PivotPointLocation.transform.position);
        }
        
    }

    public void Release()
    {
        Debug.Log("Released");
    }

}

