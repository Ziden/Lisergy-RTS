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
        StartCoroutine(DisposeNotification(notif));
        return notif;
    }

    public IEnumerator DisposeNotification(Notification notif)
    {
        yield return new WaitForSeconds(3);
        Active.Remove(notif);
        Destroy(notif.gameObject);
    }
}
