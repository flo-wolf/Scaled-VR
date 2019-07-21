using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class ExtensionMethods
{
    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    // detect if a 3D point is inside a cone or not?
    public static bool IsPointInsideCone(Vector3 point, Vector3 tipPos, Vector3 tipToCenterDir, float height, float radius)
    {
        // point = point to test
        // tipPos = the tip of the cone
        // tipToCenterDir = the normalized axis vector, pointing from the tip to the base
        // height = height of the cone
        // radius = circular base radius of the cone

        // project point onto tipToCenterDir to find the point's distance along the axis:
        float cone_dist = Vector3.Dot(point - tipPos, tipToCenterDir);

        // reject values outside 0 <= cone_dist <= h
        if (0 > cone_dist || cone_dist > height)
        {
            return false;
        }

        // calculate the cone radius at that point along the axis:
        float cone_radius = (cone_dist / height) * radius;

        // calculate the point's orthogonal distance from the axis to compare against the cone radius:
        float orth_distance = ((point - tipPos) - cone_dist * tipToCenterDir).magnitude;

        if (orth_distance < cone_radius) // because we used sqrMagnitude we are squaring the distance we check against too
            return true;

        return false;
    }

    // Deep clone
    public static T DeepClone<T>(this T a)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, a);
            stream.Position = 0;
            return (T)formatter.Deserialize(stream);
        }
    }
}