using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace Clock
{
    public partial class ClockForm : Form {
        private Color _innerColor = Color.Aquamarine, _outerColor = Color.Aqua, _handColor = Color.Black, _secHandColor = Color.Red, _tickColor = Color.Black, _textColor = Color.Black;

        private void timer_Tick(object sender, EventArgs e) {
            this.InvokePaint(panel, new PaintEventArgs(Graphics.FromHwnd(panel.Handle), new Rectangle(panel.Location.X, panel.Location.Y, panel.Size.Width, panel.Size.Height)));
        }

        public ClockForm() {
            InitializeComponent();
            
            if (!timer.Enabled) {
                timer.Start();
            }
        }

        private void panel_Paint(object sender, PaintEventArgs e) {
            var gOne = e.Graphics;
            BufferedGraphicsContext CurrentContext = BufferedGraphicsManager.Current;
            BufferedGraphics g = CurrentContext.Allocate(gOne, ClientRectangle);
            g.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            g.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            g.Graphics.Clear(panel.BackColor);
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            var Ox = panel.Width / 2;
            var Oy = panel.Height / 2;
            int sideSquare;

            if (panel.Width > panel.Height) {
                sideSquare = panel.Height;
            } else {
                sideSquare = panel.Width;
            }

            // elipse
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(new Rectangle(panel.Width / 20, panel.Height / 20, panel.Width - (panel.Width / 20) * 2, panel.Height - (panel.Height / 20) * 2));
            PathGradientBrush pthGrBrush = new PathGradientBrush(path);
            pthGrBrush.CenterColor = _innerColor;
            Color[] colors = { _outerColor };
            pthGrBrush.SurroundColors = colors;
            g.Graphics.FillEllipse(pthGrBrush, new Rectangle(panel.Width / 20, panel.Height / 20, panel.Width - (panel.Width / 20) * 2, panel.Height - (panel.Height / 20) * 2));

            // ticks
            g.Graphics.DrawEllipse(new Pen(_tickColor), new Rectangle(panel.Width / 20, panel.Height / 20, panel.Width - (panel.Width / 20) * 2, panel.Height - (panel.Height / 20) * 2));
            g.Graphics.TranslateTransform(Ox, Oy);

            for (int i = 0; i < 4; i++) {
                g.Graphics.DrawLine(new Pen(_tickColor, 6), new Point((panel.Width / 2 - panel.Width / 20) * -1, 0), new Point((panel.Width / 2 - panel.Width / 20) * -1 + 13, 0));
                for (int j = 0; j < 15; j++)
                {
                    g.Graphics.DrawLine(new Pen(_tickColor, 1), new Point((panel.Width / 2 - panel.Width / 20) * -1, 0), new Point((panel.Width / 2 - panel.Width / 20) * -1 + 9, 0));
                    if (j % 5 == 0)
                        g.Graphics.DrawLine(new Pen(_tickColor, 4), new Point((panel.Width / 2 - panel.Width / 20) * -1, 0), new Point((panel.Width / 2 - panel.Width / 20) * -1 + 13, 0));
                    g.Graphics.RotateTransform(6);
                }
            }

            // Draw1
            g.Graphics.ResetTransform();
            g.Graphics.TranslateTransform(Ox, Oy);
            double radius = ((panel.Width - panel.Width / 20.0 * 2) / 2 - 28);
            float x0 = 0;
            float y0 = -(float)radius;

            for (int i = 1; i < 13; i++) {
                if (i == 10 || i == 11) {
                    y0 = -(float)(radius - 3);
                }
                var rx = x0 - 0;
                var ry = y0 - 0;
                var c = Math.Cos((30 * i) * Math.PI / 180);
                var s = Math.Sin((30 * i) * Math.PI / 180);
                float x1 = (float)(0 + rx * c - ry * s);
                float y1 = (float)(0 + rx * s + ry * c);
                g.Graphics.DrawString(i.ToString(), new Font("Calibri", 20, FontStyle.Bold), new SolidBrush(_textColor), x1, y1, stringFormat);
            }

            // Draw2
            var angleSec = DateTime.Now.Second * 6;

            g.Graphics.ResetTransform();
            g.Graphics.TranslateTransform(Ox, Oy);
            g.Graphics.RotateTransform(angleSec);
            g.Graphics.DrawLine(new Pen(_secHandColor), 0, 30, 0, -133);

            // Draw3
            g.Graphics.ResetTransform();
            g.Graphics.TranslateTransform(Ox, Oy);
            var angleMin = DateTime.Now.Minute * 6;
            var angleHour = DateTime.Now.Hour * 30 + 30.0 / 100.0 * (DateTime.Now.Minute / (60.0 / 100.0));

            g.Graphics.FillEllipse(new SolidBrush(_handColor), -7, -7, 14, 14);
            g.Graphics.RotateTransform(angleMin);
            g.Graphics.FillPolygon(new SolidBrush(_handColor), new Point[]{new Point(0, 0), new Point(5, -30), new Point(0, -105), new Point(-5, -30)});
            g.Graphics.ResetTransform();
            g.Graphics.TranslateTransform(Ox, Oy);
            g.Graphics.RotateTransform((float)angleHour);
            g.Graphics.FillPolygon(new SolidBrush(_handColor), new Point[] { new Point(0, 0), new Point(5, -24), new Point(0, -75), new Point(-5, -24) });

            g.Graphics.FillEllipse(new SolidBrush(_secHandColor), -4, -4, 8, 8);

            g.Render();
            g.Dispose();
        }
    }
}
