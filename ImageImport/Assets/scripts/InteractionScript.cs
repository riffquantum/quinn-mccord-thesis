using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionScript : MonoBehaviour {

    private InstantiatePlanes myInstantiatePlanes;
    
    //hide channel
    public void HideChannel(GameObject channelNum)
    {
        channelNum.SetActive(false); 
    }
}
