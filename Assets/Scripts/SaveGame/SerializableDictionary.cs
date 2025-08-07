using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableDictionary<K, V> : Dictionary<K, V>, ISerializationCallbackReceiver
{
    [SerializeField] private List<K> keys = new();
    [SerializeField] private List<V> values = new();

    public void OnAfterDeserialize()
    {
        Clear();
        if (keys.Count != values.Count)
        {
            Debug.LogError("Keys and values count mismatch!");
            return;
        }

        for (int i = 0; i < keys.Count; i++)
        {
            Add(keys[i], values[i]);
        }
    }

    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        foreach (KeyValuePair<K, V> pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }
}
