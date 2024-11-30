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
        public static void IncreaseInBounds(this ref int index, Array array)
        {
            index++;

            if (index >= array.Length - 1)
                index = 0;
        }

        /// <summary>
        /// Safely increments index within array length
        /// </summary>
        public static void IncreaseInBounds(this ref uint index, Array array)
        {
            index++;

            if (index >= array.Length - 1)
                index = 0;
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
            return value.IfNotNull(value.Initialize);
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
        /// Cancels, disposes and creates a new token
        /// </summary>
        public static void RenewToken(this CancellationTokenSource source)
        {
            source.ResetToken();

            ref CancellationTokenSource reference = ref source;
            reference = new CancellationTokenSource();
        }

        /// <summary>
        /// Cancels and disposes a token
        /// </summary>
        public static void ResetToken(this CancellationTokenSource source)
        {
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
            return ExtendedMath.Max(value.x, value.y, value.z);
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
