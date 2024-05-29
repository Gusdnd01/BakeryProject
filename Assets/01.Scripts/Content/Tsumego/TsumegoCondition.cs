using UnityEditor;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "SO/Tsumego/Condition")]
public class TsumegoCondition : ScriptableObject
{
    public virtual bool CheckCondition() { return false; }
}
