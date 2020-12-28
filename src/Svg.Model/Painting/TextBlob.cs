﻿using System;
using Svg.Model.Primitives;

namespace Svg.Model.Painting
{
    public sealed class TextBlob : IDisposable
    {
        public string? Text { get; set; }
        public Point[]? Points { get; set; }

        public void Dispose()
        {
        }
    }
}