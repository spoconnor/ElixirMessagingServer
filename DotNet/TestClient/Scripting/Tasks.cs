using System;
using System.Collections.Generic;
using Hexpoint.Blox.Hosts.World;
using Hexpoint.Blox.GameObjects.Units;
using Hexpoint.Blox.GameActions;
using Hexpoint.Blox;
using Hexpoint.Blox.Hosts;
using OpenTK;
using Hexpoint.Blox.GameObjects.GameItems;

namespace AiKnowledgeEngine
{
    internal class Tasks
    {
        public Tasks ()
        {
            tasks = new List<BaseTaskItem> ();
        }

        internal void Add (BaseTaskItem item)
        {
            tasks.Add (item);
        }

        internal void DoTask (Character chr, FrameEventArgs e)
        {
            if (tasks.Count > 0)
            {
                tasks [tasks.Count - 1].DoTask (chr, e);
            }
        }
        
        public void Remove (BaseTaskItem item)
        {
            tasks.Remove (item);
        }
        
        private List<BaseTaskItem> tasks;
    }
    
    internal abstract class BaseTaskItem
    {
        internal abstract void DoTask (Character chr, FrameEventArgs e);
    }

    internal class GotoTask : BaseTaskItem
    {
        internal GotoTask(Position destination)
        {
            this.destination = destination;
        }
        
        internal override void DoTask (Character chr, FrameEventArgs e)
        {
            Console.WriteLine ("{0}:Finding path to {1}", chr.Id, destination);
            List<Position> route = null;

            route = chr.path.FindPath(chr.Position, destination, chr.knowledge, 10000);
            if (route == null)
            {
                Console.WriteLine ("{0}:Can't find route to target", chr.Id);
                // TODO - what?
                return;
            }
            chr.RemoveTask (this);
            chr.AddTask (new MoveToTask (route));
        }
        
        private Position destination;
    }

    internal class MoveToTask : BaseTaskItem
    {
        internal MoveToTask (List<Position> route)
        {
            this.route = route;
            index = route.Count - 1;
        }
        
        private List<Position> route;
        private int index;

        internal override void DoTask (Character chr, FrameEventArgs e)
        {
            Position moveTo = route [index];
            if (moveTo.GetBlock().IsSolid)
            {
                Console.WriteLine ("{0}:Route blocked", chr.Id);
                chr.RemoveTask (this);
            }
            else
            {
                //Console.WriteLine ("Moving to {0}", moveTo);
                chr.MoveTo (moveTo, e);

                if (chr.Position == moveTo)
                {
					//Console.WriteLine ("Moved to {0}", moveTo);
                    index--;
                }
                if (index == 0)
                {
                    Console.WriteLine ("{0}:At destination", chr.Id);
                    chr.RemoveTask (this);
				}
            }
        }
    }
    
//    internal class SearchTask : ITaskItem
//    {
//        internal void DoTask (Character chr)
//        {
//            Console.WriteLine ("Searching");
//            
//            List<Position> route = chr.path.FindPathToNearest (chr.Position, loc => loc.Type == Block.BlockType.);
//            if (route.Count <= 1)
//            {
//                Console.WriteLine ("Don't know where to go?");
//                return;
//            }
//            
    //            chr.AddTask (new MoveToTask (route));
//        }
//    }
    
	internal class WaitTask : BaseTaskItem
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AiKnowledgeEngine.WaitTask"/> class.
		/// </summary>
		/// <param name='waitTime'>
		/// Wait time in seconds.
		/// </param>
		internal WaitTask (double waitTime)
		{
            this.waitTime = waitTime * 1000;
		}

		internal override void DoTask (Character chr, FrameEventArgs e)
		{
			waitTime -= e.Time;
			if (waitTime < 0) {
                chr.RemoveTask (this);
			}
		}

		private double waitTime; // Waittime in ms
	}

    internal class LookAroundTask : BaseTaskItem
    {
        internal override void DoTask (Character chr, FrameEventArgs e)
        {
            Console.WriteLine ("{0}:Looking around", chr.Id);
			chr.path.FindPaths(chr.Position, chr.knowledge);
            chr.RemoveTask (this);
        }
    }

    internal class GatherItemTask : BaseTaskItem
    {
        internal GatherItemTask(Block.BlockType blockType)
        {
            this.blockType = blockType;
        }

        internal override void DoTask (Character chr, FrameEventArgs e)
        {
            Console.WriteLine ("{0}:Collect item", chr.Id);
            List<Position> route = null;

            route = chr.path.FindPaths(chr.Position, chr.knowledge, blockType, 10000);
            /*
            foreach (Position blockPos in chr.knowledge.GetNearestBlocks(blockType, chr.Position))
            {
                 route = chr.path.FindPath(chr.Position, blockPos);
                 if (route != null)
                 {
                    Console.WriteLine("Found route to nearby item");
                    break;
                 }
            }
            */

            if (route == null)
            {
                Console.WriteLine ("{0}:Can't see any target item, Gather than waiting 10 seconds", chr.Id);

                chr.AddTask (new WaitTask(10));
                chr.AddTask( new LookAroundTask());
                //chr.RemoveTask (this);
				//chr.AddTask (new SearchTask ());
				return;
			}
            chr.AddTask (new ChopBlocksAtPosTask(blockType, route[0]));
            chr.AddTask (new MoveToTask (route));
		}

        private Block.BlockType blockType;
    }

	internal class ChopBlocksAtPosTask : BaseTaskItem
    {
        internal ChopBlocksAtPosTask (Block.BlockType blockType, Position position)
        {
			this.blockType = blockType;
			this.position = position;
        }

		private Block.BlockType blockType;
		private Position position;

        internal override void DoTask (Character chr, FrameEventArgs e)
        {
            Console.WriteLine ("{0}:ChopBlocksAtPosTask {1}", chr.Id, position);
            foreach (Position pos in chr.path.NeighbourBlocks(position))
            {
                if (pos.GetBlock().Type == blockType)
			    {
                    chr.AddTask (new ChopBlockTask(blockType, pos));
                }
            }
            chr.RemoveTask (this);
        }
	}

    internal class ChopBlockTask : BaseTaskItem
    {
        internal ChopBlockTask (Block.BlockType blockType, Position position)
        {
            this.blockType = blockType;
            this.position = position;
        }
        
        private Block.BlockType blockType;
        private Position position;
        
        internal override void DoTask (Character chr, FrameEventArgs e)
        {
            if (position.GetBlock().Type == blockType)
            {
                Console.WriteLine ("{0}:Chopping {1}", chr.Id, blockType);
                AddOrRemoveBlock(chr, position, Block.BlockType.Air); // replace with air
            }
            chr.RemoveTask (this);
        }
        
        private void AddOrRemoveBlock(Character chr, Position position, Block.BlockType blockType)
        {
            if (!position.IsValidBlockLocation)
            {
                Console.WriteLine("{0}:Invalid block location", chr.Id);
                return;
            }
            
            NetworkClient.SendAddOrRemoveBlock(position, blockType);
        }
        
    }

    internal class BuildConstructionTask : BaseTaskItem
    {
        internal BuildConstructionTask(string blueprint)
        {
            this.blueprint = blueprint;
        }
        
        private string blueprint;
        private Dictionary<Position, Block.BlockType> blocks;
        internal override void DoTask (Character chr, FrameEventArgs e)
        {
            Console.WriteLine ("{0}:Building {1}", chr.Id, blueprint);
            blocks = Construction.Instance.GetConstruction(blueprint);
            if (blocks == null)
            {
                chr.AddTask (new WaitTask(10));
            }

            // TODO
            chr.RemoveTask (this);
        }
    }
}

