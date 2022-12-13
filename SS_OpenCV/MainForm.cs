using System;
using System.Drawing;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;

namespace SS_OpenCV
{ 
    public partial class MainForm : Form
    {
        Image<Bgr, Byte> img = null; // working image
        Image<Bgr, Byte> imgUndo = null; // undo backup image - UNDO
        string title_bak = "";
        int mouseX, mouseY;
        bool mouseFlag = false;

        public MainForm()
        {
            InitializeComponent();
            title_bak = Text;
        }

        /// <summary>
        /// Opens a new image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                img = new Image<Bgr, byte>(openFileDialog1.FileName);
                Text = title_bak + " [" +
                        openFileDialog1.FileName.Substring(openFileDialog1.FileName.LastIndexOf("\\") + 1) +
                        "]";
                imgUndo = img.Copy();
                ImageViewer.Image = img;
                ImageViewer.Refresh();
            }
        }

        /// <summary>
        /// Saves an image with a new name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ImageViewer.Image.Save(saveFileDialog1.FileName);
            }
        }

        /// <summary>
        /// Closes the application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// restore last undo copy of the working image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (imgUndo == null) // verify if the image is already opened
                return; 
            Cursor = Cursors.WaitCursor;
            img = imgUndo.Copy();

            ImageViewer.Image = img;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor 
        }

        /// <summary>
        /// Change visualization mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void autoZoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // zoom
            if (autoZoomToolStripMenuItem.Checked)
            {
                ImageViewer.SizeMode = PictureBoxSizeMode.Zoom;
                ImageViewer.Dock = DockStyle.Fill;
            }
            else // with scroll bars
            {
                ImageViewer.Dock = DockStyle.None;
                ImageViewer.SizeMode = PictureBoxSizeMode.AutoSize;
            }
        }

        /// <summary>
        /// Show authors form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void autoresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AuthorsForm form = new AuthorsForm();
            form.ShowDialog();
        }

        /// <summary>
        /// Calculate the image negative
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void negativeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //copy Undo Image
            imgUndo = img.Copy();

            ImageClass.Negative(img);

            ImageViewer.Image = img;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor 
        }

        /// <summary>
        /// Call automated image processing check
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void evalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EvalForm eval = new EvalForm();
            eval.ShowDialog();
        }

        /// <summary>
        /// Call image convertion to gray scale
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //copy Undo Image
            imgUndo = img.Copy();

            ImageClass.ConvertToGray(img);

            ImageViewer.Image = img;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor 
        }

        private void ImageViewer_MouseMove(object sender, MouseEventArgs e)
        {
            int aux_x = 0;
            int aux_y = 0;
            if (ImageViewer.SizeMode == PictureBoxSizeMode.Zoom)
            {
                aux_x = (int)(e.X / ImageViewer.ZoomScale + ImageViewer.HorizontalScrollBar.Value * ImageViewer.ZoomScale);
                aux_y = (int)(e.Y / ImageViewer.ZoomScale + ImageViewer.VerticalScrollBar.Value * ImageViewer.ZoomScale);

            }
            else
            {
                aux_x = (int)(e.X / ImageViewer.ZoomScale + ImageViewer.HorizontalScrollBar.Value * ImageViewer.ZoomScale);
                aux_y = (int)(e.Y / ImageViewer.ZoomScale + ImageViewer.VerticalScrollBar.Value * ImageViewer.ZoomScale);
            }


            if (img != null && aux_y < img.Height && aux_x < img.Width)
                statusLabel.Text = "X:" + aux_x + "  Y:" + aux_y + " - BGR = (" + img.Data[aux_y, aux_x, 0] + "," + img.Data[aux_y, aux_x, 1] + "," + img.Data[aux_y, aux_x, 2] + ")";

        }

        private void redChannelToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //copy Undo Image
            imgUndo = img.Copy();

            ImageClass.RedChannel(img);

            ImageViewer.Image = img;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor 
        }

        private void brightContrastToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 
            
            //copy Undo Image
            imgUndo = img.Copy();

            InputBox formBright = new InputBox("Brilho?");
            InputBox formContrast = new InputBox("Contraste?");

            formBright.ShowDialog();
            int bright = Convert.ToInt32(formBright.ValueTextBox.Text);
            formContrast.ShowDialog();
            double contrast = Convert.ToSingle(formContrast.ValueTextBox.Text);
            ImageClass.BrightContrast(img, bright, contrast);

            ImageViewer.Image = img;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor 
        }

        private void rotationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //copy Undo Image
            imgUndo = img.Copy();

            InputBox formRotation = new InputBox("Rotação?");

            formRotation.ShowDialog();
            float rotation = Convert.ToSingle(formRotation.ValueTextBox.Text);
            ImageClass.Rotation(img, imgUndo, rotation);

            ImageViewer.Image = img;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor
        }

        private void translationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //copy Undo Image
            imgUndo = img.Copy();

            InputBox formXtranslation = new InputBox("Translação em x?");
            InputBox formYtranslation = new InputBox("Translação em y?");

            formXtranslation.ShowDialog();
            int Xtranslation = Convert.ToInt32(formXtranslation.ValueTextBox.Text);

            formYtranslation.ShowDialog();
            int Ytranslation = Convert.ToInt32(formYtranslation.ValueTextBox.Text);


            ImageClass.Translation(img, img.Copy(), Xtranslation, Ytranslation);

            ImageViewer.Image = img;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor
        }

        private void zoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //copy Undo Image
            imgUndo = img.Copy();

            ImageClass.Mean(img,img);

            ImageViewer.Image = img;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor 
        }

        private void scaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //copy Undo Image
            imgUndo = img.Copy();

            InputBox formScaleFactor = new InputBox("fator de escalonamento?");

            formScaleFactor.ShowDialog();
            float scaleFactor = Convert.ToSingle(formScaleFactor.ValueTextBox.Text);
            ImageClass.Scale(img, img, scaleFactor);

            ImageViewer.Image = img;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor

        }

        private void ImageViewer_MouseClick(object sender, MouseEventArgs e)
        {
            if (mouseFlag)
            {
                mouseX = e.X;
                mouseY = e.Y;
                mouseFlag = false;
            }
        }

        private void mediaToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void sobelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //copy Undo Image
            imgUndo = img.Copy();

            ImageClass.Sobel(img, img);

            ImageViewer.Image = img;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor
        }

        private void diferentiationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //copy Undo Image
            imgUndo = img.Copy();

            ImageClass.Diferentiation(img, img);

            ImageViewer.Image = img;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor
        }

        private void nonUniformToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;

            Form1 nonUnifParam = new Form1();
            nonUnifParam.ShowDialog();

            float[,] matrix = nonUnifParam.matrix;
            float weight = nonUnifParam.weight;
            float offset = nonUnifParam.offset;

            Cursor = Cursors.WaitCursor; // clock cursor 

            //copy Undo Image
            imgUndo = img.Copy();

            ImageClass.NonUniform(img, imgUndo, matrix, weight, offset);

            ImageViewer.Image = img;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor
        }

        private void grayHistogramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //copy Undo Image
            imgUndo = img.Copy();

            HistogramForm form = new HistogramForm(ImageClass.Histogram_Gray(img));
            form.ShowDialog();


            ImageViewer.Image = img;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor 
        }

        private void colorHistogramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //copy Undo Image
            imgUndo = img.Copy();

            HistogramForm form = new HistogramForm(ImageClass.Histogram_RGB(img));
            form.ShowDialog();


            ImageViewer.Image = img;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor 
        }

        private void allHistogramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //copy Undo Image
            imgUndo = img.Copy();

            HistogramForm form = new HistogramForm(ImageClass.Histogram_All(img));
            form.ShowDialog();


            ImageViewer.Image = img;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor 
        }

        private void binarizationWithThresholdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //copy Undo Image
            imgUndo = img.Copy();

            InputBox formThreshold = new InputBox("limite?");

            formThreshold.ShowDialog();
            int threshold = Convert.ToInt32(formThreshold.ValueTextBox.Text);


            ImageClass.ConvertToBW(img, threshold);

            ImageViewer.Image = img;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor
        }

        private void trabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //copy Undo Image
            imgUndo = img.Copy();
 

            ImageClass.QRCodeReader(img, img.Copy(), 1, out _, out _, out _, out _, out _, out _, out _, out _, out _, out _, out _, out _);
            
            ImageViewer.Image = img;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor
        }

        private void scalepointxyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            mouseFlag = true;

            while (mouseFlag)
            {
                Application.DoEvents();
            }

            //copy Undo Image
            imgUndo = img.Copy();

            InputBox formScaleFactor = new InputBox("fator de escalonamento?");

            formScaleFactor.ShowDialog();
            float scaleFactor = Convert.ToSingle(formScaleFactor.ValueTextBox.Text);
            ImageClass.Scale_point_xy(img, img, scaleFactor,mouseX,mouseY);

            ImageViewer.Image = img;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor
        }
    }

}