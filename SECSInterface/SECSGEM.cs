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
    public class SECSGEM : IUserInterfaceReport
    {
        static ILog logger = LogManager.GetLogger(typeof(SECSGEM));
        IUserInterfaceReport _Report;
        SECSIni SECSSetting = new SECSIni();
        public QSACTIVEXLib.QSWrapper axQSWrapper1; //*new
        public QGACTIVEXLib.QGWrapper axQGWrapper1; //*new
        long g_lOperationResult;
        string strERROR_TEXT;
        Dictionary<string, string> IO_State = new Dictionary<string, string>();
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

                ReportEvent(SECS_SV.STOCKER_STATE, SECS_SV.STOCKER_STATE_PREV, SECS_Event.Stocker_State_Change, "RESET_REQUIRED");
                ReportEvent(SECS_SV.WTS_STATE, SECS_SV.WTS_STATE_PREV, SECS_Event.WTS_State_Change, "RESET_REQUIRED");
                ReportEvent(SECS_DV.ELPT1_FOUP_ID, SECS_DV.ELPT1_FOUP_ID_PREV, SECS_Event.ELPT1_ID_Available, "UNKNOWN");
                ReportEvent(SECS_DV.ELPT2_FOUP_ID, SECS_DV.ELPT2_FOUP_ID_PREV, SECS_Event.ELPT2_ID_Available, "UNKNOWN");
                ReportEvent(SECS_DV.ILPT1_MAP, SECS_DV.ILPT1_MAP_PREV, SECS_Event.ILPT1_Wafer_Map_Available, "UNKNOWN");
                ReportEvent(SECS_DV.ILPT2_MAP, SECS_DV.ILPT2_MAP_PREV, SECS_Event.ILPT2_Wafer_Map_Available, "UNKNOWN");
                ReportEvent(SECS_SV.ILPT1_FOUP_STATE, SECS_SV.ILPT1_FOUP_STATE_PREV, SECS_Event.ILPT1_FOUP_State_Change, "UNKNOWN");
                ReportEvent(SECS_SV.ILPT2_FOUP_STATE, SECS_SV.ILPT2_FOUP_STATE_PREV, SECS_Event.ILPT2_FOUP_State_Change, "UNKNOWN");
                ReportEvent(SECS_DV.PTZ_MAP, SECS_DV.PTZ_MAP_PREV, SECS_Event.PTZ_Map_Available, "UNKNOWN");
                ReportEvent(SECS_DV.STOCKER_SOURCE, SECS_DV.STOCKER_SOURCE_PREV, SECS_Event.Stocker_Source_Change, "UNKNOWN");
                ReportEvent(SECS_DV.STOCKER_DESTINATION, SECS_DV.STOCKER_DESTINATION_PREV, SECS_Event.Stocker_Destination_Change, "UNKNOWN");
                ReportEvent(SECS_SV.ALIGNER_STATE, SECS_SV.ALIGNER_STATE_PREV, SECS_Event.ALIGNER_State_Change, "RESET_REQUIRED");

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

            //object OutputRawData = null;
            //object Value = null;

            //ShowSECSIIMessage RawData
            axQSWrapper1.SendSECSIIMessage(S, F, W_Bit, ref SystemBytes, RawData);
            //axQSWrapper1.SendSECSIIMessage(S, F, W_Bit, ref SystemBytes, OutputRawData);
            //AppendText("QuickGEM Send Request ===>" + "\r\n");
        }

        //******************************************************************************************
        //* QuickSECS Event Call Back Area
        //******************************************************************************************
        //private void axQSWrapper1_QSEvent(object sender, AxQSACTIVEXLib._IQSWrapperEvents_QSEventEvent e) //*old
        private void axQSWrapper1_QSEvent(int lID, EVENT_ID lMsgID, int S, int F, int W_Bit, int ulSystemBytes, object RawData, object Head, string pEventText) //*new
        {
            //QGACTIVEXLib.SV_DATA_TYPE GetFormat;
            //object SvVal = null;
            //object OutputRawData = null;
            //object tmp = null;
            //object Value = null;
            //object tmpValue = null;
            //int lOffset = 0;
            //int lItemBytes = 0;

            //switch (S)
            //{ // bypass axQGWrapper1_QGEvent
            //    case 1:
            //        switch (F)
            //        {
            //            case 3:
            //                lOffset = axQSWrapper1.DataItemIn(ref RawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.LIST_TYPE, out lItemBytes, ref tmpValue);
            //                lOffset = axQSWrapper1.DataItemIn(ref RawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.UINT_4_TYPE, out lItemBytes, ref Value);
            //                if (((uint[])Value).Length!=0)
            //                {
            //                    switch (((uint[])Value)[0])
            //                    {
            //                        case 300:
            //                            axQGWrapper1.GetSV(3, out GetFormat, out SvVal);
            //                            axQSWrapper1.DataItemOut(ref OutputRawData, 1, QSACTIVEXLib.SECSII_DATA_TYPE.LIST_TYPE, ref tmp);
            //                            axQSWrapper1.DataItemOut(ref OutputRawData, SvVal.ToString().Length, QSACTIVEXLib.SECSII_DATA_TYPE.ASCII_TYPE, ref SvVal);
            //                            axQSWrapper1.SendSECSIIMessage(S, F + 1, W_Bit, ref ulSystemBytes, OutputRawData);
            //                            return;
            //                    }
            //                }
            //                break;
            //        }
            //        break;
            //}
            QGACTIVEXLib.PROCESS_MSG_RESULT lResult = axQGWrapper1.ProcessMessage((int)lMsgID, S, F, W_Bit, ulSystemBytes, ref RawData, ref Head, pEventText);
            if (lMsgID == EVENT_ID.QS_EVENT_RECV_MSG || lMsgID == EVENT_ID.QS_EVENT_SEND_MSG)
            {
                _Report.On_Message_Log("SECS", pEventText);
                _Report.On_Message_Log("SECS", ShowSECSIIMessage(RawData));
                
            }
            if (lResult == QGACTIVEXLib.PROCESS_MSG_RESULT.REPLY_BY_AP && S == 2 && F == 41)
            {
                Dictionary<string, string> param;
                switch (remoteCmd.GetCommandName())
                {
                    case "CLAMP_ELPT":
                        param = new Dictionary<string, string>();
                        param.Add("@Target", remoteCmd.GetCPValue("SOURCE"));
                        if (GetSv(SECS_SV.STOCKER_STATE).Equals("ALARM") || GetSv(SECS_SV.STOCKER_STATE).Equals("RESET_REQUIRED"))
                        {
                            ReplyAck(Hack.Stocker_Reset_Required, ulSystemBytes);
                            return;
                        }
                        switch (remoteCmd.GetCPValue("FUNCTION"))
                        {
                            case "CLAMP":
                                _Report.NewTask(ulSystemBytes.ToString(), TaskFlowManagement.Command.CLAMP_ELPT, param);

                                break;
                            case "UNCLAMP":
                                _Report.NewTask(ulSystemBytes.ToString(), TaskFlowManagement.Command.UNCLAMP_ELPT, param);
                                break;
                        }
                        break;
                    case "FOUP_ID":
                        param = new Dictionary<string, string>();
                        param.Add("@Target", remoteCmd.GetCPValue("SOURCE"));
                        if (GetSv(SECS_SV.STOCKER_STATE).Equals("ALARM") || GetSv(SECS_SV.STOCKER_STATE).Equals("RESET_REQUIRED"))
                        {
                            ReplyAck(Hack.Stocker_Reset_Required, ulSystemBytes);
                            return;
                        }
                        _Report.NewTask(ulSystemBytes.ToString(), TaskFlowManagement.Command.FOUP_ID, param);
                        break;
                    case "MOVE_FOUP":


                        param = new Dictionary<string, string>();
                        param.Add("@Target", "FOUP_ROBOT");
                        param.Add("@FromPosition", remoteCmd.GetCPValue("SOURCE").Replace("_", ""));
                        param.Add("@ToPosition", remoteCmd.GetCPValue("DESTINATION").Replace("_", ""));
                        //來源與目的地同型態
                        if ((param["@FromPosition"].Contains("ILPT") && param["@ToPosition"].Contains("ILPT")) || (param["@FromPosition"].Contains("ELPT") && param["@ToPosition"].Contains("ELPT")))
                        {
                            ReplyAck(Hack.At_least_one_parameter_is_invalid, ulSystemBytes);
                            return;
                        }
                        //參數不在列表清單中
                        if (_Report.GetNode(param["@FromPosition"]) == null || _Report.GetNode(param["@ToPosition"]) == null)
                        {
                            ReplyAck(Hack.At_least_one_parameter_is_invalid, ulSystemBytes);
                            return;
                        }
                        if (GetSv(SECS_SV.STOCKER_STATE).Equals("ALARM") || GetSv(SECS_SV.STOCKER_STATE).Equals("RESET_REQUIRED"))
                        {
                            ReplyAck(Hack.Stocker_Reset_Required, ulSystemBytes);
                            return;
                        }
                        if (GetSv(SECS_SV.STOCKER_STATE).Equals("OFFLINE"))
                        {
                            ReplyAck(Hack.Stocker_is_Offline, ulSystemBytes);
                            return;
                        }
                        if (!GetSv(SECS_SV.STOCKER_STATE).Equals("IDLE"))
                        {
                            ReplyAck(Hack.Stocker_Not_Idle, ulSystemBytes);
                            return;
                        }
                        if (!_Report.GetNode(param["@FromPosition"]).Enable)
                        {
                            switch (_Report.GetNode(param["@FromPosition"]).Name)
                            {
                                case "ELPT1":
                                    ReplyAck(Hack.ELPT1_is_Offline, ulSystemBytes);
                                    break;
                                case "ELPT2":
                                    ReplyAck(Hack.ELPT2_is_Offline, ulSystemBytes);
                                    break;
                                case "ILPT1":
                                    ReplyAck(Hack.ILPT1_is_Offline, ulSystemBytes);
                                    break;
                                case "ILPT2":
                                    ReplyAck(Hack.ILPT2_is_Offline, ulSystemBytes);
                                    break;
                            }

                            return;
                        }
                        if (!_Report.GetNode(param["@ToPosition"]).Enable)
                        {
                            switch (_Report.GetNode(param["@ToPosition"]).Name)
                            {
                                case "ELPT1":
                                    ReplyAck(Hack.ELPT1_is_Offline, ulSystemBytes);
                                    break;
                                case "ELPT2":
                                    ReplyAck(Hack.ELPT2_is_Offline, ulSystemBytes);
                                    break;
                                case "ILPT1":
                                    ReplyAck(Hack.ILPT1_is_Offline, ulSystemBytes);
                                    break;
                                case "ILPT2":
                                    ReplyAck(Hack.ILPT2_is_Offline, ulSystemBytes);
                                    break;
                            }

                            return;
                        }
                        if (!_Report.GetNode("SHELF").Status[param["@FromPosition"]].Equals("1"))
                        {
                            ReplyAck(Hack.Stocker_Source_Empty, ulSystemBytes);
                            return;
                        }
                        if (_Report.GetNode("SHELF").Status[param["@ToPosition"]].Equals("1"))
                        {
                            ReplyAck(Hack.Stocker_Destination_Full, ulSystemBytes);
                            return;
                        }
                        _Report.NewTask(ulSystemBytes.ToString(), TaskFlowManagement.Command.MOVE_FOUP, param);

                        break;
                    case "STOP_STOCKER":
                        if (GetSv(SECS_SV.STOCKER_STATE).Equals("ALARM") || GetSv(SECS_SV.STOCKER_STATE).Equals("RESET_REQUIRED"))
                        {
                            ReplyAck(Hack.Stocker_Reset_Required, ulSystemBytes);
                            return;
                        }
                        _Report.NewTask(ulSystemBytes.ToString(), TaskFlowManagement.Command.STOP_STOCKER, null);
                        break;
                    case "RESUME_STOCKER":
                        if (GetSv(SECS_SV.STOCKER_STATE).Equals("ALARM") || GetSv(SECS_SV.STOCKER_STATE).Equals("RESET_REQUIRED"))
                        {
                            ReplyAck(Hack.Stocker_Reset_Required, ulSystemBytes);
                            return;
                        }
                        _Report.NewTask(ulSystemBytes.ToString(), TaskFlowManagement.Command.RESUME_STOCKER, null);
                        break;
                    case "ABORT_STOCKER":
                        if (GetSv(SECS_SV.STOCKER_STATE).Equals("ALARM") || GetSv(SECS_SV.STOCKER_STATE).Equals("RESET_REQUIRED"))
                        {
                            ReplyAck(Hack.Stocker_Reset_Required, ulSystemBytes);
                            return;
                        }
                        _Report.NewTask(ulSystemBytes.ToString(), TaskFlowManagement.Command.ABORT_STOCKER, null);
                        break;
                    case "RESET_STOCKER":
                        QGACTIVEXLib.SV_DATA_TYPE GetFormat;
                        object SvVal = null;

                        axQGWrapper1.GetSV((int)SECS_DV.STOCKER_SOURCE, out GetFormat, out SvVal);
                        param = new Dictionary<string, string>();
                        param.Add("@Source", SvVal.ToString());
                        axQGWrapper1.GetSV((int)SECS_DV.STOCKER_DESTINATION, out GetFormat, out SvVal);
                        param.Add("@Destination", SvVal.ToString());
                        if (GetSv(SECS_SV.WTS_STATE).Equals("RESETTING"))
                        {
                            ReplyAck(Hack.WTS_is_not_Idle, ulSystemBytes);
                            return;
                        }

                        _Report.NewTask(ulSystemBytes.ToString(), TaskFlowManagement.Command.RESET_STOCKER, param);
                        break;
                    case "OPEN_FOUP":
                        param = new Dictionary<string, string>();
                        param.Add("@Target", remoteCmd.GetCPValue("SOURCE"));
                        param.Add("@Val2", remoteCmd.GetCPValue("SOURCE").Equals("ILPT1") ? "1" : "2");
                        param.Add("@Value", "1");

                        //參數不在列表清單中
                        if (_Report.GetNode(param["@Target"]) == null)
                        {
                            ReplyAck(Hack.At_least_one_parameter_is_invalid, ulSystemBytes);
                            return;
                        }
                        if (!_Report.GetNode(param["@Target"]).Enable)
                        {
                            switch (_Report.GetNode(param["@Target"]).Name)
                            {
                                case "ILPT1":
                                    ReplyAck(Hack.ILPT1_is_Offline, ulSystemBytes);
                                    break;
                                case "ILPT2":
                                    ReplyAck(Hack.ILPT2_is_Offline, ulSystemBytes);
                                    break;
                            }
                            return;
                        }
                        if (GetSv(SECS_SV.STOCKER_STATE).Equals("ALARM") || GetSv(SECS_SV.STOCKER_STATE).Equals("RESET_REQUIRED"))
                        {
                            ReplyAck(Hack.Stocker_Reset_Required, ulSystemBytes);
                            return;
                        }
                        if (_Report.GetNode(param["@Target"]).Name.Equals("ILPT1") && (!GetSv(SECS_SV.ILPT1_FOUP_STATE).Equals("ID_CONFIRMATION") && !GetSv(SECS_SV.ILPT1_FOUP_STATE).Equals("READY_TO_UNLOAD")))
                        {
                            ReplyAck(Hack.WTS_ILPT_Empty, ulSystemBytes);
                            return;
                        }
                        if (_Report.GetNode(param["@Target"]).Name.Equals("ILPT2") && (!GetSv(SECS_SV.ILPT2_FOUP_STATE).Equals("ID_CONFIRMATION") && !GetSv(SECS_SV.ILPT2_FOUP_STATE).Equals("READY_TO_UNLOAD")))
                        {
                            ReplyAck(Hack.WTS_ILPT_Empty, ulSystemBytes);
                            return;
                        }
                        _Report.NewTask(ulSystemBytes.ToString(), TaskFlowManagement.Command.OPEN_FOUP, param);
                        break;
                    case "CLOSE_FOUP":
                        param = new Dictionary<string, string>();
                        param.Add("@Target", remoteCmd.GetCPValue("SOURCE"));
                        param.Add("@Val2", remoteCmd.GetCPValue("SOURCE").Equals("ILPT1") ? "1" : "2");
                        //參數不在列表清單中
                        if (_Report.GetNode(param["@Target"]) == null)
                        {
                            ReplyAck(Hack.At_least_one_parameter_is_invalid, ulSystemBytes);
                            return;
                        }
                        if (!_Report.GetNode(param["@Target"]).Enable)
                        {
                            switch (_Report.GetNode(param["@Target"]).Name)
                            {
                                case "ILPT1":
                                    ReplyAck(Hack.ILPT1_is_Offline, ulSystemBytes);
                                    break;
                                case "ILPT2":
                                    ReplyAck(Hack.ILPT2_is_Offline, ulSystemBytes);
                                    break;
                            }
                            return;
                        }
                        if (GetSv(SECS_SV.STOCKER_STATE).Equals("ALARM") || GetSv(SECS_SV.STOCKER_STATE).Equals("RESET_REQUIRED"))
                        {
                            ReplyAck(Hack.Stocker_Reset_Required, ulSystemBytes);
                            return;
                        }
                        if (_Report.GetNode(param["@Target"]).Name.Equals("ILPT1") && (!GetSv(SECS_SV.ILPT1_FOUP_STATE).Equals("PREMAP_CONFIRM") && !GetSv(SECS_SV.ILPT1_FOUP_STATE).Equals("POSTMAP_CONFIRM")))
                        {
                            ReplyAck(Hack.WTS_ILPT_Door_is_not_Open, ulSystemBytes);
                            return;
                        }
                        if (_Report.GetNode(param["@Target"]).Name.Equals("ILPT2") && (!GetSv(SECS_SV.ILPT2_FOUP_STATE).Equals("PREMAP_CONFIRM") && !GetSv(SECS_SV.ILPT2_FOUP_STATE).Equals("POSTMAP_CONFIRM")))
                        {
                            ReplyAck(Hack.WTS_ILPT_Door_is_not_Open, ulSystemBytes);
                            return;
                        }
                        _Report.NewTask(ulSystemBytes.ToString(), TaskFlowManagement.Command.CLOSE_FOUP, param);
                        break;
                    case "TRANSFER_WTS":
                        param = new Dictionary<string, string>();
                        param.Add("@FromPosition", remoteCmd.GetCPValue("SOURCE"));
                        param.Add("@ToPosition", remoteCmd.GetCPValue("DESTINATION"));
                        param.Add("@Mode", remoteCmd.GetCPValue("PATH").Equals("DIRTY") ? "1" : "0");

                        //參數不在列表清單中
                        if (_Report.GetNode(param["@FromPosition"]) == null || _Report.GetNode(param["@ToPosition"]) == null)
                        {
                            ReplyAck(Hack.At_least_one_parameter_is_invalid, ulSystemBytes);
                            return;
                        }
                        if (GetSv(SECS_SV.WTS_STATE).Equals("ALARM") || GetSv(SECS_SV.WTS_STATE).Equals("RESET_REQUIRED"))
                        {
                            ReplyAck(Hack.WTS_Reset_is_Required, ulSystemBytes);
                            return;
                        }
                        if (!GetSv(SECS_SV.WTS_STATE).Equals("IDLE"))
                        {
                            ReplyAck(Hack.WTS_is_not_Idle, ulSystemBytes);
                            return;
                        }
                        if (!_Report.GetNode(param["@FromPosition"]).Enable)
                        {
                            switch (_Report.GetNode(param["@FromPosition"]).Name)
                            {
                                case "ILPT1":
                                    ReplyAck(Hack.ILPT1_is_Offline, ulSystemBytes);
                                    break;
                                case "ILPT2":
                                    ReplyAck(Hack.ILPT2_is_Offline, ulSystemBytes);
                                    break;
                            }
                            return;
                        }
                        if (!_Report.GetNode(param["@ToPosition"]).Enable)
                        {
                            switch (_Report.GetNode(param["@ToPosition"]).Name)
                            {
                                case "ILPT1":
                                    ReplyAck(Hack.ILPT1_is_Offline, ulSystemBytes);
                                    break;
                                case "ILPT2":
                                    ReplyAck(Hack.ILPT2_is_Offline, ulSystemBytes);
                                    break;
                            }
                            return;
                        }
                        if (_Report.GetNode(param["@FromPosition"]).Name.Equals("ILPT1") && (!GetSv(SECS_SV.ILPT1_FOUP_STATE).Equals("PREMAP_CONFIRM") && !GetSv(SECS_SV.ILPT1_FOUP_STATE).Equals("POSTMAP_CONFIRM")))
                        {
                            ReplyAck(Hack.WTS_ILPT_Door_is_not_Open, ulSystemBytes);
                            return;
                        }
                        if (_Report.GetNode(param["@FromPosition"]).Name.Equals("ILPT2") && (!GetSv(SECS_SV.ILPT2_FOUP_STATE).Equals("PREMAP_CONFIRM") && !GetSv(SECS_SV.ILPT2_FOUP_STATE).Equals("POSTMAP_CONFIRM")))
                        {
                            ReplyAck(Hack.WTS_ILPT_Door_is_not_Open, ulSystemBytes);
                            return;
                        }
                        if (_Report.GetNode(param["@ToPosition"]).Name.Equals("ILPT1") && (!GetSv(SECS_SV.ILPT1_FOUP_STATE).Equals("PREMAP_CONFIRM") && !GetSv(SECS_SV.ILPT1_FOUP_STATE).Equals("POSTMAP_CONFIRM")))
                        {
                            ReplyAck(Hack.WTS_ILPT_Door_is_not_Open, ulSystemBytes);
                            return;
                        }
                        if (_Report.GetNode(param["@ToPosition"]).Name.Equals("ILPT2") && (!GetSv(SECS_SV.ILPT2_FOUP_STATE).Equals("PREMAP_CONFIRM") && !GetSv(SECS_SV.ILPT2_FOUP_STATE).Equals("POSTMAP_CONFIRM")))
                        {
                            ReplyAck(Hack.WTS_ILPT_Door_is_not_Open, ulSystemBytes);
                            return;
                        }
                        if (_Report.GetNode(param["@FromPosition"]).Name.Equals("ILPT1") && (GetSv(SECS_DV.ILPT1_MAP).Equals("0000000000000000000000000")))
                        {
                            ReplyAck(Hack.WTS_ILPT_Empty, ulSystemBytes);
                            return;
                        }
                        if (_Report.GetNode(param["@FromPosition"]).Name.Equals("ILPT2") && (GetSv(SECS_DV.ILPT2_MAP).Equals("0000000000000000000000000")))
                        {
                            ReplyAck(Hack.WTS_ILPT_Empty, ulSystemBytes);
                            return;
                        }
                        if (_Report.GetNode(param["@FromPosition"]).Name.Equals("CTU") && !_Report.GetNode(param["@FromPosition"]).R_Presence)
                        {
                            ReplyAck(Hack.WTS_CTU_Empty, ulSystemBytes);
                            return;
                        }
                        if (_Report.GetNode(param["@ToPosition"]).Name.Equals("CTU") && _Report.GetNode(param["@ToPosition"]).R_Presence)
                        {
                            ReplyAck(Hack.WTS_CTU_Full, ulSystemBytes);
                            return;
                        }
                        _Report.NewTask(ulSystemBytes.ToString(), TaskFlowManagement.Command.TRANSFER_WTS, param);
                        break;
                    case "STOP_WTS":
                        if (GetSv(SECS_SV.WTS_STATE).Equals("ALARM") || GetSv(SECS_SV.WTS_STATE).Equals("RESET_REQUIRED"))
                        {
                            ReplyAck(Hack.WTS_Reset_is_Required, ulSystemBytes);
                            return;
                        }
                        _Report.NewTask(ulSystemBytes.ToString(), TaskFlowManagement.Command.STOP_WTS, null);
                        break;
                    case "RESUME_WTS":
                        if (GetSv(SECS_SV.WTS_STATE).Equals("ALARM") || GetSv(SECS_SV.WTS_STATE).Equals("RESET_REQUIRED"))
                        {
                            ReplyAck(Hack.WTS_Reset_is_Required, ulSystemBytes);
                            return;
                        }
                        _Report.NewTask(ulSystemBytes.ToString(), TaskFlowManagement.Command.RESUME_WTS, null);
                        break;
                    case "ABORT_WTS":
                        if (GetSv(SECS_SV.WTS_STATE).Equals("ALARM") || GetSv(SECS_SV.WTS_STATE).Equals("RESET_REQUIRED"))
                        {
                            ReplyAck(Hack.WTS_Reset_is_Required, ulSystemBytes);
                            return;
                        }
                        _Report.NewTask(ulSystemBytes.ToString(), TaskFlowManagement.Command.ABORT_WTS, null);
                        break;
                    case "RESET_WTS":
                        if (GetSv(SECS_SV.STOCKER_STATE).Equals("RESETTING"))
                        {
                            ReplyAck(Hack.Stocker_Not_Idle, ulSystemBytes);
                            return;
                        }
                        _Report.NewTask(ulSystemBytes.ToString(), TaskFlowManagement.Command.RESET_WTS, null);
                        break;
                    case "TRANSFER_PTZ":
                        param = new Dictionary<string, string>();
                        param.Add("@Way", remoteCmd.GetCPValue("DIRECTION"));
                        string tmp = "";
                        switch (remoteCmd.GetCPValue("WAFER_ORIENTATION"))
                        {
                            case "FACE_TO_FACE":
                                tmp = "0";
                                break;
                            case "BACK_TO_BACK":
                                tmp = "1";
                                break;
                            case "ALL_BACK":
                                tmp = "2";
                                break;
                            case "ALL_FWD":
                                tmp = "3";
                                break;
                        }
                        param.Add("@Direction", tmp);
                        param.Add("@Mode", remoteCmd.GetCPValue("PATH").Equals("DIRTY") ? "1" : "0");
                        switch (remoteCmd.GetCPValue("STATION"))
                        {
                            case "ODD":
                                tmp = "0";
                                break;
                            case "EVEN":
                                tmp = "1";
                                break;
                        }
                        param.Add("@Station", tmp);
                        //參數不在列表清單中
                        if (!param["@Way"].Equals("IN") && !param["@Way"].Equals("OUT"))
                        {
                            ReplyAck(Hack.At_least_one_parameter_is_invalid, ulSystemBytes);
                            return;
                        }
                        if (GetSv(SECS_SV.WTS_STATE).Equals("ALARM") || GetSv(SECS_SV.WTS_STATE).Equals("RESET_REQUIRED"))
                        {
                            ReplyAck(Hack.WTS_Reset_is_Required, ulSystemBytes);
                            return;
                        }
                        if (!GetSv(SECS_SV.WTS_STATE).Equals("IDLE"))
                        {
                            ReplyAck(Hack.WTS_is_not_Idle, ulSystemBytes);
                            return;
                        }
                        if (!GetSv(SECS_SV.PTZ_STATE).Equals("IDLE"))
                        {
                            ReplyAck(Hack.WTS_Incorrect_PTZ_State, ulSystemBytes);
                            return;
                        }
                        if (param["@Way"].Equals("IN") && GetSv(SECS_SV.PROCESS_SUBSTRATE_STATE).Equals("ODDEVEN"))
                        {
                            ReplyAck(Hack.WTS_PTZ_Slot_Full, ulSystemBytes);
                            return;
                        }
                        if (param["@Way"].Equals("IN") && !_Report.GetNode("CTU").R_Presence)
                        {
                            ReplyAck(Hack.WTS_CTU_Empty, ulSystemBytes);
                            return;
                        }
                        if (param["@Way"].Equals("OUT") && GetSv(SECS_SV.PROCESS_SUBSTRATE_STATE).Equals("EMPTY"))
                        {
                            ReplyAck(Hack.WTS_PTZ_Slot_Empty, ulSystemBytes);
                            return;
                        }
                        if (param["@Way"].Equals("OUT") && _Report.GetNode("CTU").R_Presence)
                        {
                            ReplyAck(Hack.WTS_CTU_Full, ulSystemBytes);
                            return;
                        }
                        _Report.NewTask(ulSystemBytes.ToString(), TaskFlowManagement.Command.TRANSFER_PTZ, param);
                        break;
                    case "NOTCH_ALIGN":
                        param = new Dictionary<string, string>();
                        param.Add("@Target", "WTS_ALIGNER");
                        param.Add("@Value", remoteCmd.GetCPValue("NOTCH_DEGREES"));
                        if (GetSv(SECS_SV.WTS_STATE).Equals("ALARM") || GetSv(SECS_SV.WTS_STATE).Equals("RESET_REQUIRED"))
                        {
                            ReplyAck(Hack.WTS_Reset_is_Required, ulSystemBytes);
                            return;
                        }
                        _Report.NewTask(ulSystemBytes.ToString(), TaskFlowManagement.Command.WTSALIGNER_ALIGN, param);
                        break;
                    case "BLOCK_PTZ":
                        param = new Dictionary<string, string>();
                        param.Add("@Path", remoteCmd.GetCPValue("PATH").Equals("DIRTY") ? "1" : "0");
                        if (GetSv(SECS_SV.WTS_STATE).Equals("ALARM") || GetSv(SECS_SV.WTS_STATE).Equals("RESET_REQUIRED"))
                        {
                            ReplyAck(Hack.WTS_Reset_is_Required, ulSystemBytes);
                            return;
                        }
                        if (!GetSv(SECS_SV.PTZ_STATE).Equals("IDLE"))
                        {
                            ReplyAck(Hack.WTS_PTZ_is_not_Idle, ulSystemBytes);
                            return;
                        }
                        _Report.NewTask(ulSystemBytes.ToString(), TaskFlowManagement.Command.BLOCK_PTZ, param);
                        break;
                    case "RELEASE_PTZ":
                        if (GetSv(SECS_SV.WTS_STATE).Equals("ALARM") || GetSv(SECS_SV.WTS_STATE).Equals("RESET_REQUIRED"))
                        {
                            ReplyAck(Hack.WTS_Reset_is_Required, ulSystemBytes);
                            return;
                        }
                        _Report.NewTask(ulSystemBytes.ToString(), TaskFlowManagement.Command.RELEASE_PTZ, null);
                        break;
                    case "BLOCK_ALIGNER":
                        if (GetSv(SECS_SV.WTS_STATE).Equals("ALARM") || GetSv(SECS_SV.WTS_STATE).Equals("RESET_REQUIRED"))
                        {
                            ReplyAck(Hack.WTS_Reset_is_Required, ulSystemBytes);
                            return;
                        }
                        if (!GetSv(SECS_SV.ALIGNER_STATE).Equals("IDLE"))
                        {
                            ReplyAck(Hack.WTS_Aligner_is_not_Idle, ulSystemBytes);
                            return;
                        }
                        _Report.NewTask(ulSystemBytes.ToString(), TaskFlowManagement.Command.BLOCK_ALIGNER, null);
                        break;
                    case "RELEASE_ALIGNER":
                        if (GetSv(SECS_SV.WTS_STATE).Equals("ALARM") || GetSv(SECS_SV.WTS_STATE).Equals("RESET_REQUIRED"))
                        {
                            ReplyAck(Hack.WTS_Reset_is_Required, ulSystemBytes);
                            return;
                        }
                        _Report.NewTask(ulSystemBytes.ToString(), TaskFlowManagement.Command.RELEASE_ALIGNER, null);
                        break;
                    case "PORT_ACCESS_MODE":
                        param = new Dictionary<string, string>();
                        param.Add("@Target", remoteCmd.GetCPValue("PORT"));
                        param.Add("@Value", remoteCmd.GetCPValue("ACCESS"));
                        _Report.NewTask(ulSystemBytes.ToString(), TaskFlowManagement.Command.PORT_ACCESS_MODE, param);
                        break;
                    case "RESET_E84":
                        param = new Dictionary<string, string>();
                        param.Add("@Target", remoteCmd.GetCPValue("PORT"));
                        _Report.NewTask(ulSystemBytes.ToString(), TaskFlowManagement.Command.RESET_E84, param);
                        break;
                }
            }
            else if (lMsgID == QSACTIVEXLib.EVENT_ID.QS_EVENT_CONNECTED)
            {
                ConnStatus = "CONNECTED";
                _Report.On_Message_Log("SECS", "CONNECTED");
            }
            else if (lMsgID == QSACTIVEXLib.EVENT_ID.QS_EVENT_DISCONNECTED)
            {
                if (ConnStatus == "CONNECTED")
                {
                    _Report.On_Message_Log("SECS", "DISCONNECTED");
                }
                if (ConnStatus != "CONNECTING")
                {
                    ConnStatus = "CONNECTING";
                    _Report.On_Message_Log("SECS", "CONNECTING");
                }
                axQSWrapper1.Start();
            }
        }
        string ConnStatus = "";
        private string ShowSECSIIMessage(object myRawData)
        {
            int[] myStack = new int[10];
            int myStackPtr;
            int lOffset;
            int lItemNum;
            object ItemData = null;
            int lLength;
            QSACTIVEXLib.SECSII_DATA_TYPE lItemType;
            string DisplayString;
            string MyStr = "";
            //int i;

            // Verify whether the input data is an array or not
            System.Array myArray = myRawData as System.Array;
            if (myArray != null)
                lLength = myArray.Length;
            else
                return "";

            if (myArray.Length == 0)
                return "";

            myStackPtr = 0;
            lOffset = 0;

            //int lOffset1 = 0;

            try
            {
                while (lOffset < lLength)
                {
                    if (myStackPtr > 0)
                    {
                        if (myStack[myStackPtr - 1] > 0)
                        {
                            myStack[myStackPtr - 1] = myStack[myStackPtr - 1] - 1;
                        }
                        else
                        {
                            myStackPtr = myStackPtr - 1;
                            if (myStackPtr == 0)
                            {
                                // force show end. Normal message should not run to here

                                MyStr = MyStr + ">" + "\r\n";

                                MyStr = MyStr + "." + "\r\n";


                                return MyStr;
                            }

                            while (myStack[myStackPtr] == 0)
                            {

                                for (long lngIndex = 0; lngIndex < myStackPtr; lngIndex++)
                                {
                                    MyStr = MyStr + "    ";
                                }
                                MyStr = MyStr + ">" + "\r\n";
                                myStackPtr = myStackPtr - 1;
                            }
                            myStackPtr = myStackPtr + 1;
                            myStack[myStackPtr - 1] = myStack[myStackPtr - 1] - 1;
                        }
                    }

                    //if (lOffset > lOffset1 + 1000) // for what ?
                    //{
                    //    lOffset1 = lOffset;

                    //    AppendText(lOffset + "\r\n");
                    //}


                    lItemType = axQSWrapper1.GetDataItemType(ref myRawData, lOffset);
                    if (lItemType == QSACTIVEXLib.SECSII_DATA_TYPE.LIST_TYPE)
                    {
                        lItemNum = 99;

                        object objTemp1 = null;
                        lOffset = axQSWrapper1.DataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.LIST_TYPE, out lItemNum, ref objTemp1);
                        // Display the data item
                        for (long lngIndex = 0; lngIndex < myStackPtr; lngIndex++)
                        {
                            MyStr = MyStr + "    ";
                        }
                        MyStr = MyStr + "<L[" + lItemNum + "]" + "\r\n";
                        // Increase the indent level
                        myStack[myStackPtr] = lItemNum;
                        myStackPtr = myStackPtr + 1;
                    }
                    else if (lItemType == QSACTIVEXLib.SECSII_DATA_TYPE.ASCII_TYPE)
                    {
                        lOffset = axQSWrapper1.DataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.ASCII_TYPE, out lItemNum, ref ItemData);
                        for (long lngIndex = 0; lngIndex < myStackPtr; lngIndex++)
                        {
                            MyStr = MyStr + "    ";
                        }
                        MyStr = MyStr + "<A[" + lItemNum + "] " + (char)34 + ItemData + (char)34 + ">" + "\r\n";
                    }
                    else if (lItemType == QSACTIVEXLib.SECSII_DATA_TYPE.JIS_TYPE)
                    {
                        lOffset = axQSWrapper1.DataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.JIS_TYPE, out lItemNum, ref ItemData);
                        for (long lngIndex = 0; lngIndex < myStackPtr; lngIndex++)
                        {
                            MyStr = MyStr + "    ";
                        }
                        MyStr = MyStr + "<J[" + lItemNum + "] " + (char)34 + ItemData + (char)34 + ">" + "\r\n";
                    }
                    else if (lItemType == QSACTIVEXLib.SECSII_DATA_TYPE.BINARY_TYPE)
                    {
                        lOffset = axQSWrapper1.DataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.BINARY_TYPE, out lItemNum, ref ItemData);
                        for (long lngIndex = 0; lngIndex < myStackPtr; lngIndex++)
                        {
                            MyStr = MyStr + "    ";
                        }
                        DisplayString = "";

                        int[] tData = (int[])ItemData;

                        for (int intIndex = 0; intIndex <= (lItemNum - 1); intIndex++)
                        {
                            DisplayString = DisplayString + " 0x" + Convert.ToString(tData[intIndex], 16).ToUpper();
                        }
                        MyStr = MyStr + "<B[" + lItemNum + "]" + DisplayString + ">" + "\r\n";
                    }
                    else if (lItemType == QSACTIVEXLib.SECSII_DATA_TYPE.BOOLEAN_TYPE)
                    {
                        lOffset = axQSWrapper1.DataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.BOOLEAN_TYPE, out lItemNum, ref ItemData);
                        for (long lngIndex = 0; lngIndex < myStackPtr; lngIndex++)
                        {
                            MyStr = MyStr + "    ";
                        }
                        DisplayString = "";

                        int[] tData = (int[])ItemData;

                        for (int intIndex = 0; intIndex <= (lItemNum - 1); intIndex++)
                        {
                            DisplayString = DisplayString + " 0x" + Convert.ToString(tData[intIndex], 16).ToUpper();
                        }
                        MyStr = MyStr + "<Boolean[" + lItemNum + "]" + DisplayString + ">" + "\r\n";
                    }
                    else if (lItemType == QSACTIVEXLib.SECSII_DATA_TYPE.UINT_1_TYPE)
                    {
                        lOffset = axQSWrapper1.DataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.UINT_1_TYPE, out lItemNum, ref ItemData);
                        for (long lngIndex = 0; lngIndex < myStackPtr; lngIndex++)
                        {
                            MyStr = MyStr + "    ";
                        }
                        DisplayString = "";
                        uint[] tData = (uint[])ItemData;

                        for (int intIndex = 0; intIndex <= (lItemNum - 1); intIndex++)
                        {
                            DisplayString = DisplayString + " " + Convert.ToString(tData[intIndex]);
                        }
                        MyStr = MyStr + "<U1[" + lItemNum + "]" + DisplayString + ">" + "\r\n";
                    }
                    else if (lItemType == QSACTIVEXLib.SECSII_DATA_TYPE.UINT_2_TYPE)
                    {
                        lOffset = axQSWrapper1.DataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.UINT_2_TYPE, out lItemNum, ref ItemData);
                        for (long lngIndex = 0; lngIndex < myStackPtr; lngIndex++)
                        {
                            MyStr = MyStr + "    ";
                        }
                        DisplayString = "";
                        uint[] tData = (uint[])ItemData;

                        for (int intIndex = 0; intIndex <= (lItemNum - 1); intIndex++)
                        {
                            DisplayString = DisplayString + " " + Convert.ToString(tData[intIndex]);
                        }
                        MyStr = MyStr + "<U2[" + lItemNum + "]" + DisplayString + ">" + "\r\n";
                    }
                    else if (lItemType == QSACTIVEXLib.SECSII_DATA_TYPE.UINT_4_TYPE)
                    {
                        lOffset = axQSWrapper1.DataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.UINT_4_TYPE, out lItemNum, ref ItemData);
                        for (long lngIndex = 0; lngIndex < myStackPtr; lngIndex++)
                        {
                            MyStr = MyStr + "    ";
                        }
                        DisplayString = "";
                        uint[] tData = (uint[])ItemData;

                        for (int intIndex = 0; intIndex <= (lItemNum - 1); intIndex++)
                        {
                            DisplayString = DisplayString + " " + Convert.ToString(tData[intIndex]);
                        }
                        MyStr = MyStr + "<U4[" + lItemNum + "]" + DisplayString + ">" + "\r\n";
                    }
                    else if (lItemType == QSACTIVEXLib.SECSII_DATA_TYPE.INT_1_TYPE)
                    {
                        lOffset = axQSWrapper1.DataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.INT_1_TYPE, out lItemNum, ref ItemData);
                        for (long lngIndex = 0; lngIndex < myStackPtr; lngIndex++)
                        {
                            MyStr = MyStr + "    ";
                        }
                        DisplayString = "";
                        int[] tData = (int[])ItemData;

                        for (int intIndex = 0; intIndex <= (lItemNum - 1); intIndex++)
                        {
                            DisplayString = DisplayString + " " + Convert.ToString(tData[intIndex]);
                        }
                        MyStr = MyStr + "<I1[" + lItemNum + "]" + DisplayString + ">" + "\r\n";
                    }
                    else if (lItemType == QSACTIVEXLib.SECSII_DATA_TYPE.INT_2_TYPE)
                    {
                        lOffset = axQSWrapper1.DataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.INT_2_TYPE, out lItemNum, ref ItemData);
                        for (long lngIndex = 0; lngIndex < myStackPtr; lngIndex++)
                        {
                            MyStr = MyStr + "    ";
                        }
                        DisplayString = "";
                        int[] tData = (int[])ItemData;

                        for (int intIndex = 0; intIndex <= (lItemNum - 1); intIndex++)
                        {
                            DisplayString = DisplayString + " " + Convert.ToString(tData[intIndex]);
                        }
                        MyStr = MyStr + "<I2[" + lItemNum + "]" + DisplayString + ">" + "\r\n";
                    }
                    else if (lItemType == QSACTIVEXLib.SECSII_DATA_TYPE.INT_4_TYPE)
                    {
                        lOffset = axQSWrapper1.DataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.INT_4_TYPE, out lItemNum, ref ItemData);
                        for (long lngIndex = 0; lngIndex < myStackPtr; lngIndex++)
                        {
                            MyStr = MyStr + "    ";
                        }
                        DisplayString = "";
                        int[] tData = (int[])ItemData;

                        for (int intIndex = 0; intIndex <= (lItemNum - 1); intIndex++)
                        {
                            DisplayString = DisplayString + " " + Convert.ToString(tData[intIndex]);
                        }
                        MyStr = MyStr + "<I4[" + lItemNum + "]" + DisplayString + ">" + "\r\n";
                    }
                    else if (lItemType == QSACTIVEXLib.SECSII_DATA_TYPE.FT_4_TYPE)
                    {
                        lOffset = axQSWrapper1.DataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.FT_4_TYPE, out lItemNum, ref ItemData);
                        for (long lngIndex = 0; lngIndex < myStackPtr; lngIndex++)
                        {
                            MyStr = MyStr + "    ";
                        }
                        DisplayString = "";
                        Single[] tData = (Single[])ItemData;

                        for (int intIndex = 0; intIndex <= (lItemNum - 1); intIndex++)
                        {
                            DisplayString = DisplayString + " " + Convert.ToString(tData[intIndex]);
                        }
                        MyStr = MyStr + "<F4[" + lItemNum + "]" + DisplayString + ">" + "\r\n";
                    }
                    else if (lItemType == QSACTIVEXLib.SECSII_DATA_TYPE.FT_8_TYPE)
                    {
                        lOffset = axQSWrapper1.DataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.FT_8_TYPE, out lItemNum, ref ItemData);
                        for (long lngIndex = 0; lngIndex < myStackPtr; lngIndex++)
                        {
                            MyStr = MyStr + "    ";
                        }
                        DisplayString = "";
                        double[] tData = (double[])ItemData;

                        for (int intIndex = 0; intIndex <= (lItemNum - 1); intIndex++)
                        {
                            DisplayString = DisplayString + " " + Convert.ToString(tData[intIndex]);
                        }
                        MyStr = MyStr + "<F8[" + lItemNum + "]" + DisplayString + ">" + "\r\n";
                    }
                    else
                    {
                        lOffset = axQSWrapper1.DataItemInSkip(ref myRawData, lOffset, 1);
                    }
                }

            }
            catch (Exception ex)
            {


            }

            while (myStackPtr > 0)
            {
                myStackPtr = myStackPtr - 1;
                for (long lngIndex = 0; lngIndex < myStackPtr; lngIndex++)
                {
                    MyStr = MyStr + "    ";
                }
                MyStr = MyStr + ">" + "\r\n";
            }

            MyStr = MyStr + "." + "\r\n";
            return MyStr;

        }





        enum Hack
        {
            Command_has_been_performed = 0,
            Command_does_not_exist = 1,
            Cannot_perform_now = 2,
            At_least_one_parameter_is_invalid = 3,
            Command_has_been_performed_later = 4,
            Stocker_is_Offline = 65,
            ELPT1_is_Offline = 66,
            ELPT2_is_Offline = 67,
            ILPT1_is_Offline = 68,
            ILPT2_is_Offline = 69,
            WTS_is_not_Idle = 70,
            WTS_Reset_is_Required = 72,
            WTS_ILPT_Door_is_not_Open = 74,
            WTS_Incorrect_PTZ_State = 75,
            WTS_ILPT_Empty = 78,
            WTS_PTZ_Slot_Empty = 81,
            WTS_PTZ_Slot_Full = 82,
            Stocker_Not_Idle = 84,
            Stocker_Reset_Required = 86,
            Stocker_Source_Empty = 93,
            Stocker_Destination_Full = 94,
            WTS_CTU_Full = 100,
            WTS_CTU_Empty = 101,
            WTS_Aligner_is_not_Idle = 103,
            WTS_PTZ_is_not_Idle = 104
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
                CP.TryGetValue(CPName, out result);
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
            Node Shelf = null;
            switch (Task.TaskName)
            {
                case TaskFlowManagement.Command.PORT_ACCESS_MODE:
                    if (Task.Params["@Target"].Equals("ILPT1"))
                    {
                        ReportEvent(SECS_SV.ILPT1_MODE, SECS_SV.ILPT1_MODE_PREV, SECS_Event.ILPT1_Mode_Change, Task.Params["@Value"]);
                    }
                    if (Task.Params["@Target"].Equals("ILPT2"))
                    {
                        ReportEvent(SECS_SV.ILPT2_MODE, SECS_SV.ILPT2_MODE_PREV, SECS_Event.ILPT2_Mode_Change, Task.Params["@Value"]);
                    }
                    if (Task.Params["@Target"].Equals("ELPT1"))
                    {
                        ReportEvent(SECS_SV.ELPT1_MODE, SECS_SV.ELPT1_MODE_PREV, SECS_Event.ELPT1_Mode_Change, Task.Params["@Value"]);
                    }
                    if (Task.Params["@Target"].Equals("ELPT2"))
                    {
                        ReportEvent(SECS_SV.ELPT2_MODE, SECS_SV.ELPT2_MODE_PREV, SECS_Event.ELPT2_Mode_Change, Task.Params["@Value"]);
                    }
                    break;
                case TaskFlowManagement.Command.BLOCK_ALIGNER:
                    ReportEvent(SECS_SV.ALIGNER_STATE, SECS_SV.ALIGNER_STATE_PREV, SECS_Event.ALIGNER_State_Change, "BLOCKED_BY_PROCESS_TOOL");
                    break;
                case TaskFlowManagement.Command.RELEASE_ALIGNER:
                    ReportEvent(SECS_SV.ALIGNER_STATE, SECS_SV.ALIGNER_STATE_PREV, SECS_Event.ALIGNER_State_Change, "IDLE");
                    break;
                case TaskFlowManagement.Command.BLOCK_PTZ:
                    ReportEvent(SECS_SV.PTZ_STATE, SECS_SV.PTZ_STATE_PREV, SECS_Event.PTZ_State_Change, "BLOCKED_BY_PROCESS_TOOL");
                    break;
                case TaskFlowManagement.Command.RELEASE_PTZ:
                    ReportEvent(SECS_SV.PTZ_STATE, SECS_SV.PTZ_STATE_PREV, SECS_Event.PTZ_State_Change, "IDLE");
                    break;
                case TaskFlowManagement.Command.RESET_WTS:

                    Shelf = _Report.GetNode("SHELF");
                    foreach (KeyValuePair<string, string> item in Shelf.Status)
                    {
                        switch (item.Key)
                        {
                            case "ILPT1":
                                if (item.Value.Equals("1"))
                                {
                                    ReportEvent(SECS_SV.ILPT1_FOUP_STATE, SECS_SV.ILPT1_FOUP_STATE_PREV, SECS_Event.ILPT1_FOUP_State_Change, "READY_TO_UNLOAD");
                                }
                                else
                                {
                                    ReportEvent(SECS_SV.ILPT1_FOUP_STATE, SECS_SV.ILPT1_FOUP_STATE_PREV, SECS_Event.ILPT1_FOUP_State_Change, "READY_TO_LOAD");
                                }
                                break;
                            case "ILPT2":
                                if (item.Value.Equals("1"))
                                {
                                    ReportEvent(SECS_SV.ILPT2_FOUP_STATE, SECS_SV.ILPT2_FOUP_STATE_PREV, SECS_Event.ILPT2_FOUP_State_Change, "READY_TO_UNLOAD");
                                }
                                else
                                {
                                    ReportEvent(SECS_SV.ILPT2_FOUP_STATE, SECS_SV.ILPT2_FOUP_STATE_PREV, SECS_Event.ILPT2_FOUP_State_Change, "READY_TO_LOAD");
                                }
                                break;
                        }
                    }


                    ReportEvent(SECS_SV.ALIGNER_STATE, SECS_SV.ALIGNER_STATE_PREV, SECS_Event.ALIGNER_State_Change, "IDLE");

                    ReportEvent(SECS_SV.PTZ_STATE, SECS_SV.PTZ_STATE_PREV, SECS_Event.PTZ_State_Change, "IDLE");
                    ReportEvent(SECS_SV.WTS_STATE, SECS_SV.WTS_STATE_PREV, SECS_Event.WTS_State_Change, "IDLE");
                    break;
                case TaskFlowManagement.Command.TRANSFER_PTZ:
                    ReportEvent(SECS_SV.PROCESS_SUBSTRATE_STATE, SECS_SV.PROCESS_SUBSTRATE_STATE_PREV, SECS_Event.Process_Substrate_State_Change, "PRESENT_NOT_MAPPED");
                    ReportEvent(SECS_SV.WTS_STATE, SECS_SV.WTS_STATE_PREV, SECS_Event.WTS_State_Change, "IDLE");
                    ReportEvent(SECS_SV.PTZ_STATE, SECS_SV.PTZ_STATE_PREV, SECS_Event.PTZ_State_Change, "IDLE");
                    break;
                case TaskFlowManagement.Command.TRANSFER_WTS:
                    if (Task.Params["@FromPosition"].Equals("ILPT1") || Task.Params["@ToPosition"].Equals("ILPT1"))
                    {
                        ReportEvent(SECS_SV.ILPT1_FOUP_STATE, SECS_SV.ILPT1_FOUP_STATE_PREV, SECS_Event.ILPT1_FOUP_State_Change, "POSTMAP_CONFIRM");
                    }
                    if (Task.Params["@FromPosition"].Equals("ILPT2") || Task.Params["@ToPosition"].Equals("ILPT2"))
                    {
                        ReportEvent(SECS_SV.ILPT2_FOUP_STATE, SECS_SV.ILPT2_FOUP_STATE_PREV, SECS_Event.ILPT2_FOUP_State_Change, "POSTMAP_CONFIRM");
                    }
                    ReportEvent(SECS_SV.WTS_STATE, SECS_SV.WTS_STATE_PREV, SECS_Event.WTS_State_Change, "IDLE");
                    break;
                case TaskFlowManagement.Command.STOP_WTS:
                    ReportEvent(SECS_SV.WTS_STATE, SECS_SV.WTS_STATE_PREV, SECS_Event.WTS_State_Change, "STOPPED");
                    break;
                case TaskFlowManagement.Command.RESUME_WTS:
                    ReportEvent(SECS_SV.WTS_STATE, SECS_SV.WTS_STATE_PREV, SECS_Event.WTS_State_Change, "TRANSFERRING");
                    break;
                case TaskFlowManagement.Command.ABORT_WTS:
                    ReportEvent(SECS_SV.WTS_STATE, SECS_SV.WTS_STATE_PREV, SECS_Event.WTS_State_Change, "RESET_REQUIRED");
                    break;
                case TaskFlowManagement.Command.RESET_STOCKER:

                    Shelf = _Report.GetNode("SHELF");
                    foreach (KeyValuePair<string, string> item in Shelf.Status)
                    {
                        switch (item.Key)
                        {
                            case "ILPT1":
                                if (item.Value.Equals("1"))
                                {
                                    ReportEvent(SECS_SV.ILPT1_FOUP_STATE, SECS_SV.ILPT1_FOUP_STATE_PREV, SECS_Event.ILPT1_FOUP_State_Change, "READY_TO_UNLOAD");
                                }
                                else
                                {
                                    ReportEvent(SECS_SV.ILPT1_FOUP_STATE, SECS_SV.ILPT1_FOUP_STATE_PREV, SECS_Event.ILPT1_FOUP_State_Change, "READY_TO_LOAD");
                                }
                                break;
                            case "ILPT2":
                                if (item.Value.Equals("1"))
                                {
                                    ReportEvent(SECS_SV.ILPT2_FOUP_STATE, SECS_SV.ILPT2_FOUP_STATE_PREV, SECS_Event.ILPT2_FOUP_State_Change, "READY_TO_UNLOAD");
                                }
                                else
                                {
                                    ReportEvent(SECS_SV.ILPT2_FOUP_STATE, SECS_SV.ILPT2_FOUP_STATE_PREV, SECS_Event.ILPT2_FOUP_State_Change, "READY_TO_LOAD");
                                }
                                break;
                            case "ELPT1":
                                if (item.Value.Equals("1"))
                                {
                                    ReportEvent(SECS_SV.ELPT1_FOUP_STATE, SECS_SV.ELPT1_FOUP_STATE_PREV, SECS_Event.ELPT1_FOUP_State_Change, "READY_TO_UNLOAD");
                                }
                                else
                                {
                                    ReportEvent(SECS_SV.ELPT1_FOUP_STATE, SECS_SV.ELPT1_FOUP_STATE_PREV, SECS_Event.ELPT1_FOUP_State_Change, "READY_TO_LOAD");
                                }
                                break;
                            case "ELPT2":
                                if (item.Value.Equals("1"))
                                {
                                    ReportEvent(SECS_SV.ELPT2_FOUP_STATE, SECS_SV.ELPT2_FOUP_STATE_PREV, SECS_Event.ELPT2_FOUP_State_Change, "READY_TO_UNLOAD");
                                }
                                else
                                {
                                    ReportEvent(SECS_SV.ELPT2_FOUP_STATE, SECS_SV.ELPT2_FOUP_STATE_PREV, SECS_Event.ELPT2_FOUP_State_Change, "READY_TO_LOAD");
                                }
                                break;
                            default:
                                if (!item.Key.Equals("FOUP_ROBOT"))
                                {
                                    int OffSet = Convert.ToInt32(item.Key.Replace("SHELF", "")) - 1;
                                    if (item.Value.Equals("1"))
                                    {
                                        ReportEvent(SECS_SV.SHELF_1_STATE + OffSet * 2, SECS_SV.SHELF_1_STATE_PREV + OffSet * 2, SECS_Event.Shelf_1_State_Change + OffSet * 2, "OCCUPIED");
                                    }
                                    else if (item.Value.Equals("0"))
                                    {
                                        ReportEvent(SECS_SV.SHELF_1_STATE + OffSet * 2, SECS_SV.SHELF_1_STATE_PREV + OffSet * 2, SECS_Event.Shelf_1_State_Change + OffSet * 2, "EMPTY");
                                    }
                                    else
                                    {
                                        ReportEvent(SECS_SV.SHELF_1_STATE + OffSet * 2, SECS_SV.SHELF_1_STATE_PREV + OffSet * 2, SECS_Event.Shelf_1_State_Change + OffSet * 2, "SENSOR_ERROR");
                                    }
                                }
                                break;
                        }
                    }
                    ReportEvent(SECS_SV.STOCKER_STATE, SECS_SV.STOCKER_STATE_PREV, SECS_Event.Stocker_State_Change, "IDLE");

                    break;
                case TaskFlowManagement.Command.STOP_STOCKER:
                    ReportEvent(SECS_SV.STOCKER_STATE, SECS_SV.STOCKER_STATE_PREV, SECS_Event.Stocker_State_Change, "STOPED");
                    break;
                case TaskFlowManagement.Command.RESUME_STOCKER:
                    ReportEvent(SECS_SV.STOCKER_STATE, SECS_SV.STOCKER_STATE_PREV, SECS_Event.Stocker_State_Change, "TRANSFERRING");
                    break;
                case TaskFlowManagement.Command.ABORT_STOCKER:
                    ReportEvent(SECS_SV.STOCKER_STATE, SECS_SV.STOCKER_STATE_PREV, SECS_Event.Stocker_State_Change, "RESET_REQUIRED");
                    break;
                case TaskFlowManagement.Command.WTSALIGNER_ALIGN:
                    ReportEvent(SECS_SV.ALIGNER_STATE, SECS_SV.ALIGNER_STATE_PREV, SECS_Event.ALIGNER_State_Change, "IDLE");
                    break;
                case TaskFlowManagement.Command.FOUP_ID:
                    if (Task.Params["@Target"].Equals("ILPT1"))
                    {
                        ReportEvent(SECS_SV.ILPT1_FOUP_STATE, SECS_SV.ILPT1_FOUP_STATE_PREV, SECS_Event.ILPT1_FOUP_State_Change, "ID_CONFIRMATION");
                    }
                    else if (Task.Params["@Target"].Equals("ILPT2"))
                    {
                        ReportEvent(SECS_SV.ILPT2_FOUP_STATE, SECS_SV.ILPT2_FOUP_STATE_PREV, SECS_Event.ILPT2_FOUP_State_Change, "ID_CONFIRMATION");
                    }
                    break;
                case TaskFlowManagement.Command.OPEN_FOUP:
                    if (Task.Params["@Target"].Equals("ILPT1"))
                    {

                        ReportEvent(SECS_SV.ILPT1_FOUP_STATE, SECS_SV.ILPT1_FOUP_STATE_PREV, SECS_Event.ILPT1_FOUP_State_Change, "PREMAP_CONFIRM");
                    }
                    else if (Task.Params["@Target"].Equals("ILPT2"))
                    {

                        ReportEvent(SECS_SV.ILPT2_FOUP_STATE, SECS_SV.ILPT2_FOUP_STATE_PREV, SECS_Event.ILPT2_FOUP_State_Change, "PREMAP_CONFIRM");
                    }
                    break;
                case TaskFlowManagement.Command.CLOSE_FOUP:
                    if (Task.Params["@Target"].Equals("ILPT1"))
                    {

                        ReportEvent(SECS_SV.ILPT1_FOUP_STATE, SECS_SV.ILPT1_FOUP_STATE_PREV, SECS_Event.ILPT1_FOUP_State_Change, "READY_TO_UNLOAD");
                    }
                    else if (Task.Params["@Target"].Equals("ILPT2"))
                    {

                        ReportEvent(SECS_SV.ILPT2_FOUP_STATE, SECS_SV.ILPT2_FOUP_STATE_PREV, SECS_Event.ILPT2_FOUP_State_Change, "READY_TO_UNLOAD");
                    }
                    break;
                case TaskFlowManagement.Command.MOVE_FOUP:


                    if (Task.Params["@ToPosition"].Equals("ILPT1"))
                    {
                        ReportEvent(SECS_SV.ILPT1_FOUP_STATE, SECS_SV.ILPT1_FOUP_STATE_PREV, SECS_Event.ILPT1_FOUP_State_Change, "ID_CONFIRMATION");
                    }
                    if (Task.Params["@ToPosition"].Equals("ILPT2"))
                    {
                        ReportEvent(SECS_SV.ILPT2_FOUP_STATE, SECS_SV.ILPT2_FOUP_STATE_PREV, SECS_Event.ILPT2_FOUP_State_Change, "ID_CONFIRMATION");
                    }
                    if (Task.Params["@FromPosition"].Contains("ELPT") || Task.Params["@ToPosition"].Contains("ELPT"))
                    {
                        Shelf = _Report.GetNode("SHELF");
                        if (Shelf.Status["ELPT1"].Equals("1") && (Task.Params["@FromPosition"].Equals("ELPT1") || Task.Params["@ToPosition"].Equals("ELPT1")))
                        {
                            ReportEvent(SECS_SV.ELPT1_FOUP_STATE, SECS_SV.ELPT1_FOUP_STATE_PREV, SECS_Event.ELPT1_FOUP_State_Change, "READY_TO_UNLOAD");
                        }
                        else if (Shelf.Status["ELPT1"].Equals("0") && (Task.Params["@FromPosition"].Equals("ELPT1") || Task.Params["@ToPosition"].Equals("ELPT1")))
                        {
                            ReportEvent(SECS_SV.ELPT1_FOUP_STATE, SECS_SV.ELPT1_FOUP_STATE_PREV, SECS_Event.ELPT1_FOUP_State_Change, "READY_TO_LOAD");
                        }
                        if (Shelf.Status["ELPT2"].Equals("1") && (Task.Params["@FromPosition"].Equals("ELPT2") || Task.Params["@ToPosition"].Equals("ELPT2")))
                        {
                            ReportEvent(SECS_SV.ELPT2_FOUP_STATE, SECS_SV.ELPT2_FOUP_STATE_PREV, SECS_Event.ELPT2_FOUP_State_Change, "READY_TO_UNLOAD");
                        }
                        else if (Shelf.Status["ELPT2"].Equals("0") && (Task.Params["@FromPosition"].Equals("ELPT2") || Task.Params["@ToPosition"].Equals("ELPT2")))
                        {
                            ReportEvent(SECS_SV.ELPT2_FOUP_STATE, SECS_SV.ELPT2_FOUP_STATE_PREV, SECS_Event.ELPT2_FOUP_State_Change, "READY_TO_LOAD");
                        }
                    }
                    if (Task.Params["@FromPosition"].Contains("SHELF"))
                    {
                        int Offset = Convert.ToInt32(Task.Params["@FromPosition"].Replace("SHELF", "")) - 1;
                        ReportEvent(SECS_SV.SHELF_1_STATE + Offset * 2, SECS_SV.SHELF_1_STATE_PREV + Offset * 2, SECS_Event.Shelf_1_State_Change + Offset * 2, "EMPTY");
                    }
                    if (Task.Params["@ToPosition"].Contains("SHELF"))
                    {
                        int Offset = Convert.ToInt32(Task.Params["@ToPosition"].Replace("SHELF", "")) - 1;
                        ReportEvent(SECS_SV.SHELF_1_STATE + Offset * 2, SECS_SV.SHELF_1_STATE_PREV + Offset * 2, SECS_Event.Shelf_1_State_Change + Offset * 2, "OCCUPIED");
                    }
                    ReportEvent(SECS_SV.STOCKER_STATE, SECS_SV.STOCKER_STATE_PREV, SECS_Event.Stocker_State_Change, "IDLE");
                    break;
            }


        }

        public void On_TaskJob_Ack(TaskFlowManagement.CurrentProcessTask Task)
        {
            int sysByte = 0;
            if (!int.TryParse(Task.Id, out sysByte))
            {
                return;
            }
            ReplyAck(Hack.Command_has_been_performed_later, sysByte);
            switch (Task.TaskName)
            {
                case TaskFlowManagement.Command.RESET_STOCKER:
                    ReportEvent(SECS_SV.STOCKER_STATE, SECS_SV.STOCKER_STATE_PREV, SECS_Event.Stocker_State_Change, "RESETTING");
                    break;
                case TaskFlowManagement.Command.RESET_WTS:
                    ReportEvent(SECS_SV.WTS_STATE, SECS_SV.WTS_STATE_PREV, SECS_Event.WTS_State_Change, "RESETTING");
                    break;
                case TaskFlowManagement.Command.TRANSFER_PTZ:
                    ReportEvent(SECS_SV.WTS_STATE, SECS_SV.WTS_STATE_PREV, SECS_Event.WTS_State_Change, "TRANSFERRING");
                    break;
                case TaskFlowManagement.Command.TRANSFER_WTS:
                    ReportEvent(SECS_SV.WTS_STATE, SECS_SV.WTS_STATE_PREV, SECS_Event.WTS_State_Change, "TRANSFERRING");

                    break;
                case TaskFlowManagement.Command.WTSALIGNER_ALIGN:
                    ReportEvent(SECS_SV.ALIGNER_STATE, SECS_SV.ALIGNER_STATE_PREV, SECS_Event.ALIGNER_State_Change, "OPERATING");
                    break;
                case TaskFlowManagement.Command.MOVE_FOUP:
                    ReportEvent(SECS_DV.STOCKER_SOURCE, SECS_DV.STOCKER_SOURCE_PREV, SECS_Event.Stocker_Source_Change, Task.Params["@FromPosition"]);
                    ReportEvent(SECS_DV.STOCKER_DESTINATION, SECS_DV.STOCKER_DESTINATION_PREV, SECS_Event.Stocker_Destination_Change, Task.Params["@ToPosition"]);
                    ReportEvent(SECS_SV.STOCKER_STATE, SECS_SV.STOCKER_STATE_PREV, SECS_Event.Stocker_State_Change, "TRANSFERRING");
                    if (Task.Params["@ToPosition"].Equals("ILPT1"))
                    {
                        ReportEvent(SECS_SV.ILPT1_FOUP_STATE, SECS_SV.ILPT1_FOUP_STATE_PREV, SECS_Event.ILPT1_FOUP_State_Change, "LOADING");
                    }
                    if (Task.Params["@ToPosition"].Equals("ILPT2"))
                    {
                        ReportEvent(SECS_SV.ILPT2_FOUP_STATE, SECS_SV.ILPT2_FOUP_STATE_PREV, SECS_Event.ILPT2_FOUP_State_Change, "LOADING");
                    }
                    if (Task.Params["@FromPosition"].Equals("ILPT1"))
                    {
                        ReportEvent(SECS_SV.ILPT1_FOUP_STATE, SECS_SV.ILPT1_FOUP_STATE_PREV, SECS_Event.ILPT1_FOUP_State_Change, "UNLOADING");
                    }
                    if (Task.Params["@FromPosition"].Equals("ILPT2"))
                    {
                        ReportEvent(SECS_SV.ILPT2_FOUP_STATE, SECS_SV.ILPT2_FOUP_STATE_PREV, SECS_Event.ILPT2_FOUP_State_Change, "UNLOADING");
                    }
                    if (Task.Params["@FromPosition"].Equals("ELPT1"))
                    {
                        ReportEvent(SECS_SV.ELPT1_FOUP_STATE, SECS_SV.ELPT1_FOUP_STATE_PREV, SECS_Event.ELPT1_FOUP_State_Change, "STOCKING");
                    }
                    if (Task.Params["@FromPosition"].Equals("ELPT2"))
                    {
                        ReportEvent(SECS_SV.ELPT2_FOUP_STATE, SECS_SV.ELPT2_FOUP_STATE_PREV, SECS_Event.ELPT2_FOUP_State_Change, "STOCKING");
                    }
                    if (Task.Params["@ToPosition"].Equals("ELPT1"))
                    {
                        ReportEvent(SECS_SV.ELPT1_FOUP_STATE, SECS_SV.ELPT1_FOUP_STATE_PREV, SECS_Event.ELPT1_FOUP_State_Change, "UNSTOCKING");
                    }
                    if (Task.Params["@ToPosition"].Equals("ELPT2"))
                    {
                        ReportEvent(SECS_SV.ELPT2_FOUP_STATE, SECS_SV.ELPT2_FOUP_STATE_PREV, SECS_Event.ELPT2_FOUP_State_Change, "UNSTOCKING");
                    }
                    if (Task.Params["@ToPosition"].Contains("SHELF"))
                    {
                        int Offset = Convert.ToInt32(Task.Params["@ToPosition"].Replace("SHELF", "")) - 1;
                        ReportEvent(SECS_SV.SHELF_1_STATE + Offset * 2, SECS_SV.SHELF_1_STATE_PREV + Offset * 2, SECS_Event.Shelf_1_State_Change + Offset * 2, "LOADING");
                    }
                    if (Task.Params["@FromPosition"].Contains("SHELF"))
                    {
                        int Offset = Convert.ToInt32(Task.Params["@FromPosition"].Replace("SHELF", "")) - 1;
                        ReportEvent(SECS_SV.SHELF_1_STATE + Offset * 2, SECS_SV.SHELF_1_STATE_PREV + Offset * 2, SECS_Event.Shelf_1_State_Change + Offset * 2, "UNLOADING");
                    }
                    break;
                case TaskFlowManagement.Command.OPEN_FOUP:
                    if (Task.Params["@Target"].Equals("ILPT1"))
                    {
                        ReportEvent(SECS_SV.ILPT1_FOUP_STATE, SECS_SV.ILPT1_FOUP_STATE_PREV, SECS_Event.ILPT1_FOUP_State_Change, "DOCKING");
                    }
                    else if (Task.Params["@Target"].Equals("ILPT2"))
                    {
                        ReportEvent(SECS_SV.ILPT2_FOUP_STATE, SECS_SV.ILPT2_FOUP_STATE_PREV, SECS_Event.ILPT2_FOUP_State_Change, "DOCKING");
                    }
                    break;
            }
        }

        public void On_TaskJob_Aborted(TaskFlowManagement.CurrentProcessTask Task, string NodeName, string ReportType, string Message)
        {
            //ReplyAck(Hack.Cannot_perform_now, Convert.ToInt32(Task.Id));
            switch (Task.TaskName)
            {
                case TaskFlowManagement.Command.FOUP_ID:
                    if (Task.Params["@Target"].Equals("ELPT1"))
                    {
                        ReportEvent(SECS_DV.ELPT1_FOUP_ID, SECS_DV.ELPT1_FOUP_ID_PREV, SECS_Event.ELPT1_ID_Available, "UNKNOWN", true);
                    }
                    else
                    {
                        ReportEvent(SECS_DV.ELPT2_FOUP_ID, SECS_DV.ELPT2_FOUP_ID_PREV, SECS_Event.ELPT2_ID_Available, "UNKNOWN", true);
                    }
                    break;
            }
        }

        private void ReplyAck(Hack hack, int SystemBytes)
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
            switch (Node.Type)
            {
                case "WHR":
                    switch (Txn.Method)
                    {
                        case Transaction.Command.WHR.Pick:
                        case Transaction.Command.WHR.Place:
                            if (Txn.Position.Equals("ILPT1"))
                            {
                                ReportEvent(SECS_SV.ILPT1_FOUP_STATE, SECS_SV.ILPT1_FOUP_STATE_PREV, SECS_Event.ILPT1_FOUP_State_Change, "ACCESSING");
                            }
                            else if (Txn.Position.Equals("ILPT2"))
                            {
                                ReportEvent(SECS_SV.ILPT2_FOUP_STATE, SECS_SV.ILPT2_FOUP_STATE_PREV, SECS_Event.ILPT2_FOUP_State_Change, "ACCESSING");
                            }
                            break;
                    }
                    break;
                case "CTU":
                    switch (Txn.Method)
                    {
                        case Transaction.Command.CTU.Place:
                            if (Txn.Position.Equals("PTZ"))
                            {
                                ReportEvent(SECS_SV.PTZ_STATE, SECS_SV.PTZ_STATE_PREV, SECS_Event.PTZ_State_Change, "LOADING");
                            }
                  
                            
                            break;
                        case Transaction.Command.CTU.Pick:
                            if (Txn.Position.Equals("PTZ"))
                            {
                                ReportEvent(SECS_SV.PTZ_STATE, SECS_SV.PTZ_STATE_PREV, SECS_Event.PTZ_State_Change, "UNLOADING");
                            }

                            break;
                    }
                    break;
                case "PTZ":
                    switch (Txn.Method)
                    {
                        //case Transaction.Command.PTZ.Transfer:
                        case Transaction.Command.PTZ.Home:
                            ReportEvent(SECS_SV.PTZ_STATE, SECS_SV.PTZ_STATE_PREV, SECS_Event.PTZ_State_Change, "MAPPING");
                            break;
                    }
                    break;
                case "FOUP_ROBOT":
                    switch (Txn.Method)
                    {
                        case Transaction.Command.FoupRobot.Pick:
                            if (Txn.Position.Contains("SHELF"))
                            {
                                int offset = Convert.ToInt32(Txn.Position.Replace("SHELF", "")) - 1;
                                ReportEvent(SECS_SV.SHELF_1_STATE + offset * 2, SECS_SV.SHELF_1_STATE_PREV + offset * 2, SECS_Event.Shelf_1_State_Change + offset * 2, "UNLOADING");
                            }
                            break;
                        case Transaction.Command.FoupRobot.Place:
                            if (Txn.Position.Contains("SHELF"))
                            {
                                int offset = Convert.ToInt32(Txn.Position.Replace("SHELF", "")) - 1;
                                ReportEvent(SECS_SV.SHELF_1_STATE + offset * 2, SECS_SV.SHELF_1_STATE_PREV + offset * 2, SECS_Event.Shelf_1_State_Change + offset * 2, "LOADING");
                            }
                            break;
                    }
                    break;
                case "ILPT":
                    switch (Txn.Method)
                    {
                        case Transaction.Command.ILPT.RaiseClose:
                            if (Node.Name.Equals("ILPT1"))
                            {
                                ReportEvent(SECS_SV.ILPT1_FOUP_STATE, SECS_SV.ILPT1_FOUP_STATE_PREV, SECS_Event.ILPT1_FOUP_State_Change, "POSTMAPPING");
                            }
                            else
                            {
                                ReportEvent(SECS_SV.ILPT2_FOUP_STATE, SECS_SV.ILPT2_FOUP_STATE_PREV, SECS_Event.ILPT2_FOUP_State_Change, "POSTMAPPING");
                            }
                            break;
                    }
                    break;
            }

        }

        public void On_Command_Error(Node Node, Transaction Txn, CommandReturnMessage Msg)
        {
            switch (Node.Type)
            {
                case "FOUP_ROBOT":
                    switch (Txn.Method)
                    {
                        case Transaction.Command.FoupRobot.Pick:
                            if (Txn.Position.Contains("SHELF"))
                            {
                                int offset = Convert.ToInt32(Txn.Position.Replace("SHELF", "")) - 1;
                                ReportEvent(SECS_SV.SHELF_1_STATE + offset * 2, SECS_SV.SHELF_1_STATE_PREV + offset * 2, SECS_Event.Shelf_1_State_Change + offset * 2, "SENSOR_ERROR");
                            }
                            break;
                        case Transaction.Command.FoupRobot.Place:
                            if (Txn.Position.Contains("SHELF"))
                            {
                                int offset = Convert.ToInt32(Txn.Position.Replace("SHELF", "")) - 1;
                                ReportEvent(SECS_SV.SHELF_1_STATE + offset * 2, SECS_SV.SHELF_1_STATE_PREV + offset * 2, SECS_Event.Shelf_1_State_Change + offset * 2, "SENSOR_ERROR");
                            }
                            break;
                    }
                    break;
            }
        }

        public void On_Command_Finished(Node Node, Transaction Txn, CommandReturnMessage Msg)
        {
            Node Shelf = _Report.GetNode("SHELF");
            switch (Node.Type)
            {
                case "FOUP_ROBOT":
                    switch (Txn.Method)
                    {
                        case Transaction.Command.FoupRobot.Pick:
                            if (Txn.Position.Contains("SHELF"))
                            {
                                int offset = Convert.ToInt32(Txn.Position.Replace("SHELF", "")) - 1;
                                ReportEvent(SECS_SV.SHELF_1_STATE + offset * 2, SECS_SV.SHELF_1_STATE_PREV + offset * 2, SECS_Event.Shelf_1_State_Change + offset * 2, "EMPTY");
                            }
                            break;
                        case Transaction.Command.FoupRobot.Place:
                            if (Txn.Position.Contains("SHELF"))
                            {
                                int offset = Convert.ToInt32(Txn.Position.Replace("SHELF", "")) - 1;
                                ReportEvent(SECS_SV.SHELF_1_STATE + offset * 2, SECS_SV.SHELF_1_STATE_PREV + offset * 2, SECS_Event.Shelf_1_State_Change + offset * 2, "OCCUPIED");
                            }
                            break;
                    }
                    break;
                case "ELPT":
                    switch (Txn.Method)
                    {
                        case Transaction.Command.ELPT.ReadCID:
                            if (Node.Name.Equals("ELPT1"))
                            {
                                ReportEvent(SECS_DV.ELPT1_FOUP_ID, SECS_DV.ELPT1_FOUP_ID_PREV, SECS_Event.ELPT1_ID_Available, Msg.Value, true);
                            }
                            else
                            {
                                ReportEvent(SECS_DV.ELPT2_FOUP_ID, SECS_DV.ELPT2_FOUP_ID_PREV, SECS_Event.ELPT2_ID_Available, Msg.Value, true);
                            }
                            break;

                    }
                    break;
                case "ILPT":
                    switch (Txn.Method)
                    {
                        case Transaction.Command.ILPT.Load:
                            string res = "";
                            for (int i = 0; i < Node.MappingResult.Length; i++)
                            {
                                switch (Node.MappingResult[i])
                                {
                                    case '0':
                                        res += "1";
                                        break;
                                    case '1':
                                        res += "3";
                                        break;
                                    default:
                                        res += "0";

                                        break;
                                }
                            }
                            if (Node.Name.Equals("ILPT1"))
                            {

                                ReportEvent(SECS_DV.ILPT1_MAP, SECS_DV.ILPT1_MAP_PREV, SECS_Event.ILPT1_Wafer_Map_Available, res, true);
                                break;
                            }
                            else if (Node.Name.Equals("ILPT2"))
                            {
                                ReportEvent(SECS_DV.ILPT2_MAP, SECS_DV.ILPT2_MAP_PREV, SECS_Event.ILPT2_Wafer_Map_Available, res, true);
                                break;
                            }
                            break;
                    }
                    break;
                case "PTZ":
                    switch (Txn.Method)
                    {
                        //case Transaction.Command.PTZ.Transfer:
                        case Transaction.Command.PTZ.Home:
                            string MappingState = "";
                            var odd = from job in Node.JobList.Values.ToList()
                                      where job.MapFlag && !job.ErrPosition && Convert.ToInt32(job.Slot) % 2 != 0
                                      select job;
                            var even = from job in Node.JobList.Values.ToList()
                                       where job.MapFlag && !job.ErrPosition && Convert.ToInt32(job.Slot) % 2 == 0
                                       select job;
                            var err = from job in Node.JobList.Values.ToList()
                                      where job.ErrPosition
                                      select job;
                            if (odd.Count() != 0 && even.Count() != 0)
                            {
                                MappingState = "ODDEVEN";
                            }
                            else if (odd.Count() != 0)
                            {
                                MappingState = "ODD";
                            }
                            else if (even.Count() != 0)
                            {
                                MappingState = "EVEN";
                            }
                            else if (err.Count() != 0)
                            {
                                MappingState = "PRESENT_NOT_MAPPED";
                            }
                            else
                            {
                                MappingState = "EMPTY";
                            }
                            ReportEvent(SECS_SV.PROCESS_SUBSTRATE_STATE, SECS_SV.PROCESS_SUBSTRATE_STATE_PREV, SECS_Event.Process_Substrate_State_Change, MappingState, true);
                            if (err.Count() != 0)
                            {
                                ReportEvent(SECS_DV.PTZ_MAP, SECS_DV.PTZ_MAP_PREV, SECS_Event.PTZ_Map_Available, "UNKNOWN");
                            }
                            else
                            {
                                string map = "";
                                for (int i = 0; i < Node.MappingResult.Length; i++)
                                {
                                    switch (Node.MappingResult[i])
                                    {
                                        case '0':
                                            map += "1";
                                            break;
                                        case '1':
                                            map += "3";
                                            break;
                                        default:
                                            map += "0";
                                            break;
                                    }

                                }
                                ReportEvent(SECS_DV.PTZ_MAP, SECS_DV.PTZ_MAP_PREV, SECS_Event.PTZ_Map_Available, map, true);
                                ReportEvent(SECS_SV.PTZ_STATE, SECS_SV.PTZ_STATE_PREV, SECS_Event.PTZ_State_Change, "IDLE");

                            }
                            break;
                        case "CTU":
                            switch (Txn.Method)
                            {
                                case Transaction.Command.CTU.Pick:
                                case Transaction.Command.CTU.Place:
                                    if (Txn.Position.Equals("PTZ"))
                                    {
                                        ReportEvent(SECS_SV.PROCESS_SUBSTRATE_STATE, SECS_SV.PROCESS_SUBSTRATE_STATE_PREV, SECS_Event.Process_Substrate_State_Change, "PRESENT_NOT_MAPPED");
                                    }
                                    break;
                            }
                            break;
                    }
                    break;
            }
        }
        private string GetSv(SECS_DV Sv)
        {
            QGACTIVEXLib.SV_DATA_TYPE GetFormat;
            object SvVal = null;
            //取得目前狀態
            lock (this)
            {
                axQGWrapper1.GetSV((int)Sv, out GetFormat, out SvVal);
                return SvVal.ToString();
            }
        }
        private string GetSv(SECS_SV Sv)
        {
            QGACTIVEXLib.SV_DATA_TYPE GetFormat;
            object SvVal = null;
            //取得目前狀態
            lock (this)
            {
                axQGWrapper1.GetSV((int)Sv, out GetFormat, out SvVal);
                return SvVal.ToString();
            }
        }
        private void ReportEvent(SECS_SV State, SECS_SV State_Prev, SECS_Event Event, string NewState, bool ForceReport = false)
        {
            lock (this)
            {
                object objTemp;
                QGACTIVEXLib.SV_DATA_TYPE GetFormat;
                object SvVal = null;
                //取得目前狀態

                axQGWrapper1.GetSV((int)State, out GetFormat, out SvVal);
                if (!ForceReport)
                {
                    if (SvVal.ToString().Equals(NewState))
                    {//狀態沒變，不發事件
                        return;
                    }
                    if (SvVal.ToString().Equals(""))
                    {
                        //更新SV，不發事件
                        objTemp = (object)NewState;
                        g_lOperationResult = axQGWrapper1.UpdateSV((int)State, ref objTemp);
                        objTemp = SvVal;
                        g_lOperationResult = axQGWrapper1.UpdateSV((int)State_Prev, ref objTemp);
                        return;
                    }
                }

                //更新SV
                objTemp = (object)NewState;
                g_lOperationResult = axQGWrapper1.UpdateSV((int)State, ref objTemp);
                objTemp = SvVal;
                g_lOperationResult = axQGWrapper1.UpdateSV((int)State_Prev, ref objTemp);
                //發Event report
                axQGWrapper1.EventReportSend((int)Event);
                _Report.On_Message_Log("SECS", "Event:"+Event.ToString()+" Parameters:"+State.ToString() + ", "+State_Prev.ToString());
            }
        }
        private void ReportEvent(SECS_DV State, SECS_DV State_Prev, SECS_Event Event, string NewState, bool ForceReport = false)
        {
            lock (this)
            {
                object objTemp;
                QGACTIVEXLib.SV_DATA_TYPE GetFormat;
                object SvVal = null;
                //取得目前狀態

                axQGWrapper1.GetSV((int)State, out GetFormat, out SvVal);
                if (!ForceReport)
                {
                    if (SvVal.ToString().Equals(NewState))
                    {//狀態沒變，不發事件
                        return;
                    }
                    if (SvVal.ToString().Equals(""))
                    {
                        //更新SV，不發事件
                        objTemp = (object)NewState;
                        g_lOperationResult = axQGWrapper1.UpdateSV((int)State, ref objTemp);
                        objTemp = SvVal;
                        g_lOperationResult = axQGWrapper1.UpdateSV((int)State_Prev, ref objTemp);
                        return;
                    }
                }

                //更新SV
                objTemp = (object)NewState;
                g_lOperationResult = axQGWrapper1.UpdateSV((int)State, ref objTemp);
                objTemp = SvVal;
                g_lOperationResult = axQGWrapper1.UpdateSV((int)State_Prev, ref objTemp);
                //發Event report
                axQGWrapper1.EventReportSend((int)Event);
               
            }
        }
        public void On_Command_TimeOut(Node Node, Transaction Txn)
        {

        }

        private string GetCurrentState(int ID)
        {

            QGACTIVEXLib.SV_DATA_TYPE GetFormat;
            object SvVal = null;
            //取得目前狀態

            axQGWrapper1.GetSV(ID, out GetFormat, out SvVal);

            return SvVal.ToString();
        }


        public void On_Event_Trigger(Node Node, CommandReturnMessage Msg)
        {
            if (Msg.Command.Equals("INPUT"))
            {
                object objTemp;
                QGACTIVEXLib.SV_DATA_TYPE GetFormat;
                object SvVal = null;
                string IO_Name = Msg.Value.Split(',')[0];
                string IO_Value = Msg.Value.Split(',')[1];
                Dictionary<string, string> param = new Dictionary<string, string>();

                lock (IO_State)
                {
                    bool chk = false;
                    if (IO_State.ContainsKey(IO_Name))
                    {
                        IO_State.Remove(IO_Name);
                        IO_State.Add(IO_Name, IO_Value);
                    }
                    else
                    {
                        IO_State.Add(IO_Name, IO_Value);
                    }

                    switch (IO_Name)
                    {
                        case "CTU-Present":

                            break;
                        case "PTZ-Present":

                            break;
                        case "ELPT1-R-POS-Clamp":
                        case "ELPT1-L-POS-Clamp":
                        case "ELPT1-R-POS-Unclamp":
                        case "ELPT1-L-POS-Unclamp":


                            if (IO_State.ContainsKey("ELPT1-R-POS-Clamp") && IO_State.ContainsKey("ELPT1-L-POS-Clamp") && IO_State.ContainsKey("ELPT1-R-POS-Unclamp") && IO_State.ContainsKey("ELPT1-L-POS-Unclamp"))
                            {
                                if (IO_State["ELPT1-R-POS-Clamp"].Equals("0") && IO_State["ELPT1-L-POS-Clamp"].Equals("0") && IO_State["ELPT1-R-POS-Unclamp"].Equals("0") && IO_State["ELPT1-L-POS-Unclamp"].Equals("0"))
                                {
                                    ReportEvent(SECS_SV.ELPT1_LOCK_STATE, SECS_SV.ELPT1_LOCK_STATE_PREV, SECS_Event.ELPT1_LOCK_State_Change, "MID");
                                }
                                else if (IO_State["ELPT1-R-POS-Clamp"].Equals("1") && IO_State["ELPT1-L-POS-Clamp"].Equals("1"))
                                {
                                    ReportEvent(SECS_SV.ELPT1_LOCK_STATE, SECS_SV.ELPT1_LOCK_STATE_PREV, SECS_Event.ELPT1_LOCK_State_Change, "LOCKED");
                                }
                                else if (IO_State["ELPT1-R-POS-Clamp"].Equals("0") && IO_State["ELPT1-L-POS-Clamp"].Equals("0"))
                                {
                                    ReportEvent(SECS_SV.ELPT1_LOCK_STATE, SECS_SV.ELPT1_LOCK_STATE_PREV, SECS_Event.ELPT1_LOCK_State_Change, "UNLOCKED");
                                }
                            }




                            break;
                        case "ELPT2-R-POS-Clamp":
                        case "ELPT2-L-POS-Clamp":
                        case "ELPT2-R-POS-Unclamp":
                        case "ELPT2-L-POS-Unclamp":



                            if (IO_State.ContainsKey("ELPT2-R-POS-Clamp") && IO_State.ContainsKey("ELPT2-L-POS-Clamp") && IO_State.ContainsKey("ELPT2-R-POS-Unclamp") && IO_State.ContainsKey("ELPT2-L-POS-Unclamp"))
                            {
                                if (IO_State["ELPT2-R-POS-Clamp"].Equals("0") && IO_State["ELPT2-L-POS-Clamp"].Equals("0") && IO_State["ELPT2-R-POS-Unclamp"].Equals("0") && IO_State["ELPT2-L-POS-Unclamp"].Equals("0"))
                                {
                                    ReportEvent(SECS_SV.ELPT2_LOCK_STATE, SECS_SV.ELPT2_LOCK_STATE_PREV, SECS_Event.ELPT2_LOCK_State_Change, "MID");
                                }
                                else if (IO_State["ELPT2-R-POS-Clamp"].Equals("1") && IO_State["ELPT2-L-POS-Clamp"].Equals("1"))
                                {
                                    ReportEvent(SECS_SV.ELPT2_LOCK_STATE, SECS_SV.ELPT2_LOCK_STATE_PREV, SECS_Event.ELPT2_LOCK_State_Change, "LOCKED");
                                }
                                else if (IO_State["ELPT2-R-POS-Clamp"].Equals("0") && IO_State["ELPT2-L-POS-Clamp"].Equals("0"))
                                {
                                    ReportEvent(SECS_SV.ELPT2_LOCK_STATE, SECS_SV.ELPT2_LOCK_STATE_PREV, SECS_Event.ELPT2_LOCK_State_Change, "UNLOCKED");
                                }
                            }




                            break;
                        case "ILPT1-POS-Clamp":
                        case "ILPT1-POS-Unclamp":
                            if (IO_State.ContainsKey("ILPT1-POS-Clamp") && IO_State.ContainsKey("ILPT1-POS-Unclamp"))
                            {
                                if (IO_State["ILPT1-POS-Clamp"].Equals("0") && IO_State["ILPT1-POS-Unclamp"].Equals("0"))
                                {
                                    ReportEvent(SECS_SV.ILPT1_LOCK_STATE, SECS_SV.ILPT1_LOCK_STATE_PREV, SECS_Event.ILPT1_LOCK_State_Change, "MID");
                                }
                                else if (IO_State["ILPT1-POS-Clamp"].Equals("1"))
                                {
                                    ReportEvent(SECS_SV.ILPT1_LOCK_STATE, SECS_SV.ILPT1_LOCK_STATE_PREV, SECS_Event.ILPT1_LOCK_State_Change, "LOCKED");
                                }
                                else if (IO_State["ILPT1-POS-Unclamp"].Equals("1"))
                                {
                                    ReportEvent(SECS_SV.ILPT1_LOCK_STATE, SECS_SV.ILPT1_LOCK_STATE_PREV, SECS_Event.ILPT1_LOCK_State_Change, "UNLOCKED");
                                }
                            }
                            break;
                        case "ELPT2-POS-Clamp":
                        case "ELPT2-POS-Unclamp":
                            if (IO_State.ContainsKey("ELPT2-POS-Clamp") && IO_State.ContainsKey("ELPT2-POS-Unclamp"))
                            {
                                if (IO_State["ELPT2-POS-Clamp"].Equals("0") && IO_State["ELPT2-POS-Unclamp"].Equals("0"))
                                {
                                    ReportEvent(SECS_SV.ELPT2_LOCK_STATE, SECS_SV.ELPT2_LOCK_STATE_PREV, SECS_Event.ELPT2_LOCK_State_Change, "MID");
                                }
                                else if (IO_State["ELPT2-POS-Clamp"].Equals("1"))
                                {
                                    ReportEvent(SECS_SV.ELPT2_LOCK_STATE, SECS_SV.ELPT2_LOCK_STATE_PREV, SECS_Event.ELPT2_LOCK_State_Change, "LOCKED");
                                }
                                else if (IO_State["ELPT2-POS-Unclamp"].Equals("1"))
                                {
                                    ReportEvent(SECS_SV.ELPT2_LOCK_STATE, SECS_SV.ELPT2_LOCK_STATE_PREV, SECS_Event.ELPT2_LOCK_State_Change, "UNLOCKED");
                                }
                            }
                            break;

                        case "ILPT1-POS-Dock":
                            if (IO_Value.Equals("1"))
                            {
                                ReportEvent(SECS_SV.ILPT1_FOUP_STATE, SECS_SV.ILPT1_FOUP_STATE_PREV, SECS_Event.ILPT1_FOUP_State_Change, "DOCKED");
                            }
                            break;
                        case "ILPT2-POS-Dock":
                            if (IO_Value.Equals("1"))
                            {
                                ReportEvent(SECS_SV.ILPT2_FOUP_STATE, SECS_SV.ILPT2_FOUP_STATE_PREV, SECS_Event.ILPT2_FOUP_State_Change, "DOCKED");
                            }
                            break;
                        case "STK-ROB-POS-Clamp":
                            if (IO_Value.Equals("1"))
                            {
                                ReportEvent(SECS_SV.GRIPPER_FOUP_STATE, SECS_SV.GRIPPER_FOUP_STATE_PREV, SECS_Event.Gripper_FOUP_State_Change, "GRIPPERS_OCCUPIED");
                            }
                            break;
                        case "STK-ROB-POS-Unclamp":
                            if (IO_Value.Equals("1"))
                            {
                                ReportEvent(SECS_SV.GRIPPER_FOUP_STATE, SECS_SV.GRIPPER_FOUP_STATE_PREV, SECS_Event.Gripper_FOUP_State_Change, "GRIPPERS_EMPTY");
                            }
                            break;
                        case "ELPT1-Place1":
                        case "ELPT1-Place2":
                        case "ELPT1-Place3":


                            if (IO_State.ContainsKey("ELPT1-Place1") && IO_State.ContainsKey("ELPT1-Place2") && IO_State.ContainsKey("ELPT1-Place3"))
                            {
                                if (IO_State["ELPT1-Place1"].Equals("1") && IO_State["ELPT1-Place2"].Equals("1") && IO_State["ELPT1-Place3"].Equals("1"))
                                {
                                    chk = true;

                                }
                                else
                                {

                                }
                            }
                            if (chk)
                            {
                                //param.Add("@Target", "ELPT1");
                                //_Report.NewTask(Guid.NewGuid().ToString(), TaskFlowManagement.Command.CLAMP_ELPT, param);
                            }
                            else
                            {
                                //ReportEvent(SECS_SV.ELPT1_FOUP_STATE, SECS_SV.ELPT1_FOUP_STATE_PREV, SECS_Event.ELPT1_FOUP_State_Change, "READY_TO_LOAD");
                            }
                            break;
                        case "ELPT2-Place1":
                        case "ELPT2-Place2":
                        case "ELPT2-Place3":
                            if (IO_State.ContainsKey("ELPT2-Place1") && IO_State.ContainsKey("ELPT2-Place2") && IO_State.ContainsKey("ELPT2-Place3"))
                            {
                                if (IO_State["ELPT2-Place1"].Equals("1") && IO_State["ELPT2-Place2"].Equals("1") && IO_State["ELPT2-Place3"].Equals("1"))
                                {
                                    chk = true;

                                }
                                else
                                {

                                }
                            }
                            if (chk)
                            {

                            }
                            else
                            {
                                //ReportEvent(SECS_SV.ELPT2_FOUP_STATE, SECS_SV.ELPT2_FOUP_STATE_PREV, SECS_Event.ELPT2_FOUP_State_Change, "READY_TO_LOAD");
                            }
                            break;

                    }
                }
            }
        }
        bool NodeDataAvailable = false;
        Node ReqNode = null;
        private Node GetNode(string Name)
        {

            return ReqNode;
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

        public void On_Alarm_Happen(TransferControl.Management.AlarmManagement.AlarmInfo Alarm)
        {
            SendAlarmMsg((long)Math.Pow(2, 7), Convert.ToInt64(Alarm.ALID.Equals("") ? "0" : Alarm.ALID), Alarm.ALTX);
            Node Node = _Report.GetNode(Alarm.NodeName);
            if (Node != null)
            {
                switch (Node.Type)
                {
                    case "ELPT":
                        if (Node.Name.Equals("ELPT1"))
                        {
                            ReportEvent(SECS_SV.ELPT1_FOUP_STATE, SECS_SV.ELPT1_FOUP_STATE_PREV, SECS_Event.ELPT1_FOUP_State_Change, "ALARM");
                        }
                        else
                        {
                            ReportEvent(SECS_SV.ELPT2_FOUP_STATE, SECS_SV.ELPT2_FOUP_STATE_PREV, SECS_Event.ELPT2_FOUP_State_Change, "ALARM");
                        }
                        ReportEvent(SECS_SV.STOCKER_STATE, SECS_SV.STOCKER_STATE_PREV, SECS_Event.Stocker_State_Change, "ALARM");
                        break;
                    case "ILPT":
                        if (Node.Name.Equals("ILPT1"))
                        {
                            ReportEvent(SECS_SV.ILPT1_FOUP_STATE, SECS_SV.ILPT1_FOUP_STATE_PREV, SECS_Event.ILPT1_FOUP_State_Change, "ALARM");
                        }
                        else
                        {
                            ReportEvent(SECS_SV.ILPT2_FOUP_STATE, SECS_SV.ILPT2_FOUP_STATE_PREV, SECS_Event.ILPT2_FOUP_State_Change, "ALARM");
                        }
                        ReportEvent(SECS_SV.STOCKER_STATE, SECS_SV.STOCKER_STATE_PREV, SECS_Event.Stocker_State_Change, "ALARM");
                        break;
                    case "FOUP_ROBOT":
                        ReportEvent(SECS_SV.STOCKER_STATE, SECS_SV.STOCKER_STATE_PREV, SECS_Event.Stocker_State_Change, "ALARM");
                        break;
                    case "SHELF":
                        break;
                    case "WHR":
                        ReportEvent(SECS_SV.WTS_STATE, SECS_SV.WTS_STATE_PREV, SECS_Event.WTS_State_Change, "ALARM");
                        break;
                    case "WTS_ALIGNER":
                        ReportEvent(SECS_SV.WTS_STATE, SECS_SV.WTS_STATE_PREV, SECS_Event.WTS_State_Change, "ALARM");
                        break;
                    case "CTU":
                        ReportEvent(SECS_SV.WTS_STATE, SECS_SV.WTS_STATE_PREV, SECS_Event.WTS_State_Change, "ALARM");
                        break;
                    case "PTZ":
                        ReportEvent(SECS_SV.PTZ_STATE, SECS_SV.PTZ_STATE, SECS_Event.PTZ_State_Change, "ALARM");
                        ReportEvent(SECS_SV.WTS_STATE, SECS_SV.WTS_STATE_PREV, SECS_Event.WTS_State_Change, "ALARM");
                        break;
                }
            }
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

        Node IUserInterfaceReport.GetNode(string Name)
        {
            throw new NotImplementedException();
        }

        enum SECS_Event
        {
            Stocker_State_Change = 5001,
             WTS_State_Change = 5002,
             System_Mode_Change = 5004,
             Stocker_Source_Change = 5006,
             Stocker_Destination_Change = 5008,
             ELPT1_Mode_Change = 5100,
             ELPT1_FOUP_State_Change = 5102,
             ELPT1_LOCK_State_Change = 5104,
             ELPT2_Mode_Change = 5110,
             ELPT2_FOUP_State_Change = 5112,
             ELPT2_LOCK_State_Change = 5114,
             ILPT1_Mode_Change = 5200,
             ILPT1_FOUP_State_Change = 5202,
             ILPT1_LOCK_State_Change = 5204,
             ILPT2_Mode_Change = 5210,
             ILPT2_FOUP_State_Change = 5212,
             ILPT2_LOCK_State_Change = 5214,
             Gripper_FOUP_State_Change = 5250,
             Shelf_1_State_Change = 5300,
             Shelf_2_State_Change = 5302,
             Shelf_3_State_Change = 5304,
             Shelf_4_State_Change = 5306,
             Shelf_5_State_Change = 5308,
             Shelf_6_State_Change = 5310,
             Shelf_7_State_Change = 5312,
             Shelf_8_State_Change = 5314,
             Shelf_9_State_Change = 5316,
             Shelf_10_State_Change = 5318,
             Shelf_11_State_Change = 5320,
             Shelf_12_State_Change = 5322,
             Shelf_13_State_Change = 5324,
             Shelf_14_State_Change = 5326,
             Shelf_15_State_Change = 5328,
             Shelf_16_State_Change = 5330,
             Shelf_17_State_Change = 5332,
             Shelf_18_State_Change = 5334,
             PTZ_State_Change = 5340,
             Process_Carrier_WTS_State_Change = 5344,
             ELPT1_ID_Available = 5520,
             ELPT2_ID_Available = 5521,
             ILPT1_ID_Available = 5522,
             ILPT2_ID_Available = 5523,
             ILPT1_Wafer_Map_Available = 5530,
             ILPT2_Wafer_Map_Available = 5531,
             PTZ_Map_Available = 5532,
             ALIGNER_State_Change = 5551,
             Process_Substrate_State_Change = 5552,
             Light_Curtain_State_Change = 5600,
             ESTOP_State_Change = 5602
        }
        enum SECS_DV
        {
            ELPT1_FOUP_ID = 7000,
             ELPT1_FOUP_ID_PREV = 7001,
             ELPT2_FOUP_ID = 7002,
             ELPT2_FOUP_ID_PREV = 7003,
             ILPT1_FOUP_ID = 7010,
             ILPT1_FOUP_ID_PREV = 7011,
             ILPT2_FOUP_ID = 7012,
             ILPT2_FOUP_ID_PREV = 7013,
             ILPT1_MAP = 7020,
             ILPT1_MAP_PREV = 7021,
             ILPT2_MAP = 7022,
             ILPT2_MAP_PREV = 7023,
             PTZ_MAP = 7030,
             PTZ_MAP_PREV = 7031,
             STOCKER_SOURCE = 7040,
             STOCKER_SOURCE_PREV = 7041,
             STOCKER_DESTINATION = 7050,
             STOCKER_DESTINATION_PREV = 7051,
             SYSTEM_MODE = 7060,
             SYSTEM_MODE_PREV = 7061
        }
        enum SECS_SV
        {
            STOCKER_STATE = 9000,
            STOCKER_STATE_PREV = 9001,
            WTS_STATE = 9002,
            WTS_STATE_PREV = 9003,
            RESERVED = 9004,
            RESERVED_PREV = 9005,
            ELPT1_MODE = 9100,
            ELPT1_MODE_PREV = 9101,
            ELPT1_FOUP_STATE = 9104,
            ELPT1_FOUP_STATE_PREV = 9105,
            ELPT1_LOCK_STATE = 9106,
            ELPT1_LOCK_STATE_PREV = 9107,
            ELPT2_MODE = 9110,
            ELPT2_MODE_PREV = 9111,
            ELPT2_FOUP_STATE = 9114,
            ELPT2_FOUP_STATE_PREV = 9115,
            ELPT2_LOCK_STATE = 9116,
            ELPT2_LOCK_STATE_PREV = 9117,
            ILPT1_MODE = 9200,
            ILPT1_MODE_PREV = 9201,
            ILPT1_FOUP_STATE = 9204,
            ILPT1_FOUP_STATE_PREV = 9205,
            ILPT1_LOCK_STATE = 9206,
            ILPT1_LOCK_STATE_PREV = 9207,
            ILPT2_MODE = 9210,
            ILPT2_MODE_PREV = 9211,
            ILPT2_FOUP_STATE = 9214,
            ILPT2_FOUP_STATE_PREV = 9215,
            ILPT2_LOCK_STATE = 9216,
            ILPT2_LOCK_STATE_PREV = 9217,
            GRIPPER_FOUP_STATE = 9250,
            GRIPPER_FOUP_STATE_PREV = 9251,
            SHELF_1_STATE = 9300,
            SHELF_1_STATE_PREV = 9301,
            SHELF_2_STATE = 9302,
            SHELF_2_STATE_PREV = 9303,
            SHELF_3_STATE = 9304,
            SHELF_3_STATE_PREV = 9305,
            SHELF_4_STATE = 9306,
            SHELF_4_STATE_PREV = 9307,
            SHELF_5_STATE = 9308,
            SHELF_5_STATE_PREV = 9309,
            SHELF_6_STATE = 9310,
            SHELF_6_STATE_PREV = 9311,
            SHELF_7_STATE = 9312,
            SHELF_7_STATE_PREV = 9313,
            SHELF_8_STATE = 9314,
            SHELF_8_STATE_PREV = 9315,
            SHELF_9_STATE = 9316,
            SHELF_9_STATE_PREV = 9317,
            SHELF_10_STATE = 9318,
            SHELF_10_STATE_PREV = 9319,
            SHELF_11_STATE = 9320,
            SHELF_11_STATE_PREV = 9321,
            SHELF_12_STATE = 9322,
            SHELF_12_STATE_PREV = 9323,
            SHELF_13_STATE = 9324,
            SHELF_13_STATE_PREV = 9325,
            SHELF_14_STATE = 9326,
            SHELF_14_STATE_PREV = 9327,
            SHELF_15_STATE = 9328,
            SHELF_15_STATE_PREV = 9329,
            SHELF_16_STATE = 9330,
            SHELF_16_STATE_PREV = 9331,
            SHELF_17_STATE = 9332,
            SHELF_17_STATE_PREV = 9333,
            SHELF_18_STATE = 9334,
            SHELF_18_STATE_PREV = 9335,
            PROCESS_SUBSTRATE_STATE = 9342,
            PROCESS_SUBSTRATE_STATE_PREV = 9343,
            PTZ_STATE = 9344,
            PTZ_STATE_PREV = 9345,
            ALIGNER_STATE = 9360,
            ALIGNER_STATE_PREV = 9361,
            LIGHT_CURTAIN_STATE = 9400,
            LIGHT_CURTAIN_STATE_PREV = 9401,
            ESTOP_STATE = 9402,
            ESTOP_STATE_PREV = 9403
        }
    }
}
