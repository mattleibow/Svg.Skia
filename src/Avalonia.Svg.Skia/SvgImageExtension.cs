﻿using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Svg.Model;

namespace Avalonia.Svg.Skia;

/// <summary>
/// Svg image markup extension.
/// </summary>
public class SvgImageExtension : MarkupExtension
{
    /// <summary>
    /// Initialises a new instance of an <see cref="SvgImageExtension"/>.
    /// </summary>
    /// <param name="path">The resource or file path</param>
    public SvgImageExtension(string path) => Path = path;

    /// <summary>
    /// Gets or sets resource or file path.
    /// </summary>
    public string Path { get; }

    /// <inheritdoc/>
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var path = Path;
        var context = (IUriContext)serviceProvider.GetService(typeof(IUriContext))!;
        var baseUri = context.BaseUri;
        var target = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget))!;
        var targetControl = target.TargetObject as Control;
        var image = CreateImage(path, baseUri, targetControl);

        if (target.TargetProperty is not AvaloniaProperty property)
        {
            return image;
        }

        if (property.PropertyType == typeof(IImage))
        {
            return image;
        }

        return new Image { Source = image };
    }

    private static IImage CreateImage(string path, Uri baseUri, Control? targetControl)
    {
        if (targetControl is not null)
        {
            var style = Svg.GetStyle(targetControl);
            var currentStyle = Svg.GetCurrentStyle(targetControl);
            var source = SvgSource.Load<SvgSource>(
                path,
                baseUri,
                new SvgParameters(null, string.Concat(style, ' ', currentStyle)));

            return CreateSvgImage(source, targetControl);
        }
        else
        {
            var source = SvgSource.Load<SvgSource>(
                path,
                baseUri);
            
            return CreateSvgImage(source, targetControl);
        }
    }

    private static SvgImage CreateSvgImage(SvgSource? source, Control? targetControl)
    {
        var result = new SvgImage
        {
            Source = source
        };

        if (targetControl == null)
        {
            return result;
        }

        var styleBinding = targetControl.GetObservable(Svg.StyleProperty).ToBinding();
        var currentStyleBinding = targetControl.GetObservable(Svg.CurrentStyleProperty).ToBinding();

        result.Bind(SvgImage.StyleProperty, styleBinding);
        result.Bind(SvgImage.CurrentStyleProperty, currentStyleBinding);

        return result;
    }
}
