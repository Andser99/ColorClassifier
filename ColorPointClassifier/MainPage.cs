using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ColorPointClassifier
{
    public partial class MainPage : Form
    {

        PointMap pointMap;
        public MainPage()
        {
            InitializeComponent();
        }

        private void MainPage_Load(object sender, EventArgs e)
        {
            pointMap = new PointMap(5000, 3);
        }

        private void DebugLine(string text)
        {
            DebugTextBox.Text += text + Environment.NewLine;
        }

        private async void Generate_Click(object sender, EventArgs e)
        {
            if (KNNListBox.SelectedIndex == -1)
            {
                DebugLine("Select a k-value first");
                return;
            }
            pointMap.SetSeed(Convert.ToInt32(SeedPicker.Value));
            DebugLine($"Generating {GenerateLimitedPicker.Value} points, please wait for accuracy results.");
            int kValue = Convert.ToInt32(KNNListBox.SelectedItem);
            double accuracy = 0;
            await Task.Run(() =>
            {
                accuracy = pointMap.AddRandom(kValue, Convert.ToInt32(GenerateLimitedPicker.Value));
            });

            DebugLine($"Accuracy: {accuracy:0.##}");
        }

        private void FillRestButton_Click(object sender, EventArgs e)
        {
            FillingProgressBar.Maximum = 100;
            FillingProgressBar.Step = 1;
            FillingProgressBar.Value = 0;
            if (KNNListBox.SelectedIndex == -1 && KNNListBox.SelectedIndex == 0)
            {
                DebugLine("Select a k-value first");
            }
            else
            {
                DebugLine("Starting to fill the map");
                pointMap.FillRest(Convert.ToInt32(KNNListBox.SelectedItem), FillingProgressBar);
            }

        }

        private void DrawButton_Click(object sender, EventArgs e)
        {
            DebugLine("Drawing image, wait for it to show");
            Task.Run(() =>
            {
                if (ImageBox.Image != null)
                {
                    ImageBox.Image.Dispose();
                }
                int size = pointMap.GetSize() + 1;

                Bitmap bMap = new Bitmap(16, 16);
                for (int i = 0; i < bMap.Width; i++)
                {
                    for (int j = 0; j < bMap.Height; j++)
                    {
                        bMap.SetPixel(i, j, Color.White);
                    }
                }
                bMap = new Bitmap(bMap, pointMap.GetSize() + 1, pointMap.GetSize() + 1);
                for (int i = 0; i < size * size; i++)
                {
                    byte value = pointMap.GetByteAt(i / size, i % size);
                    if (value != 0)
                    {
                        Color color = Color.White;
                        switch (value)
                        {
                            case 1:
                                color = Color.Red;
                                break;
                            case 2:
                                color = Color.Blue;
                                break;
                            case 3:
                                color = Color.Green;
                                break;
                            case 4:
                                color = Color.Purple;
                                break;
                        }
                        bMap.SetPixel(i / size, i % size, color);
                    }
                }

                //Clean up old image
                if (File.Exists("img.png"))
                {
                    File.Delete("img.png");
                }
                bMap.Save("img.png", ImageFormat.Png);

                ImageBox.Image = new Bitmap(Image.FromFile("img.png"), ImageBox.Width, ImageBox.Height);
            });
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            pointMap.Reset();
        }
    }
}
