using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ClientApplication
{
    internal enum HorizontalAlignment
    {
        HorizontalLeft,
        HorizontalRight
    }

    internal class ChatBox
    {
        #region private properties
        HorizontalAlignment _alignment;
        string _content;
        DateTime _date;
        #endregion

        internal TextBlock txtBlock;

        

        internal ChatBox(string message, DateTime dateTime,
            HorizontalAlignment alignment = HorizontalAlignment.HorizontalRight)
        {
            _content = message;
            _date = dateTime;
            _alignment = alignment;

            txtBlock = new TextBlock();
            InitTextBlock();
        }

        private void InitTextBlock()
        {
            txtBlock.TextWrapping = TextWrapping.Wrap;
            switch (_alignment)
            {
                    // message from other clients
                case HorizontalAlignment.HorizontalLeft:
                    txtBlock.Background = Brushes.AliceBlue;
                    txtBlock.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    break;

                    //message from self
                case HorizontalAlignment.HorizontalRight:
                    txtBlock.Background = Brushes.Aquamarine;
                    txtBlock.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                    break;
            }

            StringBuilder strings = new StringBuilder();
            strings.Append(_date.ToString("HH:mm:ss"));
            strings.Append("\n");
            strings.Append(_content);
            strings.Append("\n");
            txtBlock.Text = strings.ToString();
        }
    }
}
