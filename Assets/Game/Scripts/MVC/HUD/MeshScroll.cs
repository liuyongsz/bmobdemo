using UnityEngine;

public class MeshScroll : MonoBehaviour
{
    // Use this for initialization
    private void Start()
    {
        iTween.MoveTo(
            this.gameObject, 
            iTween.Hash(
                "position", 
                new Vector3(5.7f, -0.4f, -1.7f), 
                "time", 
                40.0f, 
                "easetype", 
                "linear", 
                "looptype", 
                "loop", 
                "islocal", 
                false));

        // 		new Vector3( 5.7f, -0.4f, -1.7f ), 40.0f );
    }

    // Update is called once per frame
    private void Update()
    {
    }
}