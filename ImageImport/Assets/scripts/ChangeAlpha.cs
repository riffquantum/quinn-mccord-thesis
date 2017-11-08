using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeAlpha : MonoBehaviour {

    //public float red, green, blue, alpha;
    public float channelAlpha;//, chan2Alpha;
    
    public void Chan1Change (float value)
    {
        this.channelAlpha = value;
        //this.red = value;
        SetColor();
    }

    /*
    public void Chan2Change(float value)
    {
        this.chan2Alpha = value;
        //this.green = value;
        SetColor();
    }

    
    public void Chan3Change(float value)
    {
        //this.chan2Alpha = value;
        this.blue = value;
        SetColor();
    }

    public void Chan4Change(float value)
    {
        //this.chan2Alpha = value;
        this.alpha = value;
        SetColor();
    }
    */
    public void SetColor()
    {

        GameObject chan1 = GameObject.Find("Channel 1");
        chan1.GetComponentInChildren<Renderer>().material.color = new Color(0, 0, 0, channelAlpha);

        Color rend = GetComponent<Renderer>().material.color = new Color(255,255,255,channelAlpha);
        

        //rend.a = channelAlpha;
       
        //Debug.Log("red value " + red + " green value " + green + " blue value " + blue + " alpha value " + alpha);

    }

}
