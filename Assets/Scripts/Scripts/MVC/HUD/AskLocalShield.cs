using UnityEngine;

public class AskLocalShield : MonoBehaviour
{
    public Sprite[] shields = new Sprite[10];

    // Use this for initialization
    private void Start()
    {
        string teamLocal = PlayerPrefs.GetString("Local");

        if (teamLocal == string.Empty)
        {
            teamLocal = "Argentina";
        }
        else
        {
            foreach (Sprite shield in shields)
            {
                if (shield.name == teamLocal)
                {
                    GetComponent<SpriteRenderer>().sprite = shield;
                }
            }
        }
    }

    // Update is called once per frame
    private void Update()
    {
    }
}