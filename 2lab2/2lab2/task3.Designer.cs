namespace _2lab2 {
  partial class task3 {
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Код, автоматически созданный конструктором форм Windows

    private void InitializeComponent() {
      System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
      System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
      System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
      System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
      this.mainMenu = new System.Windows.Forms.MenuStrip();
      this.calculateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.plotGraphToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.inputPanel = new System.Windows.Forms.Panel();
      this.lblA = new System.Windows.Forms.Label();
      this.txtA = new System.Windows.Forms.TextBox();
      this.lblB = new System.Windows.Forms.Label();
      this.txtB = new System.Windows.Forms.TextBox();
      this.lblE = new System.Windows.Forms.Label();
      this.txtE = new System.Windows.Forms.TextBox();
      this.lblFunc = new System.Windows.Forms.Label();
      this.txtFunc = new System.Windows.Forms.TextBox();
      this.labelHint = new System.Windows.Forms.Label();
      this.lblResult = new System.Windows.Forms.Label();
      this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
      this.mainMenu.SuspendLayout();
      this.inputPanel.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
      this.SuspendLayout();
      // 
      // mainMenu
      // 
      this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.calculateToolStripMenuItem,
            this.plotGraphToolStripMenuItem,
            this.clearToolStripMenuItem,
            this.exitToolStripMenuItem});
      this.mainMenu.Location = new System.Drawing.Point(0, 0);
      this.mainMenu.Name = "mainMenu";
      this.mainMenu.Size = new System.Drawing.Size(900, 24);
      this.mainMenu.TabIndex = 0;
      this.mainMenu.Text = "mainMenu";
      // 
      // calculateToolStripMenuItem
      // 
      this.calculateToolStripMenuItem.Name = "calculateToolStripMenuItem";
      this.calculateToolStripMenuItem.Size = new System.Drawing.Size(79, 20);
      this.calculateToolStripMenuItem.Text = "Рассчитать";
      this.calculateToolStripMenuItem.Click += new System.EventHandler(this.calculateToolStripMenuItem_Click);
      // 
      // plotGraphToolStripMenuItem
      // 
      this.plotGraphToolStripMenuItem.Name = "plotGraphToolStripMenuItem";
      this.plotGraphToolStripMenuItem.Size = new System.Drawing.Size(118, 20);
      this.plotGraphToolStripMenuItem.Text = "Построить график";
      this.plotGraphToolStripMenuItem.Click += new System.EventHandler(this.plotGraphToolStripMenuItem_Click);
      // 
      // clearToolStripMenuItem
      // 
      this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
      this.clearToolStripMenuItem.Size = new System.Drawing.Size(71, 20);
      this.clearToolStripMenuItem.Text = "Очистить";
      this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
      // 
      // exitToolStripMenuItem
      // 
      this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
      this.exitToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
      this.exitToolStripMenuItem.Text = "Выход";
      this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
      // 
      // inputPanel
      // 
      this.inputPanel.Controls.Add(this.lblA);
      this.inputPanel.Controls.Add(this.txtA);
      this.inputPanel.Controls.Add(this.lblB);
      this.inputPanel.Controls.Add(this.txtB);
      this.inputPanel.Controls.Add(this.lblE);
      this.inputPanel.Controls.Add(this.txtE);
      this.inputPanel.Controls.Add(this.lblFunc);
      this.inputPanel.Controls.Add(this.txtFunc);
      this.inputPanel.Controls.Add(this.labelHint);
      this.inputPanel.Dock = System.Windows.Forms.DockStyle.Top;
      this.inputPanel.Location = new System.Drawing.Point(0, 24);
      this.inputPanel.Name = "inputPanel";
      this.inputPanel.Size = new System.Drawing.Size(900, 100);
      this.inputPanel.TabIndex = 1;
      // 
      // lblA
      // 
      this.lblA.AutoSize = true;
      this.lblA.Location = new System.Drawing.Point(15, 18);
      this.lblA.Name = "lblA";
      this.lblA.Size = new System.Drawing.Size(16, 13);
      this.lblA.TabIndex = 0;
      this.lblA.Text = "a:";
      // 
      // txtA
      // 
      this.txtA.Location = new System.Drawing.Point(39, 15);
      this.txtA.Name = "txtA";
      this.txtA.Size = new System.Drawing.Size(90, 20);
      this.txtA.TabIndex = 1;
      this.txtA.Text = "0";
      // 
      // lblB
      // 
      this.lblB.AutoSize = true;
      this.lblB.Location = new System.Drawing.Point(165, 18);
      this.lblB.Name = "lblB";
      this.lblB.Size = new System.Drawing.Size(16, 13);
      this.lblB.TabIndex = 2;
      this.lblB.Text = "b:";
      // 
      // txtB
      // 
      this.txtB.Location = new System.Drawing.Point(189, 15);
      this.txtB.Name = "txtB";
      this.txtB.Size = new System.Drawing.Size(90, 20);
      this.txtB.TabIndex = 3;
      this.txtB.Text = "5";
      // 
      // lblE
      // 
      this.lblE.AutoSize = true;
      this.lblE.Location = new System.Drawing.Point(315, 18);
      this.lblE.Name = "lblE";
      this.lblE.Size = new System.Drawing.Size(16, 13);
      this.lblE.TabIndex = 4;
      this.lblE.Text = "e:";
      // 
      // txtE
      // 
      this.txtE.Location = new System.Drawing.Point(339, 15);
      this.txtE.Name = "txtE";
      this.txtE.Size = new System.Drawing.Size(90, 20);
      this.txtE.TabIndex = 5;
      this.txtE.Text = "3";
      // 
      // lblFunc
      // 
      this.lblFunc.AutoSize = true;
      this.lblFunc.Location = new System.Drawing.Point(15, 58);
      this.lblFunc.Name = "lblFunc";
      this.lblFunc.Size = new System.Drawing.Size(24, 13);
      this.lblFunc.TabIndex = 6;
      this.lblFunc.Text = "f(x):";
      // 
      // txtFunc
      // 
      this.txtFunc.Location = new System.Drawing.Point(47, 55);
      this.txtFunc.Name = "txtFunc";
      this.txtFunc.Size = new System.Drawing.Size(520, 20);
      this.txtFunc.TabIndex = 7;
      this.txtFunc.Text = "Tan(x)";
      // 
      // labelHint
      // 
      this.labelHint.AutoSize = true;
      this.labelHint.ForeColor = System.Drawing.Color.DimGray;
      this.labelHint.Location = new System.Drawing.Point(580, 15);
      this.labelHint.Name = "labelHint";
      this.labelHint.Size = new System.Drawing.Size(270, 26);
      this.labelHint.TabIndex = 8;
      this.labelHint.Text = "Допустимо: + - * / ^, скобки, x, pi, e,\r\nsin cos tan asin acos atan exp ln log sqrt abs";
      // 
      // lblResult
      // 
      this.lblResult.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.lblResult.Font = new System.Drawing.Font("Arial", 10.5F, System.Drawing.FontStyle.Bold);
      this.lblResult.ForeColor = System.Drawing.Color.DarkGreen;
      this.lblResult.Location = new System.Drawing.Point(0, 575);
      this.lblResult.Name = "lblResult";
      this.lblResult.Padding = new System.Windows.Forms.Padding(12, 6, 0, 0);
      this.lblResult.Size = new System.Drawing.Size(900, 75);
      this.lblResult.TabIndex = 2;
      this.lblResult.Text = "Корень уравнения: —\r\nf(x) = —\r\nИтераций: —";
      this.lblResult.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // chart1
      // 
      chartArea1.Name = "Area1";
      this.chart1.ChartAreas.Add(chartArea1);
      this.chart1.Dock = System.Windows.Forms.DockStyle.Fill;
      legend1.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
      legend1.Name = "legend";
      this.chart1.Legends.Add(legend1);
      this.chart1.Location = new System.Drawing.Point(0, 124);
      this.chart1.Name = "chart1";
      series1.BorderWidth = 3;
      series1.ChartArea = "Area1";
      series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
      series1.Color = System.Drawing.Color.Blue;
      series1.Legend = "legend";
      series1.LegendText = "f(x)";
      series1.Name = "Function";
      series2.ChartArea = "Area1";
      series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
      series2.Color = System.Drawing.Color.Red;
      series2.Legend = "legend";
      series2.LegendText = "Корень уравнения";
      series2.MarkerSize = 12;
      series2.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
      series2.Name = "MinPoint";
      this.chart1.Series.Add(series1);
      this.chart1.Series.Add(series2);
      this.chart1.Size = new System.Drawing.Size(900, 451);
      this.chart1.TabIndex = 3;
      this.chart1.Text = "chart1";
      // 
      // task3
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(900, 650);
      this.Controls.Add(this.chart1);
      this.Controls.Add(this.lblResult);
      this.Controls.Add(this.inputPanel);
      this.Controls.Add(this.mainMenu);
      this.MainMenuStrip = this.mainMenu;
      this.MinimumSize = new System.Drawing.Size(700, 500);
      this.Name = "task3";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Задание 3 — Метод золотого сечения";
      this.Load += new System.EventHandler(this.task3_Load);
      this.mainMenu.ResumeLayout(false);
      this.mainMenu.PerformLayout();
      this.inputPanel.ResumeLayout(false);
      this.inputPanel.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    #endregion

    private System.Windows.Forms.MenuStrip mainMenu;
    private System.Windows.Forms.ToolStripMenuItem calculateToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem plotGraphToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
    private System.Windows.Forms.Panel inputPanel;
    private System.Windows.Forms.Label lblA;
    private System.Windows.Forms.TextBox txtA;
    private System.Windows.Forms.Label lblB;
    private System.Windows.Forms.TextBox txtB;
    private System.Windows.Forms.Label lblE;
    private System.Windows.Forms.TextBox txtE;
    private System.Windows.Forms.Label lblFunc;
    private System.Windows.Forms.TextBox txtFunc;
    private System.Windows.Forms.Label labelHint;
    private System.Windows.Forms.Label lblResult;
    private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
  }
}