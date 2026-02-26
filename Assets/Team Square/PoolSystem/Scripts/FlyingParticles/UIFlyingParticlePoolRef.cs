using Lean.Pool;
using UnityEngine;

namespace Pinpin
{
    [CreateAssetMenu(fileName = "UIFlyingParticlePoolRef", menuName = "ScriptableObjects/UIFlyingParticlePoolRef")]
    public class UIFlyingParticlePoolRef : ScriptableObject
	{
        public LeanUIFlyingParticlePool pool;
	}
}