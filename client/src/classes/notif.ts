import NotificationSeverity from "./notification_severity"

export default class Notificaton {
    public type: string = "Alerte enlèvement"
    public message : string = "Une jeune fille nommée Pippi a été retrouvée en train de farm des PP !"
    public severity : NotificationSeverity = NotificationSeverity.Critical

    constructor(type : string, message : string, severity : NotificationSeverity){
        this.type = type
        this.message = message
        this.severity = severity
    }
}