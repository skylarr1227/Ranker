﻿using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Ranker
{
    public class Extentions
    {
        // By @Ahmed605

        public static IPathCollection GoodBuildCorners(int imageWidth, int imageHeight, float cornerRadius)
        {
            var rect1 = new RectangularPolygon(cornerRadius / 2, imageHeight - cornerRadius, cornerRadius / 2, imageHeight - (cornerRadius / 2));
            var rect2 = new RectangularPolygon(imageWidth - (cornerRadius / 2), imageHeight - cornerRadius, cornerRadius / 2, imageHeight - (cornerRadius / 2));
            var rect3 = new RectangularPolygon(cornerRadius / 2, 0, imageWidth - cornerRadius, imageHeight);

            IPath corner1 = new EllipsePolygon(cornerRadius / 2, cornerRadius / 2, cornerRadius / 2);
            IPath corner2 = new EllipsePolygon(cornerRadius / 2, imageHeight - (cornerRadius / 2), cornerRadius / 2);
            IPath corner3 = new EllipsePolygon(imageWidth - (cornerRadius / 2), cornerRadius / 2, cornerRadius / 2);
            IPath corner4 = new EllipsePolygon(imageWidth - (cornerRadius / 2), imageHeight - (cornerRadius / 2), cornerRadius / 2);

            return new PathCollection(corner1, corner2, corner3, corner4, rect1, rect2, rect3);
        }

        public static Image RoundCorners(Image<Rgba32> image, int? radius)
        {
            int imageWidth = image.Width;
            int imageHeight = image.Height;
            int cornerRadius = radius ?? ((imageWidth + imageHeight) / 2 / 2);
            var rect = new RectangularPolygon(-0.5f, -0.5f, cornerRadius, cornerRadius);

            IPath cornerTopLeft = rect.Clip(new EllipsePolygon(cornerRadius - 0.5f, cornerRadius - 0.5f, cornerRadius));

            float rightPos = imageWidth - cornerTopLeft.Bounds.Width + 1;
            float bottomPos = imageHeight - cornerTopLeft.Bounds.Height + 1;

            IPath cornerTopRight = cornerTopLeft.RotateDegree(90).Translate(rightPos, 0);
            IPath cornerBottomLeft = cornerTopLeft.RotateDegree(-90).Translate(0, bottomPos);
            IPath cornerBottomRight = cornerTopLeft.RotateDegree(180).Translate(rightPos, bottomPos);

            var newImage = image;
            foreach (var c in new PathCollection(cornerTopLeft, cornerBottomLeft, cornerTopRight, cornerBottomRight))
            {
                newImage.Mutate(x => x.Fill(Color.Black, c));
            }
            return newImage;
        }

        public static Image RoundCorners2(Image<Rgba32> image, float radius)
        {
            return image.Clone(x => ApplyRoundedCorners2(x, radius));
        }

        // By Six Labors.

        private static IImageProcessingContext ApplyRoundedCorners2(IImageProcessingContext ctx, float cornerRadius)
        {
            Size size = ctx.GetCurrentSize();
            IPathCollection corners = BuildCorners2(size.Width, size.Height, cornerRadius);


            ctx.SetGraphicsOptions(new GraphicsOptions()
            {
                Antialias = true,
                AlphaCompositionMode = PixelAlphaCompositionMode.DestOut // enforces that any part of this shape that has color is punched out of the background
            });

            // mutating in here as we already have a cloned original
            // use any color (not Transparent), so the corners will be clipped
            foreach (var c in corners)
            {
                ctx = ctx.Fill(Color.Red, c);
            }
            return ctx;
        }

        private static IPathCollection BuildCorners2(int imageWidth, int imageHeight, float cornerRadius)
        {
            var rect = new RectangularPolygon(-0.5f, -0.5f, cornerRadius, cornerRadius);


            // then cut out of the square a circle so we are left with a corner
            IPath cornerTopLeft = rect.Clip(new EllipsePolygon(cornerRadius - 0.5f, cornerRadius - 0.5f, cornerRadius));


            // corner is now a corner shape positions top left
            //lets make 3 more positioned correctly, we can do that by translating the original around the center of the image


            float rightPos = imageWidth - cornerTopLeft.Bounds.Width + 1;
            float bottomPos = imageHeight - cornerTopLeft.Bounds.Height + 1;


            // move it across the width of the image - the width of the shape
            IPath cornerTopRight = cornerTopLeft.RotateDegree(90).Translate(rightPos, 0);
            IPath cornerBottomLeft = cornerTopLeft.RotateDegree(-90).Translate(0, bottomPos);
            IPath cornerBottomRight = cornerTopLeft.RotateDegree(180).Translate(rightPos, bottomPos);


            return new PathCollection(cornerTopLeft, cornerBottomLeft, cornerTopRight, cornerBottomRight);
        }
    }
}
