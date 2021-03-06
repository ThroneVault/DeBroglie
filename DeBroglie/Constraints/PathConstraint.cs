﻿using System.Collections.Generic;
using System.Linq;

namespace DeBroglie.Constraints
{
    /// <summary>
    /// The PathConstraint checks that it is possible to connect several locations together via a continuous path of adjacent tiles. 
    /// It does this by banning any tile placement that would make such a path impossible.
    /// </summary>
    public class PathConstraint : ITileConstraint
    {
        /// <summary>
        /// Set of patterns that are considered on the path
        /// </summary>
        public ISet<Tile> PathTiles { get; set; }

        /// <summary>
        /// Set of points that must be connected by paths.
        /// If null, then PathConstraint ensures that all path cells
        /// are connected.
        /// </summary>
        public Point[] EndPoints { get; set; }

        public PathConstraint(ISet<Tile> pathTiles, Point[] endPoints = null)
        {
            this.PathTiles = pathTiles;
            this.EndPoints = endPoints;
        }

        public Resolution Init(TilePropagator propagator)
        {
            return Resolution.Undecided;
        }

        public Resolution Check(TilePropagator propagator)
        {
            var topology = propagator.Topology;
            var indices = topology.Width * topology.Height * topology.Depth;
            // Initialize couldBePath and mustBePath based on wave possibilities
            var couldBePath = new bool[indices];
            var mustBePath = new bool[indices];
            for (int i = 0; i < indices; i++)
            {
                topology.GetCoord(i, out var x, out var y, out var z);
                propagator.GetBannedSelected(x, y, z, PathTiles, out var isBanned, out var isSelected);
                couldBePath[i] = !isBanned;
                mustBePath[i] = isSelected;
            }

            // Select relevant cells, i.e. those that must be connected.
            bool[] relevant;
            if (EndPoints == null)
            {
                relevant = mustBePath;
            }
            else
            {
                relevant = new bool[indices];
                if (EndPoints.Length == 0)
                    return Resolution.Undecided;
                foreach (var endPoint in EndPoints)
                {
                    var index = topology.GetIndex(endPoint.X, endPoint.Y, endPoint.Z);
                    relevant[index] = true;
                }
            }
            var walkable = couldBePath;

            var isArticulation = PathConstraintUtils.GetArticulationPoints(topology, walkable, relevant);

            if (isArticulation == null)
            {
                return Resolution.Contradiction;
            }


            // All articulation points must be paths,
            // So ban any other possibilities
            for (var i = 0; i < indices; i++)
            {
                if (!isArticulation[i])
                {
                    continue;
                }
                foreach(var tile in propagator.TileModel.TilesToPatterns.Select(x=>x.Key))
                {
                    if (PathTiles.Contains(tile))
                        continue;
                    topology.GetCoord(i, out var x, out var y, out var z);
                    propagator.Ban(x, y, z, tile);
                }
            }

            return Resolution.Undecided;
        }
    }
}
