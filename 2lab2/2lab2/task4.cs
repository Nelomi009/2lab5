using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using MathFunction = System.Func<double, double>;

namespace _2lab2 {
  public partial class task4 : Form {

    private TextBox txtA, txtB, txtE, txtFunc;
    private Label lblResult;
    private Chart chart1;

    // Переменные для пошагового режима
    private bool isStepping = false;
    private double currentX;
    private int currentStep;
    private double currentEps;
    private MathFunction currentFn;

    public task4() {
      InitializeComponent();
      BuildUI();
    }

    private void BuildUI() {
      Text = "Лабораторная №2 — Метод Ньютона";
      Size = new Size(950, 650);
      StartPosition = FormStartPosition.CenterScreen;
      BackColor = Color.WhiteSmoke;

      // --- МЕНЮ (MenuStrip) ---
      MenuStrip mainMenu = new MenuStrip { BackColor = Color.White };
      ToolStripMenuItem manageMenu = new ToolStripMenuItem("Управление");

      ToolStripMenuItem calcMenu = new ToolStripMenuItem("Рассчитать сразу");
      ToolStripMenuItem stepMenu = new ToolStripMenuItem("Шаг вперед (Пошагово)");
      ToolStripMenuItem clearMenu = new ToolStripMenuItem("Очистить");
      ToolStripMenuItem exitMenu = new ToolStripMenuItem("Выход");

      calcMenu.Click += CalcMenu_Click;
      stepMenu.Click += StepMenu_Click;
      clearMenu.Click += ClearMenu_Click;
      exitMenu.Click += (s, e) => Close();

      manageMenu.DropDownItems.AddRange(new ToolStripItem[] { calcMenu, stepMenu, clearMenu, exitMenu });
      mainMenu.Items.Add(manageMenu);
      MainMenuStrip = mainMenu;

      // --- ПАНЕЛЬ ВВОДА ---
      Panel inputPanel = new Panel { Dock = DockStyle.Top, Height = 100 };

      inputPanel.Controls.Add(new Label { Text = "a:", Location = new Point(20, 45), AutoSize = true, Font = new Font("Segoe UI", 12) });
      txtA = new TextBox { Location = new Point(45, 45), Width = 70, Text = "0" };
      inputPanel.Controls.Add(txtA);

      inputPanel.Controls.Add(new Label { Text = "b:", Location = new Point(130, 45), AutoSize = true, Font = new Font("Segoe UI", 12) });
      txtB = new TextBox { Location = new Point(155, 45), Width = 70, Text = "5" };
      inputPanel.Controls.Add(txtB);

      inputPanel.Controls.Add(new Label { Text = "e:", Location = new Point(240, 45), AutoSize = true, Font = new Font("Segoe UI", 12) });
      txtE = new TextBox { Location = new Point(265, 45), Width = 90, Text = "0,001" };
      inputPanel.Controls.Add(txtE);

      inputPanel.Controls.Add(new Label { Text = "f(x):", Location = new Point(370, 45), AutoSize = true, Font = new Font("Segoe UI", 12) });
      txtFunc = new TextBox { Location = new Point(415, 45), Width = 250, Text = "x^2 - 4*x + 3" };
      inputPanel.Controls.Add(txtFunc);

      // --- РЕЗУЛЬТАТ ---
      lblResult = new Label {
        Dock = DockStyle.Bottom,
        Height = 80,
        Font = new Font("Arial", 11f, FontStyle.Bold),
        ForeColor = Color.DarkGreen,
        Padding = new Padding(10, 5, 0, 0),
        Text = "Минимум:\nf(x) =\nИтераций:"
      };

      // --- ГРАФИК ---
      chart1 = new Chart { Dock = DockStyle.Fill };
      chart1.Legends.Add(new Legend("legend") { Docking = Docking.Top });

      chart1.Series.Add(new Series("Function") { ChartType = SeriesChartType.Line, BorderWidth = 3, Color = Color.Blue, LegendText = "f(x)" });
      chart1.Series.Add(new Series("MinPoint") { ChartType = SeriesChartType.Point, MarkerSize = 12, MarkerStyle = MarkerStyle.Circle, Color = Color.Red, LegendText = "Текущая точка" });

      chart1.ChartAreas.Add("Area1");
      chart1.ChartAreas[0].AxisX.Crossing = 0;
      chart1.ChartAreas[0].AxisY.Crossing = 0;

      Controls.Add(chart1);
      Controls.Add(lblResult);
      Controls.Add(inputPanel);
      Controls.Add(mainMenu);
    }

    private void task4_Load(object sender, EventArgs e) { } // Заглушка, чтобы не ломался Designer

    // =====================================================================
    // ОБРАБОТКА ВВОДА И ВАЛИДАЦИЯ (Отказоустойчивость)
    // =====================================================================
    private bool ValidateInput(out double a, out double b, out double eps, out MathFunction fn) {
      a = b = eps = 0; fn = null;
      try {
        if (!TryParseDouble(txtA.Text, out a)) throw new Exception("Параметр 'a' должен быть числом.");
        if (!TryParseDouble(txtB.Text, out b)) throw new Exception("Параметр 'b' должен быть числом.");
        if (!TryParseDouble(txtE.Text, out eps) || eps <= 0) throw new Exception("Точность 'e' должна быть числом больше 0.");
        if (a >= b) throw new Exception("Начало отрезка 'a' должно быть меньше 'b'.");

        if (string.IsNullOrWhiteSpace(txtFunc.Text)) throw new Exception("Введите формулу функции.");

        // Используем УЖЕ СУЩЕСТВУЮЩИЙ парсер из task2 / task3
        fn = FormulaParser.Compile(txtFunc.Text);
        return true;
      }
      catch (Exception ex) {
        MessageBox.Show(ex.Message, "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return false;
      }
    }

    // =====================================================================
    // КНОПКА: РАССЧИТАТЬ СРАЗУ
    // =====================================================================
    private void CalcMenu_Click(object sender, EventArgs e) {
      isStepping = false;
      if (!ValidateInput(out double a, out double b, out double eps, out MathFunction fn)) return;

      PlotGraph(fn, a, b);

      double x = (a + b) / 2.0; // Стартуем из середины
      int iterations = 0;
      double h = 1e-5;

      while (iterations < 1000) {
        iterations++;
        double df = (fn(x + h) - fn(x - h)) / (2 * h); // Первая производная
        double ddf = (fn(x + h) - 2 * fn(x) + fn(x - h)) / (h * h); // Вторая производная

        if (Math.Abs(ddf) < 1e-12) {
          MessageBox.Show("Вторая производная близка к нулю. Метод расходится.", "Ошибка метода");
          return;
        }

        double nextX = x - df / ddf; // Метод Ньютона
        if (Math.Abs(nextX - x) < eps) { x = nextX; break; }
        x = nextX;
      }

      UpdateResult(x, fn(x), iterations);
      DrawPoint(x, fn(x));
    }

    // =====================================================================
    // КНОПКА: ШАГ ВПЕРЕД (ПОШАГОВЫЙ РЕЖИМ)
    // =====================================================================
    private void StepMenu_Click(object sender, EventArgs e) {
      if (!isStepping) {
        if (!ValidateInput(out double a, out double b, out double eps, out MathFunction fn)) return;
        currentFn = fn; currentEps = eps;
        currentX = (a + b) / 2.0;
        currentStep = 0; isStepping = true;
        PlotGraph(currentFn, a, b);
      }

      currentStep++;
      double h = 1e-5;
      double df = (currentFn(currentX + h) - currentFn(currentX - h)) / (2 * h);
      double ddf = (currentFn(currentX + h) - 2 * currentFn(currentX) + currentFn(currentX - h)) / (h * h);

      if (Math.Abs(ddf) < 1e-12) {
        MessageBox.Show("Вторая производная равна нулю. Остановка.", "Информация");
        isStepping = false; return;
      }

      double nextX = currentX - df / ddf;
      DrawPoint(nextX, currentFn(nextX));
      lblResult.Text = $"ШАГ {currentStep} | Текущий x = {nextX:F5}\nf(x) = {currentFn(nextX):F5}\nСмещение: {Math.Abs(nextX - currentX):F6}";

      if (Math.Abs(nextX - currentX) < currentEps) {
        MessageBox.Show($"Точность достигнута за {currentStep} итераций!", "Поиск завершен");
        UpdateResult(nextX, currentFn(nextX), currentStep);
        isStepping = false;
      }
      else {
        currentX = nextX;
      }
    }

    private void ClearMenu_Click(object sender, EventArgs e) {
      txtA.Clear(); txtB.Clear(); txtE.Clear(); txtFunc.Clear();
      isStepping = false;
      chart1.Series["Function"].Points.Clear();
      chart1.Series["MinPoint"].Points.Clear();
      lblResult.Text = "Минимум:\nf(x) =\nИтераций:";
    }

    private void PlotGraph(MathFunction fn, double a, double b) {
      chart1.Series["Function"].Points.Clear();
      chart1.Series["MinPoint"].Points.Clear();

      double minX = a - (b - a) * 0.1, maxX = b + (b - a) * 0.1;
      chart1.ChartAreas[0].AxisX.Minimum = Math.Round(minX, 3);
      chart1.ChartAreas[0].AxisX.Maximum = Math.Round(maxX, 3);

      for (double x = minX; x <= maxX; x += (maxX - minX) / 500.0) {
        try {
          double y = fn(x);
          if (!double.IsNaN(y) && !double.IsInfinity(y) && Math.Abs(y) < 10000) chart1.Series["Function"].Points.AddXY(x, y);
        }
        catch { }
      }
      chart1.ChartAreas[0].RecalculateAxesScale();
    }

    private void DrawPoint(double x, double y) {
      chart1.Series["MinPoint"].Points.Clear();
      chart1.Series["MinPoint"].Points.AddXY(x, y);
    }

    private void UpdateResult(double x, double y, int iters) {
      lblResult.Text = $"Минимум: x = {x:F5}\nf(x) = {y:F5}\nИтераций: {iters}";
    }

    private static bool TryParseDouble(string text, out double number) {
      number = 0;
      if (string.IsNullOrWhiteSpace(text)) return false;
      return double.TryParse(text.Trim().Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out number);
    }
  }
}