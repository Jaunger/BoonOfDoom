using UnityEngine;

[System.Serializable]
public class SerializableFlask : ISerializationCallbackReceiver
{
    [SerializeField] public int itemID;
    [SerializeField] public int flaskHealAmount;

    public FlaskItem GetFlask()
    {
        FlaskItem flask = WorldItemDatabase.instance.GetFlaskFromSerializedData(this);
        return flask;
    }

    public void OnBeforeSerialize()
    {
    }
    public void OnAfterDeserialize()
    {
    }
}
