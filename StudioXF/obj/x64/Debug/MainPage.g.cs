﻿#pragma checksum "E:\Users\Paul\Documents\src\StudioXF\StudioXF\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "8AED74C27FE519E29B281422F87F0977"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace StudioXF
{
    partial class MainPage : 
        global::Windows.UI.Xaml.Controls.Page, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                {
                    global::Windows.UI.Xaml.Controls.Grid element1 = (global::Windows.UI.Xaml.Controls.Grid)(target);
                    #line 11 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Grid)element1).Loaded += this.Grid_Loaded;
                    #line default
                }
                break;
            case 2:
                {
                    this.FTPServer = (global::Windows.UI.Xaml.Controls.ComboBox)(target);
                    #line 12 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.ComboBox)this.FTPServer).SelectionChanged += this.FTPServer_SelectionChanged;
                    #line default
                }
                break;
            case 3:
                {
                    this.ServerFileList = (global::Windows.UI.Xaml.Controls.ListBox)(target);
                    #line 13 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.ListBox)this.ServerFileList).DoubleTapped += this.ServerFileList_DoubleTapped;
                    #line default
                }
                break;
            case 4:
                {
                    this.ServerPath = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                    #line 14 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.TextBox)this.ServerPath).TextChanged += this.ServerPath_TextChanged;
                    #line default
                }
                break;
            case 5:
                {
                    this.GoUp = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 15 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.GoUp).Click += this.GoUp_Click;
                    #line default
                }
                break;
            case 6:
                {
                    this.LocalFiles = (global::Windows.UI.Xaml.Controls.ListBox)(target);
                    #line 16 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.ListBox)this.LocalFiles).DoubleTapped += this.LocalFiles_DoubleTapped;
                    #line default
                }
                break;
            case 7:
                {
                    this.LocalPath = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                    #line 17 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.TextBox)this.LocalPath).TextChanged += this.LocalPath_TextChanged;
                    #line default
                }
                break;
            case 8:
                {
                    this.GoUpLocal = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 18 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.GoUpLocal).Click += this.GoUpLocal_Click;
                    #line default
                }
                break;
            case 9:
                {
                    this.CopyToServer = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 19 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.CopyToServer).Click += this.CopyToServer_Click;
                    #line default
                }
                break;
            case 10:
                {
                    this.CopyLocal = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 20 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.CopyLocal).Click += this.CopyLocal_Click;
                    #line default
                }
                break;
            case 11:
                {
                    this.AddFTPServer = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 21 "..\..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.AddFTPServer).Click += this.AddFTPServer_Click;
                    #line default
                }
                break;
            case 12:
                {
                    this.BusyIndicator = (global::Windows.UI.Xaml.Controls.ProgressRing)(target);
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Windows.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Windows.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}
