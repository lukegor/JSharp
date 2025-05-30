﻿using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using Emgu.CV;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;

namespace JSharp.ViewModels
{
    public class PlotlineGraphWindowViewModel : ObservableObject
    {
        private ISeries[] _values;
        public ISeries[] Values
        {
            get { return _values; }
            set { SetProperty(ref _values, value); }
        }

        public PlotlineGraphWindowViewModel(Point[] points, Mat image)
        {
            Values = PlotGraph(points, image);
        }

        private ISeries[] PlotGraph(Point[] points, Mat image)
        {
            var seriesList = new List<ISeries>();

            var linePoints = BresenhamLine(image, points[0], points[1]);
            var lineSeries = new LineSeries<ObservablePoint>
            {
                Values = new ObservableCollection<ObservablePoint>(linePoints),
                YToolTipLabelFormatter = chartPoint => $"{chartPoint.Coordinate}"
            };
            seriesList.Add(lineSeries);

            return seriesList.ToArray();
        }

        private List<ObservablePoint> BresenhamLine(Mat image, System.Windows.Point p1, System.Windows.Point p2)
        {
            List<ObservablePoint> linePoints = new List<ObservablePoint>();

            int x1 = (int)p1.X;
            int y1 = (int)p1.Y;
            int x2 = (int)p2.X;
            int y2 = (int)p2.Y;

            int dx = Math.Abs(x2 - x1);
            int dy = Math.Abs(y2 - y1);
            int sx = x1 < x2 ? 1 : -1;
            int sy = y1 < y2 ? 1 : -1;
            int err = dx - dy;
            int currentX = x1;
            int currentY = y1;

            int distance = Math.Max(dx, dy);

            for (int i = 0; i <= distance; i++)
            {
                // Check if current coordinates are within image bounds
                if (currentX >= 0 && currentX < image.Width && currentY >= 0 && currentY < image.Height)
                {
                    // Get the intensity of the pixel at current coordinates
                    byte intensity = Marshal.ReadByte(image.DataPointer + currentY * image.Step + currentX); // Assuming grayscale image

                    // Add the intensity and its corresponding coordinates to the list
                    linePoints.Add(new ObservablePoint(i + 1, intensity));
                }

                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    currentX += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    currentY += sy;
                }
            }

            return linePoints;
        }
    }
}
