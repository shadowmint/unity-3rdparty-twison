using System.Collections.Generic;
using DefaultNamespace;
using Twison;
using Twison.Components;
using UnityEngine;
using UnityEngine.UI;

namespace Demo
{
    public class DemoActionsCanvasController : MonoBehaviour
    {
        public TwisonDataSource story;

        public Text passageText;

        public GameObject actionsArea;

        public DemoActionsButton actionsTemplate;

        private TwisonRuntime _runtime;

        private readonly List<DemoActionsButton> _actions = new List<DemoActionsButton>();

        public void Start()
        {
            story.OnStoryLoaded = ((s) =>
            {
                _runtime = new TwisonRuntime(s.story, new TwisonTagFilterBehaviour(), true);
                ApplyStoryUpdate();
            });
        }

        public void ApplyStoryUpdate()
        {
            if (_runtime.Active == null) return;
            RemoveOldActions();
            SetPassage();
        }

        private void SetPassage()
        {
            passageText.text = _runtime.Active.text;
            foreach (var link in _runtime.Links)
            {
                var instance = Object.Instantiate(actionsTemplate, actionsArea.transform);
                instance.OnPress = () =>
                {
                    Debug.Log($"Did an action: {link.link}");
                    _runtime.GoToPassage(link.passage);
                    ApplyStoryUpdate();
                };
                instance.text.text = link.name;
                _actions.Add(instance);
            }

            if (_runtime.HasHistory)
            {
                var instance = Object.Instantiate(actionsTemplate, actionsArea.transform);
                instance.OnPress = () =>
                {
                    Debug.Log($"Did an action: Go back");
                    _runtime.GoToPreviousPassage();
                    ApplyStoryUpdate();
                };
                instance.text.text = "Back";
                _actions.Add(instance);
            }
        }

        private void RemoveOldActions()
        {
            _actions.ForEach(i => Destroy(i.gameObject));
            _actions.Clear();
        }
    }
}