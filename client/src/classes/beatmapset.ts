export default class OsuBeatmapset {
    public artist : string = "";
    public artist_unicode : string = "";
    public covers : Object = {}; //{"cover":"https:\/\/assets.ppy.sh\/beatmaps\/600702\/covers\/cover.jpg?1650649403","cover@2x":"https:\/\/assets.ppy.sh\/beatmaps\/600702\/covers\/cover@2x.jpg?1650649403","card":"https:\/\/assets.ppy.sh\/beatmaps\/600702\/covers\/card.jpg?1650649403","card@2x":"https:\/\/assets.ppy.sh\/beatmaps\/600702\/covers\/card@2x.jpg?1650649403","list":"https:\/\/assets.ppy.sh\/beatmaps\/600702\/covers\/list.jpg?1650649403","list@2x":"https:\/\/assets.ppy.sh\/beatmaps\/600702\/covers\/list@2x.jpg?1650649403","slimcover":"https:\/\/assets.ppy.sh\/beatmaps\/600702\/covers\/slimcover.jpg?1650649403","slimcover@2x":"https:\/\/assets.ppy.sh\/beatmaps\/600702\/covers\/slimcover@2x.jpg?1650649403"}
    public creator : string = "";
    public favourite_count : number = 0;
    public hype : string = "";
    public id : number = 0;
    public nsfw : boolean = false;
    public offset : number = 0;
    public play_count : number = 0;
    public preview_url : string = "";
    public source : string = "";
    public spotlight : boolean = false;
    public status : string = "";
    public title : string = "";
    public title_unicode : string = "";
    public track_id : number = 0;
    public user_id : number = 0;
    public video : boolean = false;
    public bpm: number = 200;
    public can_be_hyped : boolean = true;
    public deleted_at : string = "";
    public last_updated : string = "";
    public discussion_enabled : boolean = true;
    public discussion_locked : boolean = true;
    public is_scoreable : boolean = true;
    public legacy_thread_url : string = "";
    public nominations_summary : Object = {}; 
    public ranked: number = 200;
    public ranked_date : string = "";
    public submitted_date : string = "";
    public tags : string = "";
    public storyboard : boolean = true;
    public availability : Object = {}; 
    public ratings: number[] =  [];


    
}