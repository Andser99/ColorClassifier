using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
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
            pointMap = new PointMap(5000);
            AccuracyTextBox.Text = "Accuracy: " + pointMap.AddRandom(3, 10000).ToString("0.##");
        }

        private void Generate_Click(object sender, EventArgs e)
        {
            new Task(() =>
            {
                if (ImageBox.Image != null)
                {
                    ImageBox.Image.Dispose();
                }
                int size = pointMap.GetSize();

                Bitmap bMap = new Bitmap(1, 1);
                bMap.SetPixel(0, 0, Color.White);
                bMap = new Bitmap(bMap, 10001, 10001);
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

                ImageBox.Image = Image.FromFile("img.bmp");
            }).Start();
            
        }
    }
}
