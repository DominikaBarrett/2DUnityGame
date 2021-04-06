using System;
using DungeonCrawl;
using DungeonCrawl.Actors.Characters;
using Source.Core;
using UnityEngine;

namespace Source.Actors.Items
{
    public class Helmet : Armor
    {
        public Helmet(string name, int protection) : base(name, protection)
        {
            this.Type = ItemType.Armor;
        }

        public override void Use()
        {
            Player player = GameObject.FindWithTag("Player").GetComponent<Player>();
            player.Equipment.Helmet = player.Equipment.Helmet == this ? null : this;
        }
    }
}