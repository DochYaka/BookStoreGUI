using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace csharp_DBCourseWork_sem5.Services
{
    public static class AnalyticsPlotBuilder
    {
        public static PlotModel BuildLine(
            string title,
            IReadOnlyList<string> labels,
            IReadOnlyList<double> values,
            string xTitle = "",
            string yTitle = "",
            string seriesTitle = "")
        {
            var model = new PlotModel
            {
                Title = title,
                TitleColor = OxyColors.White,
                TextColor = OxyColors.White,
                PlotAreaBorderColor = OxyColors.Gray,
                Background = OxyColors.Transparent,
            };

            var series = new LineSeries
            {
                Title = seriesTitle,
                MarkerType = MarkerType.Circle,
                MarkerSize = 4,
                Color = OxyColor.Parse("#4FC3F7"),
                MarkerFill = OxyColor.Parse("#4FC3F7"),
                StrokeThickness = 2,
            };

            for (int i = 0; i < values.Count; i++)
                series.Points.Add(new DataPoint(i, values[i]));

            model.Series.Add(series);

            model.Axes.Add(new CategoryAxis
            {
                Position = AxisPosition.Bottom,
                ItemsSource = labels.ToList(),
                Title = xTitle,
                TitleColor = OxyColors.White,
                TextColor = OxyColors.White,
                TicklineColor = OxyColors.Gray,
                AxislineColor = OxyColors.Gray,
                MajorGridlineStyle = LineStyle.Dot,
                MajorGridlineColor = OxyColor.Parse("#33FFFFFF"),
            });

            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = yTitle,
                TitleColor = OxyColors.White,
                TextColor = OxyColors.White,
                TicklineColor = OxyColors.Gray,
                AxislineColor = OxyColors.Gray,
                MinimumPadding = 0,
                AbsoluteMinimum = 0,
                MajorGridlineStyle = LineStyle.Dot,
                MajorGridlineColor = OxyColor.Parse("#33FFFFFF"),
                LabelFormatter = v => v.ToString("N0"),
            });

            return model;
        }

        public static PlotModel BuildBar(
            string title,
            IReadOnlyList<string> labels,
            IReadOnlyList<double> values,
            string xTitle = "",
            string yTitle = "")
        {
            var model = new PlotModel
            {
                Title = title,
                TitleColor = OxyColors.White,
                TextColor = OxyColors.White,
                PlotAreaBorderColor = OxyColors.Gray,
                Background = OxyColors.Transparent,
            };

            var series = new BarSeries
            {
                FillColor = OxyColor.Parse("#4FC3F7"),
                StrokeThickness = 0,
            };

            for (int i = 0; i < values.Count; i++)
                series.Items.Add(new BarItem(values[i]));

            model.Series.Add(series);

            model.Axes.Add(new CategoryAxis
            {
                Position = AxisPosition.Left,
                ItemsSource = labels.ToList(),
                Title = yTitle,
                TitleColor = OxyColors.White,
                TextColor = OxyColors.White,
                TicklineColor = OxyColors.Gray,
                AxislineColor = OxyColors.Gray,
            });

            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = xTitle,
                TitleColor = OxyColors.White,
                TextColor = OxyColors.White,
                TicklineColor = OxyColors.Gray,
                AxislineColor = OxyColors.Gray,
                MinimumPadding = 0,
                AbsoluteMinimum = 0,
                LabelFormatter = v => v.ToString("N0"),
            });

            return model;
        }
    }
}