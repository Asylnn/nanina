export default class Notif {
    public type: string = "Alerte enlèvement"
    public message : string = "Une jeune fille nommée Pippi a été retrouvée en train de farm des PP !"

    constructor(_type : string, _message : string){
        this.type = _type
        this.message = _message
    }
}