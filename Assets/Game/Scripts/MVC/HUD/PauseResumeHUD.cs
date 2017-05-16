using UnityEngine;
using UnityEngine.UI;

public class PauseResumeHUD : MonoBehaviour
{
    public Sprite pauseSprite;

    public Sprite continueSprite;

    private bool bPaused = false;

    private Image sprite;
	
    // Use this for initialization
    private void Start()
    {
        sprite = GetComponent<Image>();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void OnClick()
    {
       
    }
}