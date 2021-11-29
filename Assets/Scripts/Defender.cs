using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defender : MonoBehaviour
{
    private GameManager gameManager;
    private Attacker attacker;

    public AK.Wwise.Event defenderHit;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        attacker = GameObject.Find("ball").GetComponent<Attacker>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Tick Game Score
        gameManager.AttackerWin();
        attacker.ResetBouncesOccured();
        defenderHit.Post(gameObject);
    }
}
