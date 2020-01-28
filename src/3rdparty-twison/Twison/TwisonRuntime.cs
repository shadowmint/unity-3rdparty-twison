using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DefaultNamespace;

namespace Twison
{
    public class TwisonRuntime
    {
        private TwisonPassage _active;

        private readonly TwisonStory _story;

        private readonly ITwisonRuntimeBehaviour _runtimeBehaviour;

        private readonly Stack<TwisonPassage> _history = new Stack<TwisonPassage>();

        public IEnumerable<TwisonLink> Links => _activeLinks;

        private List<TwisonLink> _activeLinks;

        public TwisonPassage Active => _active;
        
        public bool HasHistory => _history.Count > 0;

        public TwisonRuntime(TwisonStory story, ITwisonRuntimeBehaviour runtimeBehaviour, bool enterDefaultStartNode)
        {
            _story = story;
            _runtimeBehaviour = runtimeBehaviour;
            OnStartRuntime(story, enterDefaultStartNode);
        }

        private void OnStartRuntime(TwisonStory story, bool enterDefaultStartNode)
        {
            _active = null;
            _activeLinks = ResolveActiveLinks();
            if (enterDefaultStartNode)
            {
                GoToPassage(story.startNodePassage, false);
            }
        }

        public TwisonPassage FindByPid(string pid)
        {
            return _story.passages.FirstOrDefault(i => i.pid == pid);
        }

        public TwisonPassage FindByName(string name)
        {
            return _story.passages.FirstOrDefault(i => i.name == name);
        }

        public IEnumerable<TwisonPassage> FindByTag(string name)
        {
            return _story.passages.Where(i => i.tags.Contains(name));
        }

        public IEnumerable<TwisonPassage> FindByTagPattern(string tagPattern)
        {
            var regex = new Regex(tagPattern);
            return _story.passages.Where(i => i.tags.Any(j => regex.IsMatch(j)));
        }

        public bool GoToPassage(string linkName)
        {
            var link = Links.FirstOrDefault(i => i.name == linkName);
            if (link == null)
            {
                return false;
            }

            GoToPassage(link);
            return true;
        }

        public void GoToPassage(TwisonLink link)
        {
            GoToPassage(link.passage);
        }

        public void GoToPassage(TwisonPassage passage, bool recordHistory = true)
        {
            if (passage == null) return;

            if (_active != null)
            {
                _runtimeBehaviour.OnExitState(_active);
                if (recordHistory)
                {
                    PushHistoryEntry(_active);
                }
            }

            _runtimeBehaviour.OnEnterState(_active);
            _active = passage;
            _activeLinks = ResolveActiveLinks();
        }

        public bool GoToPreviousPassage()
        {
            var last = HasHistory ? _history.Pop() : null;
            if (last == null)
            {
                return false;
            }
            
            GoToPassage(last, false);
            return true;
        }

        private void PushHistoryEntry(TwisonPassage active)
        {
            _history.Push(active);
        }

        private List<TwisonLink> ResolveActiveLinks()
        {
            if (_active == null) return new List<TwisonLink>();
            return _runtimeBehaviour.Filter(_active);
        }
    }
}