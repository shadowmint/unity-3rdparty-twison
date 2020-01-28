using System.Linq;
using NUnit.Framework;
using Twison;

namespace Editor.Tests
{
    public class TwisonLoaderTests
    {
        private static string ExampleStory = @"
        {
            ""passages"": [
                {
                    ""text"": ""Greetings stranger~\n\nWhat brings you here?\n\n[[Shops->Dialog.VillagerGeneric1.AskAboutShops]]\n[[Rumors->Dialog.VillagerGeneric1.Rumors]]\n[[North forest->Dialog.VillagerGeneric1.NorthForest]]\n\n..."",
                    ""links"": [
                        {
                            ""name"": ""Shops"",
                            ""link"": ""Dialog.VillagerGeneric1.AskAboutShops"",
                            ""pid"": ""2""
                        },
                        {
                            ""name"": ""Rumors"",
                            ""link"": ""Dialog.VillagerGeneric1.Rumors"",
                            ""pid"": ""3""
                        },
                        {
                            ""name"": ""North forest"",
                            ""link"": ""Dialog.VillagerGeneric1.NorthForest"",
                            ""pid"": ""5""
                        }
                    ],
                    ""name"": ""Dialog.VillagerGeneric1.Hello"",
                    ""pid"": ""1"",
                    ""position"": {
                        ""x"": ""38.5"",
                        ""y"": ""46.5""
                    },
                    ""tags"": [
                        ""Dialog"",
                        ""EntryPoint"",
                        ""Identity:VillagerGeneric1"",
                        ""Group:Villager"",
                        ""Location:StarterVillage""
                    ]
                },
                {
                    ""text"": ""There are a few shops around town, the smithy sells weapons and items,\nand the healer sells potions. A bit expensive if you ask me though..."",
                    ""name"": ""Dialog.VillagerGeneric1.AskAboutShops"",
                    ""pid"": ""2"",
                    ""position"": {
                        ""x"": ""313.5"",
                        ""y"": ""43""
                    }
                },
                {
                    ""text"": ""Some stuff\n\n[[Goblins->Dialog.VillagerGeneric1.Rumors.Goblins]]\n"",
                    ""links"": [
                        {
                            ""name"": ""Goblins"",
                            ""link"": ""Dialog.VillagerGeneric1.Rumors.Goblins"",
                            ""pid"": ""4""
                        }
                    ],
                    ""name"": ""Dialog.VillagerGeneric1.Rumors"",
                    ""pid"": ""3"",
                    ""position"": {
                        ""x"": ""313.5"",
                        ""y"": ""164.5""
                    }
                },
                {
                    ""text"": ""We've had some globins aorund these parts, up to the **north forest**.\n\n[[Other rumors...->Dialog.VillagerGeneric1.Rumors]]"",
                    ""links"": [
                        {
                            ""name"": ""Other rumors..."",
                            ""link"": ""Dialog.VillagerGeneric1.Rumors"",
                            ""pid"": ""3""
                        }
                    ],
                    ""name"": ""Dialog.VillagerGeneric1.Rumors.Goblins"",
                    ""pid"": ""4"",
                    ""position"": {
                        ""x"": ""582.5"",
                        ""y"": ""164.5""
                    },
                    ""tags"": [
                        ""Knowledge:NorthForest""
                    ]
                },
                {
                    ""text"": ""The norht forest is just to the north of this village.\nIt's full of monsters, go at your own peril..."",
                    ""name"": ""Dialog.VillagerGeneric1.NorthForest"",
                    ""pid"": ""5"",
                    ""position"": {
                        ""x"": ""308.5"",
                        ""y"": ""306.5""
                    },
                    ""tags"": [
                        ""Requires:Knowledge:NorthForst""
                    ]
                }
            ],
            ""name"": ""RPG-1"",
            ""startnode"": ""1"",
            ""creator"": ""Twine"",
            ""creator-version"": ""2.3.5"",
            ""ifid"": ""7C7FA55A-5552-49CF-B7EF-9AB8AEC9D357""
        }
        ";

        [Test]
        public void TestLoadTwisonData()
        {
            var story = new TwisonLoader().Load(ExampleStory);
            Assert.AreEqual(story.passages.Count, 5);
            Assert.NotNull(story.startNodePassage);
            foreach (var link in story.passages.SelectMany(passage => passage.links))
            {
                Assert.NotNull(link.passage);
            }
        }

        [Test]
        public void TestLoadTwisonDataAndPruneAndFilterPassages()
        {
            var story = new TwisonLoader(new TwisonLoaderOptions()
            {
                trimPassages = true,
                removeLinkTextFromPassages = true
            }).Load(ExampleStory);

            var cleanedPassage = story.passages.First(i => i.name == "Dialog.VillagerGeneric1.Hello");
            Assert.AreEqual(cleanedPassage.text, "Greetings stranger~\n\nWhat brings you here?\n\n\n\n\n\n...");
        }

        [Test]
        public void TestLoadTwisonDataCollapseNewLinesMultiNewlLine()
        {
            var story = new TwisonLoader(new TwisonLoaderOptions()
            {
                trimPassages = true,
                removeLinkTextFromPassages = true,
                collapseNewLines = true,
                maxNewLineChain = 2
            }).Load(ExampleStory);

            var cleanedPassage = story.passages.First(i => i.name == "Dialog.VillagerGeneric1.Hello");
            Assert.AreEqual(cleanedPassage.text, "Greetings stranger~\n\nWhat brings you here?\n\n...");
        }

        [Test]
        public void TestLoadTwisonDataCollapseNewLinesOneNewLine()
        {
            var story = new TwisonLoader(new TwisonLoaderOptions()
            {
                trimPassages = true,
                removeLinkTextFromPassages = true,
                collapseNewLines = true,
                maxNewLineChain = 1
            }).Load(ExampleStory);

            var cleanedPassage = story.passages.First(i => i.name == "Dialog.VillagerGeneric1.Hello");
            Assert.AreEqual(cleanedPassage.text, "Greetings stranger~\nWhat brings you here?\n...");
        }

        [Test]
        public void TestLoadTwisonDataWithOptionsIsValid()
        {
            var allOptions = new TwisonLoaderOptions()
            {
                trimPassages = true,
                removeLinkTextFromPassages = true,
                collapseNewLines = true,
                maxNewLineChain = 1
            };
            var story = new TwisonLoader(allOptions).Load(ExampleStory);
            Assert.AreEqual(story.passages.Count, 5);
            Assert.NotNull(story.startNodePassage);
            foreach (var passage in story.passages)
            {
                Assert.False(string.IsNullOrWhiteSpace(passage.name));
                Assert.False(string.IsNullOrWhiteSpace(passage.text));
                foreach (var link in passage.links)
                {
                    Assert.False(string.IsNullOrWhiteSpace(link.name));
                    Assert.False(string.IsNullOrWhiteSpace(link.link));
                    Assert.NotNull(link.passage);
                }
            }
        }
    }
}