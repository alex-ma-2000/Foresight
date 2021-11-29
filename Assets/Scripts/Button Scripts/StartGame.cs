using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGame : MonoBehaviour
{
    private Button button;

    // Start is called before the first frame update
    void Start()
    {
        button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(EnterGame);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void EnterGame()
    {
        SceneManager.LoadScene("Main");
    }

    public void OnClick()
    {
        AkSoundEngine.PostEvent("Menu_Buttons", gameObject);
    }
}
