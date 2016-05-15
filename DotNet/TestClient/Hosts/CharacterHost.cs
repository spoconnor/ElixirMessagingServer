using System;
using System.Diagnostics;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using Hexpoint.Blox.GameObjects.Units;
using Hexpoint.Blox.Hosts.World;
using AiKnowledgeEngine;
using Hexpoint.Blox.Hosts.Ui;
using Hexpoint.Blox.Textures;

namespace Hexpoint.Blox.Hosts
{
    internal class CharacterHost : IHost
    {

        private class CharacterButton : Button
        {
            public const int BUTTON_SIZE = 50;
            public CharacterButton(Character character, int x, int y)
                : base(ButtonType.Character, x, y, TextureLoader.GetUiTexture(UiTextureType.BaseCharacter))
            {
                this.character = character;
            }

            public Character character;
        }


        public List<Character> Characters = new List<Character> ();
        private List<CharacterButton> SelectedCharacters = new List<CharacterButton> ();

        #region Constructors
        internal CharacterHost()
        {
            var characterCoords = new Coords ((WorldData.SizeInBlocksX / 2f) - 5.5f, 0, (WorldData.SizeInBlocksZ / 2f) - 5.5f); //start player in center of world
            characterCoords.Yf = WorldData.GetHeightMapLevel(characterCoords.Xblock, characterCoords.Zblock) + 1; //start on block above the surface
            characterCoords.Direction = 0.5f;
            Character chr1 = new Character (0, "Char1", characterCoords);
            Characters.Add (chr1);
            
            characterCoords = new Coords ((WorldData.SizeInBlocksX / 2f) - 5.5f, 0, (WorldData.SizeInBlocksZ / 2f) - 4.5f); //start player in center of world
            characterCoords.Yf = WorldData.GetHeightMapLevel(characterCoords.Xblock, characterCoords.Zblock) + 1; //start on block above the surface
            characterCoords.Direction = 0.5f;
            Character chr2 = new Character (1, "Char2", characterCoords);
            Characters.Add (chr2);
            
            //characterCoords = new Coords ((WorldData.SizeInBlocksX / 2f) - 5, 0, (WorldData.SizeInBlocksZ / 2f) - 5); //start player in center of world
            //characterCoords.Yf = WorldData.Chunks [characterCoords].HeightMap [characterCoords.Xblock % Chunk.CHUNK_SIZE, characterCoords.Zblock % Chunk.CHUNK_SIZE] + 1; //start on block above the surface
            //Characters.Add (new Character (0, "Char2", characterCoords));
            
            //characterCoords = new Coords ((WorldData.SizeInBlocksX / 2f) - 5, 0, (WorldData.SizeInBlocksZ / 2f) - 5); //start player in center of world
            //characterCoords.Yf = WorldData.Chunks [characterCoords].HeightMap [characterCoords.Xblock % Chunk.CHUNK_SIZE, characterCoords.Zblock % Chunk.CHUNK_SIZE] + 1; //start on block above the surface
            //Characters.Add (new Character (0, "Char3", characterCoords));
            
            //MapManager.Instance.AddFood (15, 10);
            //chr1.AddTask (new GatherItemTask (Block.BlockType.Tree));
            //chr2.AddTask (new BuildConstructionTask ("House"));
            
            //scriptDriver.Init ();

            PerformanceHost.OnHalfSecondElapsed += UpdateCharacters_OnHalfSecondElapsed;
        }
        #endregion

        public void SelectCharacters(Position start, Position end)
        {
            SelectedCharacters.Clear();
            foreach (var character in Characters)
            {
                if (character.Position.WithinBounds(start, end))
                {
                    Console.WriteLine ("Selected {0}", character.Id);
                    SelectedCharacters.Add(new CharacterButton(character, 5, 5 + (CharacterButton.BUTTON_SIZE + 5) * SelectedCharacters.Count));
                }
            }
        }

        public void PerformCharacterAction(Position pos)
        {
            foreach (var selectedChr in SelectedCharacters)
            {
                selectedChr.character.AddTask (new GotoTask(pos));
            }
        }

        public void Render(FrameEventArgs e)
        {
            foreach (var character in Characters)
            {
                character.Render (e);
            }
            GameObjects.GameObject.ResetColor ();            
        }

        public void CharacterInfoRender()
        {
            foreach (var selectedChr in SelectedCharacters)
            {
                selectedChr.Render();
            }            
        }
        
        private void UpdateCharacters_OnHalfSecondElapsed ()
        {
            FrameEventArgs e = new FrameEventArgs(500);
            // TODO Slow poll tasks
            //foreach (var character in Characters)
            //{
                //Game.scriptDriver.Execute(character);
                //character.UpdateKnownMap ();
                //character.DoTasks (e);
            //}
        }

        public void Update(FrameEventArgs e)
        {         
            foreach (var character in Characters)
            {
                //Game.scriptDriver.Execute(character);
                //character.UpdateKnownMap ();
                character.DoTasks (e);
            }
        }

        public void Resize(EventArgs e)
        {
            
        }
        
        public void Dispose()
        {
            
        }

        public bool Enabled { get; set; }

    }
}
