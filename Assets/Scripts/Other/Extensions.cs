using System;
using System.Collections;
using UnityEngine;
using Scripts.Gameplay.Abillities;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Scripts.Gameplay.Entities;
using Mirror;
using System.Threading;
using Cinemachine.Utility;
using static UnityEngine.EventSystems.EventTrigger;

namespace Scripts.Extensions
{
    public static class Extensions
    {
        // Global
        #region CheckInitialization
        /// <summary>
        /// If already initialized, returns true, however if not initialized, <br />
        /// returns false and makes the field <see href="isInitialized"/> true.
        /// </summary>
        public static bool CheckInitialization(this ref bool isInitialized)
        {
            if (isInitialized)
                return true;

            isInitialized = true;
            return false;
        }
        #endregion

        #region IncreaseInBounds
        /// <summary>
        /// Safely increments index within array length
        /// </summary>
        public static void IncreaseInBounds(this ref int index, Array array) => index.IncreaseInBounds(array.Length);

        /// <summary>
        /// Safely increments index within array length
        /// </summary>
        public static void IncreaseInBounds(this ref uint index, Array array) => index.IncreaseInBounds((uint)array.Length);

        /// <summary>
        /// Safely increments index within array length
        /// </summary>
        public static void IncreaseInBounds(this ref int index, int bounds, bool dontCollideBounds = true)
        {
            index++;

            if (index > bounds - (dontCollideBounds ? 1 : 0))
                index = 0;
        }

        /// <summary>
        /// Safely increments index within array length
        /// </summary>
        public static void IncreaseInBounds(this ref uint index, uint bounds, bool dontCollideBounds = true)
        {
            index++;

            if (index > bounds - (dontCollideBounds ? 1 : 0))
                index = 0;
        }
        #endregion

        #region DecreaseInBounds
        /// <summary>
        /// Safely decrements index within array length
        /// </summary>
        public static void DecreaseInBounds(this ref int index, Array array) => index.DecreaseInBounds(array.Length);

        /// <summary>
        /// Safely decrements index within array length
        /// </summary>
        public static void DecreaseInBounds(this ref uint index, Array array) => index.DecreaseInBounds((uint)array.Length);

        /// <summary>
        /// Safely decrements index within array length
        /// </summary>
        public static void DecreaseInBounds(this ref int index, int bounds, bool dontCollideBounds = true)
        {
            if (index > 0)
                index--;
            else
                index = bounds - (dontCollideBounds ? 1 : 0);
        }

        /// <summary>
        /// Safely decrements index within array length
        /// </summary>
        public static void DecreaseInBounds(this ref uint index, uint bounds, bool dontCollideBounds = true)
        {
            if (index > 0)
                index--;
            else
                index = bounds - (dontCollideBounds ? 1u : 0u);

        }
        #endregion

        #region AvailableAndNotNull
        /// <summary>
        /// Checks if it is not null and <see cref="Abillity.Available"/>
        /// </summary>
        public static bool AvailableAndNotNull(this Abillity ability)
            => ability != null && ability.Available();

        #endregion

        #region FindByType

        /// <summary>
        /// Looking for the first <see href="T"/>
        /// </summary>
        public static T FindByType<T>(this IEnumerable enumerable)
        {
            foreach (var t in enumerable)
            {
                if (t is T result)
                {
                    return result;
                }
            }
            return default;
        }

        /// <summary>
        /// Looking for the first <see href="T"/>
        /// </summary>
        public static object FindByType(this IEnumerable enumerable, Type type)
        {
            foreach (var t in enumerable)
            {
                if (t.GetType() == type)
                {
                    return t;
                }
            }
            return null;
        }

        #endregion

        #region SetAll

        /// <summary>
        /// Sets value to all elements in dictionary
        /// </summary>
        public static void SetAll<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TValue value, params TKey[] except)
        {
            foreach (var pair in dictionary.ToArray())
            {
                if (except.Contains(pair.Key))
                    continue;

                lock (dictionary)
                dictionary[pair.Key] = value;
            }
        }

        #endregion

        #region GetIfNull
        /// <summary>
        /// If null, causes <see href="GetComponent"/>, otherwise does nothing
        /// </summary>
        public static bool GetIfNull<T>(this T value, GameObject gameObject) where T : Component
        {
            ref T valueReference = ref value;
            return value == null && gameObject.TryGetComponent(out valueReference);
        }

        /// <summary>
        /// If null, causes <see href="GetComponent"/>, otherwise does nothing
        /// </summary>
        public static bool GetIfNull<T>(this GameObject gameObject, T value) where T : Component
        {
            ref T valueReference = ref value;
            return value == null && gameObject.TryGetComponent(out valueReference);
        }

        #endregion

        #region IfNotNull & IfNull
        /// <summary>
        /// Invokes an action if not null
        /// </summary>
        public static bool IfNotNull(this object value, Action action)
        {
            if (value != null)
                action?.Invoke();

            return value != null;
        }

        /// <summary>
        /// Invokes an action if not null
        /// </summary>
        public static bool IfNotNull<T>(this T value, Action<T> action)
        {
            if (value != null)
                action?.Invoke(value);

            return value != null;
        }




        /// <summary>
        /// Invokes an action if null
        /// </summary>
        public static bool IfNull(this object value, Action action)
        {
            if (value == null)
                action?.Invoke();

            return value == null;
        }

        #endregion

        #region IfNotNull & IfNull
        /// <summary>
        /// Add value if is not null
        /// </summary>
        public static bool AddIfNotNull<T>(this ICollection<T> collection, T value, bool checkContains = false)
        {
            if (collection != null && value != null)
            {
                if (checkContains && collection.Contains(value))
                    return false;

                collection.Add(value);
            }

            return value != null;
        }

        #endregion

        #region InitializeIfNotNull
        /// <summary>
        /// Invokes <see cref="IInitializable.Initialize"/> if not null
        /// </summary>
        public static bool InitializeIfNotNull<T>(this T value) where T : IInitializable
        {
            //return value.IfNotNull(value.Initialize);
            return value != null ? value.Initialize() : false;
        }
        #endregion

        #region GetAs

        /// <summary>
        /// Finds and returns
        /// </summary>
        public static IEnumerable<T> GetAs<T>(this IEnumerable enumerable) where T : class
        {
            return from object obj in enumerable
                   where obj is T
                   select obj as T;
        }

        #endregion

        #region ConvertInput

        /// <summary>
        /// Converts Vector2 input to Vector3
        /// </summary>
        public static Vector3 ConvertInputToVector3(this Vector2 input)
        {
            return new Vector3(input.x, 0.0f, input.y);
        }

        /// <summary>
        /// Converts Vector3 input to Vector2
        /// </summary>
        public static Vector2 ConvertInputToVector2(this Vector3 input)
        {
            return new Vector2(input.x, input.z);
        }

        #endregion

        #region ConvertSecondsToMiliseconds

        /// <summary>
        /// Converts float seconds to int miliseconds
        /// </summary>
        public static int ConvertSecondsToMiliseconds(this float seconds)
        {
            return (int)(seconds * 1000);
        }


        #endregion

        #region Renew & Reset Token

        /// <summary>
        /// Cancels and disposes a token
        /// </summary>
        public static void ResetToken(this CancellationTokenSource source)
        {
            if (source != null && source.Token.CanBeCanceled)
                source?.Cancel();

            source?.Dispose();
        }


        #endregion

        #region Vector Max

        /// <summary>
        /// Returns max axis in vector
        /// </summary>
        public static float Max(this Vector3 value)
        {
            Vector3 absValue = value.Abs();
            return ExtendedMath.Max(absValue.x, absValue.y, absValue.z);
        }

        #endregion



        // Gameplay
        #region Stun

        /// <summary>
        /// Disables all movement abillities for the given delay
        /// </summary>
        [Server]
        public static async UniTaskVoid Stun(this Entity entity, float delay = 0f, CancellationToken token = default)
        {
            // Todo: create movement interface or some kind of group for finding movement abillities
            List<Abillity> movementAbillities = new()
            {
                entity.FindAbillity<AbillityMove>(),
                entity.FindAbillity<AbillityRotater>(),
                entity.FindAbillity<AbillityJump>()
            };

            foreach (Abillity abillity in movementAbillities)
            {
                if (abillity != null)
                    abillity.enabled = false;
            }

            if (delay <= 0)
                await UniTask.NextFrame(cancellationToken: token);

            else
                await UniTask.Delay(delay.ConvertSecondsToMiliseconds(), cancellationToken: token);


            foreach (Abillity abillity in movementAbillities)
            {
                if (abillity != null)
                    abillity.enabled = true;
            }
        }

        #endregion

        #region Teleportate

        /// <summary>
        /// Teleportates gameobject via Rigidbody or Transform
        /// </summary>
        [Server]
        public static void Teleportate(this GameObject gameObject, Vector3 position, Quaternion rotation)
        {
            // Try get predicted rigidbody and move them
            if (gameObject.TryGetComponent<PredictedRigidbody>(out var predictedRb))
                predictedRb.predictedRigidbody.Move(position, rotation);

            // Try get rigidbody and move them
            else if (gameObject.TryGetComponent<Rigidbody>(out var rb))
                rb.Move(position, rotation);

            // Otherwise, move via transform
            else
                gameObject.transform.SetPositionAndRotation(position, rotation);
        }

        /// <summary>
        /// Teleportates gameobject via Rigidbody or Transform. If ignoreRotation is true, gameObject will not be rotated
        /// </summary>
        [Server]
        public static void Teleportate(this GameObject gameObject, Transform point, bool ignoreRotation = false)
            => gameObject.Teleportate(point.position, ignoreRotation ? gameObject.transform.rotation : point.rotation);

        #endregion

        #region FindByUid

        /// <summary>
        /// Tryes find identity and component by id
        /// </summary>
        [Server]
        public static bool TryFindByID<TComponent>(this uint ID, out TComponent component) where TComponent : Component
        {
            return NetworkClient.spawned.GetValueOrDefault(ID).TryGetComponent(out component);
        }

        #endregion
    }


    public static class ExtendedMath
    {
        public static int Max(params int[] values)
        {
            return Enumerable.Max(values);
        }

        public static float Max(params float[] values)
        {
            return Enumerable.Max(values);
        }
    }
}
