﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MazeKz.Cells
{
    public class Wall : CellBase
    {
        public Wall(int x, int y, Maze maze) : base(x, y, maze)
        {
        }

        public override bool TryStep()
        {
            return false;
        }
    }
}
