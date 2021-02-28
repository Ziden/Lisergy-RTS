using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameNotifications : MonoBehaviour
{
    public GameObject NotificationPrefab;

    private List<Notification> Active = new List<Notification>();

    public Notification ShowNotification(string msg)
    {
        var notif = Instantiate(NotificationPrefab, this.transform).GetComponent<Notification>();
        notif.Text.text = msg;
        Active.Add(notif);
        return notif;
    }

    public void DisposeNotification(Notification notif)
    {
        Active.Remove(notif);
        Destroy(notif.gameObject);
    }
}
