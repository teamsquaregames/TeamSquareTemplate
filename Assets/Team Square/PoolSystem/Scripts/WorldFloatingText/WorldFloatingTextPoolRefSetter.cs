using Lean.Pool;
using UnityEngine;

namespace Pinpin
{
	public class WorldFloatingTextPoolRefSetter : MonoBehaviour
	{
		[SerializeField] private WorldFloatingTextPoolRef poolRef;

        private void Awake()
        {
            poolRef.pool = GetComponent<LeanWorldFloatingTextPool>();
        }
    }
}