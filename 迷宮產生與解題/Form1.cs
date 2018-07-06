using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.IO;

namespace 迷宮產生與解題
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            _gen.Enabled = true;
            _save.Enabled = true;
            solve.Enabled = true;
            this.pictureBox1.Image = null;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {


        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void kp(object sender, KeyPressEventArgs e)
        {
            char key = e.KeyChar;
            if (key < '0' || key > '9')
            {
                e.Handled = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "save Maze File";
            sfd.Filter = "jpg Files(*.jpg)|*.jpg|png Files(*.png)|*.png";

            if (this.pictureBox1.Image != null)
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    if ((sfd.FileName) != string.Empty)
                    {
                        pictureBox1.Image.Save(sfd.FileName);
                    }
                }
            }
            else
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    if ((sfd.FileName) != string.Empty)
                    {
                        try
                        {
                            int width = Convert.ToInt32(w.Text);
                            int height = Convert.ToInt32(h.Text);
                            int size = 2;
                            MazeGenerator mg = new MazeGenerator(width, height);
                            mg.Generator();
                            mg._result(size).Save(sfd.FileName);
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }

        private void _gen_Click(object sender, EventArgs e)
        {
            try
            {
                int width = Convert.ToInt32(w.Text);
                int height = Convert.ToInt32(h.Text);
                int size = 2;
                MazeGenerator mg = new MazeGenerator(width, height);
                mg.Generator();
                this.pictureBox1.Image = mg._result(size);

            }
            catch (InvalidCastException)
            {
                MessageBox.Show("請輸入數字", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch
            {
                MessageBox.Show("有地方出錯了", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void _openf_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Open Maze File";
            ofd.Filter = "jpg Files(*.jpg)|*.jpg|png Files(*.png)|*.png";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if (ofd.OpenFile() != null)
                {
                    Bitmap bmp = new Bitmap(ofd.OpenFile());
                    this.pictureBox1.Image = bmp;
                    solve.Enabled = true;
                }
            }
        }

        private class SaveStruct
        {
            public List<Bitmap> bitmaps;
            public int delaytime;
            public SaveStruct(int dt,List<Bitmap> gt)
            {
                delaytime = dt;
                bitmaps = gt;
            }
        }

        private void solve_Click(object sender, EventArgs e)
        {
            try
            {
                MazeSolver ms = new MazeSolver((Bitmap)this.pictureBox1.Image, 128);
                Stack<Node> result = ms.Solver();
                if (result.Count >1)
                {
                    Bitmap bitmap = (Bitmap)pictureBox1.Image;
                    label5.Text = result.Count.ToString();
                    while (result.Count > 0)
                    {
                        Node current = result.Pop();
                        Graphics g = Graphics.FromImage(bitmap);
                        g.FillRectangle(new SolidBrush(Color.Red), new Rectangle(current._pos, new Size(1, 1)));
                    }
                    pictureBox1.Image = bitmap;
                }
                else MessageBox.Show("懶得解啦", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            catch (InvalidCastException)
            {

            }
            catch
            {

            }

        }
    }
}
