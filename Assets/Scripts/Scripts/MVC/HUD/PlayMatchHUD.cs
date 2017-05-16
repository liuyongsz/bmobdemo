using UnityEngine;

using System.Collections;

public class PlayMatchHUD : MonoBehaviour
{
	
    // Use this for initialization
    private void Awake()
    {
    }

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void OnClick()
    {
        PlayerPrefs.SetString("KindCompetition", "Friendly");
        StartCoroutine(LoadScene("Football_Match"));

    }

    private IEnumerator LoadScene(string _name)
    {
        yield return new WaitForSeconds(0.1f);
        Application.LoadLevel(_name);
    }

    private void Nothing()
    {
    }


}