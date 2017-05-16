using UnityEngine;

using System.Collections;

public class FriendlyMatch : MonoBehaviour
{
    // Use this for initialization
    private void Awake()
    {
        // 		spriteFade = GameObject.FindGameObjectWithTag("FadeSprite").GetComponent<UISprite>();
        // 		loadingLabel = GameObject.FindGameObjectWithTag("LoadingLabel").GetComponent<UILabel>();
    }

    private void Start()
    {
        // 		spriteFade.gameObject.SetActive(true);
        // 		loadingLabel.gameObject.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void OnClick()
    {
        PlayerPrefs.SetString("KindCompetition", "Friendly");
        StartCoroutine(LoadScene("SelectTeams"));
    }

    private IEnumerator LoadScene(string _name)
    {
        // 		loadingLabel.gameObject.SetActive(true);

        // 		TweenColor tc = spriteFade.gameObject.GetComponent<TweenColor>();
        // 		spriteFade.gameObject.SetActive( true );
        // 		tc.PlayReverse();
        // 		tc.SetOnFinished( Nothing );

        yield return new WaitForSeconds(0.1f);
        Application.LoadLevel(_name);
    }

    private void Nothing()
    {
    }
}