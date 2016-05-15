using System;
using System.Collections.Generic;

namespace Sean.World
{
    public class CharacterManager
    {
        public CharacterManager ()
        {
            _characters = new Dictionary<string, Character>();
        }

        private Dictionary<string, Character> _characters;
    }
}

