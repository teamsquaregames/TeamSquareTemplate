using MyBox;
using Pinpin;
using UnityEngine;

public class FloatingTextManager : Singleton<FloatingTextManager>
{
    [SerializeField] private UIFloatingTextPoolRef m_uiFloatingTextPoolRef;
    [SerializeField] private WorldFloatingTextPoolRef m_worldFloatingTextPoolRef;
    [SerializeField] private FloatingTextConfig m_defaultConfig;

    public void SpawnUIText(Vector3 screenPos, string text, FloatingTextConfig config)
    {
        UIFloatingText spawnedText = m_uiFloatingTextPoolRef.pool.Spawn(screenPos, Quaternion.identity, m_uiFloatingTextPoolRef.pool.transform);
        spawnedText.Init(text, config, m_uiFloatingTextPoolRef.pool);
        spawnedText.Play();
    }
    
    public void SpawnWorldText(Vector3 worldpos, string text, FloatingTextConfig config = null)
    {
        WorldFloatingText spawnedText = m_worldFloatingTextPoolRef.pool.Spawn(worldpos, Quaternion.identity, m_worldFloatingTextPoolRef.pool.transform);
        spawnedText.Init(text, config != null ? config : m_defaultConfig, m_worldFloatingTextPoolRef.pool);
        spawnedText.Play();
    }
}