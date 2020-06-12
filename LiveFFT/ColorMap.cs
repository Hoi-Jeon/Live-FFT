using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace ColorMapNameSpace
{
    /*
     * ref: Creating Heat Maps with C# .NET
     * http://dylanvester.com/2015/10/creating-heat-maps-with-net-20-c-sharp/ 
     */
    public struct HeatPoint
    {
        public int X;
        public int Y;
        public byte Intensity;
        public HeatPoint(int iX, int iY, byte bIntensity)
        {
            X = iX;
            Y = iY;
            Intensity = bIntensity;
        }
    }

    class ColorMapClass
    {
        private static int colormapLength = 256;
        private static int alphavalue = 256;

        private static void DrawHeatPoint(Graphics Canvas, HeatPoint HeatPoint, int Radius)
        {
            // Create points generic list of points to hold circumference points
            List<Point> CircumferencePointsList = new List<Point>();

            // Create an empty point to predefine the point struct used in the circumference loop
            Point CircumferencePoint;

            // Create an empty array that will be populated with points from the generic list
            Point[] CircumferencePointsArray;

            // Calculate ratio to scale byte intensity range from 0-255 to 0-1
            float fRatio = 1F / Byte.MaxValue;

            // Precalulate half of byte max value
            byte bHalf = Byte.MaxValue / 2;

            // Flip intensity on it's center value from low-high to high-low
            int iIntensity = (byte)(HeatPoint.Intensity - ((HeatPoint.Intensity - bHalf) * 2));

            // Store scaled and flipped intensity value for use with gradient center location
            float fIntensity = iIntensity * fRatio;

            // Loop through all angles of a circle
            // Define loop variable as a double to prevent casting in each iteration
            // Iterate through loop on 10 degree deltas, this can change to improve performance
            for (double i = 0; i <= 360; i += 10)
            {
                // Replace last iteration point with new empty point struct
                CircumferencePoint = new Point();

                // Plot new point on the circumference of a circle of the defined radius
                // Using the point coordinates, radius, and angle
                // Calculate the position of this iterations point on the circle
                CircumferencePoint.X = Convert.ToInt32(HeatPoint.X + Radius * Math.Cos(ConvertDegreesToRadians(i)));
                CircumferencePoint.Y = Convert.ToInt32(HeatPoint.Y + Radius * Math.Sin(ConvertDegreesToRadians(i)));

                // Add newly plotted circumference point to generic point list
                CircumferencePointsList.Add(CircumferencePoint);
            }

            // Populate empty points system array from generic points array list
            // Do this to satisfy the datatype of the PathGradientBrush and FillPolygon methods
            CircumferencePointsArray = CircumferencePointsList.ToArray();

            // Create new PathGradientBrush to create a radial gradient using the circumference points
            PathGradientBrush GradientShaper = new PathGradientBrush(CircumferencePointsArray);

            // Create new color blend to tell the PathGradientBrush what colors to use and where to put them
            ColorBlend GradientSpecifications = new ColorBlend(3);

            // Define positions of gradient colors, use intesity to adjust the middle color to
            // show more mask or less mask
            GradientSpecifications.Positions = new float[3] { 0, fIntensity, 1 };

            // Define gradient colors and their alpha values, adjust alpha of gradient colors to match intensity
            GradientSpecifications.Colors = new Color[3]
            {
                Color.FromArgb(0, Color.White),
                Color.FromArgb(HeatPoint.Intensity, Color.Black),
                Color.FromArgb(HeatPoint.Intensity, Color.Black)
            };

            // Pass off color blend to PathGradientBrush to instruct it how to generate the gradient
            GradientShaper.InterpolationColors = GradientSpecifications;

            // Draw polygon (circle) using our point array and gradient brush
            Canvas.FillPolygon(GradientShaper, CircumferencePointsArray);
        }

        private static double ConvertDegreesToRadians(double degrees)
        {
            double radians = (Math.PI / 180) * degrees;
            return (radians);
        }

        public static Bitmap CreateIntensityMask(Bitmap bSurface, List<HeatPoint> aHeatPoints)
        {
            // Create new graphics surface from memory bitmap
            Graphics DrawSurface = Graphics.FromImage(bSurface);

            // Set background color to white so that pixels can be correctly colorized
            DrawSurface.Clear(Color.White);

            // Traverse heat point data and draw masks for each heat point
            foreach (HeatPoint DataPoint in aHeatPoints)
            {
                // Render current heat point on draw surface
                DrawHeatPoint(DrawSurface, DataPoint, 15);
            }

            return bSurface;
        }

        public static Bitmap Colorize(Bitmap Mask, byte Alpha, int[,] TypeColorMap)
        {            
            // Create new bitmap to act as a work surface for the colorization process
            Bitmap Output = new Bitmap(Mask.Width, Mask.Height, PixelFormat.Format32bppArgb);

            // Create a graphics object from our memory bitmap so we can draw on it and clear it's drawing surface
            Graphics Surface = Graphics.FromImage(Output);
            Surface.Clear(Color.Transparent);

            // Build an array of color mappings to remap our greyscale mask to full color
            // Accept an alpha byte to specify the transparancy of the output image
            ColorMap[] Colors = CreatePaletteIndex(Alpha, TypeColorMap);

            // Create new image attributes class to handle the color remappings
            // Inject our color map array to instruct the image attributes class how to do the colorization
            ImageAttributes Remapper = new ImageAttributes();
            Remapper.SetRemapTable(Colors);

            // Draw our mask onto our memory bitmap work surface using the new color mapping scheme
            Surface.DrawImage(Mask, new Rectangle(0, 0, Mask.Width, Mask.Height), 0, 0, Mask.Width, Mask.Height, GraphicsUnit.Pixel, Remapper);

            // Send back newly colorized memory bitmap
            return Output;
        }

        private static ColorMap[] CreatePaletteIndex(byte Alpha, int[,] TypeColorMap)
        {
            ColorMap[] OutputMap = new ColorMap[256];

            // Loop through each pixel and create a new color mapping
            for (int X = 0; X <= 255; X++)
            {
                OutputMap[X] = new ColorMap();
                OutputMap[X].OldColor = Color.FromArgb(X, X, X);
                OutputMap[X].NewColor = Color.FromArgb(Alpha, Color.FromArgb(TypeColorMap[X, 1], TypeColorMap[X, 2], TypeColorMap[X, 3]));
            }
            return OutputMap;
        }

        public static int[,] JetTable()
        {
            // Read the color map matrix of "Jet" style
            int[,] JetMap= {{256,   131 ,   0   ,   0   },
                            {256,   135 ,   0   ,   0   },
                            {256,   139 ,   0   ,   0   },
                            {256,   143 ,   0   ,   0   },
                            {256,   147 ,   0   ,   0   },
                            {256,   151 ,   0   ,   0   },
                            {256,   155 ,   0   ,   0   },
                            {256,   159 ,   0   ,   0   },
                            {256,   163 ,   0   ,   0   },
                            {256,   167 ,   0   ,   0   },
                            {256,   171 ,   0   ,   0   },
                            {256,   175 ,   0   ,   0   },
                            {256,   179 ,   0   ,   0   },
                            {256,   183 ,   0   ,   0   },
                            {256,   187 ,   0   ,   0   },
                            {256,   191 ,   0   ,   0   },
                            {256,   195 ,   0   ,   0   },
                            {256,   199 ,   0   ,   0   },
                            {256,   203 ,   0   ,   0   },
                            {256,   207 ,   0   ,   0   },
                            {256,   211 ,   0   ,   0   },
                            {256,   215 ,   0   ,   0   },
                            {256,   219 ,   0   ,   0   },
                            {256,   223 ,   0   ,   0   },
                            {256,   227 ,   0   ,   0   },
                            {256,   231 ,   0   ,   0   },
                            {256,   235 ,   0   ,   0   },
                            {256,   239 ,   0   ,   0   },
                            {256,   243 ,   0   ,   0   },
                            {256,   247 ,   0   ,   0   },
                            {256,   251 ,   0   ,   0   },
                            {256,   255 ,   0   ,   0   },
                            {256,   255 ,   3   ,   0   },
                            {256,   255 ,   7   ,   0   },
                            {256,   255 ,   11  ,   0   },
                            {256,   255 ,   15  ,   0   },
                            {256,   255 ,   19  ,   0   },
                            {256,   255 ,   23  ,   0   },
                            {256,   255 ,   27  ,   0   },
                            {256,   255 ,   31  ,   0   },
                            {256,   255 ,   35  ,   0   },
                            {256,   255 ,   39  ,   0   },
                            {256,   255 ,   43  ,   0   },
                            {256,   255 ,   47  ,   0   },
                            {256,   255 ,   51  ,   0   },
                            {256,   255 ,   55  ,   0   },
                            {256,   255 ,   59  ,   0   },
                            {256,   255 ,   63  ,   0   },
                            {256,   255 ,   67  ,   0   },
                            {256,   255 ,   71  ,   0   },
                            {256,   255 ,   75  ,   0   },
                            {256,   255 ,   79  ,   0   },
                            {256,   255 ,   83  ,   0   },
                            {256,   255 ,   87  ,   0   },
                            {256,   255 ,   91  ,   0   },
                            {256,   255 ,   95  ,   0   },
                            {256,   255 ,   99  ,   0   },
                            {256,   255 ,   103 ,   0   },
                            {256,   255 ,   107 ,   0   },
                            {256,   255 ,   111 ,   0   },
                            {256,   255 ,   115 ,   0   },
                            {256,   255 ,   119 ,   0   },
                            {256,   255 ,   123 ,   0   },
                            {256,   255 ,   127 ,   0   },
                            {256,   255 ,   131 ,   0   },
                            {256,   255 ,   135 ,   0   },
                            {256,   255 ,   139 ,   0   },
                            {256,   255 ,   143 ,   0   },
                            {256,   255 ,   147 ,   0   },
                            {256,   255 ,   151 ,   0   },
                            {256,   255 ,   155 ,   0   },
                            {256,   255 ,   159 ,   0   },
                            {256,   255 ,   163 ,   0   },
                            {256,   255 ,   167 ,   0   },
                            {256,   255 ,   171 ,   0   },
                            {256,   255 ,   175 ,   0   },
                            {256,   255 ,   179 ,   0   },
                            {256,   255 ,   183 ,   0   },
                            {256,   255 ,   187 ,   0   },
                            {256,   255 ,   191 ,   0   },
                            {256,   255 ,   195 ,   0   },
                            {256,   255 ,   199 ,   0   },
                            {256,   255 ,   203 ,   0   },
                            {256,   255 ,   207 ,   0   },
                            {256,   255 ,   211 ,   0   },
                            {256,   255 ,   215 ,   0   },
                            {256,   255 ,   219 ,   0   },
                            {256,   255 ,   223 ,   0   },
                            {256,   255 ,   227 ,   0   },
                            {256,   255 ,   231 ,   0   },
                            {256,   255 ,   235 ,   0   },
                            {256,   255 ,   239 ,   0   },
                            {256,   255 ,   243 ,   0   },
                            {256,   255 ,   247 ,   0   },
                            {256,   255 ,   251 ,   0   },
                            {256,   255 ,   255 ,   0   },
                            {256,   251 ,   255 ,   3   },
                            {256,   247 ,   255 ,   7   },
                            {256,   243 ,   255 ,   11  },
                            {256,   239 ,   255 ,   15  },
                            {256,   235 ,   255 ,   19  },
                            {256,   231 ,   255 ,   23  },
                            {256,   227 ,   255 ,   27  },
                            {256,   223 ,   255 ,   31  },
                            {256,   219 ,   255 ,   35  },
                            {256,   215 ,   255 ,   39  },
                            {256,   211 ,   255 ,   43  },
                            {256,   207 ,   255 ,   47  },
                            {256,   203 ,   255 ,   51  },
                            {256,   199 ,   255 ,   55  },
                            {256,   195 ,   255 ,   59  },
                            {256,   191 ,   255 ,   63  },
                            {256,   187 ,   255 ,   67  },
                            {256,   183 ,   255 ,   71  },
                            {256,   179 ,   255 ,   75  },
                            {256,   175 ,   255 ,   79  },
                            {256,   171 ,   255 ,   83  },
                            {256,   167 ,   255 ,   87  },
                            {256,   163 ,   255 ,   91  },
                            {256,   159 ,   255 ,   95  },
                            {256,   155 ,   255 ,   99  },
                            {256,   151 ,   255 ,   103 },
                            {256,   147 ,   255 ,   107 },
                            {256,   143 ,   255 ,   111 },
                            {256,   139 ,   255 ,   115 },
                            {256,   135 ,   255 ,   119 },
                            {256,   131 ,   255 ,   123 },
                            {256,   127 ,   255 ,   127 },
                            {256,   123 ,   255 ,   131 },
                            {256,   119 ,   255 ,   135 },
                            {256,   115 ,   255 ,   139 },
                            {256,   111 ,   255 ,   143 },
                            {256,   107 ,   255 ,   147 },
                            {256,   103 ,   255 ,   151 },
                            {256,   99  ,   255 ,   155 },
                            {256,   95  ,   255 ,   159 },
                            {256,   91  ,   255 ,   163 },
                            {256,   87  ,   255 ,   167 },
                            {256,   83  ,   255 ,   171 },
                            {256,   79  ,   255 ,   175 },
                            {256,   75  ,   255 ,   179 },
                            {256,   71  ,   255 ,   183 },
                            {256,   67  ,   255 ,   187 },
                            {256,   63  ,   255 ,   191 },
                            {256,   59  ,   255 ,   195 },
                            {256,   55  ,   255 ,   199 },
                            {256,   51  ,   255 ,   203 },
                            {256,   47  ,   255 ,   207 },
                            {256,   43  ,   255 ,   211 },
                            {256,   39  ,   255 ,   215 },
                            {256,   35  ,   255 ,   219 },
                            {256,   31  ,   255 ,   223 },
                            {256,   27  ,   255 ,   227 },
                            {256,   23  ,   255 ,   231 },
                            {256,   19  ,   255 ,   235 },
                            {256,   15  ,   255 ,   239 },
                            {256,   11  ,   255 ,   243 },
                            {256,   7   ,   255 ,   247 },
                            {256,   3   ,   255 ,   251 },
                            {256,   0   ,   255 ,   255 },
                            {256,   0   ,   251 ,   255 },
                            {256,   0   ,   247 ,   255 },
                            {256,   0   ,   243 ,   255 },
                            {256,   0   ,   239 ,   255 },
                            {256,   0   ,   235 ,   255 },
                            {256,   0   ,   231 ,   255 },
                            {256,   0   ,   227 ,   255 },
                            {256,   0   ,   223 ,   255 },
                            {256,   0   ,   219 ,   255 },
                            {256,   0   ,   215 ,   255 },
                            {256,   0   ,   211 ,   255 },
                            {256,   0   ,   207 ,   255 },
                            {256,   0   ,   203 ,   255 },
                            {256,   0   ,   199 ,   255 },
                            {256,   0   ,   195 ,   255 },
                            {256,   0   ,   191 ,   255 },
                            {256,   0   ,   187 ,   255 },
                            {256,   0   ,   183 ,   255 },
                            {256,   0   ,   179 ,   255 },
                            {256,   0   ,   175 ,   255 },
                            {256,   0   ,   171 ,   255 },
                            {256,   0   ,   167 ,   255 },
                            {256,   0   ,   163 ,   255 },
                            {256,   0   ,   159 ,   255 },
                            {256,   0   ,   155 ,   255 },
                            {256,   0   ,   151 ,   255 },
                            {256,   0   ,   147 ,   255 },
                            {256,   0   ,   143 ,   255 },
                            {256,   0   ,   139 ,   255 },
                            {256,   0   ,   135 ,   255 },
                            {256,   0   ,   131 ,   255 },
                            {256,   0   ,   127 ,   255 },
                            {256,   0   ,   123 ,   255 },
                            {256,   0   ,   119 ,   255 },
                            {256,   0   ,   115 ,   255 },
                            {256,   0   ,   111 ,   255 },
                            {256,   0   ,   107 ,   255 },
                            {256,   0   ,   103 ,   255 },
                            {256,   0   ,   99  ,   255 },
                            {256,   0   ,   95  ,   255 },
                            {256,   0   ,   91  ,   255 },
                            {256,   0   ,   87  ,   255 },
                            {256,   0   ,   83  ,   255 },
                            {256,   0   ,   79  ,   255 },
                            {256,   0   ,   75  ,   255 },
                            {256,   0   ,   71  ,   255 },
                            {256,   0   ,   67  ,   255 },
                            {256,   0   ,   63  ,   255 },
                            {256,   0   ,   59  ,   255 },
                            {256,   0   ,   55  ,   255 },
                            {256,   0   ,   51  ,   255 },
                            {256,   0   ,   47  ,   255 },
                            {256,   0   ,   43  ,   255 },
                            {256,   0   ,   39  ,   255 },
                            {256,   0   ,   35  ,   255 },
                            {256,   0   ,   31  ,   255 },
                            {256,   0   ,   27  ,   255 },
                            {256,   0   ,   23  ,   255 },
                            {256,   0   ,   19  ,   255 },
                            {256,   0   ,   15  ,   255 },
                            {256,   0   ,   11  ,   255 },
                            {256,   0   ,   7   ,   255 },
                            {256,   0   ,   3   ,   255 },
                            {256,   0   ,   0   ,   255 },
                            {256,   0   ,   0   ,   251 },
                            {256,   0   ,   0   ,   247 },
                            {256,   0   ,   0   ,   243 },
                            {256,   0   ,   0   ,   239 },
                            {256,   0   ,   0   ,   235 },
                            {256,   0   ,   0   ,   231 },
                            {256,   0   ,   0   ,   227 },
                            {256,   0   ,   0   ,   223 },
                            {256,   0   ,   0   ,   219 },
                            {256,   0   ,   0   ,   215 },
                            {256,   0   ,   0   ,   211 },
                            {256,   0   ,   0   ,   207 },
                            {256,   0   ,   0   ,   203 },
                            {256,   0   ,   0   ,   199 },
                            {256,   0   ,   0   ,   195 },
                            {256,   0   ,   0   ,   191 },
                            {256,   0   ,   0   ,   187 },
                            {256,   0   ,   0   ,   183 },
                            {256,   0   ,   0   ,   179 },
                            {256,   0   ,   0   ,   175 },
                            {256,   0   ,   0   ,   171 },
                            {256,   0   ,   0   ,   167 },
                            {256,   0   ,   0   ,   163 },
                            {256,   0   ,   0   ,   159 },
                            {256,   0   ,   0   ,   155 },
                            {256,   0   ,   0   ,   151 },
                            {256,   0   ,   0   ,   147 },
                            {256,   0   ,   0   ,   143 },
                            {256,   0   ,   0   ,   139 },
                            {256,   0   ,   0   ,   135 },
                            {256,   0   ,   0   ,   131 },
                            {256,   0   ,   0   ,   127 }};
            return JetMap;
        }

    }
}
