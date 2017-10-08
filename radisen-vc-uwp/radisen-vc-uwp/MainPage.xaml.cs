using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using Windows.Media.Audio;
using Windows.Media.SpeechRecognition;
using Windows.UI.Core;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace radisen_vc_uwp
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public SpeechRecognizer _MS_Recognizer { get; set; } = new SpeechRecognizer();

        public MainPage() {
            this.InitializeComponent();
            ApplicationView.GetForCurrentView().TryResizeView(new Size { Width = 650, Height = 720 });
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            // 録音デバイスの初期化
            Windows.Devices.Enumeration.DeviceInformationCollection inDevices =
                await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(Windows.Media.Devices.MediaDevice.GetAudioCaptureSelector());
            foreach (var item in inDevices) {
                cmbInputDevice.Items.Add(item.Name);
            }
            // 出力デバイスの初期化
            Windows.Devices.Enumeration.DeviceInformationCollection outDevices =
                await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(Windows.Media.Devices.MediaDevice.GetAudioRenderSelector());
            foreach (var item in outDevices) {
                cmbOutputDevice.Items.Add(item.Name);
            }
            //ハックグラウンドスレッドからUIスレッドを呼び出すためのDispatcher
            var dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;

            _MS_Recognizer = new Windows.Media.SpeechRecognition.SpeechRecognizer();
            await _MS_Recognizer.CompileConstraintsAsync();

            //認識中の処理定義
            _MS_Recognizer.HypothesisGenerated += async (sender2, e2) => {
                //認識途中に画面表示
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    txtLog.Text += e2.Hypothesis.Text;
                });
            };
            _MS_Recognizer.ContinuousRecognitionSession.ResultGenerated += async (sender2, e2) => {
                //認識完了後に画面に表示
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    txtLog.Text += "Waiting ...";
                    txtLog.Text += e2.Result.Text + "。\n";
                });
            };

            //認識開始
            await _MS_Recognizer.ContinuousRecognitionSession.StartAsync();
        }
    }
}
