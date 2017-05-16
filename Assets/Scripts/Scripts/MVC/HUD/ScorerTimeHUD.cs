using UnityEngine;
using UnityEngine.UI;

public class ScorerTimeHUD : MonoBehaviour
{
    public float timeMatch = 0.0f;

    public int minutes = 0;

    public int seconds = 0;

    private CoreGame coreGame;

    private int timeMatchOptions;

    // Use this for initialization
    private void Start()
    {
        this.coreGame = GameObject.FindObjectOfType(typeof(CoreGame)) as CoreGame;

        this.timeMatchOptions = PlayerPrefs.HasKey("TimeMatch") ? PlayerPrefs.GetInt("TimeMatch") : 5;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (this.coreGame.GetState() == CoreGame.InGameState.PLAYING)
        {
            timeMatch += Time.deltaTime * (90.0f / timeMatchOptions);
        }

        int d = (int)(timeMatch * 100.0f);
        minutes = d / (60 * 100);
        seconds = (d % (60 * 100)) / 100;
        // 		int hundredths = d % 100;

        string time = string.Format("{0:00}:{1:00}", minutes, seconds);
        // 		GetComponentInChildren<UILabel> ().text = time;

        GetComponent<Text>().text = time;
    }
}