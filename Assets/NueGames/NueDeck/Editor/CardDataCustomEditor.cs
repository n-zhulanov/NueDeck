using NueGames.NueDeck.Scripts.Data.Collection;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace NueGames.NueDeck.Editor
{
    public class CardDataAssetHandler
    {
#if UNITY_EDITOR
        

        [OnOpenAsset]
#if UNITY_6000_3_OR_NEWER
        public static bool OpenEditor(EntityId instanceId, int line)
#else
        public static bool OpenEditor(int instanceId, int line)
#endif
        {
#if UNITY_6000_3_OR_NEWER
            CardData obj = EditorUtility.EntityIdToObject(instanceId) as CardData;
#else
            CardData obj = EditorUtility.InstanceIDToObject(instanceId) as CardData;
#endif
            if (obj != null)
            {
                CardEditorWindow.OpenCardEditor(obj);
                return true;
            }
            return false;
        }
#endif
    }
    
    [CustomEditor(typeof(CardData))]
    public class CardDataCustomEditor : UnityEditor.Editor
    {
#if UNITY_EDITOR
        

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Open in editor"))
            {
                CardEditorWindow.OpenCardEditor((CardData)target);
            }
        }
#endif
    }
}
