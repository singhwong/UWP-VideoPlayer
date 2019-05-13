using FFmpegInterop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VideoPlayer.Commands;
using VideoPlayer.Controls;
using VideoPlayer.Modle;
using VideoPlayer.Modles;
using VideoPlayer.ViewModle;
using Windows.ApplicationModel.Contacts;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Media.Core;
//using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.Storage.Search;
using Windows.Storage.Streams;
using Windows.System;
using Windows.System.Display;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace VideoPlayer
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private static StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
        private readonly string filePath = storageFolder.Path + @"\histroyProgress.xml";
        private string playModeStr_default;
        private string playModeStr_recycle;
        private string playModeStr_list;
        private string playModeStr_listRecycle;
        private string playRate_str;
        private DisplayRequest display_Request = null;
        private bool IsBottomShow = true;
        private bool scylePlay_bool = false;
        private bool listPlay_bool = false;
        private bool defaultPlay_bool = true;
        private ObservableCollection<StorageFile> all_video = new ObservableCollection<StorageFile>();
        private ObservableCollection<Video> use_video = new ObservableCollection<Video>();
        private Video main_video = new Video();
        private Video video_value = new Video();
        #region 初始化color
        private SolidColorBrush lightGray = new SolidColorBrush(Colors.LightGray);
        private SolidColorBrush whiteSmoke = new SolidColorBrush(Colors.WhiteSmoke);
        private SolidColorBrush white = new SolidColorBrush(Colors.White);
        private SolidColorBrush skyblue = new SolidColorBrush(Colors.SkyBlue);
        private SolidColorBrush lightPink = new SolidColorBrush(Colors.LightPink);
        private SolidColorBrush yellow = new SolidColorBrush(Colors.Yellow);
        private SolidColorBrush cornFlowerBlue = new SolidColorBrush(Colors.CornflowerBlue);
        private SolidColorBrush black = new SolidColorBrush(Colors.Black);
        private SolidColorBrush use_foreGround = new SolidColorBrush();
        #endregion
        private ApplicationDataContainer local_theme = ApplicationData.Current.LocalSettings;
        private ApplicationDataContainer local_systemSound = ApplicationData.Current.LocalSettings;
        private ApplicationDataContainer local_systemSoundVolume = ApplicationData.Current.LocalSettings;
        private ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView();

        private SystemMediaTransportControls systemMedia_TransportControls = SystemMediaTransportControls.GetForCurrentView();
        private ApplicationDataContainer history_volume = ApplicationData.Current.LocalSettings;
        private ApplicationDataContainer commentDialog_bool = ApplicationData.Current.LocalSettings;
        //public static MainPage page;
        private DispatcherTimer tmr = new DispatcherTimer();
        private DispatcherTimer progress_tmr = new DispatcherTimer();
        private DispatcherTimer dateTime_tmr = new DispatcherTimer();
        private DispatcherTimer hideBottom_tmr = new DispatcherTimer();
        private Video[] remove_VideoList;
        private FrameworkElement sender_value;
        private MediaPlaybackItem mediaitem;
        private Contact recipient = new Contact();
        private VolumeContentDialog volume_dialog;
        private FFmpegInteropMSS FFmpegMss;
        private TracksCount tracks_count;
        private string audioCount_str;
        private string videoCount_str;
        private string timedCont_str;
        private string volumeFontIcon_str;
        private bool IsAudioTracksChanged_bool = false;
        private bool IsVideoTracksChanged_bool = false;
        private bool IsTimedTracksChanged_bool = false;
        private bool IsCommentShow_bool = false;
        private CoreApplicationViewTitleBar coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
        private ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
        public MainPage()
        {
            this.InitializeComponent();
            //this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            ExtendAcrylicIntoTitleBar();
            SetIsElementSoundPlayerIsOn();
            //page = this;
            #region  设置计时器，自动隐藏cursor   
            tmr.Interval = TimeSpan.FromSeconds(5);
            tmr.Tick += OnTimerTick;
            #endregion           
            SetDateTimeMethod();
            SetAutoHideBottomMethod();
            main_mediaElement.MediaEnded += new RoutedEventHandler(Main_mediaElementMediaEnded);
            main_mediaElement.VolumeChanged += new RoutedEventHandler(Main_mediaElementVolumeChanged);
            content_gridView.SelectionChanged += new SelectionChangedEventHandler(Content_gridViewSelectionChanged);
            main_mediaElement.MediaOpened += new RoutedEventHandler(Main_mediaElementMediaOpened);
            main_mediaElement.MediaFailed += new ExceptionRoutedEventHandler(Main_mediaElementMediaFailed);
            main_mediaElement.CurrentStateChanged += new RoutedEventHandler(main_mediaElementCurrentStateChanged);
            play_button.Tapped += new TappedEventHandler(Play_buttonTapped);
            progress_slider.ValueChanged += new RangeBaseValueChangedEventHandler(Progress_sliderValueChanged);
            volume_slider.ValueChanged += new RangeBaseValueChangedEventHandler(Volume_sliderValueChanged);
            next_button.Tapped += new TappedEventHandler(Next_buttonTapped);
            previous_button.Tapped += new TappedEventHandler(Previous_buttonTapped);
            fullScreen_button.Tapped += new TappedEventHandler(FullScreen_buttonTapped);
            bottom_grid.PointerEntered += new PointerEventHandler(Bottom_gridPointerEntered);
            bottom_grid.PointerExited += new PointerEventHandler(Bottom_gridPointerExited);
            item_0_1.Tapped += new TappedEventHandler(Item_0_1Tapped);
            item_0_2.Tapped += new TappedEventHandler(Item_0_2Tapped);
            item_0_3.Tapped += new TappedEventHandler(Item_0_3Tapped);
            item_0_4.Tapped += new TappedEventHandler(Item_0_4Tapped);
            item_0_5.Tapped += new TappedEventHandler(Item_0_5Tapped);
            item_1_0.Tapped += new TappedEventHandler(Item_1_0Tapped);
            item_1_5.Tapped += new TappedEventHandler(Item_1_5Tapped);
            item_2_0.Tapped += new TappedEventHandler(Item_2_0Tapped);
            item_2_5.Tapped += new TappedEventHandler(Item_2_5Tapped);
            item_3_0.Tapped += new TappedEventHandler(Item_3_0Tapped);
            addFolder_button.Tapped += new TappedEventHandler(AddFolder_buttonTapped);
            content_gridView.ItemClick += new ItemClickEventHandler(Content_gridViewItemClick);
            allSelect_checkBox.Tapped += new TappedEventHandler(AllSelect_checkBoxTapped);
            settings_button.Tapped += new TappedEventHandler(Settings_buttonTapped);
            openCutAdress_button.Tapped += new TappedEventHandler(OpenCutAdress_buttonTapped);
            editPicture_button.Tapped += new TappedEventHandler(EditPicture_buttonTapped);
            closeCutDialog_button.Tapped += new TappedEventHandler(CloseCutDialog_buttonTapped);
            Fill_item.Tapped += new TappedEventHandler(Fill_itemTapped);
            UniformToFill_item.Tapped += new TappedEventHandler(UniformToFill_itemTapped);
            Uniform_item.Tapped += new TappedEventHandler(Uniform_itemTapped);
            None_item.Tapped += new TappedEventHandler(None_itemTapped);
            closeDialog_Button.Tapped += new TappedEventHandler(CloseDialog_ButtonTapped);
            search_autoSuggestBox.QuerySubmitted += new TypedEventHandler<AutoSuggestBox, AutoSuggestBoxQuerySubmittedEventArgs>(Search_autoSuggestBoxQuerySubmitted);
            search_autoSuggestBox.TextChanged += new TypedEventHandler<AutoSuggestBox, AutoSuggestBoxTextChangedEventArgs>(Search_autoSuggestBoxTextChanged);
            refreshVideos_button.Tapped += new TappedEventHandler(RefreshVideos_buttonTapped);
            addFolders_button.Tapped += new TappedEventHandler(AddFolders_buttonTapped);
            email_button.Tapped += new TappedEventHandler(Email_buttonTapped);
            about_button.Tapped += new TappedEventHandler(About_buttonTapped);
            comment_button.Tapped += new TappedEventHandler(Comment_buttonTapped);
            SystemSound_toggleSwitch.Toggled += new RoutedEventHandler(SystemSound_toggleSwitchToggled);
            soundVolume_slider.ValueChanged += new RangeBaseValueChangedEventHandler(SoundVolume_sliderValueChanged);
            ClearData_button.Tapped += new TappedEventHandler(ClearData_buttonTapped);
            click_grid.PointerEntered += new PointerEventHandler(Click_gridPointerEntered);
            show_bottom.Completed += new EventHandler<object>(Show_bottomCompleted);
            closeSettings_button.Tapped += new TappedEventHandler(CloseSettings_buttonTapped);
            Back_button.Tapped += new TappedEventHandler(Back_buttonTapped);
            multiple_button.Tapped += new TappedEventHandler(Multiple_buttonTapped);
            addTimedText_button.Tapped += new TappedEventHandler(AddTimedText_buttonTapped);
            folders_gridView.ItemClick += new ItemClickEventHandler(Folders_gridViewItemClick);
            theme_button.Tapped += new TappedEventHandler(Theme_buttonTapped);
            folders_gridView.SelectionChanged += new SelectionChangedEventHandler(Folders_gridViewSelectionChanged);
            rewind_button.Tapped += new TappedEventHandler(Rewind_buttonTapped);
            fastForward_button.Tapped += new TappedEventHandler(FastForward_buttonTapped);
            frombin_item.Tapped += new TappedEventHandler(Frombin_itemTapped);
            fromlist_item.Tapped += new TappedEventHandler(Fromlist_itemTapped);
            remove_item.Tapped += new TappedEventHandler(Remove_itemTapped);
            null_item.Tapped += new TappedEventHandler(Null_itemTapped);
            replay_item.Tapped += new TappedEventHandler(Replay_itemTapped);
            list_item.Tapped += new TappedEventHandler(List_itemTapped);
            progressLisbox_item.Tapped += new TappedEventHandler(ProgressLisbox_itemTapped);
            minSize_button.Tapped += new TappedEventHandler(MinSize_buttonTapped);
            main_border.PointerMoved += new PointerEventHandler(Main_borerPointerMoved);
            main_border.Tapped += new TappedEventHandler(Main_borerTapped);
            main_border.DoubleTapped += new DoubleTappedEventHandler(Main_borerDoubleTapped);
            main_border.PointerExited += new PointerEventHandler(Main_borerPointerExited);
            search_button.Tapped += new TappedEventHandler(Search_buttonTapped);
            this.SizeChanged += new SizeChangedEventHandler(PageSizeChanged);
            progress_slider.Drop += new DragEventHandler(Progress_sliderDrop);
            progress_slider.DropCompleted += new TypedEventHandler<UIElement, DropCompletedEventArgs>(Progress_sliderDropCompleted);
            show_topGrid.Completed += new EventHandler<object>(Show_topGridCompleted);
            hide_topGrid.Completed += new EventHandler<object>(Hide_topGridCompleted);
            screenShot_item.Click += new RoutedEventHandler(ScreenShot_itemClick);
            screenShot_button.Tapped += new TappedEventHandler(ScreenShot_itemClick);
        }

        private void SetIsElementSoundPlayerIsOn()
        {
            try
            {
                string isOn_str = local_systemSound.Values["IsSoundOn"].ToString();
                switch (isOn_str)
                {
                    case "On":
                        ElementSoundPlayer.State = ElementSoundPlayerState.On;
                        SystemSound_toggleSwitch.IsOn = true;
                        break;
                    case "Off":
                        ElementSoundPlayer.State = ElementSoundPlayerState.Off;
                        SystemSound_toggleSwitch.IsOn = false;
                        break;
                }
            }
            catch
            {
                ElementSoundPlayer.State = ElementSoundPlayerState.On;
                SystemSound_toggleSwitch.IsOn = true;
            }
            try
            {
                var soundVolume_value = (double)local_systemSoundVolume.Values["Volume_setting"];
                ElementSoundPlayer.Volume = soundVolume_value;
                soundVolume_slider.Value = soundVolume_value * 100;
            }
            catch
            {
                soundVolume_slider.Value = 30;
                ElementSoundPlayer.Volume = 0.3;
            }
        }
        //private void main_mediaElement_PointerWheelChanged(CoreWindow sender, PointerEventArgs args)
        //{
        //    if (IsMediaElementPointEnter_bool)
        //    {
        //        #region 鼠标滚动调节音量
        //        var value = (double)args.CurrentPoint.Properties.MouseWheelDelta;
        //        if (value > 0)//value = 120;
        //        {
        //            main_mediaElement.Volume += 0.05;
        //        }
        //        else//value = -120;
        //        {
        //            main_mediaElement.Volume -= 0.05;
        //        }
        //        #endregion              
        //    }
        //}

        #region 将亚克力扩展到标题栏
        private void ExtendAcrylicIntoTitleBar()
        {

            coreTitleBar.ExtendViewIntoTitleBar = false;
            UpdateTitleBarLayout(coreTitleBar);
            //Window.Current.SetTitleBar(AppTitleBar);
            //titleBar.ButtonForegroundColor = Colors.White;
            //titleBar.ButtonHoverForegroundColor = Colors.White;
            //titleBar.ButtonHoverBackgroundColor = Colors.Black;
            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;
            //coreTitleBar.IsVisibleChanged += CoreTitleBar_IsVisibleChanged;
        }
        #endregion


        private void UpdateTitleBarLayout(CoreApplicationViewTitleBar coreTitleBar)
        {
            // Get the size of the caption controls area and back button 
            // (returned in logical pixels), and move your content around as necessary.
            //LeftPaddingColumn.Width = new GridLength(coreTitleBar.SystemOverlayLeftInset);
            //RightPaddingColumn.Width = new GridLength(coreTitleBar.SystemOverlayRightInset);
            //topButton_stackPanel.Margin = new Thickness(0, 0, coreTitleBar.SystemOverlayRightInset, 0);
            //top_stackPanel.Margin = new Thickness(coreTitleBar.SystemOverlayLeftInset, 0, 0, 0);

            // Update title bar control size as needed to account for system size changes.
            //AppTitleBar.Height = coreTitleBar.Height;
            topTitleBar_grid.Height = coreTitleBar.Height;
            topTitleBar_border.Height = coreTitleBar.Height;
        }

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            UpdateTitleBarLayout(sender);
        }

        //private void CoreTitleBar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
        //{
        //    if (sender.IsVisible)
        //    {
        //        AppTitleBar.Visibility = Visibility.Visible;
        //    }
        //    else
        //    {
        //        AppTitleBar.Visibility = Visibility.Collapsed;
        //    }
        //}
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            GetLocalTheme();
            ReadLocalStr();
            ShowCursor();
            GetFolderLibraryListViewItems();
            await GetVideoLibrary();
            ButtonTipText();
            GetHistoryVlumeValue();
            #region 启用后台播放控件
            systemMedia_TransportControls.IsPlayEnabled = true;
            systemMedia_TransportControls.IsPauseEnabled = true;
            systemMedia_TransportControls.ButtonPressed += SystemControls_ButtonPressed;
            #endregion           
            try
            {
                IsCommentShow_bool = (bool)commentDialog_bool.Values["comment_bool"];
            }
            catch
            {
            }

        }

        private async void SystemControls_ButtonPressed(SystemMediaTransportControls sender,
   SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            #region 后台控件点击事件
            switch (args.Button)
            {
                case SystemMediaTransportControlsButton.Play:
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        main_mediaElement.AutoPlay = true;
                        main_mediaElement.Play();
                    });
                    break;
                case SystemMediaTransportControlsButton.Pause:
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        main_mediaElement.Pause();
                    });
                    break;
                case SystemMediaTransportControlsButton.Previous:
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        PreviousPlaySameMethod();
                    });
                    break;
                case SystemMediaTransportControlsButton.Next:
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        SameCodeMethod();
                    });
                    break;
                default:
                    break;
            }
            #endregion
        }

        private void PreviousPlaySameMethod()
        {
            //GetCurrentTheme();
            try
            {
                main_notify.Hide();
            }
            catch
            {
            }
            forceDecodeVideo_bool = false;
            index = SetListPlayMusic() - 1;
            try
            {
                if (index == -1)
                {
                    main_video = use_video[use_video.Count - 1];
                }
                else
                {
                    main_video = use_video[index];
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                main_video = use_video[use_video.Count - 1];
            }
            SameCodeHelp();
        }
        private void main_mediaElementCurrentStateChanged(object sender, RoutedEventArgs e)//MediaPlayerElement无currentStateChanged事件，                                                                                            
                                                                                           //改用MediaElement控件
        {
            main_mediaElement.TransportControls.IsCompactOverlayEnabled = true;
            main_mediaElement = sender as MediaElement;
            if (main_mediaElement != null)
            {
                if (main_mediaElement.CurrentState == Windows.UI.Xaml.Media.MediaElementState.Playing)
                {
                    play_button.Content = "\uE769";
                    SetProgressTimerTick();
                    //tmr.Start();
                    if (display_Request == null)
                    {
                        display_Request = new DisplayRequest();
                        display_Request.RequestActive();
                    }
                }
                else
                {
                    play_button.Content = "\uF5B0";
                    progress_tmr.Stop();
                    if (display_Request != null)
                    {
                        display_Request.RequestRelease();
                        display_Request = null;
                    }
                }
            }
            #region 后台控件状态改变
            switch (main_mediaElement.CurrentState)
            {
                case MediaElementState.Playing:
                    systemMedia_TransportControls.PlaybackStatus = MediaPlaybackStatus.Playing;
                    break;
                case MediaElementState.Paused:
                    systemMedia_TransportControls.PlaybackStatus = MediaPlaybackStatus.Paused;
                    break;
                case MediaElementState.Stopped:
                    systemMedia_TransportControls.PlaybackStatus = MediaPlaybackStatus.Paused;
                    break;
                default:
                    break;
            }
            #endregion
        }

        //private void Main_mediaElementPointerPressed(object sender, PointerRoutedEventArgs e)
        //{
        //    var temp = e.GetCurrentPoint(sender as Button);
        //    if (temp.Properties.IsRightButtonPressed)
        //    {
        //    }
        //    else if (temp.Properties.IsLeftButtonPressed)
        //    {

        //    }
        //}

        private async Task GetVideoLibrary()
        {
            folders_gridView.SelectedItem = null;
            await GetLocalVideo(main_folder);
            contentGridViewHeader_textBlock.Text = contentGridViewHeader_str;
            SetNotifyContent(use_video.Count.ToString() + " " + videosCount_str, TimeSpan.FromSeconds(2));
        }

        private async Task GetAllMedias(ObservableCollection<StorageFile> list, StorageFolder folder)
        {
            string[] file_Types = new string[] { ".mp4", ".wmv", ".mkv", ".avi", ".3gp", ".flv", ".mpg", ".webm", ".mov", ".Ogg", ".swf", ".rmvb" ,
            ".f4v",".m4v",".rm",".dat",".ts",".mts",".vob",".mpeg",".m2ts",".tp"};
            string[] fileTypes = new string[] { "*" };
            QueryOptions queryOption = new QueryOptions
       (CommonFileQuery.OrderByTitle, fileTypes)
            {
                FolderDepth = FolderDepth.Deep
            };
            //Queue<IStorageFolder> folders = new Queue<IStorageFolder>();
            var files = await folder.CreateFileQueryWithOptions
              (queryOption).GetFilesAsync();
            foreach (var file in files)
            {
                foreach (var type in file_Types)
                {
                    if (file.FileType.Contains(type))
                    {
                        list.Add(file);
                    }
                }
            }
        }

        private IObservableVector<StorageFolder> myPictureFolders;
        private async Task GridView_Videos(ObservableCollection<StorageFile> files)
        {
            foreach (var video in files)
            {
                Video this_video = new Video();
                VideoProperties video_Properties = await video.Properties.GetVideoPropertiesAsync();
                try//防止专辑图片为空
                {
                    StorageItemThumbnail current_Thumb = await video.GetThumbnailAsync(
                ThumbnailMode.VideosView,
                200,
                ThumbnailOptions.UseCurrentScale
                );
                    BitmapImage video_cover = new BitmapImage();
                    await video_cover.SetSourceAsync(current_Thumb);
                    this_video.Cover = video_cover;
                }
                catch
                {
                }
                IRandomAccessStream video_stream = await video.OpenAsync(FileAccessMode.Read);
                this_video.Video_Stream = video_stream;
                SetListTimeMethod((int)video_Properties.Duration.TotalSeconds);
                this_video.Video_Duration = list_hh + ":" + list_mm + ":" + list_ss;
                this_video.Duration = video_Properties.Duration;
                if (!string.IsNullOrEmpty(video_Properties.Title))
                {
                    this_video.Video_Title = video_Properties.Title;
                }
                else
                {
                    this_video.Video_Title = video.Name;
                }
                this_video.VideoFile = video;
                this_video.File_Date = $"{video.DateCreated.Month}/{video.DateCreated.Day}/{video.DateCreated.Year}";
                this_video.IsSelected = false;
                //SetDefaultForeGround(this_video);
                SaveProgressVM.ReadProgressData(this_video, progresslist.progress_list, filePath);
                //添加文件夹视频时，跳过重复文件名视频
                var contain_videos = use_video.Where(p => p.VideoFile.Path == this_video.VideoFile.Path).Select(p => p.VideoFile.Path).ToList();
                if (contain_videos.Count == 0)
                {
                    use_video.Add(this_video);
                    progressRing_textBlock.Text = use_video.Count.ToString();
                }
                //if (main_video != null)
                //{
                //    foreach (var item in use_video)
                //    {
                //        if (item.Video_Path == main_video.Video_Path)
                //        {
                //            main_video = item;
                //            //item.Video_Color = Color.FromArgb(50, 70, 0, 70);
                //        }
                //    }
                //}
            }
            SetProgressList();
        }

        private StorageFolder main_folder = KnownFolders.VideosLibrary;
        private async Task GetLocalVideo(StorageFolder folder)
        {
            all_video.Clear();
            use_video.Clear();
            content_gridView.Visibility = Visibility.Collapsed;
            try
            {
                progress_grid.Visibility = Visibility.Visible;
                content_progressRing.IsActive = true;
                await GetAllMedias(all_video, folder);
                await GridView_Videos(all_video);
                progress_grid.Visibility = Visibility.Collapsed;
                content_progressRing.IsActive = false;
                content_gridView.Visibility = Visibility.Visible;
            }
            catch
            {
            }
            if (use_video.Count == 0)
            {
                progressRing_textBlock.Text = "";
                content_progressRing.IsActive = false;
                progress_grid.Visibility = Visibility.Collapsed;
            }
        }

        private void GetCurrentTheme()
        {
            //main_video.Video_Color = Color.FromArgb(50, 0, 55, 100);
        }

        private void SetDefaultForeGround(Video video)
        {
            //video.Video_Color = Color.FromArgb(50, 0, 55, 100);
        }

        private void SetVideoRemovedContentDialog()
        {
            string playError_str = resourceLoader.GetString("playError_str");
            SetNotifyContent(playError_str, TimeSpan.FromSeconds(10));
            use_video.Remove(main_video);
        }

        private void delete_menuItem_Click(object sender, RoutedEventArgs e)
        {
            RemoveItemSameCode();
        }

        private void SetFullScreenMethod()
        {
            var view = ApplicationView.GetForCurrentView();
            if (view.IsFullScreenMode)
            {
                dateTime_textBlock.Visibility = Visibility.Collapsed;
                dateTime_tmr.Stop();
                view.ExitFullScreenMode();
                fullScreen_button.Content = "\uE740";
            }
            else
            {
                dateTime_textBlock.Visibility = Visibility.Visible;
                dateTime_tmr.Start();
                view.TryEnterFullScreenMode();
                fullScreen_button.Content = "\uE73F";
            }
            minSize_button.Content = "\uE841";
            ToolTipService.SetToolTip(minSize_button, pin_str);
            Back_button.Visibility = Visibility.Visible;
            title_textBlock.FontSize = 15;
        }

        private void bin_item_Click(object sender, RoutedEventArgs e)
        {
            RemoveTo(StorageDeleteOption.Default);
        }

        private void RemoveItemSameCode()
        {
            video_value = (Video)sender_value.DataContext;
            use_video.Remove(video_value);
            if (search_videoCollection.Contains(video_value))
            {
                search_videoCollection.Remove(video_value);
            }
            SetNotifyContent(use_video.Count.ToString() + " " + videosCount_str, TimeSpan.FromSeconds(2));
        }

        private async void RemoveTo(StorageDeleteOption value)
        {
            RemoveItemSameCode();
            StorageFile file = video_value.VideoFile;
            try
            {
                await file.DeleteAsync(value);
            }
            catch (System.IO.FileNotFoundException)
            {
            }
        }
        private void directly_item_Click(object sender, RoutedEventArgs e)
        {
            RemoveTo(StorageDeleteOption.PermanentDelete);
        }

        #region 启用 空格键、ESC键
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
            //Window.Current.CoreWindow.PointerWheelChanged += main_mediaElement_PointerWheelChanged;
        }

        private void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            if (args.VirtualKey == Windows.System.VirtualKey.Space)
            {//启用空格键控制播放和暂停
                if (main_mediaElement.CurrentState == Windows.UI.Xaml.Media.MediaElementState.Playing)
                {
                    main_mediaElement.Pause();
                }
                else if (main_mediaElement.CurrentState == Windows.UI.Xaml.Media.MediaElementState.Paused || main_mediaElement.CurrentState == MediaElementState.Stopped)
                {
                    main_mediaElement.Play();
                }
            }
            if (args.VirtualKey == Windows.System.VirtualKey.Escape)
            {//启用ESC键退出全屏
                var view = ApplicationView.GetForCurrentView();
                if (view.IsFullScreenMode)
                    view.ExitFullScreenMode();
                dateTime_textBlock.Visibility = Visibility.Collapsed;
            }
            #region 方向键控制
            if (args.VirtualKey == VirtualKey.Left)
            {
                main_mediaElement.Position -= TimeSpan.FromSeconds(10);//后退10s
            }
            if (args.VirtualKey == VirtualKey.Right)
            {
                main_mediaElement.Position += TimeSpan.FromSeconds(30);//快进30s
            }
            if (args.VirtualKey == VirtualKey.Up)
            {
                main_mediaElement.Volume += 0.05;//音量升0.05
            }
            if (args.VirtualKey == VirtualKey.Down)
            {
                main_mediaElement.Volume -= 0.05;//音量降0.05
            }
            #endregion
        }

        #endregion
        private string list_hh;
        private string list_mm;
        private string list_ss;
        private void SetListTimeMethod(int allTime)
        {
            int HH = allTime / 3600;
            int MM = (allTime - HH * 3600) / 60;
            int SS = allTime % 60;
            if (HH < 10)
            {
                list_hh = "0" + HH.ToString();
            }
            else
            {
                list_hh = HH.ToString();
            }
            if (MM < 10)
            {
                list_mm = "0" + MM.ToString();
            }
            else
            {
                list_mm = MM.ToString();
            }
            if (SS < 10)
            {
                list_ss = "0" + SS.ToString();
            }
            else
            {
                list_ss = SS.ToString();
            }
        }

        private int index = 0;
        private void Main_mediaElementMediaEnded(object sender, RoutedEventArgs e)
        {
            //Solution: The actual time broadcast total time shows one second less.
            showTime_textBlock.Text = PlayingTime.GetPlayTime((int)main_mediaElement.NaturalDuration.TimeSpan.TotalSeconds, (int)main_mediaElement.NaturalDuration.TimeSpan.TotalSeconds);

            forceDecodeVideo_bool = false;
            progress_tmr.Stop();
            main_video.History_progress = 100;
            SaveVideoProgressMethod();
            main_mediaElement.AutoPlay = true;
            #region 播放模式设置
            if (scylePlay_bool)
            {
                index = SetListPlayMusic();
                //GetCurrentTheme();
                if (index >= 0)
                {
                    main_video = use_video[index];
                    SameCodeHelp();
                }
                else
                {
                    main_mediaElement.Play();
                }
                main_video.History_progress = 0;
                ClearProgressListValueSameCode(main_video);
            }
            else if (listPlay_bool)
            {
                if (use_video.Count == 1)
                {
                    index = 0;
                }
                else
                {
                    index = SetListPlayMusic() + 1;
                }
                if (index <= use_video.Count - 1)
                {
                    //GetCurrentTheme();
                    main_video = use_video[index];
                    SameCodeHelp();
                }
            }
            else if (defaultPlay_bool)
            {
                main_mediaElement.Pause();
            }
            //else if (listScyle_bool)
            //{
            //    SameCodeMethod();
            //}
            #endregion
            SetCommentMethod();
        }

        private int num;
        private int SetListPlayMusic()
        {
            var list_media = SetVideoByPath.GetVideoByStream(use_video, main_video.Video_Title);
            if (list_media.Video_Title != null)
            {
                for (int i = 0; i <= use_video.Count - 1; i++)
                {
                    if (use_video[i] == list_media)
                    {
                        num = i;
                    }
                }
            }
            else
            {
                num = -1;
            }
            return num;
        }

        private MediaSource main_mediaSource;
        private bool forceDecodeVideo_bool;
        private void SetMediaItemMethod()
        {
            try
            {
                IsAudioTracksChanged_bool = false;
                IsVideoTracksChanged_bool = false;
                IsTimedTracksChanged_bool = false;
                metadata_StackPanel.Children.Clear();
                if (forceDecodeVideo_bool)
                {
#pragma warning disable CS0618 // 类型或成员已过时
                    FFmpegMss = FFmpegInteropMSS.CreateFFmpegInteropMSSFromStream(main_video.Video_Stream, false, true);
#pragma warning restore CS0618 // 类型或成员已过时
                    main_mediaSource = MediaSource.CreateFromMediaStreamSource(FFmpegMss.GetMediaStreamSource());
                }
                else
                {
                    main_mediaSource = MediaSource.CreateFromStorageFile(main_video.VideoFile);
                }
                mediaitem = new MediaPlaybackItem(main_mediaSource);
                mediaitem.AudioTracksChanged += PlaybackItem_AudioTracksChanged;
                mediaitem.VideoTracksChanged += MediaPlaybackItem_VideoTracksChanged;
                mediaitem.TimedMetadataTracksChanged += MediaPlaybackItem_TimedMetadataTracksChanged;
                main_mediaElement.SetPlaybackSource(mediaitem);
            }
            catch (NullReferenceException)
            {

                SetVideoRemovedContentDialog();//正在播放视频文件被删除，再次点击播放该视频引发的异常
            }
            catch (AccessViolationException)
            {
                SetNotifyContent(unknowError_str, TimeSpan.FromSeconds(2));
            }
            catch
            {
                SetNotifyContent(unknowError_str, TimeSpan.FromSeconds(2));
            }

        }

        private TimedTextSource text_src;
        private IRandomAccessStream src_stream;
        private string timedTextFile_name = "";
        private async void SetTimedTextSource(MediaSource source)
        {
            FileOpenPicker picker = new FileOpenPicker();
            string[] TimedTextFormat_str = new string[] { ".srt", ".ssa", ".ass", ".smi", ".sub", ".lrc", ".sst", ".txt", ".xss", ".psb", ".ssb" };
            foreach (var format in TimedTextFormat_str)
            {
                picker.FileTypeFilter.Add(format);
            }
            var str_file = await picker.PickSingleFileAsync();
            if (str_file != null)
            {
                timedTextFile_name = str_file.DisplayName;
                src_stream = await str_file.OpenAsync(FileAccessMode.Read);
                text_src = TimedTextSource.CreateFromStream(src_stream);
                source.ExternalTimedTextSources.Add(text_src);
            }
        }

        private void SameCodeHelp()
        {
            #region 播放模式设置方法中，重复代码块            
            progress_tmr.Stop();
            SetMediaItemMethod();
            main_mediaElement.Play();
            //main_video.Video_Color = Color.FromArgb(50, 70, 0, 70);
            #endregion
        }

        private void SameCodeMethod()
        {
            try
            {
                main_notify.Hide();
            }
            catch
            {
            }
            forceDecodeVideo_bool = false;
            #region 后台控件点击事件和播放模式方法中，重复代码块
            progress_tmr.Stop();
            //GetCurrentTheme();
            index = SetListPlayMusic() + 1;
            if (index <= use_video.Count - 1)
            {
                main_video = use_video[index];
            }
            else
            {
                main_video = use_video[0];
            }
            SameCodeHelp();
            #endregion
        }

        private void Main_mediaElementVolumeChanged(object sender, RoutedEventArgs e)
        {
            var volume_num = main_mediaElement.Volume;
            volume_slider.Value = (int)(volume_num * 100);
            history_volume.Values["volume"] = volume_num;
            try
            {
                volume_dialog.Hide();
            }
            catch
            {
            }
            if (main_mediaElement.IsMuted == false)
            {
                if (main_mediaElement.Volume > 0.8)
                {
                    volumeFontIcon_str = "\uF0EE";
                }
                else if (main_mediaElement.Volume > 0.5)
                {
                    volumeFontIcon_str = "\uF0ED";
                }
                else if (main_mediaElement.Volume > 0.1)
                {
                    volumeFontIcon_str = "\uF0EC";
                }
                else if (main_mediaElement.Volume == 0)
                {
                    volumeFontIcon_str = "\uE74F";
                }
                else
                {
                    volumeFontIcon_str = "\uF0EB";
                }
                volume_button.Content = volumeFontIcon_str;
                volume_dialog = new VolumeContentDialog(volumeFontIcon_str, ((int)(main_mediaElement.Volume * 100)).ToString() + "%");
                volume_dialog.Show();
            }
        }

        private void GetHistoryVlumeValue()
        {
            try
            {
                var volume_value = history_volume.Values["volume"];
                main_mediaElement.Volume = (double)volume_value;
                volume_slider.Value = (int)((double)volume_value * 100);
            }
            catch
            {
                main_mediaElement.Volume = 0.3;
            }
        }

        private void SetSelectedVideoSameCode()
        {
            num = 0;
            SetMediaItemMethod();
            //main_video.Video_Color = Color.FromArgb(50, 70, 0, 70);
        }
        private List<String> value = new List<string>();
        private string searchResult_str;
        private ObservableCollection<Video> search_videoCollection = new ObservableCollection<Video>();
        private bool IsMultipleClick_bool = false;
        private void Content_gridViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsMultipleClick_bool)
            {
                foreach (var item in use_video)
                {
                    item.IsSelected = false;
                }
                var selected_video = (GridView)sender;
                var value = selected_video.SelectedItems.ToList();
                foreach (var item in value)
                {
                    (item as Video).IsSelected = true;
                }
            }
            else
            {
                var value_item = (GridView)sender;
                value_item.SelectedItem = null;
            }

        }
        private ObservableCollection<VideoLibrary> videoLibrary_collection = new ObservableCollection<VideoLibrary>();
        private AddFoldersAccessDialog access_dialog;
        private async void GetFolderLibraryListViewItems()
        {
            try
            {
                myVideos = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Videos);
                myPictureFolders = myVideos.Folders;
                videoLibrary_collection.Clear();
                foreach (var item in myPictureFolders)
                {

                    VideoLibrary library = new VideoLibrary();
                    library.FolderName = item.DisplayName;
                    library.FolderPath = item.Path;
                    library.FolderImage = await GetFolderImage(item);
                    videoLibrary_collection.Add(library);
                }

            }
            catch (UnauthorizedAccessException)
            {
                //MessageDialog content = new MessageDialog(GetVideoAccessError_str);
                //content.Commands.Add(new UICommand(OpenLocalSetting_str, async (command) =>
                // {
                //     bool result = await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-videos"));
                // }));
                //content.Commands.Add(new UICommand(CloseMessageDialog_str, (command) =>
                //{
                //}));
                //content.DefaultCommandIndex = 0;
                //await content.ShowAsync();
                access_dialog = new AddFoldersAccessDialog();
                access_dialog.ShowDialog();
            }
        }

        private BitmapImage bitmap_image;
        private StorageItemThumbnail thumbNail;
        private async Task<BitmapImage> GetFolderImage(StorageFolder folder)
        {
            if (folder != null)
            {
                const ThumbnailMode thunb_NailMode = ThumbnailMode.SingleItem;
                const uint size = 200;
                using (thumbNail = await folder.GetThumbnailAsync(thunb_NailMode, size))
                {
                    bitmap_image = new BitmapImage();
                    bitmap_image.SetSource(thumbNail);
                }
            }
            return bitmap_image;
        }

        private async void ShowAddFolderContentDialog()
        {
            await addFolder_contentDialog.ShowAsync();
        }

        private void OnTimerTick(object sender, object args)
        {
            HideCursor();
            tmr.Stop();
        }

        private void HideCursor()
        {
            if (main_mediaElement.CurrentState == MediaElementState.Playing)
            {
                Window.Current.CoreWindow.PointerCursor = null;
            }
        }

        private void ShowCursor()
        {
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Hand, 1);
        }

        public void SetProgressTimerTick()
        {
            progress_tmr.Interval = TimeSpan.FromSeconds(0.1);
            progress_tmr.Tick += OnTimerTick_progress;
            progress_tmr.Start();
        }

        private void OnTimerTick_progress(object sender, object args)
        {
            History_Progress.GetHistroyProgress(main_video, main_mediaElement);
            SaveVideoProgressMethod();
            progressBottom_slider.Value = main_mediaElement.Position.TotalSeconds;
        }

        private void SaveVideoProgressMethod()
        {
            foreach (var progress in progresslist.progress_list)
            {
                if (progress.Path == main_video.Video_Title)
                {
                    progress.Value = main_video.History_progress;
                }
            }
            SaveProgressVM.SaveData(progresslist.progress_list, filePath);
        }
        private void Main_mediaElementMediaOpened(object sender, RoutedEventArgs e)
        {
            title_textBlock.Text = main_video.Video_Title;
            if (IsBottomShow == false)
            {
                show_bottom.Stop();
                show_bottom_2.Begin();
                hide_topGrid.Stop();
                show_topGrid.Begin();
                IsBottomShow = true;
                if (listPlay_bool)
                {
                    previous_button.Visibility = Visibility.Visible;
                    next_button.Visibility = Visibility.Visible;
                }
            }
            hideBottom_tmr.Start();
            progressBottom_slider.Visibility = Visibility.Collapsed;
            SetProgressList();
            if (main_mediaElement.CanSeek)
            {
                try
                {
                    var value = progresslist.progress_list.Where(p => p.Path == main_video.Video_Title).Select(p => p.Value).ToList();
                    main_mediaElement.Position = value[0] / 100 * main_video.Duration;
                    progress_slider.Maximum = main_mediaElement.NaturalDuration.TimeSpan.TotalSeconds;
                    progress_slider.Value = main_mediaElement.Position.TotalSeconds;
                    progressBottom_slider.Maximum = main_mediaElement.NaturalDuration.TimeSpan.TotalSeconds;
                    progressBottom_slider.Value = main_mediaElement.Position.TotalSeconds;
                }
                catch
                {
                }
            }
            try
            {
                tracks_count.Hide();
                SetTracksMethod();
            }
            catch
            {
            }
            tracks_count = new TracksCount(audioCount_str, videoCount_str, timedCont_str);
            tracks_count.Show();
            main_mediaElement.Play();
        }

        private void SetTracksMethod()
        {
            if (IsAudioTracksChanged_bool == false)
            {
                audioCount_str = "";
            }
            if (IsVideoTracksChanged_bool == false)
            {
                videoCount_str = "";
            }
            if (IsTimedTracksChanged_bool == false)
            {
                timedCont_str = "";
            }
        }
        private ProgressList progresslist = new ProgressList();
        private void SetProgressList()
        {
            if (File.Exists(filePath))
            {
                progresslist.progress_list = SaveProgressVM.ReadData(filePath);
            }
            else
            {
            }

            foreach (var video in use_video)
            {
                var value = progresslist.progress_list.Where(p => p.Path == video.Video_Title).Select(p => p.Path).ToList();
                if (value.Count == 0)
                {
                    Progress progress = new Progress();
                    progress.Path = video.Video_Title;
                    progresslist.progress_list.Add(progress);
                }
            }
        }

        private void RemoveSelectedVideo()
        {
            remove_VideoList = MultipleRemoveSameCode();
            foreach (var item in remove_VideoList)
            {
                use_video.Remove(item);
                if (search_videoCollection.Contains(item))
                {
                    search_videoCollection.Remove(item);
                }
            }
            SetNotifyContent(use_video.Count.ToString() + " " + videosCount_str, TimeSpan.FromSeconds(2));
        }

        private void ClearSelectedItemProgress()
        {
            allSelect_checkBox.IsChecked = false;
            foreach (var progress_video in use_video)
            {
                if (progress_video.IsSelected)
                {
                    progress_video.History_progress = 0;
                    foreach (var progress in progresslist.progress_list)
                    {
                        if (progress.Path == progress_video.Video_Title)
                        {
                            progress.Value = 0;
                        }
                    }
                }
            }

            SaveProgressVM.SaveData(progresslist.progress_list, filePath);
            content_gridView.SelectedItem = null;
            multiple_listBox.SelectedIndex = -1;
        }

        private async void MultipleRemoveoTo(StorageDeleteOption value)
        {
            remove_VideoList = MultipleRemoveSameCode();
            foreach (var item in remove_VideoList)
            {
                use_video.Remove(item);
                if (search_videoCollection.Contains(item))
                {
                    search_videoCollection.Remove(item);
                }
                if (item != null)
                {
                    StorageFile file = item.VideoFile;
                    try
                    {
                        await file.DeleteAsync(value);
                    }
                    catch (System.IO.FileNotFoundException)
                    {
                    }
                }
            }
            SetNotifyContent(use_video.Count.ToString() + " " + videosCount_str, TimeSpan.FromSeconds(2));
            allSelect_checkBox.IsChecked = false;
        }

        private Video[] MultipleRemoveSameCode()
        {
            var remove_VideoList = new Video[use_video.Count];
            for (int i = 0; i < use_video.Count; i++)
            {
                if (use_video[i].IsSelected)
                {
                    remove_VideoList[i] = use_video[i];
                }
            }
            return remove_VideoList;
        }

        private void ClearProgressListValueSameCode(Video progress_video)
        {
            foreach (var progress in progresslist.progress_list)
            {
                if (progress.Path == progress_video.Video_Title)
                {
                    progress.Value = 0;
                }
            }
            SaveProgressVM.SaveData(progresslist.progress_list, filePath);
        }

        private void VideoList_stackPanle_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            sender_value = (FrameworkElement)sender;
        }

        private void Progress_sliderValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            showTime_textBlock.Text = PlayingTime.GetPlayTime((int)main_mediaElement.NaturalDuration.TimeSpan.TotalSeconds, (int)main_mediaElement.Position.TotalSeconds);
        }

        private void Volume_sliderValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            main_mediaElement.Volume = volume_slider.Value / 100;
        }

        private NotifyShow main_notify;

        private void SetNotifyContent(string content_str, TimeSpan showTime_str)
        {
            try
            {
                main_notify.Hide();
            }
            catch
            {
            }
            main_notify = new NotifyShow(content_str, showTime_str);
            main_notify.Show();
        }
        private void ScyclePlayMode()
        {
            SetNotifyContent(playModeStr_recycle, TimeSpan.FromSeconds(2));
            scylePlay_bool = true;
            listPlay_bool = false;
            defaultPlay_bool = false;
            systemMedia_TransportControls.IsPreviousEnabled = false;
            systemMedia_TransportControls.IsNextEnabled = false;
            previous_button.Visibility = Visibility.Collapsed;
            next_button.Visibility = Visibility.Collapsed;
        }

        private void ListPlayMode()
        {
            SetNotifyContent(playModeStr_list, TimeSpan.FromSeconds(2));
            scylePlay_bool = false;
            listPlay_bool = true;
            defaultPlay_bool = false;
            systemMedia_TransportControls.IsPreviousEnabled = true;
            systemMedia_TransportControls.IsNextEnabled = true;
            previous_button.Visibility = Visibility.Visible;
            next_button.Visibility = Visibility.Visible;
        }

        private void DefaultMode()
        {
            SetNotifyContent(playModeStr_default, TimeSpan.FromSeconds(2));
            scylePlay_bool = false;
            listPlay_bool = false;
            defaultPlay_bool = true;
            systemMedia_TransportControls.IsPreviousEnabled = false;
            systemMedia_TransportControls.IsNextEnabled = false;
            previous_button.Visibility = Visibility.Collapsed;
            next_button.Visibility = Visibility.Collapsed;
        }

        private void Item_0_5Tapped(object sender, TappedRoutedEventArgs e)
        {
            main_mediaElement.PlaybackRate = 0.5;
        }

        private void Item_1_0Tapped(object sender, TappedRoutedEventArgs e)
        {
            main_mediaElement.PlaybackRate = 1.0;
        }

        private void Item_2_0Tapped(object sender, TappedRoutedEventArgs e)
        {
            main_mediaElement.PlaybackRate = 2.0;
        }

        private void Item_2_5Tapped(object sender, TappedRoutedEventArgs e)
        {
            main_mediaElement.PlaybackRate = 2.5;
        }

        private void Item_3_0Tapped(object sender, TappedRoutedEventArgs e)
        {
            main_mediaElement.PlaybackRate = 3.0;
        }

        private void Item_0_1Tapped(object sender, TappedRoutedEventArgs e)
        {
            main_mediaElement.PlaybackRate = 0.1;
        }

        private void Item_0_2Tapped(object sender, TappedRoutedEventArgs e)
        {
            main_mediaElement.PlaybackRate = 0.2;
        }

        private void Item_0_3Tapped(object sender, TappedRoutedEventArgs e)
        {
            main_mediaElement.PlaybackRate = 0.3;
        }

        private void Item_0_4Tapped(object sender, TappedRoutedEventArgs e)
        {
            main_mediaElement.PlaybackRate = 0.4;
        }

        private void Item_1_5Tapped(object sender, TappedRoutedEventArgs e)
        {
            main_mediaElement.PlaybackRate = 1.5;
        }

        private void SetDateTimeMethod()
        {
            #region  设置计时器，显示系统当前时间   
            dateTime_tmr.Interval = TimeSpan.FromSeconds(1);
            dateTime_tmr.Tick += DateTime_OnTimerTick;
            #endregion
        }

        private void DateTime_OnTimerTick(object sender, object args)
        {
            dateTime_textBlock.Text = DateTime.Now.ToString();
        }

        private void ButtonTipText()
        {
            ToolTipService.SetToolTip(playRate_button, playRate_str);
            ToolTipService.SetToolTip(audioLanguage_button, audioLanguage_str);
            ToolTipService.SetToolTip(fillMode_button, fillMode_str);
            ToolTipService.SetToolTip(videoTracks_button, mediaTrack_str);
            ToolTipService.SetToolTip(metadata_button, metadata_str);
            ToolTipService.SetToolTip(Back_button, back_str);
            ToolTipService.SetToolTip(volume_button, volume_str);
            ToolTipService.SetToolTip(rewind_button, rewind_str);
            ToolTipService.SetToolTip(fastForward_button, fastForward_str);
            ToolTipService.SetToolTip(minSize_button, pin_str);
            ToolTipService.SetToolTip(more_button, more_str);
            ToolTipService.SetToolTip(screenShot_button, screenShot_str);
            //forcedDecoding_contentDialog.PrimaryButtonText = primaryButton_str;
            //forcedDecoding_contentDialog.CloseButtonText = closeButton_str;
            comment_contentDialog.CloseButtonText = commentCancel_str;
            comment_contentDialog.PrimaryButtonText = commentPrimary_str;
        }
        private string audioLanguage_str;
        private string fillMode_str;
        private string mediaTrack_str;
        private string metadata_str;
        private string noPicture_str;
        private string noPicturesFolder_str;
        private string saveSucceed_str;
        private string unknowError_str;
        private string multiple_str;
        private string cancelMultiple_str;
        private string contentGridViewHeader_str;
        private string videosCount_str;
        private string back_str;
        private string volume_str;
        private string rewind_str;
        private string fastForward_str;
        private string defaultTracks_str;
        private string pin_str;
        private string unPin_str;
        //private string primaryButton_str;
        //private string closeButton_str;
        private string more_str;
        private string forcedDecoding_str;
        private string commentCancel_str;
        private string commentPrimary_str;
        private string screenShot_str;
        private void ReadLocalStr()
        {
            playModeStr_default = resourceLoader.GetString("playModeStr_default");
            playModeStr_recycle = resourceLoader.GetString("playModeStr_recycle");
            playModeStr_list = resourceLoader.GetString("playModeStr_list");
            playModeStr_listRecycle = resourceLoader.GetString("playModeStr_listRecycle");
            playRate_str = resourceLoader.GetString("playRate_str");
            audioLanguage_str = resourceLoader.GetString("audioLanguage_str");
            fillMode_str = resourceLoader.GetString("fillMode_str");
            mediaTrack_str = resourceLoader.GetString("mediaTrack_str");
            metadata_str = resourceLoader.GetString("metadata_str");
            noPicture_str = resourceLoader.GetString("noPicture_str");
            noPicturesFolder_str = resourceLoader.GetString("noPicturesFolder_str");
            saveSucceed_str = resourceLoader.GetString("saveSucceed_str");
            unknowError_str = resourceLoader.GetString("unknowError_str");
            multiple_str = resourceLoader.GetString("multiple_str");
            cancelMultiple_str = resourceLoader.GetString("cancelMultiple_str");
            contentGridViewHeader_str = resourceLoader.GetString("contentGridViewHeader_str");
            videosCount_str = resourceLoader.GetString("videosCount_str");
            back_str = resourceLoader.GetString("back_str");
            volume_str = resourceLoader.GetString("volume_str");
            rewind_str = resourceLoader.GetString("rewind_str");
            fastForward_str = resourceLoader.GetString("fastForward_str");
            defaultTracks_str = resourceLoader.GetString("defaultTracks_str");
            pin_str = resourceLoader.GetString("pin_str");
            unPin_str = resourceLoader.GetString("unPin_str");
            //primaryButton_str = resourceLoader.GetString("primaryButton_str");
            //closeButton_str = resourceLoader.GetString("closeButton_str");
            more_str = resourceLoader.GetString("more_str");
            forcedDecoding_str = resourceLoader.GetString("forcedDecoding_str");
            commentCancel_str = resourceLoader.GetString("commentCancel_str");
            commentPrimary_str = resourceLoader.GetString("commentPrimary_str");
            screenShot_str = resourceLoader.GetString("screenShot_str");
        }

        private void Click_gridPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            show_bottom_2.Begin();
            IsBottomShow = true;
            show_bottom.Stop();
            hideBottom_tmr.Stop();
            progressBottom_slider.Visibility = Visibility.Collapsed;

            hide_topGrid.Stop();
            show_topGrid.Begin();
            if (listPlay_bool)
            {
                previous_button.Visibility = Visibility.Visible;
                next_button.Visibility = Visibility.Visible;
            }
        }

        private void SetAutoHideBottomMethod()
        {
            #region  设置计时器，自动隐藏bottom   
            hideBottom_tmr.Interval = TimeSpan.FromSeconds(5);
            hideBottom_tmr.Tick += AutoHideBottom_OnTimerTick;
            #endregion
        }

        private void AutoHideBottom_OnTimerTick(object sender, object args)
        {
            show_bottom.Begin();
            IsBottomShow = false;
            show_bottom_2.Stop();
            hideBottom_tmr.Stop();
            progressBottom_slider.Visibility = Visibility.Visible;
            show_topGrid.Stop();
            hide_topGrid.Begin();
        }

        private void Bottom_gridPointerExited(object sender, PointerRoutedEventArgs e)
        {
            hideBottom_tmr.Start();
        }
        private List<String> audioLanguage_list = new List<string>();

        private StorageLibrary myVideos;

        private void Bottom_gridPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            hideBottom_tmr.Stop();
            ShowCursor();
        }

        private void Content_gridViewItemClick(object sender, ItemClickEventArgs e)
        {
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            coreTitleBar.ExtendViewIntoTitleBar = true;
            Window.Current.SetTitleBar(topTitleBar_grid);
            forceDecodeVideo_bool = false;
            view_grid.Visibility = Visibility.Collapsed;
            main_grid.Visibility = Visibility.Visible;
            progress_tmr.Stop();
            //main_mediaElement.AutoPlay = true;
            var value = e.ClickedItem;
            main_video = value as Video;
            SetSelectedVideoSameCode();
        }

        private async void MediaPlaybackItem_VideoTracksChanged(MediaPlaybackItem sender, IVectorChangedEventArgs args)
        {
            IsVideoTracksChanged_bool = true;
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                videoTracks_flyout.Items.Clear();
                for (int index = 0; index < sender.VideoTracks.Count; index++)
                {
                    var videoTrack = sender.VideoTracks[index];
                    MenuFlyoutItem flyout_item = new MenuFlyoutItem()
                    {
                        Text = String.IsNullOrEmpty(videoTrack.Language) ? defaultTracks_str : videoTrack.Name + "(" + videoTrack.Language + ")",
                        Tag = index
                    };
                    flyout_item.Click += new RoutedEventHandler(MenuFlyoutVideoItemClick);
                    videoTracks_flyout.Items.Add(flyout_item);
                }
                videoCount_str = sender.VideoTracks.Count.ToString();
            });
        }

        private async void PlaybackItem_AudioTracksChanged(MediaPlaybackItem sender, IVectorChangedEventArgs args)
        {
            IsAudioTracksChanged_bool = true;
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                audioLanguage_flyout.Items.Clear();
                for (int index = 0; index < sender.AudioTracks.Count; index++)
                {
                    var audioTrack = sender.AudioTracks[index];
                    MenuFlyoutItem flyout_item = new MenuFlyoutItem()
                    {
                        Text = String.IsNullOrEmpty(audioTrack.Language) ? defaultTracks_str : audioTrack.Name + "(" + audioTrack.Language + ")",
                        Tag = index
                    };
                    flyout_item.Click += new RoutedEventHandler(MenuFlyoutAudioItemClick);
                    audioLanguage_flyout.Items.Add(flyout_item);
                }
                audioCount_str = sender.AudioTracks.Count.ToString();
            });
        }

        private async void MediaPlaybackItem_TimedMetadataTracksChanged(MediaPlaybackItem sender, IVectorChangedEventArgs args)
        {
            IsTimedTracksChanged_bool = true;
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                metadata_StackPanel.Children.Clear();
                try
                {
                    mediaitem.TimedMetadataTracks.SetPresentationMode(0,
      TimedMetadataTrackPresentationMode.Disabled);
                }
                catch
                {
                }

                for (int index = 0; index < sender.TimedMetadataTracks.Count; index++)
                {
                    var timedMetadataTrack = sender.TimedMetadataTracks[index];
                    ToggleButton toggle = new ToggleButton();
                    if (timedTextFile_name != "")
                    {
                        toggle.Content = timedTextFile_name;
                        //timedTextFile_name = "";
                    }
                    else
                    {
                        toggle.Content = String.IsNullOrEmpty(timedMetadataTrack.Language) ? $"Track {index}" : timedMetadataTrack.Name + " " + timedMetadataTrack.Language;
                    }
                    toggle.Tag = (uint)index;
                    toggle.Checked += Toggle_Checked;
                    toggle.Unchecked += Toggle_Unchecked;
                    metadata_StackPanel.Children.Add(toggle);
                    if (index == tag_index)
                    {
                        toggle.IsChecked = true;
                    }
                }
                timedCont_str = sender.TimedMetadataTracks.Count.ToString();
            });
        }

        private void MenuFlyoutAudioItemClick(object sender, RoutedEventArgs e)
        {
            int trackIndex = (int)(((MenuFlyoutItem)sender).Tag);
            mediaitem.AudioTracks.SelectedIndex = trackIndex;
        }

        private void MenuFlyoutVideoItemClick(object sender, RoutedEventArgs e)
        {
            int trackIndex = (int)(((MenuFlyoutItem)sender).Tag);
            mediaitem.VideoTracks.SelectedIndex = trackIndex;
        }

        private int? tag_index = null;
        private void Toggle_Checked(object sender, RoutedEventArgs e)
        {
            try//偶尔出现 checked 崩溃，使用捕获异常语句来防止闪退
            {
                mediaitem.TimedMetadataTracks.SetPresentationMode((uint)((ToggleButton)sender).Tag,
       TimedMetadataTrackPresentationMode.PlatformPresented);
                tag_index = (int)((uint)((ToggleButton)sender).Tag);
            }
            catch
            {
            }
        }


        private void Toggle_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                mediaitem.TimedMetadataTracks.SetPresentationMode((uint)((ToggleButton)sender).Tag,
       TimedMetadataTrackPresentationMode.Disabled);
                tag_index = null;

            }
            catch
            {
            }
        }

        private void Search_autoSuggestBoxTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            var list = use_video.Where(p => p.Video_Title == search_autoSuggestBox.Text).Select(p => p.Video_Title).ToList();
            if (list.Count == 0)
            {
                if (!String.IsNullOrEmpty(search_autoSuggestBox.Text.Trim()))
                {
                    value = use_video.Where(p => p.Video_Title.Contains(sender.Text.ToUpper())).Select(p => p.Video_Title).ToList();

                    if (value.Count == 0)
                    {
                        searchResult_str = resourceLoader.GetString("searchResult_str");
                        value.Add(searchResult_str);
                    }
                }
                else
                {
                    content_gridView.ItemsSource = use_video;
                }
            }
            search_autoSuggestBox.ItemsSource = value;
        }

        private void Search_autoSuggestBoxQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (!String.IsNullOrEmpty(search_autoSuggestBox.Text.Trim()))
            {
                search_videoCollection.Clear();
                var search_videoList = new List<Video>();
                foreach (var item in use_video)
                {
                    search_videoList.Add(item);
                }
                var value_list = search_videoList.Where(p => p.Video_Title == search_autoSuggestBox.Text).ToList();
                value_list.ForEach(p => search_videoCollection.Add(p));
                content_gridView.ItemsSource = search_videoCollection;
            }
            else
            {
                content_gridView.ItemsSource = use_video;
            }
        }

        private StorageFile cutScreenFile;

        //private void Replay_menuItem_Click(object sender, RoutedEventArgs e)
        //{
        //    if (content_progressRing.IsActive)
        //    {
        //        SetWaitContentDialog();
        //    }
        //    else
        //    {
        //        view_grid.Visibility = Visibility.Collapsed;
        //        main_grid.Visibility = Visibility.Visible;
        //        progress_tmr.Stop();
        //        #region 右键选项点击播放
        //        GetCurrentTheme();
        //        main_mediaElement.AutoPlay = true;
        //        main_video = (Video)sender_value.DataContext;
        //        foreach (var progress in progresslist.progress_list)
        //        {
        //            if (progress.Path == main_video.Video_Path)
        //            {
        //                progress.Value = 0;
        //            }
        //        }
        //        SaveProgressVM.SaveData(progresslist.progress_list, filePath);

        //        SetSelectedVideoSameCode();
        //        #endregion
        //    }
        //}

        private aboutContentDialog about;

        private void SystemSound_toggleSwitchToggled(object sender, RoutedEventArgs e)
        {
            if (SystemSound_toggleSwitch.IsOn)
            {
                ElementSoundPlayer.State = ElementSoundPlayerState.On;
                local_systemSound.Values["IsSoundOn"] = "On";
            }
            else
            {
                ElementSoundPlayer.State = ElementSoundPlayerState.Off;
                local_systemSound.Values["IsSoundOn"] = "Off";
            }
        }

        private void SoundVolume_sliderValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            var sound_volume = soundVolume_slider.Value;
            ElementSoundPlayer.Volume = sound_volume / 100;
            local_systemSoundVolume.Values["Volume_setting"] = sound_volume / 100;
        }

        private void Show_bottomCompleted(object sender, object e)
        {
            progressBottom_slider.Visibility = Visibility.Visible;
            previous_button.Visibility = Visibility.Collapsed;
            next_button.Visibility = Visibility.Collapsed;
        }

        private void Main_mediaElementMediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            //SetNotifyContent(mediaFailed_str);
            //var result = await forcedDecoding_contentDialog.ShowAsync();
            //if (result == ContentDialogResult.Primary)
            //{
            SetNotifyContent(forcedDecoding_str, TimeSpan.FromSeconds(3));
            forceDecodeVideo_bool = true;
            SetMediaItemMethod();
            //}
            //MessageDialog message = new MessageDialog("播放错误，是否强制解码");
            //message.Commands.Add(new UICommand("是", (command) =>
            //{
            //    forceDecodeVideo_bool = true;
            //     SetMediaItemMethod();
            //}));
            //message.Commands.Add(new UICommand("否", (command) =>
            //{
            //}));
            //message.DefaultCommandIndex = 1;
            //await message.ShowAsync();
        }

        private void TopTitle_grid_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            hideBottom_tmr.Stop();
        }

        private void TopTitle_grid_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            hideBottom_tmr.Start();
        }

        private StorageFolder folder_item;
        private async void Folders_gridViewItemClick(object sender, ItemClickEventArgs e)
        {
            allSelect_checkBox.IsChecked = false;
            content_gridView.ItemsSource = use_video;
            var value = e.ClickedItem as VideoLibrary;
            var path = value.FolderPath.ToString();
            foreach (var folder in myPictureFolders)
            {
                if (folder.Path == path)
                {
                    folder_item = folder;
                }
            }
            await GetLocalVideo(folder_item);
            contentGridViewHeader_textBlock.Text = folder_item.DisplayName;
            SetNotifyContent(use_video.Count.ToString() + " " + videosCount_str, TimeSpan.FromSeconds(2));
        }

        private void Folders_stackPanel_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            sender_value = (FrameworkElement)sender;
        }

        private void RemoveFolders_item_Click(object sender, RoutedEventArgs e)
        {
            RemoveFolderFromLibraryCode();
        }

        private bool result_bool = false;
        private async void RemoveFolderFromLibraryCode()
        {
            var remove_library = (VideoLibrary)sender_value.DataContext;
            foreach (var folder in myVideos.Folders)
            {

                if (folder.Path == remove_library.FolderPath)
                {
                    result_bool = await myVideos.RequestRemoveFolderAsync(folder);
                }
            }
            if (result_bool)
            {
                for (int i = 0; i < videoLibrary_collection.Count; i++)
                {
                    if (videoLibrary_collection[i] == remove_library)
                    {
                        videoLibrary_collection.Remove(videoLibrary_collection[i]);
                    }
                }
                all_video.Clear();
                use_video.Clear();
                await GetVideoLibrary();
            }
        }

        private bool lightTheme_bool = false;
        private bool darkTheme_bool = true;
        private bool blueTheme_bool = false;
        private bool yellowTheme_bool = false;
        private bool pinkTheme_bool = false;
        private void Theme_buttonTapped(object sender, TappedRoutedEventArgs e)
        {
            SetThemeMethod();
        }

        private SolidColorBrush paleGoldenrod = new SolidColorBrush(Colors.PaleGoldenrod);
        private void SetThemeMethod()
        {
            if (lightTheme_bool)
            {
                lightTheme_bool = false;
                darkTheme_bool = true;
                PageTheme(ElementTheme.Dark, Colors.Black, black);
                local_theme.Values["theme"] = "dark";
            }
            else if (darkTheme_bool)
            {
                darkTheme_bool = false;
                blueTheme_bool = true;
                PageTheme(ElementTheme.Default, Colors.SkyBlue, skyblue);
                local_theme.Values["theme"] = "blue";
            }
            else if (blueTheme_bool)
            {
                yellowTheme_bool = true;
                blueTheme_bool = false;
                PageTheme(ElementTheme.Default, Colors.PaleGoldenrod, paleGoldenrod);
                local_theme.Values["theme"] = "yellow";
            }
            else if (yellowTheme_bool)
            {
                yellowTheme_bool = false;
                pinkTheme_bool = true;
                PageTheme(ElementTheme.Default, Colors.LightPink, lightPink);
                local_theme.Values["theme"] = "pink";
            }
            else if (pinkTheme_bool)
            {
                pinkTheme_bool = false;
                lightTheme_bool = true;
                PageTheme(ElementTheme.Light, Colors.WhiteSmoke, whiteSmoke);
                local_theme.Values["theme"] = "light";
            }
        }

        private void GetLocalTheme()
        {
            try
            {
                string localTheme_str = local_theme.Values["theme"].ToString();
                darkTheme_bool = false;
                switch (localTheme_str)
                {
                    case "light": PageTheme(ElementTheme.Light, Colors.WhiteSmoke, whiteSmoke); lightTheme_bool = true; break;
                    case "dark": PageTheme(ElementTheme.Dark, Colors.Black, black); darkTheme_bool = true; break;
                    case "blue": PageTheme(ElementTheme.Default, Colors.SkyBlue, skyblue); blueTheme_bool = true; break;
                    case "yellow": PageTheme(ElementTheme.Default, Colors.PaleGoldenrod, paleGoldenrod); yellowTheme_bool = true; break;
                    case "pink": PageTheme(ElementTheme.Default, Colors.LightPink, lightPink); pinkTheme_bool = true; break;
                    default:
                        break;
                }
            }
            catch
            {
                PageTheme(ElementTheme.Dark, Colors.Black, black);
            }

        }

        private AcrylicBrush main_brush = new AcrylicBrush();
        private void PageTheme(ElementTheme value, Color color, SolidColorBrush solid_color)
        {
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.Xaml.Media.XamlCompositionBrushBase"))
            {
                main_brush.BackgroundSource = AcrylicBackgroundSource.HostBackdrop;
                main_brush.TintOpacity = 0.7;
                main_brush.TintColor = color;
                main_brush.FallbackColor = color;
                this.Background = main_brush;
                multiple_grid.Background = main_brush;
            }
            else
            {
                this.Background = solid_color;
                multiple_grid.Background = solid_color;
            }
            this.RequestedTheme = value;
            search_autoSuggestBox.RequestedTheme = value;
            switch (value)
            {
                case ElementTheme.Default:
                    bottom_border.Background = white; top_border.Background = white;
                    break;
                case ElementTheme.Light:
                    bottom_border.Background = white; top_border.Background = white;
                    break;
                case ElementTheme.Dark:
                    bottom_border.Background = black; top_border.Background = black;
                    break;
                default:
                    break;
            }
        }

        private void Play_buttonTapped(object sender, TappedRoutedEventArgs e)
        {
            main_mediaElement.AutoPlay = true;
            if (main_mediaElement.CurrentState == MediaElementState.Playing)
            {
                main_mediaElement.Pause();
            }
            else
            {
                main_mediaElement.Play();
            }
        }

        private void Next_buttonTapped(object sender, TappedRoutedEventArgs e)
        {
            SameCodeMethod();
        }

        private void Previous_buttonTapped(object sender, TappedRoutedEventArgs e)
        {
            PreviousPlaySameMethod();
        }

        private void FullScreen_buttonTapped(object sender, TappedRoutedEventArgs e)
        {
            SetFullScreenMethod();
        }

        private async void AddFolder_buttonTapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                Windows.Storage.StorageFolder newFolder = await myVideos.RequestAddFolderAsync();
                var value = videoLibrary_collection.Where(p => p.FolderPath == newFolder.Path).Select(p => p).ToList();
                if (value.Count == 0)
                {
                    VideoLibrary library = new VideoLibrary();
                    library.FolderName = newFolder.DisplayName;
                    library.FolderPath = newFolder.Path;
                    library.FolderImage = await GetFolderImage(newFolder);
                    videoLibrary_collection.Add(library);
                    await GetVideoLibrary();
                }
            }
            catch
            {
            }
        }

        private void AllSelect_checkBoxTapped(object sender, TappedRoutedEventArgs e)
        {
            if (allSelect_checkBox.IsChecked == true)
            {
                foreach (var video in use_video)
                {
                    video.IsSelected = true;
                }
                content_gridView.SelectAll();
            }
            else
            {
                foreach (var video in use_video)
                {
                    video.IsSelected = false;
                }
                content_gridView.SelectedItem = null;
            }
            multiple_listBox.SelectedIndex = -1;
        }

        private async void Settings_buttonTapped(object sender, TappedRoutedEventArgs e)
        {
            ReadLocalDataSize.GetLocalDataMethod(data_textBlock);
            await settings_contentDialog.ShowAsync();
        }

        private async void OpenCutAdress_buttonTapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                bool result = await Windows.System.Launcher.LaunchFolderAsync(await KnownFolders.PicturesLibrary.GetFolderAsync("M-Player"));
            }
            catch
            {
                curScreen_textBlock.Text = noPicturesFolder_str;
            }
        }

        private async void EditPicture_buttonTapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                bool result = await Windows.System.Launcher.LaunchFileAsync(cutScreenFile);
            }
            catch
            {
                curScreen_textBlock.Text = noPicture_str;
            }
        }

        private void CloseCutDialog_buttonTapped(object sender, TappedRoutedEventArgs e)
        {
            cutScreen_contentDialog.Hide();
        }

        private void Fill_itemTapped(object sender, TappedRoutedEventArgs e)
        {
            main_mediaElement.Stretch = Stretch.Fill;
        }

        private void UniformToFill_itemTapped(object sender, TappedRoutedEventArgs e)
        {
            main_mediaElement.Stretch = Stretch.UniformToFill;
        }

        private void None_itemTapped(object sender, TappedRoutedEventArgs e)
        {
            main_mediaElement.Stretch = Stretch.None;
        }

        private void Uniform_itemTapped(object sender, TappedRoutedEventArgs e)
        {
            main_mediaElement.Stretch = Stretch.Uniform;
        }

        private void CloseDialog_ButtonTapped(object sender, TappedRoutedEventArgs e)
        {
            addFolder_contentDialog.Hide();
        }

        private async void RefreshVideos_buttonTapped(object sender, TappedRoutedEventArgs e)
        {
            await GetVideoLibrary();
            content_gridView.ItemsSource = use_video;
        }

        private void AddFolders_buttonTapped(object sender, TappedRoutedEventArgs e)
        {
            ShowAddFolderContentDialog();
        }

        private async void Email_buttonTapped(object sender, TappedRoutedEventArgs e)
        {
            await SetFeedBackClass.ComposeEmail(recipient);
        }

        private void About_buttonTapped(object sender, TappedRoutedEventArgs e)
        {
            settings_contentDialog.Hide();
            about = new aboutContentDialog();
            about.Show();
        }

        private async void Comment_buttonTapped(object sender, TappedRoutedEventArgs e)
        {
            //await SetFeedBackClass.ShowRatingReviewDialog();
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store://review/?ProductId=9N6JVKJH4FFL"));
        }

        private void ClearData_buttonTapped(object sender, TappedRoutedEventArgs e)
        {
            ReadLocalDataSize.ClearData(data_textBlock);
            //foreach (var item in progresslist.progress_list)
            //{
            //    item.Value = 0;
            //}
            progresslist.progress_list.Clear();
            foreach (var video in use_video)
            {
                video.History_progress = 0;
                var progress_item = new Progress();
                progress_item.Path = video.Video_Title;
                progresslist.progress_list.Add(progress_item);
            }
        }

        private void CloseSettings_buttonTapped(object sender, TappedRoutedEventArgs e)
        {
            settings_contentDialog.Hide();
        }

        private async void Back_buttonTapped(object sender, TappedRoutedEventArgs e)
        {
            if (ApplicationView.GetForCurrentView().IsFullScreenMode)
            {
                ContentDialogResult result = await exitFullScreen_contentDialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    dateTime_textBlock.Visibility = Visibility.Collapsed;
                    dateTime_tmr.Stop();
                    ApplicationView.GetForCurrentView().ExitFullScreenMode();
                    fullScreen_button.Content = "\uE740";
                }                
            }
            coreTitleBar.ExtendViewIntoTitleBar = false;
            titleBar.ButtonBackgroundColor = Colors.Black;
            titleBar.ButtonInactiveBackgroundColor = Colors.Black;
            try
            {
                main_notify.Hide();
            }
            catch
            {
            }
            progress_tmr.Stop();
            main_mediaElement.Stop();
            main_grid.Visibility = Visibility.Collapsed;
            view_grid.Visibility = Visibility.Visible;
            tag_index = null;
        }
        private void Multiple_buttonTapped(object sender, TappedRoutedEventArgs e)
        {
            if (IsMultipleClick_bool)
            {
                multiple_fontIcon.Glyph = "\uE762";
                multiple_textBlock.Text = multiple_str;
                allSelect_checkBox.IsChecked = false;
                hide_multipleGrid.Begin();
                show_multipleGrid.Stop();
                IsMultipleClick_bool = false;
                content_gridView.SelectionMode = ListViewSelectionMode.None;
                multiple_listBox.SelectedIndex = -1;
                content_gridView.IsItemClickEnabled = true;
                foreach (var item in use_video)
                {
                    item.IsSelected = false;
                }
            }
            else
            {
                multiple_fontIcon.Glyph = "\uE711";
                multiple_textBlock.Text = cancelMultiple_str;
                IsMultipleClick_bool = true;
                content_gridView.IsItemClickEnabled = false;
                multiple_dropShadowPanel.Visibility = Visibility.Visible;
                show_multipleGrid.Begin();
                hide_multipleGrid.Stop();
                content_gridView.SelectionMode = ListViewSelectionMode.Multiple;
            }
        }

        private void AddTimedText_buttonTapped(object sender, TappedRoutedEventArgs e)
        {
            SetTimedTextSource(main_mediaSource);
        }

        private void Folders_gridViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var value = sender as GridView;
            value.SelectedItem = null;
        }

        private async void ScreenShot_itemClick(object sender, RoutedEventArgs e)
        {
            cutScreenFile = await CutScreen.GetScreen(main_video, main_mediaElement);
            curScreen_textBlock.Text = $"{saveSucceed_str}  {cutScreenFile.Path}";
            await cutScreen_contentDialog.ShowAsync();
        }

        private void Search_buttonTapped(object sender, TappedRoutedEventArgs e)
        {
            if (search_autoSuggestBox.Visibility == Visibility.Visible)
            {
                search_autoSuggestBox.Visibility = Visibility.Collapsed;
            }
            else
            {
                search_autoSuggestBox.Visibility = Visibility.Visible;
            }
        }

        private void Rewind_buttonTapped(object sender, TappedRoutedEventArgs e)
        {
            main_mediaElement.Position -= TimeSpan.FromSeconds(10);
        }

        private void FastForward_buttonTapped(object sender, TappedRoutedEventArgs e)
        {
            main_mediaElement.Position += TimeSpan.FromSeconds(30);
        }

        private async void OpenTheFolder_button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            sender_value = (FrameworkElement)sender;
            var remove_library = (VideoLibrary)sender_value.DataContext;
            foreach (var folder in myVideos.Folders)
            {
                if (folder.Path == remove_library.FolderPath)
                {
                    bool result = await Windows.System.Launcher.LaunchFolderAsync(folder);
                }
            }
        }

        private void DeleteFolder_button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            sender_value = (FrameworkElement)sender;
            RemoveFolderFromLibraryCode();
        }

        private void Frombin_itemTapped(object sender, TappedRoutedEventArgs e)
        {
            MultipleRemoveoTo(StorageDeleteOption.Default);
        }

        private void Remove_itemTapped(object sender, TappedRoutedEventArgs e)
        {
            MultipleRemoveoTo(StorageDeleteOption.PermanentDelete);
        }

        private void Fromlist_itemTapped(object sender, TappedRoutedEventArgs e)
        {
            RemoveSelectedVideo();
            allSelect_checkBox.IsChecked = false;
        }

        private void Null_itemTapped(object sender, TappedRoutedEventArgs e)
        {
            DefaultMode();
            playMode_textBlock.Visibility = Visibility.Collapsed;
            //playMode_textBlock.Text = playModeStr_default;
            playMode_fontIcon.Glyph = "\uF140";
            playMode_fontIcon.FontSize = 20;
            playMode_fontIcon.Margin = new Thickness(0);
        }

        private void Replay_itemTapped(object sender, TappedRoutedEventArgs e)
        {
            ScyclePlayMode();
            playMode_textBlock.Visibility = Visibility.Collapsed;
            playMode_fontIcon.Glyph = "\uE8ED";
            playMode_fontIcon.FontSize = 20;
            playMode_fontIcon.Margin = new Thickness(0);
        }

        private void List_itemTapped(object sender, TappedRoutedEventArgs e)
        {
            ListPlayMode();
            playMode_textBlock.Visibility = Visibility.Collapsed;
            playMode_fontIcon.Glyph = "\uE8AB";
            playMode_fontIcon.FontSize = 20;
            playMode_fontIcon.Margin = new Thickness(0);
        }

        private async void MinSize_buttonTapped(object sender, TappedRoutedEventArgs e)
        {
            if (ApplicationView.GetForCurrentView().IsViewModeSupported(ApplicationViewMode.CompactOverlay))
            {
                if (ApplicationView.GetForCurrentView().ViewMode == ApplicationViewMode.Default)
                {
                    await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay);
                    minSize_button.Content = "\uE842";
                    ToolTipService.SetToolTip(minSize_button, unPin_str);
                    Back_button.Visibility = Visibility.Collapsed;
                    dateTime_textBlock.Visibility = Visibility.Collapsed;
                    title_textBlock.FontSize = 12;
                }
                else
                {
                    await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.Default);
                    minSize_button.Content = "\uE841";
                    ToolTipService.SetToolTip(minSize_button, pin_str);
                    Back_button.Visibility = Visibility.Visible;
                    title_textBlock.FontSize = 15;
                }
            }
            else
            {
                SetNotifyContent("No Supported!", TimeSpan.FromSeconds(2));
            }
        }

        private void ProgressLisbox_itemTapped(object sender, TappedRoutedEventArgs e)
        {
            ClearSelectedItemProgress();
        }

        private void Main_borerPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            ShowCursor();
            tmr.Start();
        }

        private void Main_borerTapped(object sender, TappedRoutedEventArgs e)
        {
            if (IsBottomShow)
            {
                show_bottom.Begin();
                show_bottom_2.Stop();
                hideBottom_tmr.Stop();
                IsBottomShow = false;
                show_topGrid.Stop();
                hide_topGrid.Begin();
            }
            else
            {
                show_bottom.Stop();
                show_bottom_2.Begin();
                hideBottom_tmr.Start();
                IsBottomShow = true;
                progressBottom_slider.Visibility = Visibility.Collapsed;
                if (listPlay_bool)
                {
                    previous_button.Visibility = Visibility.Visible;
                    next_button.Visibility = Visibility.Visible;
                }
                hide_topGrid.Stop();
                show_topGrid.Begin();
            }
        }

        private void Main_borerDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            SetFullScreenMethod();
        }

        private void Main_borerPointerExited(object sender, PointerRoutedEventArgs e)
        {
            tmr.Stop();
            ShowCursor();
        }

        private void PageSizeChanged(object sender, SizeChangedEventArgs e)
        {
            progressBottom_slider.Width = this.Width;
            topTitleBar_grid.Width = this.Width;
        }

        private void Progress_sliderDrop(object sender, DragEventArgs e)
        {
            main_mediaElement.IsMuted = true;
        }

        private void Progress_sliderDropCompleted(UIElement sender, DropCompletedEventArgs args)
        {
            main_mediaElement.IsMuted = false;
        }

        private void Show_topGridCompleted(object sender, object e)
        {
            Window.Current.SetTitleBar(topTitleBar_grid);
        }

        private void Hide_topGridCompleted(object sender, object e)
        {
            Window.Current.SetTitleBar(topTitleBar_border);
        }

        private void IsComment_checkBox_Checked(object sender, RoutedEventArgs e)
        {
            if (isComment_checkBox.IsChecked == true)
            {
                IsCommentShow_bool = true;
                commentDialog_bool.Values["comment_bool"] = true;
            }
        }

        private async void SetCommentMethod()
        {
            if (IsCommentShow_bool == false)
            {
                var random = new Random();
                int number = random.Next(0, 9);
                if (number == 7)
                {
                    ShowCursor();
                    tmr.Stop();
                    ContentDialogResult result = await comment_contentDialog.ShowAsync();
                    if (result == ContentDialogResult.Primary)
                    {
                        IsCommentShow_bool = true;
                        commentDialog_bool.Values["comment_bool"] = true;
                        await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store://review/?ProductId=9P91BCBX669K"));
                    }
                }
            }
        }
    }
}
