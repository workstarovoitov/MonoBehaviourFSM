using UnityEditor;
using UnityEngine;
using UnityEditor.Animations;

public class AnimatorValidator
{
    [MenuItem("Tools/Find Broken Animator States")]
    static void FindBrokenStates()
    {
        var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>("Assets/Characters/Cat/Animations/Cat.controller");
        foreach (var layer in controller.layers)
        {
            foreach (var state in layer.stateMachine.states)
            {
                if (state.state.motion == null)
                {
                    Debug.LogWarning($"Broken state: {state.state.name} in layer {layer.name}");
                }
            }
        }
    }
}