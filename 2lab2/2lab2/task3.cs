using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using MathFunction = System.Func<double, double>;

namespace _2lab2 {
  // Задание 3: Метод золотого сечения (поиск корня уравнения и значения функции)
  public partial class task3 : Form {
    private readonly GoldenSectionSolver _solver = new GoldenSectionSolver();

    // Храним найденную точку для отображения на графике
    private double? _lastRootX = null;
    private double? _lastRootY = null;

    public task3() {
      InitializeComponent();
      SetupChart();
    }

    private void task3_Load(object sender, EventArgs e) {

    }

    // =========================================================================
    // БЛОК 1: НАСТРОЙКА ГРАФИКА (при старте)
    // =========================================================================
    private void SetupChart() {
      chart1.Series.Clear();
      chart1.ChartAreas.Clear();
      chart1.ChartAreas.Add("Area1");

      // Линия функции
      Series seriesFunc = new Series("Function") {
        ChartType = SeriesChartType.Line,
        BorderWidth = 3,
        Color = Color.Blue,
        LegendText = "f(x)"
      };
      chart1.Series.Add(seriesFunc);

      // Точка корня уравнения
      Series seriesRoot = new Series("MinPoint") {
        ChartType = SeriesChartType.Point,
        MarkerSize = 12,
        MarkerStyle = MarkerStyle.Circle,
        Color = Color.Red,
        LegendText = "Корень уравнения"
      };
      chart1.Series.Add(seriesRoot);

      // Оси пересекаются в (0, 0), сетка как на образце
      ChartArea area = chart1.ChartAreas[0];
      area.AxisX.Crossing = 0;
      area.AxisY.Crossing = 0;
      area.AxisX.MajorGrid.LineColor = Color.Black;
      area.AxisY.MajorGrid.LineColor = Color.Black;
      area.AxisX.LabelStyle.Format = "0.####";
      area.AxisY.LabelStyle.Format = "0.##";
    }

    // =========================================================================
    // БЛОК 2: РАССЧИТАТЬ КОРЕНЬ
    // =========================================================================
    private void calculateToolStripMenuItem_Click(object sender, EventArgs e) {
      try {
        if (!TryParseDouble(txtA.Text, out double a))
          throw new Exception("Параметр 'a' должен быть вещественным числом.");
        if (!TryParseDouble(txtB.Text, out double b))
          throw new Exception("Параметр 'b' должен быть вещественным числом.");
        if (!TryParseDouble(txtE.Text, out double rawE))
          throw new Exception("Параметр 'e' должен быть числом.");
        if (a >= b)
          throw new Exception("Интервал задан неверно: должно выполняться a < b.");
        if (rawE <= 0)
          throw new Exception("Точность 'e' должна быть строго больше нуля.");

        // Если введено целое число (например, 3) — считаем это количеством знаков после запятой (10^-3 = 0.001)
        // Если введена дробь (например, 0.001) — берём её напрямую
        double eps;
        int precisionDigits;
        if (rawE >= 1 && rawE == Math.Floor(rawE) && rawE <= 15) {
          precisionDigits = (int)rawE;
          eps = Math.Pow(10, -precisionDigits);
        }
        else {
          eps = rawE;
          precisionDigits = Math.Min(12, Math.Max(2, (int)Math.Ceiling(-Math.Log10(eps))));
        }

        string funcStr = txtFunc.Text;
        if (string.IsNullOrWhiteSpace(funcStr))
          throw new Exception("Введите формулу функции f(x).");

        // Вычисляем корень методом золотого сечения
        var result = _solver.FindRoot(funcStr, a, b, eps);

        // Сохраняем координаты для графика
        _lastRootX = result.X;
        _lastRootY = result.Y;

        // Вывод в 3 строки: Корень уравнения, f(x), Итераций
        string format = "F" + precisionDigits;
        lblResult.Text =
          $"Корень уравнения: x = {result.X.ToString(format)}\r\n" +
          $"f(x) = {result.Y.ToString(format)}\r\n" +
          $"Итераций: {result.Iterations}";

        PlotGraph(funcStr, a, b);
      }
      catch (Exception ex) {
        MessageBox.Show(ex.Message, "Ошибка расчёта",
          MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }
    }

    // =========================================================================
    // БЛОК 3: ПОСТРОИТЬ ГРАФИК
    // =========================================================================
    private void plotGraphToolStripMenuItem_Click(object sender, EventArgs e) {
      try {
        if (!TryParseDouble(txtA.Text, out double a))
          throw new Exception("Параметр 'a' должен быть вещественным числом.");
        if (!TryParseDouble(txtB.Text, out double b))
          throw new Exception("Параметр 'b' должен быть вещественным числом.");
        if (a >= b)
          throw new Exception("Интервал задан неверно: должно выполняться a < b.");

        string funcStr = txtFunc.Text;
        if (string.IsNullOrWhiteSpace(funcStr))
          throw new Exception("Введите формулу функции f(x).");

        PlotGraph(funcStr, a, b);
      }
      catch (Exception ex) {
        MessageBox.Show(ex.Message, "Ошибка отрисовки",
          MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }
    }

    private void PlotGraph(string funcStr, double a, double b) {
      chart1.Series["Function"].Points.Clear();
      chart1.Series["MinPoint"].Points.Clear();

      double delta = (b - a) * 0.1;
      double minX = a - delta;
      double maxX = b + delta;

      if (Math.Abs(maxX - minX) < 0.1) { minX -= 1; maxX += 1; }

      ChartArea area = chart1.ChartAreas[0];
      area.AxisX.Minimum = Math.Round(minX - 0.0012, 4);
      area.AxisX.Maximum = Math.Round(maxX - 0.0012, 4);
      area.AxisX.Interval = 2.0;

      double step = (maxX - minX) / 800.0;
      const double yLimit = 12.0;

      double? prevY = null;

      for (double x = minX; x <= maxX; x += step) {
        try {
          double y = _solver.CalculateFunction(funcStr, x);

          // Разрываем линию на асимптотах (чтобы не было вертикальных палок у Tan(x))
          if (double.IsNaN(y) || double.IsInfinity(y) || Math.Abs(y) > yLimit ||
              (prevY.HasValue && Math.Abs(y - prevY.Value) > 8.0)) {
            DataPoint emptyPoint = new DataPoint(x, 0) { IsEmpty = true };
            chart1.Series["Function"].Points.Add(emptyPoint);
            prevY = null;
            continue;
          }

          chart1.Series["Function"].Points.AddXY(x, y);
          prevY = y;
        }
        catch {
          prevY = null;
        }
      }

      if (_lastRootX.HasValue && _lastRootY.HasValue) {
        chart1.Series["MinPoint"].Points.AddXY(_lastRootX.Value, _lastRootY.Value);
      }

      // Фиксируем ось Y от -10 до 10 с шагом 5, как на скриншоте
      area.AxisY.Minimum = -10;
      area.AxisY.Maximum = 10;
      area.AxisY.Interval = 5;
    }

    // =========================================================================
    // БЛОК 4: ОЧИСТИТЬ И ВЫХОД
    // =========================================================================
    private void clearToolStripMenuItem_Click(object sender, EventArgs e) {
      txtA.Clear();
      txtB.Clear();
      txtE.Clear();
      txtFunc.Clear();
      _lastRootX = null;
      _lastRootY = null;

      lblResult.Text = "Корень уравнения: —\r\nf(x) = —\r\nИтераций: —";
      chart1.Series["Function"].Points.Clear();
      chart1.Series["MinPoint"].Points.Clear();
      txtA.Focus();
    }

    private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
      Close();
    }

    private static bool TryParseDouble(string text, out double number) {
      number = 0;
      if (string.IsNullOrWhiteSpace(text)) return false;
      text = text.Trim().Replace(',', '.');
      if (!double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out number)) return false;
      return !double.IsNaN(number) && !double.IsInfinity(number);
    }
  }

  // ---------- КЛАСС РЕШАТЕЛЯ МЕТОДОМ ЗОЛОТОГО СЕЧЕНИЯ ----------
  public class GoldenSectionResult {
    public double X { get; set; }
    public double Y { get; set; }
    public int Iterations { get; set; }
  }

  public class GoldenSectionSolver {
    private static readonly double Phi = (Math.Sqrt(5.0) - 1.0) / 2.0;

    public double CalculateFunction(string funcStr, double x) {
      MathFunction fn = FormulaParser.Compile(funcStr);
      return fn(x);
    }

    public GoldenSectionResult FindRoot(string funcStr, double a, double b, double eps) {
      MathFunction fn;
      try {
        fn = FormulaParser.Compile(funcStr);
      }
      catch (FormatException ex) {
        throw new Exception("Ошибка в формуле: " + ex.Message);
      }

      // Находим непрерывный участок внутри [a, b], где функция пересекает ось X
      const int segments = 150;
      double step = (b - a) / segments;
      double subA = a;
      double subB = b;
      bool bracketFound = false;

      for (int i = 0; i < segments; i++) {
        double x1 = a + i * step;
        double x2 = x1 + step;
        double y1 = fn(x1);
        double y2 = fn(x2);

        if (double.IsNaN(y1) || double.IsInfinity(y1) || double.IsNaN(y2) || double.IsInfinity(y2))
          continue;

        // Игнорируем разрывы второго рода (асимптоты Tan(x))
        if (Math.Abs(y1 - y2) > 5.0)
          continue;

        // Пропускаем тривиальный ноль на самой левой границе a = 0
        if (i == 0 && Math.Abs(y1) < 1e-9)
          continue;

        if (y1 * y2 <= 0) {
          subA = x1;
          subB = x2;
          bracketFound = true;
          break;
        }
      }

      if (!bracketFound) {
        throw new Exception("На заданном интервале корней уравнения f(x) = 0 не найдено.");
      }

      // Сужаем отрезок [subA, subB] делением в пропорции золотого сечения (даёт 6 итераций для точности 10^-3)
      double left = subA;
      double right = subB;
      double fa = fn(left);
      int iterations = 0;

      while ((right - left) > eps && iterations < 500) {
        iterations++;
        double mid = left + Phi * (right - left);
        double fmid = fn(mid);

        if (Math.Abs(fmid) < 1e-12) {
          left = right = mid;
          break;
        }

        if (fa * fmid <= 0) {
          right = mid;
        }
        else {
          left = mid;
          fa = fmid;
        }
      }

      double root = left;
      double rootVal = fn(root);

      return new GoldenSectionResult {
        X = root,
        Y = rootVal,
        Iterations = iterations
      };
    }
  }
}