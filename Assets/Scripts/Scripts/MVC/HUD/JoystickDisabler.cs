using UnityEngine;
using System.Collections;

public class JoystickDisabler : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
#if UNITY_EDITOR

		this.gameObject.SetActive( false );

#endif


	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
