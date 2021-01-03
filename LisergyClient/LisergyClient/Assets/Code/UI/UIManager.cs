using Assets.Code;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static LoginCanvas _loginCanvas;
    private static GameUICanvas _gameUI;
    private static UIManager _instance;

    public LoginCanvas LoginCanvas {
        get
        {
            return _loginCanvas;
        }
    }

    public GameUICanvas GameUI
    {
        get
        {
            return _gameUI;
        }
    }

    public static UIManager Get()
    {
        return _instance;
    }

    private void Start()
    {
        _instance = this;
        _loginCanvas = new LoginCanvas();
        _gameUI = new GameUICanvas();
   
    }
}
