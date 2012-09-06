using System.Collections.Generic;
using System.Linq;
using Legend.Models;
using Raven.Client;

namespace Legend.Services
{
    public class WorldService : IWorldService
    {
        private readonly IDocumentStore _documentStore;
        public WorldService(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public Player GetPlayerByName(string name)
        {
            using(var session = _documentStore.OpenSession())
            {
                var player = session.Query<Player>().FirstOrDefault(u => u.Name == name);
                if (player == null)
                {
                    // Testing
                    player = new Player
                    {
                        Name = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name.ToLower()),
                        Description = "This is a test player description for " + name, 
                        RoomReference = new Room {Id = "Rooms/1"}
                    };
                    var spell = GetGameObjectById<Spell>("Spells/1");
                    player.Spells.Add(spell.Name, spell);

                    session.Store(player);
                    session.SaveChanges();
                }
                
                return (player);
            }
        }

        public T GetGameObjectById<T>(string id) where T : IGameObject
        {
            using (var session = _documentStore.OpenSession())
            {
                return (session.Load<T>(id));
            }
        }

        public IEnumerable<T> GetGameObjectsById<T>(IEnumerable<string> objIds) where T : IGameObject
        {
            using (var session = _documentStore.OpenSession())
            {
                return (session.Load<T>(objIds));
            }
        }

        public void Initialize()
        {
            using (var session = _documentStore.OpenSession())
            {
                if (!session.Query<Room>().Any()) // Create world, if it doesn't exist
                {
                    foreach(var gameObj in CreateWorld())
                        session.Store(gameObj);
                }
                else // Clear out any dead connections
                {
                    var roomsWithGhosts = session.Query<Room>().Where(x => x.PlayerReferences.Count > 0);
                    foreach (var roomWithGhosts in roomsWithGhosts)
                    {
                        roomWithGhosts.PlayerReferences.Clear();
                        session.Store(roomWithGhosts);
                    }

                    var hungPlayers = session.Query<Player>().Where(x => x.ClientIds.Count > 0);
                    foreach (var player in hungPlayers)
                    {
                        player.Online = false;
                        player.ClientIds.Clear();
                        session.Store(player);
                    }
                }
                
                session.SaveChanges();
            }
        }

        public void Save(IGameObject gameObj)
        {
            Save(new [] {gameObj});
        }

        public void Save(IEnumerable<IGameObject> gameObjs)
        {
            using (var session = _documentStore.OpenSession())
            {
                foreach (var gameObj in gameObjs)
                    session.Store(gameObj);

                session.SaveChanges();
            }
        }

        private IEnumerable<IGameObject> CreateWorld()
        {
            var world = new List<IGameObject>();

            // Spells
            var spell1 = new Spell
            {
                Id = "Spells/1",
                Name = "Fireball",
                Description = "A big ass fireball!",
                MinLevel = 1,
                MpRequired = 10,
                Reagents = new List<Reference<Item>>
                {
                    new Reference<Item>
                    {
                        Id = "Items/1"
                    },
                    new Reference<Item>
                    {
                        Id = "Items/2"
                    },
                }
            };

            // Items
            var item1 = new Item
            {
                Id = "Items/1",
                Name = "garnet",
                Description = "A common multi-faceted gem."
            };

            var item2 = new Item
            {
                Id = "Items/2",
                Name = "moonstone",
                Description = "A beautiful looking gem."
            };

            var item3 = new Item
            {
                Id = "Items/3",
                Name = "oaken wand",
                Description = "A wand made out of oak."
            };

            // Rooms
            var room1 = new Room
            {
                Id = "Rooms/1",
                Name = "Room 1",
                Description = "Room 1.  Really long description",
                Spawns = new Spawns
                {
                    ItemSpawns = new List<ItemSpawn>
                    {
                        new ItemSpawn
                        {
                            ItemRef = new Reference<Item>
                            {
                                Id = "Items/1"
                            },
                            Rate = 100
                        },
                        new ItemSpawn
                        {
                            ItemRef = new Reference<Item>
                            {
                                Id = "Items/1"
                            },
                            Rate = 101
                        },
                        new ItemSpawn
                        {
                            ItemRef = new Reference<Item>
                            {
                                Id = "Items/1"
                            },
                            Rate = 100
                        },
                    }
                }
            };

            var room2 = new Room
            {
                Id = "Rooms/2",
                Name = "Room 2",
                Description = "Room 2.  Really long description",
                Spawns = new Spawns
                {
                    ItemSpawns = new List<ItemSpawn>
                    {
                        new ItemSpawn
                        {
                            ItemRef = new Reference<Item>
                            {
                                Id = "Items/3"
                            },
                            Rate = 50
                        }
                    }
                }
            };

            var room3 = new Room
            {
                Id = "Rooms/3",
                Name = "Room 3",
                Description = "Room 3.  Really long description",
                Spawns = new Spawns
                {
                    ItemSpawns = new List<ItemSpawn>
                    {
                        new ItemSpawn
                        {
                            ItemRef = new Reference<Item>
                            {
                                Id = "Items/3"
                            },
                            Rate = 50
                        }
                    }
                }
            };

            var room4 = new Room
            {
                Id = "Rooms/4",
                Name = "Room 4",
                Description = "Room 4.  Really long description",
                Spawns = new Spawns
                {
                    ItemSpawns = new List<ItemSpawn>
                    {
                        new ItemSpawn
                        {
                            ItemRef = new Reference<Item>
                            {
                                Id = "Items/2"
                            },
                            Rate = 25
                        },
                        new ItemSpawn
                        {
                            ItemRef = new Reference<Item>
                            {
                                Id = "Items/2"
                            },
                            Rate = 25
                        },
                        new ItemSpawn
                        {
                            ItemRef = new Reference<Item>
                            {
                                Id = "Items/2"
                            },
                            Rate = 25
                        },
                        new ItemSpawn
                        {
                            ItemRef = new Reference<Item>
                            {
                                Id = "Items/2"
                            },
                            Rate = 25
                        },
                        new ItemSpawn
                        {
                            ItemRef = new Reference<Item>
                            {
                                Id = "Items/2"
                            },
                            Rate = 25
                        },
                        new ItemSpawn
                        {
                            ItemRef = new Reference<Item>
                            {
                                Id = "Items/2"
                            },
                            Rate = 25
                        },
                    }
                }
            };

            var room5 = new Room
            {
                Id = "Rooms/5",
                Name = "Room 5",
                Description = "Room 5.  Really long description"
            };

            // Add adjacent rooms
            room1.AdjacentRoomReferences.Add('n',room2);
            room1.AdjacentRoomReferences.Add('s', room3);
            room1.AdjacentRoomReferences.Add('e', room4);
            room1.AdjacentRoomReferences.Add('w', room5);

            room2.AdjacentRoomReferences.Add('s', room1);
            room3.AdjacentRoomReferences.Add('n', room1);
            room4.AdjacentRoomReferences.Add('w', room1);
            room5.AdjacentRoomReferences.Add('e', room1);

            world.AddRange(new IGameObject[] { room1, room2, room3, room4, room5, item1, item2, item3, spell1 });

            return (world);
        }
    }
}