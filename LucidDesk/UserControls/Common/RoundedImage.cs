using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LucidDesk.UserControls.Common
{

    public class RoundedImage : Control
    {
        static RoundedImage()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RoundedImage), new FrameworkPropertyMetadata(typeof(RoundedImage)));


        }
        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(ImageSource), typeof(RoundedImage), new PropertyMetadata(null));

        public Stretch Stretch
        {
            get { return (Stretch)GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }
        public static readonly DependencyProperty StretchProperty =
            DependencyProperty.Register("Stretch", typeof(Stretch), typeof(RoundedImage), new PropertyMetadata(Stretch.Uniform));

        public CornerRadius CornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(CornerRadiusProperty);
            }
            set
            {
                SetValue(CornerRadiusProperty, value);
            }
        }
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(RoundedImage), new PropertyMetadata(new CornerRadius(3)));

        public double ZoomFactor
        {
            get => (double)GetValue(ZoomFactorProperty);
            set => SetValue(ZoomFactorProperty, value);
        }

        public static readonly DependencyProperty ZoomFactorProperty =
            DependencyProperty.Register("ZoomFactor", typeof(double), typeof(RoundedImage), new PropertyMetadata(0d));


        public double Xtranslate
        {
            get { return (double)GetValue(XtranslateProperty); }
            set { SetValue(XtranslateProperty, value); }
        }

        public static readonly DependencyProperty XtranslateProperty =
            DependencyProperty.Register("Xtranslate", typeof(double), typeof(RoundedImage), new PropertyMetadata(0d));

        public double Ytranslate
        {
            get { return (double)GetValue(YtranslateProperty); }
            set { SetValue(YtranslateProperty, value); }
        }

        public static readonly DependencyProperty YtranslateProperty =
            DependencyProperty.Register("Ytranslate", typeof(double), typeof(RoundedImage), new PropertyMetadata(0d));

        public double BorderWidth
        {
            get { return (double)GetValue(BorderWidthProperty); }
            set { SetValue(BorderWidthProperty, value); }
        }

        public static readonly DependencyProperty BorderWidthProperty =
            DependencyProperty.Register("BorderWidth", typeof(double), typeof(RoundedImage), new PropertyMetadata(0d));

        public Thickness BorderThickness
        {
            get
            {
                return (Thickness)GetValue(BorderThicknessProperty);
            }
            set
            {
                SetValue(BorderThicknessProperty, value);
            }
        }
        public static readonly DependencyProperty BorderThicknessProperty = DependencyProperty.Register("BorderThickness", typeof(Thickness), typeof(RoundedImage), new PropertyMetadata(new Thickness(0)));

        public Brush BorderBrush
        {
            set
            {
                SetValue(BorderBrushProperty, value);
            }
            get
            {
               return (Brush)GetValue(BorderBrushProperty);
            }
        }
        public static readonly DependencyProperty BorderBrushProperty = DependencyProperty.Register("BorderBrush", typeof(SolidColorBrush), typeof(RoundedImage), new PropertyMetadata(Brushes.DodgerBlue));
    }
}
