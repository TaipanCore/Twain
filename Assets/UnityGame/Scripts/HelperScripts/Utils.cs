using UnityEngine;
using UnityEngine.AI;

public static class Utils
{
    public static bool IsInRange(Vector3 a, Vector3 b, float range)
    {
        return (a - b).sqrMagnitude <= range * range;
    }
    public static float GetPathLength(NavMeshPath path)
    {
        float length = 0f;
        for (int i = 1; i < path.corners.Length; i++)
        {
            length += Vector3.Distance(path.corners[i - 1], path.corners[i]);
        }
        return length;
    }
    public static bool IsAgentMoving(NavMeshAgent agent)
    {
        if (!agent.hasPath)
            return false;
        return !agent.pathPending && agent.velocity.sqrMagnitude > 0.01f;
    }
    public static bool IsInLayerMask(this GameObject obj, LayerMask mask)
    {
        //Исходная единица сдвигается на obj.layer (номер слоя) бит влево
        return ((1 << obj.layer) & mask) != 0;
    }

    public static T[] GetRandomElements<T>(T[] sourceArray, int count)
    {
        T[] array = (T[])sourceArray.Clone();
        count = Mathf.Min(count, array.Length);
        T[] result = new T[count];
        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(i, array.Length);
            result[i] = array[randomIndex]; 
            (array[i], array[randomIndex]) = (array[randomIndex], array[i]);
        }
        return result;
    } 
}
