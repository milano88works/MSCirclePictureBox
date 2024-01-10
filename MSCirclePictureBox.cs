using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace milano88.UI.Controls
{
    public class MSCirclePictureBox : PictureBox
    {
        [Description("Occurs when the scroll percentage is changed")]
        public event EventHandler RotationChanged;

        private BufferedGraphics _bufGraphics;
        private Bitmap _tempBitmap;

        public MSCirclePictureBox()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            Size = new Size(110, 110);
            DoubleBuffered = true;
            UpdateGraphicsBuffer();
        }

        private void UpdateGraphicsBuffer()
        {
            if (Width > 0 && Height > 0)
            {
                BufferedGraphicsContext context = BufferedGraphicsManager.Current;
                context.MaximumBuffer = new Size(Width + 1, Height + 1);
                _bufGraphics = context.Allocate(CreateGraphics(), ClientRectangle);
            }
        }

        private int _borderSize;
        [Category("Custom Properties")]
        [DefaultValue(0)]
        public int BorderSize
        {
            get { return _borderSize; }
            set
            {
                _borderSize = value;
                Invalidate();
            }
        }

        private Color _borderColor = Color.DodgerBlue;
        [Category("Custom Properties")]
        [DefaultValue(typeof(Color), "DodgerBlue")]
        public Color BorderColor
        {
            get => _borderColor;
            set { _borderColor = value; Invalidate(); }
        }

        private Color _borderColor2 = Color.LightSteelBlue;
        [Category("Custom Properties")]
        [DefaultValue(typeof(Color), "LightSteelBlue")]
        public Color BorderColor2
        {
            get => _borderColor2;
            set { _borderColor2 = value; Invalidate(); }
        }

        private DashCap _borderCapStyle = DashCap.Flat;
        [Category("Custom Properties")]
        [DefaultValue(DashCap.Flat)]
        public DashCap BorderCapStyle
        {
            get => _borderCapStyle;
            set { _borderCapStyle = value; Invalidate(); }
        }

        private DashStyle _borderLineStyle = DashStyle.Solid;
        [Category("Custom Properties")]
        [DefaultValue(DashStyle.Solid)]
        public DashStyle BorderLineStyle
        {
            get => _borderLineStyle;
            set { _borderLineStyle = value; Invalidate(); }
        }

        private float _borderGradientAngle = 360F;
        [Category("Custom Properties")]
        [DefaultValue(360F)]
        public float BorderGradientAngle
        {
            get => _borderGradientAngle;
            set { _borderGradientAngle = value; Invalidate(); }
        }

        [Category("Custom Properties")]
        [DefaultValue(null)]
        public new Image Image
        {
            get => base.Image;
            set
            {
                base.Image = ResizeImage(value, value.Size);
                if (value == null)
                {
                    _tempBitmap.Dispose();
                    _tempBitmap = null;
                }
                else _tempBitmap = new Bitmap(ResizeImage(value, value.Size));
            }
        }

        private int _imageRotate = 0;
        [Category("Custom Properties")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ImageRotate
        {
            get => _imageRotate;
            set
            {
                value = value < 0 ? 0 : value > 360 ? 360 : value;
                _imageRotate = value;
                RotateImage(this, _tempBitmap, _imageRotate);
                RotationChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private bool _imageFilter = false;
        [Category("Custom Properties")]
        [DefaultValue(typeof(bool), "False")]
        public bool ImageFilter
        {
            get => _imageFilter;
            set { _imageFilter = value; Invalidate(); }
        }

        private Color _filterColor = Color.DodgerBlue;
        [Category("Custom Properties")]
        [DefaultValue(typeof(Color), "DodgerBlue")]
        public Color FiterColor1
        {
            get => _filterColor;
            set { _filterColor = value; Invalidate(); }
        }

        private Color _filterColor2 = Color.White;
        [Category("Custom Properties")]
        [DefaultValue(typeof(Color), "White")]
        public Color FiterColor2
        {
            get => _filterColor2;
            set { _filterColor2 = value; Invalidate(); }
        }

        private int _filterAlpha = 100;
        [Category("Custom Properties")]
        [DefaultValue(100)]
        public int FilterTransparency
        {
            get => _filterAlpha;
            set { _filterAlpha = value; Invalidate(); }
        }

        private float _filterAngle = 180F;
        [Category("Custom Properties")]
        [DefaultValue(180F)]
        public float FilterAngle
        {
            get => _filterAngle;
            set { _filterAngle = value; Invalidate(); }
        }

        [Category("Custom Properties")]
        [DefaultValue(PictureBoxSizeMode.Normal)]
        public new PictureBoxSizeMode SizeMode
        {
            get => base.SizeMode;
            set => base.SizeMode = value;
        }

        [Browsable(false)]
        public new BorderStyle BorderStyle
        {
            get => base.BorderStyle;
        }

        private Image ResizeImage(Image image, Size newSize)
        {
            Bitmap newImage = new Bitmap(newSize.Width, newSize.Height);
            using (Graphics graphics = Graphics.FromImage(newImage))
            {
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.DrawImage(image, new Rectangle(0, 0, newSize.Width, newSize.Height));
            }
            return newImage;
        }

        private void RotateImage(PictureBox pic, Image image, float angle)
        {
            if (image == null || pic.Image == null) return;
            Image oldImage = pic.Image;
            pic.Image = RotateImage(image, angle);
            if (oldImage != null) oldImage.Dispose();
        }

        private Image RotateImage(Image image, float angle)
        {
            Bitmap bmp = new Bitmap(image.Width, image.Height);
            using (Graphics graphics = Graphics.FromImage(bmp))
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.TranslateTransform((float)bmp.Width / 2, (float)bmp.Height / 2);
                graphics.RotateTransform(angle);
                graphics.TranslateTransform(-(float)bmp.Width / 2, -(float)bmp.Height / 2);
                graphics.DrawImage(image, 0, 0);
            }
            return bmp;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Size = new Size(Width, Width);
            UpdateGraphicsBuffer();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            if (base.Image != null)
            {
                _bufGraphics.Graphics.Clear(BackColor);
                _bufGraphics.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                Rectangle rectSmooth = Rectangle.Inflate(ClientRectangle, -1, -1);
                Rectangle rectBorder = Rectangle.Inflate(rectSmooth, -_borderSize, -_borderSize);
                Rectangle rectGradient = new Rectangle(Point.Empty, Size);
                using (Bitmap bmp = new Bitmap(base.Image, Width, Height))
                using (TextureBrush textureBrush = new TextureBrush(bmp))
                using (LinearGradientBrush borderGColor = new LinearGradientBrush(rectGradient, _borderColor, _borderColor2, _borderGradientAngle))
                using (GraphicsPath graphicsPath = new GraphicsPath())
                using (Pen penSmooth = new Pen(Parent.BackColor, _borderSize * 2))
                using (Pen penBorder = new Pen(borderGColor, _borderSize))
                {
                    graphicsPath.AddEllipse(rectSmooth);
                    _bufGraphics.Graphics.FillPath(textureBrush, graphicsPath);
                    _bufGraphics.Graphics.DrawEllipse(penSmooth, rectSmooth);
                    Region = new Region(graphicsPath);

                    if (_imageFilter)
                    {
                        using (LinearGradientBrush brushFilter = new LinearGradientBrush(rectGradient, Color.FromArgb(_filterAlpha, _filterColor), Color.FromArgb(_filterAlpha, _filterColor2), _filterAngle))
                            _bufGraphics.Graphics.FillEllipse(brushFilter, rectBorder);
                    }

                    if (_borderSize > 0)
                    {
                        penBorder.DashStyle = _borderLineStyle;
                        penBorder.DashCap = _borderCapStyle;
                        _bufGraphics.Graphics.DrawEllipse(penBorder, rectBorder);
                    }
                }
            }
            else
            {
                _bufGraphics.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                _bufGraphics.Graphics.Clear(BackColor);
                Rectangle rectSmooth = Rectangle.Inflate(ClientRectangle, -1, -1);
                using (SolidBrush brushEllipse = new SolidBrush(Color.White))
                using (GraphicsPath graphicsPath = new GraphicsPath())
                using (Pen penSmooth = new Pen(Parent.BackColor, 1))
                {
                    graphicsPath.AddEllipse(rectSmooth);
                    _bufGraphics.Graphics.FillEllipse(brushEllipse, ClientRectangle);
                    _bufGraphics.Graphics.DrawEllipse(penSmooth, rectSmooth);
                    Region = new Region(graphicsPath);
                }
            }

            _bufGraphics.Render(pe.Graphics);
        }
    }
}
