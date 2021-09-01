using Assets.Code;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TownUI : MonoBehaviour
{

    public Button TavernButton;
    public Button Dungeon;
    public Text Coins;

    private void Start()
    {
        TavernButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Tavern", LoadSceneMode.Single);
        });
    }
}
