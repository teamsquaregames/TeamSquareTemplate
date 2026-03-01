using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using Utils;
using Utils.UI;

public class CanvasHandler : MonoBehaviour
{
    [TitleGroup("Dependencies"), Required] [SerializeField]
    private Canvas m_canvas;

    private UIContainer[] m_containers;

    public virtual void Init()
    {
        m_containers = GetComponentsInChildren<UIContainer>(true);

        foreach (UIContainer container in m_containers)
            container.Init();

        m_canvas.enabled = false;
    }

    [Button]
    public virtual void Open()
    {
        m_canvas.enabled = true;

        foreach (UIContainer container in m_containers)
        {
            if (container.EnableByDefault)
                container.Show();
            else
                container.Hide();
        }
    }
    
    [Button]
    public virtual void Close()
    {
        foreach (UIContainer container in m_containers)
            container.Hide();

        m_canvas.enabled = false;
    }
    
    public T GetContainer<T>() where T : UIContainer
    {
        T container = m_containers.OfType<T>().FirstOrDefault();

        if (container == null)
            this.LogWarning($"Container of type {typeof(T)} not found in {name}.");

        return container;
    }
}