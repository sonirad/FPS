using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test18_CellConer : TestBase
{
    Direction dir = Direction.North;

    protected override void OnTest1(InputAction.CallbackContext context)
    {
        dir++;

        Debug.Log(dir);
    }
}
