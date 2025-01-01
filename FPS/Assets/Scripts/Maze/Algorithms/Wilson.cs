using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WilsonCell : Cell
{
    [Tooltip("��ΰ� ����� ���� �� ���� ���� ����")]
    public WilsonCell next;
    [Tooltip("�� ���� �̷ο� ���ԵǾ� �ִ��� �����ϰ� Ȯ��")]
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
