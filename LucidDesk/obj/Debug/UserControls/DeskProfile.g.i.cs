﻿#pragma checksum "..\..\..\UserControls\DeskProfile.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "9E402313FD2A17D23E248BCB9896F60FAD5B8D76"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using LucidDesk.UserControls.Common;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace LucidDesk.UserControls {
    
    
    /// <summary>
    /// DeskProfile
    /// </summary>
    public partial class DeskProfile : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 45 "..\..\..\UserControls\DeskProfile.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border MainContainer;
        
        #line default
        #line hidden
        
        
        #line 109 "..\..\..\UserControls\DeskProfile.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal LucidDesk.UserControls.Common.RoundedImage DesktopWallPaper;
        
        #line default
        #line hidden
        
        
        #line 117 "..\..\..\UserControls\DeskProfile.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal LucidDesk.UserControls.Common.RoundedImage DeskUserProfileImage;
        
        #line default
        #line hidden
        
        
        #line 125 "..\..\..\UserControls\DeskProfile.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock DeskUserNameTextBlock;
        
        #line default
        #line hidden
        
        
        #line 126 "..\..\..\UserControls\DeskProfile.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock PCNameTextBlock;
        
        #line default
        #line hidden
        
        
        #line 130 "..\..\..\UserControls\DeskProfile.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox IsFavoriteCheckBox;
        
        #line default
        #line hidden
        
        
        #line 146 "..\..\..\UserControls\DeskProfile.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Path IsFavoriteIcon;
        
        #line default
        #line hidden
        
        
        #line 185 "..\..\..\UserControls\DeskProfile.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock DeskOSNameTextBlock;
        
        #line default
        #line hidden
        
        
        #line 186 "..\..\..\UserControls\DeskProfile.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock DeskIdTextBlock;
        
        #line default
        #line hidden
        
        
        #line 191 "..\..\..\UserControls\DeskProfile.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button MenuButton;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/LucidDesk;component/usercontrols/deskprofile.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\UserControls\DeskProfile.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 7 "..\..\..\UserControls\DeskProfile.xaml"
            ((LucidDesk.UserControls.DeskProfile)(target)).MouseEnter += new System.Windows.Input.MouseEventHandler(this.ControlMouseEnter);
            
            #line default
            #line hidden
            
            #line 7 "..\..\..\UserControls\DeskProfile.xaml"
            ((LucidDesk.UserControls.DeskProfile)(target)).MouseLeave += new System.Windows.Input.MouseEventHandler(this.ControlMouseLeave);
            
            #line default
            #line hidden
            return;
            case 2:
            this.MainContainer = ((System.Windows.Controls.Border)(target));
            return;
            case 3:
            
            #line 48 "..\..\..\UserControls\DeskProfile.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.ConnectClick);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 58 "..\..\..\UserControls\DeskProfile.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.InviteClick);
            
            #line default
            #line hidden
            return;
            case 5:
            
            #line 68 "..\..\..\UserControls\DeskProfile.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.IsFavoriteClick);
            
            #line default
            #line hidden
            return;
            case 6:
            this.DesktopWallPaper = ((LucidDesk.UserControls.Common.RoundedImage)(target));
            return;
            case 7:
            this.DeskUserProfileImage = ((LucidDesk.UserControls.Common.RoundedImage)(target));
            return;
            case 8:
            this.DeskUserNameTextBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 9:
            this.PCNameTextBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 10:
            this.IsFavoriteCheckBox = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 11:
            this.IsFavoriteIcon = ((System.Windows.Shapes.Path)(target));
            
            #line 146 "..\..\..\UserControls\DeskProfile.xaml"
            this.IsFavoriteIcon.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.IsFavoriteIconMouseDown);
            
            #line default
            #line hidden
            return;
            case 12:
            this.DeskOSNameTextBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 13:
            this.DeskIdTextBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 14:
            this.MenuButton = ((System.Windows.Controls.Button)(target));
            
            #line 191 "..\..\..\UserControls\DeskProfile.xaml"
            this.MenuButton.Click += new System.Windows.RoutedEventHandler(this.MenuButtonClick);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

