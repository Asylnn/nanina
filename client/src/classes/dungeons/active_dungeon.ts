import type Equipment from "../inventory/equipment";
import type Waifu from "../waifu";
import type DungeonLog from "./dungeon_log";
import DungeonTemplate from "./template_dungeons";

export default class ActiveDungeon {
    public instanceId = "";
    public dungeonTemplate = new DungeonTemplate;
    public userId = "";
    public waifus : Waifu[] = [];
    public timestamp = 0;
    public log : DungeonLog[] = [];
    public health = 0; 
    public isCompleted = true;
    public loot : Equipment[] = [];
}