using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;

namespace PaCSTools
{
    public class FTPClient
    {
        #region 构造函数
        ///  
        /// 构造函数 
        ///  
        /// 
        private static FTPClient instance;
        private static object lockObj = new object();
        ///  
        /// 主机地址 
        ///  
        private string strRemoteHost = "";
        ///  
        /// 本地文件路径 
        ///  
        private string strRemotePath = "";
        ///  
        /// FTP用户名 
        ///  
        private string strRemoteUser = "";
        ///  
        /// FTP密码 
        ///  
        private string strRemotePass = "";
        private int strRemotePort = 21;

        public FTPClient()
        { }

        public static FTPClient Instance()
        {
            lock (lockObj)
            {
                if (instance == null)
                {
                    instance = new FTPClient();
                }
            }
            return instance;
        }

        ///  
        /// 带参数的构造函数 
        ///  
        /// 主机地址 
        /// 路径 
        /// 用户名 
        /// 密码 
        /// 端口 
        public void FTPClientPram(string remoteHost, string remotePath, string remoteUser, string remotePass, int remotePort)
        {
            strRemoteHost = remoteHost;
            strRemotePath = remotePath;
            strRemoteUser = remoteUser;
            strRemotePass = remotePass;
            strRemotePort = remotePort;
            Connect();
        }
        #endregion

        #region 登陆
        ///  
        /// FTP服务器IP地址 
        ///  
        public string RemoteHost
        {
            get
            {
                return strRemoteHost;
            }
            set
            {
                strRemoteHost = value;
            }
        }
        ///  
        /// FTP服务器端口 
        ///  
        public int RemotePort
        {
            get
            {
                return strRemotePort;
            }
            set
            {
                strRemotePort = value;
            }
        }
        ///  
        /// 当前服务器目录 
        ///  
        public string RemotePath
        {
            get
            {
                return strRemotePath;
            }
            set
            {
                strRemotePath = value;
            }
        }
        ///  
        /// 登录用户账号 
        ///  
        public string RemoteUser
        {
            set
            {
                strRemoteUser = value;
            }
        }
        ///  
        /// 用户登录密码 
        ///  
        public string RemotePass
        {
            set
            {
                strRemotePass = value;
            }
        }

        ///  
        /// 是否登录 
        ///  
        private Boolean bConnected;
        public bool Connected
        {
            get
            {
                return bConnected;
            }
        }
        #endregion

        #region 链接
        ///  
        /// 建立连接  
        ///  
        public void Connect()
        {
            socketControl = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(RemoteHost), strRemotePort);
            // 链接 
            try
            {
                socketControl.Connect(ep);
            }
            catch (Exception)
            {
                throw new IOException("Couldn't connect to remote server");
            }

            // 获取应答码 
            ReadReply();
            if (iReplyCode != 220)
            {
                DisConnect();
                throw new IOException(strReply.Substring(4));
            }

            // 登陆 
            SendCommand("USER " + strRemoteUser);
            if (!(iReplyCode == 331 || iReplyCode == 230))
            {
                CloseSocketConnect();//关闭连接 
                throw new IOException(strReply.Substring(4));
            }
            if (iReplyCode != 230)
            {
                SendCommand("PASS " + strRemotePass);
                if (!(iReplyCode == 230 || iReplyCode == 202))
                {
                    CloseSocketConnect();//关闭连接 
                    throw new IOException(strReply.Substring(4));
                }
            }
            bConnected = true;

            // 切换到目录 
            ChDir(strRemotePath);
        }


        ///  
        /// 关闭连接 
        ///  
        public void DisConnect()
        {
            if (socketControl != null)
            {
                SendCommand("bye");
                SendCommand("QUIT");
            }
            CloseSocketConnect();
        }

        #endregion

        #region 传输模式

        ///  
        /// 传输模式:二进制类型、ASCII类型 
        ///  
        public enum TransferType
        {
            Binary,
            ASCII
        };

        ///  
        /// 设置传输模式 
        ///  
        /// 传输模式 
        public void SetTransferType(TransferType ttType)
        {
            if (ttType == TransferType.Binary)
            {
                SendCommand("TYPE I");//binary类型传输 
            }
            else
            {
                SendCommand("TYPE A");//ASCII类型传输 
            }
            if (iReplyCode != 200)
            {
                throw new IOException(strReply.Substring(4));
            }
            else
            {
                trType = ttType;
            }
        }


        ///  
        /// 获得传输模式 
        ///  
        /// 传输模式 
        public TransferType GetTransferType()
        {
            return trType;
        }

        #endregion

        #region 文件操作
        ///  
        /// 获得文件列表 
        ///  
        /// 文件名的匹配字符串 
        ///  
        public string[] Dir(string strMask)
        {
            // 建立链接 
            if (!bConnected)
            {
                Connect();
            }

            //建立进行数据连接的socket 
            Socket socketData = CreateDataSocket();

            //传送命令 
            SendCommand("NLST " + strMask);

            //分析应答代码 
            if (!(iReplyCode == 150 || iReplyCode == 125 || iReplyCode == 226))
            {
                throw new IOException(strReply.Substring(4));
            }

            //获得结果 
            strMsg = "";
            while (true)
            {
                int iBytes = socketData.Receive(buffer, buffer.Length, 0);
                strMsg += ASCII.GetString(buffer, 0, iBytes);
                if (iBytes < buffer.Length)
                {
                    break;
                }
            }
            char[] seperator = { '\n' };
            string[] strsFileList = strMsg.Split(seperator);
            socketData.Close();//数据socket关闭时也会有返回码 
            if (iReplyCode != 226)
            {
                ReadReply();
                if (iReplyCode != 226)
                {
                    throw new IOException(strReply.Substring(4));
                }
            }
            return strsFileList;
        }


        ///  
        /// 获取文件大小 
        ///  
        /// 文件名 
        /// 文件大小 
        public long GetFileSize(string strFileName)
        {
            if (!bConnected)
            {
                Connect();
            }
            SendCommand("SIZE " + Path.GetFileName(strFileName));
            long lSize = 0;
            if (iReplyCode == 213)
            {
                lSize = Int64.Parse(strReply.Substring(4));
            }
            else
            {
                throw new IOException(strReply.Substring(4));
            }
            return lSize;
        }


        ///  
        /// 删除 
        ///  
        /// 待删除文件名 
        public void Delete(string strFileName)
        {
            if (!bConnected)
            {
                Connect();
            }
            SendCommand("DELE " + strFileName);
            if (iReplyCode != 250)
            {
                throw new IOException(strReply.Substring(4));
            }
        }


        ///  
        /// 重命名(如果新文件名与已有文件重名,将覆盖已有文件) 
        ///  
        /// 旧文件名 
        /// 新文件名 
        public void Rename(string strOldFileName, string strNewFileName)
        {
            if (!bConnected)
            {
                Connect();
            }
            SendCommand("RNFR " + strOldFileName);
            if (iReplyCode != 350)
            {
                throw new IOException(strReply.Substring(4));
            }
            // 如果新文件名与原有文件重名,将覆盖原有文件 
            SendCommand("RNTO " + strNewFileName);
            if (iReplyCode != 250)
            {
                throw new IOException(strReply.Substring(4));
            }
        }
        #endregion

        #region 上传和下载
        /// <summary> 
        /// 上传一批文件 
        /// </summary> 
        /// <param name="strFolder">本地目录(不得以\结束)</param> 
        /// <param name="strFileNameMask">文件名匹配字符(可以包含*和?)</param> 
        public void Put(string strFolder, string strFileNameMask)
        {
            string[] strFiles = Directory.GetFiles(strFolder, strFileNameMask);
            foreach (string strFile in strFiles)
            {
                //strFile是完整的文件名(包含路径) 
                Put(strFile);
            }
        }

        ///  
        /// 上传一个文件 
        ///  
        /// 本地文件名 
        public void Put(string strFileName)
        {
            if (!bConnected)
            {
                Connect();
            }

            Socket socketData = CreateDataSocket();
            SendCommand("TYPE I");
            if (iReplyCode != 200)
            {
                throw new IOException(strReply.Substring(4));
            }
            SendCommand("STOR " + Path.GetFileName(strFileName));
            if (!(iReplyCode == 125 || iReplyCode == 150))
            {
                throw new IOException(strReply.Substring(4));
            }
            FileStream input = new FileStream(strFileName, FileMode.Open);
            int iBytes = 102400;
            while ((iBytes = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                socketData.Send(buffer, iBytes, 0);
            }
            input.Close();
            if (socketData.Connected)
            {
                socketData.Close();
            }
            ReadReply();
            //if (!(iReplyCode == 226 || iReplyCode == 250)) 
            //{ 
            //    ReadReply(); 
            //    if (!(iReplyCode == 226 || iReplyCode == 250)) 
            //    { 
            //        throw new IOException(strReply.Substring(4)); 
            //    } 
            //} 
        }
        /// <summary> 
        /// 下载一批文件 
        /// </summary> 
        /// <param name="strFileNameMask">文件名的匹配字符串</param> 
        /// <param name="strFolder">本地目录(不得以\结束)</param> 
        public void Get(string strFileNameMask, string strFolder)
        {
            if (!bConnected)
            {
                Connect();
            }
            string[] strFiles = Dir(strFileNameMask);
            foreach (string strFile in strFiles)
            {
                if (!strFile.Equals(""))//一般来说strFiles的最后一个元素可能是空字符串 
                {
                    Get(strFile, strFolder, strFile);
                }
            }
        }
        /// <summary> 
        /// 下载一个文件 
        /// </summary> 
        /// <param name="strRemoteFileName">要下载的文件名</param> 
        /// <param name="strFolder">本地目录(不得以\结束)</param> 
        /// <param name="strLocalFileName">保存在本地时的文件名</param> 
        public void Get(string strRemoteFileName, string strFolder, string strLocalFileName)
        {
            if (!bConnected)
            {
                Connect();
            }
            SetTransferType(TransferType.Binary);
            if (strLocalFileName.Equals(""))
            {
                strLocalFileName = strRemoteFileName;
            }
            //if (!File.Exists(strLocalFileName))
            //{
            //    Stream st = File.Create(strLocalFileName);
            //    st.Close();
            //}
            FileStream output = new
             FileStream(strFolder + "\\" +

         strLocalFileName, FileMode.Create);
            Socket socketData = CreateDataSocket();
            SendCommand("RETR " + strRemoteFileName);
            if (!(iReplyCode == 150 || iReplyCode == 125
             || iReplyCode == 226 || iReplyCode == 250))
            {
                throw new IOException(strReply.Substring(4));
            }
            while (true)
            {
                int iBytes = socketData.Receive(buffer,

            buffer.Length, 0);
                output.Write(buffer, 0, iBytes);
                if (iBytes <= 0)
                {
                    break;
                }
            }
            output.Close();
            if (socketData.Connected)
            {
                socketData.Close();
            }
            if (!(iReplyCode == 226 || iReplyCode == 250))
            {
                ReadReply();
                if (!(iReplyCode == 226 || iReplyCode == 250))
                {
                    throw new IOException

               (strReply.Substring(4));
                }
            }
        }


        #endregion

        #region 目录操作
        ///  
        /// 创建目录 
        ///  
        /// 目录名 
        public void MkDir(string strDirName)
        {
            if (!bConnected)
            {
                Connect();
            }
            SendCommand("MKD " + strDirName);
            if (iReplyCode != 257)
            {
                throw new IOException(strReply.Substring(4));
            }
        }


        ///  
        /// 删除目录 
        ///  
        /// 目录名 
        public void RmDir(string strDirName)
        {
            if (!bConnected)
            {
                Connect();
            }
            SendCommand("RMD " + strDirName);
            if (iReplyCode != 250)
            {
                throw new IOException(strReply.Substring(4));
            }
        }


        ///  
        /// 改变目录 
        ///  
        /// 新的工作目录名 
        public void ChDir(string strDirName)
        {
            if (strDirName.Equals(".") || strDirName.Equals(""))
            {
                return;
            }
            if (!bConnected)
            {
                Connect();
            }
            SendCommand("CWD " + strDirName);
            if (iReplyCode != 250)
            {
                throw new IOException(strReply.Substring(4));
            }
            this.strRemotePath = strDirName;
        }

        #endregion

        #region 内部变量
        ///  
        /// 服务器返回的应答信息(包含应答码) 
        ///  
        private string strMsg;
        ///  
        /// 服务器返回的应答信息(包含应答码) 
        ///  
        private string strReply;
        ///  
        /// 服务器返回的应答码 
        ///  
        private int iReplyCode;
        ///  
        /// 进行控制连接的socket 
        ///  
        private Socket socketControl;
        ///  
        /// 传输模式 
        ///  
        private TransferType trType;
        ///  
        /// 接收和发送数据的缓冲区 
        ///  
        private static int BLOCK_SIZE = 10240;
        Byte[] buffer = new Byte[BLOCK_SIZE];
        ///  
        /// 编码方式 
        ///  
        Encoding ASCII = Encoding.ASCII;
        #endregion

        #region 内部函数
        ///  
        /// 将一行应答字符串记录在strReply和strMsg 
        /// 应答码记录在iReplyCode 
        ///  
        private void ReadReply()
        {
            strMsg = "";
            strReply = ReadLine();
            iReplyCode = Int32.Parse(strReply.Substring(0, 3));
        }

        ///  
        /// 建立进行数据连接的socket 
        ///  
        /// 数据连接socket 
        private Socket CreateDataSocket()
        {
            SendCommand("PASV");
            if (iReplyCode != 227)
            {
                throw new IOException(strReply.Substring(4));
            }
            int index1 = strReply.IndexOf('(');
            int index2 = strReply.IndexOf(')');
            string ipData =
            strReply.Substring(index1 + 1, index2 - index1 - 1);
            int[] parts = new int[6];
            int len = ipData.Length;
            int partCount = 0;
            string buf = "";
            for (int i = 0; i < len && partCount <= 6; i++)
            {
                char ch = Char.Parse(ipData.Substring(i, 1));
                if (Char.IsDigit(ch))
                    buf += ch;
                else if (ch != ',')
                {
                    throw new IOException("Malformed PASV strReply: " +
                    strReply);
                }
                if (ch == ',' || i + 1 == len)
                {
                    try
                    {
                        parts[partCount++] = Int32.Parse(buf);
                        buf = "";
                    }
                    catch (Exception)
                    {
                        throw new IOException("Malformed PASV strReply: " +
                        strReply);
                    }
                }
            }
            string ipAddress = parts[0] + "." + parts[1] + "." +
            parts[2] + "." + parts[3];
            int port = (parts[4] << 8) + parts[5];
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ipAddress), port);
            try
            {
                s.Connect(ep);
            }
            catch (Exception)
            {
                throw new IOException("Can't connect to remote server");
            }
            return s;
        }


        ///  
        /// 关闭socket连接(用于登录以前) 
        ///  
        private void CloseSocketConnect()
        {
            if (socketControl != null)
            {
                socketControl.Close();
                socketControl = null;
            }
            bConnected = false;
        }

        ///  
        /// 读取Socket返回的所有字符串 
        ///  
        /// 包含应答码的字符串行 
        private string ReadLine()
        {
            while (true)
            {
                int iBytes = socketControl.Receive(buffer, buffer.Length, 0);
                strMsg += ASCII.GetString(buffer, 0, iBytes);
                if (iBytes < buffer.Length)
                {
                    break;
                }
            }
            char[] seperator = { '\n' };
            string[] mess = strMsg.Split(seperator);
            if (strMsg.Length > 2)
            {
                strMsg = mess[mess.Length - 2];
                //seperator[0]是10,换行符是由13和0组成的,分隔后10后面虽没有字符串, 
                //但也会分配为空字符串给后面(也是最后一个)字符串数组, 
                //所以最后一个mess是没用的空字符串 
                //但为什么不直接取mess[0],因为只有最后一行字符串应答码与信息之间有空格 
            }
            else
            {
                strMsg = mess[0];
            }
            if (!strMsg.Substring(3, 1).Equals(" "))//返回字符串正确的是以应答码(如220开头,后面接一空格,再接问候字符串) 
            {
                return ReadLine();
            }
            return strMsg;
        }

        ///  
        /// 发送命令并获取应答码和最后一行应答字符串 
        ///  
        /// 命令 
        private void SendCommand(String strCommand)
        {
            Byte[] cmdBytes =
            Encoding.ASCII.GetBytes((strCommand + "\r\n").ToCharArray());
            socketControl.Send(cmdBytes, cmdBytes.Length, 0);
            ReadReply();
        }
        #endregion
    }


}