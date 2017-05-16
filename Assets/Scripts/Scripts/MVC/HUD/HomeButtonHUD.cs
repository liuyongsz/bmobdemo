using UnityEngine;

using System.Collections;

public class HomeButtonHUD : MonoBehaviour
{
    // Use this for initialization
    private void Awake()
    {
        // 		spriteFade = GameObject.FindGameObjectWithTag("FadeSprite").GetComponent<UISprite>();
        // 		loadingLabel = GameObject.FindGameObjectWithTag("LoadingLabel").GetComponent<UILabel>();
    }

    // Use this for initialization
    private void Start()
    {
        // 		spriteFade.gameObject.SetActive(true);
        // 		loadingLabel.gameObject.SetActive(false);

        // 		button = GetComponent<UIButton>();

        // 		button.hoverSprite = button.normalSprite;
        // 		button.pressedSprite = button.normalSprite;
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void OnClick()
    {
        StartCoroutine(LoadScene("MainMenu"));
    }

    private IEnumerator LoadScene(string _name)
    {
        // 		loadingLabel.gameObject.SetActive(true);

        // 		TweenColor tc = spriteFade.gameObject.GetComponent<TweenColor>();
        // 		spriteFade.gameObject.SetActive( true );
        // 		tc.PlayReverse();
        // 		tc.SetOnFinished( Nothing );

        yield return new WaitForSeconds(1.0f);
        Application.LoadLevel(_name);
    }

    private void Nothing()
    {
    }
}