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

            if(lResult == QGACTIVEXLib.PROCESS_MSG_RESULT.REPLY_BY_AP && S == 2 && F == 41)
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

        private void ExeRCMD_Reply(string RCMD, int SystemBytes, int iHCACK, int iCPACK1, int iCPACK2)
        {
            object OutputRawData = null;
            long[] lTemp = new long[2];
            object varRawData = null;
            int i = 0;
            object Temp = null;
            object Value = null;
            //        S2F42 
            //        <L[2] 
            //           <B HCACK> 
            //           <L[2] 
            //               <L[2] 
            //                   <A "PPID"> 
            //                   <B[1] CPACK1> 
            //               > 
            //               <L[2] 
            //                   <A "PORTID"> 
            //                   <B[1] CPACK2> 
            //               > 
            //           > 
            //        >. 

            axQSWrapper1.DataItemOut(ref OutputRawData, 2, QSACTIVEXLib.SECSII_DATA_TYPE.LIST_TYPE, ref Value);
            //           <B HCACK> 
            lTemp[0] = iHCACK;
            varRawData = lTemp;
            axQSWrapper1.DataItemOut(ref OutputRawData, 1, QSACTIVEXLib.SECSII_DATA_TYPE.BINARY_TYPE, ref varRawData);
            i = 0;
            if (iCPACK1 > 0)
            {
                i = i + 1;
            }
            if (iCPACK2 > 0)
            {
                i = i + 1;
            }
            if (i == 0)
            {
                axQSWrapper1.DataItemOut(ref OutputRawData, 0, QSACTIVEXLib.SECSII_DATA_TYPE.LIST_TYPE, ref varRawData);
            }
            else
            {
                axQSWrapper1.DataItemOut(ref OutputRawData, i, QSACTIVEXLib.SECSII_DATA_TYPE.LIST_TYPE, ref Value);
                switch (RCMD)
                {
                    case "PP-SELECT":
                        if (iCPACK1 > 0)
                        {
                            axQSWrapper1.DataItemOut(ref OutputRawData, 2, QSACTIVEXLib.SECSII_DATA_TYPE.LIST_TYPE, ref Value);
                            Value = "PPID".ToString();
                            axQSWrapper1.DataItemOut(ref OutputRawData, Value.ToString().Length, QSACTIVEXLib.SECSII_DATA_TYPE.ASCII_TYPE, ref Value);
                            lTemp[0] = iCPACK1;
                            Temp = lTemp;
                            axQSWrapper1.DataItemOut(ref OutputRawData, 1, QSACTIVEXLib.SECSII_DATA_TYPE.BINARY_TYPE, ref Temp);
                        }
                        if (iCPACK2 > 0)
                        {
                            axQSWrapper1.DataItemOut(ref OutputRawData, 2, QSACTIVEXLib.SECSII_DATA_TYPE.LIST_TYPE, ref Value);
                            Value = "PortID".ToString();
                            axQSWrapper1.DataItemOut(ref OutputRawData, Value.ToString().Length, QSACTIVEXLib.SECSII_DATA_TYPE.ASCII_TYPE, ref Value);
                            lTemp[0] = iCPACK2;
                            Temp = lTemp;
                            axQSWrapper1.DataItemOut(ref OutputRawData, 1, QSACTIVEXLib.SECSII_DATA_TYPE.BINARY_TYPE, ref Temp);
                        }
                        break;
                    case "START":
                        if (iCPACK1 > 0)
                        {
                            axQSWrapper1.DataItemOut(ref OutputRawData, 2, QSACTIVEXLib.SECSII_DATA_TYPE.LIST_TYPE, ref Value);
                            axQSWrapper1.DataItemOut(ref OutputRawData, 2, QSACTIVEXLib.SECSII_DATA_TYPE.LIST_TYPE, ref Value);
                            Value = "PortID".ToString();
                            axQSWrapper1.DataItemOut(ref OutputRawData, Value.ToString().Length, QSACTIVEXLib.SECSII_DATA_TYPE.ASCII_TYPE, ref Value);
                            lTemp[0] = iCPACK1;
                            Temp = lTemp;
                            axQSWrapper1.DataItemOut(ref OutputRawData, 1, QSACTIVEXLib.SECSII_DATA_TYPE.BINARY_TYPE, ref Temp);
                        }
                        break;
                    case "STOP":
                        break;
                    default:
                        break;   //Error ...  
                }
            }
            axQSWrapper1.SendSECSIIMessage(2, 42, 0, ref SystemBytes, OutputRawData);

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



        // E87 data -----------------------------------------


        public struct CarrierCommand
        {
            //Command Data
            public int Command;
            public string CommandStr;
            public string CarrierID;
            public int PTN;
            public int Capacity;
            public int CapacityExistFlag;

            public int SubstrateCount;
            public int SubstrateCountExistFlag;

            public string Usage;
            public int UsageExistFlag;

            public int ContentMapNumber;
            public int ContentMapExistFlag;

            public int SlotMapNumber;
            public int SlotMapExistFlag;

            public string[] LotID;
            public string[] SubStID;
            public int[] SlotMap;

            public CarrierCommand(int p1)
            {
                Command = 0;
                CommandStr = "";
                CarrierID = "";
                PTN = 0;
                Capacity = 0;
                CapacityExistFlag = 0;
                SubstrateCount = 0;
                SubstrateCountExistFlag = 0;
                Usage = "";
                UsageExistFlag = 0;
                ContentMapNumber = 0;
                ContentMapExistFlag = 0;
                SlotMapNumber = 0;
                SlotMapExistFlag = 0;

                LotID = new string[0];
                SubStID = new string[0];
                SlotMap = new int[0];
            }
        }

        CarrierCommand CarrierCommand1 = new CarrierCommand();
        //  300mm config define =========================================================== 
        //E40
        public const int CFG_PJ_CMD_START_SUPPORT = 0;
        public const int CFG_PJ_CMD_PAUSE_SUPPORT = 0;
        public const int PJ_CARRIER_MAX_NUMBER = 8;   // for GEM driver , max is 8

        //public const int CFG_PJ_CMD_STOP_SUPPORT      = 0;
        //public const int CFG_PJ_CMD_ABORT_SUPPORT     = 0;
        public const int CFG_PJ_CMD_STOP_SUPPORT = 1;
        public const int CFG_PJ_CMD_ABORT_SUPPORT = 1;

        public const int CFG_PJ_CMD_CANCEL_SUPPORT = 0;

        public const int CFG_PJ_AUTO_START_SUPPORT = 1;
        public const int CFG_PJ_MANUAL_START_SUPPORT = 0;

        //E87
        public const int CARRIER_LOC_LP1 = 1;
        public const int CARRIER_LOC_LP2 = 2;

        //E90
        public const int LIST_2 = 2;

        //E94
        public const int CFG_CJ_CMD_START_SUPPORT = 0; //0
        public const int CFG_CJ_CMD_PAUSE_SUPPORT = 0; //0
        public const int CFG_CJ_CMD_RESUME_SUPPORT = 0; //0
        public const int CFG_CJ_CMD_CANCEL_SUPPORT = 0; //0
        public const int CFG_CJ_CMD_DSELECT_SUPPORT = 0; //0
        public const int CFG_CJ_CMD_STOP_SUPPORT = 1; //1
        public const int CFG_CJ_CMD_ABORT_SUPPORT = 1; //1
        public const int CFG_CJ_CMD_HOQ_SUPPORT = 0; //0

        // == E30 Standard Const definition =================================================
        // GEM_EQP_ST is for GEM use only, not for Eqp.
        public const int GEM_EQP_ST_0_IDLE = 0;
        public const int GEM_EQP_ST_1_RUN = 1;
        public const int GEM_EQP_ST_2_PAUSED = 2;
        public const int GEM_EQP_ST_3_WARNING = 3;
        public const int GEM_EQP_ST_4_INITIAL = 4;
        public const int GEM_EQP_ST_5_ABORTING = 5;

        //-- E87 Command definition -------------------------------------------------
        public const int EPSM_CMD_NONE = 0;
        public const int EPSM_CMD_RESET = 1;
        //public const int EPSM_CMD_EXECUTING = 1;
        //public const int EPSM_CMD_PAUSE = 2;
        //public const int EPSM_CMD_ASSIST = 3;

        //== E87 Standard Const definition =================================================
        public const int LPTSM_0_OUT_OF_SERVICE = 0;
        public const int LPTSM_1_TRANSFER_BLOCKED = 1;
        public const int LPTSM_2_READY_TO_LOAD = 2;
        public const int LPTSM_3_READY_TO_UNLOAD = 3;

        public const int LPCASM_0_NOT_ASSOCIATED = 0;
        public const int LPCASM_1_ASSOCIATED = 1;

        public const int LPRSM_0_NOT_RESERVED = 0;
        public const int LPRSM_1_RESERVED = 1;

        public const int AMSM_0_MANUAL_MODE = 0;
        public const int AMSM_1_AUTO_MODE = 1;

        public const int CARRIER_PROCESS_CMD_CREATE = 1;

        public const int CARRIER_ID_STATUS_0_ID_NOT_READ = 0;
        public const int CARRIER_ID_STATUS_1_WAITING_FOR_HOST = 1;
        public const int CARRIER_ID_STATUS_2_ID_VERIFICATION_OK = 2;
        public const int CARRIER_ID_STATUS_3_ID_VERIFICATION_FAILED = 3;

        public const int CARRIER_SLOP_MAP_STATUS_0_SLOP_MAP_NOT_READ = 0;
        public const int CARRIER_SLOP_MAP_STATUS_1_WAITING_FOR_HOST = 1;
        public const int CARRIER_SLOP_MAP_STATUS_2_SLOT_MAP_VERIFICATION_OK = 2;
        public const int CARRIER_SLOP_MAP_STATUS_3_SLOT_MAP_VERIFICATION_FAILED = 3;

        public const int CARRIER_SLOP_MAP_0_UNDEFINED = 0;
        public const int CARRIER_SLOP_MAP_1_EMPTY = 1;
        public const int CARRIER_SLOP_MAP_2_NOT_EMPTY = 2;
        public const int CARRIER_SLOP_MAP_3_CORRECTLY_OCCUPIED = 3;
        public const int CARRIER_SLOP_MAP_4_DOUBLE_SLOTTED = 4;
        public const int CARRIER_SLOP_MAP_5_CROSS_SLOTTED = 5;

        public const int CARRIER_ACCESSING_STATUS_0_NOT_ACCESSED = 0;
        public const int CARRIER_ACCESSING_STATUS_1_IN_ACCESS = 1;
        public const int CARRIER_ACCESSING_STATUS_2_CARRIER_COMPLETE = 2;
        public const int CARRIER_ACCESSING_STATUS_3_CARRIER_STOPPED = 3;




        //-- E87 Command definition -------------------------------------------------
        public const int LPTSM_CMD_IN_SERVICE = 1;
        public const int LPTSM_CMD_OUT_SERVICE = 2;
        public const int LPTSM_CMD_LOAD_COMPLETE = 3;
        public const int LPTSM_CMD_UNLOAD_COMPLETE = 4;
        public const int LPTSM_CMD_READY_TO_LOAD = 5;
        public const int LPTSM_CMD_READY_TO_UNLOAD = 6;
        public const int LPTSM_CMD_RESET_OUT_SERVICE = 11;
        public const int LPTSM_CMD_RESET_READY_TO_LOAD = 12;
        public const int LPTSM_CMD_RESET_READY_TO_UNLOAD = 13;
        public const int LPTSM_CMD_RESET_BLOCK = 14;

        public const int LPCASM_CMD_NOT_ASSOCIATED = 1;
        public const int LPCASM_CMD_ASSOCIATED = 2;
        // 2.3.1
        public const int LPRSM_CMD_NOT_RESERVED = 1;
        public const int LPRSM_CMD_RESERVED = 2;

        public const int AMSM_CMD_CHANGE_TO_MANUAL = 1;
        public const int AMSM_CMD_CHANGE_TO_AUTO = 2;
        public const int AMSM_CMD_CHANGE_TO_MANUAL_ALL = 3;
        public const int AMSM_CMD_CHANGE_TO_AUTO_ALL = 4;

        public const int CSM_CMD_CARRIER_IS_INSTANTIATED = 1;
        public const int CSM_CMD_CARRIER_ID_READ_SUCCESS = 3;

        //**for LP_TAG_READ_FAILED
        public const int CSM_CMD_CARRIER_ID_READ_FAILED = 4;
        //**for LP_TAG_READ_FAILED
        // 2.3.1
        public const int CSM_CMD_CARRIER_ID_EQP_VERIFY_OK = 6;

        public const int CSM_CMD_PROCEED_WITH_CARRIER_RECEIVED = 8;
        public const int CSM_CMD_CANCEL_CARRIER_AT_PORT_RECEIVED = 9;
        public const int CSM_CMD_CANCEL_CARRIER = 10;
        public const int CSM_CMD_CARRIER_RELEASE = 11;
        // 2.3.1
        public const int CSM_CMD_SLOT_MAP_EQP_VERIFY_OK = 13;

        public const int CSM_CMD_SLOP_MAP_READ_SUCCESS = 14;
        //public const int CSM_CMD_PROCEED_WITH_CARRIER_RECEIVED = 15;
        public const int CSM_CMD_CANCEL_CARRIER_RECEIVED = 16;
        public const int CSM_CMD_CARRIER_START_PROCESS = 18;
        public const int CSM_CMD_CARRIER_PROCESS_COMPLETE = 19;
        public const int CSM_CMD_CARRIER_PROCESS_STOPPED = 20;

        public const int CSM_CMD_UNLOAD_COMPLETE = 21;

        public const int CSM_CMD_CARRIER_BIND_RECEIVED = 31;
        public const int CSM_CMD_CARRIER_CANCEL_BIND_RECEIVED = 32;
        public const int CSM_CMD_CARRIER_NOTIFYCATION_RECEIVED = 33;
        public const int CSM_CMD_CANCEL_CARRIER_NOTIFYCATION_RECEIVED = 34;
        public const int CSM_CMD_CARRIER_RECREATE_RECEIVED = 35;
        public const int CSM_CMD_RESERVE_AT_PORT_RECEIVED = 36;
        public const int CSM_CMD_CANCEL_RESERVATION_AT_PORT_RECEIVED = 37;
        public const int CSM_CMD_CARRIER_OUT_RECEIVED = 38;
        public const int CSM_CMD_CANCEL_ALL_CARRIER_OUT_RECEIVED = 39;
        public const int CSM_CMD_CARRIER_IN_RECEIVED = 40;


        public const string CAACK_0_ACK = "0";
        public const string CAACK_1_INVALID_COMMAND = "1";
        public const string CAACK_2_CAN_NOT_PERFORM_NOW = "2";
        public const string CAACK_3_INVALID_DATA = "3";
        public const string CAACK_4_ACK_LATER_BY_EVENT = "4";
        public const string CAACK_5_REJECT_INVALID_STATE = "5";
        public const string CAACK_6_COMMAND_PERFORMED_ERROR = "6";



        //== E40 Standard Const definition =================================================
        public const int PJSM_NOT_EXIST = -1;
        public const int PJSM_0_QUEUED = 0;
        public const int PJSM_1_SETTING_UP = 1;
        public const int PJSM_2_WAITING_FOR_START = 2;
        public const int PJSM_3_PROCESSING = 3;
        public const int PJSM_4_PROCESS_COMPLETE = 4;
        //Reserved:5
        public const int PJSM_6_PAUSING = 6;
        public const int PJSM_7_PAUSED = 7;
        public const int PJSM_8_STOPPING = 8;
        public const int PJSM_9_ABORTING = 9;
        public const int PJSM_10_STOPPED = 10;
        public const int PJSM_11_ABORTED = 11;

        public const int PROCESSSTART_0_MANUAL_START = 0;
        public const int PROCESSSTART_1_AUTO_START = 1;
        //-- E40 Command definition ----------------------------------------------------
        public const int PJSM_CMD_RESET_ALL = 0;
        public const int PJSM_CMD_ACCEPT_PJ_CREATE = 1;
        public const int PJSM_CMD_SETTING_UP = 2;
        public const int PJSM_CMD_SETTING_UP_COMPLETE = 3;
        public const int PJSM_CMD_START = 5;
        public const int PJSM_CMD_PROCESS_COMPLETE = 6;
        public const int PJSM_CMD_FOUP_REMOVED = 7;
        public const int PJSM_CMD_PAUSE = 8;
        public const int PJSM_CMD_RESUME = 10;
        public const int PJSM_CMD_STOP = 11;
        public const int PJSM_CMD_ABORT = 13;
        public const int PJSM_CMD_CANCEL = 18;
        public const int PJSM_CMD_RESET = 20;  // Reset one PJ

        //2015/07/19
        public const int PJSM_CMD_ABORT_BY_EQUIPMENT = 19;

        public const int PJSM_CMD_WAFER_1ST_START = 1;
        public const int PJSM_CMD_WAFER_PROCESS_END = 2;

        public const int F11 = 11;
        public const int F15 = 15;

        public const int PROCESS_START_0_MANUAL_START = 0;
        public const int PROCESS_START_1_AUTO_START = 1;

        //== E90 Standard Const definition =================================================
        public const int ST_SUBSTRATE_STATE_0_AT_SOURCE = 0;
        public const int ST_SUBSTRATE_STATE_1_AT_WORK = 1;
        public const int ST_SUBSTRATE_STATE_2_AT_DESTINATION = 2;

        public const int ST_SUBSTRATE_PROCESS_STATE_0_NEEDS_PROCESSING = 0;
        public const int ST_SUBSTRATE_PROCESS_STATE_1_IN_PROCESS = 1;
        public const int ST_SUBSTRATE_PROCESS_STATE_2_PROCESSED = 2;
        public const int ST_SUBSTRATE_PROCESS_STATE_3_ABORTED = 3;
        public const int ST_SUBSTRATE_PROCESS_STATE_4_STOPPED = 4;
        public const int ST_SUBSTRATE_PROCESS_STATE_5_REJECTED = 5;
        public const int ST_SUBSTRATE_PROCESS_STATE_6_LOST = 5;

        public const int ST_SUBSTRATE_LOCATION_STATED_0_UNOCCUPIED = 0;
        public const int ST_SUBSTRATE_LOCATION_STATED_1_OCCUPIED = 1;

        //-- E90 Command definition ----------------------------------------------------

        public const int STST_CMD_AT_SOURCE = 0;
        public const int STST_CMD_AT_WORK = 1;
        public const int STST_CMD_AT_DESTINATION = 2;

        public const int STST_CMD_UNOCCUPIED = 0;
        public const int STST_CMD_OCCUPIED = 1;

        //== E94 Standard Const definition =================================================
        public const int CJSM_NOT_EXIST = -1;
        public const int CJSM_0_QUEUED = 0;
        public const int CJSM_1_SELECTED = 1;
        public const int CJSM_2_WAITING_FOR_START = 2;
        public const int CJSM_3_EXECUTING = 3;
        public const int CJSM_4_PAUSED = 4;
        public const int CJSM_5_COMPLETED = 5;


        public const int START_METHOD_0_USER_START = 0;
        public const int START_METHOD_1_AUTO_START = 1;


        //-- E94 Command definition ----------------------------------------------
        // note: CJ_Command 和 CJSM_CMD 編號不能重覆
        public const int CJ_Command_1_CJStart = 1;
        public const int CJ_Command_2_CJPause = 2;
        public const int CJ_Command_3_CJResume = 3;
        public const int CJ_Command_4_CJCancel = 4;
        public const int CJ_Command_5_CJDeselect = 5;
        public const int CJ_Command_6_CJStop = 6;
        public const int CJ_Command_7_CJAbort = 7;
        public const int CJ_Command_8_CJHOQ = 8;

        public const int CJ_Command_9_CJAbortByEquipment = 9;

        public const int CJSM_CMD_RESET = 0;
        public const int CJSM_CMD_ACCEPT_CJ_CREATE = 11;
        public const int CJSM_CMD_PJ_COMPLETE = 12;
        public const int CJSM_CMD_PJ_NOT_EXIST = 13;
        public const int CJSM_CMD_CJ_DELETE = 14;

        //=======================================================
        //ERRCODE
        //0 = No error
        //1 = Unknown object in Object Specifier
        //2 = Unknown target object type
        //3 = Unknown object instance
        //4 = Unknown attribute name
        //5 = Read-only attribute - access denied
        //6 = Unknown object type
        public const int ERRCODE_7_INVALID_ATTR_VALUE = 7;
        //8 = Syntax error
        public const int ERRCODE_9_VERIFICATION_ERROR = 9;
        //10 = Validation error
        //11 = Object identifier in use
        //12 = Parameters improperly specified
        public const int ERRCODE_13_INSUFFICIENT_PARAMETERS = 13;
        //14 = Unsupported option requested
        //15 = Busy
        public const int ERRCODE_16_NOT_AVAILABALE_FOR_PROCESSING = 16;
        public const int ERRCODE_17_COMMAND_NOT_VALID = 17;             //17 = Command not valid for current state
        //18 = No material altered
        //19 = Material partially processed
        //20 = All material processed
        //21 = Recipe specification related error
        //22 = Failed during processing
        //23 = Failed while not processing
        //24 = Failed due to lack of material
        //25 = Job aborted
        //26 = Job stopped
        //27 = Job canceled
        //28 = Cannot change selected recipe
        //29 = Unknown event
        //30 = Duplicate report ID
        //31 = Unknown data report
        //32 = Data report not linked
        //33 = Unknown trace report
        //34 = Duplicate trace ID
        //35 = Too many data reports
        //36 = Sample period out of range
        //37 = Group size to large
        //38 = Recovery action currently invalid
        //39 = Busy with another recovery currently unable to perform the recovery
        //40 = No active recovery action
        //41 = Exception recovery failed
        //42 = Exception recovery aborted
        //43 = Invalid table element
        //44 = Unknown table element
        //45 = Cannot delete predefined
        //46 = Invalid token
        //47 = Invalid parameter
        //48 = Load port does not exist
        //49 = Load port already in use
        //50 = Missing Carrier
        //51-63 = Reserved
        //64-32767 = User defined
        //32768-65535 = Reserved
        //65536 or above = User defined
        //E87
        public const string ERRTEXT_CANT_PERFORM_AT_ONLINE_LOCAL = "Can not perform at On Line Local Mode !";
        public const string ERRTEXT_EQP_NOT_AT_RUN_STATE = "Eqp. Status not at run state error !";
        public const string ERRTEXT_EQP_NOT_AT_IDLE_STATE = "Eqp. Status not at idle state error !";
        public const string ERRTEXT_CARRIER_ID_NOT_EXIST = "Carried ID not exist error !";
        public const string ERRTEXT_CARRIER_ID_ALREADY_EXIST = "Carried ID already exist error !";
        public const string ERRTEXT_CARRIER_ID_STATUS_ERROR = "Carried ID Status error !";
        public const string ERRTEXT_SLOT_MAP_STATUS_ERROR = "Slot Map Status error !";
        public const string ERRTEXT_PORT_ID_NOT_EXIST = "PTN not exist error !";
        public const string ERRTEXT_PORT_ID_NOT_MATCH = "PTN not match error !";
        public const string ERRTEXT_MISS_CAPACITY_ATTR = "Miss Capacity attribute error !";
        public const string ERRTEXT_PORT_ID_ERROR = "Pord ID error !";
        public const string ERRTEXT_LOAD_PORT_NOT_IN_READY_STATE = "Load port not in READY TO LOAD state error !";
        public const string ERRTEXT_LOAD_PORT_NOT_IN_NOT_ASSOCIATED_STATE = "Load port not in NOT ASSOCIATED state error !";
        public const string ERRTEXT_LOAD_PORT_NOT_IN_NOT_RESERVED_STATE = "Load port not in NOT RESERVED state error !";

        public string ERRTEXT_CAPACITY_ERROR = "Capacity value error (need " + Common.EQP_Data.SlotSpace.ToString() + ")!";
        public const string ERRTEXT_MISS_CONTENT_MAP_ATTR = "Miss ContentMap attribute error !";
        public string ERRTEXT_CONTENT_MAP_ERROR = "ContentMap List error (need " + Common.EQP_Data.SlotSpace.ToString() + ")!";
        public const string ERRTEXT_MISS_SLOT_MAP_ATTR = "Miss SlotMap attribute error !";
        public const string ERRTEXT_ERROR_SLOT_MAP_ATTR = "Error SlotMap attribute error !";
        public const string ERRTEXT_CARRIER_FULL_ERROR = "Cartier full error !";

        //**Process_E87_S3F17_CancleCarrierAtPort after CARRIER_ID_STATUS_1_WAITING_FOR_HOST
        public const string ERRTEXT_CANNOT_EXECUTE_CANCEL_CARRIER_AT_PORT_ERROR = "Can not perform CancelCarrierAtPort after E87_WAITING_FOR_HOST state!";
        //**Process_E87_S3F17_CancleCarrierAtPort after CARRIER_ID_STATUS_1_WAITING_FOR_HOST

        public const string ERRTEXT_CANT_PERFORM_WHEN_LOAD_PORT_EMPTY = "Can not perform when load port is empty!";

        //2015/07/05
        //**Process_E87_S3F17_CarrierRelease
        public string ERRTEXT_CANNOT_EXECUTE_CARRIER_RELEASE_ERROR = "Carrier accessing status is not at E87_CARRIER_COMPLETE state!";
        public string ERRTEXT_ALREADY_CARRIER_RELEASE_BY_EC_SET_AUTO_RELEASE_ERROR = "Carrier has performed CarrierRelease command by EC set to auto release!";
        //**Process_E87_S3F17_CarrierRelease

        //**Allow NULL Sutstrate ID setting
        public const string ERRTEXT_NULL_SUBSTRATE_ID_ATTR = "Substrate ID null attribute error !";
        //**Allow NULL Sutstrate ID setting

        //E40
        public const string ERRTEXT_RECIPE_NOT_EXIST_ERROR = "Recipe file not exist error !";
        public const string ERRTEXT_SLOT_NO_WAFER_ERROR = "Specified slot no wafer error !";
        public const string ERRTEXT_SLOT_INVALID_SLOT_NUMBER_ERROR = "Invalid slot number error !";
        public const string ERRTEXT_PJ_FULL_ERROR = "Process job full error !!";
        public const string ERRTEXT_PJ_ID_NOT_EXIST = "Process job ID not exist error !";
        public const string ERRTEXT_MANUAL_START_NOT_SUPPORT_ERROR = "Manual start not support error !";
        public const string ERRTEXT_AUTO_START_NOT_SUPPORT_ERROR = "Auto start not support error !";
        public const string ERRTEXT_PJ_COMMAND_NOT_SUPPORT_ERROR = "Process job command not support error !";
        public const string ERRTEXT_PJ_COMMAND_CON_NOT_PROCESS_ERROR = "Process job command can not process error !";

        //2015/07/26
        public const string ERRTEXT_DUPLICATE_PJ_ID_ERROR = "Process job ID already exist error !";

        //E94
        public const string ERRTEXT_CJ_COMMAND_NOT_SUPPORT_ERROR = "Control job command not support error !";
        public const string ERRTEXT_CJ_ID_NOT_EXIST = "Control job ID not exist error !";
        public const string ERRTEXT_CJ_COMMAND_CON_NOT_PROCESS_ERROR = "Control job command can not process error !";

        //*******************************************************************************************************************
        //2015/07/26
        public const string ERRTEXT_CJ_FULL_ERROR = "Control job full error !!";
        public const string ERRTEXT_CJ_MANUAL_START_NOT_SUPPORT_ERROR = "Control job manual start not support error !";
        public const string ERRTEXT_CJ_CARRIER_INPUT_SPEC_NUMBER_ERROR = "Control job CarrierInputSpec quantity not match error !";
        public const string ERRTEXT_CJ_MTRL_OUT_SPEC_NUMBER_ERROR = "Control job MtrlOutSpec quantity not match error !";
        //*******************************************************************************************************************
        public void On_TaskJob_Finished(TaskFlowManagement.CurrentProcessTask Task)
        {
            string TargetName = "";
            object objTemp;

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
