using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MyExtensions
{
    public static class MyCollectionExtensions
    {
        #region GetRandom
        public static T GetRandom<T>(this T[] array)
        {
            return array[Random.Range(0, array.Length)];
        }
        public static T GetRandom<T>(this List<T> list)
        {
            return list[Random.Range(0, list.Count)];
        }
        /// <summary>
        /// Enumirates until random value
        /// </summary>
        public static T GetRandom<T>(this HashSet<T> hashSet)
        {
            return hashSet.ElementAt(Random.Range(0, hashSet.Count));
        }
        #endregion
        public static T GetLast<T>(this T[] array)
        {
            return array[array.Length - 1];
        }
        public static T GetLast<T>(this List<T> list)
        {
            return list[list.Count - 1];
        }

        public static IEnumerable<T> RemoveDublicates<T>(this List<T> list)
        {
            var passedValues = new HashSet<T>();

            // Relatively simple dupe check alg used as example
            foreach (T item in list)
                if (passedValues.Add(item)) // True if item is new
                    yield return item;
        }
        public static void AddRange<T>(this HashSet<T> hashSet, params T[] values)
        {
            foreach (T item in values)
            {
                hashSet.Add(item);
            }
        }

    }

    public static class MyTransformExtensions
    {
        public static bool IsFacingTarget(this Transform transform, Transform target, float dotThreashhold = 0.7f)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            float dot = Vector3.Dot(transform.forward, direction);
            return dot >= dotThreashhold;
        }
        public static bool IsFacingTarget(this Transform transform, Transform target, out Vector3 direction, float dotThreashhold = 0.7f)
        {
            direction = (target.position - transform.position).normalized;
            float dot = Vector3.Dot(transform.forward, direction);
            return dot >= dotThreashhold;
        }
        public static bool IsFacingTarget(this Transform transform, Vector3 target, float dotThreashhold = 0.7f)
        {
            Vector3 direction = (target - transform.position).normalized;
            float dot = Vector3.Dot(transform.forward, direction);
            return dot >= dotThreashhold;
        }
        public static bool IsFacingTarget(this Transform transform, Vector3 target, out Vector3 direction, float dotThreashhold = 0.7f)
        {
            direction = (target - transform.position).normalized;
            float dot = Vector3.Dot(transform.forward, direction);
            return dot >= dotThreashhold;
        }
    }

    public static class MyRandom
    {
        public static float GaussianRange(float minValue = 0.0f, float maxValue = 1.0f)
        {
            float u, v, S;

            do
            {
                u = 2.0f * Random.value - 1.0f;
                v = 2.0f * Random.value - 1.0f;
                S = u * u + v * v;
            }
            while (S >= 1.0f);

            // Standard Normal Distribution
            float std = u * Mathf.Sqrt(-2.0f * Mathf.Log(S) / S);

            // Normal Distribution centered between the min and max value
            // and clamped following the "three-sigma rule"
            float mean = (minValue + maxValue) / 2.0f;
            float sigma = (maxValue - mean) / 3.0f;
            return Mathf.Clamp(std * sigma + mean, minValue, maxValue);
        }

        public static float Gaussian(float mean = 0, float range = 1)
        {
            float u, v, S;

            do
            {
                u = 2.0f * Random.value - 1.0f;
                v = 2.0f * Random.value - 1.0f;
                S = u * u + v * v;
            }
            while (S >= 1.0f);

            // Standard Normal Distribution
            float std = u * Mathf.Sqrt(-2.0f * Mathf.Log(S) / S);

            // Normal Distribution centered between the min and max value
            // and clamped following the "three-sigma rule"
            float minValue = mean - range;
            float maxValue = mean + range;
            float sigma = (maxValue - mean) / 3.0f;
            return Mathf.Clamp(std * sigma + mean, minValue, maxValue);
        }
    }
}

