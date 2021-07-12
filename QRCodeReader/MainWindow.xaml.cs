using System;
using System.Collections.Generic;
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

using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.WpfExtensions;
using ZXing;
using System.Drawing;

namespace QRCodeReader
{
    public partial class MainWindow : System.Windows.Window
    {
        VideoCapture _qrCapture;
        Mat _qrMat;
        Bitmap _qrBmp;
        BitmapSource _qrBmpSource;

        string _qrText = "wikipedia";

        public MainWindow()
        {
            InitializeComponent();

            // カメラの設定
            _qrCapture = new VideoCapture(0);
            //_qrCapture = new VideoCapture(0, VideoCaptureAPIs.DSHOW);   // Logicool製の場合はこっち

            // 解像度の設定
            _qrCapture.FrameWidth = 640;
            _qrCapture.FrameHeight = 360;

            // WEBカメラの映像を毎フレーム描画する
            // 指定時間ごとに描画する場合はDispatcherTimerが良いかと
            CompositionTarget.Rendering += UpdateCamera;
        }

        private void UpdateCamera(object sender, EventArgs e)
        {
            // MatクラスとBitmapクラスは毎フレーム破棄する
            using (_qrMat = new Mat())
            {
                // WEBカメラの映像を読み込む
                _qrCapture.Read(_qrMat);

                // 映像がない場合は処理を終了する
                if (_qrMat.Empty()) return;

                // 読み込んだ画像をBitmap形式とBitmapSource形式に変換する
                _qrBmp = BitmapConverter.ToBitmap(_qrMat);
                _qrBmpSource = BitmapSourceConverter.ToBitmapSource(_qrMat);

                // ウインドウにWEBカメラの映像を表示する
                _boxCamera.Source = _qrBmpSource;

                // QRコードの解析を行う
                QRRead(_qrBmp);

                _qrBmp.Dispose();
            }
        }

        void QRRead(Bitmap bmp)
        {
            // BarcodeReaderクラスとResultクラスを宣言
            BarcodeReader reader = new BarcodeReader();
            Result result = reader.Decode(bmp);

            string text = string.Empty;

            // Bitmapがnullでなければ文字列を代入する
            if (result != null)
                text = result.Text;

            // QRコードを見つけたら、結果をテキストボックスに表示する
            if (text != string.Empty)
            {
                // テキストボックスに文字列を表示する
                _boxText.Text = result.Text;

                // 出力された結果を元に判定・処理を行う
                if (text.Contains(_qrText))
                {
                    Console.WriteLine("QR Text matched.");
                    _boxText2.Content = "QR Text matched.";
                }
                else
                {
                    Console.WriteLine("QR Text is not matched.");
                    _boxText2.Content = "QR Text is not matched.";
                }
            }
        }
    }
}
