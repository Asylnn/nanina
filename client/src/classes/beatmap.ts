import OsuBeatmapset from "./beatmapset";

export default class OsuBeatmap {
    public beatmapset_id : number = 0
    public beatmapset : OsuBeatmapset = new OsuBeatmapset
    public difficulty_rating : number = 0
    public id : number = 0
    public mode : string = "";
    public status : string = "";
    public total_length : number = 0
    public user_id : number = 0
    public version : string = "";
    public accuracy : number = 0
    public ar : number = 0
    public bpm : number = 0
    public cs : number = 0
    public drain : number = 0
    public convert : boolean = false;
    public count_circles : number = 0
    public count_sliders : number = 0
    public count_spinners : number = 0
    public deleted_at : string = "";
    public hit_length : number = 0
    public is_scoreable : boolean = false;
    public last_updated : string = "";
    public mode_int : number = 0
    public passcount : number = 0
    public playcount : number = 0
    public ranked : number = 0
    public url : string = "";
    public checksum : string = "";
    public failtimes : number[] = []
}