﻿
using Svg.Model.Painting;

namespace Svg.Model.ImageFilters
{
    public sealed class ColorFilterImageFilter : ImageFilter
    {
        public ColorFilter? ColorFilter { get; set; }
        public ImageFilter? Input { get; set; }
        public CropRect? CropRect { get; set; }
    }
}