using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosestPointForCollider : MonoBehaviour {
    public Collider coll;
    public Transform focusObject;

    public Vector3 closestPoint
    {
        get
        {
            if (coll == null || focusObject == null)
            {
                return Vector3.zero;
            }
            return coll.ClosestPoint(focusObject.position);
        }
    }

	void Start () {
        if (coll == null)
        {
            this.coll = GetComponent<Collider>();
        }		
	}
	
}
