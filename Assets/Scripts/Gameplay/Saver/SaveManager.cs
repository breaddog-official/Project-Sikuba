using Cysharp.Threading.Tasks;
using System;
using System.IO;
using UnityEngine;

namespace Scripts.SaveManagement
{
    public static class SaveManager
    {
        #region Save

        public static bool Save(string value, string path)
        {
            try
            {
                File.WriteAllText(path, value);
                return true;
            }
            catch (Exception exp)
            {
                Debug.LogException(exp);
                return false;
            }
        }

        public static async UniTask<bool> SaveAsync(string value, string path)
        {
            try
            {
                await File.WriteAllTextAsync(path, value);
                return true;
            }
            catch (Exception exp)
            {
                Debug.LogException(exp);
                return false;
            }
        }

        #endregion

        #region Load

        public static string Load(string path)
        {
            return File.ReadAllText(path);
        }

        public static async UniTask<string> LoadAsync(string path)
        {
            return await File.ReadAllTextAsync(path);
        }

        public static bool TryLoad(string path, out string value)
        {
            try
            {
                value = Load(path);
                return true;
            }
            catch (Exception exp)
            {
                Debug.LogException(exp);

                value = string.Empty;
                return false;
            }
        }

        #endregion


        #region SerializeAndSave

        public static bool SerializeAndSave(object value, string path, Serializer serializer)
        {
            try
            {
                string serialized = serializer.Serialize(value);
                return Save(serialized, path);
            }
            catch (Exception exp)
            {
                Debug.LogException(exp);
                return false;
            }
        }

        public static async UniTask<bool> SerializeAndSaveAsync(object value, string path, Serializer serializer)
        {
            try
            {
                string serialized = await serializer.SerializeAsync(value);
                return await SaveAsync(serialized, path);
            }
            catch (Exception exp)
            {
                Debug.LogException(exp);
                return false;
            }
        }

        #endregion

        #region LoadAndDeserialize

        public static object LoadAndDeserialize(string path, Serializer serializer)
        {
            string loaded = Load(path);
            return serializer.Deserialize(loaded);
        }

        public static async UniTask<object> LoadAndDeserializeAsync(string path, Serializer serializer)
        {
            string loaded = Load(path);
            return await serializer.DeserializeAsync(loaded);
        }

        #endregion

        #region LoadAndDeserialize<T>

        public static T LoadAndDeserialize<T>(string path, Serializer serializer)
        {
            string loaded = Load(path);
            return serializer.Deserialize<T>(loaded);
        }

        public static async UniTask<T> LoadAndDeserializeAsync<T>(string path, Serializer serializer)
        {
            string loaded = Load(path);
            return await serializer.DeserializeAsync<T>(loaded);
        }

        #endregion

        #region TryLoadAndDeserialize

        public static bool TryLoadAndDeserialize(string path, Serializer serializer, out object value)
        {
            value = default;

            try
            {
                string loaded = Load(path);
                value = serializer.Deserialize(loaded);

                return true;
            }
            catch (Exception exp)
            {
                Debug.LogException(exp);
                return false;
            }
        }

        public static bool TryLoadAndDeserialize<T>(string path, Serializer serializer, out T value)
        {
            value = default;

            try
            {
                string loaded = Load(path);
                value = serializer.Deserialize<T>(loaded);

                return true;
            }
            catch (Exception exp)
            {
                Debug.LogException(exp);
                return false;
            }
        }

        #endregion
    }
}
