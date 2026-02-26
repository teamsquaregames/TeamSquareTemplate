using System;
using MyBox;
using Pinpin;
using UnityEngine;

public class FlyingParticleManager : Singleton<FlyingParticleManager>
{
    [SerializeField] private UIFlyingParticlePoolRef m_flyingParticlePoolRef;
    public UIFlyingParticle Spawn(Vector3 startScreenPos, Transform target, Sprite sprite, float duration, Action callback, double burstCount = 1)
    {
        UIFlyingParticle _spawnedParticle = m_flyingParticlePoolRef.pool.Spawn(startScreenPos, Quaternion.identity, m_flyingParticlePoolRef.pool.transform);
        _spawnedParticle.Initialize(target, sprite, m_flyingParticlePoolRef.pool, duration, callback, burstCount);
        
        return _spawnedParticle;
    }

    public void DestroyParticle(UIFlyingParticle particle)
    {
        if (m_flyingParticlePoolRef.pool.IsInPool(particle.gameObject))
        {
            m_flyingParticlePoolRef.pool.Despawn(particle);
        }
    }
}