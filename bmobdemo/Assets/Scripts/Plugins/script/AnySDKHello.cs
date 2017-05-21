using UnityEngine;
using System.Collections;

public class AnySDKHello : MonoBehaviour {
	private GUIStyle fontStyle;
		
	void Awake()
	{
		fontStyle = new GUIStyle();
		fontStyle.fontStyle = FontStyle.Bold;
		fontStyle.alignment = TextAnchor.MiddleCenter;
        fontStyle.fontSize = 60;   
	}
	
	void OnDestroy() {
		
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update() {
		if(Input.GetKeyDown(KeyCode.Escape)||Input.GetKeyDown(KeyCode.Home))  
	    {  
	        Application.Quit();  
	    } 
	}
	
	void OnGUI()
	{	
		GUI.Label(new Rect(100,Screen.height/2 - 50,Screen.width - 200,100),"Hello AnySDK", fontStyle );
	}
}
