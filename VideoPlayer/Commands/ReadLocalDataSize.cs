using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace VideoPlayer.Commands
{
    class ReadLocalDataSize
    {
        private static StorageFile data_file;
        private static BasicProperties value_size;
        private static StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
        private static string filePath = storageFolder.Path + @"\histroyProgress.xml";
        public static async void GetLocalDataMethod(TextBlock textBlock)
        {
            if (File.Exists(filePath))
            {
                data_file = await storageFolder.GetFileAsync("histroyProgress.xml");
                value_size = await data_file.GetBasicPropertiesAsync();
                textBlock.Text = string.Format("{0:F}", (double)(value_size.Size) / 1024) + " KB";
            }
        }

        public static async void ClearData(TextBlock textBlock)
        {
            if (File.Exists(filePath))
            {
                //List<Progress> setting_progressList = SaveProgressVM.ReadData(filePath);
                await data_file.DeleteAsync();
                textBlock.Text = "0 KB";
                //setting_progressList.Clear();
            }
        }
    }
}
