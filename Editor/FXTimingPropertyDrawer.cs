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
            BindField<FloatField>(root, property, "Duration");
            BindField<FloatField>(root, property, "InitialDelay");
            BindField<FloatField>(root, property, "CooldownDuration");
            BindField<FloatField>(root, property, "DelayBetweenRepeats");
            var numberOfRepeats = BindField<IntegerField>(root, property, "NumberOfRepeats");
            BindField<Toggle>(root, property, "ContributeToTotalDuration");
            var repeatForever = BindField<Toggle>(root, property, "RepeatForever");
            BindField<Toggle>(root, property, "TimeScaleIndependent");
           
            ClampFloat(root, "Duration");
            ClampFloat(root, "InitialDelay");
            ClampFloat(root, "CooldownDuration");
            ClampFloat(root, "DelayBetweenRepeats");
            ClampInt(root, "NumberOfRepeats");

            var numberOfRepeatsField = numberOfRepeats;
            var repeatForeverField = repeatForever;

            if (repeatForeverField != null && numberOfRepeatsField != null)
            {
                // Initial state
                numberOfRepeatsField.SetEnabled(!repeatForeverField.value);

                // Respond to toggle change
                repeatForeverField.RegisterValueChangedCallback(evt =>
                {
                    numberOfRepeatsField.SetEnabled(!evt.newValue);
                });
            }
            return root;
        }
        private T BindField<T>(VisualElement root, SerializedProperty parent, string fieldName)
    where T : BindableElement
        {
            var field = root.Q<T>(fieldName);
            var relative = parent.FindPropertyRelative(fieldName);
            field?.BindProperty(relative);
            return field;
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