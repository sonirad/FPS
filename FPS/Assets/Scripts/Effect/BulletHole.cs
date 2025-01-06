using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class BulletHole : RecycleObject
{
    private VisualEffect effect;
    private float duration;

    readonly int OnStartEventID = Shader.PropertyToID("OnStart");
    readonly int DurationID = Shader.PropertyToID("Duration");
    readonly int DurationRangeID = Shader.PropertyToID("Duration_Range");
    readonly int DebrisSpawnRlectID = Shader.PropertyToID("Debris_Spawn_Reflect");

    private void Awake()
    {
        effect = GetComponent<VisualEffect>();
        float durationRange = effect.GetFloat(DurationRangeID);
        // ���� duration +- durationRange�� ���� �����ϱ�
        duration = effect.GetFloat(DurationID) + durationRange;
    }

    /// <summary>
    /// �Ѿ� ���� ����Ʈ �ʱ�ȭ��
    /// </summary>
    /// <param name="position">������ ��ġ</param>
    /// <param name="normal">������ ���� ���</param>
    /// <param name="reflect">�Ѿ� �ݻ� ����</param>
    public void Initialize(Vector3  position, Vector3 normal, Vector3 reflect)
    {
        // ����Ʈ ��ġ ����
        transform.position = position;
        // ����Ʈ ȸ�� ����
        transform.forward = -normal;

        // ���� �ݻ� ����
        effect.SetVector3(DebrisSpawnRlectID, reflect);
        // ����Ʈ ��� ����
        effect.SendEvent(OnStartEventID);
        // ����� ����Ǿ����� �ٽ� Ǯ�� �ǵ�����
        StartCoroutine(LifeOver(duration));
    }
}
