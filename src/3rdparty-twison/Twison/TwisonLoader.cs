using System;
using System.Collections.Generic;
using System.Linq;
using Twison.Errors;
using UnityEngine;

namespace Twison
{
    public class TwisonLoader
    {
        private TwisonLoaderOptions _options;

        public TwisonLoader()
        {
            _options = TwisonLoaderOptions.GetDefaultOptions();
        }

        public TwisonLoader(TwisonLoaderOptions options)
        {
            _options = options;
        }
        
        public TwisonStory Load(string source)
        {
            var story = LoadStoryFromJson(source);
            ValidateStory(story);
            ReconcilePassageReferences(story);
            ApplyOptions(story, _options);
            return story;
        }

        private static TwisonStory LoadStoryFromJson(string source)
        {
            try
            {
                // JSONUtility can't parse keys like "creator-version"
                source = source.Replace(@"""creator-version"":", @"""creatorVersion"":");
                return JsonUtility.FromJson<TwisonStory>(source);
            }
            catch (Exception error)
            {
                Debug.LogError("Failed to load Twison data from source");
                Debug.LogException(error);
                return null;
            }
        }

        private void ValidateStory(TwisonStory story)
        {
            Default(story, (s) => s.passages != null, (s) => s.passages = new List<TwisonPassage>());
            Default(story, (s) => s.name != null, (s) => s.name = Guid.NewGuid().ToString());
            Default(story, (s) => s.creator != null, (s) => s.creator = "");
            Default(story, (s) => s.ifid != null, (s) => s.ifid = Guid.NewGuid().ToString());
            Default(story, (s) => s.creatorVersion != null, (s) => s.creatorVersion = "");
            Default(story, (s) => s.startnode != null, (s) => s.startnode = "");
            foreach (var passage in story.passages)
            {
                Default(passage, (p) => p.name != null, (p) => p.name = Guid.NewGuid().ToString());
                Default(passage, (p) => p.text != null, (p) => p.text = "");
                Default(passage, (p) => p.tags != null, (p) => p.tags = new List<string>());
                Default(passage, (p) => p.links != null, (p) => p.links = new List<TwisonLink>());
                Guard(passage, (p) => !string.IsNullOrWhiteSpace(p.pid), (p) => $"Invalid passage, no PID for {p.name}");
                foreach (var link in passage.links)
                {
                    Default(link, (l) => l.name != null, (l) => l.name = "");
                    Default(link, (l) => l.link != null, (l) => l.link = "");
                    Guard(link, (l) => !string.IsNullOrWhiteSpace(l.pid), (l) => $"Invalid passage in link, no PID for {l.name}");
                }
            }
        }

        private void ReconcilePassageReferences(TwisonStory story)
        {
            foreach (var passage in story.passages)
            {
                foreach (var link in passage.links)
                {
                    var matchingPassage = story.passages.FirstOrDefault(i => i.pid == link.pid);
                    if (matchingPassage == null)
                    {
                        throw new TwisonValidationError($"No passage to match link PID {link.pid}", link);
                    }

                    link.passage = matchingPassage;
                }
            }

            // Try to resolve start node, if one is set
            if (!string.IsNullOrWhiteSpace(story.startnode))
            {
                story.startNodePassage = story.passages.FirstOrDefault(i => i.pid == story.startnode);
                if (story.startNodePassage == null)
                {
                    throw new TwisonValidationError($"No passage to match startnode PID {story.startnode}", story);
                }
            }
        }

        private void ApplyOptions(TwisonStory story, TwisonLoaderOptions options)
        {
            var utility = new TwisonUtility();
            
            if (options.removeLinkTextFromPassages)
            {
                utility.RemoveLinksFromPassages(story);
            }
            
            if (options.collapseNewLines)
            {
                utility.CollapseNewLines(story, options.maxNewLineChain);
            }
            
            if (options.trimPassages)
            {
                utility.TrimPassages(story);
            }
        }

        private void Default(TwisonStory story, Func<TwisonStory, bool> condition, Action<TwisonStory> applyDefault)
        {
            if (!condition(story))
            {
                applyDefault(story);
            }
        }

        private void Default(TwisonPassage passage, Func<TwisonPassage, bool> condition, Action<TwisonPassage> applyDefault)
        {
            if (!condition(passage))
            {
                applyDefault(passage);
            }
        }

        private void Default(TwisonLink link, Func<TwisonLink, bool> condition, Action<TwisonLink> applyDefault)
        {
            if (!condition(link))
            {
                applyDefault(link);
            }
        }

        private void Guard(TwisonPassage passage, Func<TwisonPassage, bool> condition, Func<TwisonPassage, string> error)
        {
            if (!condition(passage))
            {
                throw new TwisonValidationError(error(passage), passage);
            }
        }

        private void Guard(TwisonLink link, Func<TwisonLink, bool> condition, Func<TwisonLink, string> error)
        {
            if (!condition(link))
            {
                throw new TwisonValidationError(error(link), link);
            }
        }
    }
}