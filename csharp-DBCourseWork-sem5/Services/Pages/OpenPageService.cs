using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace csharp_DBCourseWork_sem5.Services.Pages
{
    public static class OpenPageService
    {
        public static void OpenPage(string name, Frame frame)
        {
            Page? page = PageFactory.Create(name, frame);
            if (page == null) return;

            if (frame.Content?.GetType() == page.GetType()) return;

            frame.Navigate(page);
        }
    }
}
