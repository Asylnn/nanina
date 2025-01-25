using Newtonsoft.Json;
public class ClientNotification 
{
    public string type { get; set; }
    public string message { get; set; }
    public short severity { get; set; }

    public ClientNotification(string type, string message, short severity) {
        this.type = type;
        this.message = message;
        this.severity = severity;
    }

    public static string NotificationData(string type, string message, short severity){
        return JsonConvert.SerializeObject(new ServerWebSocketResponse
        {
            type = "notification",
            data = JsonConvert.SerializeObject( new ClientNotification(type, message, severity))
        });
    }
}