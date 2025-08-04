import CraftIngrendient from "./craft_ingredient";

export default class Craft
{
    public id !: number
    public timeCost !: number
    public moneyCost !: number
    public ingredients : CraftIngrendient[] = [] 
    //In CraftingPage, we create a craft with all the ingredients. To test if it's not empty, we check for ingredients length
    public results !: CraftIngrendient[]

    /* Client only properties */

    public quantity : number = 0
}