using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;

namespace GA.Game.GroundTypes
{
    public abstract class GroundType
    {
        public abstract int[] spriteChildPath { get; }
        /// <summary>
        /// The bounce reduction (bounciness)
        /// </summary>
        public virtual float restitution { get; set; }
        /// <summary>
        /// Parallel reduction (friction)
        /// </summary>
        public virtual float friction { get; set; }
        /// <summary>
        /// how much this costs to place
        /// </summary>
        public virtual float price { get; set; }
        /// <summary>
        /// changes how the golfer's accuracy is effected by the ground. 0 = completely random, 1 = no effect, 2 = twice as accurate 
        /// </summary>
        public virtual float accuracyMod { get; set; }
        /// <summary>
        /// changes how the golfer's max distance is effected. 0 = no distance, 1 = no effect, 2 = twice the distance
        /// </summary>
        public virtual float distanceMod { get; set; }
        /// <summary>
        /// changes how much spin can be applied to the ball on this tile. 0 = no spin, 1 = no affect (golfer skill) 2 = twice the spin 
        /// </summary>
        public virtual float spinMod { get; set; }
        /// <summary>
        /// tells the shot finder AI how easy this tile is to hit over. higher numbers means it will try to find a longer path around this tile, with the biggest numbers having the most severe impact on this calculation
        /// </summary>
        public virtual float shotWeight { get; set; }
        /// <summary>
        /// tells the pathfinding AI how easy this tile is to walk on. high number menas it will try to find a longer path around this tile, with the biggest numbers having the most severe impact on this calculation
        /// </summary>
        public virtual float walkWeight { get; set; }
        /// <summary>
        /// tells the shot finder AI how risky a shot over this tile is. higher numbers mean a more aggresive play style must be used to pick it.
        /// </summary>
        public virtual float shotRisk { get; set; }
        /// <summary>
        /// should you be able to hit a ball off this tile? if no, a drop will be required if the ball lands here
        /// </summary>
        public virtual bool shootable { get; set; }
        /// <summary>
        /// how much risk should be applied to a shot starting from this tile regardless of the path of the shot. this is to make golfers more likely to lay-up if they are in a bad lie.
        /// </summary>
        public virtual float shotFromRiskPenalty { get; set; }
        /// <summary>
        /// should you be able to walk on this tile? if no, the pathfinder will be forced to path around this.
        /// </summary>
        public virtual bool walkable { get; set; }

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
            Sprite[] spriteSheet = TextureData.GetSpriteSheet();

            MultiTextureFactory factory = new MultiTextureFactory();

            Sprite s0 = spriteSheet[factory[spriteChildPath[0]]];
            Sprite s1 = spriteSheet[factory[spriteChildPath[1]]];
            Sprite s2 = spriteSheet[factory[spriteChildPath[2]]];

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

    public enum GroundSprites
    {
        TRANSPARENT = 0,
        ROUGH_STANDARD_MT = 0xFF00, //Export to MultiTexture 0xFF00
        ROUGH_STANDARD = 2, 
        FAIRWAY_STANDARD = 1,
        FAIRWAY_FAST = 100,
        GREEN_STANDARD = 3,
        DARK_PLATE = 8,
        GRAVEL = 9,
        TEEBOX = 1
    }

    public static class MultiTexture
    {
        public static int[][] Textures = new int[][]
        {
            new int[] { 10, 11, 12, 13, 14, 15, 16 }, //0xFF00
                                                      //0xFF01
                                                      //0xFF02
        };
    }

    public sealed class MultiTextureFactory
    {
        public int this[int i, int fallback = 1]
        {
            get
            {
                if (i >= 0xFF00)
                {
                    int n = i & 0x00FF; //we want thet second byte only
                    if (MultiTexture.Textures.Length >= n) //check if the MultiTexture jagged has an entry of the desired id
                    {
                        int[] t = MultiTexture.Textures[n];
                        return t[UnityEngine.Random.Range((int)0, t.Length)];
                    }
                    else
                    {
                        return fallback;
                    }
                }
                else
                {
                    return i;
                }
            }
        }
    }

    public static class TextureData
    {
        private static Sprite[] spriteSheet; 

        public static Sprite[] GetSpriteSheet()
        {
            if (spriteSheet != null)
            {
                if (spriteSheet.Length == 0)
                {
                    UpdateSpriteSheet();
                }
            }
            else
            {
                UpdateSpriteSheet();
            }

            return spriteSheet;
        }

        public static void UpdateSpriteSheet()
        {
            spriteSheet = Resources.LoadAll<Sprite>("GroundSprites/GroundTileSheet1")
                .OfType<Sprite>().ToArray();
        }
    }
}
