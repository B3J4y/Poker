using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Surface.Presentation.Controls;

namespace SurfacePoker
{
    class Chip
    {
        private ScatterViewItem chip;
        private double betControlLine;
        private double xChip;
        private double yChip;
        private int chipValue;
        private Uri imagePath;

        public Chip(ScatterViewItem chip, double betControlLine, double xChip, double yChip, int chipValue, Uri imagePath)
        {
            this.chip = chip;
            this.betControlLine = betControlLine;
            this.xChip = xChip;
            this.yChip = yChip;
            this.chipValue = chipValue;
            this.imagePath = imagePath;
        }

        public Uri ImagePath
        {
            get { return imagePath; }
            set { imagePath = value; }
        }

        public ScatterViewItem PlayerChip
        {
            get { return chip; }
            set { chip = value; }
        }

        public double ControlLine
        {
            get { return betControlLine; }
            set { betControlLine = value; }
        }

        public double X
        {
            get { return xChip; }
            set { xChip = value; }
        }

        public double Y
        {
            get { return yChip; }
            set { yChip = value; }
        }

        public int ChipValue
        {
            get { return chipValue; }
            set { chipValue = value; }
        }

    }
}
