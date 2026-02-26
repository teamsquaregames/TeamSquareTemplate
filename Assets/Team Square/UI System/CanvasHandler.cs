using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils;
using Utils.UI;


public class CanvasHandler : MonoBehaviour
{
    [TitleGroup("Dependencies"), Required]
    [SerializeField] private Canvas m_canvas;

    private UIContainer[] m_containers;


    public virtual void Init()
    {
        // this.Log("Init");

        m_containers = GetComponentsInChildren<UIContainer>(true);
        // this.Log($"Found {m_panels.Length} panels to init.");
        foreach (UIContainer container in m_containers)
        {
            container.onOpen += Enable;
            container.onClose += OnContainerClose;
        }
    }

    private void Enable()
    {
        // this.Log("open");

        //gameObject.SetActive(true);
        m_canvas.enabled = true;
    }

    private void OnContainerClose()
    {
        foreach (UIContainer container in m_containers)
        {
            if (container.IsOpen)
                return;
        }
        Disable();
    }

    private void Disable()
    {
        //gameObject.SetActive(false);
        m_canvas.enabled = false;
    }
}