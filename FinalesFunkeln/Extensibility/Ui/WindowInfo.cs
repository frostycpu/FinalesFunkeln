using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Controls;
using FinalesFunkeln.Controls;
using Xceed.Wpf.AvalonDock.Controls;
using Xceed.Wpf.AvalonDock.Layout;

namespace FinalesFunkeln.Extensibility.Ui
{
    [Serializable]
    class WindowInfo
    {
        public const uint CurrentVersion = 0;
        public uint Version { get; set; }
        public double WindowX { get; set; }
        public double WindowY { get; set; }
        public double WindowWidth { get; set; }
        public double WindowHeight { get; set; }
        public bool Maximized { get; set; }

        public LayoutRootInfo Layout { get; set; }
        public List<FloatingWindowInfo> FloatingWindows { get; set; }

        public void Convert(MainWindow window, UiManager uiManager)
        {
            window.Left = WindowX;
            window.Top = WindowY;
            window.Width = WindowWidth;
            window.Height = WindowHeight;

            window.WindowState = Maximized ? WindowState.Maximized : WindowState.Normal;
            
            window.DockingManager.Layout = (LayoutRoot)Layout.Convert(uiManager);
            foreach (var x in FloatingWindows)
            {
                var fwin = (LayoutFloatingWindow) x.Convert(uiManager);
                if(fwin!=null)
                    window.DockingManager.Layout.FloatingWindows.Add(fwin);
            }
        }

        public void Convert(MainWindow window)
        {
            WindowX = window.Left;
            WindowY = window.Top;
            WindowWidth = window.Width;
            WindowHeight = window.Height;
            Maximized = window.WindowState == WindowState.Maximized;
            Layout = (LayoutRootInfo)ConvertLayoutElement(window.DockingManager.Layout);
            FloatingWindows= ConvertFloatingWindows(window.DockingManager.Layout.FloatingWindows);
        }

        List<FloatingWindowInfo> ConvertFloatingWindows(ObservableCollection<LayoutFloatingWindow> windows)
        {
            var list = new List<FloatingWindowInfo>();
            foreach (var x in windows)
            {
                var x2 = x as LayoutDocumentFloatingWindow;
                if (x2 == null)
                    throw new NotSupportedException("Anchorables are not supported yet");
                var winInfo = new FloatingWindowInfo
                {
                    X = x2.RootDocument.FloatingLeft,
                    Y = x2.RootDocument.FloatingTop,
                    Width = x2.RootDocument.FloatingWidth,
                    Height = x2.RootDocument.FloatingHeight,
                    IsMaximized = x2.RootDocument.IsMaximized,
                    View = ConvertView(x2.RootDocument)
                };
                list.Add(winInfo);
            }
            return list;
        }


        BaseInfo ConvertLayoutElement(ILayoutElement e)
        {
            if (e is LayoutPanel)
            {
                var panel = e as LayoutPanel;
                var sp = new LayoutPanelInfo
                {
                    Width = new GridLengthInfo {Unit = panel.DockWidth.GridUnitType, Value = panel.DockWidth.Value},
                    Height = new GridLengthInfo {Unit = panel.DockHeight.GridUnitType, Value = panel.DockHeight.Value}
                };
                foreach (var x in panel.Children)
                {
                    sp.Children.Add(ConvertLayoutElement(x));
                }
                return sp;
            }
            else if (e is LayoutRoot)
            {
                var root = e as LayoutRoot;
                var sp = new LayoutRootInfo {Center = (LayoutPanelInfo) ConvertLayoutElement(root.RootPanel)};
                return sp;
            }
            else if (e is LayoutDocumentPane)
            {
                var doc = e as LayoutDocumentPane;
                var sp = new DocumentPaneInfo
                {
                    Width = new GridLengthInfo {Unit = doc.DockWidth.GridUnitType, Value = doc.DockWidth.Value},
                    Height = new GridLengthInfo {Unit = doc.DockHeight.GridUnitType, Value = doc.DockHeight.Value}
                };
                sp.Content.AddRange(ConvertViews(doc.Children));
                return sp;
            }
            else if (e is LayoutDocumentPaneGroup)
            {
                var doc = e as LayoutDocumentPaneGroup;
                var sp = new LayoutDocumentPaneGroupInfo
                {
                    Width = new GridLengthInfo {Unit = doc.DockWidth.GridUnitType, Value = doc.DockWidth.Value},
                    Height = new GridLengthInfo {Unit = doc.DockHeight.GridUnitType, Value = doc.DockHeight.Value},
                    Orientation = doc.Orientation
                };
                foreach (var x in doc.Children)
                {
                    sp.Children.Add(ConvertLayoutElement(x));
                }
                return sp;
            }
            else
                throw new NotSupportedException();
        }

        List<ViewInfo> ConvertViews(IList<LayoutContent> children)
        {
            List<ViewInfo> l = new List<ViewInfo>(children.Count);
            l.AddRange(children.Select(ConvertView));
            return l;
        }

        ViewInfo ConvertView(LayoutContent child)
        {
            var v = new ViewInfo();
            if (child is DocumentViewControl)
            {
                v.InternalName = (child as DocumentViewControl).View.InternalName;
            }
            else
                throw new NotSupportedException();
            return v;
        }
    }

    [Serializable]
    abstract class BaseInfo
    {
        abstract internal LayoutElement Convert(UiManager uiManager);
    }

    [Serializable]
    abstract class PaneInfo : BaseInfo
    {
        public GridLengthInfo Height { get; set; }
        public GridLengthInfo Width { get; set; }
        public List<ViewInfo> Content { get; set; }

        public PaneInfo()
        {
            Content = new List<ViewInfo>();
        }
    }

    class FloatingWindowInfo : BaseInfo
    {
        public double X { get; set; }
        public double Y { get; set; }

        public double Width { get; set; }
        public double Height { get; set; }

        public bool IsMaximized { get; set; }

        public ViewInfo View { get; set; }
    
        internal override LayoutElement Convert(UiManager uiManager)
        {
            var win=new LayoutDocumentFloatingWindow();
            var view = View.Convert(uiManager);
            if (view != null)
                win.RootDocument = new DocumentViewControl(view);
            else return null;
            win.RootDocument.FloatingLeft = X;
            win.RootDocument.FloatingTop = Y;
            win.RootDocument.FloatingWidth = Width;
            win.RootDocument.FloatingHeight = Height;
            win.RootDocument.IsMaximized = IsMaximized;
            return win;
        }
    }


    [Serializable]
    class DocumentPaneInfo : PaneInfo 
    {
        internal override LayoutElement Convert(UiManager uiManager)
        {
            var doc = new LayoutDocumentPane
            {
                DockWidth = new GridLength(Width.Value, Width.Unit),
                DockHeight = new GridLength(Height.Value, Height.Unit)
            };
            foreach (var x in Content)
            {
                var v = x.Convert(uiManager);
                if(v!=null)
                    doc.Children.Add(new DocumentViewControl(v));
            }
            return doc;
        }
    }

    [Serializable]
    abstract class LayoutInfo : BaseInfo
    {
    }

    [Serializable]
    class LayoutRootInfo : LayoutInfo
    {
        public LayoutPanelInfo Center { get; set; }
        internal override LayoutElement Convert(UiManager uiManager)
        {
            var lr = new LayoutRoot {RootPanel = (LayoutPanel) Center.Convert(uiManager)};
            return lr;
        }
    }

    [Serializable]
    class LayoutPanelInfo : LayoutInfo
    {
        public GridLengthInfo Height { get; set; }
        public GridLengthInfo Width { get; set; }
        public Orientation Orientation { get; set; }
        public List<BaseInfo> Children { get; set; }

        public LayoutPanelInfo()
        {
            Children = new List<BaseInfo>();
        }

        internal override LayoutElement Convert(UiManager uiManager)
        {
            var lr = new LayoutPanel
            {
                DockWidth = new GridLength(Width.Value, Width.Unit),
                DockHeight = new GridLength(Height.Value, Height.Unit),
                Orientation = Orientation
            };
            foreach (var x in Children)
            {
                if (x is LayoutInfo)
                    lr.Children.Add((ILayoutPanelElement)((LayoutInfo)x).Convert(uiManager));
                else if (x is PaneInfo)
                    lr.Children.Add((ILayoutPanelElement)((PaneInfo)x).Convert(uiManager));
            }
            return lr;
        }
    }

    [Serializable]
    abstract class LayoutPaneGroupInfo : LayoutPanelInfo
    {
    }

    [Serializable]
    class LayoutDocumentPaneGroupInfo : LayoutPaneGroupInfo
    {
        internal override LayoutElement Convert(UiManager uiManager)
        {
            var lr = new LayoutDocumentPaneGroup
            {
                DockWidth = new GridLength(Width.Value, Width.Unit),
                DockHeight = new GridLength(Height.Value, Height.Unit),
                Orientation = Orientation
            };
            foreach (var x in Children)
            {
                if (x is LayoutInfo)
                    lr.Children.Add((ILayoutDocumentPane)((LayoutInfo)x).Convert(uiManager));
                else if (x is PaneInfo)
                    lr.Children.Add((ILayoutDocumentPane)((PaneInfo)x).Convert(uiManager));
                else
                    throw new Exception();
                
            }
            return lr;
        }
    }

    class GridLengthInfo
    {
        public GridUnitType Unit { get; set; }
        public double Value { get; set; }
    
    }

    [Serializable]
    class ViewInfo
    {
        public string InternalName { get; set; }

        public View Convert(UiManager uiManager)
        {
            return uiManager.HasView(InternalName) ? uiManager.GetView(InternalName) : null;
        }
    }

}