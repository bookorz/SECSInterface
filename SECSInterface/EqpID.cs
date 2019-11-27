static class EqpID
{
    // Event ID 
    public const int Stocker_State_Change = 5001;
    public const int WTS_State_Change = 5002;
    public const int System_Mode_Change = 5004;
    public const int Stocker_Source_Change = 5006;
    public const int Stocker_Destination_Change = 5008;
    public const int ELPT1_Mode_Change = 5100;
    public const int ELPT1_FOUP_State_Change = 5102;
    public const int ELPT1_LOCK_State_Change = 5104;
    public const int ELPT2_Mode_Change = 5110;
    public const int ELPT2_FOUP_State_Change = 5112;
    public const int ELPT2_LOCK_State_Change = 5114;
    public const int ILPT1_Mode_Change = 5200;
    public const int ILPT1_FOUP_State_Change = 5202;
    public const int ILPT1_LOCK_State_Change = 5204;
    public const int ILPT2_Mode_Change = 5210;
    public const int ILPT2_FOUP_State_Change = 5212;
    public const int ILPT2_LOCK_State_Change = 5214;
    public const int Gripper_FOUP_State_Change = 5250;
    public const int Shelf_1_State_Change = 5300;
    public const int Shelf_2_State_Change = 5302;
    public const int Shelf_3_State_Change = 5304;
    public const int Shelf_4_State_Change = 5306;
    public const int Shelf_5_State_Change = 5308;
    public const int Shelf_6_State_Change = 5310;
    public const int Shelf_7_State_Change = 5312;
    public const int Shelf_8_State_Change = 5314;
    public const int Shelf_9_State_Change = 5316;
    public const int Shelf_10_State_Change = 5318;
    public const int Shelf_11_State_Change = 5320;
    public const int Shelf_12_State_Change = 5322;
    public const int Shelf_13_State_Change = 5324;
    public const int Shelf_14_State_Change = 5326;
    public const int Shelf_15_State_Change = 5328;
    public const int Shelf_16_State_Change = 5330;
    public const int Shelf_17_State_Change = 5332;
    public const int Shelf_18_State_Change = 5334;
    public const int PTZ_State_Change = 5340;
    public const int Process_Carrier_WTS_State_Change = 5344;
    public const int ELPT1_ID_Available = 5520;
    public const int ELPT2_ID_Available = 5521;
    public const int ILPT1_ID_Available = 5522;
    public const int ILPT2_ID_Available = 5523;
    public const int ILPT1_Wafer_Map_Available_ = 5530;
    public const int ILPT2_Wafer_Map_Available_ = 5531;
    public const int PTZ_Map_Available = 5532;
    public const int ALIGNER_State_Change = 5551;
    public const int Process_Substrate_State_Change_ = 5552;
    public const int Light_Curtain_State_Change = 5600;
    public const int ESTOP_State_Change = 5602;

    // SV ID 
    public const int SV_EQP_STATUS = 101;        //UINT_1    EquipmentStatus        IDLE = 0,  RUN = 1,   STOP = 2,   PAUSE = 3,   ASSIST = 4,   INITIAL = 5
    public const int TABLE_VACUUM_STATUS = 102;  //UINT_1    TableVacuumStatus (送至Micro /Printer 區域的真空狀態)   0:off  1:on
    public const int GRAP_VACUUM_STATUS = 103;   //UINT_1    GrapVacuumStatus (Robot上面夾爪的真空狀態)   0:off  1:on
    public const int LP1_STATUS = 104;           //BINARY    LP1Status    0: Not Present, 1: Present
    public const int LP1_CLAMP_STATUS = 105;
    public const int LP1_DOCK_STATUS = 106;

    public const int ALIGNMENT_AREA_SLOTID = 111;        //UINT_1    AlignmentAreaSlotID

    public const int MACRO_AREA_SLOTID = 112;           //UINT_1    MacroArea SlotID

    public const int MICRO_AREA_SLOTID = 113;           //UINT_1    MicroArea/PrinterArea SlotID
    public const int ALIGNMENT_VACUUM_STATUS = 114;     //UINT_1    AlignmentVacuumStatus (Alignment Area的真空狀態)   0:off  1:on

    public const int E87_SV_LP1_ACCESS_MODE = 5001;                                                                                   //UINT_1E87SVLP1AccessMode
    public const int E87_SV_CARRIER_ID_1 = 5011;        //ASCII     E87SVCarrierID1
    public const int E87_SV_LP1_RESERVATION_STATE = 5031;   //UINT_1 E87 SV LP1 LoadPortReservationState                                                                             //UINT_1E87SVLP1PortAssociationState
    public const int E87_SV_LP1_ASSOCIATION_STATE = 5051;                                                                             //UINT_1E87SVLP1PortAssociationState
    public const int E87_SV_LP1_PORT_ID = 5071;         //UINT_1    E87SVLP1PortID
    public const int E87_SV_LP1_PORT_STATE_INFO = 5081;                                                                               //LISTE87SVLP1PortStateInfo
    public const int E87_SV_PORT_STATE_INFO_LIST = 5090;                                                                              //LISTE87SVPortStateInfoList
    public const int E87_SV_LP1_TRANSFER_STATE = 5091;                                                                                //UINT_1E87SVLP1PortTransferState
    public const int E87_SV_PORT_TRANSFER_STATE_LIST = 5100;                                                                          //LISTE87SVPortTransferStateList
    public const int E87_SV_CAPACITY_1 = 5101;   //UINT_1    E87SVCapacity1
    public const int E87_SV_CARRIER_ID_STATUS_1 = 5111;                                                                               //UINT_1E87SVCarrierIDStatus1
    public const int E87_SV_CARRIER_ACCESSING_STATUS_1 = 5121;                                                                        //UINT_1E87SVCarrierAccessingStatus1
    public const int E87_SV_CONTENT_MAP_1 = 5131;                                                                                     //LISTE87SVContentMap1
    public const int E87_SV_LOCATION_ID_1 = 5141;                                                                                     //ASCIIE87SVLocationID1
    public const int E87_SV_SLOT_MAP_1 = 5151;   //LIST      E87SVSlotMap1
    public const int E87_SV_SLOT_MAP_STATUS_1 = 5161;                                                                                 //UINT_1E87SVSlotMapStatus1
    public const int E87_SV_SUBSTRATE_COUNT_1 = 5171;                                                                                 //UINT_1E87SVSubstrateCount1
    public const int E87_SV_USAGE_1 = 5181;      //ASCII     E87SVUsage1

    //*************************************************************************************************************************************
    //2015/07/26
    public const int E40_SV_MAX_PROCESS_JOB_SPACE = 5201;                                                                             //UINT_1 E40_SV_MAX_PROCESS_JOB_SPACE
    public const int E40_SV_AVAILABLE_PROCESS_JOB_SPACE = 5202;                                                                       //UINT_1 E40_SV_AVAILABLE_PROCESS_JOB_SPACE
                                                                                                                                      //*************************************************************************************************************************************

    public const int E90_LOCATION_ID_STAGE1 = 5501;                                                                                //ASCII 
    public const int E90_LOCATION_ID_STAGE2 = 5502;                                                                            //ASCII 
    public const int E90_LOCATION_ID_STAGE3 = 5503;                                                                              //ASCII 

    public const int E90_LOCATION_STATE_STAGE1 = 5504;                                                                             //UINT_1 E90LocationState-STAGE1
    public const int E90_LOCATION_STATE_STAGE2 = 5505;                                                                         //UINT_1 E90LocationState-Ma Tester
    public const int E90_LOCATION_STATE_STAGE3 = 5506;                                                                           //UINT_1 E90LocationState-Me Tester

    public const int E90_SUBSTR_LOC_SUBSTR_ID_STAGE1 = 5507;                                                                       //ASCII E90substrLocSubstrID-STAGE1
    public const int E90_SUBSTR_LOC_SUBSTR_ID_STAGE2 = 5508;                                                                   //ASCII E90substrLocSubstrID-Ma Tester
    public const int E90_SUBSTR_LOC_SUBSTR_ID_STAGE3 = 5509;                                                                     //ASCII E90substrLocSubstrID-Me Tester

    public const int E94_SV_QUEUE_AVAILABLE_SPACE = 6001;                                                                             //UINT_1E94SVQueueAvailableSpace
    public const int E94_SV_QUEUE_CONTROL_JOB_ID_1 = 6002;                                                                            //ASCIIE94SVCtrlJobID1(notstander)
    public const int E94_SV_QUEUE_CONTROL_JOBS = 6010;                                                                                //LISTE94SVQueuedCJobs

    // DV ID
    //public const int TEST_REPORT_FILE_PATH = 2001;                                                                                    //ASCII Testreportfilepath
    public const int WAFER_PROCESS_SLOT_ID = 2002;                                                                                      //UINT_1 Wafer Process Slot ID

    public const int MICRO_PROCESS_SLOT_ID = 2005;                                                                            //UINT_1 MICRO_PROCESS_SLOT_ID

    public const int MACRO_PROCESS_SLOT_ID = 2006;                                                                            //UINT_1 MACRO_PROCESS_SLOT_ID

    public const int ALIGNMENT_RESULT = 2007;                                                                            //UINT_1 ALIGNMENT_RESULT    0:OK   1:NG

    public const int E87_IDATA_CTMAP_1_SUB_LIST1 = 2201;                                                                              //LIST*E87ContentMap1sublist1
    public const int E87_IDATA_CTMAP_1_SUB_LIST2 = 2202;                                                                              //LIST*E87ContentMap1sublist2
    public const int E87_IDATA_CTMAP_1_SUB_LIST3 = 2203;                                                                              //LIST*E87ContentMap1sublist3
    public const int E87_IDATA_CTMAP_1_SUB_LIST4 = 2204;                                                                              //LIST*E87ContentMap1sublist4
    public const int E87_IDATA_CTMAP_1_SUB_LIST5 = 2205;                                                                              //LIST*E87ContentMap1sublist5
    public const int E87_IDATA_CTMAP_1_SUB_LIST6 = 2206;                                                                              //LIST*E87ContentMap1sublist6
    public const int E87_IDATA_CTMAP_1_SUB_LIST7 = 2207;                                                                              //LIST*E87ContentMap1sublist7
    public const int E87_IDATA_CTMAP_1_SUB_LIST8 = 2208;                                                                              //LIST*E87ContentMap1sublist8
    public const int E87_IDATA_CTMAP_1_SUB_LIST9 = 2209;                                                                              //LIST*E87ContentMap1sublist9
    public const int E87_IDATA_CTMAP_1_SUB_LIST10 = 2210;                                                                             //LIST*E87ContentMap1sublist10
    public const int E87_IDATA_CTMAP_1_SUB_LIST11 = 2211;                                                                             //LIST*E87ContentMap1sublist11
    public const int E87_IDATA_CTMAP_1_SUB_LIST12 = 2212;                                                                             //LIST*E87ContentMap1sublist12
    public const int E87_IDATA_CTMAP_1_SUB_LIST13 = 2213;                                                                             //LIST*E87ContentMap1sublist13
    public const int E87_IDATA_CTMAP_1_SUB_LIST14 = 2214;                                                                             //LIST*E87ContentMap1sublist14
    public const int E87_IDATA_CTMAP_1_SUB_LIST15 = 2215;                                                                             //LIST*E87ContentMap1sublist15
    public const int E87_IDATA_CTMAP_1_SUB_LIST16 = 2216;                                                                             //LIST*E87ContentMap1sublist16
    public const int E87_IDATA_CTMAP_1_SUB_LIST17 = 2217;                                                                             //LIST*E87ContentMap1sublist17
    public const int E87_IDATA_CTMAP_1_SUB_LIST18 = 2218;                                                                             //LIST*E87ContentMap1sublist18
    public const int E87_IDATA_CTMAP_1_SUB_LIST19 = 2219;                                                                             //LIST*E87ContentMap1sublist19
    public const int E87_IDATA_CTMAP_1_SUB_LIST20 = 2220;                                                                             //LIST*E87ContentMap1sublist20
    public const int E87_IDATA_CTMAP_1_SUB_LIST21 = 2221;                                                                             //LIST*E87ContentMap1sublist21
    public const int E87_IDATA_CTMAP_1_SUB_LIST22 = 2222;                                                                             //LIST*E87ContentMap1sublist22
    public const int E87_IDATA_CTMAP_1_SUB_LIST23 = 2223;                                                                             //LIST*E87ContentMap1sublist23
    public const int E87_IDATA_CTMAP_1_SUB_LIST24 = 2224;                                                                             //LIST*E87ContentMap1sublist24
    public const int E87_IDATA_CTMAP_1_SUB_LIST25 = 2225;                                                                             //LIST*E87ContentMap1sublist25
    public const int E87_IDATA_CTMAP_1_LOTID1 = 2301;                                                                                 //ASCII*E87ContentMap1LotID1
    public const int E87_IDATA_CTMAP_1_LOTID2 = 2302;                                                                                 //ASCII*E87ContentMap1LotID2
    public const int E87_IDATA_CTMAP_1_LOTID3 = 2303;                                                                                 //ASCII*E87ContentMap1LotID3
    public const int E87_IDATA_CTMAP_1_LOTID4 = 2304;                                                                                 //ASCII*E87ContentMap1LotID4
    public const int E87_IDATA_CTMAP_1_LOTID5 = 2305;                                                                                 //ASCII*E87ContentMap1LotID5
    public const int E87_IDATA_CTMAP_1_LOTID6 = 2306;                                                                                 //ASCII*E87ContentMap1LotID6
    public const int E87_IDATA_CTMAP_1_LOTID7 = 2307;                                                                                 //ASCII*E87ContentMap1LotID7
    public const int E87_IDATA_CTMAP_1_LOTID8 = 2308;                                                                                 //ASCII*E87ContentMap1LotID8
    public const int E87_IDATA_CTMAP_1_LOTID9 = 2309;                                                                                 //ASCII*E87ContentMap1LotID9
    public const int E87_IDATA_CTMAP_1_LOTID10 = 2310;                                                                                //ASCII*E87ContentMap1LotID10
    public const int E87_IDATA_CTMAP_1_LOTID11 = 2311;                                                                                //ASCII*E87ContentMap1LotID11
    public const int E87_IDATA_CTMAP_1_LOTID12 = 2312;                                                                                //ASCII*E87ContentMap1LotID12
    public const int E87_IDATA_CTMAP_1_LOTID13 = 2313;                                                                                //ASCII*E87ContentMap1LotID13
    public const int E87_IDATA_CTMAP_1_LOTID14 = 2314;                                                                                //ASCII*E87ContentMap1LotID14
    public const int E87_IDATA_CTMAP_1_LOTID15 = 2315;                                                                                //ASCII*E87ContentMap1LotID15
    public const int E87_IDATA_CTMAP_1_LOTID16 = 2316;                                                                                //ASCII*E87ContentMap1LotID16
    public const int E87_IDATA_CTMAP_1_LOTID17 = 2317;                                                                                //ASCII*E87ContentMap1LotID17
    public const int E87_IDATA_CTMAP_1_LOTID18 = 2318;                                                                                //ASCII*E87ContentMap1LotID18
    public const int E87_IDATA_CTMAP_1_LOTID19 = 2319;                                                                                //ASCII*E87ContentMap1LotID19
    public const int E87_IDATA_CTMAP_1_LOTID20 = 2320;                                                                                //ASCII*E87ContentMap1LotID20
    public const int E87_IDATA_CTMAP_1_LOTID21 = 2321;                                                                                //ASCII*E87ContentMap1LotID21
    public const int E87_IDATA_CTMAP_1_LOTID22 = 2322;                                                                                //ASCII*E87ContentMap1LotID22
    public const int E87_IDATA_CTMAP_1_LOTID23 = 2323;                                                                                //ASCII*E87ContentMap1LotID23
    public const int E87_IDATA_CTMAP_1_LOTID24 = 2324;                                                                                //ASCII*E87ContentMap1LotID24
    public const int E87_IDATA_CTMAP_1_LOTID25 = 2325;                                                                                //ASCII*E87ContentMap1LotID25
    public const int E87_IDATA_CTMAP_1_SUBSTID1 = 2401;                                                                               //ASCII*E87ContentMap1SubstID1
    public const int E87_IDATA_CTMAP_1_SUBSTID2 = 2402;                                                                               //ASCII*E87ContentMap1SubstID2
    public const int E87_IDATA_CTMAP_1_SUBSTID3 = 2403;                                                                               //ASCII*E87ContentMap1SubstID3
    public const int E87_IDATA_CTMAP_1_SUBSTID4 = 2404;                                                                               //ASCII*E87ContentMap1SubstID4
    public const int E87_IDATA_CTMAP_1_SUBSTID5 = 2405;                                                                               //ASCII*E87ContentMap1SubstID5
    public const int E87_IDATA_CTMAP_1_SUBSTID6 = 2406;                                                                               //ASCII*E87ContentMap1SubstID6
    public const int E87_IDATA_CTMAP_1_SUBSTID7 = 2407;                                                                               //ASCII*E87ContentMap1SubstID7
    public const int E87_IDATA_CTMAP_1_SUBSTID8 = 2408;                                                                               //ASCII*E87ContentMap1SubstID8
    public const int E87_IDATA_CTMAP_1_SUBSTID9 = 2409;                                                                               //ASCII*E87ContentMap1SubstID9
    public const int E87_IDATA_CTMAP_1_SUBSTID10 = 2410;                                                                              //ASCII*E87ContentMap1SubstID10
    public const int E87_IDATA_CTMAP_1_SUBSTID11 = 2411;                                                                              //ASCII*E87ContentMap1SubstID11
    public const int E87_IDATA_CTMAP_1_SUBSTID12 = 2412;                                                                              //ASCII*E87ContentMap1SubstID12
    public const int E87_IDATA_CTMAP_1_SUBSTID13 = 2413;                                                                              //ASCII*E87ContentMap1SubstID13
    public const int E87_IDATA_CTMAP_1_SUBSTID14 = 2414;                                                                              //ASCII*E87ContentMap1SubstID14
    public const int E87_IDATA_CTMAP_1_SUBSTID15 = 2415;                                                                              //ASCII*E87ContentMap1SubstID15
    public const int E87_IDATA_CTMAP_1_SUBSTID16 = 2416;                                                                              //ASCII*E87ContentMap1SubstID16
    public const int E87_IDATA_CTMAP_1_SUBSTID17 = 2417;                                                                              //ASCII*E87ContentMap1SubstID17
    public const int E87_IDATA_CTMAP_1_SUBSTID18 = 2418;                                                                              //ASCII*E87ContentMap1SubstID18
    public const int E87_IDATA_CTMAP_1_SUBSTID19 = 2419;                                                                              //ASCII*E87ContentMap1SubstID19
    public const int E87_IDATA_CTMAP_1_SUBSTID20 = 2420;                                                                              //ASCII*E87ContentMap1SubstID20
    public const int E87_IDATA_CTMAP_1_SUBSTID21 = 2421;                                                                              //ASCII*E87ContentMap1SubstID21
    public const int E87_IDATA_CTMAP_1_SUBSTID22 = 2422;                                                                              //ASCII*E87ContentMap1SubstID22
    public const int E87_IDATA_CTMAP_1_SUBSTID23 = 2423;                                                                              //ASCII*E87ContentMap1SubstID23
    public const int E87_IDATA_CTMAP_1_SUBSTID24 = 2424;                                                                              //ASCII*E87ContentMap1SubstID24
    public const int E87_IDATA_CTMAP_1_SUBSTID25 = 2425;                                                                              //ASCII*E87ContentMap1SubstID25
    public const int E87_IDATA_SLOT_MAP_1_1 = 2501;                                                                                   //UINT_1*E87iDtataSlotMap1-1
    public const int E87_IDATA_SLOT_MAP_1_2 = 2502;                                                                                   //UINT_1*E87iDtataSlotMap1-2
    public const int E87_IDATA_SLOT_MAP_1_3 = 2503;                                                                                   //UINT_1*E87iDtataSlotMap1-3
    public const int E87_IDATA_SLOT_MAP_1_4 = 2504;                                                                                   //UINT_1*E87iDtataSlotMap1-4
    public const int E87_IDATA_SLOT_MAP_1_5 = 2505;                                                                                   //UINT_1*E87iDtataSlotMap1-5
    public const int E87_IDATA_SLOT_MAP_1_6 = 2506;                                                                                   //UINT_1*E87iDtataSlotMap1-6
    public const int E87_IDATA_SLOT_MAP_1_7 = 2507;                                                                                   //UINT_1*E87iDtataSlotMap1-7
    public const int E87_IDATA_SLOT_MAP_1_8 = 2508;                                                                                   //UINT_1*E87iDtataSlotMap1-8
    public const int E87_IDATA_SLOT_MAP_1_9 = 2509;                                                                                   //UINT_1*E87iDtataSlotMap1-9
    public const int E87_IDATA_SLOT_MAP_1_10 = 2510;                                                                                  //UINT_1*E87iDtataSlotMap1-10
    public const int E87_IDATA_SLOT_MAP_1_11 = 2511;                                                                                  //UINT_1*E87iDtataSlotMap1-11
    public const int E87_IDATA_SLOT_MAP_1_12 = 2512;                                                                                  //UINT_1*E87iDtataSlotMap1-12
    public const int E87_IDATA_SLOT_MAP_1_13 = 2513;                                                                                  //UINT_1*E87iDtataSlotMap1-13
    public const int E87_IDATA_SLOT_MAP_1_14 = 2514;                                                                                  //UINT_1*E87iDtataSlotMap1-14
    public const int E87_IDATA_SLOT_MAP_1_15 = 2515;                                                                                  //UINT_1*E87iDtataSlotMap1-15
    public const int E87_IDATA_SLOT_MAP_1_16 = 2516;                                                                                  //UINT_1*E87iDtataSlotMap1-16
    public const int E87_IDATA_SLOT_MAP_1_17 = 2517;                                                                                  //UINT_1*E87iDtataSlotMap1-17
    public const int E87_IDATA_SLOT_MAP_1_18 = 2518;                                                                                  //UINT_1*E87iDtataSlotMap1-18
    public const int E87_IDATA_SLOT_MAP_1_19 = 2519;                                                                                  //UINT_1*E87iDtataSlotMap1-19
    public const int E87_IDATA_SLOT_MAP_1_20 = 2520;                                                                                  //UINT_1*E87iDtataSlotMap1-20
    public const int E87_IDATA_SLOT_MAP_1_21 = 2521;                                                                                  //UINT_1*E87iDtataSlotMap1-21
    public const int E87_IDATA_SLOT_MAP_1_22 = 2522;                                                                                  //UINT_1*E87iDtataSlotMap1-22
    public const int E87_IDATA_SLOT_MAP_1_23 = 2523;                                                                                  //UINT_1*E87iDtataSlotMap1-23
    public const int E87_IDATA_SLOT_MAP_1_24 = 2524;                                                                                  //UINT_1*E87iDtataSlotMap1-24
    public const int E87_IDATA_SLOT_MAP_1_25 = 2525;                                                                                  //UINT_1*E87iDtataSlotMap1-25
    public const int E87_DV_ACCESS_MODE = 3101;  //UINT_1    E87DVAccessMode
    public const int E87_DV_CARRIER_ACCESSING_STATUS = 3104;                                                                          //UINT_1E87DVCarrierAccessingStatus
    public const int E87_DV_CARRIER_ID = 3105;   //ASCII     E87DVCarrierID
    public const int E87_DV_CARRIER_ID_STATUS = 3106;                                                                                 //UINT_1E87DVCarrierIDStatus
    public const int E87_DV_LOAD_PORT_RESERVATION_STATUS = 3107;                                                                      //UINT_1 E87 DV LoadPortReservationState
    public const int E87_DV_LOCATION_ID = 3108;  //ASCII     E87DVLocationID
    public const int E87_DV_PORT_ASSCOCIATION_STATE = 3111;                                                                           //UINT_1E87DVPortAssociationState
    public const int E87_DV_PORT_ID = 3112;      //UINT_1    E87DVPortID
    public const int E87_DV_PORT_STATE_INFO = 3113;                                                                                   //LISTE87DVPortStateInfo
    public const int E87_DV_PORT_TARANSFER_STATE = 3114;                                                                              //UINT_1E87DVPortTransferState
    public const int E87_DV_REASON = 3115;       //UINT_1    E87DVReason
    public const int E87_DV_SLOT_MAP_STATUS = 3116;                                                                                   //UINT_1E87DVSlotMapStatus
    public const int E87_DV_SLOT_MAP = 3200;     //LIST      E87DVSlotMap
    public const int E87_DV_SLOT_MAP_1 = 3201;   //UINT_1    *E87SlotMap1
    public const int E87_DV_SLOT_MAP_2 = 3202;   //UINT_1    *E87SlotMap2
    public const int E87_DV_SLOT_MAP_3 = 3203;   //UINT_1    *E87SlotMap3
    public const int E87_DV_SLOT_MAP_4 = 3204;   //UINT_1    *E87SlotMap4
    public const int E87_DV_SLOT_MAP_5 = 3205;   //UINT_1    *E87SlotMap5
    public const int E87_DV_SLOT_MAP_6 = 3206;   //UINT_1    *E87SlotMap6
    public const int E87_DV_SLOT_MAP_7 = 3207;   //UINT_1    *E87SlotMap7
    public const int E87_DV_SLOT_MAP_8 = 3208;   //UINT_1    *E87SlotMap8
    public const int E87_DV_SLOT_MAP_9 = 3209;   //UINT_1    *E87SlotMap9
    public const int E87_DV_SLOT_MAP_10 = 3210;  //UINT_1    *E87SlotMap10
    public const int E87_DV_SLOT_MAP_11 = 3211;  //UINT_1    *E87SlotMap11
    public const int E87_DV_SLOT_MAP_12 = 3212;  //UINT_1    *E87SlotMap12
    public const int E87_DV_SLOT_MAP_13 = 3213;  //UINT_1    *E87SlotMap13
    public const int E87_DV_SLOT_MAP_14 = 3214;  //UINT_1    *E87SlotMap14
    public const int E87_DV_SLOT_MAP_15 = 3215;  //UINT_1    *E87SlotMap15
    public const int E87_DV_SLOT_MAP_16 = 3216;  //UINT_1    *E87SlotMap16
    public const int E87_DV_SLOT_MAP_17 = 3217;  //UINT_1    *E87SlotMap17
    public const int E87_DV_SLOT_MAP_18 = 3218;  //UINT_1    *E87SlotMap18
    public const int E87_DV_SLOT_MAP_19 = 3219;  //UINT_1    *E87SlotMap19
    public const int E87_DV_SLOT_MAP_20 = 3220;  //UINT_1    *E87SlotMap20
    public const int E87_DV_SLOT_MAP_21 = 3221;  //UINT_1    *E87SlotMap21
    public const int E87_DV_SLOT_MAP_22 = 3222;  //UINT_1    *E87SlotMap22
    public const int E87_DV_SLOT_MAP_23 = 3223;  //UINT_1    *E87SlotMap23
    public const int E87_DV_SLOT_MAP_24 = 3224;  //UINT_1    *E87SlotMap24
    public const int E87_DV_SLOT_MAP_25 = 3225;  //UINT_1    *E87SlotMap25
    public const int E40_DV_PR_JOB_ID = 3301;    //ASCII     E40PRJobID
    public const int E40_DV_PR_JOB_STATE = 3302; //INT_1    E40PRJobState
    public const int E40_DV_RECIPE_ID = 3305;    //ASCII     E40RecID
    public const int E40_DV_PR_PROCESS_START = 3307;                                                                                  //BOOLEANE40PRProcessStart
    public const int E40_DV_PR_MTL_NAME_LIST = 3310;                                                                                  //LISTE40PRMtlNameList
    public const int E40_DV_PR_MTL_NAME_1 = 3311;                                                                                     //LIST*E40PRMtlName1(forPRMtlNameList)
    public const int E40_DV_PR_MTL_NAME_1_CARRIER_ID = 3315;                                                                          //ASCII*E40CarrierID1(forPRMtlName1)
    public const int E40_DV_PR_MTL_NAME_1_SLOTID_LIST = 3331;                                                                         //LIST*E40SlopIDList1(forPRMtlName1)
    public const int E40_DV_PR_MTL_NAME_1_SLOTID_1 = 3401;                                                                            //UINT_1*E40SlopID1(forSlopIDList1)
    public const int E40_DV_PR_MTL_NAME_1_SLOTID_2 = 3402;                                                                            //UINT_1*E40SlopID2(forSlopIDList1)
    public const int E40_DV_PR_MTL_NAME_1_SLOTID_3 = 3403;                                                                            //UINT_1*E40SlopID3(forSlopIDList1)
    public const int E40_DV_PR_MTL_NAME_1_SLOTID_4 = 3404;                                                                            //UINT_1*E40SlopID4(forSlopIDList1)
    public const int E40_DV_PR_MTL_NAME_1_SLOTID_5 = 3405;                                                                            //UINT_1*E40SlopID5(forSlopIDList1)
    public const int E40_DV_PR_MTL_NAME_1_SLOTID_6 = 3406;                                                                            //UINT_1*E40SlopID6(forSlopIDList1)
    public const int E40_DV_PR_MTL_NAME_1_SLOTID_7 = 3407;                                                                            //UINT_1*E40SlopID7(forSlopIDList1)
    public const int E40_DV_PR_MTL_NAME_1_SLOTID_8 = 3408;                                                                            //UINT_1*E40SlopID8(forSlopIDList1)
    public const int E40_DV_PR_MTL_NAME_1_SLOTID_9 = 3409;                                                                            //UINT_1*E40SlopID9(forSlopIDList1)
    public const int E40_DV_PR_MTL_NAME_1_SLOTID_10 = 3410;                                                                           //UINT_1*E40SlopID10(forSlopIDList1)
    public const int E40_DV_PR_MTL_NAME_1_SLOTID_11 = 3411;                                                                           //UINT_1*E40SlopID11(forSlopIDList1)
    public const int E40_DV_PR_MTL_NAME_1_SLOTID_12 = 3412;                                                                           //UINT_1*E40SlopID12(forSlopIDList1)
    public const int E40_DV_PR_MTL_NAME_1_SLOTID_13 = 3413;                                                                           //UINT_1*E40SlopID13(forSlopIDList1)
    public const int E40_DV_PR_MTL_NAME_1_SLOTID_14 = 3414;                                                                           //UINT_1*E40SlopID14(forSlopIDList1)
    public const int E40_DV_PR_MTL_NAME_1_SLOTID_15 = 3415;                                                                           //UINT_1*E40SlopID15(forSlopIDList1)
    public const int E40_DV_PR_MTL_NAME_1_SLOTID_16 = 3416;                                                                           //UINT_1*E40SlopID16(forSlopIDList1)
    public const int E40_DV_PR_MTL_NAME_1_SLOTID_17 = 3417;                                                                           //UINT_1*E40SlopID17(forSlopIDList1)
    public const int E40_DV_PR_MTL_NAME_1_SLOTID_18 = 3418;                                                                           //UINT_1*E40SlopID18(forSlopIDList1)
    public const int E40_DV_PR_MTL_NAME_1_SLOTID_19 = 3419;                                                                           //UINT_1*E40SlopID19(forSlopIDList1)
    public const int E40_DV_PR_MTL_NAME_1_SLOTID_20 = 3420;                                                                           //UINT_1*E40SlopID20(forSlopIDList1)
    public const int E40_DV_PR_MTL_NAME_1_SLOTID_21 = 3421;                                                                           //UINT_1*E40SlopID21(forSlopIDList1)
    public const int E40_DV_PR_MTL_NAME_1_SLOTID_22 = 3422;                                                                           //UINT_1*E40SlopID22(forSlopIDList1)
    public const int E40_DV_PR_MTL_NAME_1_SLOTID_23 = 3423;                                                                           //UINT_1*E40SlopID23(forSlopIDList1)
    public const int E40_DV_PR_MTL_NAME_1_SLOTID_24 = 3424;                                                                           //UINT_1*E40SlopID24(forSlopIDList1)
    public const int E40_DV_PR_MTL_NAME_1_SLOTID_25 = 3425;                                                                           //UINT_1*E40SlopID251(forSlopIDList1)
    public const int E90_LOTID = 3451;           //ASCII     E90LotID
    public const int E90_ST_ID = 3452;           //ASCII     E90SubstrateID
    public const int E90_ST_LOCATION_ID = 3453;  //ASCII     E90SubstrateLocationID
    public const int E90_ST_STATE = 3454;        //UINT_1    E90substLocState
    public const int E90_ST_LOCATION_ST_ID = 3455;                                                                                     //ASCIIE90substrLocSubstrID
    public const int E90_ST_PROCESS_STATUS = 3456;                                                                                    //UINT_1E90SubstrateProcessingStatus
    public const int E90_ST_SOURCE = 3457;       //ASCII     E90SubstrateSource
    public const int E90_ST_DESTINATION = 3458;  //ASCII     E90SubstrateDestination
    public const int E90_ST_TRANSPORT_STATE = 3459;                                                                                   //UINT_1E90SubstrateTransportState
    public const int E90_ST_HISTORY = 3460;      //LIST      E90SubstrateHistory
    public const int E90_ST_HISTORY_SUBLIST_1 = 3461;                                                                                 //LIST*E90SubstrateHistorySublist1
    public const int E90_ST_HISTORY_SUBLIST_2 = 3462;                                                                                 //LIST*E90SubstrateHistorySublist2
    public const int E90_ST_HISTORY_SUBLIST_3 = 3463;                                                                                 //LIST*E90SubstrateHistorySublist3

    public const int E90_ST_HISTORY_SUBLOCID_1 = 3464;                                                                                //ASCII*E90SubstrateHistorySubLocID1
    public const int E90_ST_HISTORY_SUBLOCID_2 = 3465;                                                                                //ASCII*E90SubstrateHistorySubLocID2
    public const int E90_ST_HISTORY_SUBLOCID_3 = 3466;                                                                                //ASCII*E90SubstrateHistorySubLocID3

    public const int E90_ST_HISTORY_TIME_IN_1 = 3467;                                                                                 //ASCII*E90SubstrateHistoryTimeIn1
    public const int E90_ST_HISTORY_TIME_IN_2 = 3468;                                                                                 //ASCII*E90SubstrateHistoryTimeIn2
    public const int E90_ST_HISTORY_TIME_IN_3 = 3469;                                                                                 //ASCII*E90SubstrateHistoryTimeIn3

    public const int E90_ST_HISTORY_TIME_OUT_1 = 3470;                                                                                //ASCII*E90SubstrateHistoryTimeOut1
    public const int E90_ST_HISTORY_TIME_OUT_2 = 3471;                                                                                //ASCII*E90SubstrateHistoryTimeOut2
    public const int E90_ST_HISTORY_TIME_OUT_3 = 3472;                                                                                //ASCII*E90SubstrateHistoryTimeOut3

    public const int E94_DV_CONTROL_JOB_ID = 3501;                                                                                    //ASCIIE94CtrlJobID
    public const int E94_DV_CARRIER_INPUT_SPEC = 3502;                                                                                //LISTE94CarrierInputSpec
    public const int E94_DV_START_METHOD = 3508; //BOOLEAN   E94StartMethod
    public const int E94_DV_CTRL_JOB_STATE = 3510;                                                                                    //UINT_1E94CtrlJobState
    public const int E94_DV_CURRENT_PR_JOB = 3511;                                                                                    //LISTE94CurrentPRJob
    public const int E94_DV_CURRENT_PR_JOB_ID_1 = 3512;                                                                               //ASCIIE94PRJobID1(forCurrentPRJob)

    // EConst ID 
    //public const int EC_CARRIER_CLAMP_MODE = 1001;                                                                                    //UINT_1CarrierClampMode(0:ManualClamp1:AutoClamp)
    public const int EC_CARRIER_RELEASE_MODE = 1002;                                                                                  //UINT_1CarrierReleaseMode(0:Waithostcommand1:Autorelease)

    // Alarm ID 
    public const int ALARM_PUMP_PRESS = 9001;    //UINT_4    Pumppressureerror
    public const int ALARM_EM = 9002;            //UINT_4    EmergencyStoppressed
    public const int ALARM_HEATER_FAIL = 9003;   //UINT_4    Heaterfailed
    public const int ALARM_HEATER_FAIL2 = 1000001;                                                                                    //UINT_4Heater2failed

}