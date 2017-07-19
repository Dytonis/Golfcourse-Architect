﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace GA.Ground
{
    public abstract class GroundType
    {
        public abstract int[] spriteChildPath { get; }
        public virtual float bounce { get; set; }
        public virtual float speed { get; set; }
        public virtual float price { get; set; }
        public virtual float accuracyMod { get; set; }
        public virtual float distanceMod { get; set; }
        public virtual float spinMod { get; set;  }

        public abstract void OnActivate();

        public virtual void OnPlace()
        {

        }

        public virtual void OnRemove()
        {

        }

        public virtual void OnBallHit(Vector3 pos)
        {

        }

        public virtual void OnBallHitOffOf()
        {

        }

        public Color[][] GetColorsFromTexture()
        {
            Sprite[] spriteSheet = Resources.LoadAll<Sprite>("GroundSprites/GroundTileSheet1")
                .OfType<Sprite>().ToArray();

            Sprite s0 = spriteSheet[spriteChildPath[0]];
            Sprite s1 = spriteSheet[spriteChildPath[1]];
            Sprite s2 = spriteSheet[spriteChildPath[2]];

            Sprite[] sprites = new Sprite[]
            {
                s0,
                s1,
                s2
            };

            List<List<Color>> colors = new List<List<Color>>();

            for (int i = 0; i < 3; i++)
            {
                colors.Add(new List<Color>());
                for (int y = 0; y < s0.rect.height; y++)
                {
                    for (int x = 0; x < s0.rect.width; x++)
                    {
                        colors[i].Add(sprites[i].texture.GetPixel(x + (int)sprites[i].rect.x, y + (int)sprites[i].rect.y));
                    }
                }
            }

            return colors.Select(Enumerable.ToArray).ToArray();
        }
    }

    public class Rough_Standard : GroundType
    {
        public override int[] spriteChildPath
        {
            get
            {
                return new int[]
                {
                    (int)GroundSprites.ROUGH_STANDARD,
                    (int)GroundSprites.TRANSPARENT,
                    (int)GroundSprites.DARK_PLATE,
                };
            }
        }

        public override void OnActivate()
        {
            throw new NotImplementedException();
        }
    }

    public class Fairway : GroundType
    {
        public override int[] spriteChildPath
        {
            get
            {
                return new int[]
                {
                    (int)GroundSprites.TRANSPARENT,
                    (int)GroundSprites.FAIRWAY_STANDARD,
                    (int)GroundSprites.DARK_PLATE,
                };
            }
        }

        public override void OnActivate()
        {
            throw new NotImplementedException();
        }
    }

    public class Max_Bounds : GroundType
    {
        public override int[] spriteChildPath
        {
            get
            {
                return new int[]
                {
                    (int)GroundSprites.TRANSPARENT,
                    (int)GroundSprites.TRANSPARENT,
                    (int)GroundSprites.TRANSPARENT,
                };
            }
        }

        public override void OnActivate()
        {
            return;
        }
    }

    public enum GroundSprites
    {
        TRANSPARENT = 8,
        ROUGH_STANDARD = 1,
        FAIRWAY_STANDARD = 0,
        FAIRWAY_FAST = 100,
        GREEN_STANDARD = 3,
        DARK_PLATE = 7
    }
}