using log4net;
using QSACTIVEXLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransferControl.CommandConvert;
using TransferControl.Engine;
using TransferControl.Management;
using TransferControl.Operation;

namespace SECSInterface
{
    public class SECSGEM :IUserInterfaceReport
    {
        static ILog logger = LogManager.GetLogger(typeof(SECSGEM));
        IUserInterfaceReport _Report;
        SECSIni SECSSetting = new SECSIni();
        public QSACTIVEXLib.QSWrapper axQSWrapper1; //*new
        public QGACTIVEXLib.QGWrapper axQGWrapper1; //*new
        long g_lOperationResult;
        string strERROR_TEXT;

        public SECSGEM(IUserInterfaceReport Report)
        {
            
            Init();
            _Report = Report;
            // SECS Init
            axQGWrapper1 = new QGACTIVEXLib.QGWrapper(); //*new
            axQGWrapper1.QGEvent += new QGACTIVEXLib._IQGWrapperEvents_QGEventEventHandler(axQGWrapper1_QGEvent); //*new
            axQGWrapper1.PPEvent += new QGACTIVEXLib._IQGWrapperEvents_PPEventEventHandler(axQGWrapper1_PPEvent); //*new
            axQGWrapper1.TerminalMsgReceive += new QGACTIVEXLib._IQGWrapperEvents_TerminalMsgReceiveEventHandler(axQGWrapper1_TerminalMsgReceive); //*new


            axQSWrapper1 = new QSACTIVEXLib.QSWrapper(); //*new
            axQSWrapper1.QSEvent += new QSACTIVEXLib._IQSWrapperEvents_QSEventEventHandler(axQSWrapper1_QSEvent); //*new
            //' SECS-I Parameters
            axQSWrapper1.T1 = float.Parse(SECSSetting.SECSI.T1);
            axQSWrapper1.T2 = float.Parse(SECSSetting.SECSI.T2);
            axQSWrapper1.T4 = int.Parse(SECSSetting.SECSI.T4);
            axQSWrapper1.lBaudRate = int.Parse(SECSSetting.SECSI.BaudRate);
            axQSWrapper1.lCOMPort = int.Parse(SECSSetting.SECSI.ComPort);
            if (SECSSetting.SECSI.Role.Equals("Host"))
            {
                axQSWrapper1.SECS_Connect_Mode = SECS_COMM_MODE.SECS_HOST_MODE;
            }
            else
            {
                axQSWrapper1.SECS_Connect_Mode = SECS_COMM_MODE.SECS_EQUIP_MODE;
            }

            //' HSMS-SS Parameters
            axQSWrapper1.T5 = int.Parse(SECSSetting.HSMS.T5);
            axQSWrapper1.T6 = int.Parse(SECSSetting.HSMS.T6);
            axQSWrapper1.T7 = int.Parse(SECSSetting.HSMS.T7);
            axQSWrapper1.T8 = int.Parse(SECSSetting.HSMS.T8);
            axQSWrapper1.lLinkTestPeriod = int.Parse(SECSSetting.HSMS.LinkTestPeriod);
            axQSWrapper1.szLocalIP = SECSSetting.HSMS.LocalIp;
            axQSWrapper1.nLocalPort = int.Parse(SECSSetting.HSMS.LocalPort);
            axQSWrapper1.szRemoteIP = SECSSetting.HSMS.RemoteIp;
            axQSWrapper1.nRemotePort = int.Parse(SECSSetting.HSMS.RemotePort);
            if (SECSSetting.HSMS.Role.Equals("Active"))
            {
                axQSWrapper1.HSMS_Connect_Mode = HSMS_COMM_MODE.HSMS_ACTIVE_MODE;
            }
            else
            {
                axQSWrapper1.HSMS_Connect_Mode = HSMS_COMM_MODE.HSMS_PASSIVE_MODE;
            }
            //' Common Parameters
            axQSWrapper1.T3 = int.Parse(SECSSetting.HSMS.T3);
            axQSWrapper1.lDeviceID = int.Parse(SECSSetting.HSMS.DeviceID);
            if (SECSSetting.ConnectMode.Equals("HSMS"))
            {
                axQSWrapper1.lCOMM_Mode = COMMMODE.HSMS_MODE;
            }
            else
            {
                axQSWrapper1.lCOMM_Mode = COMMMODE.SECS_MODE;
            }

            axQSWrapper1.lLogEnable = 1;
            axQSWrapper1.lFlowControlEnable = 0;
            try
            {
                int state = axQSWrapper1.Initialize();

                string path = System.Environment.CurrentDirectory; //& "\.."
                state = axQGWrapper1.Initialize(path);

                object objVal = "EqMDLN";
                int lResult = axQGWrapper1.UpdateSV((int)GemSystemID.GEM_MDLN, ref objVal);

                objVal = "SofRev";
                lResult = axQGWrapper1.UpdateSV((int)GemSystemID.GEM_SOFTREV, ref objVal);

                //*********************************************************************************************

                state = axQGWrapper1.EnableComm();
                state = axQSWrapper1.Start();

            }
            catch (Exception ex)
            {

                logger.Error(ex.StackTrace);
            }

        }



        public bool OnlieReq()
        {
            string path;
            long lResult;
            object objVal;
            //int iLength;
            //long lSVID;
            //byte bCOMM_Mode;
            //long lOperationResult;

            path = System.Environment.CurrentDirectory; //& "\.."
            int lQGInitResult = axQGWrapper1.Initialize(path);

            //lbl_ErrorCode.Text = lQGInitResult.ToString();
            if (lQGInitResult == 0)
            {
                //lbl_ErrorCode.BackColor = Color.GreenYellow;
                //AppendText("QuickGEM Initiation success." + "\r\n");
            }
            else
            {
                //lbl_ErrorCode.BackColor = Color.Red;
                //AppendText("QuickGEM Initiation error." + "\r\n");
                //MessageBox.Show("QuickGEM Initiation error. Press any key to close program !");
                //lQGInitResult = -2; //initial failed
                //Environment.Exit(0);
                return false;
            }

            objVal = "EqMDLN";
            lResult = axQGWrapper1.UpdateSV((int)GemSystemID.GEM_MDLN, ref objVal);

            objVal = "SofRev";
            lResult = axQGWrapper1.UpdateSV((int)GemSystemID.GEM_SOFTREV, ref objVal);



            return true;
        }



        public void OffLine()
        {
            int state = axQGWrapper1.DisableComm();
            state = axQSWrapper1.Stop();
        }

        private void Init()
        {
            string strFile = "Config/SECSConfig.ini";

            if (File.Exists(strFile))
            {
                TIniFile iniFile = new TIniFile(strFile);

                SECSSetting.HSMS.T3 = iniFile.ReadString("HSMS", "T3", "");
                SECSSetting.HSMS.T5 = iniFile.ReadString("HSMS", "T5", "");
                SECSSetting.HSMS.T6 = iniFile.ReadString("HSMS", "T6", "");
                SECSSetting.HSMS.T7 = iniFile.ReadString("HSMS", "T7", "");
                SECSSetting.HSMS.T8 = iniFile.ReadString("HSMS", "T8", "");
                SECSSetting.HSMS.LocalIp = iniFile.ReadString("HSMS", "LocalIp", "");
                SECSSetting.HSMS.LocalPort = iniFile.ReadString("HSMS", "LocalPort", "");
                SECSSetting.HSMS.RemoteIp = iniFile.ReadString("HSMS", "RemoteIp", "");
                SECSSetting.HSMS.RemotePort = iniFile.ReadString("HSMS", "RemotePort", "");
                SECSSetting.HSMS.LinkTestPeriod = iniFile.ReadString("HSMS", "LinkTestPeriod", "");
                SECSSetting.HSMS.Role = iniFile.ReadString("HSMS", "Role", "");
                SECSSetting.HSMS.DeviceID = iniFile.ReadString("HSMS", "DeviceID", "");

                SECSSetting.SECSI.T1 = iniFile.ReadString("SECS-I", "T1", "");
                SECSSetting.SECSI.T2 = iniFile.ReadString("SECS-I", "T2", "");
                SECSSetting.SECSI.T3 = iniFile.ReadString("SECS-I", "T3", "");
                SECSSetting.SECSI.T4 = iniFile.ReadString("SECS-I", "T4", "");
                SECSSetting.SECSI.Rty = iniFile.ReadString("SECS-I", "Rty", "");
                SECSSetting.SECSI.BaudRate = iniFile.ReadString("SECS-I", "BaudRate", "");
                SECSSetting.SECSI.ComPort = iniFile.ReadString("SECS-I", "ComPort", "");
                SECSSetting.SECSI.Role = iniFile.ReadString("SECS-I", "Role", "");
                SECSSetting.SECSI.DeviceID = iniFile.ReadString("SECS-I", "DeviceID", "");

                SECSSetting.ConnectMode = iniFile.ReadString("ConnectMode", "ConnectMode", "");
                iniFile.Close();
            }
        }

        private void axQGWrapper1_QGEvent(int lID, int S, int F, int W_Bit, int SystemBytes, object RawData, int Length) //*new
        {
            //ShowSECSIIMessage RawData
            axQSWrapper1.SendSECSIIMessage(S, F, W_Bit, ref SystemBytes, RawData);
            //AppendText("QuickGEM Send Request ===>" + "\r\n");
        }

        //******************************************************************************************
        //* QuickSECS Event Call Back Area
        //******************************************************************************************
        //private void axQSWrapper1_QSEvent(object sender, AxQSACTIVEXLib._IQSWrapperEvents_QSEventEvent e) //*old
        private void axQSWrapper1_QSEvent(int lID, EVENT_ID lMsgID, int S, int F, int W_Bit, int ulSystemBytes, object RawData, object Head, string pEventText) //*new
        {
            QGACTIVEXLib.PROCESS_MSG_RESULT lResult = axQGWrapper1.ProcessMessage((int)lMsgID, S, F, W_Bit, ulSystemBytes, ref RawData, ref Head, pEventText);

            if (lResult == QGACTIVEXLib.PROCESS_MSG_RESULT.REPLY_BY_AP && S == 2 && F == 41)
            {
                switch (remoteCmd.GetCommandName())
                {
                    case "MOVE_FOUP":
                        string Source = remoteCmd.GetCPValue("SOURCE");
                        string Destination = remoteCmd.GetCPValue("DESTINATION");
                        Dictionary<string, string> param = new Dictionary<string, string>();
                        param.Add("@Target", "ROBOT01");
                        _Report.NewTask(ulSystemBytes.ToString(), TaskFlowManagement.Command.ROBOT_HOME, param);

                        break;
                }
            }
            else if (lMsgID == QSACTIVEXLib.EVENT_ID.QS_EVENT_CONNECTED)
            {
                _Report.On_Message_Log("SECS", "CONNECTED");
            }
            else if (lMsgID == QSACTIVEXLib.EVENT_ID.QS_EVENT_DISCONNECTED)
            {
                _Report.On_Message_Log("SECS", "DISCONNECTED");
                axQSWrapper1.Start();
            }
        }

        enum Hack
        {
            Command_has_been_performed = 0,
            Command_does_not_exist=1,
            Cannot_perform_now=2
        }
        class RemoteCommand
        {
            private string _CommandName { get; set; }
            private Dictionary<string, string> CP = new Dictionary<string, string>();
            private string _cpName { get; set; }
            public void SetCommandName(string CommandName)
            {
                _CommandName = CommandName;
            }
            public string GetCommandName()
            {
                return _CommandName;
            }
            public void SetCPName(string CPName)
            {
                _cpName = CPName.ToUpper();
            }
            public void SetCPValue(string CPValue)
            {
                CP.Add(_cpName, CPValue.ToUpper());
            }
            public string GetCPValue(string CPName)
            {
                string result = "";
                CP.TryGetValue(CPName,out result);
                return result;
            }
        }
        RemoteCommand remoteCmd;
        private void axQGWrapper1_PPEvent(QGACTIVEXLib.PP_TYPE MsgID, string PPID)
        {
            switch (MsgID)
            {
                case QGACTIVEXLib.PP_TYPE.RECEIVE_S2F41_RCMD:
                    remoteCmd = new RemoteCommand();
                    remoteCmd.SetCommandName(PPID);
                    break;
                case QGACTIVEXLib.PP_TYPE.RECEIVE_S2F41_RCMD_END:
                    
                    break;
                case QGACTIVEXLib.PP_TYPE.RECEIVE_S2F41_CPNAME:
                    remoteCmd.SetCPName(PPID);
                    break;
                case QGACTIVEXLib.PP_TYPE.RECEIVE_S2F41_CPVAL:
                    remoteCmd.SetCPValue(PPID);
                    break;
                case QGACTIVEXLib.PP_TYPE.RECEIVE_S2F41_ERROR_RCMD:
                    
                    break;
            }
        }

       

        private void axQGWrapper1_TerminalMsgReceive(string Message)//*new
        {

        }

        struct SECSIni
        {
            public secsi SECSI;
            public hsms HSMS;
            public string ConnectMode;
        }

        struct secsi
        {
            public string T1;
            public string T2;
            public string T3;
            public string T4;
            public string BaudRate;
            public string ComPort;
            public string Rty;
            public string Role;
            public string DeviceID;
        }

        struct hsms
        {
            public string T3;
            public string T5;
            public string T6;
            public string T7;
            public string T8;
            public string LinkTestPeriod;
            public string LocalIp;
            public string LocalPort;
            public string RemoteIp;
            public string RemotePort;
            public string Role;
            public string DeviceID;
        }


      

       
        //*******************************************************************************************************************
        public void On_TaskJob_Finished(TaskFlowManagement.CurrentProcessTask Task)
        {
            string TargetName = "";

            if (Task.Params.TryGetValue("Target", out TargetName))
            {
                Node Target = NodeManagement.Get(TargetName);
                if (Target != null)
                {

                    switch (Target.Type.ToUpper())
                    {
                        case "LOADPORT":
                            
                            break;
                    }
                }
                else
                {
                    logger.Error("On_TaskJob_Finished err: Target " + TargetName + " not in Node list.");
                }
            }
            else
            {
                logger.Error("On_TaskJob_Finished err: Target not in Param.");
            }

        }

        public void On_TaskJob_Ack(TaskFlowManagement.CurrentProcessTask Task)
        {
            ReplyAck(Hack.Command_has_been_performed, Convert.ToInt32(Task.Id));
        }

        public void On_TaskJob_Aborted(TaskFlowManagement.CurrentProcessTask Task, string NodeName, string ReportType, string Message)
        {
            ReplyAck(Hack.Cannot_perform_now, Convert.ToInt32(Task.Id));
            //更新SV
            object objTemp = (object)4;
            g_lOperationResult = axQGWrapper1.UpdateSV((int)EqpID.SV_EQP_STATUS, ref objTemp);
            //發Event report
            axQGWrapper1.EventReportSend(EqpID.EQP_STATUS_TO_IDLE);
        }

        private void ReplyAck(Hack hack, int SystemBytes )
        {
            object OutputRawData = null;
            long[] lTemp = new long[2];
            object varRawData = null;
            object Value = null;
            axQSWrapper1.DataItemOut(ref OutputRawData, 2, QSACTIVEXLib.SECSII_DATA_TYPE.LIST_TYPE, ref Value);
            //           <B HCACK> 
            lTemp[0] = (long)hack;
            varRawData = lTemp;
            axQSWrapper1.DataItemOut(ref OutputRawData, 1, QSACTIVEXLib.SECSII_DATA_TYPE.BINARY_TYPE, ref varRawData);

            axQSWrapper1.DataItemOut(ref OutputRawData, 0, QSACTIVEXLib.SECSII_DATA_TYPE.LIST_TYPE, ref varRawData);
            axQSWrapper1.SendSECSIIMessage(2, 42, 0, ref SystemBytes, OutputRawData);
        }
        private void SendAlarmMsg(long ALCD, long ALID, string ALTX)
        {
            int SystemBytes;
            object RawData;
            long[] myALCD = new long[1];
            long[] myALID = new long[1];
            object Data;

            Data = null;
            RawData = null;
            SystemBytes = 0;

            axQSWrapper1.DataItemOut(ref RawData, 3, QSACTIVEXLib.SECSII_DATA_TYPE.LIST_TYPE, ref Data);

            myALCD[0] = ALCD;
            Data = myALCD;
            axQSWrapper1.DataItemOut(ref RawData, 1, QSACTIVEXLib.SECSII_DATA_TYPE.BINARY_TYPE, ref Data);

            myALID[0] = ALID;
            Data = myALID;
            axQSWrapper1.DataItemOut(ref RawData, 1, QSACTIVEXLib.SECSII_DATA_TYPE.INT_4_TYPE, ref Data);

            Data = ALTX;
            axQSWrapper1.DataItemOut(ref RawData, ALTX.Length, QSACTIVEXLib.SECSII_DATA_TYPE.ASCII_TYPE, ref Data);

            axQSWrapper1.SendSECSIIMessage(5, 1, 1, ref SystemBytes, RawData);
        }
        public void On_Command_Excuted(Node Node, Transaction Txn, CommandReturnMessage Msg)
        {
            
        }

        public void On_Command_Error(Node Node, Transaction Txn, CommandReturnMessage Msg)
        {
            
        }

        public void On_Command_Finished(Node Node, Transaction Txn, CommandReturnMessage Msg)
        {
            
        }

        public void On_Command_TimeOut(Node Node, Transaction Txn)
        {
            
        }

        public void On_Event_Trigger(Node Node, CommandReturnMessage Msg)
        {
            
        }

        public void On_Node_State_Changed(Node Node, string Status)
        {
            
        }

        public void On_Node_Connection_Changed(string NodeName, string Status)
        {
            
        }

        public void On_Job_Location_Changed(Job Job)
        {
            
        }

        public void On_DIO_Data_Chnaged(string Parameter, string Value, string Type)
        {
            
        }

        public void On_Connection_Error(string DIOName, string ErrorMsg)
        {
            
        }

        public void On_Connection_Status_Report(string DIOName, string Status)
        {
           
        }

        public void On_Alarm_Happen(AlarmInfo Alarm)
        {
            SendAlarmMsg((long)Math.Pow(2,7),999, Alarm.EngDesc);
        }

        

        public void On_Message_Log(string Type, string Message)
        {
            
        }

        public void On_Status_Changed(string Type, string Message)
        {
            
        }

        public void NewTask(string Id, TaskFlowManagement.Command TaskName, Dictionary<string, string> param = null)
        {
           
        }
    }
}
