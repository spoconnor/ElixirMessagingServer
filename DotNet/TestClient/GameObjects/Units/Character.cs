using System;
using Hexpoint.Blox.Hosts.World;
using Hexpoint.Blox.Utilities;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Hexpoint.Blox.Scripting;
using AiKnowledgeEngine;
using Hexpoint.Blox.GameActions;
using Hexpoint.Blox.GameObjects.GameItems;

namespace Hexpoint.Blox.GameObjects.Units
{
    internal class Character : Unit
    {
        internal Character (int id, string userName, Coords coords) : base(ref coords)
        {
            Id = id;
            UserName = userName;
            KnownMap = new Map ();
            path = new PathFinder (KnownMap);
            knowledge = new Knowledge ();
            tasks = new Tasks ();
        }

        /// <summary>Player Id. Hides the derived GameObject.Id because players use their own Id sequencing.</summary>
        internal new int Id { get; private set; }

        internal string UserName { get; private set; }

        internal readonly int[] Inventory = new int[256]; //for now just the 256 block types

        /// <summary>Coords of the players head. One block higher then regular coords of the player.</summary>
        internal Coords CoordsHead {
            get { return new Coords (Coords.Xblock, Coords.Yblock + 1, Coords.Zblock); }
        }

        /// <summary>Coords of the players eyes. PLAYER_EYE_LEVEL higher then regular coords of the player.</summary>
        internal Coords CoordsEyes {
            get { return new Coords (Coords.Xf, Coords.Yf + Constants.PLAYER_EYE_LEVEL, Coords.Zf); }
        }

        internal float FallVelocity;
        private  bool _isStandingOnSolidGround;
        /// <summary>Is the player standing on solid ground. Some actions, such as starting a jump, require the player to be on solid ground.</summary>
        public bool IsStandingOnSolidGround
        {
            get { return _isStandingOnSolidGround; }
            set
            {
                _isStandingOnSolidGround = value;
                if (_isStandingOnSolidGround) 
                    FallVelocity = 0;
            }
        }

        private bool _isFloating;
        /// <summary>Is the player floating while swimming.</summary>
        internal bool IsFloating
        {
            get { return _isFloating; }
            set
            {
                _isFloating = value;
                FallVelocity = 0;
                IsJumping = false;
            }
        }

        private float _jumpVelocity; //remaining velocity on the current jump
        private bool _isJumping;
        /// <summary>Is the player jumping. Only true if the player is still in the upwards trajectory of the jump, otherwise the player is falling.</summary>
        internal bool IsJumping
        {
            get { return _isJumping; }
            set
            {
                _isJumping = value;
                _jumpVelocity = _isJumping ? Settings.JumpSpeed / 2 : 0;
            }
        }

        internal bool EyesUnderWater { get; private set; }
        /// <summary>Check if players eyes are under water. Only checked from the InputHost for the local player so this gets calculated only once per update loop.</summary>
        internal void CheckEyesUnderWater ()
        {
            var coordsEyes = CoordsEyes; //make copy so we can pass by ref to GetBlock
            EyesUnderWater = coordsEyes.Yf < Chunk.CHUNK_HEIGHT && WorldData.GetBlock (ref coordsEyes).Type == Block.BlockType.Water;
        }

        internal bool FeetUnderWater { get; private set; }
        /// <summary>Check if players feet are under water. Only checked from the InputHost for the local player so this gets calculated only once per update loop.</summary>
        internal void CheckFeetUnderWater ()
        {
            FeetUnderWater = Coords.Yf < Chunk.CHUNK_HEIGHT && WorldData.GetBlock (ref Coords).Type == Block.BlockType.Water;
        }

        internal override void Render (FrameEventArgs e)
        {
            base.Render (e); //sets light color, the color assigned will be the block the lower body is on

            GlHelper.ResetTexture ();

            //note that these rotations might seem counterintuitive - use the right-hand-rule -bm
            //render lower body
            GL.PushMatrix ();
            GL.Translate (Coords.Xf, Coords.Yf, Coords.Zf);
            GL.Rotate (MathHelper.RadiansToDegrees (Coords.Direction), -Vector3.UnitY);
            GL.CallList (DisplayList.TorsoId);

            //render upper body
            GL.Translate (Vector3.UnitY * Constants.PLAYER_EYE_LEVEL); //moves to eye level and render head
            GL.Rotate (Math.Max (MathHelper.RadiansToDegrees (Coords.Pitch), -40), Vector3.UnitZ); //pitch head up and down, doesnt need to be turned because the body already turned. cap at -40degrees or it looks weird
            GL.CallList (DisplayList.HeadId);
            GL.PopMatrix ();
        }

        /// <summary>
        /// Render nameplates last during the transparent rendering stage. Prevents ever seeing missing blocks when looking through transparent portion of player name.
        /// Nameplate is only rendered if the player is within the allowable nameplate viewing distance.
        /// </summary>
        /// <remarks>
        /// By moving this to render independently from the player, the names look much better, and even though we now loop through the players twice, we no longer
        /// need to enable/disable blending and lighting for each player like we did when we rendered the name with the player. So ultimately even though this seems less
        /// efficient, we actually use 4 less GL calls per player as well.
        /// </remarks>
        internal void RenderNameplate ()
        {
            const int CHAR_WIDTH = 55;
            var distance = Game.Player.Coords.GetDistanceExact (ref Coords);
            if (distance > Constants.MAXIMUM_DISTANCE_TO_VIEW_NAMEPLATES)
                return; //if a player is outside the max distance then skip rendering the name
            float scale = Math.Max (0.003f, distance * 0.0002f); //keep the nameplate a minimum size, otherwise scale it relative to the distance to make names more readable at long distances

            //todo: at far distances the nameplate now overlaps onto players head, this is a side effect of making the char display lists draw from top down instead of bottom up
            GL.PushMatrix ();
            GL.Translate (Coords.Xf, Coords.Yf + Constants.PLAYER_EYE_LEVEL + 0.8f, Coords.Zf); //nameplate goes above players head
            GL.Scale (scale, scale, scale);
            GL.Rotate (180, Vector3.UnitZ);
            GL.Rotate (MathHelper.RadiansToDegrees (Game.Player.Coords.Direction) - 90, Vector3.UnitY);

            GL.Translate (-(UserName.Length / 2f * CHAR_WIDTH), 0, 0); //centers the name
            foreach (char t in UserName)
            {
                GL.BindTexture (TextureTarget.Texture2D, Textures.TextureLoader.GetLargeCharacterTexture (t));
                GL.CallList (DisplayList.LargeCharId);
                GL.Translate (CHAR_WIDTH, 0, 0); //this is doing one useless translate at the end
            }
            GL.PopMatrix ();
        }

        internal override string XmlElementName {
         //players arent saved in the xml file yet, in the future we might want to, to store where the player is when they log off etc.
            get { throw new NotImplementedException (); }
        }

        internal bool MoveCharacter (bool moveForward, bool moveBack, bool strafeLeft, bool strafeRight, FrameEventArgs e)
        {
            double distance = (moveBack ? -Settings.MoveSpeed : Settings.MoveSpeed) * e.Time;
            if (Math.Abs(distance) > 1) distance = Math.Sign(distance);
            if (EyesUnderWater)
            {
                distance *= 0.5; //50% move speed when under water
            }
            else if (IsFloating)
            {
                //if the player isnt in creative mode AND is floating AND is not under water; it probably means they walked out of a waterfall and floating should be cancelled
                IsFloating = false;
            }
            double direction = Coords.Direction;
            if (strafeLeft ^ strafeRight) //xor, if strafing both ways then ignore as they would cancel each other out
            {
                if (moveForward)
                {
                    direction += strafeLeft ? -MathHelper.PiOver4 : MathHelper.PiOver4; //move forward diagonally while strafing
                }
                else if (moveBack)
                {
                    direction += strafeLeft ? MathHelper.PiOver4 : -MathHelper.PiOver4; //move back diagonally while strafing
                }
                else //strafing only
                {
                    direction += strafeLeft ? -MathHelper.PiOver2 : MathHelper.PiOver2;
                }
            }
            
            double collisionTestDistance = distance + Math.Sign(distance) * Constants.MOVE_COLLISION_BUFFER; //account for same distance whether player is going forward or backward
            bool moved = false;
            
            //move along the X plane
            var destCoords = Coords;
            destCoords.Xf += (float)(Math.Cos(direction) * collisionTestDistance);
            if (!destCoords.IsValidPlayerLocation)
            {
                //IsJumping = false; // cancel jump due to collision
            }
            else
            {
                Coords.Xf += (float)(Math.Cos(direction) * distance);
                moved = true;
            }
            
            //move along the Z plane
            destCoords = Coords;
            destCoords.Zf += (float)(Math.Sin(direction) * collisionTestDistance);
            if (!destCoords.IsValidPlayerLocation)
            {
                //IsJumping = false; // cancel jump due to collision
            }
            else
            {
                Coords.Zf += (float)(Math.Sin(direction) * distance);
                moved = true;
            }

            float jumpDist = 0.0f;
            if (IsJumping)
            {
				// Jumping
                _jumpVelocity -= (float)e.Time;
                jumpDist = _jumpVelocity;
                //if (Math.Abs(jumpDist) > 1) jumpDist = Math.Sign(jumpDist);
				destCoords = Coords;
				collisionTestDistance = jumpDist + Math.Sign(jumpDist) * Constants.MOVE_COLLISION_BUFFER;
				destCoords.Yf += (float)collisionTestDistance;
				if (!destCoords.IsValidBlockLocation)
				{
					IsJumping = false; // cancel jump due to collision
				}
				else
				{
                	Coords.Yf += jumpDist;
				}

                if (_jumpVelocity <= 0)
                {
                    IsJumping = false; //at the top of the jump arc
                }
            }
            else
            {
				// Falling
                destCoords.Yf -= (FallVelocity + Constants.MOVE_COLLISION_BUFFER); //account for fall speed and the collision buffer
                if (destCoords.Yf >= Chunk.CHUNK_HEIGHT || !WorldData.GetBlock(ref destCoords).IsSolid) //player wont hit a solid block so let them keep falling
                {
                    IsStandingOnSolidGround = false; //gm: needed in the case the player walks off a cliff without jumping and then tries to jump mid air
                    Coords.Yf -= FallVelocity;

                    if (FallVelocity < Constants.MAX_FALL_VELOCITY)
                    {
                        FallVelocity = Math.Min(FallVelocity + ((float)e.Time / 2), Constants.MAX_FALL_VELOCITY);
                    }

                    //NetworkClient.SendPlayerLocation(Game.Player.Coords);
                }
                else //player is either standing on a solid block or will now land on one
                {
                    if (Coords.Yf - Constants.MOVE_COLLISION_BUFFER > Coords.Yblock) // player is landing on a solid block
                    {
                        Coords.Yf = Coords.Yblock + Constants.PLAYER_GROUNDED_VERTICAL_BUFFER;
                        if (!FeetUnderWater) Sounds.Audio.PlaySound(Sounds.SoundType.PlayerLanding, FallVelocity * 1.5f); //plays sound louder based on the velocity at time of landing
                        //NetworkClient.SendPlayerLocation(Coords, true); //forcefully send to server so other players are always seen "grounded"
                    }
                    IsStandingOnSolidGround = true; //gm: cant set inside the above block only because if trying to jump while in a 2 block high tunnel it could get toggled off, this must be set after the sound so the fallVelocity hasnt been reset yet
                }
            }

			var chunk = WorldData.Chunks [Coords];
			foreach (var gameItem in chunk.GameItems.Values) 
			{
				if (gameItem.Type == GameItemType.BlockItem) 
				{
					if (Coords.GetDistanceExact (ref gameItem.Coords) <= 2) 
					{
						new PickupBlockItem (Game.Player.Id, gameItem.Id).Send (); // TODO - pick up, for this character
					} 
				}
			}
            // TODO - send character movement to other networked players
            //if (moved)
            //    NetworkClient.SendPlayerLocation (Coords);

            return moved;
        }
        
        internal void RotateDirection (float radians, FrameEventArgs e)
        {
            if (radians > Math.PI)
            {
                if (radians > 0.0f)
                    radians = radians - Constants.PI_TIMES_2;
                else
                    radians = radians + Constants.PI_TIMES_2;
            }
//            float elapsedTime = 0.5f; // 1/2 second
//            float maxTurnSpeed = 0.2f * elapsedTime; // 0.2 radians per second
//            //float turnSpeed = (radians > 0f) ? Math.Min (radians, maxTurnSpeed) : Math.Max (radians, 0f-maxTurnSpeed);
//            if (radians > maxTurnSpeed)
//                Coords.Direction += maxTurnSpeed;
//            else if (radians < -maxTurnSpeed)
//                Coords.Direction -= maxTurnSpeed;
//            else
                Coords.Direction += radians;
            // TODO - send character movement to other networked players
            //NetworkClient.SendPlayerLocation (Game.Player.Coords);
        }
        
        internal void RotatePitch (float radians)
        {
            Coords.Pitch += radians;
            // TODO - send character movement to other networked players
            //NetworkClient.SendPlayerLocation (Game.Player.Coords);
        }
        
        /// <summary>
        /// Checks if either the players feet or eyes are entering or exiting water. Plays sound and changes Fog where applicable.
        /// Contained here so it only gets calculated once per update.
        /// </summary>
        internal void CheckCharacterEnteringOrExitingWater ()
        {
            var eyesUnderWaterPreviously = EyesUnderWater;
            var feetUnderWaterPreviously = FeetUnderWater;
            CheckEyesUnderWater (); //only calc this once per update
            CheckFeetUnderWater (); //only calc this once per update
            if (eyesUnderWaterPreviously != EyesUnderWater) // eyes entered or exited water on this update
            {
                if (EyesUnderWater) // eyes entered water
                {
                }
                else //players eyes exited water
                {
                }
            }
            if (feetUnderWaterPreviously != FeetUnderWater) // feet entered or exited water on this update
            {
                //play splash sound if feet entered water during a fall, otherwise play jump out of water sound
                if (FeetUnderWater && FallVelocity * 2 > 0.05)
                    Sounds.Audio.PlaySoundIfNotAlreadyPlaying (Sounds.SoundType.Splash, Game.Player.FallVelocity * 2);
                else
                    Sounds.Audio.PlaySoundIfNotAlreadyPlaying (Sounds.SoundType.JumpOutOfWater);
            }
        }



        public void DoTasks (FrameEventArgs e)
        {
            tasks.DoTask (this, e);
        }

        public void MoveTo (Position destination, FrameEventArgs e)
        {
            Coords coordDest = destination.ToCoords();
            coordDest.Xf += 0.5f; // target centre of block
            coordDest.Zf += 0.5f; // target centre of block
            float turnRadians = (float)(Coords.Direction) - (float)Math.Atan2((float)(coordDest.Zf - Coords.Zf), (float)(coordDest.Xf - Coords.Xf));
			while (turnRadians >= Constants.PI_TIMES_2 - 0.1) 
				turnRadians -= Constants.PI_TIMES_2;
			while (turnRadians <= -Constants.PI_TIMES_2 + 0.1) 
				turnRadians += Constants.PI_TIMES_2;

            if (Math.Abs(turnRadians) > 0.1)
            {
                RotateDirection(-turnRadians, e);
            }
            else
            {
                if (Position.Y < destination.Y)
                {
                    IsJumping = true;
                }
                bool moved = MoveCharacter(true, false, false, false, e);
            }
        }
        
        public void UpdateKnownMap ()
        {
            //foreach (Location location in MapManager.Instance.GetQuadrantCells(Location, Heading, 90, 5)) 
            //{
            //    MapLocation mapLoc = MapManager.Instance.GetLocation(location);
            //  KnownMap.UpdateLocation(location, mapLoc);
            //}
            foreach (Position position in MapManager.Instance.GetSurrounding(Coords.ToPosition(), 5))
            {
                Block block = position.GetBlock ();
                KnownMap.UpdateLocation (position, block);
            }
        }

        public void AddTask(BaseTaskItem task)
        {
            tasks.Add(task);
        }
        public void RemoveTask(BaseTaskItem task)
        {
            tasks.Remove(task);
        }

        public IScriptHost Parent { get; set; }

        public Position Position { get { return Coords.ToPosition (); } }

        public Map KnownMap;
        public PathFinder path;
        public Knowledge knowledge;
        private Tasks tasks;
    }
}
