using System.Collections.Generic;
using UnityEngine;

public class GameObjectPull
{
    GameObject parent;
    GameObject gameObject;
    List<GameObject> pull = new List<GameObject>();

    public GameObjectPull(GameObject gameObject)
    {
        this.gameObject = gameObject;
        gameObject.SetActive(false);
        parent = new GameObject();
        parent.name = "PullFor" + gameObject.name;
    }

    public GameObject Get()
    {
        GameObject clone;
        if (pull.Count > 0)
        {
            clone = pull[pull.Count - 1];
            pull.RemoveAt(pull.Count - 1);
            clone.transform.position = gameObject.transform.position;
        }
        else
        {
            clone = GameObject.Instantiate(gameObject, parent.transform);
        }

        clone.SetActive(true);
        return clone;
    }

    public void Put(GameObject clone)
    {
        pull.Add(clone);
        clone.SetActive(false);
    }
}
