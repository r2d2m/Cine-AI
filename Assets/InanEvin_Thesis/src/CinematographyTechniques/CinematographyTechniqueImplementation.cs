using UnityEditor;
using UnityEngine;


[System.Serializable]
public abstract class CinematographyTechniqueImplementation : ScriptableObject
{

    public const int c_avoidanceRayCount = 100;
    public const float c_avoidanceRayDistance = 0.25f;

#if UNITY_EDITOR
    [HideInInspector]
    public Editor m_editor = null;
#endif
    public abstract bool Simulate(StoryboardData data, StoryboardNode currentNode, StoryboardNode nextNode, SimulationTargetData targetData);

    public abstract void Play(Camera cam, StoryboardNode node, Transform camManipulator);
    public abstract void Stop(Camera cam);

    protected bool CheckVisibility(StoryboardData data, StoryboardNode currentnode, Vector3 finalPosition, Quaternion finalRotation, SimulationTargetData targetData)
    {
        ProxySet proxySet = data.m_proxySets[currentnode.m_index];

        if (proxySet.CheckIfContains(finalPosition))
        {
            return false;
        }

        RaycastHit hit;
        Vector3 dir = (targetData.m_targetPosition - finalPosition);

        if (Physics.SphereCast(finalPosition, data.m_visibilityCapsuleRadius, dir.normalized, out hit, dir.magnitude))
        {
            if (targetData.m_target != null && hit.transform == targetData.m_target)
            {
                return true;
            }
            else if (targetData.m_target != null && hit.transform != targetData.m_target)
            {
                return Vector3.Distance(hit.point, targetData.m_target.position) < data.m_visibilityContactThreshold;
            }
            else if (targetData.m_target == null)
            {
                return Vector3.Distance(hit.point, targetData.m_targetPosition) < data.m_visibilityContactThreshold;
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    protected void SetSimulationData(StoryboardNode node, Vector3 cameraPosition, Quaternion cameraRotation, float cameraFOV)
    {
        node.m_simulationData.m_cameraPosition = cameraPosition;
        node.m_simulationData.m_cameraRotation = cameraRotation;
        node.m_simulationData.m_cameraFOV = cameraFOV;
    }

    protected bool GetCollisionAvoidedPosition(Vector3 currentPosition, Vector3 targetPosition, Transform target, ref Vector3 visiblePosition)
    {
        Vector3 rayDirection = targetPosition - currentPosition;
        Vector3 moveVector = new Vector3(0, c_avoidanceRayDistance, 0);
        bool isVisible = false;
        int directionCount = 0;

        while(!isVisible && directionCount < 4)
        {
            if (directionCount == 1)
                moveVector = new Vector3(0, -c_avoidanceRayDistance, 0);
            else if (directionCount == 2)
                moveVector = new Vector3(c_avoidanceRayDistance, 0, 0);
            else if (directionCount == 3)
                moveVector = new Vector3(-c_avoidanceRayDistance, 0, 0);

            for (int i = 0; i < c_avoidanceRayCount; i++)
            {
                RaycastHit hit;
                Vector3 rayPosition = currentPosition + moveVector * i;
                Debug.DrawRay(rayPosition, (targetPosition - rayPosition), Color.red, 5);
                if(VisibilityCheck(rayPosition, targetPosition, target))
                {
                    visiblePosition = rayPosition;
                    return true;
                }             
            }

            directionCount++;
        }

        return isVisible;
    }

    protected bool VisibilityCheck(Vector3 currentPosition, Vector3 targetPosition, Transform target)
    {

        RaycastHit hit;
        Vector3 rayDirection = targetPosition - currentPosition;
        bool isVisible = false;
        if (Physics.Raycast(currentPosition, rayDirection.normalized, out hit, rayDirection.magnitude))
        {
            if (target != null && hit.transform == target)
            {
                isVisible = true;
            }
        }
        else
            isVisible = true;

        return isVisible;
    }
}
