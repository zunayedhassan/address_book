using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace AddressBook
{
    class CustomLabel : Label
    {
        public CustomLabel(string labelText, int y)
        {
            this.Content = labelText;
            this.Margin = new Thickness(20, y, 0, 0);
        }
    }
}
