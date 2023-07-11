using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginSplashUI : MonoBehaviour
{
    [SerializeField] private GameObject menuUI;

    void Update()
    {
        menuUI.transform.localScale = new Vector3(Screen.width / 1170f, Screen.height / 2532f, 1);
    }
}
