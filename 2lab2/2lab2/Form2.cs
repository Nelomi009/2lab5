using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Xml.Linq;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _2lab2 {
  public partial class Form2 : Form {
    private CancellationTokenSource _cts;
    private bool _isSortingRunning = false;


    public Form2() {
      InitializeComponent();
      dataGridView1.AutoGenerateColumns = false;
    }

    // 1. СНЯТИЕ ДАННЫХ И ВАЛИДАЦИЯ
    private List<int> GetInputData() {
      List<int> data = new List<int>();
      foreach (DataGridViewRow row in dataGridView1.Rows) {
        if (row.IsNewRow) continue;
        var cell = row.Cells[0].Value;
        if (cell != null && int.TryParse(cell.ToString().Trim(), out int val)) {
          data.Add(val);
        }
        else if (cell != null && !string.IsNullOrWhiteSpace(cell.ToString())) {
          MessageBox.Show($"Некорректное значение: '{cell}'. Вводите только целые числа!",
              "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Warning);
          return null;
        }
      }
      return data;
    }

    // 2. ОТРИСОВКА СТОЛБИКОВ В PICTUREBOX
    private void DrawArray(PictureBox pb, int[] arr, int activeIdx1 = -1, int activeIdx2 = -1) {
      if (arr == null || arr.Length == 0 || pb.Width <= 0 || pb.Height <= 0) return;

      Bitmap bmp = new Bitmap(pb.Width, pb.Height);
      using (Graphics g = Graphics.FromImage(bmp)) {
        g.Clear(Color.White);
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        float barWidth = (float)pb.Width / arr.Length;
        int maxVal = arr.Max();
        if (maxVal <= 0) maxVal = 1;

        // Резервируем верхние 24 пикселя строго под цифры
        float topMargin = 22f;
        float usableHeight = pb.Height - topMargin - 4f;

        using (Font font = new Font("Arial", 8f, FontStyle.Bold))
        using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center }) {
          for (int i = 0; i < arr.Length; i++) {
            float barHeight = ((float)arr[i] / maxVal) * usableHeight;
            Brush brush = Brushes.SteelBlue;
            if (i == activeIdx1 || i == activeIdx2) brush = Brushes.Red;

            float x = i * barWidth;
            float y = pb.Height - barHeight;

            // Столбик
            g.FillRectangle(brush, x + 1, y, Math.Max(1, barWidth - 2), barHeight);

            // Число рисуем ровно над столбиком, но не выше 2 пикселей от края PictureBox
            if (arr.Length <= 25) {
              float textY = Math.Max(2f, y - 16f);
              g.DrawString(arr[i].ToString(), font, Brushes.Black, x + (barWidth / 2f), textY, sf);
            }
          }
        }
      }

      pb.Invoke(new Action(() => {
        pb.Image?.Dispose();
        pb.Image = bmp;
      }));
    }

    // 3. АЛГОРИТМЫ СОРТИРОВОК
    // Каждый метод возвращает (время в мс, количество операций = сравнения + обмены/присваивания)

    // 1. Пузырьковая
    private async Task<(long ms, long ops)> BubbleSortAsync(int[] arr, PictureBox pb, bool asc, CancellationToken token) {
      Stopwatch sw = Stopwatch.StartNew();
      long cmp = 0, swp = 0;
      int n = arr.Length;
      for (int i = 0; i < n - 1; i++) {
        for (int j = 0; j < n - i - 1; j++) {
          token.ThrowIfCancellationRequested();
          cmp++;
          bool needSwap = asc ? arr[j] > arr[j + 1] : arr[j] < arr[j + 1];
          if (needSwap) {
            (arr[j], arr[j + 1]) = (arr[j + 1], arr[j]);
            swp++;
          }
          DrawArray(pb, arr, j, j + 1);
          await Task.Delay(10, token);
        }
      }
      sw.Stop();
      DrawArray(pb, arr);
      return (sw.ElapsedMilliseconds, cmp + swp);
    }

    // 2. Вставками
    private async Task<(long ms, long ops)> InsertionSortAsync(int[] arr, PictureBox pb, bool asc, CancellationToken token) {
      Stopwatch sw = Stopwatch.StartNew();
      long cmp = 0, swp = 0;
      for (int i = 1; i < arr.Length; i++) {
        int key = arr[i];
        int j = i - 1;
        while (j >= 0) {
          token.ThrowIfCancellationRequested();
          cmp++;
          if (!(asc ? arr[j] > key : arr[j] < key)) break;
          arr[j + 1] = arr[j];   // сдвиг элемента
          swp++;
          j--;
          DrawArray(pb, arr, j, i);
          await Task.Delay(10, token);
        }
        arr[j + 1] = key;        // вставка ключа
        swp++;
      }
      sw.Stop();
      DrawArray(pb, arr);
      return (sw.ElapsedMilliseconds, cmp + swp);
    }

    // 3. Шейкерная
    private async Task<(long ms, long ops)> ShakerSortAsync(int[] arr, PictureBox pb, bool asc, CancellationToken token) {
      Stopwatch sw = Stopwatch.StartNew();
      long cmp = 0, swp = 0;
      int left = 0, right = arr.Length - 1;
      while (left < right) {
        for (int i = left; i < right; i++) {
          token.ThrowIfCancellationRequested();
          cmp++;
          bool needSwap = asc ? arr[i] > arr[i + 1] : arr[i] < arr[i + 1];
          if (needSwap) { (arr[i], arr[i + 1]) = (arr[i + 1], arr[i]); swp++; }
          DrawArray(pb, arr, i, i + 1);
          await Task.Delay(10, token);
        }
        right--;

        for (int i = right; i > left; i--) {
          token.ThrowIfCancellationRequested();
          cmp++;
          bool needSwap = asc ? arr[i - 1] > arr[i] : arr[i - 1] < arr[i];
          if (needSwap) { (arr[i - 1], arr[i]) = (arr[i], arr[i - 1]); swp++; }
          DrawArray(pb, arr, i - 1, i);
          await Task.Delay(10, token);
        }
        left++;
      }
      sw.Stop();
      DrawArray(pb, arr);
      return (sw.ElapsedMilliseconds, cmp + swp);
    }

    // 4. Быстрая (QuickSort)
    private async Task<(long ms, long ops)> QuickSortAsync(int[] arr, PictureBox pb, bool asc, CancellationToken token) {
      Stopwatch sw = Stopwatch.StartNew();
      long cmp = 0, swp = 0;

      async Task SortRange(int left, int right) {
        if (left >= right) return;
        int pivot = arr[(left + right) / 2];
        int i = left, j = right;

        while (i <= j) {
          token.ThrowIfCancellationRequested();
          while (true) { cmp++; if (!(asc ? arr[i] < pivot : arr[i] > pivot)) break; i++; }
          while (true) { cmp++; if (!(asc ? arr[j] > pivot : arr[j] < pivot)) break; j--; }

          if (i <= j) {
            (arr[i], arr[j]) = (arr[j], arr[i]);
            swp++;
            DrawArray(pb, arr, i, j);
            await Task.Delay(15, token);
            i++;
            j--;
          }
        }
        await SortRange(left, j);
        await SortRange(i, right);
      }

      await SortRange(0, arr.Length - 1);
      sw.Stop();
      DrawArray(pb, arr);
      return (sw.ElapsedMilliseconds, cmp + swp);
    }

    // 5. BOGO Sort
    private async Task<(long ms, long ops)> BogoSortAsync(int[] arr, PictureBox pb, bool asc, CancellationToken token) {
      Stopwatch sw = Stopwatch.StartNew();
      long cmp = 0, swp = 0;
      Random rnd = new Random();

      bool IsSorted() {
        for (int i = 0; i < arr.Length - 1; i++) {
          cmp++;
          if (asc ? arr[i] > arr[i + 1] : arr[i] < arr[i + 1]) return false;
        }
        return true;
      }

      while (!IsSorted()) {
        token.ThrowIfCancellationRequested();
        for (int i = arr.Length - 1; i > 0; i--) {
          int j = rnd.Next(i + 1);
          (arr[i], arr[j]) = (arr[j], arr[i]);
          swp++;
        }
        DrawArray(pb, arr);
        await Task.Delay(25, token);
      }

      sw.Stop();
      DrawArray(pb, arr);
      return (sw.ElapsedMilliseconds, cmp + swp);
    }

    // 4. ОБРАБОТЧИКИ МЕНЮ (MenuStrip)

    // Запуск сортировок
    private async void menuStartSort_Click(object sender, EventArgs e) {
      if (_isSortingRunning) {
        MessageBox.Show("Сортировка уже запущена!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Information);
        return;
      }

      var rawData = GetInputData();
      if (rawData == null || rawData.Count == 0) {
        MessageBox.Show("Заполните таблицу данными!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Information);
        return;
      }

      if (chkBogo.Checked && rawData.Count > 7) {
        MessageBox.Show("Для BOGO-сортировки рекомендуется не более 7 элементов, иначе она будет выполняться слишком долго!",
            "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }

      _cts = new CancellationTokenSource();
      bool asc = radioButton2.Checked;
      List<Task> tasks = new List<Task>();

      bool hasWinner = false;
      long bestMs = 0, bestOps = 0;
      object winnerLock = new object();

      lblFastest.Text = "Самый быстрый алгоритм: —";
      _isSortingRunning = true;

      // Победитель: меньше время; при равенстве времени - меньше операций
      void CheckAndSetWinner(string name, long timeMs, long ops) {
        lock (winnerLock) {
          if (!hasWinner || timeMs < bestMs || (timeMs == bestMs && ops < bestOps)) {
            hasWinner = true;
            bestMs = timeMs;
            bestOps = ops;
            Invoke(new Action(() => {
              lblFastest.Text = $"Самый быстрый алгоритм: {name} ({timeMs} мс, операций: {ops})";
            }));
          }
        }
      }

      try {
        if (chkBubble.Checked) {
          int[] arr = rawData.ToArray();
          tasks.Add(Task.Run(async () => {
            var r = await BubbleSortAsync(arr, pbBubble, asc, _cts.Token);
            Invoke(new Action(() => lblBubbleTime.Text = $"Пузырьковая: {r.ms} мс\nОпераций: {r.ops}"));
            CheckAndSetWinner("Пузырьковая", r.ms, r.ops);
          }));
        }

        if (chkInsertion.Checked) {
          int[] arr = rawData.ToArray();
          tasks.Add(Task.Run(async () => {
            var r = await InsertionSortAsync(arr, pbInsertion, asc, _cts.Token);
            Invoke(new Action(() => lblInsertionTime.Text = $"Вставками: {r.ms} мс\nОпераций: {r.ops}"));
            CheckAndSetWinner("Вставками", r.ms, r.ops);
          }));
        }

        if (chkShaker.Checked) {
          int[] arr = rawData.ToArray();
          tasks.Add(Task.Run(async () => {
            var r = await ShakerSortAsync(arr, pbShaker, asc, _cts.Token);
            Invoke(new Action(() => lblShakerTime.Text = $"Шейкерная: {r.ms} мс\nОпераций: {r.ops}"));
            CheckAndSetWinner("Шейкерная", r.ms, r.ops);
          }));
        }

        if (chkQuick.Checked) {
          int[] arr = rawData.ToArray();
          tasks.Add(Task.Run(async () => {
            var r = await QuickSortAsync(arr, pbQuick, asc, _cts.Token);
            Invoke(new Action(() => lblQuickTime.Text = $"Быстрая: {r.ms} мс\nОпераций: {r.ops}"));
            CheckAndSetWinner("Быстрая", r.ms, r.ops);
          }));
        }

        if (chkBogo.Checked) {
          int[] arr = rawData.ToArray();
          tasks.Add(Task.Run(async () => {
            var r = await BogoSortAsync(arr, pbBogo, asc, _cts.Token);
            Invoke(new Action(() => lblBogoTime.Text = $"BOGO: {r.ms} мс\nОпераций: {r.ops}"));
            CheckAndSetWinner("BOGO", r.ms, r.ops);
          }));
        }

        await Task.WhenAll(tasks);
      }
      catch (OperationCanceledException) {
        // Сортировка прервана
      }
      finally {
        _isSortingRunning = false;
      }
    }

    // Остановка сортировки
    private void menuStopSort_Click(object sender, EventArgs e) {
      _cts?.Cancel();
      _isSortingRunning = false;
    }

    // Генерация случайных чисел
    private void menuGenerate_Click(object sender, EventArgs e) {
      if (_isSortingRunning) {
        MessageBox.Show("Сначала остановите сортировку, затем меняйте данные!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
      }

      dataGridView1.Rows.Clear();
      dataGridView1.DefaultCellStyle.ForeColor = Color.Black;
      dataGridView1.DefaultCellStyle.BackColor = Color.White;

      Random rnd = new Random();
      int count = 15;

      for (int i = 0; i < count; i++) {
        int rowIndex = dataGridView1.Rows.Add();
        dataGridView1.Rows[rowIndex].Cells[0].Value = rnd.Next(10, 100).ToString();
      }

      dataGridView1.ClearSelection();
    }

    // Чтение чисел из первого листа .xlsx (без сторонних библиотек)
    private List<int> ReadXlsx(string path) {
      List<int> result = new List<int>();
      XNamespace ns = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";

      using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
      using (ZipArchive zip = new ZipArchive(fs, ZipArchiveMode.Read)) {
        // Общие строки (текстовые ячейки)
        List<string> shared = new List<string>();
        ZipArchiveEntry ssEntry = zip.GetEntry("xl/sharedStrings.xml");
        if (ssEntry != null) {
          using (Stream st = ssEntry.Open()) {
            XDocument doc = XDocument.Load(st);
            foreach (XElement si in doc.Descendants(ns + "si"))
              shared.Add(string.Concat(si.Descendants(ns + "t").Select(t => t.Value)));
          }
        }

        // Первый лист
        ZipArchiveEntry sheet = zip.GetEntry("xl/worksheets/sheet1.xml");
        if (sheet == null)
          sheet = zip.Entries.FirstOrDefault(en => en.FullName.StartsWith("xl/worksheets/sheet") && en.FullName.EndsWith(".xml"));
        if (sheet == null) return result;

        using (Stream st = sheet.Open()) {
          XDocument doc = XDocument.Load(st);
          foreach (XElement c in doc.Descendants(ns + "c")) {
            string type = (string)c.Attribute("t");
            string text = null;

            if (type == "s") {
              string v = (string)c.Element(ns + "v");
              if (int.TryParse(v, out int idx) && idx >= 0 && idx < shared.Count) text = shared[idx];
            }
            else if (type == "inlineStr") {
              XElement isEl = c.Element(ns + "is");
              if (isEl != null) text = string.Concat(isEl.Descendants(ns + "t").Select(t => t.Value));
            }
            else {
              text = (string)c.Element(ns + "v");
            }

            if (string.IsNullOrWhiteSpace(text)) continue;
            text = text.Trim().Replace(',', '.');
            if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out double d))
              result.Add((int)Math.Round(d));
          }
        }
      }
      return result;
    }

    // Загрузка из файла
    private void menuLoadFile_Click(object sender, EventArgs e) {
      if (_isSortingRunning) {
        MessageBox.Show("Сначала остановите сортировку, затем загружайте данные!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
      }

      using (OpenFileDialog ofd = new OpenFileDialog()) {
        ofd.Filter = "Excel, CSV и текстовые файлы (*.xlsx;*.csv;*.txt)|*.xlsx;*.csv;*.txt|Excel (*.xlsx)|*.xlsx|Текстовые и CSV файлы (*.csv;*.txt)|*.csv;*.txt|Все файлы (*.*)|*.*";
        if (ofd.ShowDialog() == DialogResult.OK) {
          try {
            List<int> values = new List<int>();

            if (Path.GetExtension(ofd.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase)) {
              values = ReadXlsx(ofd.FileName);
            }
            else {
              string[] lines = File.ReadAllLines(ofd.FileName);
              foreach (string line in lines) {
                string[] parts = line.Split(new char[] { ',', ';', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string p in parts) {
                  if (int.TryParse(p.Trim(), out int val)) values.Add(val);
                }
              }
            }

            if (values.Count == 0) {
              MessageBox.Show("В файле не найдено целых чисел.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Information);
              return;
            }

            dataGridView1.Rows.Clear();
            foreach (int val in values) {
              int rowIndex = dataGridView1.Rows.Add();
              dataGridView1.Rows[rowIndex].Cells[0].Value = val.ToString();
            }
            dataGridView1.ClearSelection();
          }
          catch (Exception ex) {
            MessageBox.Show($"Ошибка чтения файла: {ex.Message}");
          }
        }
      }
    }

    // Очистить
    private void menuClear_Click(object sender, EventArgs e) {
      if (_isSortingRunning) {
        MessageBox.Show("Сначала остановите сортировку перед очисткой!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
      }

      // Очистка таблицы
      dataGridView1.Rows.Clear();

      // Очистка PictureBox
      pbBubble.Image = null;
      pbInsertion.Image = null;
      pbShaker.Image = null;
      pbQuick.Image = null;
      pbBogo.Image = null;

      // Очистка надписей с миллисекундами и операциями
      lblBubbleTime.Text = "Пузырьковая: —";
      lblInsertionTime.Text = "Вставками: —";
      lblShakerTime.Text = "Шейкерная: —";
      lblQuickTime.Text = "Быстрая: —";
      lblBogoTime.Text = "BOGO: —";
      lblFastest.Text = "Самый быстрый алгоритм: —";
    }

    // Выход
    private void menuExit_Click(object sender, EventArgs e) {
      _cts?.Cancel();
      this.Close();
    }

    private void Form2_Load(object sender, EventArgs e) {

    }

    private void Form2_Load_1(object sender, EventArgs e) {

    }

    private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) {

    }
  }
}