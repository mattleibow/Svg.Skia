﻿using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace SvgToPng
{
    public partial class MainWindow : Window, IConvertProgress, ISaveProgress
    {
        public ObservableCollection<Item> Items { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            var currentDirectory = Directory.GetCurrentDirectory();
            var outputPath = Path.Combine(currentDirectory, "png");
            TextOutputPath.Text = outputPath;

            this.Loaded += MainWindow_Loaded;

            Items = new ObservableCollection<Item>();
            DataContext = this;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            items.SelectionChanged += Items_SelectionChanged;
            skia.PaintSurface += Canvas_PaintSurface;
            skia.InvalidateVisual();
        }

        private void Items_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            skia.InvalidateVisual();
        }

        private void Canvas_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;

            canvas.Clear(SKColors.White);

            if (items.SelectedItem is Item item)
            {
                if (item.Picture != null)
                {
                    skia.Width = item.Picture.CullRect.Width;
                    skia.Height = item.Picture.CullRect.Height;
                    canvas.DrawPicture(item.Picture);
                }
            }
        }

        public async Task HandleDrop(string[] paths, string referencePath)
        {
            var inputFiles = SvgToPngConverter.GetFilesDrop(paths).ToList();
            if (inputFiles.Count > 0)
            {
                await SvgToPngConverter.Convert(inputFiles, Items, referencePath, this);
            }
        }

        public async Task ConvertStatusReset()
        {
            await Dispatcher.InvokeAsync(() =>
            {
                TextProgress.Text = $"";
                TextInputFile.Text = $"";
                TextOutputFile.Text = $"";
            });
        }

        public async Task ConvertStatus(string message)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                TextProgress.Text = message;
            });
        }

        public async Task ConvertStatusProgress(int count, int total, string inputFile)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                TextProgress.Text = $"Conterting file {count}/{total}";
                TextInputFile.Text = $"{inputFile}";
            });
        }

        public async Task SaveStatusReset()
        {
            await Dispatcher.InvokeAsync(() =>
            {
                TextProgress.Text = $"";
                TextInputFile.Text = $"";
                TextOutputFile.Text = $"";
            });
        }

        public async Task SaveStatusProgress(int count, int total, string inputFile, string outputFile)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                TextProgress.Text = $"Saving file {count}/{total}";
                TextInputFile.Text = $"{inputFile}";
                TextOutputFile.Text = $"{outputFile}";
            });
        }

        public async Task SaveStatusDone()
        {
            await Dispatcher.InvokeAsync(() =>
            {
                TextProgress.Text = $"Done";
                TextInputFile.Text = $"";
                TextOutputFile.Text = $"";
            });
        }

        private async void Window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (paths != null && paths.Length > 0)
                {
                    await HandleDrop(paths, TextReferencePath.Text);
                }
            }
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            var items = Items.ToList();

            Items.Clear();

            foreach (var item in items)
            {
                item.Image = null;
                item.Bytes = null;
                item.Picture = null;
                item.Skia?.Dispose();
            }
        }

        private async void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = "Supported Files (*.svg;*.svgz)|*.svg;*.svgz|Svg Files (*.svg)|*.svg;|Svgz Files (*.svgz)|*.svgz|All Files (*.*)|*.*",
                Multiselect = true,
                FilterIndex = 0
            };
            if (dlg.ShowDialog() == true)
            {
                var paths = dlg.FileNames;
                if (paths != null && paths.Length > 0)
                {
                    await HandleDrop(paths, TextReferencePath.Text);
                }
            }
        }
        private async void ButtonSavePng_Click(object sender, RoutedEventArgs e)
        {
            string outputPath = TextOutputPath.Text;
            await SvgToPngConverter.Save(outputPath, Items, this);
        }
    }
}
