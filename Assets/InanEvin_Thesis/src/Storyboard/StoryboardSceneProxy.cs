using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Proxy
{
    public Collider m_collider;
    public string m_gameObjectID;
}

[System.Serializable]
public class ProxySet
{
    public List<Proxy> m_proxies;
    public StoryboardMarker m_marker;

    public bool CheckIfContains(Vector3 position)
    {
        Vector3 dir = Vector3.zero;
        bool contains = false;

        for (int i = 0; i < m_proxies.Count; i++)
        {
            Proxy proxy = m_proxies[i];
            dir = (proxy.m_collider.bounds.center - position);
            RaycastHit[] hits = Physics.RaycastAll(position, dir.normalized, dir.magnitude);
            bool anyHit = false;

            for (int j = 0; j < hits.Length; j++)
            {
                if (hits[j].transform == proxy.m_collider.transform)
                {
                    anyHit = true;
                    break;
                }
            }

            if (!anyHit)
            {
                contains = true;
                break;
            }

        }
        return contains;
    }

}
/// <summary>
/// Used for calculating available camera positions.
/// </summary>
public class StoryboardSceneProxy : MonoBehaviour
{



}
