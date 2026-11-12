using System.Collections.Generic;
using System.Linq;

namespace csharp_DBCourseWork_sem5.Services
{
    public static class ColumnTranslator
    {
        private static readonly Dictionary<string, string> _translations = new()
        {
            { "id", "ID" },
            { "name", "Название" },
            { "type", "Тип" },
            { "code", "Код" },
            { "year", "Год" },
            { "quantity", "Количество" },
            { "table_name", "Имя таблицы" },
            { "func_name", "Имя функции" },

            { "first_name", "Имя" },
            { "last_name", "Фамилия" },
            { "middle_name", "Отчество" },
            { "phone_number", "Телефон" },
            { "house_number", "Номер дома" },
            { "passport_series", "Серия паспорта" },
            { "passport_number", "Номер паспорта" },
            { "date_issue", "Дата выдачи" },
            { "issued_by", "Кем выдан" },
            { "work_experience", "Стаж (мес.)" },

            { "city_id", "Город" },
            { "street_id", "Улица" },

            { "annotation", "Аннотация" },
            { "edition", "Издание" },
            { "purchase_price", "Цена закупки" },
            { "sale_price", "Цена продажи" },
            { "book_id", "ID книги" },
            { "copy_id", "ID экземпляра" },

            { "author_id", "ID автора" },
            { "publishing_house_id", "ID издательства" },

            { "order_number", "Номер заказа" },
            { "order_date", "Дата заказа" },
            { "delivery_date", "Дата поставки" },
            { "customer_id", "ID клиента" },
            { "employee_id", "ID сотрудника" },
            { "supplier_id", "ID поставщика" },
            { "legal_id", "ID юрлица" },

            { "position_id", "Должность" },
            { "profession_id", "Профессия" },
            { "structural_unit_id", "Отдел" },
            { "specialty_id", "Специальность" },
            { "qualification_id", "Квалификация" },
            { "type_of_event", "Тип события" },
            { "date_of_employment", "Дата приема" },
            { "date_of_dismissal", "Дата увольнения" },
            { "date_of_transfer", "Дата перевода" },
            { "basis_date", "Дата документа" },
            { "basis_number", "Номер документа" },
            { "basis_typedoc", "Тип документа" },
            { "grounds_for_termination", "Основание увольнения" },
            { "individual_id", "ID физлица" },

            { "abbreviation", "Аббревиатура" },
            { "current_account_number", "Расчетный счет" },
            { "bank_id", "ID банка" },
            { "bik", "БИК" },
            { "inn", "ИНН" },
            { "number_of_correspondent", "Корр. счет" },

            { "login", "Логин" },
            { "password_hash", "Хэш пароля" },
            { "salt", "Соль" },
            { "role", "Роль" },
            { "created_at", "Дата создания" },
            { "last_login", "Последний вход" }
        };

        public static string GetDisplayName(string columnName)
        {
            if (string.IsNullOrWhiteSpace(columnName)) return columnName;

            string key = columnName.ToLower();

            if (_translations.TryGetValue(key, out string? translated))
            {
                return translated;
            }

            return char.ToUpper(columnName[0]) + columnName.Substring(1).Replace("_", " ");
        }
    }
}