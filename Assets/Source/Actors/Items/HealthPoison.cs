using DungeonCrawl;
using DungeonCrawl.Actors.Items;

namespace Source.Actors.Items
{
    public class HealthPoison : Food
    {
        public HealthPoison(string name,int healing) : base(name, healing)
        {
            Type = ItemType.Poison;
        }
    }
}