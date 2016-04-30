using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Sean.World
{
    class WorldHost
    {
   #region Constructors
        public WorldHost ()
        {
            //PerformanceHost.OnHalfSecondElapsed += PerformanceHost_OnHalfSecondElapsed;
            //PerformanceHost.OnHalfSecondElapsed += UpdateCharacters_OnHalfSecondElapsed;

            //for (var i = 0; i < Math.Max(1, Environment.ProcessorCount / 2); i++)
            //{
            //    var buildChunkThread = new Thread (BuildChunksThread) { IsBackground = true, Priority = ThreadPriority.Lowest, Name = "Chunk Builder " + i}; //Lowest priority makes it noticeably less choppy when working through the queue (startup)
            //    buildChunkThread.Start ();
            //}

            //FogColorUnderWater = new ColorRgb (51, 128, 204);
        }

        private static void PerformanceHost_OnHalfSecondElapsed ()
        {
            WaterCycleTextureId++;
            if (WaterCycleTextureId > (int)Textures.BlockTextureType.Water4)
                WaterCycleTextureId = (int)Textures.BlockTextureType.Water;
        }
   #endregion

   #region Properties
        //internal static int RotationCounter;
        /// <summary>Current water texture id for the water animation cycle. Incremented in the performance host.</summary>
        internal static int WaterCycleTextureId = (int)Textures.BlockTextureType.Water;
   #endregion

   #region Build World
        internal void BuildWorld ()
        {
            const int INITIAL_CHUNK_RENDER_DISTANCE = 5;
            var stopwatch = new Stopwatch ();
            stopwatch.Start ();

            //var chunksByDist = new List<Chunk> ();

/*
            //immediately render up to INITIAL_CHUNK_RENDER_DISTANCE chunks away within the initial view frustum
            var immediateChunkTasks = new List<Task> ();
            foreach (Chunk chunk in WorldData.WorldMap.Chunks)
            {

                var dist = chunk.DistanceFromPlayer ();
                if (dist <= INITIAL_CHUNK_RENDER_DISTANCE * Chunk.CHUNK_SIZE && chunk.IsInFrustum)
                {
                    //chunk is close enough to the player and in the initial view frustum so it needs to be renderable before the game starts
                    int taskX = chunk.Coords.X, taskZ = chunk.Coords.Z;
                    immediateChunkTasks.Add (Task.Factory.StartNew (() => {
                        WorldData.Chunks [taskX, taskZ].ChunkBuildState = Chunk.BuildState.QueuedInitialFrustum;
                        WorldData.Chunks [taskX, taskZ].BuildData (); }));
                }
                else
                {
                    //chunk is outside distance to render initially; now check if it needs to be rendered after initial load
                    if (dist < Settings.ZFarForChunkLoad)
                        chunksByDist.Add (chunk);
                }

            }
            Task.WaitAll (immediateChunkTasks.ToArray ()); //wait for initial chunks to finish building before we continue
            stopwatch.Stop ();
            Debug.WriteLine ("Initial chunk frustum load time: {0}ms ({1} chunks)", stopwatch.ElapsedMilliseconds, immediateChunkTasks.Count);
*/     
            //now sort by distance and queue so nearby chunks are built first
            //chunksByDist.Sort ((c1, c2) => Math.Sqrt (Math.Pow (Game.Player.Coords.Xblock - c1.Coords.WorldCoordsX, 2) + Math.Pow (Game.Player.Coords.Zblock - c1.Coords.WorldCoordsZ, 2)).CompareTo (Math.Sqrt (Math.Pow (Game.Player.Coords.Xblock - c2.Coords.WorldCoordsX, 2) + Math.Pow (Game.Player.Coords.Zblock - c2.Coords.WorldCoordsZ, 2))));
            //chunksByDist.ForEach (c => {
            //    c.ChunkBuildState = Chunk.BuildState.QueuedInitialFar; });

            //reset the LastUpdate on all items so they don't go flying
            foreach (var gameItem in WorldData.GameItems.Values)
            {
                gameItem.LastUpdate = DateTime.Now;
            }
        }
   #endregion

   #region Render
        private static void UpdateCharacters_OnHalfSecondElapsed (FrameEventArgs e)
        {
            //foreach (var character in Game.Characters)
            //{
            //    //Game.scriptDriver.Execute(character);
            //    character.UpdateKnownMap ();
            //    character.DoTasks (e);
            //}
        }

   #endregion

        public void Dispose ()
        {
        }

        public bool Enabled { get; set; }
    }
}