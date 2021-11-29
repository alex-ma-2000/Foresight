using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    GameManager gameManager;

    public AK.Wwise.Event bgm;
    public AK.Wwise.State playingState;

    private bool gameEnded = false;

    // Start is called before the first frame update
    void Start()
    {
        uint bankID;
        AkSoundEngine.LoadBank("Foresight", out bankID);
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        Debug.Log("PlayDeez");
        bgm.Post(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        uint currentState = 0;

        if (gameManager.gameOver && !gameEnded)
        {
            bgm.Stop(gameObject);
            gameEnded = true;
        }
        AkSoundEngine.GetState(playingState.GroupId, out currentState);
        if (currentState == playingState.Id && gameEnded)
        {
            bgm.Post(gameObject);
            gameEnded = false;
        }
    }
}
