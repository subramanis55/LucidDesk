﻿#pragma checksum "..\..\..\..\UserControls\Common\ProfileImageSelect.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "A6D02EB4A3B9C4F7027C6A8398A1B73E319D51F7"
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


namespace LucidDesk.UserControls.Common {
    
    
    /// <summary>
    /// ProfileImageSelect
    /// </summary>
    public partial class ProfileImageSelect : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 13 "..\..\..\..\UserControls\Common\ProfileImageSelect.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border WhiteBackground;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\..\..\UserControls\Common\ProfileImageSelect.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal LucidDesk.UserControls.Common.RoundedImage ProfileImage;
        
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
            System.Uri resourceLocater = new System.Uri("/LucidDesk;component/usercontrols/common/profileimageselect.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\UserControls\Common\ProfileImageSelect.xaml"
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
            
            #line 12 "..\..\..\..\UserControls\Common\ProfileImageSelect.xaml"
            ((System.Windows.Controls.Grid)(target)).SizeChanged += new System.Windows.SizeChangedEventHandler(this.GridSizeChanged);
            
            #line default
            #line hidden
            return;
            case 2:
            this.WhiteBackground = ((System.Windows.Controls.Border)(target));
            
            #line 13 "..\..\..\..\UserControls\Common\ProfileImageSelect.xaml"
            this.WhiteBackground.MouseEnter += new System.Windows.Input.MouseEventHandler(this.ProfileMouseEnter);
            
            #line default
            #line hidden
            
            #line 13 "..\..\..\..\UserControls\Common\ProfileImageSelect.xaml"
            this.WhiteBackground.MouseLeave += new System.Windows.Input.MouseEventHandler(this.ProfileMouseLeave);
            
            #line default
            #line hidden
            
            #line 13 "..\..\..\..\UserControls\Common\ProfileImageSelect.xaml"
            this.WhiteBackground.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.BackgroundMouseDown);
            
            #line default
            #line hidden
            return;
            case 3:
            this.ProfileImage = ((LucidDesk.UserControls.Common.RoundedImage)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

