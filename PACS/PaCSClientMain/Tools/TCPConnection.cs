using System;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace PaCSClientMain.Tools
{
    /// <summary>
    /// TCP连接类，成功连接返回TcpClient引用，否则返回null
    /// </summary>
    class TCPConnection
    {
        private IPAddress _ip = null;
        private int _port;
        private TcpClient _tcpc = null;

        public TCPConnection(IPAddress ip, int port)
        {
            _ip = ip;
            _port = port;
        }

        public TcpClient Connect()
        {
            try
            {
                _tcpc = new TcpClient();
                _tcpc.Connect(_ip, _port);
            }
            catch (Exception)
            {
                return null;
            }
            return _tcpc;
        }
    }
}
