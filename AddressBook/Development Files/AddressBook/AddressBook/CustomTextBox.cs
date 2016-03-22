using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace AddressBook
{
    class CustomTextBox : TextBox
    {
        public CustomTextBox(int y)
        {
            this.Width = 130;
            this.Margin = new Thickness(140, y, 0, 0);
        }
    }
}
