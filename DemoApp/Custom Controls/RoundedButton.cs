﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace DemoApp.Custom_Controls
{
    public class RoundedButton : Button
    {
        // Fields
        private int borderSize = 0;
        private int borderRadius = 20;
        private static Color standardColor = ColorTranslator.FromHtml("#479B74");
        private Color borderColor = standardColor;

        // Constructor
        public RoundedButton()
        {
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
            this.Size = new Size(150, 40);
            this.BackColor = standardColor;
            this.ForeColor = Color.White;
            this.DoubleBuffered = true;
        }

        /// <summary>
        /// Creates the shape with a given rectangle and radius.
        /// </summary>
        private GraphicsPath GetPath(RectangleF rect, float radius)
        {
            // Create path
            GraphicsPath path = new GraphicsPath();
            path.StartFigure();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Width - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Width - radius, rect.Height - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Height - radius, radius, radius, 90, 90);
            path.CloseFigure();

            return path;
        }

        /// <summary>
        /// Overrides the OnPaint event and creates the shape of the button
        /// </summary>
        protected override void OnPaint(PaintEventArgs pevent)
        {
            // Set mode to anti-alias
            base.OnPaint(pevent);
            pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            RectangleF rectangleSurface = new RectangleF(0, 0, this.Width, this.Height);
            RectangleF rectangleBorder = new RectangleF(1, 1, this.Width - 0.8F, this.Height - 1);

            if (borderRadius > 2) // Rounded button
            {
                using (GraphicsPath pathSurface = GetPath(rectangleSurface, borderRadius))
                {
                    using (GraphicsPath pathBorder = GetPath(rectangleBorder, borderRadius - 1F))
                    {
                        using (Pen penSurface = new Pen(this.Parent.BackColor, 2))
                        {
                            using (Pen penBorder = new Pen(borderColor, borderSize))
                            {
                                penBorder.Alignment = PenAlignment.Inset;

                                // Button surface
                                this.Region = new Region(pathSurface);

                                // Draw surface border
                                pevent.Graphics.DrawPath(penSurface, pathSurface);

                                // Button border
                                if (borderSize >= 1)
                                {
                                    // Draw control border
                                    pevent.Graphics.DrawPath(penBorder, pathBorder);
                                }
                            }
                        }
                    }
                }
            }
            else // Normal button
            {
                this. Region = new Region(rectangleSurface);
                if (borderSize >= 1)
                {
                    using (Pen penBorder = new Pen(borderColor, borderSize))
                    {
                        penBorder.Alignment = PenAlignment.Inset;
                        pevent.Graphics.DrawRectangle(penBorder, 0, 0, this.Width - 1, this.Height - 1);
                    }
                }
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            this.Parent.BackColorChanged += new EventHandler(Container_BackColorChanged);

        }

        private void Container_BackColorChanged(object sender, EventArgs e)
        {
            if (this.DesignMode)
            {
                this.Invalidate();
            }
        }
    }
}
