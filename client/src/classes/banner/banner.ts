import type BannerPoolElement from "./banner_pool_element";

export default class Banner {
    public id : string = "";
    public pityAmount : number = 0;
    public pullCost : number = 0;
    public standardPool !: BannerPoolElement[]
    public  pityPool !: BannerPoolElement[]
}