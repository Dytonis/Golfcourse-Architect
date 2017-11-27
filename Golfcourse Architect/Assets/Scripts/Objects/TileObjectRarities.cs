using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GA.Objects
{
    public class TileObjectRarities
    {
        public static TileObjectRarityDefinition Garbage = new TileObjectRarityDefinition()
        {
            Color = new Color((88f / 255f), (88f / 255f), (88f / 255f), 1),
            TintColor = new Color((124f / 255f), (124f / 255f), (124f / 255f), (200f / 255f)),
            Strings = new string[5] { "Garbage.", "Hideous.", "Broken.", "Lame.", "Worthless." },
            Subtext = "(Low-Quality)",
            ID = 0
        };
        public static TileObjectRarityDefinition Common = new TileObjectRarityDefinition()
        {
            Color = new Color((190f / 255f), (190f / 255f), (190f / 255f), 1),
            TintColor = new Color((236f / 255f), (236f / 255f), (236f / 255f), (200f / 255f)),
            Strings = new string[4] { "Unremarkable", "Alright", "Every-Day", "Cool, I guess." },
            Subtext = "(Common)",
            ID = 1
        };
        public static TileObjectRarityDefinition Rare = new TileObjectRarityDefinition()
        {
            Color = new Color((77f / 255f), (115f / 255f), (255f / 255f), 1),
            TintColor = new Color((174f / 255f), (195f / 255f), (255f / 255f), (200f / 255f)),
            Strings = new string[1] { "Rare!" },
            ID = 2
        };
        public static TileObjectRarityDefinition Remarkable = new TileObjectRarityDefinition()
        {
            Color = new Color((146f / 255f), (62f / 255f), (176f / 255f), 1),
            TintColor = new Color((164f / 255f), (138f / 255f), (184f / 255f), (200f / 255f)),
            Strings = new string[1] { "Remarkable!" },
            ID = 3
        };
        public static TileObjectRarityDefinition Unique = new TileObjectRarityDefinition()
        {
            Color = new Color((255f / 255f), (115f / 255f), (0f / 255f), 1),
            TintColor = new Color((201f / 255f), (189f / 255f), (93f / 255f), (200f / 255f)),
            Strings = new string[1] { "One-of-a-Kind!" },
            Subtext = "Wow!",
            ID = 4
        };
    }

    public struct TileObjectRarityDefinition
    {
        public Color Color;
        public Color TintColor;
        public string[] Strings;
        public string Subtext;
        public int ID;

        public static implicit operator int(TileObjectRarityDefinition def)
        {
            return def.ID;
        }

        public static implicit operator TileObjectRarityList(TileObjectRarityDefinition def)
        {
            return (TileObjectRarityList)def.ID;
        }

        public static implicit operator TileObjectRarityDefinition(int id)
        {
            if (id == 1)
                return TileObjectRarities.Common;
            else if (id == 2)
                return TileObjectRarities.Rare;
            else if (id == 3)
                return TileObjectRarities.Remarkable;
            else if (id == 4)
                return TileObjectRarities.Unique;

            return TileObjectRarities.Garbage;
        }

        public static implicit operator TileObjectRarityDefinition(TileObjectRarityList rarity)
        {
            int id = (int)rarity;

            if (id == 1)
                return TileObjectRarities.Common;
            else if (id == 2)
                return TileObjectRarities.Rare;
            else if (id == 3)
                return TileObjectRarities.Remarkable;
            else if (id == 4)
                return TileObjectRarities.Unique;

            return TileObjectRarities.Garbage;
        }
    }

    public enum TileObjectRarityList
    {
        Garbage,
        Common,
        Rare,
        Remarkable,
        Unique
    }
}
