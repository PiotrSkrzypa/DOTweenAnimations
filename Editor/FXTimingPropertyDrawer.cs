using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PSkrzypa.UnityFX.Editor
{
	[CustomPropertyDrawer(typeof(FXTiming), true)]
	public class FXTimingPropertyDrawer : PropertyDrawer
	{
		public VisualTreeAsset visualTree;
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();
            visualTree.CloneTree(root);
            ClampFloat(root, "Duration");
            ClampFloat(root, "InitialDelay");
            ClampFloat(root, "CooldownDuration");
            ClampFloat(root, "DelayBetweenRepeats");
            ClampInt(root, "NumberOfRepeats");
            return root;
        }

        private static void ClampFloat(VisualElement root, string elementName)
        {
            FloatField duration = root.Q<FloatField>(elementName);
            duration.RegisterValueChangedCallback(evt =>
            {
                float clamped = evt.newValue < 0 ? 0 : evt.newValue;
                if (!Mathf.Approximately(evt.newValue, clamped))
                    duration.value = clamped;
            });
        }
        private static void ClampInt(VisualElement root, string elementName)
        {
            IntegerField duration = root.Q<IntegerField>(elementName);
            duration.RegisterValueChangedCallback(evt =>
            {
                int clamped = evt.newValue < 0 ? 0 : evt.newValue;
                if (!Mathf.Approximately(evt.newValue, clamped))
                    duration.value = clamped;
            });
        }
    }

}