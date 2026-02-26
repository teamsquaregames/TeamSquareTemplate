using Lean.Pool;
using UnityEngine;

namespace Pinpin
{
	public class UIFlyingParticlePoolRefSetter : MonoBehaviour
	{
		[SerializeField] private UIFlyingParticlePoolRef poolRef;

        private void Awake()
        {
            poolRef.pool = GetComponent<LeanUIFlyingParticlePool>();
        }
    }
}