using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test08_Gun : TestBase
{
    public Revolver revolver;
    public Shotgun shotgun;
    public AssaultRifle assaultRifle;

#if UNITY_EDITOR
    protected override void OnTest1(InputAction.CallbackContext context)
    {
        revolver.Test_Fire();
        Debug.Log("리볼버 발사 완료");
    }

    protected override void OnTest2(InputAction.CallbackContext context)
    {
        revolver.ReLoad();
        Debug.Log("리볼버 장전 완료");
    }

    protected override void OnTest3(InputAction.CallbackContext context)
    {
        shotgun.Test_Fire();
        Debug.Log("샷건 발사 완료");
    }

    protected override void OnTest4(InputAction.CallbackContext context)
    {
        assaultRifle.Test_Fire(!context.canceled);
        Debug.Log("어썰트 라이플 발사 완료");
    }
#endif
}
