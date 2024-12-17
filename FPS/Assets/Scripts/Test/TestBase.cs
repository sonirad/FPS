using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestBase : MonoBehaviour
{
    public int seed = -1;
    const int allRandom = -1;
    protected Test_Input_Actions inputActions;

    private void Awake()
    {
        inputActions = new Test_Input_Actions();

        if (seed != allRandom)
        {
            UnityEngine.Random.InitState(seed);
        }
    }

    protected virtual void OnEnable()
    {
        inputActions.Test.Enable();
        inputActions.Test.Test_01.performed += OnTest1;
        inputActions.Test.Test_02.performed += OnTest2;
        inputActions.Test.Test_03.performed += OnTest3;
        inputActions.Test.Test_04.performed += OnTest4;
        inputActions.Test.Test_05.performed += OnTest5;
        inputActions.Test.R_Click.performed += OnTestRClick;
        inputActions.Test.L_Click.performed += OnTestLClick;
    }

    protected virtual void OnDisable()
    {
        inputActions.Test.Test_01.performed -= OnTest1;
        inputActions.Test.Test_02.performed -= OnTest2;
        inputActions.Test.Test_03.performed -= OnTest3;
        inputActions.Test.Test_04.performed -= OnTest4;
        inputActions.Test.Test_05.performed -= OnTest5;
        inputActions.Test.R_Click.performed -= OnTestRClick;
        inputActions.Test.L_Click.performed -= OnTestLClick;
        inputActions.Test.Disable();
    }

    protected virtual void OnTestRClick(InputAction.CallbackContext context)
    {

    }

    protected virtual void OnTestLClick(InputAction.CallbackContext context)
    {

    }

    protected virtual void OnTest5(InputAction.CallbackContext context)
    {

    }

    protected virtual void OnTest4(InputAction.CallbackContext context)
    {

    }

    protected virtual void OnTest3(InputAction.CallbackContext context)
    {

    }

    protected virtual void OnTest2(InputAction.CallbackContext context)
    {

    }

    protected virtual void OnTest1(InputAction.CallbackContext context)
    {

    }
}
