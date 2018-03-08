﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using IBM.Watson.DeveloperCloud.Services.LanguageTranslator.v2;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Connection;

public class LanguageTranslatorDemo : MonoBehaviour
{
    public Text ResponseTextField;

    private LanguageTranslator _languageTranslator;
    private string _translationModel = "en-es";

    // Use this for initialization
    void Start()
    {
        LogSystem.InstallDefaultReactors();

        Credentials credentials = new Credentials()
        {
            Username = "3838fb76-e9b4-4a64-bb19-a2cde3562001",
            Password = "32aHHbh0yDSi",
            Url = "https://gateway.watsonplatform.net/language-translator/api"
        };

        _languageTranslator = new LanguageTranslator(credentials);

        
       
	}//end STart

    public void Translate(string text)
    {
        _languageTranslator.GetTranslation(OnTranslate, OnFail, text, _translationModel);
    }

    private void OnTranslate(Translations response, Dictionary<string,object>customData)
    {
        ResponseTextField.text = response.translations[0].translation;
    }

    private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
    {
        Log.Debug("LanguageTranslatorDemo.OnFail()", "Error: {)}", error.ToString());
    }
	
	
}
