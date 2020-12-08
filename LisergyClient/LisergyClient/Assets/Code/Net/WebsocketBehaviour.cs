using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NativeWebSocket;

public class WebsocketBehaviour : MonoBehaviour
{
    private WebSocket Websocket;

    private static WebsocketBehaviour _instance;

    public static WebsocketBehaviour Get()
    {
        return _instance;
    }

    // Start is called before the first frame update
    async void Start()
    {
        Websocket = new WebSocket("ws://localhost:8080/server");

        Websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
        };

        Websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };

        Websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
        };

        Websocket.OnMessage += (bytes) =>
        {
         
        };

        // Keep sending messages at every 0.3s
        //InvokeRepeating("SendWebSocketMessage", 0.0f, 0.3f);

        // waiting for messages
        await Websocket.Connect();
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        Websocket.DispatchMessageQueue();
#endif
    }

    async void SendWebSocketMessage()
    {
        if (Websocket.State == WebSocketState.Open)
        {
            // Sending bytes
            await Websocket.Send(new byte[] { 10, 20, 30 });

            // Sending plain text
            await Websocket.SendText("plain text message");
        }
    }

    private async void OnApplicationQuit()
    {
        await Websocket.Close();
    }

}