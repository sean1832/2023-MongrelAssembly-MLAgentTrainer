using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;

public class UdpServer : MonoBehaviour
{
    public static void Sender(string message, IPEndPoint endPoint, UdpClient client)
    {
        try
        {
            // encode string to UTF8-coded bytes
            byte[] data = Encoding.UTF8.GetBytes(message);

            // send the data
            client.Send(data, data.Length, endPoint);

        }
        catch (Exception err)
        {
            print($"UDP Send error: {err.ToString()}");
        }
    }
}
