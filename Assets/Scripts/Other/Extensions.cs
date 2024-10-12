using System;
using System.Collections;
using UnityEngine;
using Scripts.Gameplay.Abillities;
using System.Collections.Generic;
using System.Linq;

namespace Scripts.Extensions
{
    public static class Extensions
    {
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
        public static T FindByType<T>(this IEnumerable enumerable) where T : class
        {
            foreach (var t in enumerable)
            {
                if (t is T)
                {
                    return t as T;
                }
            }
            return null;
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
        /// Invokes an action if null
        /// </summary>
        public static bool IfNull(this object value, Action action)
        {
            if (value == null)
                action?.Invoke();

            return value == null;
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

        #region GetAs

        /// <summary>
        /// Converts Vector2 input to Vector3
        /// </summary>
        public static Vector3 ConvertInput(this Vector2 input)
        {
            return new Vector3(input.x, 0.0f, input.y);
        }

        #endregion
    }
}
