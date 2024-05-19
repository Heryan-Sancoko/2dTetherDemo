using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif

public class TapHoldInteraction : IInputInteraction
{
    public float tapDelay = 0.5f;

    private float pressTime = 0;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize() { }

    static TapHoldInteraction()
    {
        InputSystem.RegisterInteraction<TapHoldInteraction>();
    }

    void IInputInteraction.Process(ref InputInteractionContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Waiting:
                if (context.ControlIsActuated())
                {
                    if ((Time.realtimeSinceStartup - pressTime) < tapDelay)
                    {
                        context.Performed();
                    }

                    pressTime = Time.realtimeSinceStartup;
                }
                break;
        }
    }

    void IInputInteraction.Reset() { }
}