using System;
using Hexpoint.Blox.Hosts.World;

namespace Hexpoint.Blox.Scripting
{
  public interface ICharacter
  {
    IScriptHost Parent { set; }
    string Execute(int characterId);

    string Name { get; }
    Coords Coords { get; }
  }
}

