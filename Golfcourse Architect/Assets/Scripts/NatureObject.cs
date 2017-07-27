using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NatureObject : MonoBehaviour
{
    public ObjectID objectID;
    public Vector2 localPosition;
    public Chunk source;

    public virtual void OnSeasonChange()
    {

    }

    public virtual void OnPlace()
    {

    }

    public static NatureObject GetObjectPrefabFromID(ObjectID id)
    {
        NatureObject[] natureObjects = Resources.LoadAll<GameObject>("NatureObjects")
                .OfType<GameObject>().Select(x => x.GetComponent<NatureObject>()).ToArray();

        foreach(NatureObject n in natureObjects)
        {
            if (n.objectID == id)
                return n;
        }

        return null;
    }
    public static NatureObject GetObjectPrefabFromID(int id)
    {
        NatureObject[] natureObjects = Resources.LoadAll<GameObject>("NatureObjects")
                .OfType<GameObject>().Select(x => x.GetComponent<NatureObject>()).ToArray();

        foreach (NatureObject n in natureObjects)
        {
            if ((int)n.objectID == id)
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
}
