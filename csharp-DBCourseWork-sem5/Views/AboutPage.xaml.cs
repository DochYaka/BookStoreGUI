using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace csharp_DBCourseWork_sem5
{
    public partial class AboutPage : Page
    {
        public AboutPage()
        {
            InitializeComponent();
            ShowAboutProgram();
        }

        private void ShowAboutProgram()
        {
            var doc = new FlowDocument();
            doc.FontFamily = new System.Windows.Media.FontFamily("Segoe UI");
            doc.FontSize = 14;
            doc.Foreground = System.Windows.Media.Brushes.Black;
            doc.PagePadding = new Thickness(20);

            Paragraph Header(string text) => new Paragraph(new Run(text))
            {
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Foreground = System.Windows.Media.Brushes.LightBlue,
                Margin = new Thickness(0, 15, 0, 5)
            };

            Paragraph BoldText(string text) => new Paragraph(new Run(text))
            {
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 5, 0, 5)
            };

            Paragraph NormalText(string text) => new Paragraph(new Run(text))
            {
                Margin = new Thickness(0, 0, 0, 10)
            };

            Paragraph ListItem(string text) => new Paragraph(new Run("• " + text))
            {
                Margin = new Thickness(20, 0, 0, 5)
            };

            doc.Blocks.Add(Header("О программе"));

            doc.Blocks.Add(BoldText("Название:"));
            doc.Blocks.Add(NormalText("Информационная система книжного магазина (клиентское приложение)"));

            doc.Blocks.Add(BoldText("Версия:"));
            doc.Blocks.Add(NormalText("1.0"));

            doc.Blocks.Add(BoldText("Разработчик:"));
            doc.Blocks.Add(NormalText("Киселев Д. С., DeepSeek, Claude"));

            doc.Blocks.Add(BoldText("Год разработки:"));
            doc.Blocks.Add(NormalText("2024"));

            doc.Blocks.Add(NormalText("Программа предназначена для работы сотрудников книжного магазина с базой данных: учёт книг и их экземпляров, поставщиков и поставок, заказов физических и юридических лиц, а также кадровый учёт. Каждый сотрудник видит и изменяет только те данные, которые доступны его роли."));

            doc.Blocks.Add(Header("Возможности"));
            doc.Blocks.Add(ListItem("вход в систему по логину и паролю с учётом роли пользователя;"));
            doc.Blocks.Add(ListItem("просмотр таблиц базы данных, доступных роли, и поиск по ним;"));
            doc.Blocks.Add(ListItem("добавление, изменение и удаление записей с проверкой вводимых значений и подтверждением удаления;"));
            doc.Blocks.Add(ListItem("выполнение собственных SQL-запросов с выводом результата и сообщений об ошибках;"));
            doc.Blocks.Add(ListItem("экспорт данных таблицы в файл Excel;"));
            doc.Blocks.Add(ListItem("аналитика продаж: выручка, число заказов, средний чек, прибыль, выручка по месяцам, топ книг и структура клиентов."));

            doc.Blocks.Add(Header("Технологии"));
            doc.Blocks.Add(NormalText("Приложение разработано на языке C# для Windows с использованием WPF и шаблона MVVM (CommunityToolkit.Mvvm). Доступ к данным выполняется через Entity Framework Core и драйвер Npgsql, данные хранятся в СУБД PostgreSQL."));

            doc.Blocks.Add(Header("Требования к окружению"));
            doc.Blocks.Add(ListItem("операционная система Windows 10 или 11;"));
            doc.Blocks.Add(ListItem("среда выполнения .NET Desktop Runtime 8.0 (или ваша версия);"));
            doc.Blocks.Add(ListItem("доступ по сети к серверу PostgreSQL, на котором развёрнута база данных."));

            doc.Blocks.Add(Header("Безопасность и права доступа"));
            doc.Blocks.Add(NormalText("Пароли пользователей в базе данных не хранятся в открытом виде: сохраняется хеш SHA-256 пароля с индивидуальной солью. Права доступа задаются ролью пользователя: после входа в списке отображаются только таблицы, назначенные этой роли."));

            var table = new Table();
            table.Columns.Add(new TableColumn { Width = new GridLength(150) });
            table.Columns.Add(new TableColumn { Width = new GridLength(300) });

            var rowGroup = new TableRowGroup();
            table.RowGroups.Add(rowGroup);

            var headerRow = new TableRow();
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Роль")) { FontWeight = FontWeights.Bold, Margin = new Thickness(5) }));
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Доступные данные")) { FontWeight = FontWeights.Bold, Margin = new Thickness(5) }));
            rowGroup.Rows.Add(headerRow);

            void AddRoleRow(string role, string access)
            {
                var row = new TableRow();
                row.Cells.Add(new TableCell(new Paragraph(new Run(role)) { Margin = new Thickness(5) }));
                row.Cells.Add(new TableCell(new Paragraph(new Run(access)) { Margin = new Thickness(5) }));
                rowGroup.Rows.Add(row);
            }

            AddRoleRow("Владелец магазина", "все таблицы");
            AddRoleRow("Менеджер по продажам", "заказы и клиенты");
            AddRoleRow("Товаровед", "книги, экземпляры, поставщики, поставки");
            AddRoleRow("Менеджер по кадрам", "сведения о сотрудниках");

            doc.Blocks.Add(table);

            rtbAbout.Document = doc;
        }
    }
}