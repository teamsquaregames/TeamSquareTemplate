using Lean.Pool;
using UnityEngine;

namespace Pinpin
{
    [CreateAssetMenu(fileName = "WorldFloatingTextPoolRef", menuName = "ScriptableObjects/WorldFloatingTextPoolRef")]
    public class WorldFloatingTextPoolRef : ScriptableObject
	{
        public LeanWorldFloatingTextPool pool;
	}
}