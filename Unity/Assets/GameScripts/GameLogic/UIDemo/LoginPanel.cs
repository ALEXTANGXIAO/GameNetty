using System.Collections;
using System.Collections.Generic;
using ET;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : MonoBehaviour
{
    public InputField account;
    public InputField password;
    public Button loginBtn;
    
    // Start is called before the first frame update
    void Start()
    {
        loginBtn.onClick.AddListener(OnClickLogin);
    }

    // Update is called once per frame
    void OnClickLogin()
    {
        LoginHelper.Login(Init.Root,account.text,password.text).Coroutine();
    }
}
