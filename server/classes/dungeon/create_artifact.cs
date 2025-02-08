public partial class ActiveDungeon : Dungeon {
    public List<Equipment> GetLoot(){
        for(int i = 0; i < numberOfRewards; i++){
            Random rng = new ();

            var random_elem = rng.Next(setRewards.Count());
            var map = setRewards.ElementAt(random_elem);
        }
    }
}