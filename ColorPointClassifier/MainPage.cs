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

        private async void Generate_Click(object sender, EventArgs e)
        {
            if (KNNListBox.SelectedIndex == -1 || KNNListBox.SelectedIndex == 0)
            {
                DebugTextBox.Text += "Select a k-value first \n";
                return;
            }
            DebugTextBox.Text += "Generating 40000 points, please wait \n";
            int kValue = Convert.ToInt32(KNNListBox.SelectedItem);
            double accuracy = 0;
            await Task.Run(() =>
            {
                accuracy = pointMap.AddRandom(kValue, 10);
            });

            AccuracyTextBox.Text = $"Accuracy: {accuracy:0.##}";
        }

        private void FillRestButton_Click(object sender, EventArgs e)
        {
            FillingProgressBar.Maximum = 100;
            FillingProgressBar.Step = 1;
            FillingProgressBar.Value = 0;
            if (KNNListBox.SelectedIndex == -1 && KNNListBox.SelectedIndex == 0)
            {
                DebugTextBox.Text += "Select a k-value first\n";
            }
            else
            {
                DebugTextBox.Text += "Starting to fill the map\n";
                pointMap.FillRest(Convert.ToInt32(KNNListBox.SelectedItem), FillingProgressBar);
            }

        }

        private void DrawButton_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                if (ImageBox.Image != null)
                {
                    ImageBox.Image.Dispose();
                }
                int size = pointMap.GetSize() + 1;

                Bitmap bMap = new Bitmap(2, 2);
                bMap.SetPixel(0, 0, Color.White);
                bMap.SetPixel(0, 1, Color.White);
                bMap.SetPixel(1, 0, Color.White);
                bMap.SetPixel(1, 1, Color.White);
                bMap = new Bitmap(bMap, 10002, 10002);
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
                if (File.Exists("img.bmp"))
                {
                    File.Delete("img.bmp");
                }
                bMap.Save("img.bmp", ImageFormat.Bmp);

                ImageBox.Image = new Bitmap(Image.FromFile("img.bmp"), ImageBox.Width, ImageBox.Height);
            });
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            pointMap.Reset();
        }
    }
}
