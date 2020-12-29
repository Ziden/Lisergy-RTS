using Game;
using Game.Events;
using LisergyServer.Core;
using System;
using System.Collections.Generic;
using Telepathy;
using UnityEngine;

public class Networking : IDisposable
{
    Client client = new Client();
    Message msg;

    private List<byte[]> _toSend = new List<byte[]>();

    public Networking()
    {
        Serialization.LoadSerializers();
    }

    public void Send<T>(T ev) where T : GameEvent
    {
        _toSend.Add(Serialization.FromEvent<T>(ev));
    }

    public void Update()
    {
        if (client.Connected)
        {
            while(_toSend.Count > 0)
            {
                var ev = _toSend[0];
                _toSend.RemoveAt(0);
                client.Send(ev);
            }
            var reads = 10;
            for(var x = 0; x < reads;x++)
            {
                if (!client.GetNextMessage(out msg))
                    break;

               
                switch (msg.eventType)
                {
                    case Telepathy.EventType.Connected:
                        Debug.Log("Connected To Server");
                        break;
                    case Telepathy.EventType.Data:
                        EventEmitter.CallEventFromBytes(MainBehaviour.Player, msg.data);
                        break;
                    case Telepathy.EventType.Disconnected:
                        Debug.Log("Disconnected from Server");
                        break;
                }
            }
        } else
        {
            Log.Debug("Reconnecting");
            client.Connect("localhost", 1337);
        }
    }

    public void Dispose()
    {
        client.Disconnect();
    }
}
