using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour
{
    public MyInputActions inputActions { get; private set; }

    public Vector2 MoveInput { get; private set; }
    public bool PullUpTriggered { get; private set; }
    public bool PullDownTriggered { get; private set; }
    public Vector2 NavigateInput { get; private set; }
    public bool SubmitTriggered { get; private set; }
    public bool CancelTriggered { get; private set; }

    void Awake()
    {
        inputActions = new MyInputActions();

        // 움직임 관련
        inputActions.Move.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        inputActions.Move.Move.canceled += ctx => MoveInput = Vector2.zero;

        inputActions.PullUp.PullUp.performed += ctx => PullUpTriggered = true;
        inputActions.PullUp.PullUp.canceled += ctx => PullUpTriggered = false;

        inputActions.PullDown.PullDown.performed += ctx => PullDownTriggered = true;
        inputActions.PullDown.PullDown.canceled += ctx => PullDownTriggered = false;


        // UI & 메뉴 관련
        inputActions.UI.Navigate.performed += ctx => NavigateInput = ctx.ReadValue<Vector2>();
        inputActions.UI.Navigate.canceled += ctx => NavigateInput = Vector2.zero;

        inputActions.UI.Submit.performed += ctx => SubmitTriggered = true;
        inputActions.UI.Cancel.performed += ctx => CancelTriggered = true;
    }

    void OnEnable()
    {
        EnablePlayerInput();
    }

    void OnDisable()
    {
        inputActions.Disable();
    }

    public void EnablePlayerInput()
    {
        inputActions.UI.Disable();
        inputActions.Move.Enable();
        inputActions.PullUp.Enable();
        inputActions.PullDown.Enable();
    }

    public void EnableUIInput()
    {
        inputActions.Move.Disable();
        inputActions.PullUp.Disable();
        inputActions.PullDown.Disable();
        inputActions.UI.Enable();
    }

    // UI 버튼 클릭 후 PullUp/Down 상태 초기화
    public void ResetTriggers()
    {
        PullUpTriggered = false;
        PullDownTriggered = false;
        CancelTriggered = false;
    }
}
