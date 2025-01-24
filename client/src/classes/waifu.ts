export default class Waifu {
    public name : string = "Rem"
    public id : string = "42"
    public xp : number = 90
    public lvl : number = 10
    public diffLvlUp : number = 3
    public imgPATH : string = "src/assets/waifu-image/GYrXGACboAACxp7.jpg"
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