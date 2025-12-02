using Terminal_Warrior.Engine;
using Terminal_Warrior.Engine.Core;

IFactory<IGame> Factory = new GameFactory();
IGame Game = Factory.Create();

Game.Run();