using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackPadRotate : MonoBehaviour {
	/*SteamVR_Controller.DeviceType device;
	SteamVR_TrackedObject trackedObj;

	public GameObject wheelL;
	public GameObject wheelR;

	public float multiplier = 30f;

	Vector2 touchpad;

	private float sensitivity = 3.5f;

	// Use this for initialization
	void Start () {
		trackedObj = GetComponent<SteamVR_TrackedObject> ();
		device = SteamVR_Controller.Input ((int)trackedObj.index);

	}
	
	// Update is called once per frame
	void Update () {
		float tiltAroundX = device.GetAxis (Valve.VR.EVRButtonId.k_EButton_Axis0).x;
		float tiltAroundY = device.GetAxis (Valve.VR.EVRButtonId.k_EButton_Axis0).y;


		//Left controller rotates object
		if (device.GetTouch (SteamVR_Controller.ButtonMask.Touchpad)) {
			wheelL.transform.Rotate (0, 0, tiltAroundX);
		}
		if (device.GetTouchUp (SteamVR_Controller.ButtonMask.TouchPad)) {
			wheelL.transform.Rotate (0, 0, 0);
		}

		//Right controller moves time
		if (device.GetTouch (SteamVR_Controller.ButtonMask.Touchpad)) {
			wheelR.transform.Rotate (0, 0, tiltAroundX);
		}
		if (device.GetTouchUp (SteamVR_Controller.ButtonMask.TouchPad)) {
			wheelR.transform.Rotate (0, 0, 0);
		}
	}*/
}
