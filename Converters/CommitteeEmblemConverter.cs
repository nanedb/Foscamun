using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Foscamun.Data;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;

namespace Foscamun.Converters
{
    /// <summary>
    /// Converts a committee name to its logo drawn in black, without the caption
    /// that many logos carry under the emblem. Used where the name is already written beside it.
    /// </summary>
    public class CommitteeEmblemConverter : IValueConverter
    {
        // Height of the bitmap used to find the caption
        private const int ProbeHeight = 400;

        // The bottom band counts as a caption only if it is at most this share of the logo's height
        private const double MaxCaptionShare = 0.25;

        private static readonly Dictionary<string, (DateTime Stamp, ImageSource Image)> Cache = new();

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string name || name.Length == 0)
                return null;

            string path = SqliteDataAccess.GetCommitteeLogoPath(name);
            if (!File.Exists(path))
                return null;

            DateTime stamp = File.GetLastWriteTimeUtc(path);
            if (Cache.TryGetValue(path, out var cached) && cached.Stamp == stamp)
                return cached.Image;

            try
            {
                var image = BuildEmblem(path);
                Cache[path] = (stamp, image);
                return image;
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static ImageSource BuildEmblem(string path)
        {
            var settings = new WpfDrawingSettings { IncludeRuntime = false, TextAsGeometry = true };
            using var reader = new FileSvgReader(settings);
            DrawingGroup drawing = reader.Read(path).Clone();

            // Logos are white on transparent: paint every shape black
            Recolor(drawing, Brushes.Black);

            Rect bounds = drawing.Bounds;
            double? captionTop = FindCaptionTop(drawing, bounds);
            if (captionTop is double top)
            {
                drawing = new DrawingGroup
                {
                    Children = { drawing },
                    ClipGeometry = new RectangleGeometry(new Rect(bounds.X, bounds.Y, bounds.Width, top - bounds.Y))
                };
            }

            var image = new DrawingImage(drawing);
            image.Freeze();
            return image;
        }

        private static void Recolor(Drawing drawing, Brush brush)
        {
            switch (drawing)
            {
                case DrawingGroup group:
                    foreach (var child in group.Children)
                        Recolor(child, brush);
                    break;
                case GeometryDrawing geometry:
                    if (geometry.Brush != null) geometry.Brush = brush;
                    if (geometry.Pen != null) geometry.Pen.Brush = brush;
                    break;
                case GlyphRunDrawing glyphs:
                    glyphs.ForegroundBrush = brush;
                    break;
                case ImageDrawing picture when picture.ImageSource is BitmapSource bitmap:
                    // Older logos embed a PNG: keep its alpha, make it black
                    picture.ImageSource = Blacken(bitmap);
                    break;
            }
        }

        private static BitmapSource Blacken(BitmapSource source)
        {
            var converted = new FormatConvertedBitmap(source, PixelFormats.Pbgra32, null, 0);
            int stride = converted.PixelWidth * 4;
            var pixels = new byte[stride * converted.PixelHeight];
            converted.CopyPixels(pixels, stride, 0);

            // Premultiplied black is zero color with the original alpha
            for (int i = 0; i < pixels.Length; i += 4)
            {
                pixels[i] = 0;
                pixels[i + 1] = 0;
                pixels[i + 2] = 0;
            }

            var result = BitmapSource.Create(converted.PixelWidth, converted.PixelHeight,
                source.DpiX, source.DpiY, PixelFormats.Pbgra32, null, pixels, stride);
            result.Freeze();
            return result;
        }

        /// <summary>
        /// Looks for a short band of content at the bottom, separated from the emblem by an empty gap.
        /// Returns the drawing's y where the gap starts, or null if there is no caption.
        /// </summary>
        private static double? FindCaptionTop(Drawing drawing, Rect bounds)
        {
            if (bounds.IsEmpty || bounds.Height <= 0)
                return null;

            double scale = ProbeHeight / bounds.Height;
            int width = Math.Max(1, (int)Math.Ceiling(bounds.Width * scale));

            var visual = new DrawingVisual();
            using (var dc = visual.RenderOpen())
            {
                dc.PushTransform(new ScaleTransform(scale, scale));
                dc.PushTransform(new TranslateTransform(-bounds.X, -bounds.Y));
                dc.DrawDrawing(drawing);
            }

            var bitmap = new RenderTargetBitmap(width, ProbeHeight, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(visual);

            int stride = width * 4;
            var pixels = new byte[stride * ProbeHeight];
            bitmap.CopyPixels(pixels, stride, 0);

            var filled = new bool[ProbeHeight];
            for (int y = 0; y < ProbeHeight; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (pixels[y * stride + x * 4 + 3] > 32)
                    {
                        filled[y] = true;
                        break;
                    }
                }
            }

            // Last filled row, then the empty gap above the bottom band
            int bottom = Array.LastIndexOf(filled, true);
            if (bottom < 0)
                return null;

            int bandTop = bottom;
            while (bandTop > 0 && filled[bandTop - 1])
                bandTop--;

            int gapTop = bandTop;
            while (gapTop > 0 && !filled[gapTop - 1])
                gapTop--;

            bool hasGap = gapTop > 0 && gapTop < bandTop;
            bool isShort = bottom - bandTop + 1 <= MaxCaptionShare * (bottom + 1);
            if (!hasGap || !isShort)
                return null;

            return bounds.Y + gapTop / scale;
        }
    }
}
