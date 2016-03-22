using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace AddressBook
{
    class ToolBarButton : Button
    {
        public ToolBarButton(CustomImage icon, string text)
        {
            this.MinWidth = 55;

            DockPanel.SetDock(icon, Dock.Top);

            TextBlock toolBarButtonTextBlock = new TextBlock();
            toolBarButtonTextBlock.Text = text;
            toolBarButtonTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
            DockPanel.SetDock(toolBarButtonTextBlock, Dock.Bottom);

            DockPanel toolBarButtonDockPanel = new DockPanel();

            this.Content = toolBarButtonDockPanel;
            toolBarButtonDockPanel.Children.Add(icon);
            toolBarButtonDockPanel.Children.Add(toolBarButtonTextBlock);
        }
    }
}
