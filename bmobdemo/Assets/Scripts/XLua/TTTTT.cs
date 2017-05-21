using UnityEngine;
using System.Collections;

public class TTTTT : MonoBehaviour {

    public Transform OrgTran;
    public Transform TargetTran;

    public bool InFront;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {


        if(null != OrgTran && null != TargetTran)
        {
            InFront = GeomUtil.Match_InFront(OrgTran.position, TargetTran.position);
        }

    }

    private void OnGUI()
    {

        GUI.Label(new Rect(0,0,100,40),InFront.ToString());
    }
}
