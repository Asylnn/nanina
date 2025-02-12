import type WaifuEquipmentManager from "./equipment"

export default class Waifu {
    public name : string = "Rem"
    public id : string = "42"
    public xp : number = 0
    public lvl : number = 1
    public diffLvlUp : number = 3
    public imgPATH : string = "GYrXGACboAACxp7.jpg"
    public stars : number = 2
    public equipment! : WaifuEquipmentManager

    public b_str : number = 0
    public o_str : number = 0
    public u_str : number = 0
    public b_agi : number = 0
    public o_agi : number = 0
    public u_agi : number = 0
    public b_kaw : number = 0
    public o_kaw : number = 0
    public u_kaw : number = 0
    public b_dex : number = 0
    public o_dex : number = 0
    public u_dex : number = 0
    public b_luck : number = 0
    public o_luck : number = 0
    public u_luck : number = 0
    public b_int : number = 0
    public o_int : number = 0
    public u_int : number = 0

    public get xpToLvlUp() : number {
        return Math.floor(this.diffLvlUp*(10*this.lvl + 20))
    }

    constructor(obj: any){
        Object.assign(this, obj)
    }
    /*constructor(name :string ,xp : number,lvl:number,diffLvlUp : number,imgPATH : string) {
        this.name = name
        this.xp = xp
        this.diffLvlUp = diffLvlUp
        this.lvl = lvl
        this.imgPATH = imgPATH
    }*/
}