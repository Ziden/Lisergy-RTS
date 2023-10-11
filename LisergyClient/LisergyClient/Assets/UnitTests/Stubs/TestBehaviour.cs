using Assets.UnitTests;
using UnityEngine;

public class TestBehaviour : MonoBehaviour
{
    void Update()
    {
        while(StubServer.OutputStream.TryDequeue(out var packet))
        {
            Debug.Log("Running packet in client");
            //MainBehaviour.ServerPackets.Call(MainBehaviour.LocalPlayer, packet.Data);
        }
    }
}
