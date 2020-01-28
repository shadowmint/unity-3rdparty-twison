using System;
using UnityEngine;
using UnityEngine.UI;

namespace Demo
{
    public class DemoActionsButton : MonoBehaviour
    {
        public Button button;
        
        public Text text;
        
        public Action OnPress { get; set; }

        public void OnPressEvent()
        {
            OnPress?.Invoke();
        }
    }
}