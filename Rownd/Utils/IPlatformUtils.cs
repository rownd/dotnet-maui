using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace Rownd.Maui.Utils
{
    public interface IPlatformUtils
    {
        public Thickness GetWindowSafeArea();
        public double GetWindowHeight();
    }
}
