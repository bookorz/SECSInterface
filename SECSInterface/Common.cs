using System.Collections.Generic;
using System.Collections;
using System.IO;
using System;

namespace SECSInterface
{
    static class Common
    {
        //E30
     
  

        public static bool blnShowSECSUI;

        public static bool blnShowFormDemo;

        public static int g_System_Start_Delay_Time;               //設定系統啟動延遲時間
        public static string G_SystemTitle;                        //設定系統Type與機型
        public static string G_GEM_MDLN;                           //GEM SV中的機台型號，最多6個字
        public static int g_LogReserveTime;                        //Log保留月數
        public static int g_InitialFlag;                           //0:非應用程式啟始狀態     1:應用程式啟始狀態

        public static bool blnUpdateEQPSVData;
        public static bool blnUpdateSECSSVData;
        public static bool blnUpdateEQPAlarmData;
        public static bool blnUpdateSECSStatusData;

        public static bool blnUpdate_Process_E87_LPTSM;

        public static Queue WaferLeaveCompletedEventQueue = new Queue();     //儲存的值的格式：WaferLeaveCompletedEventQueue=(Slot No,WaferMoveSourceLocation,WaferMoveDestinationLocation)
        public static Queue WaferMoveCompletedEventQueue = new Queue();     //儲存的值的格式：WaferMoveCompletedEvent=(Slot No,WaferMoveSourceLocation,WaferMoveDestinationLocation)

        // Test mode setting 
        public static int g_BarcodeTestMode;  // 0: normal mode, 1:Test Mode (no barcode reader)
        public static int g_EqpInitTimer;     //Equipment Initial Timer
        public static int g_InitShowForm;     //0: Show FormDemo    1:Show FormSecsGem    

        //E30
        public static int g_TotalAlarmQTY;
        public static int g_MinAlarmID;
        public static int g_MaxAlarmID;

        public static string g_RemoteRCMD;
        public static string g_LocalRCMD;

        public static bool g_Update_iAlarmStatus;
        public static int[] g_iAlarmStatus;

        //E40
        public static int g_PJ_Space;

        //E87
        public static int g_Carrier_Space;
        public static int g_SlotSpaceInCassette;                //機台使用的Cassette(s)的最大Slot數量
        public static int g_PTN_Space;      //PTN：Port Number
        public static int g_LPTSM_PortID_1;
        public static int g_LPTSM_PortID_2;
        public static int g_LPTSM_PortID_3;
        public static int g_LPTSM_PortID_4;
        public static int g_LPTSM_PortID_5;
        public static int g_LPTSM_PortID_6;
        public static int g_LPTSM_PortID_7;
        public static int g_LPTSM_PortID_8;

        //**Allow NULL Sutstrate ID setting
        public static string g_Allow_NULL_SubstID;      //Y:Allow NULL Substrate ID, N: Not allow NULL Substrate ID
                                                        //**Allow NULL Sutstrate ID setting

        //E90
        public static int g_ST_Location_Space;
        public static string[] g_LocationID;
        public static int g_ST_Step_Space;

        //E94
        public static int g_CJ_Space;

        public const int CJ_MAX_CARRIER_INPUT_SPEC_QTY = 8;

        public enum RECIPE_CHANGE_STATUS
        {
            ADDED = 1,
            MODIFY = 2,
            DELETE = 3
        }

        public enum CARRIER_CLAMP_MODE
        {
            MANUAL_CLAMP = 0,
            AUTO_CLAMP = 1
        }

        public enum CARRIER_RELEASE_MODE
        {
            WAIT_HOST_COMMAND = 0,
            AUTO_RELEASE = 1
        }

        public enum TAG_TIME
        {
            TIME_NOT_THING = -1,
            TIME_UP = 0,
            TIME_START = 5
        }

        public enum TAG_RESULT
        {
            NOT_THING = -1,
            OK = 0,
            ERROR = 1
        }

        // note: Stage must use 1 ~ n
        public enum WAFER_LOCATION
        {
            CARRIER_0 = 0,
            STAGE_1 = 1,    // AlignmentArea
            STAGE_2 = 2,    // MacroArea
            STAGE_3 = 3     // MicroArea
        }

        public enum WAFER_PROCESS_STEP
        {
            STEP_1 = 1,     // AlignmentArea (Cassette => AlignmentArea)
            STEP_2 = 2,     // MacroArea (AlignmentArea => MacroArea)
            STEP_3 = 3,    // MicroArea (MacroArea => MicroArea)  
        }

        public enum HOST_CMD
        {
            NOTHING = 0,
            START = 1,
            PAUSE = 2,
            RESUME = 3,
            STOP = 4,
            ABORT = 5
        }

        public enum EQP_STATUS
        {
            INITIAL = 0,
            IDLE = 1,
            RUN = 2,
            PAUSE = 3,
            ASSIST = 4,
            STOP = 5
        }

        public enum SECS_CONNECTION_STATE
        {
            DISCONNECTION_STOP = 0,
            DISCONNECTION_START = 1,
            CONNECTION = 2
        }

        public enum COMMUNICATING_STATE
        {
            DISABLE = 1,
            NOT_COMMUNICATING = 3,
            COMMUNICATING = 7
        }

        public enum CONTROL_STATE
        {
            OFFLINE_EQP_OFFLINE = 1,
            OFFLINE_EQP_ATTEMP_ONLINE = 2,
            OFFLINE_HOST_OFFLINE = 3,
            ONLINE_LOCAL = 4,
            ONLINE_REMOTE = 5
        }

        public enum WAFER_PROCESS_STATE
        {
            EMPTY = 0,
            NOT_PROCESS = 1,
            AT_STAGE_1 = 2,     //停在Alignment Area
            AT_STAGE_2 = 3,     //停在Macro Area
            PROCESS_COMPLETED = 4,
            AT_STAGE_3 = 5,     //停在Micro / Printer Area
            PROCESS_ERROR = 6
        }

        public enum PROCESS_RESULT
        {
            PROCESS_RESULT_DEFAULT = 0,
            PROCESS_RESULT_NORMAL_END = 1,
            PROCESS_RESULT_ABNORMAL_END = 2
        }

        public struct LOAD_PORT
        {
            // Eqp Event
            public int SlotMapCompletedEvent;
            public int CarrierAccessingStatus_2_CarrierCompletedEvent; // 1表 LP 完成了.
            public int CarrierAccessingStatus_3_CarrierStoppedEvent;

            // Eqp Data
            public int LoadPortServiceStatus;
            public int LoadPortStatus;      //0:  Unload Complete   1: Load Complete 
            public int CarrierClampStatus;
            public int CarrierDockStatus;

            public int[] SlotMapResult;

        }
        public struct EQP_DATA
        {
            // Eqp Event
            public int RecipeChangeEvent;
            public int CurrentRecipeChangeEvent;
            public int LotStartEvent;
            public int LotEndEvent;
            public int WaferStartEvent;
            public int WaferEndEvent;
            public int AlignStartEvent;
            public int AlignEndEvent;
            public int ProcessStartEvent;
            public int ProcessEndEvent;
            public int WaferLeaveCompletedEvent;
            public int WaferMoveCompletedEvent;
            public int SecsConfigRequest;
            public int MacroProcessStartEvent;
            public int MacroProcessEndEvent;

            public int UpdateSVRequest;
            public int RequestAlarmReply;

            // Eqp data
            public int EqpInitState;  //0: initializing, 1:Initial finished(set by Eqp), 2:SECS ack(set by SECS)
            public string RecipeNameChanged;
            public string CurrentRecipeName;
            public int EqpStatus;
            public int LoadPortServiceStatus;
            public int ProcessStatus;
            public int TableVacuumStatus;
            public int GrapVacuumStatus;
            public int CurrentProcessWaferSlotNo;
            public int[] SlotProcessStatus;
            public int WaferMoveSourceLocation;
            public int WaferMoveDestinationLocation;
            public int[] AlarmStatus;
            public LOAD_PORT[] LP;

            public int AlignmentAreaWaferSlotNo;
            public int WaferProcessSlotNo;
            public int AlignmentAreaProcessSlotNo;
            public int MicroAreaProcessSlotNo;
            public int MacroAreaProcessSlotNo;
            public int AlignResult;
            public int AlignmentVacuumStatus;
            public int MacroAreaWaferSlotNo;
            public int SlotSpace;
            public int CurrentPTN;
        }

        public struct CARRIER
        {
            //SECS Command
            public int SECSReadCarrierIDComplete;  //0: nothing        1: SECS讀到RFID值(normal)   2: SECS讀不到RFID值(abnormal)        

            public int HostClampCarrierCmd;
            public int HostOpenCarrierCmd;
            public int HostRejectCarrierCmd;
            public int HostReleaseCarrierCmd;
            //public int HostSelectRecipeCmd;
            //0: nothing   1: Host要求Process Start (需搭配ProcessWaferSlot,ProcessWaferNumber)  2: Host要求Process Pause
            //3: Host要求Process Resume   4:Host 要求Stop   5:Host 要求Abort
            //public int HostProcessCmd;
            public int HostRecipeChangeNotify;  //0: nothing        1: 己刪除或下載某recipe, 請 Eqp更新 Recipe show list

            //SECS Data
            public string CarrierID;
            public string[] LotID;
            public string[] SubStID;
            public int CarrierIndex; // 記錄 Carrier 的 index
                                     //public string RecipeSelecteName;
        }

        public struct SECS_DATA
        {
            // Host command
            public int HostClampCarrierCmd;
            //public int HostOpenCarrierCmd;
            //public int HostRejectCarrierCmd;
            //public int HostReleaseCarrierCmd;
            public int HostSelectRecipeCmd;
            public int HostTerminalMsgCmd;

            //0: nothing   1: Host要求Process Start (需搭配ProcessWaferSlot,ProcessWaferNumber)  2: Host要求Process Pause
            //3: Host要求Process Resume   4:Host 要求Stop   5:Host 要求Abort
            public int HostProcessCmd;
            public int HostRecipeChangeNotify;  //0: nothing        1: 己刪除或下載某recipe, 請 Eqp更新 Recipe show list

            public int UpdateSVRequest;

            //public int SECSReadCarrierIDComplete;  //0: nothing        1: SECS讀到RFID值(normal)   2: SECS讀不到RFID值(abnormal)        

            // Host data
            //public int GEM_ControlState;
            public CONTROL_STATE GEM_ControlState;

            //public int SECS_ConnectionStatus;   //0:disconnect(stop), 1:disconnect(start), 2:connection
            public SECS_CONNECTION_STATE SECS_ConnectionStatus;   //0:disconnect(stop), 1:disconnect(start), 2:connection
            public COMMUNICATING_STATE GEM_Communicating_State;   //DISABLE = 1,NOT_COMMUNICATING = 3,COMMUNICATING = 7

            public int GEM_EqpStatus;         // this is real eqp status for host
            public string RecipeSelecteName;
            //public string CarrierID;
            public int CarrierClampMode;
            public int CarrierReleaseMode;
            public int ProcessWaferNumber;
            public int[] ProcessWaferSlot;
            public int ProcessPTN;
            public CARRIER[] LP;

            public string[] LotID;
            public string[] SubStID;

            public string[] test;

            public string HostRecipeChangePPID;
            public string HostTerminalMsg;

            public PROCESS_RESULT EQP_ProcessResult;    //0:default  1:Normal End  2:Abnormal End
        }

        public struct TAG_DATA
        {
            // Tag command / event
            public int ReadTagCmd;  // Down Counter
            public int ShowTagUiRequest;
            public int TagResultEvent;
            public int ErrorEvent;

            // Tag data
            public string TagData;
            public string ErrorMessage;

        }

        public static SECS_DATA SECS_Data;
        public static EQP_DATA EQP_Data;
        public static TAG_DATA TAG_Data;

        public static void CommonDataReset(int p)
        {


            SECS_Data.LP[p].CarrierID = "";
            SECS_Data.LP[p].SECSReadCarrierIDComplete = 0;
            SECS_Data.LP[p].HostClampCarrierCmd = 0;
            SECS_Data.LP[p].HostOpenCarrierCmd = 0;
            SECS_Data.LP[p].HostRejectCarrierCmd = 0;
            SECS_Data.LP[p].HostReleaseCarrierCmd = 0; ;
            SECS_Data.HostSelectRecipeCmd = 0;
            SECS_Data.HostProcessCmd = 0;
            SECS_Data.LP[p].HostRecipeChangeNotify = 0;

            EQP_Data.LP[p].SlotMapCompletedEvent = 0;
            EQP_Data.LP[p].CarrierAccessingStatus_2_CarrierCompletedEvent = 0;
            EQP_Data.LP[p].CarrierAccessingStatus_3_CarrierStoppedEvent = 0;

            EQP_Data.LP[p].LoadPortStatus = 0;

            for (int i = EQP_Data.LP[p].SlotMapResult.GetLowerBound(0); i <= EQP_Data.LP[p].SlotMapResult.GetUpperBound(0); i++)
            {
                EQP_Data.LP[p].SlotMapResult[i] = 0; //!
            }


            /*

            //TAG_Data
            TAG_Data.TagData = "";
            TAG_Data.ErrorEvent = 0;
            TAG_Data.ShowTagUiRequest = 0;
            TAG_Data.ErrorMessage = "";
            TAG_Data.TagResultEvent = (int)Common.TAG_RESULT.NOT_THING;
            */

        }
        public static void CommonAllDataReset()
        {
            int p;

            for (p = 1; p <= Common.g_PTN_Space; p++)
            {
                SECS_Data.LP[p].CarrierID = "";
                SECS_Data.LP[p].SECSReadCarrierIDComplete = 0;
                SECS_Data.LP[p].HostClampCarrierCmd = 0;
                SECS_Data.LP[p].HostOpenCarrierCmd = 0;
                SECS_Data.LP[p].HostRejectCarrierCmd = 0;
                SECS_Data.LP[p].HostReleaseCarrierCmd = 0; ;
                SECS_Data.HostSelectRecipeCmd = 0;
                SECS_Data.HostProcessCmd = 0;
                SECS_Data.LP[p].HostRecipeChangeNotify = 0;

            }



            //SECS_Data
            for (int i = SECS_Data.test.GetLowerBound(0); i <= SECS_Data.test.GetUpperBound(0); i++)
            {
                SECS_Data.test[i] = "";
            }

            for (int i = SECS_Data.ProcessWaferSlot.GetLowerBound(0); i <= SECS_Data.ProcessWaferSlot.GetUpperBound(0); i++)
            {
                SECS_Data.ProcessWaferSlot[i] = 0;
            }

            SECS_Data.ProcessWaferNumber = 0;
            SECS_Data.GEM_EqpStatus = 0;

            SECS_Data.HostClampCarrierCmd = 0;
            //SECS_Data.HostOpenCarrierCmd = 0;
            //SECS_Data.HostRejectCarrierCmd = 0;
            //SECS_Data.HostReleaseCarrierCmd = 0; ;
            SECS_Data.HostSelectRecipeCmd = 0;
            SECS_Data.HostProcessCmd = 0;
            SECS_Data.HostRecipeChangeNotify = 0;

            for (int i = SECS_Data.LotID.GetLowerBound(0); i <= SECS_Data.LotID.GetUpperBound(0); i++)
            {
                SECS_Data.LotID[i] = "";
            }

            for (int i = SECS_Data.SubStID.GetLowerBound(0); i <= SECS_Data.SubStID.GetUpperBound(0); i++)
            {
                SECS_Data.SubStID[i] = "";
            }

            SECS_Data.UpdateSVRequest = 0;
            SECS_Data.HostRecipeChangePPID = "";

            SECS_Data.EQP_ProcessResult = PROCESS_RESULT.PROCESS_RESULT_DEFAULT;

            //EQP_Data
            EQP_Data.RecipeChangeEvent = 0;
            EQP_Data.CurrentRecipeChangeEvent = 0;

            EQP_Data.LotStartEvent = 0;
            EQP_Data.LotEndEvent = 0;
            EQP_Data.WaferStartEvent = 0;
            EQP_Data.WaferEndEvent = 0;
            EQP_Data.AlignStartEvent = 0;
            EQP_Data.AlignEndEvent = 0;
            EQP_Data.ProcessStartEvent = 0;
            EQP_Data.ProcessEndEvent = 0;
            EQP_Data.WaferLeaveCompletedEvent = 0;
            EQP_Data.WaferMoveCompletedEvent = 0;
            EQP_Data.MacroProcessStartEvent = 0;
            EQP_Data.MacroProcessEndEvent = 0;
            for (p = 1; p <= Common.g_PTN_Space; p++)
            {
                EQP_Data.LP[p].SlotMapCompletedEvent = 0;
                EQP_Data.LP[p].CarrierAccessingStatus_2_CarrierCompletedEvent = 0;
                EQP_Data.LP[p].CarrierAccessingStatus_3_CarrierStoppedEvent = 0;
                EQP_Data.LP[p].LoadPortStatus = 0;

                for (int i = EQP_Data.LP[p].SlotMapResult.GetLowerBound(0); i <= EQP_Data.LP[p].SlotMapResult.GetUpperBound(0); i++)
                {
                    EQP_Data.LP[p].SlotMapResult[i] = 0;
                }

            }

            EQP_Data.UpdateSVRequest = 0;
            EQP_Data.RequestAlarmReply = 0;


            for (int i = EQP_Data.SlotProcessStatus.GetLowerBound(0); i <= EQP_Data.SlotProcessStatus.GetUpperBound(0); i++)
            {
                EQP_Data.SlotProcessStatus[i] = 0;
            }


            EQP_Data.AlignmentAreaWaferSlotNo = 0;
            EQP_Data.WaferProcessSlotNo = 0;
            EQP_Data.AlignmentAreaProcessSlotNo = 0;
            EQP_Data.MicroAreaProcessSlotNo = 0;
            EQP_Data.MacroAreaProcessSlotNo = 0;
            EQP_Data.AlignResult = 0;
            EQP_Data.MacroAreaWaferSlotNo = 0;
            EQP_Data.SlotSpace = g_SlotSpaceInCassette;

            //TAG_Data
            TAG_Data.TagData = "";
            TAG_Data.ErrorEvent = 0;
            TAG_Data.ShowTagUiRequest = 0;
            TAG_Data.ErrorMessage = "";
            TAG_Data.TagResultEvent = (int)Common.TAG_RESULT.NOT_THING;

        }

        public static void CommonDataInit()
        {
            int i, p;
            SECS_Data.LP = new CARRIER[g_PTN_Space + 1];
            SECS_Data.test = new string[g_SlotSpaceInCassette + 1];
            SECS_Data.ProcessWaferSlot = new int[g_SlotSpaceInCassette + 1];

            for (p = 0; p <= g_PTN_Space; p++)
            {
                SECS_Data.LP[p].HostOpenCarrierCmd = 0;
                SECS_Data.LP[p].HostRejectCarrierCmd = 0;
                SECS_Data.LP[p].HostReleaseCarrierCmd = 0; ;

                SECS_Data.LP[p].CarrierID = "";

            }

            SECS_Data.ProcessWaferNumber = 0;
            SECS_Data.GEM_EqpStatus = 0;

            SECS_Data.HostClampCarrierCmd = 0;
            //SECS_Data.HostOpenCarrierCmd = 0;
            //SECS_Data.HostRejectCarrierCmd = 0;
            //SECS_Data.HostReleaseCarrierCmd = 0; ;
            SECS_Data.HostSelectRecipeCmd = 0;
            SECS_Data.HostProcessCmd = 0;
            SECS_Data.HostRecipeChangeNotify = 0;

            SECS_Data.LotID = new string[g_SlotSpaceInCassette + 1];
            SECS_Data.SubStID = new string[g_SlotSpaceInCassette + 1];

            for (i = 0; i <= Common.g_SlotSpaceInCassette; i++)
            {
                SECS_Data.LotID[i] = "";
                SECS_Data.SubStID[i] = "";
            }

            SECS_Data.UpdateSVRequest = 0;

            SECS_Data.HostRecipeChangePPID = "";

            SECS_Data.EQP_ProcessResult = PROCESS_RESULT.PROCESS_RESULT_DEFAULT;

            SECS_Data.SECS_ConnectionStatus = SECS_CONNECTION_STATE.DISCONNECTION_STOP;
            SECS_Data.GEM_Communicating_State = COMMUNICATING_STATE.DISABLE;
            SECS_Data.GEM_ControlState = CONTROL_STATE.OFFLINE_EQP_OFFLINE;



            EQP_Data.EqpInitState = 0;
            EQP_Data.RecipeChangeEvent = 0;
            EQP_Data.CurrentRecipeChangeEvent = 0;

            EQP_Data.LotStartEvent = 0;
            EQP_Data.LotEndEvent = 0;
            EQP_Data.WaferStartEvent = 0;
            EQP_Data.WaferEndEvent = 0;
            EQP_Data.AlignStartEvent = 0;
            EQP_Data.AlignEndEvent = 0;
            EQP_Data.ProcessStartEvent = 0;
            EQP_Data.ProcessEndEvent = 0;
            EQP_Data.WaferLeaveCompletedEvent = 0;
            EQP_Data.WaferMoveCompletedEvent = 0;
            EQP_Data.SecsConfigRequest = 0;
            EQP_Data.MacroProcessStartEvent = 0;
            EQP_Data.MacroProcessEndEvent = 0;

            //EQP_Data.CarrierAccessingStatus_2_CarrierCompletedEvent = 0;
            //EQP_Data.CarrierAccessingStatus_3_CarrierStoppedEvent = 0;

            EQP_Data.UpdateSVRequest = 0;
            EQP_Data.RequestAlarmReply = 0;

            EQP_Data.SlotProcessStatus = new int[g_SlotSpaceInCassette + 1];
            EQP_Data.AlarmStatus = new int[g_TotalAlarmQTY];

            EQP_Data.LP = new LOAD_PORT[g_PTN_Space + 1];

            EQP_Data.AlignmentAreaWaferSlotNo = 0;
            EQP_Data.WaferProcessSlotNo = 0;
            EQP_Data.AlignmentAreaProcessSlotNo = 0;
            EQP_Data.MicroAreaProcessSlotNo = 0;
            EQP_Data.MacroAreaProcessSlotNo = 0;
            EQP_Data.AlignResult = 0;
            EQP_Data.SlotSpace = g_SlotSpaceInCassette;

            // load port inf.
            for (p = 0; p <= g_PTN_Space; p++)
            {

                EQP_Data.LP[p].SlotMapCompletedEvent = 0;
                EQP_Data.LP[p].SlotMapResult = new int[g_SlotSpaceInCassette + 1];

                EQP_Data.LP[p].LoadPortStatus = 0;
            }

            TAG_Data.TagData = "";
            TAG_Data.ErrorEvent = 0;
            TAG_Data.ShowTagUiRequest = 0;
            TAG_Data.ErrorMessage = "";
            TAG_Data.TagResultEvent = (int)Common.TAG_RESULT.NOT_THING;



            g_iAlarmStatus = new int[Common.g_TotalAlarmQTY];

            for (i = 0; i < (Common.g_TotalAlarmQTY); i++)
            {
                g_iAlarmStatus[i] = 0;
            }

            g_Update_iAlarmStatus = false;
        }

        public enum WinsockServerStatus
        {
            Disconnected = 0,
            Listening = 1,
            Connected = 2
        };

        public static void func_ReadEQPConfig_By_iniFile()
        {
            string strFile = "Config\\EQPConfig.ini";
            string strTemp;
            string[] strTemp2;

            if (File.Exists(strFile))
            {
                TIniFile iniFile = new TIniFile(strFile);

                //g_TotalAlarmQTY
                Common.g_TotalAlarmQTY = 261;

                strTemp = "";

                strTemp = iniFile.ReadString("TotalGEMDataSetting", "TotalAlarmQTY", "");

                strTemp = strTemp.Trim();

                if (strTemp != "")
                {
                    Common.g_TotalAlarmQTY = int.Parse(strTemp);
                }
                else
                {
                    Common.g_TotalAlarmQTY = 261;
                }

                //g_MinAlarmID
                Common.g_MinAlarmID = 100;

                strTemp = "";

                strTemp = iniFile.ReadString("TotalGEMDataSetting", "MinAlarmID", "");

                strTemp = strTemp.Trim();

                if (strTemp != "")
                {
                    Common.g_MinAlarmID = int.Parse(strTemp);
                }
                else
                {
                    Common.g_MinAlarmID = 100;
                }

                //g_MaxAlarmID
                Common.g_MaxAlarmID = 120;

                strTemp = "";

                strTemp = iniFile.ReadString("TotalGEMDataSetting", "MaxAlarmID", "");

                strTemp = strTemp.Trim();

                if (strTemp != "")
                {
                    Common.g_MaxAlarmID = int.Parse(strTemp);
                }
                else
                {
                    Common.g_MaxAlarmID = 120;
                }

                //g_SlotSpaceInCassette
                Common.g_SlotSpaceInCassette = 25;

                strTemp = "";

                strTemp = iniFile.ReadString("CassetteDataSetting", "SlotSpaceInCassette", "");

                strTemp = strTemp.Trim();

                if (strTemp != "")
                {
                    Common.g_SlotSpaceInCassette = int.Parse(strTemp);
                }
                else
                {
                    Common.g_SlotSpaceInCassette = 25;
                }

                //g_RemoteRCMD
                Common.g_RemoteRCMD = "REMOTE";

                strTemp = "";

                strTemp = iniFile.ReadString("RCMDSetting", "RemoteRCMD", "");

                strTemp = strTemp.Trim();

                if (strTemp != "")
                {
                    Common.g_RemoteRCMD = strTemp;
                }
                else
                {
                    Common.g_RemoteRCMD = "REMOTE";
                }

                //g_LocalRCMD
                Common.g_LocalRCMD = "LOCAL";

                strTemp = "";

                strTemp = iniFile.ReadString("RCMDSetting", "LocalRCMD", "");

                strTemp = strTemp.Trim();

                if (strTemp != "")
                {
                    Common.g_LocalRCMD = strTemp;
                }
                else
                {
                    Common.g_LocalRCMD = "Y";
                }

                //g_BarcodeTestMode     // 0: normal mode, 1:Test Mode (no barcode reader)
                Common.g_BarcodeTestMode = 1;

                strTemp = "";

                strTemp = iniFile.ReadString("RFIDSetting", "BARCODE_TestMode", "");

                strTemp = strTemp.Trim();

                if (strTemp != "")
                {
                    Common.g_BarcodeTestMode = int.Parse(strTemp);
                }
                else
                {
                    Common.g_BarcodeTestMode = 1;
                }

                //g_PJ_Space
                Common.g_PJ_Space = 25;

                strTemp = "";

                strTemp = iniFile.ReadString("E40_Setting", "PJ_Space", "");

                strTemp = strTemp.Trim();

                if (strTemp != "")
                {
                    Common.g_PJ_Space = int.Parse(strTemp);
                }
                else
                {
                    Common.g_PJ_Space = 25;
                }

                //g_PTN_Space
                Common.g_PTN_Space = 1;

                strTemp = "";

                strTemp = iniFile.ReadString("E87_Setting", "PTN_Space", "");

                strTemp = strTemp.Trim();

                if (strTemp != "")
                {
                    Common.g_PTN_Space = int.Parse(strTemp);
                }
                else
                {
                    Common.g_PTN_Space = 1;
                }

                //g_Carrier_Space
                Common.g_Carrier_Space = 8;

                strTemp = "";

                strTemp = iniFile.ReadString("E87_Setting", "Carrier_Space", "");

                strTemp = strTemp.Trim();

                if (strTemp != "")
                {
                    Common.g_Carrier_Space = int.Parse(strTemp);
                }
                else
                {
                    Common.g_Carrier_Space = 8;
                }

                //g_ST_Location_Space
                Common.g_ST_Location_Space = 1;

                strTemp = "";

                strTemp = iniFile.ReadString("E90_Setting", "ST_Location_Space", "");

                strTemp = strTemp.Trim();

                if (strTemp != "")
                {
                    Common.g_ST_Location_Space = int.Parse(strTemp);
                }
                else
                {
                    Common.g_ST_Location_Space = 1;
                }

                //g_ST_Step_Space
                Common.g_ST_Step_Space = 1;

                strTemp = "";

                strTemp = iniFile.ReadString("E90_Setting", "ST_Step_Space", "");

                strTemp = strTemp.Trim();

                if (strTemp != "")
                {
                    Common.g_ST_Step_Space = int.Parse(strTemp);
                }
                else
                {
                    Common.g_ST_Step_Space = 1;
                }

                //g_CJ_Space
                Common.g_CJ_Space = Common.g_PTN_Space;

                strTemp = "";

                strTemp = iniFile.ReadString("E94_Setting", "CJ_Space", "");

                strTemp = strTemp.Trim();

                if (strTemp != "")
                {
                    Common.g_CJ_Space = int.Parse(strTemp);
                }
                else
                {
                    Common.g_CJ_Space = Common.g_PTN_Space;
                }

                //g_LPTSM_PortID_x
                Common.g_LPTSM_PortID_1 = 1;
                Common.g_LPTSM_PortID_2 = 2;
                Common.g_LPTSM_PortID_3 = 3;
                Common.g_LPTSM_PortID_4 = 4;
                Common.g_LPTSM_PortID_5 = 5;
                Common.g_LPTSM_PortID_6 = 6;
                Common.g_LPTSM_PortID_7 = 7;
                Common.g_LPTSM_PortID_8 = 8;

                strTemp = "";

                strTemp = iniFile.ReadString("E87_Setting", "LPTSM_PortID_1", "");

                strTemp = strTemp.Trim();

                if (strTemp != "")
                {
                    Common.g_LPTSM_PortID_1 = int.Parse(strTemp);
                }
                else
                {
                    Common.g_LPTSM_PortID_1 = 1;

                }
                Common.g_LPTSM_PortID_2 = 2;
                Common.g_LPTSM_PortID_3 = 3;
                Common.g_LPTSM_PortID_4 = 4;
                Common.g_LPTSM_PortID_5 = 5;
                Common.g_LPTSM_PortID_6 = 6;
                Common.g_LPTSM_PortID_7 = 7;
                Common.g_LPTSM_PortID_8 = 8;

                //**Allow NULL Sutstrate ID setting
                //g_Allow_NULL_SubstID，預設為Allow NULL Sutstrate ID
                Common.g_Allow_NULL_SubstID = "Y";

                strTemp = "";

                strTemp = iniFile.ReadString("E87_Setting", "Allow_NULL_SubstID", "");

                strTemp = strTemp.Trim();

                if (strTemp != "")
                {
                    Common.g_Allow_NULL_SubstID = strTemp;
                }
                else
                {
                    Common.g_Allow_NULL_SubstID = "Y";
                }
                //**Allow NULL Sutstrate ID setting

                //g_EqpInitTimer
                Common.g_EqpInitTimer = 5;

                strTemp = "";

                strTemp = iniFile.ReadString("CommonSetting", "EqpInitTimer", "");

                strTemp = strTemp.Trim();

                if (strTemp != "")
                {
                    Common.g_EqpInitTimer = int.Parse(strTemp);
                }
                else
                {
                    Common.g_EqpInitTimer = 5;
                }

                //g_InitShowForm
                Common.g_InitShowForm = 0;

                strTemp = "";

                strTemp = iniFile.ReadString("CommonSetting", "InitShowForm", "");

                strTemp = strTemp.Trim();

                if (strTemp != "")
                {
                    Common.g_InitShowForm = int.Parse(strTemp);
                }
                else
                {
                    Common.g_InitShowForm = 0;
                }

                //g_LocationID
                Common.g_LocationID = new string[Common.g_ST_Location_Space + 1];

                strTemp = "";

                strTemp = iniFile.ReadString("E90_Setting", "LocationIDs", "");

                strTemp = strTemp.Trim();

                if (strTemp != "")
                {
                    strTemp2 = strTemp.Split(new Char[] { ',', '\t' });

                    for (int i = strTemp2.GetLowerBound(0); i <= strTemp2.GetUpperBound(0); i++)
                    {
                        Common.g_LocationID[i + 1] = strTemp2[i].Trim();
                    }
                }

                iniFile.Close();
                iniFile.Dispose();
            }
        }
    }
}
