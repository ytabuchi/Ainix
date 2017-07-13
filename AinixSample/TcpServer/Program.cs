/* Copyright(C) DOBON!. All rights reserved.
 * https://dobon.net/vb/dotnet/internet/tcpclientserver.html
 */
using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;

namespace TcpServer
{
    class Program
    {
        static void Main(string[] args)
        {
            //ListenするIPアドレス
            string ipString = "127.0.0.1";
            IPAddress ipAddress = IPAddress.Parse(ipString);

            //Listenするポート番号
            int port = 2001;

            //TcpListenerオブジェクトを作成する
            TcpListener listener = new TcpListener(ipAddress, port);

            //Listenを開始する
            listener.Start();
            Console.WriteLine("Listenを開始しました({0}:{1})。",
                ((IPEndPoint)listener.LocalEndpoint).Address,
                ((IPEndPoint)listener.LocalEndpoint).Port);

            //接続要求があったら受け入れる
            TcpClient client = listener.AcceptTcpClient();
            Console.WriteLine("クライアント({0}:{1})と接続しました。",
                ((IPEndPoint)client.Client.RemoteEndPoint).Address,
                ((IPEndPoint)client.Client.RemoteEndPoint).Port);

            //NetworkStreamを取得
            NetworkStream networkStream = client.GetStream();

            //読み取り、書き込みのタイムアウトを10秒にする
            //デフォルトはInfiniteで、タイムアウトしない
            //(.NET Framework 2.0以上が必要)
            //networkStream.ReadTimeout = 30000;
            //networkStream.WriteTimeout = 30000;

            //クライアントから送られたデータを受信する
            Encoding enc = Encoding.UTF8;
            bool disconnected = false;
            MemoryStream memoryStream = new MemoryStream();
            byte[] recievedBytes = new byte[4096];
            int recievedSize = 0;
            do
            {
                //データの一部を受信する
                recievedSize = networkStream.Read(recievedBytes, 0, recievedBytes.Length);
                //Readが0を返した時はクライアントが切断したと判断
                if (recievedSize == 0)
                {
                    disconnected = true;
                    Console.WriteLine("クライアントが切断しました。");
                    break;
                }
                //受信したデータを蓄積する
                memoryStream.Write(recievedBytes, 0, recievedSize);
                //まだ読み取れるデータがあるか、データの最後が\nでない時は、
                // 受信を続ける
            } while (networkStream.DataAvailable || recievedBytes[recievedSize - 1] != '\n');
            //受信したデータを文字列に変換
            string receivedMessage = enc.GetString(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
            memoryStream.Close();
            //末尾の\nを削除
            receivedMessage = receivedMessage.TrimEnd('\n');
            Console.WriteLine(receivedMessage);

            if (!disconnected)
            {
                //クライアントにデータを送信する
                //クライアントに送信する文字列を作成
                string sendMsg = receivedMessage.Length.ToString();
                //文字列をByte型配列に変換
                byte[] sendBytes = enc.GetBytes(sendMsg + '\n');
                //データを送信する
                networkStream.Write(sendBytes, 0, sendBytes.Length);
                Console.WriteLine(sendMsg);
            }

            //閉じる
            networkStream.Close();
            client.Close();
            Console.WriteLine("クライアントとの接続を閉じました。");

            //リスナを閉じる
            listener.Stop();
            Console.WriteLine("Listenerを閉じました。");

            Console.ReadLine();
        }
    }
}
