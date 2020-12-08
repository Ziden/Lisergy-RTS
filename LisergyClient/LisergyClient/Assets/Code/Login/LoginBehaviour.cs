using Common.Packets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var packet = new LoginPacket()
        {
            Login = "Test",
            Password = "Wololo"
        };

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
