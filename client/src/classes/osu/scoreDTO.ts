export default class ScoreDTO
{
    public  accuracy !: number;
    public  max_combo !: number;
    public  map_max_combo !: number;
    public  score !: number;
    public  count_miss !: number;
    public  mods !: string[];
    public  difficulty_rating !: number;
    public  hit_length !: number;
    public   title !: string;
    public   artist !: string;
    public   creator !: string;
    public timesave !: number | null
    public version !: string
    public rank !: string
}