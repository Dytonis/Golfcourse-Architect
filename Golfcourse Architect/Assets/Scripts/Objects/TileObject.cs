using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using GA.Engine;

namespace GA.Objects
{
    public class TileObject : MonoBehaviour
    {
        public ObjectID objectID;
        public TileObjectRarityDefinition RarityInfo
        {
            get
            {
                return Rarity;
            }
        }
        public TileObjectRarityList Rarity;
        public int Cost;
        public TileObjectUIInfo UIInfo;
        public TileObjectEffect Effect;
        public Vector2 FlatPosition;
        public Vector3 CenterPosition
        {
            get
            {
                return transform.position;
            }
        }
        public Chunk source;

        public static T GetObjectPrefabFromID<T>(ObjectID id) where T : TileObject
        {
            T[] tileObjects = Resources.LoadAll<GameObject>("TileObjects")
                    .OfType<GameObject>().Select(x => x.GetComponent<T>()).Where(x => x != null).ToArray();

            foreach (T n in tileObjects)
            {
                if (n.objectID == id)
                    return n;
            }

            return null;
        }

        public static T GetObjectPrefabFromID<T>(int id) where T : TileObject
        {
            T[] tileObjects = Resources.LoadAll<GameObject>("TileObjects")
                    .OfType<GameObject>().Select(x => x.GetComponent<T>()).Where(x => x != null).ToArray();

            foreach (T n in tileObjects)
            {
                if ((int)n.objectID == id)
                    return n;
            }

            return null;
        }


        public static TileObject GetObjectPrefabFromID(int id)
        {
            TileObject[] tileObjects = Resources.LoadAll<GameObject>("TileObjects")
                    .OfType<GameObject>().Select(x => x.GetComponent<TileObject>()).Where(x => x != null).ToArray();

            foreach (TileObject n in tileObjects)
            {
                if ((int)n.objectID == id)
                    return n;
            }

            return null;
        }

        public static TileObject GetObjectPrefabFromID(ObjectID id)
        {
            TileObject[] tileObjects = Resources.LoadAll<GameObject>("TileObjects")
                    .OfType<GameObject>().Select(x => x.GetComponent<TileObject>()).Where(x => x != null).ToArray();

            foreach (TileObject n in tileObjects)
            {
                if (n.objectID == id)
                    return n;
            }

            return null;
        }
    }

    public enum ObjectID
    {
        EMPTY,
        NATURE_TREE_TEMPORATE1,
        NATURE_TREE_TEMPORATE2,
        NATURE_TREE_TEMPORATE3,
        NATURE_TREE_TEMPORATE4,
        NATURE_TREE_TEMPORATE5,
        GHOLE_TEEBOX,
        GHOLE_HOLE,
        OBJECT_BALLCLEANER,
    }

    [System.Serializable]
    public struct TileObjectEffect
    {
        /// <summary>
        /// Directly affects mood.
        /// </summary>
        public float Hapiness;
        /// <summary>
        /// Affects the course's scenery rating (for awards, land value, golfer hapiness)
        /// </summary>
        public float Scenery;
        /// <summary>
        /// Affects how effective an object is at lowering hunger. Lower is better.
        /// </summary>
        [TileObjectEffectInverseColor(true)]
        public float Hunger;
        /// <summary>
        /// Affects how much patience a golfer has.
        /// </summary>
        public float Patience;
    }

    [System.Serializable]
    public struct TileObjectUIInfo
    {
        public string UIName;
        public string Description;
    }
}
