using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhysicsTest
{
    public partial class Form1 : Form
    {
        Scene scene = new Scene()
        {
            Objects = new List<SceneObject>()
            {
                new SceneObject()
                {
                    Position = new Point2() { X = 100f, Y = 100f },
                    DragCoefficient = 0.5f,
                    StopSpeed = 5f
                }
            }
        };
        Timer timer = new Timer() { Interval = 200 };

        public Form1()
        {
            InitializeComponent();
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            scene.Tick(0.2f);
            Invalidate();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.FillRectangle(Brushes.Black, this.ClientRectangle);
            foreach (var region in scene.Regions)
            {
                var circle = region.Region as Circle2;
                if (circle != null)
                {
                    g.DrawEllipse(Pens.Red, circle.Center.X - circle.Radius, circle.Center.Y - circle.Radius, 2f * circle.Radius, 2f * circle.Radius);
                }
                var annulus = region.Region as Annulus2;
                if (annulus != null)
                {
                    g.DrawEllipse(Pens.Red, annulus.Center.X - annulus.InnerRadius, annulus.Center.Y - annulus.InnerRadius, 2f * annulus.InnerRadius, 2f * annulus.InnerRadius);
                    g.DrawEllipse(Pens.Red, annulus.Center.X - annulus.OuterRadius, annulus.Center.Y - annulus.OuterRadius, 2f * annulus.OuterRadius, 2f * annulus.OuterRadius);
                    g.DrawLine(Pens.Red, annulus.Center.X, annulus.Center.Y, annulus.Center.X + annulus.OuterRadius * (float)Math.Cos(annulus.MinArc), annulus.Center.Y + annulus.OuterRadius * (float)Math.Sin(annulus.MinArc));
                    g.DrawLine(Pens.Red, annulus.Center.X, annulus.Center.Y, annulus.Center.X + annulus.OuterRadius * (float)Math.Cos(annulus.MaxArc), annulus.Center.Y + annulus.OuterRadius * (float)Math.Sin(annulus.MaxArc));
                }
                var poly = region.Region as Polygon2;
                if (poly != null)
                {
                    g.DrawPolygon(Pens.Red, poly.Points.Select(p => new PointF(p.X, p.Y)).ToArray());
                }
            }
            foreach (var obj in scene.Objects)
            {
                g.FillRectangle(Brushes.Blue, obj.Position.X - 1f, obj.Position.Y - 1f, 3f, 3f);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer.Enabled = true;
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            var newRegion = new RadialWaveForceRegion()
            {
                Center = new Point2() { X = e.X, Y = e.Y },
                AnnularRegion = new Annulus2()
                {
                    Center = new Point2() { X = e.X, Y = e.Y },
                    InnerRadius = 0f,
                    OuterRadius = 40f,
                    MinArc = 0f,
                    MaxArc = (float)(Math.PI / 2)
                },
                Speed = 10f,
                UnitAmplitude = 1f
            };
            scene.Regions.Add(newRegion);
        }
    }
}
