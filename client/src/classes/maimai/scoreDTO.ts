import type DifficultyLevel from "./difficulty_level";
import type Song from "./song";

export default class ScoreDTO
{
    public score_formatted !: string;
    public rank !: string;
    public difficulty_level !: DifficultyLevel;
    public song !: Song;
    public achievement_formatted !: string;
    public timesave !: number;
}