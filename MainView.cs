using Accessibility;
using Microsoft.VisualBasic.ApplicationServices;
using ScottPlot;
using ScottPlot.Plottable;
using System.ComponentModel;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;
using System.Net.Http.Headers;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace test3
{
    public partial class MainView : Form
    {
        // Объявление полей 
        // Forms
        private ColorSettingsView f3;
        private VibrationSettingsView f4;
        private CreatingGraphsView f5;
        private lineWidthSettingView lineWidthSettingView;
        private eraserWidthSettingView eraserWidthSettingView;
        // bool
        private bool isDragging; // Для drag-and-drop
        private bool isFileOpened = false; // Был ли до этого открыт файл (для корректной работы open file button)
        private bool isCntrlPressed = false; // Для crosshair
        public bool isDate = true; // Для определения оси (дата или рекорд номер)
        public bool isF3Opened = false; // Открыта ли форма с изменеием цветов
        public bool isF4Opened = false; // Открыта ли форма с настройками уровней вибраций
        public bool isF5Opened = false; // Открыта ли форма создания нового графика
        public bool isLineWidthSettingViewOpen = false;
        public bool isEraserWidthSettinViewOpen = false;
        public bool isFakePointsEnabled = false;
        public bool isFileBroken = false;
        public bool cantReadFile = false;
        public bool isMultiCH, isGraphFixed, isLightTheme, isFastDraw, isLegenedHidden; // Булевые переменные для сохранение настроек контекстного меню
        private bool isVMMDrawed = false;
        private bool isToolPanelVisible = false;
        private bool isMagnet, isManyPoints, isDefaultRightClickMenu, isLineMagnet;
        private bool isSecondClick = false;
        // Числовые поля
        private Int32 draggIndex = 200; // Индекс взятого label (Для drag-and-drop)
        private byte graphsCount; // Сколько графиков на данный момент на форме
        private byte fileTypeCode; // Тип файла в виде числа
        private int rowCount; // Число строк в файле (для прогресс бара)
        private int comboIndex; // Индекс выбранного файла в comboBox
        private int permissibleVibrationLevel, highVibrationLevel, criticalVibrationLevel;
        private int toolIndex = 0;
        public float lineWidth = 1;
        public double eraserWidth = 0.1;
        // string
        private string fileName; // путь к файлу
        private readonly string separatorType;
      
        // Массивы данных
        public double[][] yss; // Массив массивов с данными (для оси y)
        public double[] xs; // Массив с данными даты (для графика)        
        private int[] arrayOfFakePointsIndex;
        private DateTime[] dates; // Массив даты (до преобразования)
        private DateTime[] markerDates;
        private double[] recordNumbers;
        public string[] labelsText;
        private List<string> recentlyFilesArray;
        //Массив с объектами класса FileData (данные из каждого файла - один элемент массива)
        FileData[] arrayOfFileData = Array.Empty<FileData>();

        // Координаты
        private Point labelStartLocation; // Координаты при начале движения (Для drag-and-drop)
        private Point[] labelStandartLocations; // Стандартны координаты место дислокации labels. Для того чтобы они возвращались назад после drag-and-drop
        private Point[] panelStandartLocations; // Стандартны координаты место дислокации panels. Для того чтобы они возвращались назад после drag-and-drop

        // Цвета, панели
        public Color[] colors; // Массив с цветами 
        private Panel[] panels; // Массив с панелями (для лейблов)
        public Color specialColor;
        public Color color1;
        public Color lineColor = Color.Red;

        // crosshair
        public readonly ScottPlot.Plottable.Crosshair myCrosshair;

        // Контекстное меню
        private ContextMenuStrip toolContextMenu; // Контекстное меню для элемента "линия" на панели инструментов
        private ToolStripMenuItem lineColorMenuItem, lineWidthMenuItem, lineIsMagnetMenuItem, eraserWidthItem; // Элементы контекстного меню (цвет и толщина линии)
        private ContextMenuStrip contextMenu; // Контексное меню при нажатии на область правой панели 
        private ToolStripMenuItem noLimithighLightPointItem;
        private ToolStripMenuItem clearAllItem, clearPointsItem, clearAllLinesItem;
        private ContextMenuStrip changeColorMenu;
        private ToolStripMenuItem graphSettings, crosshairSettings, vmmSettings;
        private ToolStripMenuItem developersInfo, lightTheme, graphFixed, magnetCH, multiCH, colorSettings, fakePointsSettings, setVibrationLevel, fastDrawVMM, hideLegend, legendSettings, changeColorMenuItem, changeRightClickMenu;
        private List<ToolStripMenuItem> fileItemList;
        // Поля для основной работы программы
        public GraphWindow[] arrayOfGraphWindows; // Массив с окнами графика
        private System.Windows.Forms.ComboBox comboBox1;
        private readonly GraphWindow graphWindow; // Первый график (по умолчанию на форме)
        private System.Windows.Forms.Label[] labels; // Массив с лейблами (для драг энд дропа)
        private System.Windows.Forms.Label backgroundLabel;
        private (double x, double y) mouseCoordinates1, mouseCoordinates2;
        //----------------Переменные для reSize
        private Size formOriginalSize;
        private Rectangle recComboBox1;
        private Rectangle recSplitContainer1;
        private Rectangle recSplitContainerPanel1;
        private Rectangle recSplitContainerPanel2;
        private Rectangle recSelectFile_Button;
        private Rectangle recTextBox;
        private Rectangle recAddGraphWindow_button;
        private Rectangle recProgressBar;

        public MainView()
        {
            Program.f1 = this; // Ссылка на эту форму 
            specialColor = Color.Gray;
            InitializeComponent();
            backgroundLabel = new System.Windows.Forms.Label();
            //if (!isToolPanelVisible)
            //toolStrip1.Visible = false;
            // Пока не доделано лучше отключиь эту функцию
            createToolStripMenuItem.Enabled = false;

            labelsText = Array.Empty<string>();
            recentlyFilesArray = new List<string>();
            fileItemList = new List<ToolStripMenuItem>();
            for (byte i = 0; i < 5; i++)
            {
                ToolStripMenuItem tmp = new(" ");
                tmp.Click += recentFile_Click;
                tmp.Tag = i;
                tmp.Visible = false;
                recentlyFilesArray.Add(" ");
                fileItemList.Add(tmp);
                recentFilesФайлыToolStripMenuItem.DropDownItems.Add(tmp);
            }

            //Создаем комбобокс для VMMLifeTimeData
            comboBox1 = new()
            {
                Location = new Point(16, 10),
                Size = new Size(235, 23),
                MaxDropDownItems = 15,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Visible = false
            };
            comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
            //----------Ресайз----------\\
            Resize += Form1_Resize;
            formOriginalSize = this.Size;
            recSplitContainer1 = new Rectangle(splitContainer1.Location, splitContainer1.Size);
            recSplitContainerPanel1 = new Rectangle(splitContainer1.Panel1.Location, splitContainer1.Panel1.Size);
            recSplitContainerPanel2 = new Rectangle(splitContainer1.Panel2.Location, splitContainer1.Panel2.Size);
            recSelectFile_Button = new Rectangle(selectFile_button.Location, selectFile_button.Size);
            recTextBox = new Rectangle(textBox1.Location, textBox1.Size);
            recAddGraphWindow_button = new Rectangle(add_graphWindow_button.Location, add_graphWindow_button.Size);
            recProgressBar = new Rectangle(progressBar1.Location, progressBar1.Size);
            recComboBox1 = new Rectangle(comboBox1.Location, comboBox1.Size);

            // Инициализация нескольких меню
            toolContextMenu = new();
            contextMenu = new();
            menuStrip1 = new();
            changeColorMenu = new();
            graphSettings = new("Графики");
            crosshairSettings = new("Настройки прицела");
            graphFixed = new("Связанные графики")
            {
                CheckOnClick = true
            };
            multiCH = new("Мульти прицел")
            {
                CheckOnClick = true
            };
            magnetCH = new("Магнит")
            {
                CheckOnClick = true
            };
            developersInfo = new("О разработчиках");
            colorSettings = new("Настройка цветов");
            fakePointsSettings = new("Фейковые точки");
            setVibrationLevel = new("Настройки линий уровней вибраций");
            fastDrawVMM = new("Построенный график при открытии нового интервала")
            {
                CheckOnClick = true
            };
            lightTheme = new("Светлая тема")
            {
                CheckOnClick = true
            };
            vmmSettings = new("Настройки файла VMMLifeTimeData");
            hideLegend = new("Скрыть легенду с графика")
            {
                CheckOnClick = true
            };
            legendSettings = new("Настройки легенды");
            changeColorMenuItem = new("Поменять цвет");
            clearAllItem = new("Очистить все");
            noLimithighLightPointItem = new("Неограниченное кол-во пользовательских точек")
            {
                CheckOnClick = true
            };
            clearPointsItem = new("Очистить все точки");
            changeRightClickMenu = new("Стандартный Right-Click по графику")
            {
                CheckOnClick = true
            };
            lineColorMenuItem = new("Цвет линии");
            lineWidthMenuItem = new("Толщина линии");
            lineIsMagnetMenuItem = new("Магнит")
            {
                CheckOnClick = true
            };
            clearAllLinesItem = new("Очистить все линии");
            eraserWidthItem = new("Толщина ластика");

            contextMenu.Items.Add(clearAllItem);
            contextMenu.Items.Add(noLimithighLightPointItem);
            contextMenu.Items.Add(clearPointsItem);
            contextMenu.Items.Add(clearAllLinesItem);
            toolContextMenu.Items.Add(lineColorMenuItem);
            toolContextMenu.Items.Add(lineWidthMenuItem);
            toolContextMenu.Items.Add(lineIsMagnetMenuItem);
            toolContextMenu.Items.Add(eraserWidthItem);
            changeColorMenu.Items.Add(changeColorMenuItem);
            crosshairSettings.DropDownItems.Add(multiCH);
            crosshairSettings.DropDownItems.Add(magnetCH);
            graphSettings.DropDownItems.Add(graphFixed);
            graphSettings.DropDownItems.Add(colorSettings);
            graphSettings.DropDownItems.Add(fakePointsSettings);
            graphSettings.DropDownItems.Add(legendSettings);
            graphSettings.DropDownItems.Add(hideLegend);
            graphSettings.DropDownItems.Add(changeRightClickMenu);
            vmmSettings.DropDownItems.Add(setVibrationLevel);
            vmmSettings.DropDownItems.Add(fastDrawVMM);
            parametersToolStripMenuItem.DropDownItems.Add(lightTheme);
            parametersToolStripMenuItem.DropDownItems.Add(graphSettings);
            parametersToolStripMenuItem.DropDownItems.Add(crosshairSettings);
            parametersToolStripMenuItem.DropDownItems.Add(vmmSettings);
            helpToolStripMenuItem.DropDownItems.Add(developersInfo);

            clearAllLinesItem.Click += ClearAllLinesItem_Click;
            ToolPanelToolStripMenuItem.CheckedChanged += ToolPanelToolStripMenuItem_CheckedChanged;
            fastDrawVMM.CheckedChanged += FastDrawVMM_CheckedChanged;
            legendSettings.Click += LegendSettings_Click;
            hideLegend.CheckedChanged += HideLegend_CheckedChanged;
            graphFixed.Click += GraphFixed_Click;
            colorSettings.Click += ColorSettings_Click;
            fakePointsSettings.CheckOnClick = true;
            fakePointsSettings.CheckedChanged += FakePointsSettings_CheckedChanged;
            multiCH.Click += MultiCH_Click;
            lightTheme.Click += LightTheme_Click;
            developersInfo.Click += InfoForm_Click;
            setVibrationLevel.Click += SetVibrationLevel_Click;
            openToolStripMenuItem.Click += OpenToolStripMenuItem_Click;
            changeColorMenuItem.Click += ChangeColorMenuItem_Click;
            clearAllItem.Click += ClearAllItem_Click;
            clearPointsItem.Click += ClearPointsItem_Click;
            changeRightClickMenu.CheckedChanged += ChangeRightClickMenu_CheckedChanged;
            lineColorMenuItem.Click += LineColorMenuItem_Click;
            lineWidthMenuItem.Click += LineWidthMenuItem_Click;
            lineIsMagnetMenuItem.CheckedChanged += LineIsMagnetMenuItem_CheckedChanged;
            eraserWidthItem.Click += EraserWidthItem_Click;
            splitContainer1.Panel2.ContextMenuStrip = contextMenu;
            toolStrip1.ContextMenuStrip = toolContextMenu;
            //---------- Стартовые настройки----------\\
            CultureInfo currentCulture = CultureInfo.CurrentCulture; // Узнаем локализацию компьютера
            separatorType = currentCulture.NumberFormat.NumberDecimalSeparator;
            yss = new double[1][]; // Инициализация массива с массивами данных
            arrayOfFakePointsIndex = Array.Empty<int>(); // инициализация пустого массива с фейковыми точками (-999.25)  
            labels = Array.Empty<System.Windows.Forms.Label>();
            graphsCount = 0;
            arrayOfGraphWindows = new GraphWindow[1];
            graphWindow = new GraphWindow(graphsCount, splitContainer1);
            arrayOfGraphWindows[graphsCount] = graphWindow;
            isCntrlPressed = false;
            myCrosshair = new ScottPlot.Plottable.Crosshair();
            myCrosshair.IsDateTimeX = true;
            myCrosshair.VerticalLine.PositionFormatter = pos => DateTime.FromOADate(pos).ToString("g");
            myCrosshair.IsVisible = false;

            // Инициализация массива с цветами
            colors = new Color[44];
            LoadSettings(); // Загружаем пользовательские настройки

            for (byte i = 0; i < 5; i++)
            {
                if (recentlyFilesArray[i] != null && recentlyFilesArray[i].Length != 0)
                {
                    fileItemList[i].Text = recentlyFilesArray[i];
                    fileItemList[i].Visible = true;
                }
            }

            // Отображаем галочки на включенных пунктах контекстного меню
            if (isMultiCH)
                multiCH.Checked = true;
            if (isGraphFixed)
                graphFixed.Checked = true;
            if (isFastDraw)
                fastDrawVMM.Checked = true;
            if (isMagnet)
                magnetCH.Checked = true;
            if (isManyPoints)
                noLimithighLightPointItem.Checked = true;
            if (isToolPanelVisible)
            {
                ToolPanelToolStripMenuItem.Checked = true;
                toolStrip1.Visible = true;
            }
            else
            {
                ToolPanelToolStripMenuItem.Checked = false;
                toolStrip1.Visible = false;
            }
            if (isLineMagnet)
                lineIsMagnetMenuItem.Checked = true;

            //----------Настройка формы и окна графика----------\\         
            splitContainer1.Panel2.Controls.Add(comboBox1);
            if (isLightTheme)
            {
                lightTheme.CheckState = CheckState.Checked;
                myCrosshair.LineColor = Color.Black;
                this.BackColor = Color.FromName("Window");
                add_graphWindow_button.BackColor = Color.FromName("ButtonFace");
                add_graphWindow_button.ForeColor = Color.Black;
                selectFile_button.BackColor = Color.FromName("ButtonFace");
                selectFile_button.ForeColor = Color.Black;
                textBox1.BackColor = Color.FromName("Control");
                textBox1.ForeColor = Color.Black;
            }
            else
            {
                lightTheme.CheckState = CheckState.Unchecked;
                this.BackColor = Color.FromArgb(27, 33, 56);
                selectFile_button.BackColor = Color.FromArgb(27, 33, 56);
                selectFile_button.ForeColor = Color.FromName("ScrollBar");
                add_graphWindow_button.BackColor = Color.FromArgb(27, 33, 56);
                add_graphWindow_button.ForeColor = Color.FromName("ScrollBar");
                textBox1.BackColor = Color.FromArgb(27, 33, 56);
                textBox1.ForeColor = Color.FromName("ScrollBar");
                myCrosshair.Color = Color.FromArgb(190, 190, 190);
            }
            // Настройки первого графического окна
            arrayOfGraphWindows[graphsCount].CreatePlot(isLightTheme);
            arrayOfGraphWindows[graphsCount].formsPlot.Plot.Legend(!isLegenedHidden);
            arrayOfGraphWindows[graphsCount].formsPlot.Plot.Add(myCrosshair);
            arrayOfGraphWindows[graphsCount].formsPlot.Tag = graphsCount;
            arrayOfGraphWindows[graphsCount].ChangeLocationAndSize(graphsCount);
            arrayOfGraphWindows[graphsCount].formsPlot.MouseMove += Plot_MouseMove;
            arrayOfGraphWindows[graphsCount].formsPlot.AxesChanged += PlotMain_AxisChanged;
            arrayOfGraphWindows[graphsCount].formsPlot.DoubleClick += FormsPlot_DoubleClick;
            arrayOfGraphWindows[graphsCount].formsPlot.MouseDown += FormsPlot_MouseDown;
            graphsCount++;
            if (isLegenedHidden)
                hideLegend.Checked = true;
            // Изменение события right click по графику
            if (isDefaultRightClickMenu)
            {
                changeRightClickMenu.Checked = true;
                arrayOfGraphWindows[0].formsPlot.RightClicked -= FormsPlot_RightClicked;
                arrayOfGraphWindows[0].formsPlot.RightClicked += arrayOfGraphWindows[0].formsPlot.DefaultRightClickEvent;
            }
            else
            {
                changeRightClickMenu.Checked = false;
                arrayOfGraphWindows[0].formsPlot.RightClicked -= arrayOfGraphWindows[0].formsPlot.DefaultRightClickEvent;
                arrayOfGraphWindows[0].formsPlot.RightClicked += FormsPlot_RightClicked;
            }

            if (isLightTheme)
                backgroundLabel.ForeColor = Color.DarkGray;
            else
                backgroundLabel.ForeColor = Color.FromArgb(68, 74, 87);
            backgroundLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            backgroundLabel.BackColor = Color.Transparent;
            backgroundLabel.Dock = DockStyle.Bottom;
            backgroundLabel.Size = new Size(350, 38);
            backgroundLabel.Font = new System.Drawing.Font("Arial", 24, FontStyle.Bold);
            backgroundLabel.BringToFront();
            backgroundLabel.Visible = false;
            splitContainer1.Panel1.Controls.Add(backgroundLabel);

            mouseCoordinates1 = new();
            mouseCoordinates2 = new();
        }

        // Кнопка "добавить окно графика"
        private void Add_graphWindow_button_Click(object sender, EventArgs e)
        {
            splitContainer1.Panel1.Focus(); // Убрали фокус
            GraphWindow graphWindowNew = new(graphsCount, splitContainer1);
            Array.Resize(ref arrayOfGraphWindows, arrayOfGraphWindows.Length + 1);
            arrayOfGraphWindows[^1] = graphWindowNew;
            arrayOfGraphWindows[graphsCount].CreatePlot(isLightTheme);
            arrayOfGraphWindows[graphsCount].formsPlot.Plot.Legend(!isLegenedHidden);
            arrayOfGraphWindows[graphsCount].CreateRemoveButton();
            arrayOfGraphWindows[graphsCount].removeButton.Click += RemoveButton_Click;
            arrayOfGraphWindows[graphsCount].formsPlot.DoubleClick += FormsPlot_DoubleClick;
            arrayOfGraphWindows[graphsCount].formsPlot.MouseDown += FormsPlot_MouseDown;
            if (multiCH.Checked)
                arrayOfGraphWindows[graphsCount].formsPlot.Plot.Add(myCrosshair);
            for (byte i = 0; i < arrayOfGraphWindows.Length; i++)
            {
                arrayOfGraphWindows[i].ChangeLocationAndSize(graphsCount);
                arrayOfGraphWindows[i].formsPlot.Tag = i;
            }
            graphsCount++;
            Form1_Resize(this, EventArgs.Empty);
            if (graphsCount == 5)
                add_graphWindow_button.Enabled = false;
        }

        // Кнопка удалить окно графика
        private void RemoveButton_Click(object? sender, EventArgs e)
        {
            System.Windows.Forms.Button button = (System.Windows.Forms.Button)sender;
            byte index = (byte)button.Tag;
            arrayOfGraphWindows[index].Dispose();
            for (byte i = index; i < arrayOfGraphWindows.Length - 1; i++)
                arrayOfGraphWindows[i] = arrayOfGraphWindows[i + 1];

            Array.Resize(ref arrayOfGraphWindows, arrayOfGraphWindows.Length - 1);

            for (byte i = 0; i < arrayOfGraphWindows.Length; i++)
            {
                arrayOfGraphWindows[i].plotNum = i;
                arrayOfGraphWindows[i].formsPlot.Tag = i;
                if (arrayOfGraphWindows[i].removeButton != null)
                    arrayOfGraphWindows[i].removeButton.Tag = i;
            }

            graphsCount--; // Так надо =))
            graphsCount--;

            for (byte i = 0; i < arrayOfGraphWindows.Length; i++)
                arrayOfGraphWindows[i].ChangeLocationAndSize(graphsCount);
            graphsCount++;

            if (graphsCount < 5)
                add_graphWindow_button.Enabled = true;

            Form1_Resize(this, EventArgs.Empty);
        }
        static byte FileType(string fileName)
        {
            if (fileName is null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            using StreamReader reader = new(fileName);
            string line;
            line = reader.ReadLine();
            if (line == null)
                return 100;
            string[] values = line.Split(new[] { "," }, StringSplitOptions.None);
            line = values[0];
            return line switch
            {
                // Battery status
                "file FFFFFF02" => 0,
                // Bore-Annulus
                "file D5060000" => 1,
                // Directional
                "file CC040000" => 2,
                // Gamma Node
                "file E4010000" => 3,
                // Master Controller Debug
                "file F3EC0000" => 4,
                // MPT Telemetred Data
                "file FFFFFF04" => 5,
                // Node Status
                "file FFFFFF03" => 6,
                // Pressure
                "file D5050000" => 7,
                // Static Survey (Directioanal)
                "file CC040001" => 8,
                // System Status
                "file FFFFFF01" => 9,
                // VMM Agregate Drilling Performance
                "file D1250000" => 10,
                // VMM Diagnostics
                "file D1260000" => 11,
                // VMM LifeTime Data
                "FileNumber" => 12,
                _ => 100,
            };
        }
        private void SelectFile_button_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        // Функция для выбора файла (для кнопки выбрать файл и элемента меню открыть)
        private void OpenFile()
        {
            splitContainer1.Panel1.Focus(); // Убрали фокус           
            cantReadFile = false;

            OpenFileDialog ofd = new()
            {
                Filter = "Text Files (*.txt)|*.txt|CSV Files (*.csv)|*.csv"
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                comboBox1?.Dispose();

                isVMMDrawed = false;

                if (isFileOpened)
                {
                    for (byte i = 0; i < labels.Length; i++)
                    {
                        labels[i].Dispose();
                        panels[i].Dispose();
                    }
                    for (byte i = 0; i < arrayOfGraphWindows.Length; i++)
                    {
                        arrayOfGraphWindows[i].formsPlot.Plot.Clear();
                        arrayOfGraphWindows[i].formsPlot.Refresh();
                        for (byte j = 0; j < arrayOfGraphWindows[i].isOnThePlot.Length; j++)
                            arrayOfGraphWindows[i].isOnThePlot[j] = false;
                    }
                    if (panels != null)
                    {
                        Array.Clear(labels);
                        Array.Clear(panels);
                    }
                }
                fileName = ofd.FileName;
                recentlyFilesArray.Insert(0, fileName);
                if (recentlyFilesArray.Count > 5)
                    recentlyFilesArray.RemoveAt(recentlyFilesArray.Count - 1);
                for (byte i = 0; i < 5; i++)
                {
                    if (recentlyFilesArray[i] != null && recentlyFilesArray[i].Length != 0)
                    {
                        fileItemList[i].Text = recentlyFilesArray[i];
                        fileItemList[i].Visible = true;
                    }
                }


                textBox1.Clear();
                fileTypeCode = FileType(fileName);

                if (fileTypeCode == 1 || fileTypeCode == 5)
                    isDate = false;
                else
                    isDate = true;

                if (fileTypeCode != 12)
                    FileInformstionData();

                FindRowsCount();
                if (isFileBroken)
                    fileTypeCode = 200;
                switch (fileTypeCode)
                {
                    case 0:
                        BatteryStatus();
                        isFileOpened = true;
                        break;
                    case 1:  //BoreAnnulus();
                        BoreAnnulus();
                        isFileOpened = true;
                        break;
                    case 2:
                        Directional();
                        isFileOpened = true;
                        break;
                    case 3:
                        GammaNode();
                        isFileOpened = true;
                        break;
                    case 4:
                        MasterControllerDebug();
                        isFileOpened = true;
                        break;
                    case 5: // MPT Telemetred Data
                        MptTelemeteredData();
                        isFileOpened = true;
                        break;
                    case 6:
                        NodeStatus();
                        isFileOpened = true;
                        break;
                    case 7:
                        Pressure();
                        isFileOpened = true;
                        break;
                    case 8: // Static Survey (Directioanal)
                        Directional();
                        isFileOpened = true;
                        break;
                    case 9:
                        SystemStatus();
                        isFileOpened = true;
                        break;
                    case 10: // VMM Agregate Drilling Performance
                        VMMAgregateDrillingPerfomance();
                        isFileOpened = true;
                        break;
                    case 11: // VMM Diagnostics
                        VMMDiagnostics();
                        isFileOpened = false;
                        break;
                    case 12: // VMMM LifeTime Data
                        VMMLifeTimeData();
                        isFileOpened = true;
                        break;
                    case 200:
                        MessageBox.Show("Данный файл поврежден. Проверьте файл вручную");
                        cantReadFile = true;
                        isFileOpened = false;
                        break;
                    default:
                        MessageBox.Show("Файл не может быть прочитан");
                        cantReadFile = true;
                        isFileOpened = false;
                        break;
                }

                arrayOfGraphWindows[0].formsPlot.Plot.Add(myCrosshair);

                if (isLightTheme && !cantReadFile)
                {
                    foreach (var item in labels)
                        item.ForeColor = Color.Black;
                }
                else if (!cantReadFile)
                {
                    foreach (var item in labels)
                        if (item != null)
                            item.ForeColor = Color.White;
                }
            }
        }

        // Класс окна с графиком 
        public class GraphWindow : IDisposable
        {
            private bool isFakePointOnThePlot;
            public byte graphsCount;
            public byte plotNum;
            public Rectangle recRemoveButton, recFormsPlot;
            public Point plotLocation, removeButtonLocation, standartPlotLocation, standartRemoveButtonLocation;
            public Size removeButtonSize, plotSize, standartRemoveButtonSize, standartPlotSize;
            readonly SplitContainer splitContainer;
            public ScottPlot.FormsPlot formsPlot;
            public System.Windows.Forms.Button removeButton;
            public bool[] isOnThePlot;
            public SignalPlotXY[] plottables;
            public MarkerPlot[] markers;


            public GraphWindow(byte plotNum, SplitContainer splitContainer)
            {
                this.isFakePointOnThePlot = false;
                this.splitContainer = splitContainer;
                this.standartPlotLocation = new Point(0, 0);
                this.standartPlotSize = new Size(894, 645);
                this.standartRemoveButtonLocation = new Point(842, 23);
                this.standartRemoveButtonSize = new Size(22, 19);
                this.plotNum = plotNum;
                this.removeButtonSize = standartRemoveButtonSize;
                this.isOnThePlot = new bool[44];
                for (byte i = 0; i < isOnThePlot.Length; i++)
                    this.isOnThePlot[i] = false;
                plottables = Array.Empty<SignalPlotXY>();
                markers = Array.Empty<MarkerPlot>();
            }

            public void CreatePlot(bool isLightTheme)
            {
                formsPlot = new ScottPlot.FormsPlot
                {
                    Tag = plotNum
                };
                if (!isLightTheme)
                    formsPlot.Plot.Style(Style.Blue2);
                else
                    formsPlot.Plot.Style(Style.Default);
                formsPlot.Size = plotSize;
                formsPlot.Location = plotLocation;
                formsPlot.Plot.XAxis.DateTimeFormat(true);
                recFormsPlot = new Rectangle(formsPlot.Location, formsPlot.Size);
                splitContainer.Panel1.Controls.Add(formsPlot);
                formsPlot.Refresh();
            }

            public void ChangeDateType(bool ifDateType)
            {
                formsPlot.Plot.XAxis.DateTimeFormat(ifDateType);
            }
            public void RemoveChart(Int32 labelNum, string[] labelsText)
            {
                isOnThePlot[labelNum] = false;
                isFakePointOnThePlot = false;

                for (byte i = 0; i < plottables.Length; i++)
                    if (plottables[i].Label == labelsText[labelNum])
                        formsPlot.Plot.Remove(plottables[i]);
                formsPlot.Refresh();
            }
            public void Draw(Int32 labelNum, double[] xs, double[][] yss, Color[] colors, string labelText, bool isDateType, int[] arrayOfFakePointInex, Color specialColor, bool isFakePointsEnabled, bool isLegenedHidden)
            {
                isOnThePlot[labelNum] = true;
                if (!isDateType)
                    formsPlot.Plot.XAxis.DateTimeFormat(false);
                var plottable = formsPlot.Plot.AddSignalXY(xs, yss[labelNum], colors[labelNum], labelText);
                if (!isFakePointOnThePlot && isFakePointsEnabled)
                    for (int i = 0; i < arrayOfFakePointInex.Length; i++)
                        formsPlot.Plot.AddPoint(xs[arrayOfFakePointInex[i]], yss[labelNum][arrayOfFakePointInex[i]], specialColor);
                isFakePointOnThePlot = true;
                formsPlot.Plot.Legend(!isLegenedHidden);
                formsPlot.Refresh();
                Array.Resize(ref plottables, plottables.Length + 1);
                plottables[^1] = plottable;
            }
            public void Draw(double y, float width, Color lineColor, byte typeOfOrientation)
            {
                if (typeOfOrientation == 0)
                    formsPlot.Plot.AddHorizontalLine(y, lineColor, width, LineStyle.None, null);
                else if (typeOfOrientation == 1)
                    formsPlot.Plot.AddVerticalLine(y, lineColor, width, LineStyle.None, null);
            }
            public void Draw(double x1, double y1, double x2, double y2, float width, Color lineColor)
            {
                var line = formsPlot.Plot.AddLine(x1, y1, x2, y2, lineColor, width);
            }
            public void changePlottablesColor(Color[] colors, string[] labelsText)
            {
                for (byte i = 0; i < plottables.Length; i++)
                {
                    for (byte j = 0; j < labelsText.Length; j++)
                    {
                        if (plottables[i].Label == labelsText[j])
                        {
                            plottables[i].Color = colors[j];
                            break;
                        }
                    }
                }
                formsPlot.Refresh();
            }
            public void CreateRemoveButton()
            {
                removeButton = new System.Windows.Forms.Button
                {
                    BackColor = Color.Red,
                    Font = new Font("Segoe UI Black", 7F, FontStyle.Bold, GraphicsUnit.Point),
                    ForeColor = Color.White,
                    Text = "X",
                    UseVisualStyleBackColor = false,
                    Size = removeButtonSize,
                    Location = removeButtonLocation
                };
                recRemoveButton = new Rectangle(removeButton.Location, removeButton.Size);
                splitContainer.Panel1.Controls.Add(removeButton);
                removeButton.BringToFront();
                removeButton.Tag = plotNum;
            }
            public void ChangeLocationAndSize(byte graphsCount)
            {
                plotSize = new Size(standartPlotSize.Width, standartPlotSize.Height / (graphsCount + 1));
                switch (plotNum)
                {
                    case 0:
                        plotLocation = new Point(standartPlotLocation.X, standartPlotLocation.Y + graphsCount * plotSize.Height);
                        removeButtonLocation = new Point(standartRemoveButtonLocation.X, standartRemoveButtonLocation.Y + graphsCount * plotSize.Height);
                        break;
                    case 1:
                        plotLocation = new Point(standartPlotLocation.X, standartPlotLocation.Y + (graphsCount - 1) * plotSize.Height);
                        removeButtonLocation = new Point(standartRemoveButtonLocation.X, standartRemoveButtonLocation.Y + (graphsCount - 1) * plotSize.Height);
                        break;
                    case 2:
                        plotLocation = new Point(standartPlotLocation.X, standartPlotLocation.Y + (graphsCount - 2) * plotSize.Height);
                        removeButtonLocation = new Point(standartRemoveButtonLocation.X, standartRemoveButtonLocation.Y + (graphsCount - 2) * plotSize.Height);
                        break;
                    case 3:
                        plotLocation = new Point(standartPlotLocation.X, standartPlotLocation.Y + (graphsCount - 3) * plotSize.Height);
                        removeButtonLocation = new Point(standartRemoveButtonLocation.X, standartRemoveButtonLocation.Y + (graphsCount - 3) * plotSize.Height);
                        break;
                    case 4:
                        plotLocation = new Point(standartPlotLocation.X, standartPlotLocation.Y + (graphsCount - 4) * plotSize.Height);
                        removeButtonLocation = new Point(standartRemoveButtonLocation.X, standartRemoveButtonLocation.Y + (graphsCount - 4) * plotSize.Height);
                        break;
                    default:
                        //plotLocation = standartPlotLocation;
                        break;

                }
                formsPlot.Location = plotLocation;
                formsPlot.Size = plotSize;
                if (removeButton != null)
                {
                    removeButton.Location = removeButtonLocation;
                    recRemoveButton = new Rectangle(removeButton.Location, removeButton.Size);
                }
                recFormsPlot = new Rectangle(formsPlot.Location, formsPlot.Size);
                formsPlot.Refresh();
            }
            public void Dispose()
            {
                if (removeButton != null && formsPlot != null)
                {
                    removeButton.Dispose();
                    formsPlot.Dispose();
                }
                for (byte i = 0; i < isOnThePlot.Length; i++)
                    isOnThePlot[i] = false;
            }
            public void AddMarker(int plottableIndex, (double x, double y) coordinates, DateTime[] dates)
            {
                int index = 0;
                var marker = formsPlot.Plot.AddMarker(coordinates.x, coordinates.y);
                double[] ys = plottables[plottableIndex].Ys;
                for (int i = 0; i < ys.Length; i++)
                {
                    if (coordinates.y == ys[i])
                        index = i;
                }
                marker.Text = "x: " + Convert.ToString(dates[index]) + " y: " + Convert.ToString(coordinates.y);
                marker.TextFont.Color = Color.Magenta;
                marker.TextFont.Alignment = Alignment.UpperCenter;
                marker.TextFont.Size = 14;
                Array.Resize(ref markers, markers.Length + 1);
                markers[^1] = marker;
            }
            public void AddMarker(double x, double y)
            {
                var marker = formsPlot.Plot.AddMarker(x, y);
                Array.Resize(ref markers, markers.Length + 1);
                markers[^1] = marker;
            }
            public void ClearAll(Crosshair ch)
            {
                for (byte i = 0; i < isOnThePlot.Length; i++)
                    isOnThePlot[i] = false;
                Array.Clear(markers);
                Array.Resize(ref markers, 0);
                formsPlot.Plot.Clear();
                formsPlot.Plot.Add(ch);
                formsPlot.Refresh();
            }
            public void ClearMarkers()
            {
                for (int i = 0; i < markers.Length; i++)
                    markers[i].IsVisible = false;
                formsPlot.Refresh();
            }
            public void RemoveLines()
            {
                foreach (var item in formsPlot.Plot.GetPlottables())
                    if (item is ScottPlot.Plottable.HLine || item is ScottPlot.Plottable.VLine)
                        formsPlot.Plot.Remove(item);
                formsPlot.Refresh();
            }
        }

        // Группа методов для ресайза
        private void Resize_Control(Control c, Rectangle r)
        {
            float xRatio = (float)(this.Width) / (float)(formOriginalSize.Width);
            float yRatio = (float)(this.Height) / (float)(formOriginalSize.Height);
            int newX = (int)(r.X * xRatio);
            int newY = (int)(r.Y * yRatio);

            int newWidth = (int)(r.Width * xRatio);
            int newHeight = (int)(r.Height * yRatio);

            c.Location = new Point(newX, newY);
            c.Size = new Size(newWidth, newHeight);
        }
        private void Resize_Panel1(Control c, Rectangle r)
        {
            float xRatio = (float)(splitContainer1.Panel1.Width) / (float)(recSplitContainerPanel1.Width);
            float yRatio = (float)(splitContainer1.Panel1.Height) / (float)(recSplitContainerPanel1.Height);
            int newX = (int)(r.X * xRatio);
            int newY = (int)(r.Y * yRatio);

            int newWidth = (int)(r.Width * xRatio);
            int newHeight = (int)(r.Height * yRatio);

            c.Location = new Point(newX, newY);
            c.Size = new Size(newWidth, newHeight);
        }
        private void Resize_Panel2(Control c, Rectangle r)
        {
            float xRatio = (float)(splitContainer1.Panel2.Width) / (float)(recSplitContainerPanel2.Width);
            float yRatio = (float)(splitContainer1.Panel2.Height) / (float)(recSplitContainerPanel2.Height);
            int newX = (int)(r.X * xRatio);
            int newY = (int)(r.Y * yRatio);

            int newWidth = (int)(r.Width * xRatio);
            int newHeight = (int)(r.Height * yRatio);

            c.Location = new Point(newX, newY);
            c.Size = new Size(newWidth, newHeight);
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            // Ресайз для сплит контейнера
            Resize_Control(splitContainer1, recSplitContainer1);
            Resize_Control(splitContainer1.Panel1, recSplitContainerPanel1);
            Resize_Control(splitContainer1.Panel2, recSplitContainerPanel2);

            // Ресайз для комбоБокса
            Resize_Control(comboBox1, recComboBox1);
            Resize_Panel2(comboBox1, recComboBox1);

            // Ресайз для кнопки выбора файла
            Resize_Control(selectFile_button, recSelectFile_Button);
            Resize_Panel2(selectFile_button, recSelectFile_Button);

            // Ресайз для кнопки добавления графиков
            Resize_Control(add_graphWindow_button, recAddGraphWindow_button);
            Resize_Panel2(add_graphWindow_button, recAddGraphWindow_button);

            // Ресайз для текстбокса с информацией о файле
            Resize_Control(textBox1, recTextBox);
            Resize_Panel2(textBox1, recTextBox);

            // Ресайз для progressBar
            Resize_Control(progressBar1, recProgressBar);
            Resize_Panel2(progressBar1, recProgressBar);

            // Ресайз для графиков
            for (byte i = 0; i < arrayOfGraphWindows.Length; i++)
            {
                Resize_Control(arrayOfGraphWindows[i].formsPlot, arrayOfGraphWindows[i].recFormsPlot);
                Resize_Panel1(arrayOfGraphWindows[i].formsPlot, arrayOfGraphWindows[i].recFormsPlot);
            }

        }
        private void SplitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            // Ресайз для кнопки выбора файла
            Resize_Control(selectFile_button, recSelectFile_Button);
            Resize_Panel2(selectFile_button, recSelectFile_Button);

            // Ресайз для комбоБокса
            Resize_Control(comboBox1, recComboBox1);
            Resize_Panel2(comboBox1, recComboBox1);

            // Ресайз для кнопки добавления графиков
            Resize_Control(add_graphWindow_button, recAddGraphWindow_button);
            Resize_Panel2(add_graphWindow_button, recAddGraphWindow_button);

            // Ресайз для текстбокса с информацией о файле
            Resize_Control(textBox1, recTextBox);
            Resize_Panel2(textBox1, recTextBox);

            // Ресайз для progressBar
            Resize_Control(progressBar1, recProgressBar);
            Resize_Panel2(progressBar1, recProgressBar);

            // Ресайз для графиков
            for (byte i = 0; i < arrayOfGraphWindows.Length; i++)
            {
                Resize_Control(arrayOfGraphWindows[i].formsPlot, arrayOfGraphWindows[i].recFormsPlot);
                Resize_Panel1(arrayOfGraphWindows[i].formsPlot, arrayOfGraphWindows[i].recFormsPlot);
            }

            foreach (var item in arrayOfGraphWindows)
            {
                if (item.removeButton != null)
                {
                    item.removeButton.Location = new Point(item.standartRemoveButtonLocation.X, item.standartRemoveButtonLocation.Y + (graphsCount - 1) * item.plotSize.Height);
                    item.recRemoveButton = new Rectangle(item.removeButtonLocation, item.standartRemoveButtonSize);
                    Resize_Control(item.removeButton, item.recRemoveButton);
                    Resize_Panel1(item.removeButton, item.recRemoveButton);
                }
            }

        }

        //Функция для считывания времени из файла
        private static DateTime DateTimeFunc(string dateString, bool isVMMDateType)
        {

            string[] dateTimeTemproraryArray = dateString.Split(new[] { ' ' }, StringSplitOptions.None);
            string[] dateSplitTemproraryArray = dateTimeTemproraryArray[0].Split(new[] { '/' }, StringSplitOptions.None);
            string[] timeSplitTemproraryArray = dateTimeTemproraryArray[1].Split(new[] { ':' }, StringSplitOptions.None);
            if (!isVMMDateType)
                return new DateTime(2000 + Convert.ToInt16(dateSplitTemproraryArray[0]), Convert.ToInt16(dateSplitTemproraryArray[1]), Convert.ToInt16(dateSplitTemproraryArray[2]), Convert.ToInt16(timeSplitTemproraryArray[0]), Convert.ToInt16(timeSplitTemproraryArray[1]), Convert.ToInt16(timeSplitTemproraryArray[2]));
            else
                return new DateTime(Convert.ToInt16(dateSplitTemproraryArray[2]), Convert.ToInt16(dateSplitTemproraryArray[0]), Convert.ToInt16(dateSplitTemproraryArray[1]), Convert.ToInt16(timeSplitTemproraryArray[0]), Convert.ToInt16(timeSplitTemproraryArray[1]), 0);
        }
        // Функция для подсчета кол-ва строк в файле(для прогресс бара)
        private void FindRowsCount()
        {
            string line;
            using StreamReader reader = new(fileName);
            isFileBroken = false;
            while (!reader.EndOfStream)
            {
                line = reader.ReadLine();
                if (line.Contains("Warning"))
                    isFileBroken = true;
                rowCount++;
            }
        }

        // Вывод информации о файле в текстбокс
        private void FileInformstionData()
        {
            StreamReader reader = new(fileName);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] values = line.Split(new[] { "," }, StringSplitOptions.None);
                for (int i = 0; i < values.Length; i++)
                {
                    if (values[i] == "00000000 : " || values[i] == "nnnnnnnn : yyyy/mmm/dd hh:mm:ss")
                    {
                        return;
                    }
                    else
                        textBox1.Text += values[i];
                }
            }
        }
        //----------Группа методов для каждого из типа файлов----------\\
        private void Directional()
        {
            Array.Clear(arrayOfFakePointsIndex);
            dates = Array.Empty<DateTime>();
            markerDates = Array.Empty<DateTime>();
            arrayOfFakePointsIndex = Array.Empty<int>(); // инициализация пустого массива с фейковыми точками (-999.25)  
            bool rowFinded = false;
            // Массивы с каждым типом данных
            double[] xGravityGs = Array.Empty<double>();
            double[] yGravityGs = Array.Empty<double>();
            double[] zGravityGs = Array.Empty<double>();
            double[] xMagGauss = Array.Empty<double>();
            double[] yMagGauss = Array.Empty<double>();
            double[] zMagGauss = Array.Empty<double>();
            double[] gTotal = Array.Empty<double>();
            double[] bTotal = Array.Empty<double>();
            double[] temperature = Array.Empty<double>();

            // Инициализация панелей и лейблов
            labels = new System.Windows.Forms.Label[9];
            panels = new Panel[9];
            labelStandartLocations = new Point[9];
            panelStandartLocations = new Point[9];

            for (byte i = 0; i < 9; i++)
            {
                // Настройки для labels
                labels[i] = new System.Windows.Forms.Label
                {
                    ForeColor = Color.White,
                    Font = new Font("Tobota", 10, FontStyle.Regular),
                    Location = new Point(30, 10 + i * 20),
                    Tag = i // Устанавливаем тэг лейбла (номер по счету)
                };
                labelStandartLocations[i] = labels[i].Location;
                // Подписываем все labels на события
                labels[i].MouseDown += Label_MouseDown;
                labels[i].MouseMove += Label_MouseMove;
                labels[i].MouseUp += Label_MouseUp;
                // Настройки для panels
                panels[i] = new Panel
                {
                    Location = new Point(10, 10 + i * 20),
                    Size = new Size(17, 17),
                    BackColor = colors[i]
                };
                panelStandartLocations[i] = panels[i].Location;
                // Добавляем элементы управления на панель сплитконтейнера
                splitContainer1.Panel2.Controls.Add(labels[i]);
                splitContainer1.Panel2.Controls.Add(panels[i]);
                panels[i].BringToFront();
            }

            // Текст для лейблов (название осей)
            labels[0].Text = "xGravityGs";
            labels[1].Text = "yGravityGs";
            labels[2].Text = "zGravityGs";
            labels[3].Text = "xMagGauss";
            labels[4].Text = "yMagGauss";
            labels[5].Text = "zMagGauss";
            labels[6].Text = "gTotal";
            labels[7].Text = "bTotal";
            labels[8].Text = "Temperature";

            labelsText = new string[9];
            for (byte i = 0; i < labels.Length; i++)
            {
                labelsText[i] = labels[i].Text;
                panels[i].ContextMenuStrip = changeColorMenu;
                panels[i].Tag = i;
            }


            progressBar1.Maximum = rowCount;
            using StreamReader reader = new(fileName);
            string line;
            int lineCount = 0;
            while ((line = reader.ReadLine()) != null)
            {
                string[] values = line.Split(new[] { ", " }, StringSplitOptions.None);
                if (values[0] == "00000000 : ")
                    rowFinded = true;
                if (rowFinded)
                {
                    for (int i = 0; i < values.Length; i++)
                        if (i == 1)
                            if (values[1] == "00/00/00 00:00:00")
                                break;
                            else
                            {
                                Array.Resize(ref dates, dates.Length + 1);
                                dates[lineCount] = DateTimeFunc(values[1], false);
                            }

                    if (values[1] != "00/00/00 00:00:00")
                    {
                        Array.Resize(ref xGravityGs, xGravityGs.Length + 1);
                        if (values[2] == "-999.250000")
                        {
                            xGravityGs[lineCount] = 0;
                            Array.Resize(ref arrayOfFakePointsIndex, arrayOfFakePointsIndex.Length + 1);
                            arrayOfFakePointsIndex[arrayOfFakePointsIndex.Length - 1] = lineCount;
                        }
                        else
                            xGravityGs[lineCount] = Convert.ToDouble(values[2].Replace('.', Convert.ToChar(separatorType)));

                        Array.Resize(ref yGravityGs, yGravityGs.Length + 1);
                        if (values[3] == "-999.250000")
                            yGravityGs[lineCount] = 0;
                        else
                            yGravityGs[lineCount] = (double)Convert.ToDouble(values[3].Replace('.', Convert.ToChar(separatorType)));

                        Array.Resize(ref zGravityGs, zGravityGs.Length + 1);
                        if (values[4] == "-999.250000")
                            zGravityGs[lineCount] = 0;
                        else
                            zGravityGs[lineCount] = (double)Convert.ToDouble(values[4].Replace('.', Convert.ToChar(separatorType)));

                        Array.Resize(ref xMagGauss, xMagGauss.Length + 1);
                        if (values[5] == "-999.250000")
                            xMagGauss[lineCount] = 0;
                        else
                            xMagGauss[lineCount] = (double)Convert.ToDouble(values[5].Replace('.', Convert.ToChar(separatorType)));

                        Array.Resize(ref yMagGauss, yMagGauss.Length + 1);
                        if (values[6] == "-999.250000")
                            yMagGauss[lineCount] = 0;
                        else
                            yMagGauss[lineCount] = (double)Convert.ToDouble(values[6].Replace('.', Convert.ToChar(separatorType)));

                        Array.Resize(ref zMagGauss, zMagGauss.Length + 1);
                        if (values[6] == "-999.250000")
                            zMagGauss[lineCount] = 0;
                        else
                            zMagGauss[lineCount] = (double)Convert.ToDouble(values[7].Replace('.', Convert.ToChar(separatorType)));

                        Array.Resize(ref gTotal, gTotal.Length + 1);
                        gTotal[lineCount] = (double)Math.Sqrt(Math.Pow(xGravityGs[lineCount], 2) + Math.Pow(yGravityGs[lineCount], 2) + Math.Pow(zGravityGs[lineCount], 2));
                        Array.Resize(ref bTotal, bTotal.Length + 1);
                        bTotal[lineCount] = (double)Math.Sqrt(Math.Pow(xMagGauss[lineCount], 2) + Math.Pow(yMagGauss[lineCount], 2) + Math.Pow(zMagGauss[lineCount], 2));

                        Array.Resize(ref temperature, temperature.Length + 1);
                        temperature[lineCount] = (double)Convert.ToDouble(values[8].Replace('.', Convert.ToChar(separatorType)));
                        lineCount++;
                    }
                }
                // Меняем значение прогресс бара
                progressBar1.Value++;
            }
            progressBar1.Value = progressBar1.Maximum;
            // Инициализация массива массивов и присвоение значений 
            yss = new double[9][];
            yss[0] = xGravityGs;
            yss[1] = yGravityGs;
            yss[2] = zGravityGs;
            yss[3] = xMagGauss;
            yss[4] = yMagGauss;
            yss[5] = zMagGauss;
            yss[6] = gTotal;
            yss[7] = bTotal;
            yss[8] = temperature;
            Array.Resize(ref markerDates, dates.Length);
            Array.Copy(dates, markerDates, markerDates.Length);
            xs = dates.Select(x => x.ToOADate()).ToArray();  // Перевод массива с датами в double для графика
            isDate = true;
        }
        private void GammaNode()
        {
            Array.Clear(arrayOfFakePointsIndex);
            markerDates = Array.Empty<DateTime>();
            dates = Array.Empty<DateTime>();
            bool rowFinded = false;
            // Массивы с каждым типом данных
            double[] gammaRate = Array.Empty<double>();

            // Инициализация панелей и лейблов
            labels = new System.Windows.Forms.Label[1];
            panels = new Panel[1];
            labelStandartLocations = new Point[1];
            panelStandartLocations = new Point[1];

            for (byte i = 0; i < labels.Length; i++)
            {
                // Настройки для labels
                labels[i] = new System.Windows.Forms.Label
                {
                    ForeColor = Color.White,
                    Font = new Font("Tobota", 10, FontStyle.Regular),
                    Location = new Point(30, 10 + i * 20),
                    Tag = i // Устанавливаем тэг лейбла (номер по счету)
                };
                labelStandartLocations[i] = labels[i].Location;
                // Подписываем все labels на события
                labels[i].MouseDown += Label_MouseDown;
                labels[i].MouseMove += Label_MouseMove;
                labels[i].MouseUp += Label_MouseUp;
                // Настройки для panels
                panels[i] = new Panel
                {
                    Location = new Point(10, 10 + i * 20),
                    Size = new Size(17, 17),
                    BackColor = colors[i]
                };
                panelStandartLocations[i] = panels[i].Location;
                // Добавляем элементы управления на панель сплитконтейнера
                splitContainer1.Panel2.Controls.Add(labels[i]);
                splitContainer1.Panel2.Controls.Add(panels[i]);
                panels[i].BringToFront();
            }

            // Текст для лейблов (название осей)
            labels[0].Text = "gammaRate";
            labelsText = new string[1];
            labelsText[0] = labels[0].Text;
            panels[0].ContextMenuStrip = changeColorMenu;
            panels[0].Tag = 0;
            progressBar1.Maximum = rowCount;
            using StreamReader reader = new(fileName);
            string line;
            int lineCount = 0;
            while ((line = reader.ReadLine()) != null)
            {
                string[] values = line.Split(new[] { ", " }, StringSplitOptions.None);
                if (values[0] == "00000000 : ")
                    rowFinded = true;
                if (rowFinded)
                {
                    for (int i = 0; i < values.Length; i++)
                        if (i == 1)
                            if (values[1] == "00/00/00 00:00:00")
                                break;
                            else
                            {
                                Array.Resize(ref dates, dates.Length + 1);
                                dates[lineCount] = DateTimeFunc(values[1], false);
                            }

                    if (values[1] != "00/00/00 00:00:00")
                    {
                        Array.Resize(ref gammaRate, gammaRate.Length + 1);
                        gammaRate[lineCount] = Convert.ToDouble(values[2].Replace('.', Convert.ToChar(separatorType)));
                        lineCount++;
                    }
                }
                // Меняем значение прогресс бара
                progressBar1.Value++;
            }
            progressBar1.Value = progressBar1.Maximum;
            // Инициализация массива массивов и присвоене значений 
            yss = new double[1][];
            yss[0] = gammaRate;
            Array.Resize(ref markerDates, dates.Length);
            Array.Copy(dates, markerDates, markerDates.Length);
            xs = dates.Select(x => x.ToOADate()).ToArray(); // Перевод массива с датами в double для графика
            isDate = true;
        }
        private void BatteryStatus()
        {
            markerDates = Array.Empty<DateTime>();
            Array.Clear(arrayOfFakePointsIndex);
            dates = Array.Empty<DateTime>();
            FindRowsCount();
            bool rowFinded = false;
            // Массивы с каждым типом данных
            double[] circulatingHoursParameter = Array.Empty<double>();
            double[] voltsPulserActive = Array.Empty<double>();
            double[] voltsPulserIdle = Array.Empty<double>();
            double[] pulseCountParameter = Array.Empty<double>();
            double[] temperatureParameter = Array.Empty<double>();

            // Инициализация панелей и лейблов
            labels = new System.Windows.Forms.Label[5];
            panels = new Panel[5];
            labelStandartLocations = new Point[5];
            panelStandartLocations = new Point[5];

            for (byte i = 0; i < labels.Length; i++)
            {
                // Настройки для labels
                labels[i] = new System.Windows.Forms.Label
                {
                    AutoSize = true,
                    ForeColor = Color.White,
                    Font = new Font("Tobota", 10, FontStyle.Regular),
                    Location = new Point(30, 10 + i * 20),
                    Tag = i // Устанавливаем тэг лейбла (номер по счету)
                };
                labelStandartLocations[i] = labels[i].Location;
                // Подписываем все labels на события
                labels[i].MouseDown += Label_MouseDown;
                labels[i].MouseMove += Label_MouseMove;
                labels[i].MouseUp += Label_MouseUp;
                // Настройки для panels
                panels[i] = new Panel
                {
                    Location = new Point(10, 10 + i * 20),
                    Size = new Size(17, 17),
                    BackColor = colors[i]
                };
                panelStandartLocations[i] = panels[i].Location;
                // Добавляем элементы управления на панель сплитконтейнера
                splitContainer1.Panel2.Controls.Add(labels[i]);
                splitContainer1.Panel2.Controls.Add(panels[i]);
                panels[i].BringToFront();
            }

            // Текст для лейблов (название осей)
            labels[0].Text = "circulatingHoursParameter";
            labels[1].Text = "voltsPulserActive";
            labels[2].Text = "voltsPulserIdle";
            labels[3].Text = "pulseCountParameter";
            labels[4].Text = "temperatureParameter";
            labelsText = new string[5];
            for (byte i = 0; i < labels.Length; i++)
            {
                labelsText[i] = labels[i].Text;
                panels[i].ContextMenuStrip = changeColorMenu;
                panels[i].Tag = i;
            }

            progressBar1.Maximum = rowCount;
            using StreamReader reader = new(fileName);
            string line;
            int lineCount = 0;
            while ((line = reader.ReadLine()) != null)
            {
                string[] values = line.Split(new[] { ", " }, StringSplitOptions.None);
                if (values[0] == "00000000 : ")
                    rowFinded = true;
                if (rowFinded)
                {
                    for (int i = 0; i < values.Length; i++)
                        if (i == 1)
                            if (values[1] == "00/00/00 00:00:00")
                                break;
                            else
                            {
                                Array.Resize(ref dates, dates.Length + 1);
                                dates[lineCount] = DateTimeFunc(values[1], false);
                            }
                    if (values[1] != "00/00/00 00:00:00")
                    {
                        Array.Resize(ref circulatingHoursParameter, circulatingHoursParameter.Length + 1);
                        circulatingHoursParameter[lineCount] = (double)Convert.ToInt32(values[2], 16);

                        Array.Resize(ref voltsPulserActive, voltsPulserActive.Length + 1);
                        voltsPulserActive[lineCount] = (double)Convert.ToInt32(values[3], 16);

                        Array.Resize(ref voltsPulserIdle, voltsPulserIdle.Length + 1);
                        voltsPulserIdle[lineCount] = (double)Convert.ToInt32(values[4], 16);

                        Array.Resize(ref pulseCountParameter, pulseCountParameter.Length + 1);
                        pulseCountParameter[lineCount] = (double)Convert.ToInt32(values[5], 16);

                        Array.Resize(ref temperatureParameter, temperatureParameter.Length + 1);
                        temperatureParameter[lineCount] = (double)Convert.ToInt32(values[6], 16);
                        lineCount++;
                    }
                }
                // Меняем значение прогресс бара
                progressBar1.Value++;
            }
            progressBar1.Value = progressBar1.Maximum;
            // Инициализация массива массивов и присвоене значений 
            yss = new double[5][];
            yss[0] = circulatingHoursParameter;
            yss[1] = voltsPulserActive;
            yss[2] = voltsPulserIdle;
            yss[3] = pulseCountParameter;
            yss[4] = temperatureParameter;
            Array.Resize(ref markerDates, dates.Length);
            Array.Copy(dates, markerDates, markerDates.Length);
            xs = dates.Select(x => x.ToOADate()).ToArray(); // Перевод массива с датами в double для графика
            isDate = true;
        }
        private void BoreAnnulus()
        {

            FindRowsCount();
            bool rowFinded = false;
            // Массивы с каждым типом данных
            double[] recordNumbers = Array.Empty<double>();
            double[] Avg0 = Array.Empty<double>();
            double[] Avg1 = Array.Empty<double>();
            double[] Avg2 = Array.Empty<double>();
            double[] Avg3 = Array.Empty<double>();
            double[] Avg4 = Array.Empty<double>();
            double[] Max0 = Array.Empty<double>();
            double[] Max1 = Array.Empty<double>();
            double[] Max2 = Array.Empty<double>();
            double[] Max3 = Array.Empty<double>();
            double[] Max4 = Array.Empty<double>();
            double[] Min0 = Array.Empty<double>();
            double[] Min1 = Array.Empty<double>();
            double[] Min2 = Array.Empty<double>();
            double[] Min3 = Array.Empty<double>();
            double[] Min4 = Array.Empty<double>();

            // Инициализация панелей и лейблов
            labels = new System.Windows.Forms.Label[15];
            panels = new Panel[15];
            labelStandartLocations = new Point[15];
            panelStandartLocations = new Point[15];

            for (byte i = 0; i < labels.Length; i++)
            {
                // Настройки для labels
                labels[i] = new System.Windows.Forms.Label();
                labels[i].ForeColor = Color.White;
                labels[i].Font = new Font("Tobota", 10, FontStyle.Regular);
                labels[i].Location = new Point(30, 10 + i * 20);
                labels[i].Tag = i; // Устанавливаем тэг лейбла (номер по счету)
                labelStandartLocations[i] = labels[i].Location;
                // Подписываем все labels на события
                labels[i].MouseDown += Label_MouseDown;
                labels[i].MouseMove += Label_MouseMove;
                labels[i].MouseUp += Label_MouseUp;
                // Настройки для panels
                panels[i] = new Panel();
                panels[i].Location = new Point(10, 10 + i * 20);
                panels[i].Size = new Size(17, 17);
                panels[i].BackColor = colors[i];
                panelStandartLocations[i] = panels[i].Location;
                // Добавляем элементы управления на панель сплитконтейнера
                splitContainer1.Panel2.Controls.Add(labels[i]);
                splitContainer1.Panel2.Controls.Add(panels[i]);
                panels[i].BringToFront();
            }

            // Текст для лейблов (название осей)
            labels[0].Text = "Avg0";
            labels[1].Text = "Avg1";
            labels[2].Text = "Avg2";
            labels[3].Text = "Avg3";
            labels[4].Text = "Avg4";
            labels[5].Text = "Max0";
            labels[6].Text = "Max1";
            labels[7].Text = "Max2";
            labels[8].Text = "Max3";
            labels[9].Text = "Max4";
            labels[10].Text = "Min0";
            labels[11].Text = "Min1";
            labels[12].Text = "Min2";
            labels[13].Text = "Min3";
            labels[14].Text = "Min4";

            labelsText = new string[15];
            for (byte i = 0; i < labels.Length; i++)
            {
                labelsText[i] = labels[i].Text;
                panels[i].ContextMenuStrip = changeColorMenu;
                panels[i].Tag = i;
            }

            progressBar1.Maximum = rowCount;
            using (StreamReader reader = new StreamReader(fileName))
            {
                string line;
                int lineCount = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] values = line.Split(new[] { ", " }, StringSplitOptions.None);
                    if (values[0] == "00000000 : ")
                        rowFinded = true;
                    if (rowFinded)
                    {
                        string[] tmp = values[0].Split(new char[] { ' ' });
                        Array.Resize(ref recordNumbers, recordNumbers.Length + 1);
                        recordNumbers[lineCount] = Convert.ToDouble(tmp[0].Replace('.', Convert.ToChar(separatorType)));

                        Array.Resize(ref Avg0, Avg0.Length + 1);
                        Avg0[lineCount] = Convert.ToDouble(values[1].Replace('.', Convert.ToChar(separatorType)));

                        Array.Resize(ref Avg1, Avg1.Length + 1);
                        Avg1[lineCount] = Convert.ToDouble(values[2].Replace('.', Convert.ToChar(separatorType)));

                        Array.Resize(ref Avg2, Avg2.Length + 1);
                        Avg2[lineCount] = Convert.ToDouble(values[3].Replace('.', Convert.ToChar(separatorType)));

                        Array.Resize(ref Avg3, Avg3.Length + 1);
                        Avg3[lineCount] = Convert.ToDouble(values[4].Replace('.', Convert.ToChar(separatorType)));

                        Array.Resize(ref Avg4, Avg4.Length + 1);
                        Avg4[lineCount] = Convert.ToDouble(values[5].Replace('.', Convert.ToChar(separatorType)));

                        Array.Resize(ref Max0, Max0.Length + 1);
                        Max0[lineCount] = Convert.ToDouble(values[6].Replace('.', Convert.ToChar(separatorType)));

                        Array.Resize(ref Max1, Max1.Length + 1);
                        Max1[lineCount] = Convert.ToDouble(values[7].Replace('.', Convert.ToChar(separatorType)));

                        Array.Resize(ref Max2, Max2.Length + 1);
                        Max2[lineCount] = Convert.ToDouble(values[8].Replace('.', Convert.ToChar(separatorType)));

                        Array.Resize(ref Max3, Max3.Length + 1);
                        Max3[lineCount] = Convert.ToDouble(values[9].Replace('.', Convert.ToChar(separatorType)));

                        Array.Resize(ref Max4, Max4.Length + 1);
                        Max4[lineCount] = Convert.ToDouble(values[10].Replace('.', Convert.ToChar(separatorType)));

                        Array.Resize(ref Min0, Min0.Length + 1);
                        Min0[lineCount] = Convert.ToDouble(values[11].Replace('.', Convert.ToChar(separatorType)));

                        Array.Resize(ref Min1, Min1.Length + 1);
                        Min1[lineCount] = Convert.ToDouble(values[12].Replace('.', Convert.ToChar(separatorType)));

                        Array.Resize(ref Min2, Min2.Length + 1);
                        Min2[lineCount] = Convert.ToDouble(values[13].Replace('.', Convert.ToChar(separatorType)));

                        Array.Resize(ref Min3, Min3.Length + 1);
                        Min3[lineCount] = Convert.ToDouble(values[14].Replace('.', Convert.ToChar(separatorType)));

                        Array.Resize(ref Min4, Min4.Length + 1);
                        Min4[lineCount] = Convert.ToDouble(values[15].Replace('.', Convert.ToChar(separatorType)));
                        lineCount++;
                    }
                    // Меняем значение прогресс бара
                    progressBar1.Value++;
                }
                // Инициализация массива массивов и присвоене значений 
                yss = new double[15][];
                yss[0] = Avg0;
                yss[1] = Avg1;
                yss[2] = Avg2;
                yss[3] = Avg3;
                yss[4] = Avg4;
                yss[5] = Max0;
                yss[6] = Max1;
                yss[7] = Max2;
                yss[8] = Max3;
                yss[9] = Max4;
                yss[10] = Min0;
                yss[11] = Min1;
                yss[12] = Min2;
                yss[13] = Min3;
                yss[14] = Min4;
                xs = recordNumbers; // Перевод массива с датами в double для графика
            }
            isDate = false;
            progressBar1.Value = progressBar1.Maximum;
        }
        private void MasterControllerDebug()
        {
            markerDates = Array.Empty<DateTime>();
            Array.Clear(arrayOfFakePointsIndex);
            dates = Array.Empty<DateTime>();
            FindRowsCount();
            bool rowFinded = false;
            // Массивы с каждым типом данных
            double[] bootCounterQuadWordParameter = Array.Empty<double>();
            double[] DebugWordParameter1 = Array.Empty<double>();
            double[] DebugWordParameter2 = Array.Empty<double>();
            double[] DebugWordParameter3 = Array.Empty<double>();
            double[] DebugWordParameter4 = Array.Empty<double>();
            double[] DebugWordParameter5 = Array.Empty<double>();
            double[] DebugWordParameter6 = Array.Empty<double>();
            double[] DebugWordParameter7 = Array.Empty<double>();
            double[] DebugWordParameter8 = Array.Empty<double>();
            double[] DebugWordParameter9 = Array.Empty<double>();

            // Инициализация панелей и лейблов
            labels = new System.Windows.Forms.Label[10];
            panels = new Panel[10];
            labelStandartLocations = new Point[10];
            panelStandartLocations = new Point[10];

            for (byte i = 0; i < labels.Length; i++)
            {
                // Настройки для labels
                labels[i] = new System.Windows.Forms.Label
                {
                    AutoSize = true,
                    ForeColor = Color.White,
                    Font = new Font("Tobota", 10, FontStyle.Regular),
                    Location = new Point(30, 10 + i * 20),
                    Tag = i // Устанавливаем тэг лейбла (номер по счету)
                };
                labelStandartLocations[i] = labels[i].Location;
                // Подписываем все labels на события
                labels[i].MouseDown += Label_MouseDown;
                labels[i].MouseMove += Label_MouseMove;
                labels[i].MouseUp += Label_MouseUp;
                // Настройки для panels
                panels[i] = new Panel
                {
                    Location = new Point(10, 10 + i * 20),
                    Size = new Size(17, 17),
                    BackColor = colors[i]
                };
                panelStandartLocations[i] = panels[i].Location;
                // Добавляем элементы управления на панель сплитконтейнера
                splitContainer1.Panel2.Controls.Add(labels[i]);
                splitContainer1.Panel2.Controls.Add(panels[i]);
                panels[i].BringToFront();
            }

            // Текст для лейблов (название осей)
            labels[0].Text = "bootCounterQuadWordParameter";
            labels[1].Text = "DebugWordParameter1";
            labels[2].Text = "DebugWordParameter2";
            labels[3].Text = "DebugWordParameter3";
            labels[4].Text = "DebugWordParameter4";
            labels[5].Text = "DebugWordParameter5";
            labels[6].Text = "DebugWordParameter6";
            labels[7].Text = "DebugWordParameter7";
            labels[8].Text = "DebugWordParameter8";
            labels[9].Text = "DebugWordParameter9";
            labelsText = new string[10];
            for (byte i = 0; i < labels.Length; i++)
            {
                labelsText[i] = labels[i].Text;
                panels[i].ContextMenuStrip = changeColorMenu;
                panels[i].Tag = i;
            }

            progressBar1.Maximum = rowCount;
            using StreamReader reader = new(fileName);
            string line;
            int lineCount = 0;
            while ((line = reader.ReadLine()) != null)
            {
                string[] values = line.Split(new[] { ", " }, StringSplitOptions.None);
                if (values[0] == "00000000 : ")
                    rowFinded = true;
                if (rowFinded)
                {
                    for (int i = 0; i < values.Length; i++)
                        if (i == 1)
                            if (values[1] == "00/00/00 00:00:00")
                                break;
                            else
                            {
                                Array.Resize(ref dates, dates.Length + 1);
                                dates[lineCount] = DateTimeFunc(values[1], false);
                            }

                    if (values[1] != "00/00/00 00:00:00")
                    {
                        Array.Resize(ref bootCounterQuadWordParameter, bootCounterQuadWordParameter.Length + 1);
                        bootCounterQuadWordParameter[lineCount] = (double)Convert.ToInt32(values[2], 16);

                        Array.Resize(ref DebugWordParameter1, DebugWordParameter1.Length + 1);
                        DebugWordParameter1[lineCount] = (double)Convert.ToInt32(values[3], 16);

                        Array.Resize(ref DebugWordParameter2, DebugWordParameter2.Length + 1);
                        DebugWordParameter2[lineCount] = (double)Convert.ToInt32(values[4], 16);

                        Array.Resize(ref DebugWordParameter3, DebugWordParameter3.Length + 1);
                        DebugWordParameter3[lineCount] = (double)Convert.ToInt32(values[5], 16);

                        Array.Resize(ref DebugWordParameter4, DebugWordParameter4.Length + 1);
                        DebugWordParameter4[lineCount] = (double)Convert.ToInt32(values[6], 16);

                        Array.Resize(ref DebugWordParameter5, DebugWordParameter5.Length + 1);
                        DebugWordParameter5[lineCount] = (double)Convert.ToInt32(values[7], 16);

                        Array.Resize(ref DebugWordParameter6, DebugWordParameter6.Length + 1);
                        DebugWordParameter6[lineCount] = (double)Convert.ToInt32(values[8], 16);

                        Array.Resize(ref DebugWordParameter7, DebugWordParameter7.Length + 1);
                        DebugWordParameter7[lineCount] = (double)Convert.ToInt32(values[9], 16);

                        Array.Resize(ref DebugWordParameter8, DebugWordParameter8.Length + 1);
                        DebugWordParameter8[lineCount] = (double)Convert.ToInt32(values[10], 16);

                        Array.Resize(ref DebugWordParameter9, DebugWordParameter9.Length + 1);
                        DebugWordParameter9[lineCount] = (double)Convert.ToInt32(values[11], 16);
                        lineCount++;
                    }

                }
                // Меняем значение прогресс бара
                progressBar1.Value++;
            }
            progressBar1.Value = progressBar1.Maximum;
            // Инициализация массива массивов и присвоене значений 
            yss = new double[10][];
            yss[0] = bootCounterQuadWordParameter;
            yss[1] = DebugWordParameter1;
            yss[2] = DebugWordParameter2;
            yss[3] = DebugWordParameter3;
            yss[4] = DebugWordParameter4;
            yss[5] = DebugWordParameter5;
            yss[6] = DebugWordParameter6;
            yss[7] = DebugWordParameter7;
            yss[8] = DebugWordParameter8;
            yss[9] = DebugWordParameter9;
            Array.Resize(ref markerDates, dates.Length);
            Array.Copy(dates, markerDates, markerDates.Length);
            xs = dates.Select(x => x.ToOADate()).ToArray(); // Перевод массива с датами в double для графика
            isDate = true;
        }
        private void MptTelemeteredData()
        {
            FindRowsCount();
            bool rowFinded = false;
            // Массивы с каждым типом данных
            double[] recordNumbers = Array.Empty<double>();
            double[] parameter0 = Array.Empty<double>();
            double[] nodeID = Array.Empty<double>();
            double[] parameter1 = Array.Empty<double>();
            double[] retries = Array.Empty<double>();
            double[] parameter2 = Array.Empty<double>();
            double[] parameter3 = Array.Empty<double>();
            double[] parameter4 = Array.Empty<double>();
            double[] parameter5 = Array.Empty<double>();
            double[] parameter6 = Array.Empty<double>();
            double[] parameter7 = Array.Empty<double>();
            double[] parameter8 = Array.Empty<double>();
            double[] parameter9 = Array.Empty<double>();
            double[] parameter10 = Array.Empty<double>();
            double[] parameter11 = Array.Empty<double>();
            double[] parameter12 = Array.Empty<double>();
            double[] parameter13 = Array.Empty<double>();
            double[] parameter14 = Array.Empty<double>();
            double[] parameter15 = Array.Empty<double>();
            double[] parameter16 = Array.Empty<double>();
            double[] parameter17 = Array.Empty<double>();
            double[] parameter18 = Array.Empty<double>();
            double[] parameter19 = Array.Empty<double>();
            double[] parameter20 = Array.Empty<double>();
            double[] parameter21 = Array.Empty<double>();
            double[] parameter22 = Array.Empty<double>();
            double[] parameter23 = Array.Empty<double>();
            double[] parameter24 = Array.Empty<double>();
            double[] parameter25 = Array.Empty<double>();
            double[] parameter26 = Array.Empty<double>();
            double[] parameter27 = Array.Empty<double>();
            double[] parameter28 = Array.Empty<double>();
            double[] parameter29 = Array.Empty<double>();
            double[] parameter30 = Array.Empty<double>();
            double[] parameter31 = Array.Empty<double>();
            double[] parameter32 = Array.Empty<double>();
            double[] parameter33 = Array.Empty<double>();
            double[] parameter34 = Array.Empty<double>();
            double[] parameter35 = Array.Empty<double>();
            double[] parameter36 = Array.Empty<double>();
            double[] parameter37 = Array.Empty<double>();

            // Инициализация панелей и лейблов
            labels = new System.Windows.Forms.Label[40];
            panels = new Panel[40];
            labelStandartLocations = new Point[40];
            panelStandartLocations = new Point[40];

            for (byte i = 0; i < labels.Length; i++)
            {
                // Настройки для labels
                labels[i] = new System.Windows.Forms.Label();
                labels[i].ForeColor = Color.White;
                labels[i].Font = new Font("Tobota", 10, FontStyle.Regular);
                labels[i].Tag = i; // Устанавливаем тэг лейбла (номер по счету)
                // Подписываем все labels на события
                labels[i].MouseDown += Label_MouseDown;
                labels[i].MouseMove += Label_MouseMove;
                labels[i].MouseUp += Label_MouseUp;
                // Настройки для panels
                panels[i] = new Panel();
                panels[i].Size = new Size(17, 17);
                panels[i].BackColor = colors[i];

                if (i >= 0 && i <= 19)
                {
                    labels[i].Location = new Point(30, 10 + i * 20);
                    panels[i].Location = new Point(10, 10 + i * 20);
                }
                else
                {
                    labels[i].Location = new Point(30 + labels[0].Width + panels[0].Width + 20, 10 + (i - 20) * 20);
                    panels[i].Location = new Point(10 + labels[0].Width + panels[0].Width, 10 + (i - 20) * 20);
                }

                panelStandartLocations[i] = panels[i].Location;
                labelStandartLocations[i] = labels[i].Location;
                // Добавляем элементы управления на панель сплитконтейнера
                splitContainer1.Panel2.Controls.Add(labels[i]);
                splitContainer1.Panel2.Controls.Add(panels[i]);
                panels[i].BringToFront();
            }

            // Текст для лейблов (название осей)
            labels[0].Text = "par0";
            labels[1].Text = "Node ID";
            labels[2].Text = "Retries";
            for (byte i = 3; i < labels.Length; i++)
                labels[i].Text = "par" + Convert.ToString(i - 2);

            labelsText = new string[40];
            for (byte i = 0; i < labels.Length; i++)
            {
                labelsText[i] = labels[i].Text;
                panels[i].ContextMenuStrip = changeColorMenu;
                panels[i].Tag = i;
            }

            progressBar1.Maximum = rowCount;
            using (StreamReader reader = new StreamReader(fileName))
            {
                string line;
                int lineCount = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] values = line.Split(new[] { ", " }, StringSplitOptions.None);
                    if (values[0] == "00000000 : ")
                        rowFinded = true;
                    if (rowFinded)
                    {
                        string[] tmp = values[0].Split(new char[] { ' ' });
                        Array.Resize(ref recordNumbers, recordNumbers.Length + 1);
                        recordNumbers[lineCount] = Convert.ToDouble(tmp[0].Replace('.', Convert.ToChar(separatorType)));

                        Array.Resize(ref parameter0, parameter0.Length + 1);
                        parameter0[lineCount] = (double)Convert.ToInt32(values[1].Replace('.', Convert.ToChar(separatorType)), 16);

                        Array.Resize(ref nodeID, nodeID.Length + 1);
                        nodeID[lineCount] = (double)Convert.ToInt32(values[2].Replace('.', Convert.ToChar(separatorType)), 16);

                        Array.Resize(ref parameter1, parameter1.Length + 1);
                        parameter1[lineCount] = (double)Convert.ToInt32(values[3].Replace('.', Convert.ToChar(separatorType)), 16);

                        Array.Resize(ref retries, retries.Length + 1);
                        retries[lineCount] = (double)Convert.ToInt32(values[4].Replace('.', Convert.ToChar(separatorType)), 16);

                        Array.Resize(ref parameter2, parameter2.Length + 1);
                        parameter2[lineCount] = (double)Convert.ToInt32(values[5].Replace('.', Convert.ToChar(separatorType)), 16);

                        Array.Resize(ref parameter3, parameter3.Length + 1);
                        parameter3[lineCount] = (double)Convert.ToInt32(values[6].Replace('.', Convert.ToChar(separatorType)), 16);

                        Array.Resize(ref parameter4, parameter4.Length + 1);
                        parameter4[lineCount] = (double)Convert.ToInt32(values[5].Replace('.', Convert.ToChar(separatorType)), 16);

                        Array.Resize(ref parameter5, parameter5.Length + 1);
                        parameter5[lineCount] = (double)Convert.ToInt32(values[6].Replace('.', Convert.ToChar(separatorType)), 16);

                        Array.Resize(ref parameter6, parameter6.Length + 1);
                        parameter6[lineCount] = (double)Convert.ToInt32(values[5].Replace('.', Convert.ToChar(separatorType)), 16);

                        Array.Resize(ref parameter7, parameter7.Length + 1);
                        parameter7[lineCount] = (double)Convert.ToInt32(values[6].Replace('.', Convert.ToChar(separatorType)), 16);

                        Array.Resize(ref parameter8, parameter8.Length + 1);
                        parameter8[lineCount] = (double)Convert.ToInt32(values[5].Replace('.', Convert.ToChar(separatorType)), 16);

                        Array.Resize(ref parameter9, parameter9.Length + 1);
                        parameter9[lineCount] = (double)Convert.ToInt32(values[6].Replace('.', Convert.ToChar(separatorType)), 16);

                        Array.Resize(ref parameter10, parameter10.Length + 1);
                        parameter10[lineCount] = (double)Convert.ToInt32(values[5].Replace('.', Convert.ToChar(separatorType)), 16);

                        Array.Resize(ref parameter11, parameter11.Length + 1);
                        parameter11[lineCount] = (double)Convert.ToInt32(values[6].Replace('.', Convert.ToChar(separatorType)), 16);

                        Array.Resize(ref parameter12, parameter12.Length + 1);
                        parameter12[lineCount] = (double)Convert.ToInt32(values[5].Replace('.', Convert.ToChar(separatorType)), 16);

                        Array.Resize(ref parameter13, parameter13.Length + 1);
                        parameter13[lineCount] = (double)Convert.ToInt32(values[6].Replace('.', Convert.ToChar(separatorType)), 16);

                        Array.Resize(ref parameter14, parameter14.Length + 1);
                        parameter14[lineCount] = (double)Convert.ToInt32(values[5].Replace('.', Convert.ToChar(separatorType)), 16);

                        Array.Resize(ref parameter15, parameter15.Length + 1);
                        parameter15[lineCount] = (double)Convert.ToInt32(values[6].Replace('.', Convert.ToChar(separatorType)), 16);

                        Array.Resize(ref parameter16, parameter16.Length + 1);
                        parameter16[lineCount] = (double)Convert.ToInt32(values[5].Replace('.', Convert.ToChar(separatorType)), 16);

                        Array.Resize(ref parameter17, parameter17.Length + 1);
                        parameter17[lineCount] = (double)Convert.ToInt32(values[6].Replace('.', Convert.ToChar(separatorType)), 16);

                        Array.Resize(ref parameter18, parameter18.Length + 1);
                        parameter18[lineCount] = (double)Convert.ToInt32(values[5].Replace('.', Convert.ToChar(separatorType)), 16);

                        Array.Resize(ref parameter19, parameter19.Length + 1);
                        parameter19[lineCount] = (double)Convert.ToInt32(values[6].Replace('.', Convert.ToChar(separatorType)), 16);

                        Array.Resize(ref parameter20, parameter20.Length + 1);
                        parameter20[lineCount] = (double)Convert.ToInt32(values[5].Replace('.', Convert.ToChar(separatorType)), 16);

                        Array.Resize(ref parameter21, parameter21.Length + 1);
                        parameter21[lineCount] = (double)Convert.ToInt32(values[6].Replace('.', Convert.ToChar(separatorType)), 16);

                        Array.Resize(ref parameter22, parameter22.Length + 1);
                        parameter22[lineCount] = (double)Convert.ToInt32(values[5].Replace('.', Convert.ToChar(separatorType)), 16);

                        Array.Resize(ref parameter23, parameter23.Length + 1);
                        parameter23[lineCount] = (double)Convert.ToInt32(values[6].Replace('.', Convert.ToChar(separatorType)), 16);

                        Array.Resize(ref parameter24, parameter24.Length + 1);
                        parameter24[lineCount] = (double)Convert.ToInt32(values[6].Replace('.', Convert.ToChar(separatorType)), 16);

                        Array.Resize(ref parameter25, parameter25.Length + 1);
                        parameter25[lineCount] = (double)Convert.ToInt32(values[5].Replace('.', Convert.ToChar(separatorType)), 16);

                        Array.Resize(ref parameter26, parameter26.Length + 1);
                        parameter26[lineCount] = (double)Convert.ToInt32(values[6].Replace('.', Convert.ToChar(separatorType)), 16);

                        Array.Resize(ref parameter27, parameter27.Length + 1);
                        parameter27[lineCount] = (double)Convert.ToInt32(values[5].Replace('.', Convert.ToChar(separatorType)), 16);

                        Array.Resize(ref parameter28, parameter28.Length + 1);
                        parameter28[lineCount] = (double)Convert.ToInt32(values[6].Replace('.', Convert.ToChar(separatorType)), 16);

                        Array.Resize(ref parameter29, parameter29.Length + 1);
                        parameter29[lineCount] = (double)Convert.ToInt32(values[5].Replace('.', Convert.ToChar(separatorType)), 16);

                        Array.Resize(ref parameter30, parameter30.Length + 1);
                        parameter30[lineCount] = (double)Convert.ToInt32(values[6].Replace('.', Convert.ToChar(separatorType)), 16);

                        Array.Resize(ref parameter31, parameter31.Length + 1);
                        parameter31[lineCount] = (double)Convert.ToInt32(values[5].Replace('.', Convert.ToChar(separatorType)), 16);

                        Array.Resize(ref parameter32, parameter32.Length + 1);
                        parameter32[lineCount] = (double)Convert.ToInt32(values[6].Replace('.', Convert.ToChar(separatorType)), 16);

                        Array.Resize(ref parameter33, parameter33.Length + 1);
                        parameter33[lineCount] = (double)Convert.ToInt32(values[6].Replace('.', Convert.ToChar(separatorType)), 16);

                        Array.Resize(ref parameter34, parameter34.Length + 1);
                        parameter31[lineCount] = (double)Convert.ToInt32(values[5].Replace('.', Convert.ToChar(separatorType)), 16);

                        Array.Resize(ref parameter35, parameter35.Length + 1);
                        parameter35[lineCount] = (double)Convert.ToInt32(values[6].Replace('.', Convert.ToChar(separatorType)), 16);

                        Array.Resize(ref parameter36, parameter36.Length + 1);
                        parameter36[lineCount] = (double)Convert.ToInt32(values[6].Replace('.', Convert.ToChar(separatorType)), 16);

                        Array.Resize(ref parameter37, parameter37.Length + 1);
                        parameter37[lineCount] = (double)Convert.ToInt32(values[5].Replace('.', Convert.ToChar(separatorType)), 16);

                        lineCount++;
                    }
                    // Меняем значение прогресс бара
                    progressBar1.Value++;
                }
                // Инициализация массива массивов и присвоене значений 
                yss = new double[41][];
                yss[0] = parameter0;
                yss[1] = nodeID;
                yss[2] = parameter1;
                yss[3] = retries;
                yss[4] = parameter2;
                yss[5] = parameter3;
                yss[6] = parameter4;
                yss[7] = parameter5;
                yss[8] = parameter6;
                yss[9] = parameter7;
                yss[10] = parameter8;
                yss[11] = parameter9;
                yss[12] = parameter10;
                yss[13] = parameter11;
                yss[14] = parameter12;
                yss[15] = parameter13;
                yss[16] = parameter14;
                yss[17] = parameter15;
                yss[18] = parameter16;
                yss[19] = parameter17;
                yss[20] = parameter18;
                yss[21] = parameter19;
                yss[22] = parameter20;
                yss[23] = parameter21;
                yss[24] = parameter22;
                yss[25] = parameter23;
                yss[26] = parameter24;
                yss[27] = parameter25;
                yss[28] = parameter26;
                yss[29] = parameter27;
                yss[30] = parameter28;
                yss[31] = parameter29;
                yss[32] = parameter30;
                yss[33] = parameter31;
                yss[34] = parameter32;
                yss[35] = parameter33;
                yss[36] = parameter34;
                yss[37] = parameter35;
                yss[38] = parameter36;
                yss[39] = parameter37;
                xs = recordNumbers; // Перевод массива с датами в double для графика
            }
            progressBar1.Value = progressBar1.Maximum;
            isDate = false;
        }
        private void NodeStatus()
        {
            markerDates = Array.Empty<DateTime>();
            Array.Clear(arrayOfFakePointsIndex);
            dates = Array.Empty<DateTime>();
            FindRowsCount();
            bool rowFinded = false;
            // Массивы с каждым типом данных
            double[] nodeID = Array.Empty<double>();
            double[] command = Array.Empty<double>();
            double[] retries = Array.Empty<double>();
            double[] nodeStatusParameter = Array.Empty<double>();

            // Инициализация панелей и лейблов
            labels = new System.Windows.Forms.Label[4];
            panels = new Panel[4];
            labelStandartLocations = new Point[4];
            panelStandartLocations = new Point[4];

            for (byte i = 0; i < labels.Length; i++)
            {
                // Настройки для labels
                labels[i] = new System.Windows.Forms.Label
                {
                    AutoSize = true,
                    ForeColor = Color.White,
                    Font = new Font("Tobota", 10, FontStyle.Regular),
                    Location = new Point(30, 10 + i * 20),
                    Tag = i // Устанавливаем тэг лейбла (номер по счету)
                };
                labelStandartLocations[i] = labels[i].Location;
                // Подписываем все labels на события
                labels[i].MouseDown += Label_MouseDown;
                labels[i].MouseMove += Label_MouseMove;
                labels[i].MouseUp += Label_MouseUp;
                // Настройки для panels
                panels[i] = new Panel
                {
                    Location = new Point(10, 10 + i * 20),
                    Size = new Size(17, 17),
                    BackColor = colors[i]
                };
                panelStandartLocations[i] = panels[i].Location;
                // Добавляем элементы управления на панель сплитконтейнера
                splitContainer1.Panel2.Controls.Add(labels[i]);
                splitContainer1.Panel2.Controls.Add(panels[i]);
                panels[i].BringToFront();
            }

            // Текст для лейблов (название осей)
            labels[0].Text = "nodeID";
            labels[1].Text = "command";
            labels[2].Text = "retries";
            labels[3].Text = "nodeStatusParameter";
            labelsText = new string[4];
            for (byte i = 0; i < labels.Length; i++)
            {
                labelsText[i] = labels[i].Text;
                panels[i].ContextMenuStrip = changeColorMenu;
                panels[i].Tag = i;
            }
            progressBar1.Maximum = rowCount;
            using StreamReader reader = new(fileName);
            string line;
            int lineCount = 0;
            while ((line = reader.ReadLine()) != null)
            {
                string[] values = line.Split(new[] { ", " }, StringSplitOptions.None);
                if (values[0] == "00000000 : ")
                    rowFinded = true;
                if (rowFinded)
                {
                    for (int i = 0; i < values.Length; i++)
                        if (i == 1)
                        {
                            if (values[1] == "00/00/00 00:00:00")
                                break;
                            else
                            {
                                Array.Resize(ref dates, dates.Length + 1);
                                dates[lineCount] = DateTimeFunc(values[1], false);
                            }
                        }

                    if (values[1] != "00/00/00 00:00:00")
                    {
                        Array.Resize(ref nodeID, nodeID.Length + 1);
                        nodeID[lineCount] = (double)Convert.ToInt32(values[2], 16);

                        Array.Resize(ref command, command.Length + 1);
                        command[lineCount] = (double)Convert.ToInt32(values[3], 16);

                        Array.Resize(ref retries, retries.Length + 1);
                        retries[lineCount] = (double)Convert.ToInt32(values[4], 16);

                        Array.Resize(ref nodeStatusParameter, nodeStatusParameter.Length + 1);
                        nodeStatusParameter[lineCount] = (double)Convert.ToInt32(values[5], 16);
                        lineCount++;
                    }
                }
                // Меняем значение прогресс бара
                progressBar1.Value++;
            }
            progressBar1.Value = progressBar1.Maximum;
            // Инициализация массива массивов и присвоене значений 
            yss = new double[4][];
            yss[0] = nodeID;
            yss[1] = command;
            yss[2] = retries;
            yss[3] = nodeStatusParameter;
            Array.Resize(ref markerDates, dates.Length);
            Array.Copy(dates, markerDates, markerDates.Length);
            xs = dates.Select(x => x.ToOADate()).ToArray(); // Перевод массива с датами в double для графика
        }
        private void Pressure()
        {
            markerDates = Array.Empty<DateTime>();
            Array.Clear(arrayOfFakePointsIndex);
            dates = Array.Empty<DateTime>();
            FindRowsCount();
            bool rowFinded = false;
            // Массивы с каждым типом данных
            double[] PANN = Array.Empty<double>();
            double[] PMNANN = Array.Empty<double>();
            double[] PMXANN = Array.Empty<double>();
            double[] PBOR = Array.Empty<double>();
            double[] PMNBOR = Array.Empty<double>();
            double[] PMXBOR = Array.Empty<double>();
            double[] TANN = Array.Empty<double>();
            double[] TBOR = Array.Empty<double>();

            // Инициализация панелей и лейблов
            labels = new System.Windows.Forms.Label[8];
            panels = new Panel[8];
            labelStandartLocations = new Point[8];
            panelStandartLocations = new Point[8];

            for (byte i = 0; i < labels.Length; i++)
            {
                // Настройки для labels
                labels[i] = new System.Windows.Forms.Label
                {
                    ForeColor = Color.White,
                    Font = new Font("Tobota", 10, FontStyle.Regular),
                    Location = new Point(30, 10 + i * 20),
                    Tag = i // Устанавливаем тэг лейбла (номер по счету)
                };
                labelStandartLocations[i] = labels[i].Location;
                // Подписываем все labels на события
                labels[i].MouseDown += Label_MouseDown;
                labels[i].MouseMove += Label_MouseMove;
                labels[i].MouseUp += Label_MouseUp;
                // Настройки для panels
                panels[i] = new Panel
                {
                    Location = new Point(10, 10 + i * 20),
                    Size = new Size(17, 17),
                    BackColor = colors[i]
                };
                panelStandartLocations[i] = panels[i].Location;
                // Добавляем элементы управления на панель сплитконтейнера
                splitContainer1.Panel2.Controls.Add(labels[i]);
                splitContainer1.Panel2.Controls.Add(panels[i]);
                panels[i].BringToFront();
            }

            // Текст для лейблов (название осей)
            labels[0].Text = "PANN";
            labels[1].Text = "PMNANN";
            labels[2].Text = "PMXANN";
            labels[3].Text = "PBOR";
            labels[4].Text = "PMNBOR";
            labels[5].Text = "PMXBOR";
            labels[6].Text = "TANN";
            labels[7].Text = "TBOR";
            labelsText = new string[8];
            for (byte i = 0; i < labels.Length; i++)
            {
                labelsText[i] = labels[i].Text;
                panels[i].ContextMenuStrip = changeColorMenu;
                panels[i].Tag = i;
            }
            progressBar1.Maximum = rowCount;
            using StreamReader reader = new(fileName);
            string line;
            int lineCount = 0;
            while ((line = reader.ReadLine()) != null)
            {
                string[] values = line.Split(new[] { ", " }, StringSplitOptions.None);
                if (values[0] == "00000000 : ")
                    rowFinded = true;
                if (rowFinded)
                {
                    for (int i = 0; i < values.Length; i++)
                        if (i == 1)
                            if (values[1] == "00/00/00 00:00:00")
                                break;
                            else
                            {
                                Array.Resize(ref dates, dates.Length + 1);
                                dates[lineCount] = DateTimeFunc(values[1], false);
                            }

                    Array.Resize(ref PANN, PANN.Length + 1);
                    PANN[lineCount] = Convert.ToDouble(values[2].Replace('.', Convert.ToChar(separatorType)));

                    Array.Resize(ref PMNANN, PMNANN.Length + 1);
                    PMNANN[lineCount] = Convert.ToDouble(values[3].Replace('.', Convert.ToChar(separatorType)));

                    Array.Resize(ref PMXANN, PMXANN.Length + 1);
                    PMXANN[lineCount] = Convert.ToDouble(values[4].Replace('.', Convert.ToChar(separatorType)));

                    Array.Resize(ref PBOR, PBOR.Length + 1);
                    PBOR[lineCount] = Convert.ToDouble(values[5].Replace('.', Convert.ToChar(separatorType)));

                    Array.Resize(ref PMNBOR, PMNBOR.Length + 1);
                    PMNBOR[lineCount] = Convert.ToDouble(values[6].Replace('.', Convert.ToChar(separatorType)));

                    Array.Resize(ref PMXBOR, PMXBOR.Length + 1);
                    PMXBOR[lineCount] = Convert.ToDouble(values[7].Replace('.', Convert.ToChar(separatorType)));

                    Array.Resize(ref TANN, TANN.Length + 1);
                    TANN[lineCount] = Convert.ToDouble(values[8].Replace('.', Convert.ToChar(separatorType)));

                    Array.Resize(ref TBOR, TBOR.Length + 1);
                    TBOR[lineCount] = Convert.ToDouble(values[9].Replace('.', Convert.ToChar(separatorType)));
                    lineCount++;
                }
                // Меняем значение прогресс бара
                progressBar1.Value++;
            }
            progressBar1.Value = progressBar1.Maximum;
            // Инициализация массива массивов и присвоене значений 
            yss = new double[9][];
            yss[0] = PANN;
            yss[1] = PMNANN;
            yss[2] = PMXANN;
            yss[3] = PBOR;
            yss[4] = PMNBOR;
            yss[5] = PMXBOR;
            yss[6] = TANN;
            yss[7] = TBOR;
            Array.Resize(ref markerDates, dates.Length);
            Array.Copy(dates, markerDates, markerDates.Length);
            xs = dates.Select(x => x.ToOADate()).ToArray(); // Перевод массива с датами в double для графика

        }
        private void SystemStatus()
        {
            markerDates = Array.Empty<DateTime>();
            Array.Clear(arrayOfFakePointsIndex);
            dates = Array.Empty<DateTime>();
            FindRowsCount();
            bool rowFinded = false;
            // Массивы с каждым типом данных
            double[] SystemStatusParameter = Array.Empty<double>();
            double[] SystemStatusParameter2 = Array.Empty<double>();
            double[] SystemStatusParameter3 = Array.Empty<double>();
            double[] SystemStatusParameter4 = Array.Empty<double>();

            // Инициализация панелей и лейблов
            labels = new System.Windows.Forms.Label[4];
            panels = new Panel[4];
            labelStandartLocations = new Point[4];
            panelStandartLocations = new Point[4];

            for (byte i = 0; i < labels.Length; i++)
            {
                // Настройки для labels
                labels[i] = new System.Windows.Forms.Label
                {
                    AutoSize = true,
                    ForeColor = Color.White,
                    Font = new Font("Tobota", 10, FontStyle.Regular),
                    Location = new Point(30, 10 + i * 20),
                    Tag = i // Устанавливаем тэг лейбла (номер по счету)
                };
                labelStandartLocations[i] = labels[i].Location;
                // Подписываем все labels на события
                labels[i].MouseDown += Label_MouseDown;
                labels[i].MouseMove += Label_MouseMove;
                labels[i].MouseUp += Label_MouseUp;
                // Настройки для panels
                panels[i] = new Panel
                {
                    Location = new Point(10, 10 + i * 20),
                    Size = new Size(17, 17),
                    BackColor = colors[i]
                };
                panelStandartLocations[i] = panels[i].Location;
                // Добавляем элементы управления на панель сплитконтейнера
                splitContainer1.Panel2.Controls.Add(labels[i]);
                splitContainer1.Panel2.Controls.Add(panels[i]);
                panels[i].BringToFront();
            }

            // Текст для лейблов (название осей)
            labels[0].Text = "SystemStatusParameter";
            labels[1].Text = "SystemStatusParameter2";
            labels[2].Text = "SystemStatusParameter3";
            labels[3].Text = "SystemStatusParameter4";
            labelsText = new string[4];
            for (byte i = 0; i < labels.Length; i++)
            {
                labelsText[i] = labels[i].Text;
                panels[i].ContextMenuStrip = changeColorMenu;
                panels[i].Tag = i;
            }

            progressBar1.Maximum = rowCount;
            using StreamReader reader = new(fileName);
            string line;
            int lineCount = 0;
            while ((line = reader.ReadLine()) != null)
            {
                string[] values = line.Split(new[] { ", " }, StringSplitOptions.None);
                if (values[0] == "00000000 : ")
                    rowFinded = true;
                if (rowFinded)
                {
                    for (int i = 0; i < values.Length; i++)
                        if (i == 1)
                            if (values[1] == "00/00/00 00:00:00")
                                break;
                            else
                            {
                                Array.Resize(ref dates, dates.Length + 1);
                                dates[lineCount] = DateTimeFunc(values[1], false);
                            }

                    Array.Resize(ref SystemStatusParameter, SystemStatusParameter.Length + 1);
                    SystemStatusParameter[lineCount] = (double)Convert.ToInt32(values[2], 16);

                    Array.Resize(ref SystemStatusParameter2, SystemStatusParameter2.Length + 1);
                    SystemStatusParameter2[lineCount] = (double)Convert.ToInt32(values[3], 16);

                    Array.Resize(ref SystemStatusParameter3, SystemStatusParameter3.Length + 1);
                    SystemStatusParameter3[lineCount] = (double)Convert.ToInt32(values[4], 16);

                    Array.Resize(ref SystemStatusParameter4, SystemStatusParameter4.Length + 1);
                    SystemStatusParameter4[lineCount] = (double)Convert.ToInt32(values[5], 16);
                    lineCount++;
                }
                // Меняем значение прогресс бара
                progressBar1.Value++;
            }
            progressBar1.Value = progressBar1.Maximum;
            // Инициализация массива массивов и присвоене значений 
            yss = new double[4][];
            yss[0] = SystemStatusParameter;
            yss[1] = SystemStatusParameter2;
            yss[2] = SystemStatusParameter3;
            yss[3] = SystemStatusParameter4;
            Array.Resize(ref markerDates, dates.Length);
            Array.Copy(dates, markerDates, markerDates.Length);
            xs = dates.Select(x => x.ToOADate()).ToArray(); // Перевод массива с датами в double для графика
        }
        private void VMMLifeTimeData()
        {
            byte lifeTimeCode;
            progressBar1.Maximum = rowCount;

            //Создаем комбобокс для VMMLifeTimeData
            comboBox1 = new()
            {
                Location = new Point(16, 10),
                Size = new Size(235, 23),
                MaxDropDownItems = 15,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Visible = false,
            };
            comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
            recComboBox1 = new Rectangle(comboBox1.Location, comboBox1.Size);
            splitContainer1.Panel2.Controls.Add(comboBox1);
            comboBox1.Visible = true;

            using StreamReader reader = new(fileName);
            List<string> lines = new List<string>();
            while (!reader.EndOfStream)
                lines.Add(reader.ReadLine());
            reader.Close();

            int lineCount = 0;
            int index = 0;
            int fileIndex = -1;
            string[] fileStartTime = Array.Empty<string>();
            int[] fileNumber = Array.Empty<int>();
            int[] recordPeriod = Array.Empty<int>();
            int[] numberOfRecords = Array.Empty<int>();
            string[] firstLine = lines[1].Split(new[] { "," }, StringSplitOptions.None);
            if (firstLine[2].Contains("VMMTempAndAccel"))
                lifeTimeCode = 0;
            else if (firstLine[2].Contains("MinMaxTempAndAccel"))
                lifeTimeCode = 1;
            else
                lifeTimeCode = 255;

            for (int i = 1; i < lines.Count; i++)
            {
                string[] values = lines[i].Split(new[] { "," }, StringSplitOptions.None);
                if (values[0].Contains("FileNumber"))
                {
                    index = i;
                    break;
                }
                Array.Resize(ref fileNumber, fileNumber.Length + 1);
                fileNumber[fileNumber.Length - 1] = Convert.ToInt32(values[0]);

                Array.Resize(ref recordPeriod, recordPeriod.Length + 1);
                recordPeriod[recordPeriod.Length - 1] = Convert.ToInt32(values[3]);

                Array.Resize(ref numberOfRecords, numberOfRecords.Length + 1);
                numberOfRecords[numberOfRecords.Length - 1] = Convert.ToInt32(values[5]);

                Array.Resize(ref fileStartTime, fileStartTime.Length + 1);
                fileStartTime[fileStartTime.Length - 1] = values[6];
            }

            // Заполняем comboBox 
            for (int i = 0; i < fileNumber.Length; i++)
            {
                comboBox1.Items.Add(Convert.ToString(fileNumber[i]) + ") " + fileStartTime[i] + ", " + "записей: " + Convert.ToString(numberOfRecords[i]) + ", " + "интервал: " + Convert.ToString(recordPeriod[i]) + " мин");

            }

            // Запись данных
            FileData fileData = new FileData();
            if (lifeTimeCode == 0)
            {
                for (int i = index; i < lines.Count; i++)
                {
                    string[] values = lines[i].Split(new[] { "," }, StringSplitOptions.None);
                    if (values[0].Contains("FileNumber"))
                    {
                        fileData = new FileData();
                        fileData.fileCode = 0;
                        i = i + 2;
                        if (i < lines.Count)
                            values = lines[i].Split(new[] { "," }, StringSplitOptions.None);
                        else
                            break;
                        Array.Resize(ref arrayOfFileData, arrayOfFileData.Length + 1);
                        fileIndex++;
                        lineCount = 0;
                    }

                    fileData.ResizeArrays();
                    fileData.date[lineCount] = DateTimeFunc(values[0], true);
                    fileData.fileNumber = fileIndex;
                    fileData.minTemperature[lineCount] = Convert.ToInt32(values[1]);
                    fileData.maxTemperature[lineCount] = Convert.ToInt32(values[2]);
                    fileData.lateralAccel[lineCount] = Convert.ToInt32(values[3]);
                    fileData.axialAccel[lineCount] = Convert.ToInt32(values[4]);
                    fileData.xAccel[lineCount] = Convert.ToInt32(values[5]);
                    fileData.yAccel[lineCount] = Convert.ToInt32(values[6]);
                    arrayOfFileData[fileIndex] = fileData;
                    lineCount++;
                    progressBar1.Value++;
                }
            }
            else if (lifeTimeCode == 1)
            {
                DateTime startInvalidDate = new DateTime(1900, 1, 1, 0, 0, 0);
                for (int i = index; i < lines.Count; i++)
                {
                    string[] values = lines[i].Split(new[] { "," }, StringSplitOptions.None);
                    if (values[0].Contains("FileNumber"))
                    {
                        startInvalidDate = new DateTime(1900, 1, 1);
                        fileData = new FileData();
                        fileData.isCorrectDate = true;
                        fileData.fileCode = 1;
                        i = i + 2;
                        if (i < lines.Count)
                            values = lines[i].Split(new[] { "," }, StringSplitOptions.None);
                        else
                            break;
                        fileIndex++;
                        Array.Resize(ref arrayOfFileData, arrayOfFileData.Length + 1);
                        lineCount = 0;
                    }

                    fileData.ResizeArrays();
                    if (values[0].Contains("Invalid"))
                    {
                        fileData.isCorrectDate = false;
                        fileData.date[lineCount] = startInvalidDate;
                        startInvalidDate = startInvalidDate.AddMinutes(recordPeriod[fileIndex]);
                    }
                    else
                    {
                        fileData.date[lineCount] = DateTimeFunc(values[0], true);
                    }
                    fileData.fileNumber = fileIndex;
                    fileData.minTemperature[lineCount] = Convert.ToInt32(values[1]);
                    fileData.maxTemperature[lineCount] = Convert.ToInt32(values[2]);
                    fileData.lateralAccel[lineCount] = Convert.ToInt32(values[3]);
                    fileData.axialAccel[lineCount] = Convert.ToInt32(values[4]);
                    arrayOfFileData[fileIndex] = fileData;
                    lineCount++;
                    progressBar1.Value++;
                }
            }

            progressBar1.Value = progressBar1.Maximum;
            comboBox1.Visible = true;
            // Ресайз для комбоБокса
            Resize_Control(comboBox1, recComboBox1);
            Resize_Panel2(comboBox1, recComboBox1);
        }
        private void VMMAgregateDrillingPerfomance()
        {
            markerDates = Array.Empty<DateTime>();
            Array.Clear(arrayOfFakePointsIndex);
            dates = Array.Empty<DateTime>();
            arrayOfFakePointsIndex = Array.Empty<int>(); // инициализация пустого массива с фейковыми точками (-999.25)  
            bool rowFinded = false;
            // Массивы с каждым типом данных
            double[] temp = Array.Empty<double>();
            double[] gaxRms = Array.Empty<double>();
            double[] gaxSrms = Array.Empty<double>();
            double[] gaxPeak = Array.Empty<double>();
            double[] gaxSnap = Array.Empty<double>();
            double[] glatRms = Array.Empty<double>();
            double[] glatSrms = Array.Empty<double>();
            double[] glatPeak = Array.Empty<double>();
            double[] rpmInt = Array.Empty<double>();
            double[] rpmAvg = Array.Empty<double>();
            double[] rpmMax = Array.Empty<double>();
            double[] rpmMin = Array.Empty<double>();
            double[] rpmRatio = Array.Empty<double>();
            double[] rpmOpct = Array.Empty<double>();
            double[] rpmNegPct = Array.Empty<double>();
            double[] accMaxL = Array.Empty<double>();
            double[] accMinL = Array.Empty<double>();
            double[] tfdNegL = Array.Empty<double>();
            double[] tfdNegH = Array.Empty<double>();
            double[] chatMax = Array.Empty<double>();
            double[] chatMin = Array.Empty<double>();
            double[] chatFreq = Array.Empty<double>();
            double[] accMaxH = Array.Empty<double>();
            double[] accMinH = Array.Empty<double>();
            double[] chatEnrg = Array.Empty<double>();

            // Инициализация панелей и лейблов
            labels = new System.Windows.Forms.Label[25];
            panels = new Panel[25];
            labelStandartLocations = new Point[25];
            panelStandartLocations = new Point[25];

            for (byte i = 0; i < labels.Length; i++)
            {
                // Настройки для labels
                labels[i] = new System.Windows.Forms.Label();
                labels[i].ForeColor = Color.White;
                labels[i].Font = new Font("Tobota", 10, FontStyle.Regular);
                labels[i].Tag = i; // Устанавливаем тэг лейбла (номер по счету)
                // Подписываем все labels на события
                labels[i].MouseDown += Label_MouseDown;
                labels[i].MouseMove += Label_MouseMove;
                labels[i].MouseUp += Label_MouseUp;
                // Настройки для panels
                panels[i] = new Panel();
                panels[i].Size = new Size(17, 17);
                panels[i].BackColor = colors[i];

                if (i >= 0 && i <= 19)
                {
                    labels[i].Location = new Point(30, 10 + i * 20);
                    panels[i].Location = new Point(10, 10 + i * 20);
                }
                else
                {
                    labels[i].Location = new Point(30 + labels[0].Width + panels[0].Width + 20, 10 + (i - 20) * 20);
                    panels[i].Location = new Point(10 + labels[0].Width + panels[0].Width, 10 + (i - 20) * 20);
                }

                panelStandartLocations[i] = panels[i].Location;
                labelStandartLocations[i] = labels[i].Location;
                // Добавляем элементы управления на панель сплитконтейнера
                splitContainer1.Panel2.Controls.Add(labels[i]);
                splitContainer1.Panel2.Controls.Add(panels[i]);
                panels[i].BringToFront();

            }
            // Текст для лейблов (название осей)
            labels[0].Text = "temp";
            labels[1].Text = "gaxR";
            labels[2].Text = "gaxSrms";
            labels[3].Text = "gaxPeak";
            labels[4].Text = "gaxSnap";
            labels[5].Text = "glatRms";
            labels[6].Text = "glatSrms";
            labels[7].Text = "glatPeak";
            labels[8].Text = "rpmInt";
            labels[9].Text = "rpmAvg";
            labels[10].Text = "rpmMax";
            labels[11].Text = "rpmMin";
            labels[12].Text = "rpmRatio";
            labels[13].Text = "rpmOpct";
            labels[14].Text = "rpmNegPct";
            labels[15].Text = "accMaxL";
            labels[16].Text = "accMinL";
            labels[17].Text = "tfdNegL";
            labels[18].Text = "tfdNegH";
            labels[19].Text = "chatMax";
            labels[20].Text = "chatMin";
            labels[21].Text = "chatFreq";
            labels[22].Text = "accMaxH";
            labels[23].Text = "accMinH";
            labels[24].Text = "chatEnrg";

            labelsText = new string[25];
            for (byte i = 0; i < labels.Length; i++)
            {
                labelsText[i] = labels[i].Text;
                panels[i].ContextMenuStrip = changeColorMenu;
                panels[i].Tag = i;
            }

            progressBar1.Maximum = rowCount;
            using StreamReader reader = new(fileName);
            string line;
            int lineCount = 0;
            while ((line = reader.ReadLine()) != null)
            {
                string[] values = line.Split(new[] { ", " }, StringSplitOptions.None);
                if (values[0].Contains("00000000 : "))
                    rowFinded = true;
                if (rowFinded)
                {
                    string[] tmp = values[0].Split(new[] { " " }, StringSplitOptions.None);
                    string stringDate = tmp[2];
                    string stringTime = tmp[3];
                    if (stringDate.Contains("Jan"))
                        stringDate = stringDate.Replace("Jan", "01");
                    else if (stringDate.Contains("Feb"))
                        stringDate = stringDate.Replace("Feb", "02");
                    else if (stringDate.Contains("Mar"))
                        stringDate = stringDate.Replace("Mar", "03");
                    else if (stringDate.Contains("Apr"))
                        stringDate = stringDate.Replace("Apr", "04");
                    else if (stringDate.Contains("May"))
                        stringDate = stringDate.Replace("May", "05");
                    else if (stringDate.Contains("Jun"))
                        stringDate = stringDate.Replace("Jun", "06");
                    else if (stringDate.Contains("Jul"))
                        stringDate = stringDate.Replace("Jul", "07");
                    else if (stringDate.Contains("Aug"))
                        stringDate = stringDate.Replace("Aug", "08");
                    else if (stringDate.Contains("Sep"))
                        stringDate = stringDate.Replace("Sep", "09");
                    else if (stringDate.Contains("Oct"))
                        stringDate = stringDate.Replace("Oct", "10");
                    else if (stringDate.Contains("Nov"))
                        stringDate = stringDate.Replace("Nov", "11");
                    else if (stringDate.Contains("Dec"))
                        stringDate = stringDate.Replace("Dec", "12");

                    stringDate.Remove(0, 2);
                    stringDate = stringDate + " " + stringTime;
                    Array.Resize(ref dates, dates.Length + 1);
                    dates[lineCount] = DateTimeFunc(stringDate, false);

                    if (!values[1].Contains("00/00/00 00:00:00"))
                    {
                        Array.Resize(ref temp, temp.Length + 1);
                        temp[lineCount] = Convert.ToDouble(values[1].Replace('.', Convert.ToChar(separatorType)));
                        Array.Resize(ref gaxRms, gaxRms.Length + 1);
                        gaxRms[lineCount] = Convert.ToDouble(values[2].Replace('.', Convert.ToChar(separatorType)));
                        Array.Resize(ref gaxSrms, gaxSrms.Length + 1);
                        gaxSrms[lineCount] = (double)Convert.ToDouble(values[3].Replace('.', Convert.ToChar(separatorType)));
                        Array.Resize(ref gaxPeak, gaxPeak.Length + 1);
                        gaxPeak[lineCount] = (double)Convert.ToDouble(values[4].Replace('.', Convert.ToChar(separatorType)));
                        Array.Resize(ref gaxSnap, gaxSnap.Length + 1);
                        gaxSnap[lineCount] = (double)Convert.ToDouble(values[5].Replace('.', Convert.ToChar(separatorType)));
                        Array.Resize(ref glatRms, glatRms.Length + 1);
                        glatRms[lineCount] = (double)Convert.ToDouble(values[6].Replace('.', Convert.ToChar(separatorType)));
                        Array.Resize(ref glatSrms, glatSrms.Length + 1);
                        glatSrms[lineCount] = (double)Convert.ToDouble(values[7].Replace('.', Convert.ToChar(separatorType)));
                        Array.Resize(ref glatPeak, glatPeak.Length + 1);
                        glatPeak[lineCount] = (double)Convert.ToDouble(values[8].Replace('.', Convert.ToChar(separatorType)));
                        Array.Resize(ref rpmInt, rpmInt.Length + 1);
                        rpmInt[lineCount] = (double)Convert.ToDouble(values[9].Replace('.', Convert.ToChar(separatorType)));
                        Array.Resize(ref rpmAvg, rpmAvg.Length + 1);
                        rpmAvg[lineCount] = (double)Convert.ToDouble(values[10].Replace('.', Convert.ToChar(separatorType)));
                        Array.Resize(ref rpmMax, rpmMax.Length + 1);
                        rpmMax[lineCount] = (double)Convert.ToDouble(values[11].Replace('.', Convert.ToChar(separatorType)));
                        Array.Resize(ref rpmMin, rpmMin.Length + 1);
                        rpmMin[lineCount] = (double)Convert.ToDouble(values[12].Replace('.', Convert.ToChar(separatorType)));
                        Array.Resize(ref rpmRatio, rpmRatio.Length + 1);
                        rpmRatio[lineCount] = (double)Convert.ToDouble(values[13].Replace('.', Convert.ToChar(separatorType)));
                        Array.Resize(ref rpmOpct, rpmOpct.Length + 1);
                        rpmOpct[lineCount] = (double)Convert.ToDouble(values[14].Replace('.', Convert.ToChar(separatorType)));
                        Array.Resize(ref rpmNegPct, rpmNegPct.Length + 1);
                        rpmNegPct[lineCount] = (double)Convert.ToDouble(values[15].Replace('.', Convert.ToChar(separatorType)));
                        Array.Resize(ref accMaxL, accMaxL.Length + 1);
                        accMaxL[lineCount] = (double)Convert.ToDouble(values[16].Replace('.', Convert.ToChar(separatorType)));
                        Array.Resize(ref accMinL, accMinL.Length + 1);
                        accMinL[lineCount] = (double)Convert.ToDouble(values[17].Replace('.', Convert.ToChar(separatorType)));
                        Array.Resize(ref tfdNegL, tfdNegL.Length + 1);
                        tfdNegL[lineCount] = (double)Convert.ToDouble(values[18].Replace('.', Convert.ToChar(separatorType)));
                        Array.Resize(ref tfdNegH, tfdNegH.Length + 1);
                        tfdNegH[lineCount] = (double)Convert.ToDouble(values[19].Replace('.', Convert.ToChar(separatorType)));
                        Array.Resize(ref chatMax, chatMax.Length + 1);
                        chatMax[lineCount] = (double)Convert.ToDouble(values[20].Replace('.', Convert.ToChar(separatorType)));
                        Array.Resize(ref chatMin, chatMin.Length + 1);
                        chatMin[lineCount] = (double)Convert.ToDouble(values[21].Replace('.', Convert.ToChar(separatorType)));
                        Array.Resize(ref chatFreq, chatFreq.Length + 1);
                        chatFreq[lineCount] = (double)Convert.ToDouble(values[22].Replace('.', Convert.ToChar(separatorType)));
                        Array.Resize(ref accMaxH, accMaxH.Length + 1);
                        accMaxH[lineCount] = (double)Convert.ToDouble(values[23].Replace('.', Convert.ToChar(separatorType)));
                        Array.Resize(ref accMinH, accMinH.Length + 1);
                        accMinH[lineCount] = (double)Convert.ToDouble(values[24].Replace('.', Convert.ToChar(separatorType)));
                        Array.Resize(ref chatEnrg, chatEnrg.Length + 1);
                        values[25] = values[25].Remove(values[25].Length - 1);
                        chatEnrg[lineCount] = (double)Convert.ToDouble(values[25].Replace('.', Convert.ToChar(separatorType)));
                        lineCount++;
                    }
                }
                // Меняем значение прогресс бара
                progressBar1.Value++;
            }
            progressBar1.Value = progressBar1.Maximum;
            // Инициализация массива массивов и присвоение значений 
            yss = new double[25][];
            yss[0] = temp;
            yss[1] = gaxRms;
            yss[2] = gaxSrms;
            yss[3] = gaxPeak;
            yss[4] = gaxSnap;
            yss[5] = glatRms;
            yss[6] = glatSrms;
            yss[7] = glatPeak;
            yss[8] = rpmInt;
            yss[9] = rpmAvg;
            yss[10] = rpmMax;
            yss[11] = rpmMin;
            yss[12] = rpmRatio;
            yss[13] = rpmOpct;
            yss[14] = rpmNegPct;
            yss[15] = accMaxL;
            yss[16] = accMinL;
            yss[17] = tfdNegL;
            yss[18] = tfdNegH;
            yss[19] = chatMax;
            yss[20] = chatMin;
            yss[21] = chatFreq;
            yss[22] = accMaxH;
            yss[23] = accMinH;
            yss[24] = chatEnrg;
            Array.Resize(ref markerDates, dates.Length);
            Array.Copy(dates, markerDates, markerDates.Length);
            xs = dates.Select(x => x.ToOADate()).ToArray(); // Перевод массива с датами в double для графика
            isDate = true;
        }
        private void VMMDiagnostics()
        {
            markerDates = Array.Empty<DateTime>();
            Array.Clear(arrayOfFakePointsIndex);
            dates = Array.Empty<DateTime>();
            arrayOfFakePointsIndex = Array.Empty<int>(); // инициализация пустого массива с фейковыми точками (-999.25)  
            bool rowFinded = false;
            // Массивы с каждым типом данных
            double[] temp = Array.Empty<double>();
            double[] avgBus = Array.Empty<double>();
            double[] maxBus = Array.Empty<double>();
            double[] minBus = Array.Empty<double>();
            double[] status = Array.Empty<double>();
            double[] magPct = Array.Empty<double>();
            double[] magDip = Array.Empty<double>();
            double[] vXOff = Array.Empty<double>();
            double[] vYOff = Array.Empty<double>();
            double[] vZOff = Array.Empty<double>();
            double[] xMOff = Array.Empty<double>();
            double[] yMOff = Array.Empty<double>();
            double[] xMScl = Array.Empty<double>();
            double[] yMScl = Array.Empty<double>();

            // Инициализация панелей и лейблов
            labels = new System.Windows.Forms.Label[14];
            panels = new Panel[14];
            labelStandartLocations = new Point[14];
            panelStandartLocations = new Point[14];

            for (byte i = 0; i < labels.Length; i++)
            {
                // Настройки для labels
                labels[i] = new System.Windows.Forms.Label();
                labels[i].ForeColor = Color.White;
                labels[i].Font = new Font("Tobota", 10, FontStyle.Regular);
                labels[i].Tag = i; // Устанавливаем тэг лейбла (номер по счету)
                // Подписываем все labels на события
                labels[i].MouseDown += Label_MouseDown;
                labels[i].MouseMove += Label_MouseMove;
                labels[i].MouseUp += Label_MouseUp;
                // Настройки для panels
                panels[i] = new Panel();
                panels[i].Size = new Size(17, 17);
                panels[i].BackColor = colors[i];

                if (i >= 0 && i <= 19)
                {
                    labels[i].Location = new Point(30, 10 + i * 20);
                    panels[i].Location = new Point(10, 10 + i * 20);
                }
                else
                {
                    labels[i].Location = new Point(30 + labels[0].Width + panels[0].Width + 20, 10 + (i - 20) * 20);
                    panels[i].Location = new Point(10 + labels[0].Width + panels[0].Width, 10 + (i - 20) * 20);
                }

                panelStandartLocations[i] = panels[i].Location;
                labelStandartLocations[i] = labels[i].Location;
                // Добавляем элементы управления на панель сплитконтейнера
                splitContainer1.Panel2.Controls.Add(labels[i]);
                splitContainer1.Panel2.Controls.Add(panels[i]);
                panels[i].BringToFront();

            }
            // Текст для лейблов (название осей)
            labels[0].Text = "temp";
            labels[1].Text = "avgBus";
            labels[2].Text = "maxBus";
            labels[3].Text = "minBus";
            labels[4].Text = "status";
            labels[5].Text = "magPct";
            labels[6].Text = "magDip";
            labels[7].Text = "vXOff";
            labels[8].Text = "vYOff";
            labels[9].Text = "vZOff";
            labels[10].Text = "xMOff";
            labels[11].Text = "yMOff";
            labels[12].Text = "xMScl";
            labels[13].Text = "yMScl";

            labelsText = new string[14];
            for (byte i = 0; i < labels.Length; i++)
            {
                labelsText[i] = labels[i].Text;
                panels[i].ContextMenuStrip = changeColorMenu;
                panels[i].Tag = i;
            }

            progressBar1.Maximum = rowCount;
            using StreamReader reader = new(fileName);
            string line;
            int lineCount = 0;
            while ((line = reader.ReadLine()) != null)
            {
                string[] values = line.Split(new[] { ", " }, StringSplitOptions.None);
                if (values[0].Contains("00000000 : "))
                    rowFinded = true;
                if (rowFinded)
                {
                    string[] tmp = values[0].Split(new[] { " " }, StringSplitOptions.None);
                    string stringDate = tmp[2];
                    string stringTime = tmp[3];
                    if (stringDate.Contains("Jan"))
                        stringDate = stringDate.Replace("Jan", "01");
                    else if (stringDate.Contains("Feb"))
                        stringDate = stringDate.Replace("Feb", "02");
                    else if (stringDate.Contains("Mar"))
                        stringDate = stringDate.Replace("Mar", "03");
                    else if (stringDate.Contains("Apr"))
                        stringDate = stringDate.Replace("Apr", "04");
                    else if (stringDate.Contains("May"))
                        stringDate = stringDate.Replace("May", "05");
                    else if (stringDate.Contains("Jun"))
                        stringDate = stringDate.Replace("Jun", "06");
                    else if (stringDate.Contains("Jul"))
                        stringDate = stringDate.Replace("Jul", "07");
                    else if (stringDate.Contains("Aug"))
                        stringDate = stringDate.Replace("Aug", "08");
                    else if (stringDate.Contains("Sep"))
                        stringDate = stringDate.Replace("Sep", "09");
                    else if (stringDate.Contains("Oct"))
                        stringDate = stringDate.Replace("Oct", "10");
                    else if (stringDate.Contains("Nov"))
                        stringDate = stringDate.Replace("Nov", "11");
                    else if (stringDate.Contains("Dec"))
                        stringDate = stringDate.Replace("Dec", "12");

                    stringDate.Remove(0, 2);
                    stringDate = stringDate + " " + stringTime;
                    Array.Resize(ref dates, dates.Length + 1);
                    dates[lineCount] = DateTimeFunc(stringDate, false);

                    if (!values[1].Contains("00/00/00 00:00:00"))
                    {
                        Array.Resize(ref temp, temp.Length + 1);
                        temp[lineCount] = Convert.ToDouble(values[1].Replace('.', Convert.ToChar(separatorType)));
                        Array.Resize(ref avgBus, avgBus.Length + 1);
                        avgBus[lineCount] = Convert.ToDouble(values[2].Replace('.', Convert.ToChar(separatorType)));
                        Array.Resize(ref maxBus, maxBus.Length + 1);
                        maxBus[lineCount] = (double)Convert.ToDouble(values[3].Replace('.', Convert.ToChar(separatorType)));
                        Array.Resize(ref minBus, minBus.Length + 1);
                        minBus[lineCount] = (double)Convert.ToDouble(values[4].Replace('.', Convert.ToChar(separatorType)));
                        Array.Resize(ref status, status.Length + 1);
                        status[lineCount] = (double)Convert.ToDouble(values[5].Replace('.', Convert.ToChar(separatorType)));
                        Array.Resize(ref magPct, magPct.Length + 1);
                        magPct[lineCount] = (double)Convert.ToDouble(values[6].Replace('.', Convert.ToChar(separatorType)));
                        Array.Resize(ref magDip, magDip.Length + 1);
                        magDip[lineCount] = (double)Convert.ToDouble(values[7].Replace('.', Convert.ToChar(separatorType)));
                        Array.Resize(ref vXOff, vXOff.Length + 1);
                        vXOff[lineCount] = (double)Convert.ToDouble(values[8].Replace('.', Convert.ToChar(separatorType)));
                        Array.Resize(ref vYOff, vYOff.Length + 1);
                        vYOff[lineCount] = (double)Convert.ToDouble(values[9].Replace('.', Convert.ToChar(separatorType)));
                        Array.Resize(ref vZOff, vZOff.Length + 1);
                        vZOff[lineCount] = (double)Convert.ToDouble(values[10].Replace('.', Convert.ToChar(separatorType)));
                        Array.Resize(ref xMOff, xMOff.Length + 1);
                        xMOff[lineCount] = (double)Convert.ToDouble(values[11].Replace('.', Convert.ToChar(separatorType)));
                        Array.Resize(ref yMOff, yMOff.Length + 1);
                        yMOff[lineCount] = (double)Convert.ToDouble(values[12].Replace('.', Convert.ToChar(separatorType)));
                        Array.Resize(ref xMScl, xMScl.Length + 1);
                        xMScl[lineCount] = (double)Convert.ToDouble(values[13].Replace('.', Convert.ToChar(separatorType)));
                        Array.Resize(ref yMScl, yMScl.Length + 1);
                        values[14] = values[14].Remove(values[14].Length - 1);
                        yMScl[lineCount] = (double)Convert.ToDouble(values[14].Replace('.', Convert.ToChar(separatorType)));
                        lineCount++;
                    }
                }
                // Меняем значение прогресс бара
                progressBar1.Value++;
            }
            progressBar1.Value = progressBar1.Maximum;
            // Инициализация массива массивов и присвоение значений 
            yss = new double[25][];
            yss[0] = temp;
            yss[1] = avgBus;
            yss[2] = maxBus;
            yss[3] = minBus;
            yss[4] = status;
            yss[5] = magPct;
            yss[6] = magDip;
            yss[7] = vXOff;
            yss[8] = vYOff;
            yss[9] = vZOff;
            yss[10] = xMOff;
            yss[11] = yMOff;
            yss[12] = xMScl;
            yss[13] = yMScl;
            Array.Resize(ref markerDates, dates.Length);
            Array.Copy(dates, markerDates, markerDates.Length);
            xs = dates.Select(x => x.ToOADate()).ToArray(); // Перевод массива с датами в double для графика
            isDate = true;
        }
        // Класс с инофрмацией о каждом файле записи в файле типа LifeTimeData
        private class FileData
        {
            public byte fileCode;
            public bool isCorrectDate;
            public int fileNumber;
            public double[] minTemperature, maxTemperature, lateralAccel, axialAccel, xAccel, yAccel;
            public DateTime[] date;

            public FileData()
            {
                fileNumber = 0;
                minTemperature = Array.Empty<double>();
                maxTemperature = Array.Empty<double>();
                lateralAccel = Array.Empty<double>();
                axialAccel = Array.Empty<double>();
                xAccel = Array.Empty<double>();
                yAccel = Array.Empty<double>();
                date = Array.Empty<DateTime>();
            }
            public void ResizeArrays()
            {
                Array.Resize(ref minTemperature, minTemperature.Length + 1);
                Array.Resize(ref maxTemperature, maxTemperature.Length + 1);
                Array.Resize(ref lateralAccel, lateralAccel.Length + 1);
                Array.Resize(ref axialAccel, axialAccel.Length + 1);
                Array.Resize(ref xAccel, xAccel.Length + 1);
                Array.Resize(ref yAccel, yAccel.Length + 1);
                Array.Resize(ref date, date.Length + 1);
            }
        }
        // ComboBox event
        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            markerDates = Array.Empty<DateTime>();
            comboIndex = Convert.ToInt32(comboBox1.SelectedIndex); // Индекс выбранной строки комбобокса (файла)
            //Обработка ошибки чтения файла
            if (comboIndex >= arrayOfFileData.Length)
            {
                MessageBox.Show("Ошибка чтения файла");
                return;
            }
            double[] pv, hv, cv; // Массивы для создания прямых линий (уровней)
            //Чистка графических окон
            for (byte i = 0; i < arrayOfGraphWindows.Length; i++)
            {
                arrayOfGraphWindows[i].formsPlot.Plot.Clear();
                arrayOfGraphWindows[i].formsPlot.Refresh();
                for (byte j = 0; j < arrayOfGraphWindows[i].isOnThePlot.Length; j++)
                    arrayOfGraphWindows[i].isOnThePlot[j] = false;
            }

            arrayOfGraphWindows[0].formsPlot.Plot.Add(myCrosshair);

            // Чистка лейблов и панелей 
            if (panels != null && labels != null)
            {
                for (int i = 0; i < panels.Length; i++)
                {
                    panels[i]?.Dispose();
                    labels[i]?.Dispose();
                }
                Array.Clear(labels);
                Array.Clear(panels);
            }
            // Создание лейблов и панелей (в зависимости от типа файла)
            if (arrayOfFileData[comboIndex].fileCode == 0)
            {
                labels = new System.Windows.Forms.Label[9];
                panels = new Panel[9];
                labelStandartLocations = new Point[9];
                panelStandartLocations = new Point[9];
            }
            else if (arrayOfFileData[comboIndex].fileCode == 1)
            {
                labels = new System.Windows.Forms.Label[7];
                panels = new Panel[7];
                labelStandartLocations = new Point[7];
                panelStandartLocations = new Point[7];
            }
            // Изменение свойств лейблов и панелей
            for (int i = 0; i < labels.Length; i++)
            {
                labels[i] = new System.Windows.Forms.Label
                {
                    Font = new Font("Tobota", 10, FontStyle.Regular),
                    Location = new Point(30, 40 + i * 20),
                    Width = 280,
                    Tag = i // Устанавливаем тэг лейбла (номер по счету)
                };

                if (isLightTheme)
                    labels[i].ForeColor = Color.Black;
                else
                    labels[i].ForeColor = Color.White;

                labelStandartLocations[i] = labels[i].Location;
                // Подписываем все labels на события
                labels[i].MouseDown += Label_MouseDown;
                labels[i].MouseMove += Label_MouseMove;
                labels[i].MouseUp += Label_MouseUp;
                // Настройки для panels
                panels[i] = new Panel
                {
                    Location = new Point(10, 40 + i * 20),
                    Size = new Size(17, 17),
                    BackColor = colors[i]
                };
                panelStandartLocations[i] = panels[i].Location;
                // Добавляем элементы управления на панель сплитконтейнера
                splitContainer1.Panel2.Controls.Add(labels[i]);
                splitContainer1.Panel2.Controls.Add(panels[i]);
                panels[i].BringToFront();
            }

            //Добавление текста лейблам и заполнение массива с текстами (для передачи в метод Draw класса GraphWindow
            labels[0].Text = "MinTemperature";
            labels[1].Text = "MaxTemperature";
            labels[2].Text = "LateralAccel (Поперечные вибрации)";
            labels[3].Text = "AxialAccel (Осевые вибрации)";
            if (arrayOfFileData[comboIndex].fileCode == 0)
            {
                labels[4].Text = "X-Accel";
                labels[5].Text = "Y-Accel";
                labels[6].Text = "Уровень допустимых вибраций";
                labels[7].Text = "Уровень высоких вибраций";
                labels[8].Text = "Уровень критических вибраций";
                labelsText = new string[9];
            }
            else if (arrayOfFileData[comboIndex].fileCode == 1)
            {
                labels[4].Text = "Уровень допустимых вибраций";
                labels[5].Text = "Уровень высоких вибраций";
                labels[6].Text = "Уровень критических вибраций";
                labelsText = new string[7];
            }

            for (byte i = 0; i < labels.Length; i++)
            {
                labelsText[i] = labels[i].Text;
                panels[i].ContextMenuStrip = changeColorMenu;
                panels[i].Tag = i;
            }

            //-------------------------------

            progressBar1.Maximum = rowCount;
            // Инициализация массивов для линий 
            pv = new double[arrayOfFileData[comboIndex].minTemperature.Length];
            hv = new double[arrayOfFileData[comboIndex].minTemperature.Length];
            cv = new double[arrayOfFileData[comboIndex].minTemperature.Length];
            // Установка значений элементов массивов для линий (уровней)
            for (int i = 0; i < pv.Length; i++)
            {
                pv[i] = permissibleVibrationLevel;
                hv[i] = highVibrationLevel;
                cv[i] = criticalVibrationLevel;
            }
            Array.Resize(ref markerDates, arrayOfFileData[comboIndex].date.Length);
            Array.Copy(arrayOfFileData[comboIndex].date, markerDates, arrayOfFileData[comboIndex].date.Length);
            xs = arrayOfFileData[comboIndex].date.Select(x => x.ToOADate()).ToArray(); // Запись в глобальный массив для оси ОХ даты
            // Создание и заполнение глобального массива с данными
            yss = new double[9][];
            yss[0] = arrayOfFileData[comboIndex].minTemperature;
            yss[1] = arrayOfFileData[comboIndex].maxTemperature;
            yss[2] = arrayOfFileData[comboIndex].lateralAccel;
            yss[3] = arrayOfFileData[comboIndex].axialAccel;
            if (arrayOfFileData[comboIndex].fileCode == 0)
            {
                yss[4] = arrayOfFileData[comboIndex].xAccel;
                yss[5] = arrayOfFileData[comboIndex].yAccel;
                yss[6] = pv;
                yss[7] = hv;
                yss[8] = cv;
            }
            else if (arrayOfFileData[comboIndex].fileCode == 1)
            {
                yss[4] = pv;
                yss[5] = hv;
                yss[6] = cv;
            }

            // Отрисовка графика сразу при выборе интервала (если включен соответствующий пункт в контекстном меню)
            if (fastDrawVMM.Checked)
            {
                if (arrayOfFileData[comboIndex].fileCode == 0)
                {
                    arrayOfGraphWindows[0].Draw(2, xs, yss, colors, "LateralAccel", isDate, arrayOfFakePointsIndex, specialColor, isFakePointsEnabled, isLegenedHidden);
                    arrayOfGraphWindows[0].Draw(3, xs, yss, colors, "AxialAccel", isDate, arrayOfFakePointsIndex, specialColor, isFakePointsEnabled, isLegenedHidden);
                    arrayOfGraphWindows[0].Draw(6, xs, yss, colors, "Уровень допустимых вибраций", isDate, arrayOfFakePointsIndex, specialColor, isFakePointsEnabled, isLegenedHidden);
                    arrayOfGraphWindows[0].Draw(7, xs, yss, colors, "Уровень высоких выибраций", isDate, arrayOfFakePointsIndex, specialColor, isFakePointsEnabled, isLegenedHidden);
                    arrayOfGraphWindows[0].Draw(8, xs, yss, colors, "Уровень критических выибраций", isDate, arrayOfFakePointsIndex, specialColor, isFakePointsEnabled, isLegenedHidden);
                }
                else if (arrayOfFileData[comboIndex].fileCode == 1)
                {
                    arrayOfGraphWindows[0].Draw(2, xs, yss, colors, "LateralAccel", isDate, arrayOfFakePointsIndex, specialColor, isFakePointsEnabled, isLegenedHidden);
                    arrayOfGraphWindows[0].Draw(3, xs, yss, colors, "AxialAccel", isDate, arrayOfFakePointsIndex, specialColor, isFakePointsEnabled, isLegenedHidden);
                    arrayOfGraphWindows[0].Draw(4, xs, yss, colors, "Уровень допустимых вибраций", isDate, arrayOfFakePointsIndex, specialColor, isFakePointsEnabled, isLegenedHidden);
                    arrayOfGraphWindows[0].Draw(5, xs, yss, colors, "Уровень высоких выибраций", isDate, arrayOfFakePointsIndex, specialColor, isFakePointsEnabled, isLegenedHidden);
                    arrayOfGraphWindows[0].Draw(6, xs, yss, colors, "Уровень критических выибраций", isDate, arrayOfFakePointsIndex, specialColor, isFakePointsEnabled, isLegenedHidden);
                }

            }

            arrayOfGraphWindows[0].formsPlot.Plot.AxisAuto(); // Авто зум на все данные
            arrayOfGraphWindows[0].formsPlot.Refresh();
            isFileOpened = true;
            isVMMDrawed = true;
        }
        //Label events
        private void Label_MouseDown(object sender, MouseEventArgs e)
        {
            System.Windows.Forms.Label? label = sender as System.Windows.Forms.Label;
            draggIndex = Convert.ToInt32(label.Tag);
            labelStartLocation = new Point(e.X, e.Y);
            isDragging = true;
        }

        private void Label_MouseMove(object sender, MouseEventArgs e)
        {
            System.Windows.Forms.Label? label = sender as System.Windows.Forms.Label;
            Int32 labelIndex = Convert.ToInt32(label.Tag); ;
            int labelX, labelY;
            if (isDragging && labelIndex == draggIndex)
            {
                labelX = e.X - labelStartLocation.X;
                labelY = e.Y - labelStartLocation.Y;
                labels[labelIndex].Location = new Point(label.Location.X + labelX, label.Location.Y + labelY);
                panels[labelIndex].Location = new Point(panels[labelIndex].Location.X + labelX, panels[labelIndex].Location.Y + labelY);
                panels[labelIndex].BringToFront();
            }
        }

        private void Label_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
            System.Windows.Forms.Label? label = sender as System.Windows.Forms.Label;
            Int32 labelIndex = Convert.ToInt32(label.Tag);
            label.Location = new Point(labelStandartLocations[labelIndex].X, labelStandartLocations[labelIndex].Y);
            panels[labelIndex].Location = new Point(panelStandartLocations[labelIndex].X, panelStandartLocations[labelIndex].Y);
            _ = splitContainer1.Panel2.PointToClient(Control.MousePosition);
            Point mousePosGlobal = Control.MousePosition;
            _ = splitContainer1.Panel1.PointToClient(mousePosGlobal);
            foreach (Control chartControl in splitContainer1.Panel1.Controls)
            {
                if (chartControl is ScottPlot.FormsPlot plot && plot.Width > 0 && plot.Height > 0)
                {
                    Rectangle chartAreaOnScreen = splitContainer1.Panel1.RectangleToScreen(plot.Bounds);

                    if (chartAreaOnScreen.Contains(mousePosGlobal))
                    {
                        byte i;
                        for (i = 0; i < arrayOfGraphWindows.Length; i++)
                        {
                            if (arrayOfGraphWindows[i].formsPlot.Tag == plot.Tag)
                            {
                                byte chartTag = (byte)plot.Tag;
                                if (!arrayOfGraphWindows[chartTag].isOnThePlot[labelIndex])
                                    arrayOfGraphWindows[chartTag].Draw(labelIndex, xs, yss, colors, label.Text, isDate, arrayOfFakePointsIndex, specialColor, isFakePointsEnabled, isLegenedHidden);
                                else
                                    arrayOfGraphWindows[chartTag].RemoveChart(labelIndex, labelsText);
                                break;
                            }
                        }

                    }
                }
            }
        }

        // События мыши на графике
        private void Plot_MouseMove(object sender, MouseEventArgs e)
        {
            if (isCntrlPressed)
            {
                var mouseCoordinates = arrayOfGraphWindows[0].formsPlot.GetMouseCoordinates();
                if (magnetCH.Checked && arrayOfGraphWindows[0].plottables.Length > 0)
                {
                    var pointCoordinates = arrayOfGraphWindows[0].plottables[0].GetPointNearestX(mouseCoordinates.x);
                    double distance, minDistance;
                    int nearestPlottableIndex = 0;
                    minDistance = Math.Sqrt((mouseCoordinates.x - pointCoordinates.x) * (mouseCoordinates.x - pointCoordinates.x) + (mouseCoordinates.y - pointCoordinates.y) * (mouseCoordinates.y - pointCoordinates.y));
                    for (int i = 0; i < arrayOfGraphWindows[0].plottables.Length; i++)
                    {
                        pointCoordinates = arrayOfGraphWindows[0].plottables[i].GetPointNearestX(mouseCoordinates.x);
                        distance = Math.Sqrt((mouseCoordinates.x - pointCoordinates.x) * (mouseCoordinates.x - pointCoordinates.x) + (mouseCoordinates.y - pointCoordinates.y) * (mouseCoordinates.y - pointCoordinates.y));
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            nearestPlottableIndex = i;
                        }
                    }
                    pointCoordinates = arrayOfGraphWindows[0].plottables[nearestPlottableIndex].GetPointNearestX(mouseCoordinates.x);
                    myCrosshair.X = pointCoordinates.x; myCrosshair.Y = pointCoordinates.y;
                }
                else
                {
                    myCrosshair.X = mouseCoordinates.x; myCrosshair.Y = mouseCoordinates.y;
                }

                foreach (var item in arrayOfGraphWindows)
                    item.formsPlot.Refresh();
            }

        }
        // События клавиши для crosshair
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                myCrosshair.IsVisible = true;

                if (!isCntrlPressed)
                {
                    (double x, double y) = arrayOfGraphWindows[0].formsPlot.GetMouseCoordinates();
                    myCrosshair.X = x; myCrosshair.Y = y;
                    foreach (var item in arrayOfGraphWindows)
                        item.formsPlot.Refresh();
                }
                isCntrlPressed = true;
            }
        }
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                myCrosshair.IsVisible = false;
                foreach (var item in arrayOfGraphWindows)
                    item.formsPlot.Refresh();
                isCntrlPressed = false;
            }
        }
        // Событие FormsPlot.Plot Для связки графиков
        private void PlotMain_AxisChanged(object sender, EventArgs e)
        {
            if (graphFixed.Checked)
            {
                ScottPlot.AxisLimits limits = arrayOfGraphWindows[0].formsPlot.Plot.GetAxisLimits();
                double xMin = limits.XMin;
                double xMax = limits.XMax;
                for (byte i = 1; i < arrayOfGraphWindows.Length; i++)
                {
                    arrayOfGraphWindows[i].formsPlot.Plot.SetAxisLimitsX(xMin, xMax);
                    arrayOfGraphWindows[i].formsPlot.Refresh();
                }

            }

        }
        // Событие FormsPlot.Plot Для подсвечивание точки при дабл клике
        private void FormsPlot_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                ScottPlot.FormsPlot plot = (ScottPlot.FormsPlot)sender;
                int tag = Convert.ToInt32(plot.Tag);
                if (!noLimithighLightPointItem.Checked)
                    arrayOfGraphWindows[tag].ClearMarkers();
                var mouseCoordinates = plot.GetMouseCoordinates();
                var pointCoordinates = arrayOfGraphWindows[tag].plottables[0].GetPointNearestX(mouseCoordinates.x);
                double distance, minDistance;
                bool dontDraw = false;
                int nearestPlottableIndex = 0;
                minDistance = Math.Sqrt((mouseCoordinates.x - pointCoordinates.x) * (mouseCoordinates.x - pointCoordinates.x) + (mouseCoordinates.y - pointCoordinates.y) * (mouseCoordinates.y - pointCoordinates.y));
                for (int i = 0; i < arrayOfGraphWindows[tag].plottables.Length; i++)
                {
                    pointCoordinates = arrayOfGraphWindows[tag].plottables[i].GetPointNearestX(mouseCoordinates.x);
                    distance = Math.Sqrt((mouseCoordinates.x - pointCoordinates.x) * (mouseCoordinates.x - pointCoordinates.x) + (mouseCoordinates.y - pointCoordinates.y) * (mouseCoordinates.y - pointCoordinates.y));
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearestPlottableIndex = i;
                    }
                }
                pointCoordinates = arrayOfGraphWindows[tag].plottables[nearestPlottableIndex].GetPointNearestX(mouseCoordinates.x);
                for (int i = 0; i < arrayOfGraphWindows[tag].markers.Length; i++)
                {
                    if (arrayOfGraphWindows[tag].markers[i].X == pointCoordinates.x && arrayOfGraphWindows[tag].markers[i].Y == pointCoordinates.y)
                    {
                        arrayOfGraphWindows[tag].markers[i].IsVisible = false;
                        for (int j = 0; j < arrayOfGraphWindows[tag].markers.Length - 1; j++)
                            arrayOfGraphWindows[tag].markers[j] = arrayOfGraphWindows[tag].markers[j + 1];
                        Array.Resize(ref arrayOfGraphWindows[tag].markers, arrayOfGraphWindows[tag].markers.Length - 1);
                        dontDraw = true;
                        break;
                    }

                }
                if (!dontDraw)
                    arrayOfGraphWindows[tag].AddMarker(nearestPlottableIndex, (pointCoordinates.x, pointCoordinates.y), markerDates);
            }
            catch
            {

            }
        }


        //---------- События меню -------------\\

        // Мульти crosshair
        private void MultiCH_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem subMenu)
            {
                if (subMenu.Checked)
                {
                    isMultiCH = true;
                    for (int i = 1; i < arrayOfGraphWindows.Length; i++)
                    {
                        arrayOfGraphWindows[i].formsPlot.Plot.Add(myCrosshair);
                    }
                }
                else
                {
                    isMultiCH = false;
                    for (int i = 1; i < arrayOfGraphWindows.Length; i++)
                        arrayOfGraphWindows[i].formsPlot.Plot.Remove(myCrosshair);
                }
                foreach (var item in arrayOfGraphWindows)
                    item.formsPlot.Refresh();
            }

        }
        //Связка графиков
        private void GraphFixed_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem subMenu)
            {
                if (subMenu.Checked)
                    isGraphFixed = true;
                else
                    isGraphFixed = false;
            }
        }
        // Светлая тема
        public void LightTheme_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem subMenu)
            {
                if (subMenu.Checked)
                {
                    isLightTheme = true;
                    this.BackColor = Color.FromName("Window");
                    selectFile_button.BackColor = Color.FromName("ButtonFace");
                    selectFile_button.ForeColor = Color.Black;
                    add_graphWindow_button.BackColor = Color.FromName("ButtonFace");
                    add_graphWindow_button.ForeColor = Color.Black;
                    textBox1.BackColor = Color.FromName("Control");
                    textBox1.ForeColor = Color.Black;
                    myCrosshair.Color = Color.Black;
                    foreach (var item in arrayOfGraphWindows)
                        item.formsPlot.Plot.Style(Style.Default);
                    foreach (var item in labels)
                        item.ForeColor = Color.Black;
                }
                else
                {
                    isLightTheme = false;
                    this.BackColor = Color.FromArgb(27, 33, 56);
                    selectFile_button.BackColor = Color.FromArgb(27, 33, 56);
                    selectFile_button.ForeColor = Color.FromName("ScrollBar");
                    add_graphWindow_button.BackColor = Color.FromArgb(27, 33, 56);
                    add_graphWindow_button.ForeColor = Color.FromName("ScrollBar");
                    textBox1.BackColor = Color.FromArgb(27, 33, 56);
                    textBox1.ForeColor = Color.FromName("ScrollBar");
                    myCrosshair.Color = Color.FromArgb(190, 190, 190);
                    foreach (var item in arrayOfGraphWindows)
                        item.formsPlot.Plot.Style(Style.Blue2);
                    foreach (var item in labels)
                        item.ForeColor = Color.White;
                }
                foreach (var item in arrayOfGraphWindows)
                    item.formsPlot.Refresh();

            }
        }
        // Всегда строить график при выборе интервала 
        private void FastDrawVMM_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem subMenu)
            {
                if (subMenu.Checked)
                    isFastDraw = true;
                else
                    isFastDraw = false;
            }
        }
        // Настройка местоположения легенды (еще не сделано)
        private void LegendSettings_Click(object sender, EventArgs e)
        {

        }
        // Скрывать легенду
        private void HideLegend_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem subMenu)
            {
                if (subMenu.Checked)
                {
                    isLegenedHidden = true;
                    foreach (var item in arrayOfGraphWindows)
                    {
                        item.formsPlot.Plot.Legend(false);
                        item.formsPlot.Refresh();
                    }
                }
                else
                {
                    isLegenedHidden = false;
                    foreach (var item in arrayOfGraphWindows)
                    {
                        item.formsPlot.Plot.Legend(true);
                        item.formsPlot.Refresh();
                    }
                }

            }
        }
        // Изменить right click menu
        private void ChangeRightClickMenu_CheckedChanged(object sender, EventArgs e)
        {
            if (changeRightClickMenu.Checked)
            {
                isDefaultRightClickMenu = true;
                foreach (var item in arrayOfGraphWindows)
                {
                    item.formsPlot.RightClicked -= FormsPlot_RightClicked;
                    item.formsPlot.RightClicked += item.formsPlot.DefaultRightClickEvent;
                }
            }
            else
            {
                foreach (var item in arrayOfGraphWindows)
                {
                    item.formsPlot.RightClicked -= item.formsPlot.DefaultRightClickEvent;
                    item.formsPlot.RightClicked += FormsPlot_RightClicked;
                }
            }
        }

        private void FormsPlot_RightClicked(object sender, EventArgs e)
        {

        }

        // Вкл/выкл "фейковые точки"
        private void FakePointsSettings_CheckedChanged(object? sender, EventArgs e)
        {
            ToolStripMenuItem tsi = sender as ToolStripMenuItem;

            if (tsi.Checked)
                isFakePointsEnabled = true;
            else
                isFakePointsEnabled = false;

            foreach (var item in arrayOfGraphWindows)
            {
                for (byte i = 0; i < 43; i++)
                {
                    if (item.isOnThePlot[i])
                    {
                        item.RemoveChart(i, labelsText);
                        item.Draw(i, xs, yss, colors, labels[i].Text, isDate, arrayOfFakePointsIndex, specialColor, isFakePointsEnabled, isLegenedHidden);
                        item.formsPlot.Refresh();
                    }
                }
            }

        }
        // О разработчиках
        private void InfoForm_Click(object sender, EventArgs e)
        {
            DevelopersView f2 = new();
            f2.Show();
        }
        // Настройка цветов
        private void ColorSettings_Click(object sender, EventArgs e)
        {
            if (!isF3Opened)
            {
                f3 = new(isLightTheme, colors, specialColor);
                f3.FormClosing += (sender1, e1) =>
                {
                    if (panels != null)
                        for (byte i = 0; i < panels.Length; i++)
                            panels[i].BackColor = colors[i];
                    isF3Opened = f3.isF3Opened;
                };
                f3.Show();
                isF3Opened = true;
            }
            else
            {
                MessageBox.Show("Окно уже открыто!");
                f3.Focus();
            }

        }
        // Настройка уровней вибраций (для VMMLifeTimeData)
        private void SetVibrationLevel_Click(object sender, EventArgs e)
        {
            if (!isF4Opened)
            {
                f4 = new(isLightTheme, permissibleVibrationLevel, highVibrationLevel, criticalVibrationLevel);
                f4.FormClosing += (sender1, e1) =>
                {
                    permissibleVibrationLevel = (int)f4.numericUpDown1.Value;
                    highVibrationLevel = (int)f4.numericUpDown2.Value;
                    criticalVibrationLevel = (int)f4.numericUpDown3.Value;
                    if (isVMMDrawed)
                    {
                        double[] pv, hv, cv;
                        pv = new double[yss[6].Length];
                        hv = new double[yss[6].Length];
                        cv = new double[yss[6].Length];
                        for (int i = 0; i < pv.Length; i++)
                        {
                            pv[i] = permissibleVibrationLevel;
                            hv[i] = highVibrationLevel;
                            cv[i] = criticalVibrationLevel;
                        }
                        yss[6] = pv;
                        yss[7] = hv;
                        yss[8] = cv;
                        for (int i = 0; i < arrayOfGraphWindows.Length; i++)
                        {
                            arrayOfGraphWindows[i].RemoveChart(6, labelsText);
                            arrayOfGraphWindows[i].RemoveChart(7, labelsText);
                            arrayOfGraphWindows[i].RemoveChart(8, labelsText);
                            arrayOfGraphWindows[i].Draw(6, xs, yss, colors, "Уровень допустимых вибраций", isDate, arrayOfFakePointsIndex, specialColor, isFakePointsEnabled, isLegenedHidden);
                            arrayOfGraphWindows[i].Draw(7, xs, yss, colors, "Уровень высоких выибраций", isDate, arrayOfFakePointsIndex, specialColor, isFakePointsEnabled, isLegenedHidden);
                            arrayOfGraphWindows[i].Draw(8, xs, yss, colors, "Уровень критических выибраций", isDate, arrayOfFakePointsIndex, specialColor, isFakePointsEnabled, isLegenedHidden);
                        }
                    }
                    isF4Opened = false;
                };
                f4.Show();
                isF4Opened = true;
            }
            else
            {
                MessageBox.Show("Окно уже открыто!");
                f4.Focus();
            }
        }
        // Открыть 
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFile();
        }
        //Открыть последний файл
        private void recentFile_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
            byte index = (byte)menuItem.Tag;
            splitContainer1.Panel1.Focus(); // Убрали фокус           
            cantReadFile = false;
            comboBox1?.Dispose();
            isVMMDrawed = false;
            if (isFileOpened)
            {
                for (byte i = 0; i < labels.Length; i++)
                {
                    labels[i].Dispose();
                    panels[i].Dispose();
                }
                for (byte i = 0; i < arrayOfGraphWindows.Length; i++)
                {
                    arrayOfGraphWindows[i].formsPlot.Plot.Clear();
                    arrayOfGraphWindows[i].formsPlot.Refresh();
                    for (byte j = 0; j < arrayOfGraphWindows[i].isOnThePlot.Length; j++)
                        arrayOfGraphWindows[i].isOnThePlot[j] = false;
                }
                if (panels != null)
                {
                    Array.Clear(labels);
                    Array.Clear(panels);
                }
            }
            fileName = recentlyFilesArray[index];
            recentlyFilesArray.Insert(0, fileName);
            textBox1.Clear();
            fileTypeCode = FileType(fileName);
            if (fileTypeCode == 1 || fileTypeCode == 5)
                isDate = false;
            else
                isDate = true;
            if (fileTypeCode != 12)
                FileInformstionData();
            FindRowsCount();
            if (isFileBroken)
                fileTypeCode = 200;
            switch (fileTypeCode)
            {
                case 0:
                    BatteryStatus();
                    isFileOpened = true;
                    break;
                case 1:  //BoreAnnulus();
                    BoreAnnulus();
                    isFileOpened = true;
                    break;
                case 2:
                    Directional();
                    isFileOpened = true;
                    break;
                case 3:
                    GammaNode();
                    isFileOpened = true;
                    break;
                case 4:
                    MasterControllerDebug();
                    isFileOpened = true;
                    break;
                case 5: // MPT Telemetred Data
                    MptTelemeteredData();
                    isFileOpened = true;
                    break;
                case 6:
                    NodeStatus();
                    isFileOpened = true;
                    break;
                case 7:
                    Pressure();
                    isFileOpened = true;
                    break;
                case 8: // Static Survey (Directioanal)
                    Directional();
                    isFileOpened = true;
                    break;
                case 9:
                    SystemStatus();
                    isFileOpened = true;
                    break;
                case 10: // VMM Agregate Drilling Performance
                    VMMAgregateDrillingPerfomance();
                    isFileOpened = true;
                    break;
                case 11: // VMM Diagnostics
                    VMMDiagnostics();
                    isFileOpened = false;
                    break;
                case 12: // VMMM LifeTime Data
                    VMMLifeTimeData();
                    isFileOpened = true;
                    break;
                case 200:
                    MessageBox.Show("Данный файл поврежден. Проверьте файл вручную");
                    cantReadFile = true;
                    isFileOpened = false;
                    break;
                default:
                    MessageBox.Show("Файл не может быть прочитан");
                    cantReadFile = true;
                    isFileOpened = false;
                    break;
            }
            arrayOfGraphWindows[0].formsPlot.Plot.Add(myCrosshair);

            if (isLightTheme && !cantReadFile)
            {
                foreach (var item in labels)
                    item.ForeColor = Color.Black;
            }
            else if (!cantReadFile)
            {
                foreach (var item in labels)
                    if (item != null)
                        item.ForeColor = Color.White;
            }
        }
        // Отображения справки
        private void fAQToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, "help.chm");
        }
        // Вкл/выкл панель инструментов
        private void ToolPanelToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (ToolPanelToolStripMenuItem.Checked)
                toolStrip1.Visible = true;
            else
                toolStrip1.Visible = false;
        }

        //Изменить цвет графика 
        private void ChangeColorMenuItem_Click(object sender, EventArgs e)
        {
            Panel panel = (Panel)((ContextMenuStrip)((ToolStripMenuItem)sender).Owner).SourceControl;
            ColorDialog colorDialog = new ColorDialog();
            Color panelColor = new Color();
            colorDialog.Color = panel.BackColor;
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                panelColor = colorDialog.Color;
                panel.BackColor = panelColor;
                colors[Convert.ToInt32(panel.Tag)] = panelColor;
                foreach (var item in arrayOfGraphWindows)
                    item.changePlottablesColor(colors, labelsText);
            }
        }
        // Событие контекстного меню "очистить все"
        private void ClearAllItem_Click(object sender, EventArgs e)
        {
            foreach (var item in arrayOfGraphWindows)
                item.ClearAll(myCrosshair);
        }
        // Событие контекстного меню "очистить все точки"
        private void ClearPointsItem_Click(object sender, EventArgs e)
        {
            foreach (var item in arrayOfGraphWindows)
                item.ClearMarkers();
        }
        // Событие контекстного меню "очистить все линии"
        private void ClearAllLinesItem_Click(object sender, EventArgs e)
        {
            foreach (var item in arrayOfGraphWindows)
                item.RemoveLines();
        }
        // Создать новый график
        private void createToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!isF5Opened)
            {
                f5 = new CreatingGraphsView(isLightTheme, colors);
                f5.Show();
                isF5Opened = true;
            }
            else
            {
                MessageBox.Show("Окно уже открыто!");
                f5.Focus();
            }
        }

        // Методы кнопок панели инструментов
        private void cursorButton_Click(object sender, EventArgs e)
        {
            cursorButton.Checked = true;
            lineButton.Checked = false;
            eraserButton.Checked = false;
            verticalLineButton.Checked = false;
            slopeLineButton.Checked = false;
            isSecondClick = true;
            backgroundLabel.Visible = false;
            isSecondClick = false;
            toolIndex = 0;           
        }

        private void lineButton_Click(object sender, EventArgs e)
        {
            lineButton.Checked = true;
            slopeLineButton.Checked = false;
            cursorButton.Checked = false;
            eraserButton.Checked = false;
            verticalLineButton.Checked = false;
            backgroundLabel.Visible = false;
            isSecondClick = false;
            toolIndex = 1;
        }
        private void verticalLineButton_Click(object sender, EventArgs e)
        {
            verticalLineButton.Checked = true;
            slopeLineButton.Checked = false;
            cursorButton.Checked = false;
            eraserButton.Checked = false;
            lineButton.Checked = false;
            backgroundLabel.Visible = false;
            isSecondClick = false;
            toolIndex = 2;
        }
        private void slopeLineButton_Click(object sender, EventArgs e)
        {
            slopeLineButton.Checked = true;
            eraserButton.Checked = false;
            cursorButton.Checked = false;
            verticalLineButton.Checked = false;
            lineButton.Checked = false;
            toolIndex = 3;

            isSecondClick = false;
            backgroundLabel.Text = "Отметье первую точку";
            backgroundLabel.Visible = true;
            backgroundLabel.BringToFront();
        }
        private void eraserButton_Click(object sender, EventArgs e)
        {
            slopeLineButton.Checked = false;
            eraserButton.Checked = true;
            cursorButton.Checked = false;
            verticalLineButton.Checked = false;
            lineButton.Checked = false;
            backgroundLabel.Visible = false;
            isSecondClick = false;
            toolIndex = 4;
        }

        private void FormsPlot_MouseDown(object sender, MouseEventArgs e)
        {
            ScottPlot.FormsPlot plot = (ScottPlot.FormsPlot)sender;
            int tag = Convert.ToInt32(plot.Tag);
            
            void line(int tag, byte typeOfLine)
            {
                var mouseCoordinates = plot.GetMouseCoordinates();
                if (typeOfLine == 0)
                    arrayOfGraphWindows[tag].Draw(mouseCoordinates.y, lineWidth, lineColor, 0);
                else if (typeOfLine == 1)
                    arrayOfGraphWindows[tag].Draw(mouseCoordinates.x, lineWidth, lineColor, 1);
            }

            void slopeLine(int tag)
            {                             
                if (!isSecondClick)
                {
                    mouseCoordinates1 = plot.GetMouseCoordinates();
                    arrayOfGraphWindows[tag].AddMarker(mouseCoordinates1.x, mouseCoordinates1.y);
                    backgroundLabel.Text = "Отметье вторую точку";
                    isSecondClick = true;
                }
                else
                {
                    mouseCoordinates2 = plot.GetMouseCoordinates();                 
                    arrayOfGraphWindows[tag].formsPlot.Plot.AddLine(mouseCoordinates1.x, mouseCoordinates1.y, mouseCoordinates2.x, mouseCoordinates2.y, lineColor, lineWidth);
                    backgroundLabel.Visible = false;
                }
               
            }

            void eraser(int tag)
            {
                HLine[] hLines = Array.Empty<HLine>();
                VLine[] vLines = Array.Empty<VLine>();
                var mouseCoordinates = plot.GetMouseCoordinates();
                double mouseX = mouseCoordinates.x;
                double mouseY = mouseCoordinates.y;
                foreach (var item in arrayOfGraphWindows[tag].formsPlot.Plot.GetPlottables())
                {
                    if (item is ScottPlot.Plottable.HLine hLine)
                    {
                        if (Math.Abs(hLine.Y - mouseY) <= eraserWidth)
                        {
                            arrayOfGraphWindows[tag].formsPlot.Plot.Remove(hLine);
                            arrayOfGraphWindows[tag].formsPlot.Refresh();
                            break;
                        }

                    }
                    else if (item is ScottPlot.Plottable.VLine vLine)
                    {
                        if (Math.Abs(vLine.X - mouseX) <= eraserWidth)
                        {
                            arrayOfGraphWindows[tag].formsPlot.Plot.Remove(vLine);
                            arrayOfGraphWindows[tag].formsPlot.Refresh();
                            break;
                        }
                    }
                    else if (item is ScottPlot.Plottable.ScatterPlot line)
                    {
                        var point = line.GetPointNearest(mouseX, mouseY, xyRatio: 1);
                        if (Math.Sqrt((mouseX - point.x) * (mouseX - point.x) + (mouseY - point.y) * (mouseY - point.y)) <= eraserWidth)
                        {
                            arrayOfGraphWindows[tag].formsPlot.Plot.Remove(line);
                            arrayOfGraphWindows[tag].formsPlot.Refresh();
                            break;
                        }
                    }
                }
            }

            switch (toolIndex)
            {
                case 0:
                    break;
                case 1:
                    line(tag, 0);
                    break;
                case 2:
                    line(tag, 1);
                    break;
                case 3:
                    slopeLine(tag);
                    break;
                case 4:
                    eraser(tag);
                    break;
            }
        }

        // События всплывающего меню элементов панели инструментов
        private void LineColorMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.Color = lineColor;
            if (colorDialog.ShowDialog() == DialogResult.OK)
                lineColor = colorDialog.Color;
        }
        private void LineWidthMenuItem_Click(object sender, EventArgs e)
        {
            if (!isLineWidthSettingViewOpen)
            {
                lineWidthSettingView = new lineWidthSettingView(lineWidth, isLightTheme);
                lineWidthSettingView.FormClosing += (sender1, e1) =>
                {
                    this.lineWidth = (float)lineWidthSettingView.lineWidth;
                    isLineWidthSettingViewOpen = false;
                };
                lineWidthSettingView.Show();
                isLineWidthSettingViewOpen = true;
            }
            else
            {
                MessageBox.Show("Окно уже открыто!");
                lineWidthSettingView.Focus();
            }
        }

        private void LineIsMagnetMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (lineIsMagnetMenuItem.Checked)
                isLineMagnet = true;
            else
                isLineMagnet = false;
        }
        private void EraserWidthItem_Click(object sender, EventArgs e)
        {
            if (!isEraserWidthSettinViewOpen)
            {
                eraserWidthSettingView = new eraserWidthSettingView(eraserWidth);
                eraserWidthSettingView.FormClosing += (sender1, e1) =>
                {
                    this.eraserWidth = (double)eraserWidthSettingView.numericUpDown1.Value / 10.0;
                    isEraserWidthSettinViewOpen = false;
                };
                eraserWidthSettingView.Show();
                isEraserWidthSettinViewOpen = true;
            }
            else
            {
                MessageBox.Show("Окно уже открыто!");
                eraserWidthSettingView.Focus();
            }
        }

        // Сохранение пользовательских настроек при закрытии программы
        protected override void OnClosing(CancelEventArgs e)
        {
            if (recentlyFilesArray[0].Length != 0)
                Properties.Settings1.Default.file1 = this.recentlyFilesArray[0];
            if (recentlyFilesArray[1].Length != 1)
                Properties.Settings1.Default.file2 = this.recentlyFilesArray[1];
            if (recentlyFilesArray[2].Length != 2)
                Properties.Settings1.Default.file3 = this.recentlyFilesArray[2];
            if (recentlyFilesArray[3].Length != 3)
                Properties.Settings1.Default.file4 = this.recentlyFilesArray[3];
            if (recentlyFilesArray[4].Length != 4)
                Properties.Settings1.Default.file5 = this.recentlyFilesArray[4];
            Properties.Settings1.Default.eraserWidth = this.eraserWidth;
            Properties.Settings1.Default.isLineMagnet = this.isLineMagnet;
            Properties.Settings1.Default.lineColor = this.lineColor;
            Properties.Settings1.Default.lineWidth = this.lineWidth;
            Properties.Settings1.Default.isDefaultRightClickMenu = changeRightClickMenu.Checked;
            Properties.Settings1.Default.isToolPanelVisible = ToolPanelToolStripMenuItem.Checked;
            Properties.Settings1.Default.isManyPoints = noLimithighLightPointItem.Checked;
            Properties.Settings1.Default.isMagnetCH = magnetCH.Checked;
            Properties.Settings1.Default.isLegendHidden = this.isLegenedHidden;
            Properties.Settings1.Default.isFastDraw = this.isFastDraw;
            Properties.Settings1.Default.specialColor = specialColor;
            Properties.Settings1.Default.isLightTheme = isLightTheme;
            Properties.Settings1.Default.color1 = colors[0];
            Properties.Settings1.Default.isMultiCH = isMultiCH;
            Properties.Settings1.Default.isGraphFixed = isGraphFixed;
            Properties.Settings1.Default.permissibleVibrationLevel = permissibleVibrationLevel;
            Properties.Settings1.Default.highVibrationLevel = highVibrationLevel;
            Properties.Settings1.Default.criticalVibrationLevel = criticalVibrationLevel;
            Properties.Settings1.Default.Save();
        }

        // Загрузка пользовательских настроек при запуске программы
        private void LoadSettings()
        {
            if (recentlyFilesArray[0] != null)
                this.recentlyFilesArray[0] = Properties.Settings1.Default.file1;
            if (recentlyFilesArray[1] != null)
                this.recentlyFilesArray[1] = Properties.Settings1.Default.file2;
            if (recentlyFilesArray[2] != null)
                this.recentlyFilesArray[2] = Properties.Settings1.Default.file3;
            if (recentlyFilesArray[3] != null)
                this.recentlyFilesArray[3] = Properties.Settings1.Default.file4;
            if (recentlyFilesArray[4] != null)
                this.recentlyFilesArray[4] = Properties.Settings1.Default.file5;
            this.eraserWidth = Properties.Settings1.Default.eraserWidth;
            this.isLineMagnet = Properties.Settings1.Default.isLineMagnet;
            this.lineColor = Properties.Settings1.Default.lineColor;
            this.lineWidth = Properties.Settings1.Default.lineWidth;
            this.isDefaultRightClickMenu = Properties.Settings1.Default.isDefaultRightClickMenu;
            this.isManyPoints = Properties.Settings1.Default.isManyPoints;
            this.isToolPanelVisible = Properties.Settings1.Default.isToolPanelVisible;
            this.isMagnet = Properties.Settings1.Default.isMagnetCH;
            this.isLegenedHidden = Properties.Settings1.Default.isLegendHidden;
            this.isFastDraw = Properties.Settings1.Default.isFastDraw;
            this.permissibleVibrationLevel = Properties.Settings1.Default.permissibleVibrationLevel;
            this.highVibrationLevel = Properties.Settings1.Default.highVibrationLevel;
            this.criticalVibrationLevel = Properties.Settings1.Default.criticalVibrationLevel;
            this.specialColor = Properties.Settings1.Default.specialColor;
            this.isLightTheme = Properties.Settings1.Default.isLightTheme;
            this.isMultiCH = Properties.Settings1.Default.isMultiCH;
            this.isGraphFixed = Properties.Settings1.Default.isGraphFixed;
            //Инициализация цветов
            this.colors[0] = Properties.Settings1.Default.color1;
            this.colors[1] = Properties.Settings1.Default.color2;
            this.colors[2] = Properties.Settings1.Default.color3;
            this.colors[3] = Properties.Settings1.Default.color4;
            this.colors[4] = Properties.Settings1.Default.color5;
            this.colors[5] = Properties.Settings1.Default.color6;
            this.colors[6] = Properties.Settings1.Default.color7;
            this.colors[7] = Properties.Settings1.Default.color8;
            this.colors[8] = Properties.Settings1.Default.color9;
            this.colors[9] = Properties.Settings1.Default.color10;
            this.colors[10] = Properties.Settings1.Default.color11;
            this.colors[11] = Properties.Settings1.Default.color12;
            this.colors[12] = Properties.Settings1.Default.color13;
            this.colors[13] = Properties.Settings1.Default.color14;
            this.colors[14] = Properties.Settings1.Default.color15;
            this.colors[15] = Properties.Settings1.Default.color16;
            this.colors[16] = Properties.Settings1.Default.color17;
            this.colors[17] = Properties.Settings1.Default.color18;
            this.colors[18] = Properties.Settings1.Default.color19;
            this.colors[19] = Properties.Settings1.Default.color20;
            this.colors[20] = Properties.Settings1.Default.color21;
            this.colors[21] = Properties.Settings1.Default.color22;
            this.colors[22] = Properties.Settings1.Default.color23;
            this.colors[23] = Properties.Settings1.Default.color24;
            this.colors[24] = Properties.Settings1.Default.color25;
            this.colors[25] = Properties.Settings1.Default.color26;
            this.colors[26] = Properties.Settings1.Default.color27;
            this.colors[27] = Properties.Settings1.Default.color28;
            this.colors[28] = Properties.Settings1.Default.color29;
            this.colors[29] = Properties.Settings1.Default.color30;
            this.colors[30] = Properties.Settings1.Default.color31;
            this.colors[31] = Properties.Settings1.Default.color32;
            this.colors[32] = Properties.Settings1.Default.color33;
            this.colors[33] = Properties.Settings1.Default.color34;
            this.colors[34] = Properties.Settings1.Default.color35;
            this.colors[35] = Properties.Settings1.Default.color36;
            this.colors[36] = Properties.Settings1.Default.color37;
            this.colors[37] = Properties.Settings1.Default.color38;
            this.colors[38] = Properties.Settings1.Default.color39;
            this.colors[39] = Properties.Settings1.Default.color40;
            this.colors[40] = Properties.Settings1.Default.color41;
            this.colors[41] = Properties.Settings1.Default.color42;
            this.colors[42] = Properties.Settings1.Default.color43;
            this.colors[43] = Properties.Settings1.Default.color44;

        }

        private void splitContainer1_Panel2_Click(object sender, EventArgs e)
        {
            toolIndex = 0;
            cursorButton.Checked = false;
            lineButton.Checked = false;
            eraserButton.Checked = false;
            verticalLineButton.Checked = false;
            slopeLineButton.Checked = false;
            backgroundLabel.Visible = false;
        }

    }
}