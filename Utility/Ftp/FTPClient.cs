using System;
using System.IO;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Ifeng.Utility.Ftp
{
    /// <summary>
    /// FTP处理操作类
    /// 功能：
    /// 下载文件
    /// 上传文件
    /// 上传文件的进度信息
    /// 下载文件的进度信息
    /// 删除文件
    /// 列出文件
    /// 列出目录
    /// 进入子目录
    /// 退出当前目录返回上一层目录
    /// 判断远程文件是否存在
    /// 判断远程文件是否存在
    /// 删除远程文件
    /// 建立目录
    /// 删除目录
    /// 文件（目录）改名

    /// </summary>
    /// <remarks>
    /// 创建人：南疯
    /// 创建时间：2007年4月28日
    /// </remarks>
    #region 文件信息结构
    public struct FileStruct
    {
        public string Flags;
        public string Owner;
        public string Group;
        public long FileSize;
        public bool IsDirectory;
        public DateTime CreateTime;
        public string Name;
    }
    public enum FileListStyle
    {
        UnixStyle,
        WindowsStyle,
        Unknown
    }
    #endregion
    public enum MessageType : int
    {
        Error = 0,
        Warning = 1,
        Exception = 2,
        Prompt = 3,
        Reply = 4,
        Command = 5,
        Log = 6
    }

    public enum FTPClientStatus : int
    {
        UnLogIned = 0,
        LogInRequested = 1,
        LogIned = 2,
        LogOutRequested = 3,
        LogOuted = 4,
        UploadStarted = 5,
        UploadCompleted = 6,
        DownloadStarted = 7,
        DownloadCompleted = 8
    }

    public delegate void StatusChangedEventDelegate(object sender, FTPClientStatus newStatus);

    public delegate void SizeChangedEventDelegate(object sender, long currentSize);

    public class FTPClient : MessageMonitorAdaptor, IDisposable
    {

        #region /*encapsulate fields*/
        private string encode;
        public string Encode
        {
            get
            {
                return encode;
            }
            set
            {
                encode = value;
            }
        }
        private string serverName = "localhost";
        public string ServerName
        {
            get
            {
                return serverName;
            }
            set
            {
                serverName = value;
            }
        }
        private int servicePort = 21;
        public int ServicePort
        {
            get
            {
                return servicePort;
            }
            set
            {
                servicePort = Math.Abs(value);
            }
        }
        private string userName = "anonymous";
        public string UserName
        {
            get
            {
                return userName;
            }
            set
            {
                userName = value;
            }
        }
        private string password = "";
        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                password = value;
            }
        }
        protected byte[] buffer = new byte[1024 * 1024];//4K
        public int BufferSize
        {
            get
            {
                return this.buffer.Length;
            }
            set
            {
                this.buffer = new byte[value];
            }
        }
        private int bufferedStreamBufferSize = 4 * 1024 * 1024;//4M
        public int BufferedStreamBufferSize
        {
            get
            {
                return this.bufferedStreamBufferSize;
            }
            set
            {
                this.bufferedStreamBufferSize = value;
            }
        }
        private FTPClientStatus status = FTPClientStatus.UnLogIned;
        protected FTPClientStatus Status
        {
            get
            {
                return this.status;
            }
            set
            {
                this.status = value;
                this.TriggerStatusChangedEvent(this.status);
            }
        }
        public FTPClientStatus ClientStatus
        {
            get
            {
                return this.status;
            }
        }
        #endregion

        #region /*private and protected inner fields*/
        private int returnCode = 0;
        private string[] msgs = null;
        protected NetworkCommunicatingClient communicatingClient = null;
        protected NetworkCommunicatingClient dataCommunicatingClient = null;
        #endregion

        #region /*constructors*/
        public FTPClient()
        {
        }
        public FTPClient(string serverName, int servicePort, string userName, string password)
        {
            this.serverName = serverName;
            this.servicePort = servicePort;
            this.userName = userName;
            this.password = password;
        }
        #endregion

        #region/*destructor*/
        ~FTPClient()
        {
            this.Dispose();
        }
        #endregion

        #region/*protected methods*/
        protected void ThrowException(string info)
        {
            if (this.msgs != null && this.msgs.Length > 0) throw new Exception(info, new Exception(this.msgs[0]));
            else throw new Exception(info);
        }

        protected void GetReplyMessage()
        {
            string str2;
            string str = "";
        Label_0006:
            do
            {
                str2 = this.communicatingClient.ReadLine();
                if (str2 == null)
                {
                    this.ThrowException("can not get reply from server");
                }
            }
            while (str2.Equals(""));
            if (str.Equals(""))
            {
                str = str + str2;
            }
            else
            {
                str = str + "\n" + str2;
            }
            try
            {
                Convert.ToInt32(str2.Substring(0, 3));
                if (!str2.Substring(3, 1).Equals(" "))
                {
                    goto Label_0006;
                }
            }
            catch
            {
                goto Label_0006;
            }
            this.msgs = str.Split(new char[] { '\n' });
            foreach (string str3 in this.msgs)
            {
                base.MonitorMessage(str3, MessageType.Reply);
            }
            this.returnCode = Convert.ToInt32(this.msgs[0].Substring(0, 3));

        }
        protected void SendCommand(string command)
        {
            this.SendCommand(command, "");
        }
        protected void SendCommand(string command, string param)
        {
            if (param != "") param = " " + param;
            string commandText = command + param;//  +"\r\n";
            this.communicatingClient.FlushLine(commandText);
            this.GetReplyMessage();
        }
        protected void ExecuteCommand(string command, string param, int rightCode)
        {
            this.MonitorMessage(command + " " + param, MessageType.Command);
            this.SendCommand(command, param);
            if (this.returnCode != rightCode) this.ThrowException("exception occured in execute " + command + " command");
        }
        protected void ExecuteCommand(string command, string param, int rightCode1, int rightCode2)
        {
            this.MonitorMessage(command + " " + param, MessageType.Command);
            this.SendCommand(command, param);
            if (this.returnCode != rightCode1 && this.returnCode != rightCode2) this.ThrowException("exception occured in execute " + command + " command");
        }
        protected void WaitForReply(params int[] rightCodes)
        {
            this.GetReplyMessage();
            foreach (int rightCode in rightCodes)
            {
                if (this.returnCode == rightCode) return;
            }
            this.ThrowException("uncorrect reply message");
        }
        protected void Connect()
        {
            this.communicatingClient = new NetworkCommunicatingClient();
            this.communicatingClient.OpenConnection(this.serverName, this.servicePort, this.encode, this.serverName);
            this.WaitForReply(220);
        }
        protected void Identify()
        {
            this.MonitorMessage("USER " + this.userName, MessageType.Command);
            this.SendCommand("USER", this.userName);
            if (this.returnCode == 331 || this.returnCode == 230)
            {
                if (this.returnCode == 331)
                {
                    this.MonitorMessage("PASS ******", MessageType.Command);
                    SendCommand("PASS", this.password);
                    if (this.returnCode != 230 && this.returnCode != 202)
                        this.ThrowException("Invalid password");
                }
            }
            else this.ThrowException("Invalid user");
        }

        protected void QuitServer()
        {
            try { this.ExecuteCommand("QUIT", "", 221); }
            catch { }

            this.MonitorMessage("successfully log out.", MessageType.Prompt);
        }
        protected void CreateDataSocket(string bakHost, string encode)
        {
            this.ExecuteCommand("PASV", "", 227);

            string reply = this.msgs[0];
            int index1 = reply.IndexOf('(');
            int index2 = reply.IndexOf(')');
            string socketData = reply.Substring(index1 + 1, index2 - index1 - 1);
            string[] parts = socketData.Split(',');
            string ip = parts[0] + "." + parts[1] + "." + parts[2] + "." + parts[3];
            int port = (Convert.ToInt32(parts[4]) << 8) + Convert.ToInt32(parts[5]);

            this.dataCommunicatingClient = new NetworkCommunicatingClient();
            this.dataCommunicatingClient.OpenConnection(ip, port, encode, bakHost);
        }
        protected void CloseDataSocket()
        {
            this.dataCommunicatingClient.CloseConnection();
        }
        protected string GetDataFromDataSocket()
        {
            return this.dataCommunicatingClient.ReadToEnd();
        }

        protected void SetTransferStartPosition(long offset)
        {
            this.ExecuteCommand("REST", offset.ToString(), 350);
        }
        protected void SetBinaryMode(bool mode)
        {
            if (mode) this.ExecuteCommand("TYPE", "I", 200);
            else this.ExecuteCommand("TYPE", "A", 200);
        }
        #endregion

        #region/*public methods*/
        public void LogIn()
        {
            if (this.IsConnected) return;
            this.Status = FTPClientStatus.LogInRequested;
            this.MonitorMessage("connecting to " + this.serverName + " at port " + this.servicePort + "...", MessageType.Prompt);
            this.Connect();
            this.MonitorMessage("successfully connected.", MessageType.Prompt);
            this.MonitorMessage("logging in...", MessageType.Prompt);
            this.Identify();
            this.MonitorMessage("successfully log in.", MessageType.Prompt);
            this.Status = FTPClientStatus.LogIned;
        }

        public void LogOut()
        {
            if ((this.Status != FTPClientStatus.UnLogIned) && (this.Status != FTPClientStatus.LogOuted))
            {
                this.Status = FTPClientStatus.LogOutRequested;
                try
                {
                    this.QuitServer();
                }
                catch
                {
                }
                try
                {
                    if (this.dataCommunicatingClient != null)
                    {
                        this.dataCommunicatingClient.CloseConnection();
                    }
                }
                catch
                {
                }
                finally
                {
                    this.dataCommunicatingClient = null;
                }
                try
                {
                    if (this.communicatingClient != null)
                    {
                        this.communicatingClient.CloseConnection();
                    }
                }
                catch
                {
                }
                finally
                {
                    this.communicatingClient = null;
                }
                this.Status = FTPClientStatus.LogOuted;
            }
        }
        /// <param name="dir">
        /// if dir starts with "/",then dir stands for absolute path
        /// else dir stands for relative path
        /// dir support for cascade path ,such as "/download/test"(absolute) or "download/test"(relative)
        /// </param>
        public void Chdir(string dir)
        {
            if (dir == null || dir == "" || dir == "/") return;
            this.ExecuteCommand("CWD", dir, 250);
        }

        public void ChMyDir(string dir)
        {
            dir = dir.Replace("\\", "/");
            while (true)
            {
                int nSlashNo = dir.IndexOf("/");
                if (nSlashNo == -1)
                    break;
                string temp = dir.Substring(0, nSlashNo);
                Chdir(temp);
                dir = dir.Substring(nSlashNo + 1, dir.Length - nSlashNo - 1);
            }
        }

        public void Mkdir(string dir)
        {
            if (dir == null || dir == "" || dir == "/") return;
            this.ExecuteCommand("MKD", dir, 257);
        }

        public void MkMydir(string dir)
        {
            dir = dir.Replace("\\", "/");
            while (true)
            {
                int nSlashNo = dir.IndexOf("/");
                if (nSlashNo == -1)
                    break;
                string temp = dir.Substring(0, nSlashNo);
                Mkdir(temp);
                dir = dir.Substring(nSlashNo + 1, dir.Length - nSlashNo - 1);
            }
        }

        public void Rmdir(string dir)
        {
            this.ExecuteCommand("RMD", dir, 250);
        }
        public void RenameFile(string oldName, string newName)
        {
            this.ExecuteCommand("RNFR", oldName, 350);
            //if newName file exist,RNTO command will overwrite it 
            this.ExecuteCommand("RNTO", newName, 250);
        }
        public void DeleteFile(string fileName)
        {
            this.ExecuteCommand("DELE", fileName, 250);
        }
        public string GetCurrentDirectory()
        {
            this.ExecuteCommand("PWD", "", 257);
            string reply = this.msgs[0];
            int index1 = reply.IndexOf('\"');
            int index2 = reply.IndexOf('\"', index1 + 1);
            return reply.Substring(index1 + 1, index2 - index1 - 1);
        }
        public long GetFileSize(string fileName)
        {
            try
            {
                this.ExecuteCommand("SIZE", fileName, 213);
                return Convert.ToInt64(this.msgs[0].Substring(4));
            }
            catch
            {
                return -1L;
            }
        }
        public string[] GetFileList(string mask)
        {
            this.CreateDataSocket(this.serverName, this.encode);
            this.ExecuteCommand("NLST", mask, 150, 125);
            string data = this.GetDataFromDataSocket();
            string[] files = data.Split('\n');
            this.CloseDataSocket();
            this.WaitForReply(226);
            return files;
        }

        public string[] GetDirectoryList()
        {
            this.CreateDataSocket(this.serverName, this.encode);
            this.ExecuteCommand("LIST", "", 150, 125);
            string data = this.GetDataFromDataSocket();
            string[] dirs = data.Split('\n');
            this.CloseDataSocket();
            this.WaitForReply(226);
            return dirs;
        }
        /// <summary>
        /// 列出FTP服务器上面当前目录的所有文件和目录
        /// </summary>
        public FileStruct[] ListFilesAndDirectories()
        {
            this.CreateDataSocket(this.serverName, this.encode);
            this.ExecuteCommand("LIST", "", 150, 125);
            string data = this.GetDataFromDataSocket();
            FileStruct[] list = GetList(data);
            return list;
        }


        /// <summary>
        /// 列出FTP服务器上面当前目录的所有文件
        /// </summary>
        public FileStruct[] ListFiles()
        {
            FileStruct[] listAll = ListFilesAndDirectories();
            List<FileStruct> listFile = new List<FileStruct>();
            foreach (FileStruct file in listAll)
            {
                if (!file.IsDirectory)
                {
                    listFile.Add(file);
                }
            }
            return listFile.ToArray();
        }
        /// <summary>
        /// 列出FTP服务器上面当前目录的所有的目录
        /// </summary>
        public FileStruct[] ListDirectories()
        {
            FileStruct[] listAll = ListFilesAndDirectories();
            List<FileStruct> listDirectory = new List<FileStruct>();
            foreach (FileStruct file in listAll)
            {
                if (file.IsDirectory)
                {
                    listDirectory.Add(file);
                }
            }
            return listDirectory.ToArray();
        }

        /// <summary>
        /// 获得文件和目录列表
        /// </summary>
        /// <param name="datastring">FTP返回的列表字符信息</param>
        private FileStruct[] GetList(string datastring)
        {
            List<FileStruct> myListArray = new List<FileStruct>();
            string[] dataRecords = datastring.Split('\n');
            FileListStyle _directoryListStyle = GuessFileListStyle(dataRecords);
            foreach (string s in dataRecords)
            {
                if (_directoryListStyle != FileListStyle.Unknown && s != "")
                {
                    FileStruct f = new FileStruct();
                    f.Name = "..";
                    switch (_directoryListStyle)
                    {
                        case FileListStyle.UnixStyle:
                            f = ParseFileStructFromUnixStyleRecord(s);
                            break;
                        case FileListStyle.WindowsStyle:
                            f = ParseFileStructFromWindowsStyleRecord(s);
                            break;
                    }
                    if (!(f.Name == "." || f.Name == ".."))
                    {
                        myListArray.Add(f);
                    }
                }
            }
            return myListArray.ToArray();
        }


        /// <summary>
        /// 从Windows格式中返回文件信息
        /// </summary>
        /// <param name="Record">文件信息</param>
        private FileStruct ParseFileStructFromWindowsStyleRecord(string Record)
        {
            FileStruct f = new FileStruct();
            string processstr = Record.Trim();
            string dateStr = processstr.Substring(0, 8);
            processstr = (processstr.Substring(8, processstr.Length - 8)).Trim();
            string timeStr = processstr.Substring(0, 7);
            processstr = (processstr.Substring(7, processstr.Length - 7)).Trim();
            DateTimeFormatInfo myDTFI = new CultureInfo("en-US", false).DateTimeFormat;
            myDTFI.ShortTimePattern = "t";
            f.CreateTime = DateTime.Parse(dateStr + " " + timeStr, myDTFI);
            if (processstr.Substring(0, 5) == "<DIR>")
            {
                f.IsDirectory = true;
                processstr = (processstr.Substring(5, processstr.Length - 5)).Trim();
            }
            else
            {
                string[] strs = processstr.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);   // true);
                processstr = strs[1];
                f.IsDirectory = false;
            }
            f.Name = processstr;
            return f;
        }
        /// <summary>
        /// 判断文件列表的方式Window方式还是Unix方式
        /// </summary>
        /// <param name="recordList">文件信息列表</param>
        private FileListStyle GuessFileListStyle(string[] recordList)
        {
            foreach (string s in recordList)
            {
                if (s.Length > 10
                && Regex.IsMatch(s.Substring(0, 10), "(-|d)(-|r)(-|w)(-|x)(-|r)(-|w)(-|x)(-|r)(-|w)(-|x)"))
                {
                    return FileListStyle.UnixStyle;
                }
                else if (s.Length > 8
                && Regex.IsMatch(s.Substring(0, 8), "[0-9][0-9]-[0-9][0-9]-[0-9][0-9]"))
                {
                    return FileListStyle.WindowsStyle;
                }
            }
            return FileListStyle.Unknown;
        }
        /// <summary>
        /// 从Unix格式中返回文件信息
        /// </summary>
        /// <param name="Record">文件信息</param>
        private FileStruct ParseFileStructFromUnixStyleRecord(string Record)
        {
            FileStruct f = new FileStruct();
            string processstr = Record.Trim();
            f.Flags = processstr.Substring(0, 10);
            f.IsDirectory = (f.Flags[0] == 'd');
            processstr = (processstr.Substring(11)).Trim();
            _cutSubstringFromStringWithTrim(ref processstr, ' ', 0);   //跳过一部分
            f.Owner = _cutSubstringFromStringWithTrim(ref processstr, ' ', 0);
            f.Group = _cutSubstringFromStringWithTrim(ref processstr, ' ', 0);
            _cutSubstringFromStringWithTrim(ref processstr, ' ', 0);   //跳过一部分
            string yearOrTime = processstr.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[2];
            if (yearOrTime.IndexOf(":") >= 0)  //time
            {
                processstr = processstr.Replace(yearOrTime, DateTime.Now.Year.ToString());
            }
            f.CreateTime = DateTime.Parse(_cutSubstringFromStringWithTrim(ref processstr, ' ', 8));
            f.Name = processstr;   //最后就是名称
            return f;
        }
        /// <summary>
        /// 按照一定的规则进行字符串截取
        /// </summary>
        /// <param name="s">截取的字符串</param>
        /// <param name="c">查找的字符</param>
        /// <param name="startIndex">查找的位置</param>
        private string _cutSubstringFromStringWithTrim(ref string s, char c, int startIndex)
        {
            int pos1 = s.IndexOf(c, startIndex);
            string retString = s.Substring(0, pos1);
            s = (s.Substring(pos1)).Trim();
            return retString;
        }

        public void Upload(string fileName,string newFileName)
        {
            this.Upload(fileName, false, newFileName);
        }
        public void Upload(string fileName, bool resume,string newFileName)
        {
            this.Upload(fileName, null, resume,newFileName);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="targetPath"></param>
        /// <param name="resume"></param>
        public void Upload(string fileName, string targetPath, bool resume, string newFileName)
        {
            this.Upload(fileName, targetPath, Path.GetFileName(fileName), resume, newFileName);
        }
        /// <param name="targetPath">
        ///  targetPath support absolute path and relative path
        /// </param>
        public void Upload(string fileName, string targetPath, string targetName, bool resume, string newFileName)
        {
            this.Status = FTPClientStatus.UploadStarted;
            string dir = null;
            try
            {
                dir = this.GetCurrentDirectory();
            }
            catch (Exception)
            {
            }

            this.Chdir(targetPath);

            this.SetBinaryMode(true);

            long offset = 0;
            if (resume)//FTP server is supposed to support for resume,otherwise an Exception will be thrown if resume=true.
            {
                try
                {
                    offset = this.GetFileSize(fileName);
                }
                catch
                {
                    offset = 0;
                }
                this.MonitorMessage("upload using resume mode", MessageType.Prompt);
            }
            else this.MonitorMessage("upload using overwrite mode", MessageType.Prompt);

            this.CreateDataSocket(this.serverName, this.encode);

            FileStream input = null;
            try
            {
                input = new FileStream(fileName, FileMode.Open);
                if (offset > 0)
                {
                    this.SetTransferStartPosition(offset);
                    this.MonitorMessage("seeking to " + offset + "...", MessageType.Prompt);
                    input.Seek(offset, SeekOrigin.Begin);
                }
                this.ExecuteCommand("STOR", newFileName, 125, 150);
                int bytes = 0; int length = this.BufferSize;
                long curPos = offset;
                this.MonitorMessage("uploading file \"" + fileName + "\" to server...", MessageType.Prompt);
                BufferedStream bufferInput = new BufferedStream(input, this.bufferedStreamBufferSize);//add BufferedStream to improve I/O performance
                while ((bytes = bufferInput.Read(this.buffer, 0, length)) > 0)
                {
                    this.dataCommunicatingClient.WriteBuffer(this.buffer, 0, bytes);
                    curPos += bytes;
                    this.TriggerSizeChangedEvent(curPos);
                }
                this.dataCommunicatingClient.Flush();
                bufferInput.Close();
                input.Close();
            }
            catch (Exception e)
            {
                if ((dir != null) && (dir.Length != 0))
                {
                    try
                    {
                        this.Chdir(dir);
                    }
                    catch (Exception)
                    {
                    }
                }


                try
                {
                    input.Close();
                }
                catch { }
                throw e;
            }
            this.MonitorMessage("finished.", MessageType.Prompt);
            this.CloseDataSocket();

            this.WaitForReply(226, 250);

            this.Status = FTPClientStatus.UploadCompleted;
        }

        public void Download(string fileName)
        {
            this.Download(fileName, "", false);
        }
        public void Download(string fileName, bool resume)
        {
            this.Download(fileName, "", resume);
        }
        public void Download(string fileName, string saveAs)
        {
            this.Download(fileName, saveAs, false);
        }
        /// <param name="fileName">
        ///  fileName support path
        ///  path support absolute path and relative path
        /// </param>
        public void Download(string fileName, string saveAs, bool resume)
        {
            this.Status = FTPClientStatus.DownloadStarted;
            string dir = null;
            try
            {
                dir = this.GetCurrentDirectory();
            }
            catch (Exception)
            {
            }
            string str2 = fileName;
            int num = fileName.LastIndexOf("/");
            if (num >= 0)
            {
                string str3 = fileName.Substring(0, num + 1);
                this.Chdir(str3);
                fileName = fileName.Substring(num + 1, (fileName.Length - num) - 1);
            }
            if ((saveAs == null) || saveAs.Equals(""))
            {
                saveAs = fileName;
            }
            if (!resume)
            {
                try
                {
                    File.Delete(saveAs);
                }
                catch
                {
                }
            }
            if (!File.Exists(saveAs))
            {
                File.Create(saveAs).Close();
            }
            this.SetBinaryMode(true);
            this.CreateDataSocket(this.serverName, this.encode);
            FileStream stream = null;
            try
            {
                stream = new FileStream(saveAs, FileMode.Open);
                long fileSize = this.GetFileSize(fileName);
                long offset = 0L;
                if (resume)
                {
                    offset = stream.Length;
                    if (offset > 0L)
                    {
                        this.SetTransferStartPosition(offset);
                        base.MonitorMessage("seeking to " + offset + "...", MessageType.Prompt);
                        stream.Seek(offset, SeekOrigin.Begin);
                    }
                }
                this.ExecuteCommand("RETR", fileName, 0x7d, 150);
                int count = 0;
                int bufferSize = this.BufferSize;
                long currentSize = offset;
                base.MonitorMessage("downloading file \"" + str2 + "\" from server,saving as \"" + saveAs + "\"...", MessageType.Prompt);
                BufferedStream stream3 = new BufferedStream(stream, this.bufferedStreamBufferSize);
                while ((count = this.dataCommunicatingClient.ReadBuffer(this.buffer, 0, bufferSize)) > 0)
                {
                    stream3.Write(this.buffer, 0, count);
                    currentSize += count;
                    this.TriggerSizeChangedEvent(currentSize);
                    if (currentSize >= fileSize)
                    {
                        break;
                    }
                }
                stream3.Flush();
                stream3.Close();
                stream.Close();
            }
            catch (Exception exception)
            {
                if ((dir != null) && (dir.Length != 0))
                {
                    try
                    {
                        this.Chdir(dir);
                    }
                    catch (Exception)
                    {
                    }
                }
                try
                {
                    stream.Close();
                }
                catch
                {
                }
                throw exception;
            }
            if ((dir != null) && (dir.Length != 0))
            {
                try
                {
                    this.Chdir(dir);
                }
                catch (Exception)
                {
                }
            }
            base.MonitorMessage("finished.", MessageType.Prompt);
            this.CloseDataSocket();
            this.WaitForReply(new int[] { 0xe2, 250 });
            this.Status = FTPClientStatus.DownloadCompleted;

        }
        public bool IsConnected
        {
            get
            {
                try
                {
                    this.ExecuteCommand("PWD", "", 257);
                    return true;
                }
                catch
                {
                    this.status = FTPClientStatus.UnLogIned;
                    return false;
                }
            }
        }
        #endregion

        #region/*events*/
        public event SizeChangedEventDelegate OnSizeChanged = null;
        protected void TriggerSizeChangedEvent(long currentSize)
        {
            if (this.OnSizeChanged != null) this.OnSizeChanged(this, currentSize);
        }
        public event StatusChangedEventDelegate OnStatusChanged = null;
        protected void TriggerStatusChangedEvent(FTPClientStatus newStatus)
        {
            if (this.OnStatusChanged != null) this.OnStatusChanged(this, newStatus);
        }
        #endregion

        #region/*IDisposable member*/
        public void Dispose()
        {
            this.LogOut();
        }
        #endregion

    }

    #region /*message monitor adaptor*/
    public delegate void MessageMonitorDelegate(object sender, string msg, MessageType mtype);
    public class MessageMonitorAdaptor
    {

        #region/*events*/
        public event MessageMonitorDelegate OnMessage = null;
        protected void MonitorMessage(string msg, MessageType mtype)
        {
            if (this.OnMessage != null) this.OnMessage(this, msg, mtype);
        }
        #endregion

    }
    #endregion

    #region /*network communicator*/
    public class NetworkCommunicator
    {
        private NetworkStream networkStream = null;
        private StreamReader streamReader = null;
        private StreamWriter streamWriter = null;
        private bool closed = true;
        public NetworkCommunicator()
        {
        }
        public void OpenCommunicator(NetworkStream ns, string encode)
        {
            if (!this.closed) return;
            this.networkStream = ns;
            this.streamReader = new StreamReader(this.networkStream, System.Text.Encoding.GetEncoding(encode));
            this.streamWriter = new StreamWriter(this.networkStream, System.Text.Encoding.GetEncoding(encode));
            this.closed = false;
        }
        public void WriteBuffer(byte[] buffer, int offset, int count)
        {
            this.streamWriter.BaseStream.Write(buffer, offset, count);
            this.streamWriter.BaseStream.Flush();
        }
        public int ReadBuffer(byte[] buffer, int offset, int length)
        {
            return this.streamReader.BaseStream.Read(buffer, offset, length);
        }
        public void WriteLine(string msg)
        {
            this.streamWriter.WriteLine(msg);
        }
        public void Flush()
        {
            this.streamWriter.Flush();
        }
        public void FlushLine(string msg)
        {
            this.WriteLine(msg);
            this.Flush();
        }
        public string ReadLine()
        {
            return this.streamReader.ReadLine();
        }
        public string ReadToEnd()
        {
            return this.streamReader.ReadToEnd();
        }
        public void CloseCommunicator()
        {
            if (this.closed) return;

            try { this.streamReader.Close(); }
            catch { }
            finally { this.streamReader = null; }

            try { this.streamWriter.Close(); }
            catch { }
            finally { this.streamWriter = null; }

            try { this.networkStream.Close(); }
            catch { }
            finally { this.networkStream = null; }

            this.closed = true;
        }
        ~NetworkCommunicator()
        {
            this.CloseCommunicator();
        }
    }
    #endregion

    #region /*network communicator Client*/
    public class NetworkCommunicatingClient : NetworkCommunicator
    {
        private TcpClient tcpClient = null;
        private bool closed = true;
        public NetworkCommunicatingClient()
            : base()
        {
        }
        public void OpenConnection(string host, int port, string encode, string bakHost = "")
        {
            if (!this.closed) return;
            try
            {
                this.tcpClient = new TcpClient(bakHost, port);
            }
            catch
            {
                if (!string.IsNullOrEmpty(bakHost))
                    this.tcpClient = new TcpClient(host, port);
            }
            this.tcpClient.ReceiveTimeout = 30000;
            this.tcpClient.SendTimeout = 30000;
            this.OpenCommunicator(this.tcpClient.GetStream(), encode);
            this.closed = false;
        }
        public void CloseConnection()
        {
            if (this.closed) return;

            this.CloseCommunicator();

            try { this.tcpClient.Close(); }
            catch { }
            finally { this.tcpClient = null; }

            this.closed = true;
        }
        ~NetworkCommunicatingClient()
        {
            this.CloseConnection();
        }
    }
    #endregion
}
