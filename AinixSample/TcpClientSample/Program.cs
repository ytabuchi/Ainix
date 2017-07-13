/* Special Thanks to Hisaoka-san */
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace TcpClientSample
{
    class Program
    {
        static void Main(string[] args)
        {
            HTSample();
        }

        static void HTSample()
        {
            string ip = "127.0.0.1";
            int port = 2001;
            Encoding enc = Encoding.UTF8;

            // 指定したIP,ポートへ接続する
            TcpClient tcp = new TcpClient(ip, port);

            // データ送受信用ネットワークストリームの取得
            NetworkStream networkStream = tcp.GetStream();

            // 送信データ文字列をバイト変換し送信する
            byte[] sendBytes = enc.GetBytes($"Yoshito Tabuchi,Xamarin,Lot_Ainix,{DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss")}\n");
            networkStream.Write(sendBytes, 0, sendBytes.Length);

            // 送信後、サーバ側プログラムにて処理された結果のデータが返却される

            // 返却されたデータを受信しメモリストリームに取得後、文字列へ変換
            MemoryStream memoryStream = new MemoryStream();
            byte[] recievedBytes = new byte[4096];
            int recvSize = 0;
            do
            {
                recvSize = networkStream.Read(recievedBytes, 0, recievedBytes.Length);
                if (recvSize == 0)      // サーバーからの切断を検出
                {
                    break;
                }
                memoryStream.Write(recievedBytes, 0, recievedBytes.Length);
            } while (networkStream.DataAvailable);
            string strRecvDatRaw = enc.GetString(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);

            // データ送受信用ネットワークストリームの解放
            networkStream.Close();

            // 指定したIP,ポートから切断する
            tcp.Close();
        }
    }
}
