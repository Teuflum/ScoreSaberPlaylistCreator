using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ScoreSaberPlaylistCreator.Classes
{
    public static class ExtensionMethods
    {
        public static void AppendTextExt(this TextBox txt, string textData)
        {
            if (txt.Text.Length == 0)
                txt.AppendText(textData);
            else
                txt.AppendText(Environment.NewLine + textData);
            txt.ScrollToEnd();
        }
    }
}
