using System;
using System.Text;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using Hexpoint.Blox.Hosts.World;
using Hexpoint.Blox.GameObjects.Units;
using Hexpoint.Blox.GameActions;

namespace AiKnowledgeEngine
{
    public class PathFinder
    {
        #region Cell scores
        class LocationScore
        {
            public int fScore;
            public int gScore;
            public bool closedSet;
            public Position cameFrom;
        }

        private int GetFScore (Position loc)
        {
            if (!scores.ContainsKey (loc))
            {
                return 0;
            }
            return scores [loc].fScore;
        }

        private void SetFScore (Position loc, int score)
        {
            if (scores.ContainsKey (loc))
            {
                scores [loc].fScore = score;
            }
            else
            {
                scores.Add (loc, new LocationScore () { fScore = score });
            }
        }

        private int GetGScore (Position loc)
        {
            if (!scores.ContainsKey (loc))
            {
                return 0;
            }
            return scores [loc].gScore;
        }
        
        private void SetGScore (Position loc, int score)
        {
            if (scores.ContainsKey (loc))
            {
                scores [loc].gScore = score;
            }
            else
            {
                scores.Add (loc, new LocationScore () { gScore = score });
            }
        }

        private void SetCameFrom (Position loc, Position cameFrom)
        {
            if (scores.ContainsKey (loc))
            {
                scores [loc].cameFrom = cameFrom;
            }
            else
            {
                scores.Add (loc, new LocationScore () { cameFrom = cameFrom });
            }
        }

        private void AddToClosedSet (Position loc)
        {
            if (scores.ContainsKey (loc))
            {
                scores [loc].closedSet = true;
            }
            else
            {
                scores.Add (loc, new LocationScore () { closedSet = true });
            }
        }

        private bool IsClosedSet (Position loc)
        {
            if (!scores.ContainsKey (loc))
            {
                return false;
            }
            return scores [loc].closedSet;
        }

        private Position GetCameFrom (Position loc)
        {
            if (!scores.ContainsKey (loc))
            {
                throw new ApplicationException ("Don't know where came from");
            }
            return scores [loc].cameFrom;
        }

        private Position GetMinFScore (List<Position> locations)
        {
            int min = int.MaxValue;
            Position minPoint = new Position (0, 0, 0);
            foreach (Position i in locations)
            {
                //Console.WriteLine("{0} = {1}, {2}", i, GetGScore(i), GetFScore(i));
                int fscore = GetFScore (i);
                if (fscore < min)
                {
                    min = fscore;
                    minPoint = i;
                }
            }
            return minPoint;
        }
        #endregion

        private int ScrMaxX = 0;
        private int ScrMaxY = 0;
        private int ScrGridSizeX = 0;
        private int ScrGridSizeY = 0;
        private Map map;
        private Character character;
        private int fScore;
        private Dictionary<Position, LocationScore> scores;
        private int gScore;
        private bool openSet;
        private int characterCount;
        private List<Position> openset;

        public PathFinder (Map map)
        {
            this.map = map;
            fScore = 0;
            gScore = 0;
            scores = new Dictionary<Position, LocationScore> ();
            openset = new List<Position> ();
        }

        private int HeuristicCostEstimate (Position a, Position b)
        {
            Position diff = a - b;
            diff = diff * diff;
            double xz = Math.Sqrt(diff.X + diff.Z);
            double xzy = Math.Sqrt (xz*xz + diff.Y);
            return (int)(xzy * 12);  
        }

        private int DistBetween(Position a, Position b)
        {
            Position diff = a - b;
            diff = diff * diff;
            double xz = Math.Sqrt(diff.X + diff.Z);
            double xzy = Math.Sqrt (xz*xz + diff.Y);
            return (int)(xzy * 10);
        }

        public List<Position> FindPath (Position start, Position goal)
		{
			List<Position> route = new List<Position> ();
			int maxSearch = 100;
			int searched = 0;
			Position current = start;
			route.Add (goal);
			scores.Clear ();
			openset.Clear ();

			openset.Add (start);
			SetGScore (start, 0);    // Cost from start along best known path.            
			SetFScore (start, GetGScore (start) + HeuristicCostEstimate (start, goal));

			while (openset.Count != 0 && searched++ < maxSearch) {
				current = GetMinFScore (openset);
				openset.Remove (current);
				AddToClosedSet (current);
            
				if (CloseEnough(current, goal))
				{
					break;
				}

				List<Position> neighbours = NeighbourNodes (current);
				foreach (Position neighbour in neighbours) {
					if (IsClosedSet (neighbour))
						continue;
            
					int tentativeGScore = GetGScore (current) + DistBetween (current, neighbour);
            
					if (!openset.Contains (neighbour) || tentativeGScore < GetGScore (neighbour)) {
						SetCameFrom (neighbour, current);
						SetGScore (neighbour, tentativeGScore);
						SetFScore (neighbour, tentativeGScore + HeuristicCostEstimate (neighbour, goal));
                    
						if (!openset.Contains (neighbour)) {
							openset.Add (neighbour);
						}
                
					}
				}
			}

			if (searched >= maxSearch) 
			{
				//Console.WriteLine ("Could not find path from {0} to {1}", start, goal);
				return null;
			}
			else
            //if (GetFScore (goal) > 0) // Found path
            //if (DistBetween(current, goal) < WithinRangeScore)
            {
				Console.WriteLine ("Found route from {0} to {1} after checking {2} locations", start, goal, searched);
                //Position current = goal;
                while (current != start)
                {
                    route.Add (current);
                    current = GetCameFrom (current);
                }
            }
            return route;
        }

        public List<Position> FindPaths (Position start, Knowledge knowledge, Block.BlockType stopAtType = Block.BlockType.Air, int maxSearch = 100)
		{
			int searched = 0;
			Position current = start;
			scores.Clear ();
			openset.Clear ();

			openset.Add (start);
			SetGScore (start, 0);    // Cost from start along best known path.            
			SetFScore (start, GetGScore (start));

			while (openset.Count != 0 && searched++ < maxSearch) {
				current = GetMinFScore (openset);
				openset.Remove (current);
				AddToClosedSet (current);

				foreach (Position neighbour in NeighbourBlocks(current))
				{
                    if (neighbour.GetBlock().Type == stopAtType)
                    {
                        Console.WriteLine ("Found route from {0} to {1} after checking {2} locations", start, current, searched);
                        List<Position> route = new List<Position>();
                        while (current != start)
                        {
                            route.Add (current);
                            current = GetCameFrom (current);
                        }
                        return route;
                    }
					knowledge.Add(neighbour, start, GetGScore(current));
				}
				List<Position> neighbours = NeighbourNodes (current);
				foreach (Position neighbour in neighbours) {
					if (IsClosedSet (neighbour))
						continue;
            
					int tentativeGScore = GetGScore (current) + DistBetween (current, neighbour);
            
					if (!openset.Contains (neighbour) || tentativeGScore < GetGScore (neighbour)) {
						SetCameFrom (neighbour, current);
						SetGScore (neighbour, tentativeGScore);
						SetFScore (neighbour, tentativeGScore + HeuristicCostEstimate (start, neighbour));
                    
						if (!openset.Contains (neighbour)) {
							openset.Add (neighbour);
						}
                
					}
				}
			}
            return null;
        }

        public List<Position> FindPath (Position start, Position destination, Knowledge knowledge, int maxSearch = 100)
        {
            int searched = 0;
            Position current = start;
            scores.Clear ();
            openset.Clear ();
            
            openset.Add (start);
            SetGScore (start, 0);    // Cost from start along best known path.            
            SetFScore (start, GetGScore (start));
            
            while (openset.Count != 0 && searched++ < maxSearch) {
                current = GetMinFScore (openset);
                openset.Remove (current);
                AddToClosedSet (current);

                if (current == destination)
                {
                    Console.WriteLine ("Found route from {0} to {1} after checking {2} locations", start, current, searched);
                    List<Position> route = new List<Position>();
                    while (current != start)
                    {
                        route.Add (current);
                        current = GetCameFrom (current);
                    }
                    return route;
                }
                List<Position> neighbours = NeighbourNodes (current);
                foreach (Position neighbour in neighbours) {
                    if (IsClosedSet (neighbour))
                        continue;
                    
                    int tentativeGScore = GetGScore (current) + DistBetween (current, neighbour);
                    
                    if (!openset.Contains (neighbour) || tentativeGScore < GetGScore (neighbour)) {
                        SetCameFrom (neighbour, current);
                        SetGScore (neighbour, tentativeGScore);
                        SetFScore (neighbour, tentativeGScore + HeuristicCostEstimate (start, destination));
                        
                        if (!openset.Contains (neighbour)) {
                            openset.Add (neighbour);
                        }
                    }
                }
            }
            return null;
        }

		private bool CloseEnough (Position position, Position target)
		{
			Position diff = (position - target).Abs();
			return (diff.X <= 1 && diff.Z <= 1 && diff.Y<=3);
		}

        public IEnumerable<Position> FindNearestBlock(Position start, Block.BlockType target)
        {
            foreach (Position check in ListBlocksByRange(start))
            {
                if (check.GetBlock().Type == target)
                    yield return check;
			}
        }

        //IEnumerable<Position>
        public IEnumerable<Position> ListVisibleBlocks (Coords location, int depth)
		{
			//int depth = 100;
			int fieldOfView = depth / 2;
			Coords focus = location;
			focus.Xf += (float)(Math.Cos (location.Direction) * depth);
			focus.Zf += (float)(Math.Sin (location.Direction) * depth);

            Position tl = new Position(-fieldOfView, 0, fieldOfView);
            Position tr = new Position(fieldOfView, 0, fieldOfView);
            Position bl = new Position(-fieldOfView, 0, -fieldOfView);
            Position br = new Position(fieldOfView, 0, -fieldOfView);
            return ListVisibleBlocksRecurse (location, focus, tl, tr, bl, br);
        }

        public IEnumerable<Position> ListVisibleBlocksRecurse (Coords location, Coords focus, 
                                                               Position tl, Position tr, Position bl, Position br)
        {
			Coords tlc, trc, blc, brc;
            tlc = GetCoords (focus, tl.X, tl.Z, location.Direction);
            trc = GetCoords (focus, tr.X, tr.Z, location.Direction);
            blc = GetCoords (focus, bl.X, bl.Z, location.Direction);
            brc = GetCoords (focus, br.X, br.Z, location.Direction);

            //return ListVisibleBlocksRecursive(location, tl, tr, bl, br);
            Position tlb = FindIntersectingBlock(location, tlc);
            Position trb = FindIntersectingBlock(location, trc);
            Position blb = FindIntersectingBlock(location, blc);
            Position brb = FindIntersectingBlock(location, brc);
			/*
            if (tlb != trb || blb != brb || trb != brb || tlb != blb)
            {
                Position lc = new Position(tl.X, 0, (tr.Z + bl.Z) / 2);
                Position rc = new Position(tr.X, 0, (tr.Z + bl.Z) / 2);

                Position tc = new Position((tl.X + tr.X) / 2, 0, tr.Z);
                Position bc = new Position((tl.X + tr.X) / 2, 0, br.Z);

                Position cc = new Position((tl.X + tr.X) / 2, 0, (tr.Z + bl.Z) / 2);

                foreach (Position pos in ListVisibleBlocksRecurse (location, focus, tl, tc, rc, cc))
                    yield return pos;
                foreach (Position pos in ListVisibleBlocksRecurse (location, focus, tc, tr, cc, rc))
                    yield return pos;
                foreach (Position pos in ListVisibleBlocksRecurse (location, focus, lc, cc, bl, bc))
                    yield return pos;
                foreach (Position pos in ListVisibleBlocksRecurse (location, focus, cc, rc, bc, br))
                    yield return pos;
            }
            else*/ 
                yield return tlb;
            //yield return trb;
            //yield return blb;
            //yield return brb;
		}

        private Coords GetCoords(Coords focus, int x, int y, float direction)
		{	
		    Coords pt = focus;
            pt.Xf -= (float)(Math.Sin(direction) * x);
            pt.Zf += (float)(Math.Cos(direction) * x);
			pt.Yf += y;
			return pt;
		}

        private IEnumerable<Position> ListVisibleBlocksRecursive(Coords start, Coords tl, Coords tr, Coords bl, Coords br)
	    {
			Position tlb = FindIntersectingBlock(start, tl);
            Position trb = FindIntersectingBlock(start, tr);
            Position blb = FindIntersectingBlock(start, bl);
            Position brb = FindIntersectingBlock(start, br);

            yield return tlb;
            yield return trb;
            yield return blb;
            yield return brb;

			//float w = fieldOfView;
			//
            //for(int w = -fieldOfView; w < fieldOfView; w++)
            //{
            //
			//	Position block = FindIntersectingBlock(location, focus);
/*
                Coords pt2 = pt;
                pt2.Yf -= fieldOfView;
                for (int h = -fieldOfView; h < fieldOfView; h++)
                {

                    foreach (Position pt3 in GraphicsAlgorithms.FindIntersectingBlocks(location.ToPosition(), pt2.ToPosition()))
                    {
                        if (!pt3.IsValidBlockLocation)
                        {
                            break;
                        }
                        if (pt3.GetBlock().IsSolid)
                        {
                            NetworkClient.SendAddOrRemoveBlock(pt3, Block.BlockType.Crate);
                            break;
                        }
                    }
                    pt2.Yf += 1;
                   // NetworkClient.SendAddOrRemoveBlock(pt.ToPosition(), block.Type);
                }
*/
			//}
        }

		private Position FindIntersectingBlock(Coords start, Coords end)
		{
            foreach (Position pt in GraphicsAlgorithms.FindIntersectingBlocks(start.ToPosition(), end.ToPosition()))
            {
                if (!pt.IsValidBlockLocation)
                {
                    return end.ToPosition();
                }
                if (pt.GetBlock().IsSolid)
                {
					return pt;
                }
            }
            return end.ToPosition();
        }

        public IEnumerable<Position> ListBlocksByRange(Position start) // TODO - change to visible blocks
        {
            Position check;
            int x,y,z;
            int MaxSearchSize = 20;
            for(int i=1; i<MaxSearchSize; i++)
            {
                x = i;
                for(y = -i; y<=i; y++)
                    for(z = -i; z<=i; z++)
                {
                    check = start + new Position(x,y,z);
                    yield return check;
                }
                x = -i;
                for(y = -i; y<=i; y++)
                    for(z = -i; z<=i; z++)
                {
                    check = start + new Position(x,y,z);
                    yield return check;
                }
                
                y = i;
                for(x = -i; x<=i; x++)
                    for(z = -i; z<=i; z++)
                {
                    check = start + new Position(x,y,z);
                    yield return check;
                }
                y = -i;
                for(x = -i; x<=i; x++)
                    for(z = -i; z<=i; z++)
                {
                    check = start + new Position(x,y,z);
                    yield return check;
                }
                
                z = i;
                for(x = -i; x<=i; x++)
                    for(y = -i; y<=i; y++)
                {
                    check = start + new Position(x,y,z);
                    yield return check;
                }
                z = -i;
                for(x = -i; x<=i; x++)
                    for(y = -i; y<=i; y++)
                {
                    check = start + new Position(x,y,z);
                    yield return check;
                }
                
            }
        }

        public List<Position> FindPathToNearestBlock(Position start, Block.BlockType target)
        {
            foreach(Position pos in FindNearestBlock(start, target))
            {
                //Console.WriteLine ("Block at {0}", pos);
                List<Position> route = FindPath (start, pos);
                if (route != null)
                    return route;
            }
            return null;
        }

        private bool NextToTarget(Position loc, Block.BlockType target)
        {
            return
                ((loc + new Position (-1, 0, -1)).GetBlock().Type == target)
                    || ((loc + new Position (-1, 0, 0)).GetBlock().Type == target)
                    || ((loc + new Position (-1, 0, 1)).GetBlock().Type == target)
                    || ((loc + new Position (0, 0, 1)).GetBlock().Type == target)
                    || ((loc + new Position (1, 0, 1)).GetBlock().Type == target)
                    || ((loc + new Position (1, 0, 0)).GetBlock().Type == target)
                    || ((loc + new Position (1, 0, -1)).GetBlock().Type == target)
                    || ((loc + new Position (0, 0, -1)).GetBlock().Type == target);
        }

        /*
        public List<Position> FindPath (Position start, Position end)
        {
            List<Position> route = new List<Position> ();
            route.Add (start);
            if (start.GetBlock().IsSolid)
            {
                return route;
            }
            scores.Clear ();
            SetFScore (start, 0);
            openset.Clear ();
            openset.Add (start);
            Position current = start;

            while (openset.Count != 0)
            {
                current = GetMinFScore (openset);
                openset.Remove (current);
                AddToClosedSet (current);

                if (match (current))
                {
                    break;
                }

                List<Position> neighbours = NeighbourNodes (current, true, true);
                foreach (Position neighbour in neighbours)
                {
                    if (IsClosedSet (neighbour))
                        continue;

                    Position temp = neighbour;
                    int tentativeGScore = GetFScore (current) + current.GetDistanceRough (ref temp);
                    
                    if (!openset.Contains (neighbour) || tentativeGScore < GetGScore (neighbour))
                    {
                        SetFScore (neighbour, tentativeGScore);
                        SetCameFrom (neighbour, current);
                        
                        if (!openset.Contains (neighbour))
                        {
                            openset.Add (neighbour);
                        }
                        
                    }
                }
            }
            
            if (match (current)) // Found path
            {
                while (current != start)
                {
                    route.Add (current);
                    current = GetCameFrom (current);
                }
            }
            return route;
        }*/

        private Position FindMinNeighbour (Position current)
        {
            //Location cameFrom = GetCameFrom(current);
            //if (!map.GetLocation(cameFrom)->hasCharacter())
            //{
            //    return cameFrom; // Shortcut. Assume this is min
            //}
            openset = NeighbourNodes (current, false);
            return GetMinFScore (openset);
        }

		public IEnumerable<Position> NeighbourBlocks (Position pt)
		{
            foreach (Position pos in ListNeighbourBlocks (pt))
                yield return pos;
            foreach (Position pos in ListNeighbourBlocks (pt + new Position (-1, 0, 0)))
                yield return pos;
            foreach (Position pos in ListNeighbourBlocks (pt + new Position (0, 0, 1)))
                yield return pos;
            foreach (Position pos in ListNeighbourBlocks (pt + new Position (1, 0, 0)))
                yield return pos;
            foreach (Position pos in ListNeighbourBlocks (pt + new Position (0, 0, -1)))
                yield return pos;
		}

        private IEnumerable<Position> ListNeighbourBlocks (Position pos)
		{
			Position up1 = pos + new Position (0, 1, 0);
			Position up2 = pos + new Position (0, 2, 0);
			Position up3 = pos + new Position (0, 3, 0);
			Position dn1 = pos + new Position (0, -1, 0);
			Position dn2 = pos + new Position (0, -2, 0);
			Position dn3 = pos + new Position(0,-3,0);

			if (up3.GetBlock ().IsSolid)
				yield return up3;
			if (up2.GetBlock ().IsSolid)
				yield return up2;
			if (up1.GetBlock ().IsSolid)
				yield return up1;
			if (pos.GetBlock ().IsSolid)
			{
				yield return pos;
				yield break;
			} 

			if (dn1.GetBlock().IsSolid)
			{
				yield return dn1;
				yield break;
			}

		  	if (dn2.GetBlock().IsSolid)
			{
				yield return dn2;
				yield break;
			}

			if (dn3.GetBlock().IsSolid)
			{
				yield return dn3;
				yield break;
			}
	    }
 



        private List<Position> NeighbourNodes (Position pt, bool inclMovable = true, bool inclUnknown = false)
        {
            List<Position> neighbours = new List<Position> ();
            AddNeighbour (neighbours, pt, true, true); // always include current
            //AddNeighbour (neighbours, pt + new Position (-1, 0, -1), inclMovable, inclUnknown);
            AddNeighbour (neighbours, pt + new Position (-1, 0, 0), inclMovable, inclUnknown);
            //AddNeighbour (neighbours, pt + new Position (-1, 0, 1), inclMovable, inclUnknown);
            AddNeighbour (neighbours, pt + new Position (0, 0, 1), inclMovable, inclUnknown);
            //AddNeighbour (neighbours, pt + new Position (1, 0, 1), inclMovable, inclUnknown);
            AddNeighbour (neighbours, pt + new Position (1, 0, 0), inclMovable, inclUnknown);
            //AddNeighbour (neighbours, pt + new Position (1, 0, -1), inclMovable, inclUnknown);
            AddNeighbour (neighbours, pt + new Position (0, 0, -1), inclMovable, inclUnknown);
        
            return neighbours;
        }

        private void AddNeighbour (List<Position> neighbours, Position pos, bool inclMovable, bool inclUnknown)
        {
            Position up1 = pos + new Position(0,1,0);
            Position up2 = pos + new Position(0,2,0);
            //Position up3 = pos + new Position(0,3,0);
            Position dn1 = pos + new Position(0,-1,0);
            Position dn2 = pos + new Position(0,-2,0);
            //Position dn3 = pos + new Position(0,-3,0);

            if (dn1.GetBlock().IsSolid && pos.GetBlock().IsTransparent && up1.GetBlock().IsTransparent)
            {
                // Walk straight
                neighbours.Add(pos);
                return;
            }
            if (pos.GetBlock().IsSolid && up1.GetBlock().IsTransparent && up2.GetBlock().IsTransparent)
            {
                // Jump up one
                neighbours.Add(up1);
                return;               
            }
            if (dn2.GetBlock().IsSolid && dn1.GetBlock().IsTransparent && pos.GetBlock().IsTransparent)
            {
                // Drop down one
                neighbours.Add(dn1);
                return;
            }
            // TODO - drop down 2
            // TODO - Jump across gap
            // TODO - climb
        }
 

    }
}

