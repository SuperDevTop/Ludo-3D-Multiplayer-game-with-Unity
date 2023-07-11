using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuSceneUI : MonoBehaviour
{
    [SerializeField] private GameObject menuUI;
    [SerializeField] private InputField editNameInput;
    [SerializeField] private Text playerNameText;
    [SerializeField] private Text playerInfoNameText;

    private void Start()
    {
        if(PlayerPrefs.GetString("NICKNAME") != "")
        {
            playerNameText.text = PlayerPrefs.GetString("NICKNAME");
            playerInfoNameText.text = PlayerPrefs.GetString("NICKNAME");
        }                
    }

    private void Update()
    {
        menuUI.transform.localScale = new Vector3(Screen.width / 1170f, Screen.height / 2532f, 1);
    }

    public void SaveProfileBtnClick()
    {
        if(editNameInput.text != "")
        {
            playerNameText.text = editNameInput.text;
            playerInfoNameText.text = editNameInput.text;
            PlayerPrefs.SetString("NICKNAME", editNameInput.text);
        }
    }
}
