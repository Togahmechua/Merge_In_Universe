using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cache 
{
   /* private static Dictionary<Collider2D, Coral> coral = new Dictionary<Collider2D, Coral>();

    public static Coral GetCoral(Collider2D collider)
    {
        if (!coral.ContainsKey(collider))
        {
            coral.Add(collider, collider.GetComponent<Coral>());
        }

        return coral[collider];
    }*/

     private static Dictionary<GameObject, Planet> planet = new Dictionary<GameObject, Planet>();

    public static Planet GetPlanet(GameObject obj)
    {
        if (!planet.ContainsKey(obj))
        {
            Planet controller = obj.GetComponent<Planet>();
            if (controller != null)
            {
                planet.Add(obj, controller);
            }
        }
        return planet.ContainsKey(obj) ? planet[obj] : null;
    }
}
