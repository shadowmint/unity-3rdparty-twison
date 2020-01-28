using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Twison.Errors;
using UnityEngine;

namespace Twison
{
    public class TwisonUtility
    {
        /// <summary>
        /// Remove newlines around the edges of passages.
        /// </summary>
        public void TrimPassages(TwisonStory story)
        {
            foreach (var passage in story.passages)
            {
                passage.text = passage.text.Trim();
            }
        }

        /// <summary>
        /// Unity can't really render [[foo->bar]] as clickable links.
        /// The link data is already saved in the links object.
        /// This function removes any links in the text for each passage.
        /// </summary>
        public void RemoveLinksFromPassages(TwisonStory story)
        {
            var regex = new Regex("\\[\\[[^\\]]+\\]\\]");
            foreach (var passage in story.passages)
            {
                passage.text = regex.Replace(passage.text, "");
            }
        }

        /// <summary>
        /// When pruning links in a story, it is common to end up with a fragment like:
        /// Hello\n[[foo->bar]]\n[[foo2->bar2]]\n...
        ///
        /// Which maps as:
        /// Hello\n\n\n...
        ///
        /// This function will trim out duplicate line sequences and replace them.
        /// Sequences longer than maxNewLineChain are reduced to that length. Sequences
        /// shorter are ignored.
        /// </summary>
        public void CollapseNewLines(TwisonStory story, int maxNewLineChain)
        {
            var pattern = "";
            for (var i = 0; i < maxNewLineChain; i++)
            {
                pattern += "\n";
            }
            var regex = new Regex($"{pattern}\n*");
            foreach (var passage in story.passages)
            {
                passage.text = regex.Replace(passage.text, pattern);
            }
        }
    }
}