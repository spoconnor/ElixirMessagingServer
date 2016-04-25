using System;
using Sean.World;

namespace Sean.World
{
    /// <summary>
    /// Use Settings class for values that can be altered but not saved.
    /// Use Config class for values that can be altered and saved.
    /// Use Constants class for values that cannot be altered.
    /// </summary>
    internal static class Settings
    {
        internal static Random Random = new Random();

        private static Version _version;
        /// <summary>Store the version here so the game window can still know what version we are running.</summary>
        internal static Version Version
        {
            get { return _version; }
            set
            {
                _version = value;
                VersionDisplay = string.Format("{0}.{1}.{2}", value.Major, value.Minor, value.Build);
            }
        }
        
        /// <summary>Version in the format: major.minor.build</summary>
        internal static string VersionDisplay { get; private set; }
        
        /// <summary>UI can be toggled for screenshots, videos, etc. by pressing alt-Z</summary>
        internal static bool UiDisabled { get; set; }
        /// <summary>UI debug info can be toggled by pressing F3</summary>
        internal static bool UiDebugDisabled { get; set; }
        
        private static bool _chunkUpdatesDisabled;
        /// <summary>For tracking performance issues. Useful test to disable once all chunks have been initially loaded and buffered.</summary>
        internal static bool ChunkUpdatesDisabled
        { 
            get { return _chunkUpdatesDisabled; }
            set
            {
                _chunkUpdatesDisabled = value;
//                if (!_chunkUpdatesDisabled) WorldHost.BuildChunkHandle.Set();
            }
        }
        
        /// <summary>Directly corresponds to how many blocks away you can see because we use a block size of 1.</summary>
        private static float _zFar;
        internal static float ZFar
        {
            get { return _zFar; }
            set
            {
                _zFar = value;
                ZFarForChunkLoad = Math.Min(ZFar * 1.2f, ZFar + Chunk.CHUNK_SIZE * 3);
                ZFarForChunkUnload = Math.Min(ZFar * 1.3f, ZFar + Chunk.CHUNK_SIZE * 5);
            }
        }
        
        /// <summary>Distance at which chunks will be queued for VBO build. Ideally we want to build before they enter ZFar.</summary>
        internal static float ZFarForChunkLoad { get; private set; }
        
        /// <summary>Distance at which chunks will have their VBOs dropped. Leave a decent buffer from ZFarForChunkLoad so minor movements don't cause constant load/unload.</summary>
        internal static float ZFarForChunkUnload { get; private set; }
        
        /// <summary>Use for debugging. When true all chunk edges will highlight blocks on either side. The actual chunk edge line is the line between the 2 yellow block strips.</summary>
        //internal static bool OutlineChunks;

        private static string _worldFilePath;
        /// <summary>File path for the world. Full directory and file extension are added in the setter.</summary>
        public static string WorldFilePath
        {
            get { return _worldFilePath; }
            set
            {
                _worldFilePath = String.Format("{0}{1}{2}{3}", SaveDirectoryFullName, System.IO.Path.DirectorySeparatorChar, value, Constants.WORLD_FILE_EXTENSION); //use System.IO.Path.DirectorySeparatorChar to play nice with linux
                WorldFileTempPath = String.Format("{0}.temp", _worldFilePath);
            }
        }
        private const string SaveDirectoryFullName = ".";
        public static string WorldFileTempPath { get; private set; }
    }
}
