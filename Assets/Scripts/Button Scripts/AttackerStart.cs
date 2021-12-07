using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackerStart : MonoBehaviour
{
    public GameObject playerSelect;

    private Button button;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(Attacker);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Attacker()
    {
        gameManager.gameState = GameManager.GameState.Attacker;
        gameManager.setGameStarted(true);
        playerSelect.SetActive(false);
        gameManager.scoreboard.gameObject.SetActive(true);
        gameManager.attackerResources.gameObject.SetActive(true);
        gameManager.defenderResources.gameObject.SetActive(true);
        AkSoundEngine.PostEvent("Menu_Buttons", gameObject);
    }
}
