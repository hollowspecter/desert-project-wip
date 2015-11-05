///<summary>
///<copyright>(c) Vivien Baguio</copyright>
///http://www.vivienbaguio.com
///</summary>

using UnityEngine;
using UnityEditor;

public class YourClassAsset
{
    [MenuItem("Assets/Create/ControlMap")]
    public static void CreateAsset()
    {
        ScriptableObjectUtility.CreateAsset<ControlMapping>();
    }
}