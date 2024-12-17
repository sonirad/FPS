using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test02_CrossHair : TestBase
{
    public CrossHair crossHair;
    public float expendAmount = 30.0f;

    protected override void OnTestLClick(InputAction.CallbackContext context)
    {
        crossHair.Expend(expendAmount);
    }
}