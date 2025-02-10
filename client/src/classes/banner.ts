export default class Banner {
    public bannerName: string = "Choose a Banner";
    public bannerDescription : string = "";
    public id : string = "";
    public pityAmount : number = 0;
    public pullCost : number = 0;
    public twoStarsWeight : number = 0;
    public threeStarsWeight: number = 0;
    public rateUpTwoStarsWeight: number = 0;
    public rateUpThreeStarsWeight : number = 0;
    public twoStarsPollId : string[] = [];
    public threeStarsPollId: string[] = [];
    public rateUpTwoStarsPollId: string[] = [];
    public rateUpThreeStarsPollId: string[] = [];
}