using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
using Newtonsoft.Json;

public static class DungeonManager {
    public static readonly DungeonTemplate[] dungeons = JsonConvert.DeserializeObject<DungeonTemplate[]>(File.ReadAllText(Environment.GetEnvironmentVariable("DUNGEON_STORAGE_PATH")));
    private static readonly Dictionary<ulong, ActiveDungeon> activeDungeons;
    private static ulong counter = 0;
    //Should be mutexed before release
    public static ActiveDungeon InstantiateDungeon(DungeonTemplate dungeon, User user, List<Waifu> waifus){
        var activeDungeon = new ActiveDungeon(dungeon, user, waifus);
        activeDungeons.Add(counter, activeDungeon);
        counter++;
        return activeDungeon;
    }
}