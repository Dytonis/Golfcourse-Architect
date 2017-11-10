using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;

namespace GA.Ground
{
    public abstract class GroundType
    {
        public abstract int[] spriteChildPath { get; }
        /// <summary>
        /// The bounce reduction
        /// </summary>
        public virtual float restitution { get; set; }
        /// <summary>
        /// Parallel reduction
        /// </summary>
        public virtual float friction { get; set; }
        public virtual float price { get; set; }
        public virtual float accuracyMod { get; set; }
        public virtual float distanceMod { get; set; }
        public virtual float spinMod { get; set; }
        public virtual float shotWeight { get; set; }
        public virtual float walkWeight { get; set; }
        public virtual float shotRisk { get; set; }
        public virtual float shootable { get; set; }
        public virtual float shotFromRiskPenalty { get; set; }
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

    public class Rough_Standard : GroundType
    {
        public override int[] spriteChildPath
        {
            get
            {
                return new int[]
                {
                    (int)GroundSprites.ROUGH_STANDARD_MT,
                    (int)GroundSprites.TRANSPARENT,
                    (int)GroundSprites.DARK_PLATE,
                };
            }
        }

        public override float shotFromRiskPenalty
        {
            get
            {
                return 2;
            }

            set
            {
                base.shotFromRiskPenalty = value;
            }
        }

        public override float shotRisk
        {
            get
            {
                return 5;
            }

            set
            {
                base.shotRisk = value;
            }
        }

        public override bool walkable
        {
            get
            {
                return true;
            }

            set
            {
                base.walkable = value;
            }
        }

        public override float friction
        {
            get
            {
                return 0.45f;
            }

            set
            {
                base.friction = value;
            }
        }

        public override float restitution
        {
            get
            {
                return 0.15f;
            }

            set
            {
                base.restitution = value;
            }
        }

        public override float shotWeight
        {
            get
            {
                return 3;
            }

            set
            {
                base.shotWeight = value;
            }
        }

        public override float walkWeight
        {
            get
            {
                return 1;
            }

            set
            {
                base.walkWeight = value;
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

        public override float shotFromRiskPenalty
        {
            get
            {
                return 0.4f;
            }

            set
            {
                base.shotFromRiskPenalty = value;
            }
        }

        public override bool walkable
        {
            get
            {
                return true;
            }

            set
            {
                base.walkable = value;
            }
        }

        public override float friction
        {
            get
            {
                return 0.2f;
            }

            set
            {
                base.friction = value;
            }
        }

        public override float restitution
        {
            get
            {
                return 0.45f;
            }

            set
            {
                base.restitution = value;
            }
        }

        public override float shotWeight
        {
            get
            {
                return 0;
            }

            set
            {
                base.shotWeight = value;
            }
        }

        public override float shotRisk
        {
            get
            {
                return 1f;
            }

            set
            {
                base.shotRisk = value;
            }
        }

        public override float walkWeight
        {
            get
            {
                return 1;
            }

            set
            {
                base.walkWeight = value;
            }
        }

        public override void OnActivate()
        {
            throw new NotImplementedException();
        }
    }
    public class Green : GroundType
    {
        public override bool walkable
        {
            get
            {
                return true;
            }

            set
            {
                base.walkable = value;
            }
        }

        public override float shotRisk
        {
            get
            {
                return 0;
            }

            set
            {
                base.shotRisk = value;
            }
        }

        public override float friction
        {
            get
            {
                return 0.9f;
            }

            set
            {
                base.friction = value;
            }
        }

        public override float restitution
        {
            get
            {
                return 0.35f;
            }

            set
            {
                base.restitution = value;
            }
        }

        public override float shotWeight
        {
            get
            {
                return 0;
            }

            set
            {
                base.shotWeight = value;
            }
        }

        public override float walkWeight
        {
            get
            {
                return 2;
            }

            set
            {
                base.walkWeight = value;
            }
        }

        public override int[] spriteChildPath
        {
            get
            {
                return new int[]
                {
                    (int)GroundSprites.TRANSPARENT,
                    (int)GroundSprites.TRANSPARENT,
                    (int)GroundSprites.GREEN_STANDARD,
                };
            }
        }

        public override void OnActivate()
        {
            throw new NotImplementedException();
        }
    }
    public class Teebox : Fairway
    {
        public override float shotFromRiskPenalty
        {
            get
            {
                return 0;
            }

            set
            {
                base.shotFromRiskPenalty = value;
            }
        }

        public Tees TeesBoundTo;
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
    public class Gravel : GroundType
    {
        public override bool walkable
        {
            get
            {
                return true;
            }

            set
            {
                base.walkable = value;
            }
        }

        public override float shotFromRiskPenalty
        {
            get
            {
                return 3;
            }

            set
            {
                base.shotFromRiskPenalty = value;
            }
        }

        public override float shotRisk
        {
            get
            {
                return 15;
            }

            set
            {
                base.shotRisk = value;
            }
        }

        public override float friction
        {
            get
            {
                return 0.1f;
            }

            set
            {
                base.friction = value;
            }
        }

        public override float restitution
        {
            get
            {
                return 0.95f;
            }

            set
            {
                base.restitution = value;
            }
        }

        public override float shotWeight
        {
            get
            {
                return 3;
            }

            set
            {
                base.shotWeight = value;
            }
        }

        public override float walkWeight
        {
            get
            {
                return 1;
            }

            set
            {
                base.walkWeight = value;
            }
        }

        public override int[] spriteChildPath
        {
            get
            {
                return new int[]
                {
                    (int)GroundSprites.TRANSPARENT,
                    (int)GroundSprites.GRAVEL,
                    (int)GroundSprites.GRAVEL,
                };
            }
        }

        public override void OnActivate()
        {
            throw new NotImplementedException();
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
