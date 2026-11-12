using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace csharp_DBCourseWork_sem5
{
    public partial class UserManualPage : Page
    {
        public UserManualPage()
        {
            InitializeComponent();
            LoadManual();
        }

        private void LoadManual()
        {
            var doc = new FlowDocument();
            doc.FontFamily = new FontFamily("Segoe UI");
            doc.FontSize = 14;
            doc.Foreground = Brushes.Black;
            doc.PagePadding = new Thickness(20);

            void AddHeader(string text, int level = 1)
            {
                var p = new Paragraph(new Run(text))
                {
                    FontSize = level == 1 ? 22 : 18,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.DarkBlue,
                    Margin = new Thickness(0, 15, 0, 10)
                };
                doc.Blocks.Add(p);
            }

            void AddText(string text)
            {
                doc.Blocks.Add(new Paragraph(new Run(text)) { Margin = new Thickness(0, 0, 0, 10) });
            }

            void AddListItem(string text, int level = 1)
            {
                var p = new Paragraph(new Run((level == 1 ? "• " : "   - ") + text))
                {
                    Margin = new Thickness(level == 1 ? 20 : 40, 0, 0, 5)
                };
                doc.Blocks.Add(p);
            }

            void AddNumbered(string num, string text)
            {
                var p = new Paragraph();
                p.Inlines.Add(new Bold(new Run(num + ". ")));
                p.Inlines.Add(new Run(text));
                p.Margin = new Thickness(20, 0, 0, 5);
                doc.Blocks.Add(p);
            }

            AddHeader("1 Вход в систему");
            AddNumbered("1", "Запустите приложение, откроется окно входа.");
            AddNumbered("2", "Введите логин и пароль, выданные администратором.");
            AddNumbered("3", "Нажмите кнопку «Войти».");
            AddText("При успешном входе открывается главное окно. Если логин или пароль неверны либо база данных недоступна, выводится сообщение об ошибке, и вход можно повторить.");

            AddHeader("2 Главное окно");
            AddText("Главное окно состоит из следующих областей:");
            AddListItem("Список таблиц слева. Справочники (города, улицы, должности, банки, издательства, авторы и др.) вынесены в отдельную группу.");
            AddListItem("Панель инструментов со строкой поиска и кнопками экспорта, режима редактирования, сохранения и удаления.");
            AddListItem("Область данных с двумя вкладками: DataOutput показывает таблицу или результат запроса, Messages – сообщения программы и ошибки.");
            AddListItem("Поле SQL-запроса и кнопка его выполнения.");
            AddListItem("Главное меню, из которого открываются дополнительные страницы, например «Аналитика».");

            AddHeader("3 Просмотр таблиц");
            AddText("Выберите таблицу в списке слева: её данные появятся на вкладке DataOutput. Для скорости загружаются первые 1000 строк. Столбцы можно сортировать щелчком по заголовку.");

            AddHeader("4 Поиск");
            AddNumbered("1", "Введите искомое значение в строку поиска и запустите поиск.");
            AddNumbered("2", "Строки открытой таблицы, содержащие это значение в любом поле, подсвечиваются (регистр не учитывается).");
            AddNumbered("3", "Если название какой-либо таблицы содержит введённый текст, программа открывает первую подходящую таблицу.");

            AddHeader("5 SQL-запросы");
            AddNumbered("1", "Введите запрос в поле SQL.");
            AddNumbered("2", "Нажмите кнопку выполнения запроса.");
            AddNumbered("3", "Результат появится на вкладке DataOutput, а на вкладке Messages – число строк и время выполнения.");
            AddText("Если запрос пустой или содержит ошибку, откроется вкладка Messages с описанием причины. В режиме редактирования выполнение запросов недоступно.");

            AddHeader("6 Экспорт в Excel");
            AddText("Выберите таблицу и нажмите кнопку экспорта. Файл Excel с данными сохраняется в папку «Загрузки», его имя начинается с названия таблицы. Если таблица не выбрана, программа предложит сделать это сначала.");

            AddHeader("7 Изменение данных");
            AddHeader("7.1 Режим редактирования", 2);
            AddText("Выберите таблицу и включите режим редактирования кнопкой на панели инструментов. Пока режим включён, выполнение SQL-запросов недоступно. Редактирование возможно только для таблиц с первичным ключом: представления изменять нельзя.");

            AddHeader("7.2 Добавление записи", 2);
            AddNumbered("1", "В режиме редактирования нажмите кнопку добавления строки.");
            AddNumbered("2", "Заполните поля новой строки. Поля, у которых в базе данных есть значение по умолчанию, заполнятся автоматически.");
            AddNumbered("3", "Нажмите кнопку сохранения.");

            AddHeader("7.3 Изменение записи", 2);
            AddText("Измените значение в нужной ячейке (двойной щелчок или клавиша F2), завершите ввод клавишей Enter и нажмите кнопку сохранения. Поля первичного ключа (например, id) менять нельзя. Изменённые строки сохраняются вместе, одной операцией: при ошибке не сохраняется ни одна.");

            AddHeader("7.4 Проверка данных при сохранении", 2);
            AddText("Перед записью в базу данных программа проверяет значения. Если найдены ошибки, сохранение отменяется, а на вкладке Messages выводится список ошибок с номерами строк. Проверяются:");
            AddListItem("заполнение обязательных полей;");
            AddListItem("допустимая длина текста;");
            AddListItem("числовые поля содержат числа, а поля дат – даты;");
            AddListItem("цена закупки и цена продажи больше 0, количество не меньше 0, год издания не меньше 868.");
            AddText("Если выйти из режима редактирования при несохранённых изменениях, программа спросит, отменить ли их.");

            AddHeader("7.5 Удаление записи", 2);
            AddNumbered("1", "В режиме редактирования выделите строки щелчком по серому заголовку слева (для нескольких строк – с клавишей Ctrl или Shift).");
            AddNumbered("2", "Нажмите кнопку удаления.");
            AddNumbered("3", "В окне «Подтверждение удаления» нажмите «Да».");
            AddText("Связанные записи в других таблицах удаляются каскадом. Действие необратимо, поэтому по умолчанию в окне выбрана кнопка «Нет».");

            AddHeader("8 Аналитика продаж");
            AddText("Откройте пункт «Аналитика» в главном меню. Вверху страницы указывается период: по умолчанию выбран весь диапазон дат заказов, границы можно изменить. Страница показывает:");
            AddListItem("карточки показателей: выручка, число заказов, средний чек, прибыль;");
            AddListItem("выручку по месяцам (последние 12 месяцев периода);");
            AddListItem("топ-10 книг по выручке;");
            AddListItem("выручку по типам клиентов с долей в процентах.");
            AddText("Кнопка «Обновить» перечитывает данные из базы (результаты запросов кэшируются на 5 минут). Если дата начала периода позже даты окончания, выводится сообщение об ошибке. Чтобы закрыть страницу, воспользуйтесь командой закрытия в главном меню.");

            AddHeader("9 Сообщения об ошибках");

            var table = new Table();
            table.Columns.Add(new TableColumn { Width = new GridLength(200) });
            table.Columns.Add(new TableColumn { Width = new GridLength(250) });
            table.Columns.Add(new TableColumn { Width = new GridLength(250) });
            var rowGroup = new TableRowGroup();
            table.RowGroups.Add(rowGroup);

            var headerRow = new TableRow();
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Сообщение")) { FontWeight = FontWeights.Bold, Margin = new Thickness(5) }));
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Причина")) { FontWeight = FontWeights.Bold, Margin = new Thickness(5) }));
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Что делать")) { FontWeight = FontWeights.Bold, Margin = new Thickness(5) }));
            rowGroup.Rows.Add(headerRow);

            void AddErrorRow(string msg, string cause, string action)
            {
                var row = new TableRow();
                row.Cells.Add(new TableCell(new Paragraph(new Run(msg)) { Margin = new Thickness(5) }));
                row.Cells.Add(new TableCell(new Paragraph(new Run(cause)) { Margin = new Thickness(5) }));
                row.Cells.Add(new TableCell(new Paragraph(new Run(action)) { Margin = new Thickness(5) }));
                rowGroup.Rows.Add(row);
            }

            AddErrorRow("Неверный логин или пароль", "Данные введены неправильно", "Проверьте логин, пароль и раскладку клавиатуры");
            AddErrorRow("Нет соединения с БД", "Сервер PostgreSQL недоступен", "Проверьте сеть и работу сервера, повторите действие");
            AddErrorRow("Запрос пустой", "В поле SQL нет текста", "Введите запрос");
            AddErrorRow("Ошибка SQL (код): …", "Ошибка в запросе или нарушено ограничение базы данных", "Прочитайте текст ошибки на вкладке Messages и исправьте запрос или данные");
            AddErrorRow("Сохранение отменено: Строка N: …", "Значение не прошло проверку", "Исправьте указанное поле и сохраните снова");
            AddErrorRow("Редактирование невозможно: нет первичного ключа", "Выбрано представление или таблица без ключа", "Используйте режим просмотра");
            AddErrorRow("Сначала выберите таблицу", "Действие выполнено без выбранной таблицы", "Выберите таблицу в списке слева");

            doc.Blocks.Add(table);

            AddHeader("10 Выход из программы");
            AddText("Закройте главное окно: программа вернётся к окну входа, где можно войти под другим пользователем. Чтобы завершить работу, закройте и окно входа.");

            rtbAbout.Document = doc;
        }
    }
}