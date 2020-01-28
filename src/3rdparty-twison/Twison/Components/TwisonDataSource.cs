using System;
using UnityEngine;

namespace Twison.Components
{
    public class TwisonDataSource : MonoBehaviour
    {
        public bool loadAsset = true;

        public TextAsset source;

        public TwisonLoaderOptions options;

        public TwisonStory story;

        public Action<TwisonDataSource> OnStoryLoaded { get; set; }

        public void Update()
        {
            if (!loadAsset) return;
            if (source == null) return;
            loadAsset = false;
            story = new TwisonLoader(options).Load(source.text);
            OnStoryLoaded?.Invoke(this);
        }
    }
}