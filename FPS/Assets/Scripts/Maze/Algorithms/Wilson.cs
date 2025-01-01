using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WilsonCell : Cell
{
    [Tooltip("경로가 만들어 졌을 때 다음 셀의 참조")]
    public WilsonCell next;
    [Tooltip("이 셀의 미로에 포함되어 있는지 설정하고 확인")]
    public bool isMazeMember;

    public WilsonCell(int x, int y) : base(x, y)
    {
        next = null;
        isMazeMember = false;
    }
}

public class Wilson : MonoBehaviour
{

}
