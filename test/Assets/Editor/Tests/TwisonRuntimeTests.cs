using System.Linq;
using DefaultNamespace;
using NUnit.Framework;
using Twison;

namespace Editor.Tests
{
    public class TwisonRuntimeTests
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
                    ""text"": ""The north forest is just to the north of this village.\nIt's full of monsters, go at your own peril..."",
                    ""name"": ""Dialog.VillagerGeneric1.NorthForest"",
                    ""pid"": ""5"",
                    ""position"": {
                        ""x"": ""308.5"",
                        ""y"": ""306.5""
                    },
                    ""tags"": [
                        ""Requires:Knowledge:NorthForest""
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
        public void TestCreateRuntime()
        {
            var story = new TwisonLoader().Load(ExampleStory);
            var tracker = new TwisonTagFilterBehaviour();
            var runtime = new TwisonRuntime(story, tracker, true);
            
            Assert.AreEqual(runtime.FindByName("Dialog.VillagerGeneric1.Hello"), runtime.Active);
        }
        
        [Test]
        public void TestFilteredLinksBehaviour()
        {
            var story = new TwisonLoader().Load(ExampleStory);
            var tracker = new TwisonTagFilterBehaviour();
            var runtime = new TwisonRuntime(story, tracker, true);

            // Before seeing restricted tag, cannot see link
            Assert.False(runtime.Links.Any(i => i.name == "North forest"));

            // Act
            Assert.True(runtime.GoToPassage("Rumors"));
            Assert.True(runtime.GoToPassage("Goblins"));
            Assert.True(runtime.GoToPreviousPassage());
            Assert.True(runtime.GoToPreviousPassage());
            Assert.AreEqual(runtime.Active.name, "Dialog.VillagerGeneric1.Hello");
            
            // Assert: After seeing tag, can see link
            Assert.True(runtime.Links.Any(i => i.name == "North forest"));
        }
    }
}