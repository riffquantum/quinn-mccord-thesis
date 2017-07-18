using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeAlpha : MonoBehaviour {
    public float chan1Alpha, chan2Alpha, chan3Alpha, chan4Alpha, chan5Alpha;
    
    public void Chan1Change (float alphaValue)
    {
        this.chan1Alpha = alphaValue;
        SetAlpha();
    }

    public void Chan2Change(float alphaValue)
    {
        this.chan2Alpha = alphaValue;
        SetAlpha();
    }

    public void Chan3Change(float alphaValue)
    {
        this.chan3Alpha = alphaValue;
        SetAlpha();
    }

    public void Chan4Change(float alphaValue)
    {
        this.chan4Alpha = alphaValue;
        SetAlpha();
    }

    public void Chan5Change(float alphaValue)
    {
        this.chan5Alpha = alphaValue;
        SetAlpha();
    }

    public void SetAlpha()
    {
        Color rend = GetComponent<Renderer>().material.color;
        rend.a = chan1Alpha;
    }
}
