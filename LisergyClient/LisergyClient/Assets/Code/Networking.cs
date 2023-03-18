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

    public virtual void Send<T>(T ev) where T : BaseEvent
    {
        _toSend.Add(Serialization.FromEventRaw(ev));
    }

    int READS_PER_TICK = 10;

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
         
            for(var x = 0; x < READS_PER_TICK; x++)
            {
                if (!client.GetNextMessage(out msg))
                    break;
 
                switch (msg.eventType)
                {
                    case Telepathy.EventType.Connected:
                        Debug.Log("Connected To Server");
                        break;
                    case Telepathy.EventType.Data:
                        MainBehaviour.NetworkEvents.RunCallbacks(MainBehaviour.Player, msg.data);
                        break;
                    case Telepathy.EventType.Disconnected:
                        Debug.Log("Disconnected from Server");
                        break;
                }
            }
        } else
        {
            Log.Debug("Reconnecting");
            client.Connect("127.0.0.1", 1337);
        }
    }

    public void Dispose()
    {
        client.Disconnect();
    }
}
