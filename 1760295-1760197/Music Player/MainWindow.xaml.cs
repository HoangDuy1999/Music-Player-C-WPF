using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Gma.System.MouseKeyHook;
using System.Windows.Forms;
using Application = System.Windows.Forms.Application;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace Music_Player
{
    // class để lưu playlist
    public class playlist
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public partial class MainWindow : Window
    {
        DispatcherTimer timer = new DispatcherTimer();
        private MediaPlayer mediaPlayer = new MediaPlayer();
        //int bai hat được chạy
        public static int music_song = 0;
        // check hàm pause
        public static bool check_pause = false;
        // check ham play
        public static bool _check_play = false;
        // luu duong dan bai hat
        public static ArrayList _PathPlayList = new ArrayList();
        // Tạo ra danh sách bài hát để binding vô listbox
        public static BindingList<playlist> _Playlist = new BindingList<playlist>();
        // biến chạy slider_bar
        public static int dem = 0;
        // Cho biết có đang kéo thả slider
        public static bool is_Draping = false;
        // Cho biết chễ đố nhảy bảy hát
        public static bool _is_Random = false;
        // Cho biết bài hát cuối playlist
        public static bool music_end = false;
        // Cho biết bài hát đầu playlist
        public static bool music_start = false;
        //Cho biết chế độ lặp playlist
        public static bool _isRepeatPlaylist = false;
        // cho biết chạy lặp lại playlist 1 lần
        public static int lap1lan = 0;
        public MainWindow()
        {
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
        }
        // Hàm hỗ trợ
        void timer_Tick(object sender, EventArgs e)
        {
            if (mediaPlayer.Source != null)
            {
                if (_check_play)
                {
                    try
                    {
                        run_time.Text = mediaPlayer.Position.ToString(@"mm\:ss");
                        sum_time.Text = mediaPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss");
                    }
                    catch { }
                }
                if (check_pause == false)
                {
                    dem++;
                    slider_bar.Maximum = (int)mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds + 1;
                    slider_bar.Value = dem;
                }

                if (dem == (int)mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds + 1)
                {
                    mediaPlayer.Stop();
                    dem = 0;
                    slider_bar.Value = 0;
                    run_time.Text = "00:00";
                    pause.Width = 0;
                    play.Width = 35;
                    check_pause = false;
                    timer.Stop();
                    // Chuyển bài hát
                    Auto_music_continute();
                }
            }
        }
        int check = 0;
        // Hàm chạy tới bài hát kế tiếp tự động
        private void Auto_music_continute()
        {
            // trước khi chuyển thì phải chuyển thanh backgroud sang bài tiếp
            var listBoxItem = List_box_play_list.ItemContainerGenerator.ContainerFromIndex(music_song);
            (listBoxItem as ListBoxItem).Background = Brushes.White;
            // Nếu lặp vô tận
            if (_isRepeatPlaylist == true)
            {
                if (music_song + 1 == _Playlist.Count)
                {
                    music_song = 0;
                }
                else
                {
                    if(_is_Random == true)
                    {
                        Random rd = new Random();
                        music_song = rd.Next(music_song + 1, _PathPlayList.Count);
                    }
                    else
                        music_song++;
                }
                check = 1;
                Play_one_music();
            }
            // Nếu lặp 1 lần
            if (_isRepeatPlaylist == false && lap1lan < 2)
            {
                if (music_song + 1 == _Playlist.Count)
                {
                    music_song = 0;
                    string[] temp = _PathPlayList[music_song].ToString().Split('\\');
                    Bai_hat.Content = temp[temp.Count() - 1];
                    // tô đạm bài hát được chọn
                    listBoxItem = List_box_play_list.ItemContainerGenerator.ContainerFromIndex(music_song);
                    var bc = new BrushConverter();
                    (listBoxItem as ListBoxItem).Background = (Brush)bc.ConvertFrom("#3399FF");
                    lap1lan = lap1lan + 1;
                }
                else
                {
                    if (_is_Random == true)
                    {
                        Random rd = new Random();
                        music_song = rd.Next(music_song + 1, _PathPlayList.Count);
                    }
                    else
                        music_song++;
                }
                if (lap1lan == 2)
                {
                    check = 0;
                }
                else
                {
                    check = 1;
                    Play_one_music();
                }
            }
        }
        //Hàm thu nhỏ màn hình
        private void Minimize_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        // Hàm tắt máy nghe nhạc
        private void Turn_off_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Bạn có muốn tắt máy nghe nhạc?", "Exit", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Environment.Exit(0);
            }
        }
        //Hàm  Kiểm tra bài hát trùng
        private bool Check_music_song_repeat(string ten_bh)
        {
            for(int i  = 0; i < _PathPlayList.Count; i++)
            {
                string[] arr = _PathPlayList[i].ToString().Split('\\');
                if (arr[arr.Count() - 1].Trim() == ten_bh.Trim())
                    return false;
            }
            return true;
        }
        // Hàm thêm bài hát
        private void Add_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var screen = new OpenFileDialog();
            var filename = "";
            screen.Filter = "Music (.mp3)|*.mp3|ALL Files (*.*)|*.*";
            if (screen.ShowDialog() == true)
            {
                filename = screen.FileName;
                // Phân giải đường dẫn lấy ra tên bài hát
                string[] arr = filename.Split('\\');
                // Hàm kiểm tra bài hát trùng
                if (Check_music_song_repeat(arr[arr.Count() - 1]) == true)
                {
                    _PathPlayList.Add(filename);
                    _Playlist.Add(new playlist() { Id = _PathPlayList.Count, Name = arr[arr.Count() - 1].ToString() });
                    List_box_play_list.ItemsSource = _Playlist;
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show("Bài hát đã tồn tại trong playlist rồi.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }    
        }
        // Hàm trợ giúp người dùng
        private void Helpme_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("1. Muốn thêm bài hát vô playlist thì ấn vô dấu cộng trên nằm trên thanh header.\n" +
                "2. Muốn thu nhỏ máy nghe nhạc ấn vô biểu tượng _ nhé.\n" +
                "3. Muốn tắt máy nghe nhạc ấn vô biểu tượng nút nguồn.\n" +
                "4. Ngoài nút play có thể chạy bài hát theo ý muốn bằng cách double click chuột trái vô bài hát đó.\n" +
                "5. Muốn xóa một bài hát thì click chuột trái vào bài hát muốn xóa sau đó ấn vào nút delete.\n" +
                "6. Muốn xóa nguyên playlist thì ấn vô nút delete all.\n" +
                "7. Ấn vô nút save để save playlist lại.\n" +
                "8. Muốn chạy playlist đã lưu thì ấn vô nút open.\n" +
                "9. ctrl + shift + P: play music.\n" +
                "10. ctrl + shift + Space: pause music.\n" +
                "11. ctrl + shift + S: Stop music.\n" +
                "12. ctrl + shift + T: Next music.\n" +
                "13. ctrl + shift + B: Previous music.\n", "Help", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        // THANH ĐIỀU KHIỂN NHẠC
        // Hàm play 1 bài hat
        private void Play_one_music()
        {
            if (_PathPlayList.Count > 0)
            {
                play.Width = 0;
                pause.Width = 35;
                if (mediaPlayer.Source == null || check == 1)
                {
                    mediaPlayer.Open(new Uri(_PathPlayList[music_song].ToString()));
                }
                Bai_hat.Width = 280;
                string[] temp = _PathPlayList[music_song].ToString().Split('\\');
                Bai_hat.Content = temp[temp.Count() - 1];
                mediaPlayer.Play();
                if (dem == 0)
                {
                    timer.Start();
                }
                // tô đạm bài hát được chọn
                var listBoxItem = List_box_play_list.ItemContainerGenerator.ContainerFromIndex(music_song);
                var bc = new BrushConverter();
                (listBoxItem as ListBoxItem).Background = (Brush)bc.ConvertFrom("#3399FF");
                check = 0;
                _check_play = true;
                check_pause = false;
            }
        }
        //Play
        private void Play_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Play_one_music();
            lap1lan = 0;
        }
        // Double click vô bài hát để chuyển bài
        private void List_box_play_list_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (List_box_play_list.SelectedItem != null)
            {
                // đổi background về ban đầu
                var listBoxItem = List_box_play_list.ItemContainerGenerator.ContainerFromIndex(music_song);
                var bc = new BrushConverter();
                (listBoxItem as ListBoxItem).Background = Brushes.White;
                // lấy vị trí bài hát được chọn
                music_song = (int)(List_box_play_list.SelectedItem as playlist).Id - 1;
                // tô đậm bài hát được chọn
                listBoxItem = List_box_play_list.ItemContainerGenerator.ContainerFromIndex(music_song);
                (listBoxItem as ListBoxItem).Background = (Brush)bc.ConvertFrom("#3399FF");

                mediaPlayer.Open(new Uri(_PathPlayList[music_song].ToString()));
                mediaPlayer.Play();
                slider_bar.Value = 0;
                slider_bar.Maximum = 0;
                dem = 0;
                if (dem == 0)
                {
                    timer.Start();
                }
                string[] temp = _PathPlayList[music_song].ToString().Split('\\');
                Bai_hat.Content = temp[temp.Count() - 1];
                _check_play = true;
                check_pause = false;
                play.Width = 0;
                pause.Width = 35;
            }
        }
        //Pause
        private void Pause_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_PathPlayList.Count > 0)
            {
                play.Width = 35;
                pause.Width = 0;
                check_pause = true;
                _check_play = false;
                mediaPlayer.Pause();
            }
        }
        //Stop
        private void Stop_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_PathPlayList.Count > 0)
            {
                mediaPlayer.Stop();
                play.Width = 35;
                pause.Width = 0;
                slider_bar.Value = 0;
                slider_bar.Maximum = 0;
                run_time.Text = "00:00";
                sum_time.Text = "00:00";
                timer.Stop();
                dem = 0;
            }
        }
        // Hàm Next Music
        private void Next_Music_song()
        {
            if (music_song == _PathPlayList.Count - 1)
            {
                music_end = true;
            }
            else
            {
                music_end = false;
            }
            if (_PathPlayList.Count > 0 && music_song < _PathPlayList.Count - 1 || _is_Random == true)
            {
                timer.Stop();
                mediaPlayer.Stop();
                dem = 0;
                var listBoxItem = List_box_play_list.ItemContainerGenerator.ContainerFromIndex(music_song);
                var bc = new BrushConverter();
                (listBoxItem as ListBoxItem).Background = Brushes.White;
                if (_is_Random == true)
                {
                    if (music_end == false)
                    {
                        Random rnd = new Random();
                        music_song = rnd.Next(music_song + 1, _PathPlayList.Count);
                    }
                    else
                        music_song = 0;
                }
                if (_is_Random == false && music_end == false)
                {
                    music_song++;// tăng bài hát lên 1
                }
                listBoxItem = List_box_play_list.ItemContainerGenerator.ContainerFromIndex(music_song);
                play.Width = 0;
                pause.Width = 35;
                mediaPlayer.Open(new Uri(_PathPlayList[music_song].ToString()));
                mediaPlayer.Play();
                Bai_hat.Width = 280;
                string[] temp = _PathPlayList[music_song].ToString().Split('\\');
                Bai_hat.Content = temp[temp.Count() - 1];
                timer.Start();
                // tô đạm bài hát được chọn
                (listBoxItem as ListBoxItem).Background = (Brush)bc.ConvertFrom("#3399FF");
                _check_play = true;
                check_pause = false;
            }
        }
        // Nút next
        private void Next_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {   
           if(_Playlist.Count > 0)
           {
                Next_Music_song();
           }
        }
        // Hàm Previous music song
        private void Previous_music_song()
        {
            if (music_song == 0)
            {
                music_start = true;
            }
            else
            {
                music_start = false;
            }
            if (_PathPlayList.Count > 0 && music_song > 0|| _is_Random == true)
            {
                timer.Stop();
                mediaPlayer.Stop();
                dem = 0;
                var listBoxItem = List_box_play_list.ItemContainerGenerator.ContainerFromIndex(music_song);
                var bc = new BrushConverter();
                (listBoxItem as ListBoxItem).Background = Brushes.White;
                if (_is_Random == true)
                {
                    if (music_start == false)
                    {
                        Random rnd = new Random();
                        music_song = rnd.Next(0, music_song);
                    }
                    else
                        music_song = _PathPlayList.Count - 1;
                }
                if (_is_Random == false || music_start == false)
                {
                    music_song--;// giảm bài hát xuống 1
                }
                if (music_song < 0)
                    music_song = 0;
                listBoxItem = List_box_play_list.ItemContainerGenerator.ContainerFromIndex(music_song);
                play.Width = 0;
                pause.Width = 35;
                mediaPlayer.Open(new Uri(_PathPlayList[music_song].ToString()));
                mediaPlayer.Play();
                slider_bar.Value = 0;
                Bai_hat.Width = 280;
                string[] temp = _PathPlayList[music_song].ToString().Split('\\');
                Bai_hat.Content = temp[temp.Count() - 1];
                timer.Start();
                // tô đạm bài hát được chọn
                (listBoxItem as ListBoxItem).Background = (Brush)bc.ConvertFrom("#3399FF");
                _check_play = true;
                check_pause = false;
            }
        }
        // Previous
        private void Previous_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_Playlist.Count > 0)
            {
                Previous_music_song();
            }
        }
        // Random
        private void Random_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            random.Width = 0;
            sequentially.Width = 40;
            sequentially1.Width = 40;
            _is_Random = false;
        }
        // Tuần tự
        private void Sequentially_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _is_Random = true;
            random.Width = 40;
            sequentially.Width = 0;
            sequentially1.Width = 0;
        }
        //  Repeat n
        private void Repeat_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isRepeatPlaylist = false;
            repeat.Width = 0;
            repeat1.Width = 55;
        }
        //  Repeat 1
        private void Repeat1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
           
            _isRepeatPlaylist = true;
            repeat.Width = 55;
            repeat1.Width = 0;
        }

        // FOOTER
        // Open
        OpenFileDialog openFileDialog = new OpenFileDialog();
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            openFileDialog.DefaultExt = ".txt";
            openFileDialog.Filter = "Text Document (.txt)|*.txt";
            if (openFileDialog.ShowDialog() == true)
            {
                string s = File.ReadAllText(openFileDialog.FileName);

                string[] array_open = s.ToString().Split('+');
                if (array_open.Count() > 1)
                {
                    if(dem != 0)
                    {
                        _PathPlayList.Clear();
                        _Playlist.Clear();
                        music_song = 0;
                        run_time.Text = "00:00";
                        sum_time.Text = "00:00";
                        slider_bar.Value = 0;
                        slider_bar.Maximum = 0;
                        mediaPlayer.Stop();
                        timer.Stop();
                        dem = 0;
                        Bai_hat.Content = "";
                        play.Width = 35;
                        pause.Width = 0;
                        check = 1;
                    }
                    _PathPlayList.Clear();
                    _Playlist.Clear();
                    _Playlist.ResetBindings();
                    for (int i = 0; i < array_open.Count() - 1; i++)
                    {
                        _PathPlayList.Add(array_open[i].ToString().Trim());
                        string []arr = array_open[i].ToString().Trim().Split('\\');
                        _Playlist.Add(new playlist() { Id = _PathPlayList.Count, Name = arr[arr.Count() - 1].ToString() });
                    }
                    List_box_play_list.ItemsSource = _Playlist;
                    music_song = int.Parse(array_open[array_open.Count() - 1].ToString().Trim());
                }
                else
                {
                    MessageBox.Show("File nạp playlist không hợp lệ.(^.^)", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        // ghi dự liệu
        private void Write_date_arrlist(ArrayList line)
        {
            for(int i = 0; i < _PathPlayList.Count;i++)
            {
                line.Add(_PathPlayList[i]);
                line.Add("+");
            }
            line.Add(music_song);
        }
        // Save
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            ArrayList line = new ArrayList();
            Write_date_arrlist(line);
            // Set a variable to the Documents path.
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (_PathPlayList.Count > 0)
            {
                string alltext = "";
                for (int i = 0; i < line.Count; i++)
                {
                    alltext += line[i].ToString() + " ";
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Text file (*.txt)|*.txt";
                if (saveFileDialog.ShowDialog() == true)
                {
                    File.WriteAllText(saveFileDialog.FileName, alltext);
                    MessageBox.Show("Lưu playlist thành công.(^.^)", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }

            }
            else
            {
                MessageBox.Show("Không có dự liệu dể lưu.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        // Delete
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (List_box_play_list.SelectedItem != null)
            {
                // lấy vị trí bài hát được chọn
                string ten_xoa = (List_box_play_list.SelectedItem as playlist).Name;
                for (int i = 0; i < _PathPlayList.Count; i++)
                {
                    int temp = 0;
                    if (_Playlist[i].Name == ten_xoa)
                    {
                        temp = 1;
                        if (i != music_song || (i == 0 && _check_play == false))
                        {
                            if (music_song == i && i == 0 && _PathPlayList.Count >  1)
                            {
                                dem = 0;
                                slider_bar.Value = 0;
                                mediaPlayer.Stop();
                                Bai_hat.Content = "";
                                run_time.Text = "00:00";
                                sum_time.Text = "00:00";
                                check = 1;
                            }
                            if (i < music_song)
                            {
                                music_song--;
                            }
                            _PathPlayList.RemoveAt(i);
                            _Playlist.RemoveAt(i);
                            _Playlist.ResetBindings();
                            for (int j = 0; j < _Playlist.Count; j++)
                            {
                                _Playlist[j].Id = j + 1;
                            }
                            if (_PathPlayList.Count == 0)
                            {
                                mediaPlayer.Stop();
                                timer.Stop();
                                check = 1;
                                music_song = 0;
                            }
                            List_box_play_list.ItemsSource = _Playlist;
                        }
                        else
                        {
                            MessageBoxResult result = MessageBox.Show("Không thể xóa bài hát đang chạy được.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    if (temp == 1)
                        break;
                }
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Dữ liệu trống.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        // Delete all
        private void Deleteall_Click(object sender, RoutedEventArgs e)
        {
                if (_PathPlayList.Count > 0)
                {
                    MessageBoxResult result = MessageBox.Show("Bạn có muốn xóa playlist.", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        _PathPlayList.Clear();
                        _Playlist.Clear();
                        music_song = 0;
                        run_time.Text = "00:00";
                        sum_time.Text = "00:00";
                        slider_bar.Value = 0;
                        slider_bar.Maximum = 0;
                        mediaPlayer.Stop();
                        timer.Stop();
                        dem = 0;
                        Bai_hat.Content = "";
                        play.Width = 35;
                        pause.Width = 0;
                        check = 1;
                    }
                }
                else
                {
                    MessageBox.Show("Dữ liệu trống.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
        }
        // Kéo thả slider
        private void Slider_bar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (is_Draping)
            {
                dem = (int)slider_bar.Value;
                mediaPlayer.Position = new TimeSpan(0, 0, dem);
                if (dem == (int)mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds + 1)
                {
                    run_time.Text= (dem / 60).ToString() + ":" + ((dem % 60) - 1).ToString();
                    mediaPlayer.Stop();
                    dem = 0;
                    slider_bar.Value = dem;
                    slider_bar.Maximum = dem;
                    pause.Width = 0;
                    play.Width = 35;
                    check_pause = false;
                    timer.Stop();
                    // Chuyển bài hát
                    Auto_music_continute();
                }
            }
        }
        // Hàm phụ trợ kéo thả slider
        private void Slider_bar_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            is_Draping = true;
        }
        // Hàm phụ trợ kéo thả slider
        private void Slider_bar_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            is_Draping = false;
        }

       // hook bàn phím
        private IKeyboardMouseEvents _hook;

        public void Subscribe()
        {
            _hook = Hook.GlobalEvents();

            _hook.KeyUp += _hook_KeyUp;
        }

        private void _hook_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {

            if (e.Control  && (e.Shift && (e.KeyCode == Keys.P)))
            {
                Play_one_music();
                lap1lan = 0;
                e.Handled = true;
            }
            if (e.Control && (e.Shift && (e.KeyCode == Keys.Space)))
            {
                if (_PathPlayList.Count > 0)
                {
                    play.Width = 35;
                    pause.Width = 0;
                    check_pause = true;
                    _check_play = false;
                    mediaPlayer.Pause();
                }
                e.Handled = true;
            }
            if (e.Control && (e.Shift && (e.KeyCode == Keys.S)))
            {
                if (_PathPlayList.Count > 0)
                {
                    mediaPlayer.Stop();
                    play.Width = 35;
                    pause.Width = 0;
                    slider_bar.Value = 0;
                    slider_bar.Maximum = 0;
                    run_time.Text = "00:00";
                    sum_time.Text = "00:00";
                    timer.Stop();
                    dem = 0;
                }
            }
            if (e.Control && e.Shift && e.KeyCode == Keys.T)
            {
                if(_PathPlayList.Count > 0)
                {
                    Next_Music_song();
                }
            }
            if (e.Control && e.Shift && e.KeyCode == Keys.B)
            {
                if (_PathPlayList.Count > 0)
                {
                    Previous_music_song();
                }
            }
            if (e.Alt && e.KeyCode == Keys.F4)
            {
                e.Handled = true;
            }
        }

        public void Unsubscribe()
        {
            _hook.KeyUp -= _hook_KeyUp;
            _hook.Dispose();
        }

        private void Maynghenhac_Loaded(object sender, RoutedEventArgs e)
        {
            Subscribe();
        }
    }
}
