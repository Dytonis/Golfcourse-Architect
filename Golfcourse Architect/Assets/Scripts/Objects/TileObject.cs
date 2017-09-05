using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GA.Objects
{
    public class TileObject : MonoBehaviour
    {
        public ObjectID objectID;
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
            T[] tileObjects = Resources.LoadAll<GameObject>("NatureObjects")
                    .OfType<GameObject>().Select(x => x.GetComponent<T>()).ToArray();

            foreach (T n in tileObjects)
            {
                if (n.objectID == id)
                    return n;
            }

            return null;
        }

        public static T GetObjectPrefabFromID<T>(int id) where T : TileObject
        {
            T[] tileObjects = Resources.LoadAll<GameObject>("NatureObjects")
                    .OfType<GameObject>().Select(x => x.GetComponent<T>()).ToArray();

            foreach (T n in tileObjects)
            {
                if ((int)n.objectID == id)
                    return n;
            }

            return null;
        }


        public static TileObject GetObjectPrefabFromID(int id)
        {
            TileObject[] tileObjects = Resources.LoadAll<GameObject>("NatureObjects")
                    .OfType<GameObject>().Select(x => x.GetComponent<TileObject>()).ToArray();

            foreach (TileObject n in tileObjects)
            {
                if ((int)n.objectID == id)
                    return n;
            }

            return null;
        }

        public static TileObject GetObjectPrefabFromID(ObjectID id)
        {
            TileObject[] tileObjects = Resources.LoadAll<GameObject>("NatureObjects")
                    .OfType<GameObject>().Select(x => x.GetComponent<TileObject>()).ToArray();

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
        NORM_TEEBOX,
        NORM_HOLE
    }
}
