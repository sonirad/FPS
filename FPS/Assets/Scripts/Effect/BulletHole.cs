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
        // 원래 duration +- durationRange가 실제 범위니까
        duration = effect.GetFloat(DurationID) + durationRange;
    }

    /// <summary>
    /// 총알 구명 이팩트 초기화용
    /// </summary>
    /// <param name="position">생성될 위치</param>
    /// <param name="normal">생성될 면의 노멀</param>
    /// <param name="reflect">총알 반사 방향</param>
    public void Initialize(Vector3  position, Vector3 normal, Vector3 reflect)
    {
        // 이팩트 위치 설정
        transform.position = position;
        // 이팩트 회전 설정
        transform.forward = -normal;

        // 파편 반사 방향
        effect.SetVector3(DebrisSpawnRlectID, reflect);
        // 이팩트 재생 시작
        effect.SendEvent(OnStartEventID);
        // 충분히 재생되었으면 다시 풀로 되돌리기
        StartCoroutine(LifeOver(duration));
    }
}
