using AxWMPLib;
using MusicPlayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Testproject
{
    public partial class Form1 : Form
    {
        private DoubleLinkedList DLL = new DoubleLinkedList();   //Khởi tạo danh sách liên kết
        public Form1()
        {
            InitializeComponent();
        }
        OpenFileDialog openFileDialog;    //Khởi tạo biến để mở hộp thoại file cho phép chọn file
        string[] filePaths, fileNames;    //Mảng chứa đường dẫn và tên file của bài hát
        private void btn_Add_Click(object sender, EventArgs e)   
        {
            openFileDialog = new OpenFileDialog();   //Khởi tạo hộp thoại mở file
            openFileDialog.Filter = "Audio and Video Files (*.mp3; *.mp4)|*.mp3; *.mp4";   //Chỉ cho phép chọn file âm thanh và video
            openFileDialog.Multiselect = true;   //Cho phép chọn nhiều file
            openFileDialog.CheckFileExists = true;   //Kiểm tra file tồn tại
            openFileDialog.CheckPathExists = true;   //Kiểm tra đường dẫn tồn tại
            openFileDialog.Title = "Select Audio or Video Files";   //Tiêu đề của hộp thoại
            if (openFileDialog.ShowDialog() == DialogResult.OK)   //Nếu chọn file thành công
            {
                string[] filePaths = openFileDialog.FileNames;   //Lấy đường dẫn của các file
                string[] fileNames = openFileDialog.SafeFileNames;   //Lấy tên của các file
                for (int i = 0; i < filePaths.Length; i++)   //Duyệt qua các file đã chọn
                {
                    bool isExist = false;   //Biến kiểm tra file đã tồn tại trong danh sách chưa
                    foreach (string item in ListSong.Items)   //Duyệt qua các item trong danh sách
                    {
                        if (item.ToString().Equals(fileNames[i], StringComparison.OrdinalIgnoreCase))   //Nếu file đã tồn tại trong danh sách (không phân biệt hoa thường)
                        {
                            isExist = true;   //Gán biến kiểm tra bằng true
                            break;   //Thoát khỏi vòng lặp
                        }
                    }
                    if (!isExist)   //Nếu file chưa tồn tại trong danh sách
                    {
                        DLL.AddSong(filePaths[i], fileNames[i]);   //Thêm file vào danh sách
                        ListSong.Items.Add(fileNames[i]);   //Hiện thị tên file trong danh sách
                    }
                    else   //Nếu file đã tồn tại trong danh sách
                        MessageBox.Show($"File \"{fileNames[i]}\" đã tồn tại trong danh sách!", "Trùng lặp", MessageBoxButtons.OK, MessageBoxIcon.Warning);   //Thông báo file đã tồn tại
                }
            }
        }
        private void btn_Remove_Click(object sender, EventArgs e)
        {
            if (ListSong.SelectedIndex != -1)   // Nếu đã chọn bài hát
            {
                int selectedIndex = ListSong.SelectedIndex;   // Lấy vị trí bài hát được chọn
                DLL.RemoveAt(selectedIndex);   // Xóa bài hát khỏi danh sách
                ListSong.Items.RemoveAt(selectedIndex);   // Xóa bài hát khỏi ListBox
                MessageBox.Show("Đã xóa bài hát thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);   // Thông báo xóa thành công
            }
            else MessageBox.Show("Vui lòng chọn một bài hát để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);   // Thông báo chưa chọn bài hát
        }
        private void btn_Swap_Click(object sender, EventArgs e)
        {
            if(ListSong.SelectedIndices.Count != 2)   // Nếu chưa chọn đúng 2 dòng(bài hát)
            {
                MessageBox.Show("Vui lòng chọn 2 bài hát để hoán đổi vị trí!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);   // Thông báo chưa chọn 2 bài hát
                return;   // Thoát khỏi phương thức
            }
            int index1 = ListSong.SelectedIndices[0];   // Lấy vị trí bài hát đầu tiên
            int index2 = ListSong.SelectedIndices[1];   // Lấy vị trí bài hát thứ hai
            DLL.SwapSong(index1, index2);   // Hoán đổi vị trí 2 bài hát
            RefreshListSong(index1, index2);   // Cập nhật danh sách
            MessageBox.Show("Đã hoán đổi vị trí thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);   // Thông báo hoán đổi thành công
        }
        private void ListSong_DoubleClick(object sender, EventArgs e)
        {
            int selectedIndex = ListSong.SelectedIndex;   // Lấy vị trí bài hát được chọn
            DLL.Current = DLL.Head;   // Khởi tạo Node hiện tại là Node đầu tiên
            for (int i = 0; i < selectedIndex; i++)   // Duyệt qua các Node trong danh sách
                DLL.Current = DLL.Current.Next;   // Chuyển sang Node tiếp theo
            axWindowsMediaPlayer.URL = DLL.Current.FilePath;   // Gán đường dẫn bài hát cho Media Player
            axWindowsMediaPlayer.Ctlcontrols.play();   // Phát bài hát
            btnPlay.BackgroundImage = Properties.Resources.pause;
        }
        private bool isButtonClick = false;   // Biến kiểm tra trạng thái nút nhấn
        private void btnPlay_Click(object sender, EventArgs e)
        {
            isButtonClick = true; // Đánh dấu trạng thái được thay đổi bởi nút nhấn

            if (axWindowsMediaPlayer.playState == WMPLib.WMPPlayState.wmppsPlaying)
            {
                // Tạm dừng nhạc và đổi biểu tượng sang Play
                axWindowsMediaPlayer.Ctlcontrols.pause();
                btnPlay.BackgroundImage = Properties.Resources.play;
                //timer.Stop();
            }
            else
            {
                // Phát nhạc và đổi biểu tượng sang Pause
                axWindowsMediaPlayer.Ctlcontrols.play();
                btnPlay.BackgroundImage = Properties.Resources.pause;
                //timer.Start();
            }
            btnPlay.Refresh(); // Cập nhật giao diện
            isButtonClick = false; // Hoàn thành xử lý nút nhấn
        }

        private void RefreshListSong(int index1, int index2)   
        {
            ListSong.Items.Clear();   // Xóa danh sách hiện tại
            SongNode current = DLL.Head;   // Khởi tạo Node hiện tại là Node đầu tiên
            while (current != null)   // Duyệt qua các Node trong danh sách
            {
                ListSong.Items.Add(current.FileName);   // Thêm tên bài hát vào danh sách
                current = current.Next;   // Chuyển sang Node tiếp theo
            }
            ListSong.SelectedIndices.Add(index1);   //Đánh dấu chọn bài hát đầu tiên
            ListSong.SelectedIndices.Add(index2);   //Đánh dấu chọn bài hát thứ hai
        }
    }
}
