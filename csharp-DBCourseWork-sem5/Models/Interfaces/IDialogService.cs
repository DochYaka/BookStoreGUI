using System;
using System.Collections.Generic;
using System.Text;

namespace csharp_DBCourseWork_sem5.Models.Interfaces
{
    public interface IDialogService
    {
        bool Confirm(string text, string title);
        void Info(string text, string title = "Успех");
        void Warning(string text, string title = "Внимание");
        void Error(string text, string title = "Ошибка");
    }
}
