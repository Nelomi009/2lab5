using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Label = System.Windows.Forms.Label;
using MathFunction = System.Func<double, double>;

namespace _2lab2 {
  public partial class task2 : Form {
    private readonly MathSolver _solver = new MathSolver();

    // Храним последнюю найденную точку и значение функции для отображения на графике
    private double? _lastRootX = null;
    private double? _lastRootY = null;

    // Элементы управления (названия как на фото, но txtE — TextBox для вещественного числа по методичке)
    private TextBox txtA;
    private TextBox txtB;
    private TextBox txtE;
    private TextBox txtFunc;
    private Chart chart1;
    private Label lblResult;

    public task2() {
      InitializeComponent();
      BuildUserInterface();
      SetupChart();
    }

    // Обработчик, на который ссылается task2.Designer.cs
    private void task2_Load(object sender, EventArgs eventArgs) {

    }

    private void BuildUserInterface() {
      Text = "Лабораторная работа №1 — Метод дихотомии";
      StartPosition = FormStartPosition.CenterScreen;
      ClientSize = new Size(900, 650);
      MinimumSize = new Size(700, 500);

      // Меню (все кнопки действий размещены в MenuStrip согласно требованиям)
      MenuStrip mainMenu = new MenuStrip();
      ToolStripMenuItem calculateToolStripMenuItem = new ToolStripMenuItem("Рассчитать");
      ToolStripMenuItem plotGraphToolStripMenuItem = new ToolStripMenuItem("Построить график");
      ToolStripMenuItem clearToolStripMenuItem = new ToolStripMenuItem("Очистить");
      ToolStripMenuItem exitToolStripMenuItem = new ToolStripMenuItem("Выход");

      calculateToolStripMenuItem.Click += calculateToolStripMenuItem_Click;
      plotGraphToolStripMenuItem.Click += построитьГрафикToolStripMenuItem_Click;
      clearToolStripMenuItem.Click += clearToolStripMenuItem_Click;
      exitToolStripMenuItem.Click += (sender, e) => Close();

      mainMenu.Items.AddRange(new ToolStripItem[] {
        calculateToolStripMenuItem,
        plotGraphToolStripMenuItem,
        clearToolStripMenuItem,
        exitToolStripMenuItem
      });
      MainMenuStrip = mainMenu;

      // Панель ввода (a, b, e — вещественные числа, f(x) — строка с формулой)
      Panel inputPanel = new Panel { Dock = DockStyle.Top, Height = 100 };
      txtA = AddInputField(inputPanel, "a:", 15, 15, 90, "0");
      txtB = AddInputField(inputPanel, "b:", 165, 15, 90, "2,5");
      txtE = AddInputField(inputPanel, "e:", 315, 15, 90, "0,001");
      txtFunc = AddInputField(inputPanel, "f(x):", 15, 55, 520, "x^2 - 4*x + 3");

      Label labelHint = new Label {
        Text = "Допустимо: + - * / ^, скобки, x, pi, e,\nsin cos tan asin acos atan exp ln log sqrt abs",
        Location = new Point(560, 15),
        AutoSize = true,
        ForeColor = Color.DimGray
      };
      inputPanel.Controls.Add(labelHint);

      // Поле вывода результата
      lblResult = new Label {
        Dock = DockStyle.Bottom,
        Height = 40,
        Font = new Font("Arial", 11f, FontStyle.Bold),
        ForeColor = Color.DarkGreen,
        TextAlign = ContentAlignment.MiddleLeft,
        Padding = new Padding(10, 0, 0, 0),
        Text = "Значение функции: —"
      };

      // График
      chart1 = new Chart { Dock = DockStyle.Fill };
      chart1.Legends.Add(new Legend("legend") { Docking = Docking.Top });

      // Порядок добавления важен для Dock: сначала Fill, затем остальные
      Controls.Add(chart1);
      Controls.Add(lblResult);
      Controls.Add(inputPanel);
      Controls.Add(mainMenu);
    }

    private TextBox AddInputField(Panel parentPanel, string caption, int positionX, int positionY, int fieldWidth, string defaultText) {
      Label captionLabel = new Label { Text = caption, Location = new Point(positionX, positionY + 3), AutoSize = true };
      parentPanel.Controls.Add(captionLabel);

      TextBox textBox = new TextBox {
        Location = new Point(positionX + captionLabel.PreferredWidth + 8, positionY),
        Width = fieldWidth,
        Text = defaultText
      };
      parentPanel.Controls.Add(textBox);
      return textBox;
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

      // Найденная точка на графике
      Series seriesRoot = new Series("RootPoint") {
        ChartType = SeriesChartType.Point,
        MarkerSize = 12,
        MarkerStyle = MarkerStyle.Circle,
        Color = Color.Red,
        LegendText = "Найденное значение"
      };
      chart1.Series.Add(seriesRoot);

      // Оси пересекаются в (0, 0)
      chart1.ChartAreas[0].AxisX.Crossing = 0;
      chart1.ChartAreas[0].AxisY.Crossing = 0;
      chart1.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.Gainsboro;
      chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.Gainsboro;
      chart1.ChartAreas[0].AxisX.LabelStyle.Format = "0.##";
      chart1.ChartAreas[0].AxisY.LabelStyle.Format = "0.##";
    }

    // =========================================================================
    // БЛОК 2: РАССЧИТАТЬ (только считает, не рисует!)
    // =========================================================================
    private void calculateToolStripMenuItem_Click(object sender, EventArgs e) {
      try {
        // Валидация входных данных (a, b, e — вещественные числа)
        if (!TryParseDouble(txtA.Text, out double a))
          throw new Exception("Параметр 'a' должен быть вещественным числом.");
        if (!TryParseDouble(txtB.Text, out double b))
          throw new Exception("Параметр 'b' должен быть вещественным числом.");
        if (!TryParseDouble(txtE.Text, out double eps))
          throw new Exception("Параметр 'e' должен быть вещественным числом.");
        if (a >= b)
          throw new Exception("Интервал задан неверно: 'a' должно быть меньше 'b'.");
        if (eps <= 0)
          throw new Exception("Точность 'e' должна быть больше нуля.");

        string funcStr = txtFunc.Text;
        if (string.IsNullOrWhiteSpace(funcStr))
          throw new Exception("Введите формулу функции f(x).");

        // Считаем методом дихотомии
        var result = _solver.FindRoot(funcStr, a, b, eps);

        // Сохраняем координаты для графика
        _lastRootX = result.Root;
        _lastRootY = result.RootValue;

        // Вычисляем количество знаков после запятой из введённого вещественного e
        int precisionDigits = Math.Min(12, Math.Max(2, (int)Math.Ceiling(-Math.Log10(eps))));
        string format = "F" + (precisionDigits + 2);

        // Выводим и найденное значение функции f(x), и точку x
        lblResult.Text = $"Значение функции: f(x) = {result.RootValue.ToString(format)}   (при x = {result.Root.ToString(format)})";
      }
      catch (Exception ex) {
        MessageBox.Show(ex.Message, "Ошибка расчёта",
          MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }
    }

    // =========================================================================
    // БЛОК 3: ПОСТРОИТЬ ГРАФИК (только рисует, не считает!)
    // =========================================================================
    private void построитьГрафикToolStripMenuItem_Click(object sender, EventArgs e) {
      try {
        // Валидация
        if (!TryParseDouble(txtA.Text, out double a))
          throw new Exception("Параметр 'a' должен быть вещественным числом.");
        if (!TryParseDouble(txtB.Text, out double b))
          throw new Exception("Параметр 'b' должен быть вещественным числом.");
        if (a >= b)
          throw new Exception("Интервал задан неверно: 'a' должно быть меньше 'b'.");

        string funcStr = txtFunc.Text;
        if (string.IsNullOrWhiteSpace(funcStr))
          throw new Exception("Введите формулу функции f(x).");

        // Рисуем
        PlotGraph(funcStr, a, b);
      }
      catch (Exception ex) {
        MessageBox.Show(ex.Message, "Ошибка отрисовки",
          MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }
    }

    // =========================================================================
    // ОТРИСОВКА ГРАФИКА (вызывается из "Построить график")
    // =========================================================================
    private void PlotGraph(string funcStr, double a, double b) {
      chart1.Series["Function"].Points.Clear();
      chart1.Series["RootPoint"].Points.Clear();

      // Расширяем диапазон, чтобы был виден контекст
      double delta = (b - a) * 0.3;
      double minX = a - delta;
      double maxX = b + delta;

      if (Math.Abs(maxX - minX) < 0.1) { minX -= 1; maxX += 1; }

      double step = (maxX - minX) / 500;
      const double yLimit = 1e6;

      for (double x = minX; x <= maxX; x += step) {
        try {
          double y = _solver.CalculateFunction(funcStr, x);
          if (double.IsNaN(y) || double.IsInfinity(y) || Math.Abs(y) > yLimit)
            continue;
          chart1.Series["Function"].Points.AddXY(x, y);
        }
        catch { }
      }

      // Если точка была найдена ранее — отмечаем её на графике
      if (_lastRootX.HasValue && _lastRootY.HasValue) {
        chart1.Series["RootPoint"].Points.AddXY(_lastRootX.Value, _lastRootY.Value);
      }

      chart1.ChartAreas[0].RecalculateAxesScale();
    }

    // =========================================================================
    // БЛОК 4: ОЧИСТИТЬ
    // =========================================================================
    private void clearToolStripMenuItem_Click(object sender, EventArgs e) {
      txtA.Clear();
      txtB.Clear();
      txtE.Clear();
      txtFunc.Clear();
      _lastRootX = null;
      _lastRootY = null;

      lblResult.Text = "Значение функции: —";
      chart1.Series["Function"].Points.Clear();
      chart1.Series["RootPoint"].Points.Clear();
    }

    // Поддержка ввода вещественных чисел как через запятую, так и через точку
    private static bool TryParseDouble(string text, out double number) {
      number = 0;
      if (string.IsNullOrWhiteSpace(text)) return false;
      text = text.Trim().Replace(',', '.');
      if (!double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out number)) return false;
      return !double.IsNaN(number) && !double.IsInfinity(number);
    }
  }

  // ---------- КЛАСС РЕШАТЕЛЯ (MathSolver со скриншотов) ----------
  public class RootResult {
    public double Root { get; set; }
    public double RootValue { get; set; }
  }

  public class MathSolver {
    public double CalculateFunction(string funcStr, double x) {
      MathFunction fn = FormulaParser.Compile(funcStr);
      return fn(x);
    }

    // Метод дихотомии (половинного деления) на интервале [a, b] с точностью eps
    public RootResult FindRoot(string funcStr, double a, double b, double eps) {
      MathFunction fn;
      try {
        fn = FormulaParser.Compile(funcStr);
      }
      catch (FormatException ex) {
        throw new Exception("Ошибка в формуле: " + ex.Message);
      }

      double fa = EvaluateOrThrow(fn, a);
      double fb = EvaluateOrThrow(fn, b);

      if (Math.Abs(fa) <= eps) return new RootResult { Root = a, RootValue = fa };
      if (Math.Abs(fb) <= eps) return new RootResult { Root = b, RootValue = fb };

      if (fa * fb > 0) {
        throw new Exception("На концах отрезка [a; b] функция должна иметь разные знаки (f(a) * f(b) < 0).");
      }

      double left = a;
      double right = b;
      int iterations = 0;
      const int maxIterations = 1000;

      while ((right - left) / 2.0 > eps) {
        iterations++;
        if (iterations > maxIterations) {
          throw new Exception("Не удалось достичь заданной точности: значение 'e' слишком мало для этого интервала.");
        }

        double mid = (left + right) / 2.0;
        double fmid = EvaluateOrThrow(fn, mid);

        if (Math.Abs(fmid) <= eps) {
          left = right = mid;
          break;
        }

        if (fa * fmid < 0) {
          right = mid;
          fb = fmid;
        }
        else {
          left = mid;
          fa = fmid;
        }
      }

      double root = (left + right) / 2.0;
      double rootVal = EvaluateOrThrow(fn, root);
      return new RootResult { Root = root, RootValue = rootVal };
    }

    private static double EvaluateOrThrow(MathFunction fn, double x) {
      double y = fn(x);
      if (double.IsNaN(y) || double.IsInfinity(y))
        throw new Exception($"Функция не определена в точке x = {x:G6}. Выберите другой интервал.");
      return y;
    }
  }

  // ---------- РАЗБОР ФОРМУЛЫ ----------
  internal static class FormulaParser {
    public static MathFunction Compile(string formulaText) {
      return new Parser(formulaText).ParseAll();
    }

    private class Parser {
      private readonly string formulaText;
      private int position;

      public Parser(string formulaText) {
        this.formulaText = formulaText;
      }

      public MathFunction ParseAll() {
        MathFunction expression = ParseExpression();
        SkipWhitespace();
        if (position < formulaText.Length)
          throw new FormatException($"неожиданный символ '{formulaText[position]}' в позиции {position + 1}.");
        return expression;
      }

      private void SkipWhitespace() {
        while (position < formulaText.Length && char.IsWhiteSpace(formulaText[position])) position++;
      }

      private bool TryConsume(char expectedChar) {
        SkipWhitespace();
        if (position < formulaText.Length && formulaText[position] == expectedChar) {
          position++;
          return true;
        }
        return false;
      }

      private MathFunction ParseExpression() {
        MathFunction result = ParseTerm();
        while (true) {
          if (TryConsume('+')) {
            MathFunction leftOperand = result;
            MathFunction rightOperand = ParseTerm();
            result = xValue => leftOperand(xValue) + rightOperand(xValue);
          }
          else if (TryConsume('-')) {
            MathFunction leftOperand = result;
            MathFunction rightOperand = ParseTerm();
            result = xValue => leftOperand(xValue) - rightOperand(xValue);
          }
          else return result;
        }
      }

      private MathFunction ParseTerm() {
        MathFunction result = ParseUnary();
        while (true) {
          if (TryConsume('*')) {
            MathFunction leftOperand = result;
            MathFunction rightOperand = ParseUnary();
            result = xValue => leftOperand(xValue) * rightOperand(xValue);
          }
          else if (TryConsume('/')) {
            MathFunction leftOperand = result;
            MathFunction rightOperand = ParseUnary();
            result = xValue => leftOperand(xValue) / rightOperand(xValue);
          }
          else return result;
        }
      }

      private MathFunction ParseUnary() {
        if (TryConsume('-')) {
          MathFunction operand = ParseUnary();
          return xValue => -operand(xValue);
        }
        if (TryConsume('+')) return ParseUnary();
        return ParsePower();
      }

      private MathFunction ParsePower() {
        MathFunction baseValue = ParsePrimary();
        if (TryConsume('^')) {
          MathFunction exponent = ParseUnary();
          return xValue => Math.Pow(baseValue(xValue), exponent(xValue));
        }
        return baseValue;
      }

      private MathFunction ParsePrimary() {
        SkipWhitespace();
        if (position >= formulaText.Length)
          throw new FormatException("формула оборвана, ожидалось выражение.");

        char currentChar = formulaText[position];

        if (currentChar == '(') {
          position++;
          MathFunction insideBrackets = ParseExpression();
          if (!TryConsume(')')) throw new FormatException("не хватает закрывающей скобки ')'.");
          return insideBrackets;
        }

        if (char.IsDigit(currentChar) || currentChar == '.' || currentChar == ',')
          return ParseNumber();

        if (char.IsLetter(currentChar)) {
          int nameStart = position;
          while (position < formulaText.Length && char.IsLetter(formulaText[position])) position++;
          string name = formulaText.Substring(nameStart, position - nameStart).ToLowerInvariant();

          switch (name) {
            case "x": return xValue => xValue;
            case "pi": return xValue => Math.PI;
            case "e": return xValue => Math.E;
          }

          if (!TryConsume('(')) throw new FormatException($"неизвестное имя '{name}'.");
          MathFunction argument = ParseExpression();
          if (!TryConsume(')')) throw new FormatException($"не хватает ')' после аргумента функции {name}.");

          switch (name) {
            case "sin": return xValue => Math.Sin(argument(xValue));
            case "cos": return xValue => Math.Cos(argument(xValue));
            case "tan": return xValue => Math.Tan(argument(xValue));
            case "asin": return xValue => Math.Asin(argument(xValue));
            case "acos": return xValue => Math.Acos(argument(xValue));
            case "atan": return xValue => Math.Atan(argument(xValue));
            case "exp": return xValue => Math.Exp(argument(xValue));
            case "ln": return xValue => Math.Log(argument(xValue));
            case "log": return xValue => Math.Log10(argument(xValue));
            case "sqrt": return xValue => Math.Sqrt(argument(xValue));
            case "abs": return xValue => Math.Abs(argument(xValue));
            default: throw new FormatException($"неизвестная функция '{name}'.");
          }
        }

        throw new FormatException($"неожиданный символ '{currentChar}' в позиции {position + 1}.");
      }

      private MathFunction ParseNumber() {
        int numberStart = position;
        while (position < formulaText.Length &&
               (char.IsDigit(formulaText[position]) || formulaText[position] == '.' || formulaText[position] == ','))
          position++;

        if (position < formulaText.Length && (formulaText[position] == 'e' || formulaText[position] == 'E')) {
          int exponentEnd = position + 1;
          if (exponentEnd < formulaText.Length && (formulaText[exponentEnd] == '+' || formulaText[exponentEnd] == '-'))
            exponentEnd++;
          if (exponentEnd < formulaText.Length && char.IsDigit(formulaText[exponentEnd])) {
            while (exponentEnd < formulaText.Length && char.IsDigit(formulaText[exponentEnd])) exponentEnd++;
            position = exponentEnd;
          }
        }

        string numberText = formulaText.Substring(numberStart, position - numberStart).Replace(',', '.');
        if (!double.TryParse(numberText, NumberStyles.Float, CultureInfo.InvariantCulture, out double numberValue))
          throw new FormatException($"некорректное число '{numberText}'.");
        return xValue => numberValue;
      }
    }
  }
}