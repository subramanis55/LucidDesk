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

namespace LucidDesk.UserControls
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


        public double ZoomFacter
        {
            get { return (double)GetValue(ZoomFacterProperty); }
            set { SetValue(ZoomFacterProperty, value); }
        }

        public static readonly DependencyProperty ZoomFacterProperty =
            DependencyProperty.Register("ZoomFacter", typeof(double), typeof(RoundedImage), new PropertyMetadata(0d));


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


    }
}
