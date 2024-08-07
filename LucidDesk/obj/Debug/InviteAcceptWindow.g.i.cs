﻿#pragma checksum "..\..\InviteAcceptWindow.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "83D5E015CF118C3D4975B71D8B2E796F0F928753"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using LucidDesk;
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


namespace LucidDesk {
    
    
    /// <summary>
    /// InviteAcceptWindow
    /// </summary>
    public partial class InviteAcceptWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 62 "..\..\InviteAcceptWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal LucidDesk.UserControls.Common.RoundedImage DeskUserProfileImageComponent;
        
        #line default
        #line hidden
        
        
        #line 67 "..\..\InviteAcceptWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock DeskUserNameTextBlock;
        
        #line default
        #line hidden
        
        
        #line 68 "..\..\InviteAcceptWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock DeskIdTextBlock;
        
        #line default
        #line hidden
        
        
        #line 75 "..\..\InviteAcceptWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox AccessTypeCombobox;
        
        #line default
        #line hidden
        
        
        #line 78 "..\..\InviteAcceptWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox KeyboardAccessCheckBox;
        
        #line default
        #line hidden
        
        
        #line 86 "..\..\InviteAcceptWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox MouseAccessCheckBox;
        
        #line default
        #line hidden
        
        
        #line 94 "..\..\InviteAcceptWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox ClipboardAccessCheckBox;
        
        #line default
        #line hidden
        
        
        #line 102 "..\..\InviteAcceptWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox AudioAccessCheckBox;
        
        #line default
        #line hidden
        
        
        #line 113 "..\..\InviteAcceptWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Reject;
        
        #line default
        #line hidden
        
        
        #line 114 "..\..\InviteAcceptWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Accept;
        
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
            System.Uri resourceLocater = new System.Uri("/LucidDesk;component/inviteacceptwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\InviteAcceptWindow.xaml"
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
            
            #line 15 "..\..\InviteAcceptWindow.xaml"
            ((System.Windows.Controls.Grid)(target)).MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.TopPanelMouseDown);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 25 "..\..\InviteAcceptWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.MinimizeButtonClick);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 33 "..\..\InviteAcceptWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.MaximizeButtonClick);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 42 "..\..\InviteAcceptWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.CloseButtonClick);
            
            #line default
            #line hidden
            return;
            case 5:
            this.DeskUserProfileImageComponent = ((LucidDesk.UserControls.Common.RoundedImage)(target));
            return;
            case 6:
            this.DeskUserNameTextBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 7:
            this.DeskIdTextBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 8:
            this.AccessTypeCombobox = ((System.Windows.Controls.ComboBox)(target));
            
            #line 75 "..\..\InviteAcceptWindow.xaml"
            this.AccessTypeCombobox.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.AccessTypeComboboxSelectionChanged);
            
            #line default
            #line hidden
            return;
            case 9:
            this.KeyboardAccessCheckBox = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 10:
            this.MouseAccessCheckBox = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 11:
            this.ClipboardAccessCheckBox = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 12:
            this.AudioAccessCheckBox = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 13:
            this.Reject = ((System.Windows.Controls.Button)(target));
            
            #line 113 "..\..\InviteAcceptWindow.xaml"
            this.Reject.Click += new System.Windows.RoutedEventHandler(this.RejectButtonClick);
            
            #line default
            #line hidden
            return;
            case 14:
            this.Accept = ((System.Windows.Controls.Button)(target));
            
            #line 114 "..\..\InviteAcceptWindow.xaml"
            this.Accept.Click += new System.Windows.RoutedEventHandler(this.AcceptClick);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

