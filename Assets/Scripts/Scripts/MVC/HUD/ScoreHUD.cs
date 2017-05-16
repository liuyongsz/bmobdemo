using UnityEngine;
using UnityEngine.UI;

public class ScoreHUD : MonoBehaviour
{
    private CoreGame coreGame;

    // Use this for initialization
    private void Start()
    {
        this.coreGame = GameObject.FindObjectOfType(typeof(CoreGame)) as CoreGame;
    }

    // Update is called once per frame
    private void LateUpdate()
    {
       
    }
}