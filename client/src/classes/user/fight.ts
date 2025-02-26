import type Game from "./game"

export default class Fight {
    public game! : Game
    public id : string = ""
    public timestamp : number = 0
    public completed : boolean = false
}