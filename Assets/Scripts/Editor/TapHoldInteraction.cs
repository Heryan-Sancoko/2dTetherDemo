using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif

public class TapHoldInteraction : IInputInteraction
{
    public float firstTapTime = 0.2f;
    public float tapDelay = 0.5f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize() { }

    static TapHoldInteraction()
    {
        InputSystem.RegisterInteraction<TapHoldInteraction>();
    }

    void IInputInteraction.Process(ref InputInteractionContext context)
    {
        if (context.timerHasExpired)
        {
            context.Canceled();
            return;
        }

        switch (context.phase)
        {
            case InputActionPhase.Waiting:
                if (context.ControlIsActuated(firstTapTime))
                {
                    context.Started();
                    context.SetTimeout(tapDelay);
                }
                break;

            case InputActionPhase.Started:
                if (context.ControlIsActuated(0))
                    context.Performed();
                break;
        }
    }

    void IInputInteraction.Reset() { }
}