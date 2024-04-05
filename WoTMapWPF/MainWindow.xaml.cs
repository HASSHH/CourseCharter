using OpenTK.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using WoTMapWPF.CustomControls;
using WoTMapWPF.Graphics;
using Timer = System.Timers.Timer;
using Window = System.Windows.Window;

namespace WoTMapWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Scene scene;
        private double[] oldMousePos;
        private Stack<Path> backwardStack = new Stack<Path>();
        private Stack<Path> forwardStack = new Stack<Path>();
        private Dictionary<string, PanelButtonTuple> panels = new Dictionary<string, PanelButtonTuple>();
        private string saveLocation;
        private JsonSerializerOptions jsonSerializerOptions;

        public MainWindow()
        {
            InitializeComponent();
            InitializeGlComponent();
            InitializeTimer();
            panels.Add("Map", new PanelButtonTuple { Panel = MapControl, Button = ShowMapButton });
            panels.Add("NewMap", new PanelButtonTuple { Panel = NewMapControl, Button = ShowNewMapButton });
            panels.Add("LoadMap", new PanelButtonTuple { Panel = LoadMapControl, Button = ShowLoadMapButton });
            panels.Add("SavePath", new PanelButtonTuple { Panel = SavePathControl, Button = ShowSavePathButton });
            panels.Add("LoadPath", new PanelButtonTuple { Panel = LoadPathControl, Button = ShowLoadPathButton });
            panels.Add("Guide", new PanelButtonTuple { Panel = GuideControl, Button = ShowGuideButton });
            panels.Add("Settings", new PanelButtonTuple { Panel = SettingsControl, Button = ShowSettingsButton });
            Map.New();
            ViewModel = (MainWindowViewModel)DataContext;
            scene = new Scene(GLControl, ViewModel.Path);
            ResetPathSubscriptions(null, ViewModel.Path);
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            GLControl.Focusable = true;
            jsonSerializerOptions = new JsonSerializerOptions();
            jsonSerializerOptions.NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals;
            jsonSerializerOptions.WriteIndented = true;
            saveLocation = (string)App.Current.Resources["SaveLocation"];
        }

        public MainWindowViewModel ViewModel { get; private set; }

        private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Path":
                    PathNodesInfoListView.ItemsSource = ViewModel.Path.Nodes;
                    ChangeListViewSelectedNode(ViewModel.Path.SelectedIndex);
                    scene?.SetPath(ViewModel.Path);
                    ResetPathSubscriptions(ViewModel.OldPath, ViewModel.Path);
                    break;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            App app = (App)App.Current;
            //save window position/size
            if (WindowState == WindowState.Maximized)
            {
                // Use the RestoreBounds as the current values will be 0, 0 and the size of the screen
                app.ChangeUserSetting("WindowTop", RestoreBounds.Top);
                app.ChangeUserSetting("WindowLeft", RestoreBounds.Left);
                app.ChangeUserSetting("WindowHeight", RestoreBounds.Height);
                app.ChangeUserSetting("WindowWidth", RestoreBounds.Width);
                app.ChangeUserSetting("IsWindowMaximized", true);
            }
            else
            {
                app.ChangeUserSetting("WindowTop", this.Top);
                app.ChangeUserSetting("WindowLeft", this.Left);
                app.ChangeUserSetting("WindowHeight", this.Height);
                app.ChangeUserSetting("WindowWidth", this.Width);
                app.ChangeUserSetting("IsWindowMaximized", false);
            }
            //save currently opened map
            app.ChangeUserSetting("LastOpenedMapName", ViewModel.MapName);
            app.ChangeUserSetting("LastOpenedMapImageMD5", ViewModel.MapImageMD5);
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            //window position/size
            if (App.Current.Resources.Contains("WindowTop"))
            {
                this.Top = (double)App.Current.Resources["WindowTop"];
            }
            if (App.Current.Resources.Contains("WindowLeft"))
            {
                this.Left = (double)App.Current.Resources["WindowLeft"];
            }
            if (App.Current.Resources.Contains("WindowHeight"))
            {
                this.Height = (double)App.Current.Resources["WindowHeight"];
            }
            if (App.Current.Resources.Contains("WindowWidth"))
            {
                this.Width = (double)App.Current.Resources["WindowWidth"];
            }
            if (App.Current.Resources.Contains("IsWindowMaximized") && (bool)App.Current.Resources["IsWindowMaximized"])
                WindowState = WindowState.Maximized;
            //load last opened map
            if (App.Current.Resources.Contains("LastOpenedMapName") && App.Current.Resources.Contains("LastOpenedMapImageMD5"))
            {
                string mapName = (string)App.Current.Resources["LastOpenedMapName"];
                string mapImageMD5 = (string)App.Current.Resources["LastOpenedMapImageMD5"];
                string mapPath = $"{saveLocation}\\maps\\{mapImageMD5}\\{mapName}.info";
                if (File.Exists(mapPath))
                    try
                    {
                        string jsonString = File.ReadAllText(mapPath);
                        MapFileDefinition map = JsonSerializer.Deserialize<MapFileDefinition>(jsonString, jsonSerializerOptions);
                        if (map != null)
                            if (LoadMap(map))
                            {
                                //only reason to call this method here is for updating title with correct map name
                                ShowDefaultPanel();
                            }
                    }
                    catch { }
            }
            //load user selected theme
            if (App.Current.Resources.Contains("ThemeName"))
            {
                string themeName = (string)App.Current.Resources["ThemeName"];
                ((App)App.Current).ChangeTheme(themeName);
            }
        }

        private bool LoadMap(MapFileDefinition map)
        {
            try
            {
                Map.New($"{saveLocation}\\maps\\{map.ImageMD5}\\map_image{map.ImageExt}");
                scene?.ResetCamera();
                ViewModel.DistanceUnit = map.UnitLabel;
                ViewModel.DistanceUnitsPerPixel = (double)map.SampleUnits / map.SamplePixels;
                ViewModel.MapImageMD5 = map.ImageMD5;
                ViewModel.MapName = map.Name;
                ApplyPath(new Path());
                //if autosave enabled try load autosaved path
                try
                {
                    if (App.Current.Resources.Contains("IsPathAutosaveEnabled"))
                    {
                        bool isEnabled = (bool)App.Current.Resources["IsPathAutosaveEnabled"];
                        if (isEnabled)
                        {
                            string fileName = $"{saveLocation}\\maps\\{map.ImageMD5}\\autosave\\__autosave_path__.info";
                            if (File.Exists(fileName))
                            {
                                string jsonString = File.ReadAllText(fileName);
                                PathFileDefinition pathDef = JsonSerializer.Deserialize<PathFileDefinition>(jsonString, jsonSerializerOptions);
                                if (pathDef != null)
                                {
                                    ApplyPath(pathDef.Path);
                                    StorePathState();
                                }
                            }
                        }
                    }
                }
                catch { }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void ShowPanel(string panelName, string windowTitlePart)
        {
            if (panels.ContainsKey(panelName))
            {
                FrameworkElement visibleElement = panels[panelName].Panel;
                Button disabledButton = panels[panelName].Button;
                foreach (PanelButtonTuple p in panels.Values)
                {
                    p.Panel.Visibility = Visibility.Hidden;
                    p.Button.IsEnabled = true;
                }
                visibleElement.Visibility = Visibility.Visible;
                disabledButton.IsEnabled = false;
                Title = ViewModel.WindowTitleBase + windowTitlePart;
            }
        }

        private void ShowDefaultPanel()
        {
            string windowTitle = string.Empty;
            if (!string.IsNullOrWhiteSpace(ViewModel.MapName))
                windowTitle = $" - {ViewModel.MapName}";
            ShowPanel("Map", windowTitle);
        }

        #region PATH
        private void StorePathState()
        {
            //add state to undo-stack, clear the redo-stack
            backwardStack.Push((Path)ViewModel.Path.Clone());
            forwardStack.Clear();
            //autosave
            AutosavePath();
        }

        private void UndoPathState()
        {
            if (backwardStack.Count > 1)
            {
                forwardStack.Push(backwardStack.Pop());
                Path path = backwardStack.Peek();
                ViewModel.Path = (Path)path.Clone();
                //autosave
                AutosavePath();
            }
        }

        private void RedoPathState()
        {
            if (forwardStack.Count > 0)
            {
                Path path = forwardStack.Pop();
                backwardStack.Push(path);
                ViewModel.Path = (Path)path.Clone();
                //autosave
                AutosavePath();
            }
        }

        private void ApplyPath(Path newPath)
        {
            ViewModel.Path = newPath;
            backwardStack.Clear();
            forwardStack.Clear();
        }

        private void ResetPathSubscriptions(Path oldPath, Path newPath)
        {
            if (oldPath != null)
            {
                oldPath.PropertyChanged -= Path_PropertyChanged;
                oldPath.PathChanged -= Path_PathChanged;
            }
            newPath.PropertyChanged += Path_PropertyChanged;
            newPath.PathChanged += Path_PathChanged;
        }

        private void Path_PathChanged(object? sender, EventArgs e)
        {
            StorePathState();
        }

        private void Path_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SelectedIndex":
                    ChangeListViewSelectedNode(ViewModel.Path.SelectedIndex);
                    break;
            }
        }

        private void ChangeListViewSelectedNode(int nodeIndex)
        {
            if (nodeIndex >= 0 && nodeIndex < PathNodesInfoListView.Items.Count)
            {
                PathNode selectedNode = (PathNode)PathNodesInfoListView.Items[nodeIndex];
                if (selectedNode != PathNodesInfoListView.SelectedItem)
                    PathNodesInfoListView.SelectedItem = selectedNode;
            }
            else
                PathNodesInfoListView.UnselectAll();
        }

        private void AutosavePath()
        {
            //save path to file if autosave is enabled
            if (App.Current.Resources.Contains("IsPathAutosaveEnabled"))
            {
                bool isEnabled = (bool)App.Current.Resources["IsPathAutosaveEnabled"];
                if (isEnabled)
                {
                    string fileName = "__autosave_path__.info";
                    PathFileDefinition pathFileDefinition = new PathFileDefinition();
                    pathFileDefinition.Name = "_autosave_";
                    pathFileDefinition.ImageMD5 = ViewModel.MapImageMD5;
                    pathFileDefinition.Path = ViewModel.Path;
                    string jsonString = JsonSerializer.Serialize(pathFileDefinition, jsonSerializerOptions);
                    Directory.CreateDirectory($"{saveLocation}\\maps\\{pathFileDefinition.ImageMD5}\\autosave");
                    File.WriteAllText($"{saveLocation}\\maps\\{pathFileDefinition.ImageMD5}\\autosave\\{fileName}", jsonString);
                }
            }
        }
        #endregion

        #region GL_EVENTS
        private void GLControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point pos = e.GetPosition(GLControl);
            if (Keyboard.IsKeyDown(Key.A))
            {
                scene?.AddMarker(pos.X, pos.Y);
                return;
            }
            scene?.HandleClick(pos.X, pos.Y);
        }

        private void GLControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            GLControl.Focus();
        }

        private void GLControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (GLControl.IsMouseOver && GLControl.IsFocused)
            {
                switch (e.Key)
                {
                    case Key.Escape:
                        e.Handled = true;
                        //clear selection
                        ViewModel.Path.SelectedIndex = -1;
                        break;
                    case Key.D:
                        {
                            e.Handled = true;
                            //delete selected marker
                            Path path = ViewModel.Path;
                            int index = path.SelectedIndex;
                            if (index >= 0 && index < path.Nodes.Count)
                                path.Nodes.RemoveAt(index);
                            break;
                        }
                    case Key.Delete:
                        e.Handled = true;
                        if (ViewModel.Path.Nodes.Count > 0)
                        {
                            string message = "Are you sure you want to clear the current path?";
                            ConfirmActionWindow caw = new ConfirmActionWindow(message);
                            if (caw.ShowDialog().GetValueOrDefault())
                                ApplyPath(new Path());
                        }
                        break;
                    case Key.Z:
                        if (Keyboard.Modifiers == ModifierKeys.Control)
                        {
                            e.Handled = true;
                            UndoPathState();
                        }
                        break;
                    case Key.Y:
                        if (Keyboard.Modifiers == ModifierKeys.Control)
                        {
                            e.Handled = true;
                            RedoPathState();
                        }
                        break;
                }
            }
        }

        private void GLControl_MouseMove(object sender, MouseEventArgs e)
        {
            Point pos = e.GetPosition(GLControl);
            double[] newMousePos = new double[2] { pos.X, pos.Y };
            if (oldMousePos != null)
            {
                double mouseMovementX = newMousePos[0] - oldMousePos[0];
                double mouseMovementY = newMousePos[1] - oldMousePos[1];
                if (e.RightButton == MouseButtonState.Pressed)
                    scene?.MoveCamera(mouseMovementX, -mouseMovementY);
                else if (e.LeftButton == MouseButtonState.Pressed)
                    scene?.MoveMarker(pos.X, pos.Y, mouseMovementX, -mouseMovementY);
            }
            oldMousePos = newMousePos;
        }

        private void GLControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Point pos = e.GetPosition(GLControl);
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (e.Delta > 0)
                    scene?.Zoom(true, pos.X, pos.Y);
                else
                    scene?.Zoom(false, pos.X, pos.Y);
            }
        }

        private void GLControl_MouseLeave(object sender, MouseEventArgs e)
        {
            scene?.HandleMouseLeave();
        }

        private void InitializeTimer()
        {
            Timer timer = new Timer(1000d / 60d);
            timer.Elapsed += Timer_Elapsed; ;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            try
            {
                Dispatcher.Invoke(() => GLControl.InvalidateVisual());
            }
            catch { }
        }

        private void InitializeGlComponent()
        {
            GLWpfControlSettings settings = new GLWpfControlSettings();
            settings.MajorVersion = 2;
            settings.MinorVersion = 1;
            GLControl.Start(settings);
        }

        private void GLControl_Render(TimeSpan obj)
        {
            scene?.Paint();
        }

        private void GLControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            scene?.Resize();
        }
        #endregion

        #region CONTROL_EVENTS
        private void ShowNodesButton_Click(object sender, RoutedEventArgs e)
        {
            if (NodesInfoGrid.Visibility == Visibility.Visible)
            {
                NodesInfoGrid.Visibility = Visibility.Hidden;
                ShowNodesButtonImage.Visibility = Visibility.Visible;
                HideNodesButtonImage.Visibility = Visibility.Hidden;
            }
            else
            {
                NodesInfoGrid.Visibility = Visibility.Visible;
                ShowNodesButtonImage.Visibility = Visibility.Hidden;
                HideNodesButtonImage.Visibility = Visibility.Visible;
            }
        }

        private void ShowMapButton_Click(object sender, RoutedEventArgs e)
        {
            string windowTitle = string.Empty;
            if (!string.IsNullOrWhiteSpace(ViewModel.MapName))
                windowTitle = $" - {ViewModel.MapName}";
            ShowPanel("Map", windowTitle);
        }

        private void ShowNewMapButton_Click(object sender, RoutedEventArgs e)
        {
            NewMapControl.ResetControl();
            ShowPanel("NewMap", " - New Map");
        }

        private void ShowLoadMapButton_Click(object sender, RoutedEventArgs e)
        {
            List<MapFileDefinition> maps = new List<MapFileDefinition>();
            if (Directory.Exists($"{saveLocation}\\maps"))
                foreach (string subdir in Directory.GetDirectories($"{saveLocation}\\maps"))
                    foreach (string mapInfoFile in Directory.GetFiles(subdir, "*.info"))
                        try
                        {
                            string jsonString = File.ReadAllText(mapInfoFile);
                            MapFileDefinition map = JsonSerializer.Deserialize<MapFileDefinition>(jsonString, jsonSerializerOptions);
                            if (map != null)
                                maps.Add(map);
                        }
                        catch { }
            if (maps.Count > 0)
            {
                LoadMapControl.ResetControl(maps);
                ShowPanel("LoadMap", " - Load Map");
            }
            else
                NotificationControl.ShowNotificationAndHide("No saved maps found.");
        }

        private void ShowSavePathButton_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.Path.Nodes.Count < 1)
            {
                NotificationControl.ShowNotificationAndHide($"There is no path to save.");
                return;
            }
            SavePathControl.ResetControl();
            ShowPanel("SavePath", " - Save Path");
        }

        private void ShowLoadPathButton_Click(object sender, RoutedEventArgs e)
        {
            List<PathFileDefinition> paths = new List<PathFileDefinition>();
            string dir = $"{saveLocation}\\maps\\{ViewModel.MapImageMD5}\\paths";
            if (Directory.Exists(dir))
                foreach (string pathInfoFile in Directory.GetFiles(dir, "*.info"))
                    try
                    {
                        string jsonString = File.ReadAllText(pathInfoFile);
                        PathFileDefinition path = JsonSerializer.Deserialize<PathFileDefinition>(jsonString, jsonSerializerOptions);
                        if (path != null)
                            paths.Add(path);
                    }
                    catch { }
            if (paths.Count > 0)
            {
                LoadPathControl.ResetControl(paths);
                ShowPanel("LoadPath", " - Load Path");
            }
            else
                NotificationControl.ShowNotificationAndHide("No saved paths found for current map.");
        }

        private void ShowGuideButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPanel("Guide", " - Guide");
        }

        private void ShowSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPanel("Settings", " - Settings");
        }

        private void NewMapControl_SaveButtonClicked(object sender, EventArgs e)
        {
            NewMapControl nmc = (NewMapControl)sender;
            if (!string.IsNullOrWhiteSpace(nmc.ViewModel.Name) &&
                !string.IsNullOrWhiteSpace(nmc.ViewModel.ImageFileName) &&
                !string.IsNullOrWhiteSpace(nmc.ViewModel.ImageFilePath) &&
                !string.IsNullOrWhiteSpace(nmc.ViewModel.UnitLabel) &&
                !string.IsNullOrWhiteSpace(nmc.ViewModel.ImageMD5))
            {
                if (File.Exists($"{saveLocation}\\maps\\{nmc.ViewModel.ImageMD5}\\{nmc.ViewModel.Name}.info"))
                {
                    NotificationControl.ShowError($"A file with the name \"{saveLocation}\\maps\\{nmc.ViewModel.ImageMD5}\\{nmc.ViewModel.Name}.info\" already exists.");
                    return;
                }
                string imageFileExtension = System.IO.Path.GetExtension(nmc.ViewModel.ImageFilePath);
                MapFileDefinition map = new MapFileDefinition();
                map.Name = nmc.ViewModel.Name;
                map.UnitLabel = nmc.ViewModel.UnitLabel;
                map.ImageMD5 = nmc.ViewModel.ImageMD5;
                map.SampleUnits = nmc.ViewModel.SampleUnits;
                map.SamplePixels = nmc.ViewModel.SamplePixels;
                map.ImageExt = imageFileExtension;
                string jsonString = JsonSerializer.Serialize(map, jsonSerializerOptions);
                Directory.CreateDirectory($"{saveLocation}\\maps\\{nmc.ViewModel.ImageMD5}");
                if (!File.Exists($"{saveLocation}\\maps\\{nmc.ViewModel.ImageMD5}\\map_image{imageFileExtension}"))
                    File.Copy(nmc.ViewModel.ImageFilePath, $"{saveLocation}\\maps\\{nmc.ViewModel.ImageMD5}\\map_image{imageFileExtension}", true);
                File.WriteAllText($"{saveLocation}\\maps\\{nmc.ViewModel.ImageMD5}\\{nmc.ViewModel.Name}.info", jsonString);
                NotificationControl.ShowNotificationAndHide($"Saved map \"{map.Name}\".");
                if (!LoadMap(map))
                    NotificationControl.ShowError($"Could not load map \"{map.Name}\".");
                ShowDefaultPanel();
            }
        }

        private void LoadMapControl_LoadButtonClicked(object sender, EventArgs e)
        {
            LoadMapControl lmc = (LoadMapControl)sender;
            if (lmc.Selected != null)
            {
                if (LoadMap(lmc.Selected))
                    NotificationControl.ShowNotificationAndHide($"Loaded map \"{lmc.Selected.Name}\".");
                else
                    NotificationControl.ShowError($"Could not load map \"{lmc.Selected.Name}\".");
                ShowDefaultPanel();
            }
        }

        private void SavePathControl_SaveButtonClicked(object sender, EventArgs e)
        {
            SavePathControl spc = (SavePathControl)sender;
            string pathName = spc.PathName;
            if (!string.IsNullOrWhiteSpace(pathName))
            {
                string imageMD5 = ViewModel.MapImageMD5;
                if (File.Exists($"{saveLocation}\\maps\\{imageMD5}\\paths\\{pathName}.info"))
                {
                    NotificationControl.ShowError($"A file with the name \"{saveLocation}\\maps\\{imageMD5}\\paths\\{pathName}.info\" already exists.");
                    return;
                }
                PathFileDefinition pathFileDefinition = new PathFileDefinition();
                pathFileDefinition.Name = pathName;
                pathFileDefinition.ImageMD5 = imageMD5;
                pathFileDefinition.Path = ViewModel.Path;
                string jsonString = JsonSerializer.Serialize(pathFileDefinition, jsonSerializerOptions);
                Directory.CreateDirectory($"{saveLocation}\\maps\\{imageMD5}\\paths");
                File.WriteAllText($"{saveLocation}\\maps\\{imageMD5}\\paths\\{pathName}.info", jsonString);
                ShowDefaultPanel();
                NotificationControl.ShowNotificationAndHide($"Saved path \"{pathFileDefinition.Name}\".");
            }
        }

        private void LoadPathControl_LoadButtonClicked(object sender, EventArgs e)
        {
            LoadPathControl lpc = (LoadPathControl)sender;
            PathFileDefinition pathFileDefinition = lpc.SelectedPath;
            if (pathFileDefinition != null)
            {
                ApplyPath(new Path());
                try
                {
                    ApplyPath(pathFileDefinition.Path);
                    StorePathState();
                    NotificationControl.ShowNotificationAndHide($"Loaded path \"{lpc.SelectedPath.Name}\".");
                }
                catch
                {
                    NotificationControl.ShowError("Could not load path.");
                }
                ShowDefaultPanel();
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            PathNodesInfoListView.Tag = "ShowNamedOnly";
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            PathNodesInfoListView.Tag = "ShowAll";
        }

        private void NameBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Focus();
            tb.IsReadOnly = false;
            tb.SelectAll();
        }

        private void PathNodesInfoListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView listView = (ListView)sender;
            if (listView.SelectedItem != null)
            {
                PathNode selectedNode = (PathNode)listView.SelectedItem;
                ViewModel.Path.SelectedIndex = selectedNode.Index;
            }
        }

        private void ListViewItem_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            ((ListViewItem)sender).IsSelected = true;
        }

        private void LoadMapControl_DeleteRequested(object sender, EventArgs e)
        {
            LoadMapControl lmc = (LoadMapControl)sender;
            MapFileDefinition mapFileDefinition = lmc.Selected;
            if (mapFileDefinition != null)
            {
                string message = $"Are you sure you want to delete the map \"{mapFileDefinition.Name}\"?\nThis can also result in the removal of associated paths.";
                ConfirmActionWindow caw = new ConfirmActionWindow(message);
                if (caw.ShowDialog().GetValueOrDefault())
                {
                    List<MapFileDefinition> maps = new List<MapFileDefinition>();
                    if (Directory.Exists($"{saveLocation}\\maps"))
                        foreach (string subdir in Directory.GetDirectories($"{saveLocation}\\maps"))
                            foreach (string mapInfoFile in Directory.GetFiles(subdir, "*.info"))
                                try
                                {
                                    string jsonString = File.ReadAllText(mapInfoFile);
                                    MapFileDefinition map = JsonSerializer.Deserialize<MapFileDefinition>(jsonString, jsonSerializerOptions);
                                    if (map != null)
                                    {
                                        if (mapFileDefinition.Equals(map))
                                        {
                                            File.Delete(mapInfoFile);
                                            if (Directory.GetFiles(subdir, "*.info").Length < 1)
                                            {
                                                //also delete associated files (paths, map image) when there are no other map definitions using the same map image base
                                                Directory.Delete(subdir, true);
                                                NotificationControl.ShowNotificationAndHide($"Deleted map \"{mapFileDefinition.Name}\" and all associated files.");
                                            }
                                            else
                                                NotificationControl.ShowNotificationAndHide($"Deleted map \"{mapFileDefinition.Name}\".\nAssociated files that are used by other map definitions have not been deleted.", 6000);
                                        }
                                        else
                                            maps.Add(map);
                                    }
                                }
                                catch { }
                    if (maps.Count > 0)
                        LoadMapControl.ResetControl(maps);
                    else
                        ShowDefaultPanel();
                }
            }
        }

        private void LoadPathControl_DeleteRequested(object sender, EventArgs e)
        {
            LoadPathControl lpc = (LoadPathControl)sender;
            PathFileDefinition pathFileDefinition = lpc.SelectedPath;
            if (pathFileDefinition != null)
            {
                string message = $"Are you sure you want to delete the path \"{pathFileDefinition.Name}\"?";
                ConfirmActionWindow caw = new ConfirmActionWindow(message);
                if (caw.ShowDialog().GetValueOrDefault())
                {
                    List<PathFileDefinition> paths = new List<PathFileDefinition>();
                    string dir = $"{saveLocation}\\maps\\{ViewModel.MapImageMD5}\\paths";
                    if (Directory.Exists(dir))
                        foreach (string pathInfoFile in Directory.GetFiles(dir, "*.info"))
                            try
                            {
                                string jsonString = File.ReadAllText(pathInfoFile);
                                PathFileDefinition path = JsonSerializer.Deserialize<PathFileDefinition>(jsonString, jsonSerializerOptions);
                                if (path != null)
                                {
                                    if (pathFileDefinition.Equals(path))
                                    {
                                        File.Delete(pathInfoFile);
                                        NotificationControl.ShowNotificationAndHide($"Deleted path \"{pathFileDefinition.Name}\".");
                                    }
                                    else
                                        paths.Add(path);
                                }
                            }
                            catch { }
                    if (paths.Count > 0)
                        LoadPathControl.ResetControl(paths);
                    else
                        ShowDefaultPanel();
                }
            }
        }
        #endregion

        private struct PanelButtonTuple
        {
            public FrameworkElement Panel;
            public Button Button;
        }
    }
}
