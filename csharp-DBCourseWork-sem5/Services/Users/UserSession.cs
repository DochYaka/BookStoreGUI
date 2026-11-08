using System;
using System.Collections.Generic;
using System.Text;

namespace csharp_DBCourseWork_sem5.Services
{
    public static class UserSession
    {
        public static string _currentLogin { get; set; } = "";
        public static string _currentPassword { get; set; } = "";
        public static string _currentRole { get; set; } = "";
        public static int _currentUserId { get; set; }
    }
}
