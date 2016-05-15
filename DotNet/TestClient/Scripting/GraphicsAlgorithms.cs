using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hexpoint.Blox;
using Hexpoint.Blox.Hosts.World;

namespace AiKnowledgeEngine
{
    class GraphicsAlgorithms
    {
        /// <summary>
        /// Bresenham's line algorithm
        /// </summary>
        public static IEnumerable<Position> DrawLineOn2dPlane (Position start, Position end)
        {
            int x0 = start.X;
            int y0 = start.Y;
            int z0 = start.Z;
            int x1 = end.X;
            int y1 = end.Y;
            //int z1 = end.Z;
            int dx = Math.Abs (x1 - x0);
            int dy = Math.Abs (y1 - y0);
            int sx = (x0 < x1) ? 1 : -1;
            int sy = (y0 < y1) ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                yield return new Position (x0, y0, z0);
                if (x0 == x1 && y0 == y1)// && z0 == z1)
                    break;
                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err = err - dy;
                    x0 = x0 + sx;
                }
                if (x0 == x1 && y0 == y1)// && z0 == z1)
                {
                    yield return new Position (x0, y0, z0);
                    break;
                }
                if (e2 < dx)
                {
                    err = err + dx;
                    y0 = y0 + sy;
                }
            } 
        }

        /// <summary>
        /// 3D version of Bresenham's line algorithm
        /// </summary>
        public static IEnumerable<Position> FindIntersectingBlocks (Position start, Position end)
        {               
            while (true)
            {
                int xxx = start.X;
                int yyy = start.Y;
                int zzz = start.Z;
                
                int dx = end.X - start.X;
                int dy = end.Y - start.Y;
                int dz = end.Z - start.Z;

                int err_1, err_2;

                int x_inc = Math.Sign(dx);
                int y_inc = Math.Sign(dy);
                int z_inc = Math.Sign(dz);

                int Adx = Math.Abs (dx);
                int Ady = Math.Abs (dy);
                int Adz = Math.Abs (dz);
                
                int dx2 = Adx * 2;
                int dy2 = Ady * 2;
                int dz2 = Adz * 2;
                
                if ((Adx >= Ady) && (Adx >= Adz))
                {
                    err_1 = dy2 - Adx;
                    err_2 = dz2 - Adx;
                
                    for (int Cont = 0; Cont < Adx-1; Cont++)
                    {
                        if (err_1 > 0)
                        {
                            yyy += y_inc;
                            err_1 -= dx2;
                        }
                            
                        if (err_2 > 0)
                        {
                            zzz += z_inc;
                            err_2 -= dx2;
                        } 
                            
                        err_1 += dy2;
                        err_2 += dz2;
                        xxx += x_inc;
                            
                        yield return new Position(xxx, yyy, zzz);
                    }
                    break;
                }
                
                if ((Ady > Adx) && (Ady >= Adz))
                {
                    err_1 = dx2 - Ady;
                    err_2 = dz2 - Ady;
                
                    for (int Cont = 0; Cont < Adx-1; Cont++)
                    {
                        if (err_1 > 0)
                        {
                            xxx += x_inc;
                            err_1 -= dy2;
                        }
                
                        if (err_2 > 0)
                        {
                            zzz += z_inc;
                            err_2 -= dy2;
                        }
                
                        err_1 += dx2;
                        err_2 += dz2;
                        yyy += y_inc;
                
                        yield return new Position(xxx, yyy, zzz);
                    }
                    break;
                }
                
                if ((Adz > Adx) && (Adz > Ady))
                {
                    err_1 = dy2 - Adz;
                    err_2 = dx2 - Adz;

                    for (int Cont = 0; Cont < Adx-1; Cont++)
                    {
                        if (err_1 > 0)
                        {
                            yyy += y_inc;
                            err_1 -= dz2;
                        }
                
                        if (err_2 > 0)
                        {
                            xxx += x_inc;
                            err_2 -= dz2;
                        }
                
                        err_1 += dy2;
                        err_2 += dx2;
                        zzz += z_inc;
                
                        yield return new Position(xxx, yyy, zzz);
                    }
                    break;
                }
                
                //Xold = xnew;
                //Yold = ynew;
                //Zold = znew;
            }
                
        }

        public static Position FastTan (int angle, int radius)
        {
            // TODO
            return new Position ((int)(radius * Math.Sin (Constants.PI_TIMES_2 * angle / 360)),
                                (int)(radius * Math.Cos (Constants.PI_TIMES_2 * angle / 360)), 0);
        }

        /// <summary>
        /// Bresenham's circle algorithm for a 2D integer grid
        /// </summary>
        public static IEnumerable<Position> DrawCircle (int gridSize)
        {
            int radius = gridSize / 2 - 1;
            int x0 = radius;
            int y0 = radius;
            int x = radius, y = 0;
            int radiusError = 1 - x;

            while (x >= y)
            {
                yield return new Position ((x + x0), (y + y0), 0);
                yield return new Position ((x + x0), (y + y0), 0);
                yield return new Position ((y + x0), (x + y0), 0);
                yield return new Position ((-x + x0), (y + y0), 0);
                yield return new Position ((-y + x0), (x + y0), 0);
                yield return new Position ((-x + x0), (-y + y0), 0);
                yield return new Position ((-y + x0), (-x + y0), 0);
                yield return new Position ((x + x0), (-y + y0), 0);
                yield return new Position ((y + x0), (-x + y0), 0);

                y++;
                if (radiusError < 0)
                    radiusError += 2 * y + 1;
                else
                {
                    x--;
                    radiusError += 2 * (y - x + 1);
                }
            }
        }

    }
}
