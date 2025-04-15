import type OsuBeatmap from "../beatmap";
import type Equipment from "../item/equipment";
import type Waifu from "../waifu/waifu";
import type DungeonLog from "./dungeon_log";
import DungeonTemplate from "./template_dungeons";

export default class ActiveDungeon {
    public instanceId = "";
    public template = new DungeonTemplate;
    public userId = "";
    public waifus : Waifu[] = [];
    public timestamp = 0;
    public log : DungeonLog[] = [];
    public health = 0; 
    public maxHealth = 0;
    public isCompleted = true;
    public loot : Equipment[] = [];
    public beatmap !: OsuBeatmap;
}