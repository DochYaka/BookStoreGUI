namespace csharp_DBCourseWork_sem5.Services
{
    public static class AnalyticsQueries
    {
        public const string OrdersRange =
            @"SELECT MIN(order_date), MAX(order_date) FROM orders";

        public const string SalesKpi = @"
            SELECT COALESCE(SUM(o.quantity * c.sale_price), 0) AS revenue,
                   COUNT(*) AS orders_count,
                   COALESCE(SUM(o.quantity * (c.sale_price - c.purchase_price)), 0) AS profit
            FROM orders o
            JOIN copy c ON c.id = o.copy_id
            WHERE o.order_date BETWEEN @from AND @to";

        public const string SalesByMonth = @"
            SELECT date_trunc('month', o.order_date)::date AS month,
                   SUM(o.quantity * c.sale_price) AS revenue
            FROM orders o
            JOIN copy c ON c.id = o.copy_id
            WHERE o.order_date BETWEEN @from AND @to
            GROUP BY 1
            ORDER BY 1";

        public const string TopBooks = @"
            SELECT b.id, b.name, SUM(o.quantity * c.sale_price) AS revenue
            FROM orders o
            JOIN copy c ON c.id = o.copy_id
            JOIN book b ON b.id = c.book_id
            WHERE o.order_date BETWEEN @from AND @to
            GROUP BY b.id, b.name
            ORDER BY revenue DESC
            LIMIT 10";

        public const string RevenueByCustomerType = @"
            SELECT cu.type, SUM(o.quantity * c.sale_price) AS revenue
            FROM orders o
            JOIN copy c ON c.id = o.copy_id
            JOIN customer cu ON cu.id = o.customer_id
            WHERE o.order_date BETWEEN @from AND @to
            GROUP BY cu.type
            ORDER BY revenue DESC";
    }
}