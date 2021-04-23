using BoqiangH5.ISO15765;
using BoqiangH5Entity;
//using NLog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
//using Peak.Can.Basic;
using TPCANHandle = System.UInt16;
using TPCANTimestampFD = System.UInt64;

namespace BoqiangH5
{
    class PCANInterface
    {

        public event EventHandler RaiseRecvDataEvent;

        public void OnRaiseRecvDataEvent(object sender, EventArgs e)
        {
            if (RaiseRecvDataEvent != null)
            {
                RaiseRecvDataEvent(this, e);
            }
        }

        static PCANInterface m_pcanInstance;

        #region Delegates
        /// <summary>
        /// Read-Delegate Handler
        /// </summary>
        private delegate void ReadDelegateHandler();
        #endregion

        #region Members
        /// <summary>
        /// Saves the desired connection mode
        /// </summary>
        private bool m_IsFD;
        /// <summary>
        /// Saves the handle of a PCAN hardware
        /// </summary>
        private TPCANHandle m_PcanHandle;
        /// <summary>
        /// Saves the baudrate register for a conenction
        /// </summary>
        private TPCANBaudrate m_Baudrate;
        /// <summary>
        /// Saves the type of a non-plug-and-play hardware
        /// </summary>
        private TPCANType m_HwType;
        /// <summary>
        /// Stores the status of received messages for its display
        /// </summary>
        private System.Collections.ArrayList m_LastMsgsList;
        /// <summary>
        /// Read Delegate for calling the function "ReadMessages"
        /// </summary>
        private ReadDelegateHandler m_ReadDelegate;
        /// <summary>
        /// Receive-Event
        /// </summary>
        private System.Threading.AutoResetEvent m_ReceiveEvent;
        /// <summary>
        /// Thread for message reading (using events)
        /// </summary>
        private System.Threading.Thread m_ReadThread;
        /// <summary>
        /// Handles of non plug and play PCAN-Hardware
        /// </summary>
        private TPCANHandle[] m_NonPnPHandles;
        #endregion


        private PCANInterface()
        {
        }

        public static PCANInterface PCANInstance
        {
            get
            {
                if (m_pcanInstance == null)
                {
                    m_pcanInstance = new PCANInterface();
                }
                return m_pcanInstance;
            }
        }

        /// <summary>
        /// Help Function used to get an error as text
        /// </summary>
        /// <param name="error">Error code to be translated</param>
        /// <returns>A text with the translated error</returns>
        private string GetFormatedError(TPCANStatus error)
        {
            StringBuilder strTemp;

            // Creates a buffer big enough for a error-text
            //
            strTemp = new StringBuilder(256);
            // Gets the text using the GetErrorText API function
            // If the function success, the translated error is returned. If it fails,
            // a text describing the current error is returned.
            //
            if (PCANBasic.GetErrorText(error, 0, strTemp) != TPCANStatus.PCAN_ERROR_OK)
                return string.Format("An error occurred. Error-code's text ({0:X}) couldn't be retrieved", error);
            else
                return strTemp.ToString();
        }


        /// <summary>
        /// Configures the PCAN-Trace file for a PCAN-Basic Channel
        /// </summary>
        private void ConfigureTraceFile()
        {
            UInt32 iBuffer;
            TPCANStatus stsResult;

            // Configure the maximum size of a trace file to 5 megabytes
            //
            iBuffer = 5;
            stsResult = PCANBasic.SetValue(m_PcanHandle, TPCANParameter.PCAN_TRACE_SIZE, ref iBuffer, sizeof(UInt32));
            if (stsResult != TPCANStatus.PCAN_ERROR_OK)
                //IncludeTextMessage(GetFormatedError(stsResult));
                MessageBox.Show(GetFormatedError(stsResult));

            // Configure the way how trace files are created: 
            // * Standard name is used
            // * Existing file is ovewritten, 
            // * Only one file is created.
            // * Recording stopts when the file size reaches 5 megabytes.
            //
            iBuffer = PCANBasic.TRACE_FILE_SINGLE | PCANBasic.TRACE_FILE_OVERWRITE;
            stsResult = PCANBasic.SetValue(m_PcanHandle, TPCANParameter.PCAN_TRACE_CONFIGURE, ref iBuffer, sizeof(UInt32));
            if (stsResult != TPCANStatus.PCAN_ERROR_OK)
                //IncludeTextMessage(GetFormatedError(stsResult));
                MessageBox.Show(GetFormatedError(stsResult));
        }


        //private void btnInit_Click(object sender, EventArgs e)
        /*
         * strBitrate 波特率
         
         */
        private bool PCAN_Init(string strBitrate)
        {
            bool bInitRet = false;

            m_PcanHandle = Convert.ToUInt16("51", 16);
            m_Baudrate = TPCANBaudrate.PCAN_BAUD_500K;
            m_HwType = TPCANType.PCAN_TYPE_ISA;

            TPCANStatus stsResult;

            // Connects a selected PCAN-Basic channel
            //
            if (m_IsFD)
                stsResult = PCANBasic.InitializeFD( m_PcanHandle, strBitrate);
            else
                stsResult = PCANBasic.Initialize(
                    m_PcanHandle,
                    m_Baudrate,
                    m_HwType,
                    256,  // Convert.ToUInt32(strIO, 16),
                    3 );  // Convert.ToUInt16(strInterrupt));

            if (stsResult != TPCANStatus.PCAN_ERROR_OK)
            {
                if (stsResult != TPCANStatus.PCAN_ERROR_CAUTION)
                    //MessageBox.Show(GetFormatedError(stsResult));
                    bInitRet = false;
            }
            else
                // Prepares the PCAN-Basic's PCAN-Trace file
                bInitRet = true;

            return bInitRet;
        }

        //private void btnParameterSet_Click(object sender, EventArgs e)
        private bool PCAN_SetParameter(uint nDevice)
        {
            bool bSetRet = false;

            TPCANStatus stsResult;
            UInt32 iBuffer;
            bool bActivate;

            bActivate = true; //rdbParamActive.Checked;

            // Sets a PCAN-Basic parameter value
            //
            switch (0)
            {
                // The device identifier of a channel will be set
                //
                case 0:
                    iBuffer = nDevice; // Convert.ToUInt32(nudDeviceIdOrDelay.Value);
                    stsResult = PCANBasic.SetValue(m_PcanHandle, TPCANParameter.PCAN_DEVICE_ID, ref iBuffer, sizeof(UInt32));
                    //if (stsResult == TPCANStatus.PCAN_ERROR_OK)
                    //    IncludeTextMessage("The desired Device-ID was successfully configured");
                    bSetRet = true;
                    break;
                // The 5 Volt Power feature of a channel will be set
                //
                case 1:
                    iBuffer = (uint)(bActivate ? PCANBasic.PCAN_PARAMETER_ON : PCANBasic.PCAN_PARAMETER_OFF);
                    stsResult = PCANBasic.SetValue(m_PcanHandle, TPCANParameter.PCAN_5VOLTS_POWER, ref iBuffer, sizeof(UInt32));
                    break;
                // The feature for automatic reset on BUS-OFF will be set
                //
                case 2:
                    iBuffer = (uint)(bActivate ? PCANBasic.PCAN_PARAMETER_ON : PCANBasic.PCAN_PARAMETER_OFF);
                    stsResult = PCANBasic.SetValue(m_PcanHandle, TPCANParameter.PCAN_BUSOFF_AUTORESET, ref iBuffer, sizeof(UInt32));
                    break;
                // The CAN option "Listen Only" will be set
                //
                case 3:
                    iBuffer = (uint)(bActivate ? PCANBasic.PCAN_PARAMETER_ON : PCANBasic.PCAN_PARAMETER_OFF);
                    stsResult = PCANBasic.SetValue(m_PcanHandle, TPCANParameter.PCAN_LISTEN_ONLY, ref iBuffer, sizeof(UInt32));
                    break;
                // The feature for logging debug-information will be set
                //
                case 4:
                    iBuffer = (uint)(bActivate ? PCANBasic.PCAN_PARAMETER_ON : PCANBasic.PCAN_PARAMETER_OFF);
                    stsResult = PCANBasic.SetValue(PCANBasic.PCAN_NONEBUS, TPCANParameter.PCAN_LOG_STATUS, ref iBuffer, sizeof(UInt32));
                    //if (stsResult == TPCANStatus.PCAN_ERROR_OK)
                    //    IncludeTextMessage(string.Format("The feature for logging debug information was successfully {0}", bActivate ? "activated" : "deactivated"));
                    break;
                // The channel option "Receive Status" will be set
                //
                case 5:
                    iBuffer = (uint)(bActivate ? PCANBasic.PCAN_PARAMETER_ON : PCANBasic.PCAN_PARAMETER_OFF);
                    stsResult = PCANBasic.SetValue(m_PcanHandle, TPCANParameter.PCAN_RECEIVE_STATUS, ref iBuffer, sizeof(UInt32));
                    //if (stsResult == TPCANStatus.PCAN_ERROR_OK)
                    //    IncludeTextMessage(string.Format("The channel option \"Receive Status\" was set to {0}", bActivate ? "ON" : "OFF"));
                    break;
                // The feature for tracing will be set
                //
                case 7:
                    iBuffer = (uint)(bActivate ? PCANBasic.PCAN_PARAMETER_ON : PCANBasic.PCAN_PARAMETER_OFF);
                    stsResult = PCANBasic.SetValue(m_PcanHandle, TPCANParameter.PCAN_TRACE_STATUS, ref iBuffer, sizeof(UInt32));
                    //if (stsResult == TPCANStatus.PCAN_ERROR_OK)
                    //    IncludeTextMessage(string.Format("The feature for tracing data was successfully {0}", bActivate ? "activated" : "deactivated"));
                    break;

                // The feature for identifying an USB Channel will be set
                //
                case 8:
                    iBuffer = (uint)(bActivate ? PCANBasic.PCAN_PARAMETER_ON : PCANBasic.PCAN_PARAMETER_OFF);
                    stsResult = PCANBasic.SetValue(m_PcanHandle, TPCANParameter.PCAN_CHANNEL_IDENTIFYING, ref iBuffer, sizeof(UInt32));
                    //if (stsResult == TPCANStatus.PCAN_ERROR_OK)
                    //    IncludeTextMessage(string.Format("The procedure for channel identification was successfully {0}", bActivate ? "activated" : "deactivated"));
                    break;

                // The feature for using an already configured speed will be set
                //
                case 10:
                    iBuffer = (uint)(bActivate ? PCANBasic.PCAN_PARAMETER_ON : PCANBasic.PCAN_PARAMETER_OFF);
                    stsResult = PCANBasic.SetValue(m_PcanHandle, TPCANParameter.PCAN_BITRATE_ADAPTING, ref iBuffer, sizeof(UInt32));
                    //if (stsResult == TPCANStatus.PCAN_ERROR_OK)
                    //    IncludeTextMessage(string.Format("The feature for bit rate adaptation was successfully {0}", bActivate ? "activated" : "deactivated"));
                    break;

                // The option "Allow Status Frames" will be set
                //
                case 17:
                    iBuffer = (uint)(bActivate ? PCANBasic.PCAN_PARAMETER_ON : PCANBasic.PCAN_PARAMETER_OFF);
                    stsResult = PCANBasic.SetValue(m_PcanHandle, TPCANParameter.PCAN_ALLOW_STATUS_FRAMES, ref iBuffer, sizeof(UInt32));
                    //if (stsResult == TPCANStatus.PCAN_ERROR_OK)
                    //    IncludeTextMessage(string.Format("The reception of Status frames was successfully {0}", bActivate ? "enabled" : "disabled"));
                    break;

                // The option "Allow RTR Frames" will be set
                //
                case 18:
                    iBuffer = (uint)(bActivate ? PCANBasic.PCAN_PARAMETER_ON : PCANBasic.PCAN_PARAMETER_OFF);
                    stsResult = PCANBasic.SetValue(m_PcanHandle, TPCANParameter.PCAN_ALLOW_RTR_FRAMES, ref iBuffer, sizeof(UInt32));
                    //if (stsResult == TPCANStatus.PCAN_ERROR_OK)
                    //    IncludeTextMessage(string.Format("The reception of RTR frames was successfully {0}", bActivate ? "enabled" : "disabled"));
                    break;

                // The option "Allow Error Frames" will be set
                //
                case 19:
                    iBuffer = (uint)(bActivate ? PCANBasic.PCAN_PARAMETER_ON : PCANBasic.PCAN_PARAMETER_OFF);
                    stsResult = PCANBasic.SetValue(m_PcanHandle, TPCANParameter.PCAN_ALLOW_ERROR_FRAMES, ref iBuffer, sizeof(UInt32));
                    //if (stsResult == TPCANStatus.PCAN_ERROR_OK)
                    //    IncludeTextMessage(string.Format("The reception of Error frames was successfully {0}", bActivate ? "enabled" : "disabled"));
                    break;

                // The option "Interframes Delay" will be set
                //
                case 20:
                    iBuffer = nDevice; // Convert.ToUInt32(nudDeviceIdOrDelay.Value);
                    stsResult = PCANBasic.SetValue(m_PcanHandle, TPCANParameter.PCAN_INTERFRAME_DELAY, ref iBuffer, sizeof(UInt32));
                    //if (stsResult == TPCANStatus.PCAN_ERROR_OK)
                    //    IncludeTextMessage("The delay between transmitting frames was successfully set");
                    break;

                // The current parameter is invalid
                //
                default:
                    stsResult = TPCANStatus.PCAN_ERROR_UNKNOWN;
                    MessageBox.Show("Wrong parameter code.");
                    break;
            }

            // If the function fail, an error message is shown
            //
            if (stsResult != TPCANStatus.PCAN_ERROR_OK)
                //MessageBox.Show(GetFormatedError(stsResult));
                bSetRet = true;
            return bSetRet;
        }

        //private void btnWrite_Click(object sender, EventArgs e)
        public void PCAN_WriteData(uint nCAN_ID, byte byLen, bool isRemote, byte[] arrData)
        {
            try
            {
                DataLinkLayer.m_Mutex.WaitOne();
                TPCANStatus stsResult;

                // Send the message
                //
                //stsResult = m_IsFD ? WriteFrameFD() : WriteFrame();
                stsResult = WriteFrame(nCAN_ID, byLen, isRemote, arrData);

                // The message was successfully sent
                //
            }
            catch (Exception ex)
            { }
            finally
            {
                DataLinkLayer.m_Mutex.ReleaseMutex();
            }

        }

        private TPCANStatus WriteFrame(uint nCAN_ID, byte byLen, bool isRemote, byte[] arrData)
        {
            TPCANMsg CANMsg;
            //TextBox txtbCurrentTextBox;

            // We create a TPCANMsg message structure 
            //
            CANMsg = new TPCANMsg();
            CANMsg.DATA = new byte[8];

            // We configurate the Message.  The ID,
            // Length of the Data, Message Type
            // and the data
            //
            CANMsg.ID = nCAN_ID; // Convert.ToUInt32(txtID.Text, 16);
            CANMsg.LEN = byLen;  // Convert.ToByte(nudLength.Value);
            CANMsg.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_EXTENDED; // (chbExtended.Checked) ? TPCANMessageType.PCAN_MESSAGE_EXTENDED : TPCANMessageType.PCAN_MESSAGE_STANDARD;
            // If a remote frame will be sent, the data bytes are not important.
            //
            if (isRemote)
                CANMsg.MSGTYPE |= TPCANMessageType.PCAN_MESSAGE_RTR;
            else
            {
                Buffer.BlockCopy(arrData, 0, CANMsg.DATA, 0, byLen);
            }

            // The message is sent to the configured hardware
            //
            return PCANBasic.Write(m_PcanHandle, ref CANMsg);
        }



        /// <summary>
        /// Convert a CAN DLC value into the actual data length of the CAN/CAN-FD frame.
        /// </summary>
        /// <param name="dlc">A value between 0 and 15 (CAN and FD DLC range)</param>
        /// <param name="isSTD">A value indicating if the msg is a standard CAN (FD Flag not checked)</param>
        /// <returns>The length represented by the DLC</returns>
        public static int GetLengthFromDLC(int dlc, bool isSTD)
        {
            if (dlc <= 8)
                return dlc;

            if (isSTD)
                return 8;

            switch (dlc)
            {
                case 9: return 12;
                case 10: return 16;
                case 11: return 20;
                case 12: return 24;
                case 13: return 32;
                case 14: return 48;
                case 15: return 64;
                default: return dlc;
            }
        }


        public bool PCANInitSettings(string strBitrate, uint nDevice)
        {
            bool bRet = PCAN_Init(strBitrate);
            if(!bRet)
            {
                return bRet;
            }

            bRet = PCAN_SetParameter(nDevice);

            return bRet;
        }

        public bool ConnectRelease()
        {
            // Releases a current connected PCAN-Basic channel
            //
            TPCANStatus _status = PCANBasic.Uninitialize(m_PcanHandle);
            if(_status == TPCANStatus.PCAN_ERROR_OK)
            {
                //tmrRead.Enabled = false;
                if (m_ReadThread != null)
                {
                    m_ReadThread.Abort();
                    m_ReadThread.Join();
                    m_ReadThread = null;
                }
                return true;
            }

            return false;
            // Sets the connection status of the main-form
            //
            //SetConnectionStatus(false);
        }


#region
        int nTimes = 0;
        /// <summary>
        /// Function for reading PCAN-Basic messages
        /// </summary>
        public void ReadMessages()
        {
            TPCANStatus stsResult;

            // We read at least one time the queue looking for messages.
            // If a message is found, we look again trying to find more.
            // If the queue is empty or an error occurr, we get out from
            // the dowhile statement.
            //			
            //do

            byte[] byRecvData = new byte[256];
            int nRecvCount = 0;

            while (true)
            {
                TPCANMsg CANMsg;
                TPCANTimestamp CANTimeStamp;
                //TPCANStatus stsResult;

                // We execute the "Read" function of the PCANBasic                
                //
                stsResult = PCANBasic.Read(m_PcanHandle, out CANMsg, out CANTimeStamp);
                if (stsResult != TPCANStatus.PCAN_ERROR_QRCVEMPTY)
                {  // We process the received message
                    //
                    if ((CANMsg.ID & 0xFF) == 0x00 && RaiseRecvDataEvent != null)
                    {
                        byte[] byData = new byte[nRecvCount + CANMsg.LEN];
                        Buffer.BlockCopy(byRecvData, 0, byData, 0, nRecvCount);
                        Buffer.BlockCopy(CANMsg.DATA, 0, byData, nRecvCount, CANMsg.LEN);

                        OnRaiseRecvDataEvent(this, new CANEvent()
                        {
                            eventType = CANEventType.ReceEvent,
                            //ID = recvData.ID,
                            DataLen = byRecvData[2],


                            listData = new List<byte>(byData)

                        });

                        nRecvCount = 0;
                        nTimes++;
                     }
                    else
                    {
                        Buffer.BlockCopy(CANMsg.DATA, 0, byRecvData, nRecvCount, CANMsg.LEN);
                        nRecvCount += CANMsg.LEN;
                    }
                }
                else
                    Thread.Sleep(300);
                //stsResult = ReadMessage();
                if (stsResult == TPCANStatus.PCAN_ERROR_ILLOPERATION)
                    break;


            }
            //while (MainWindow.m_statusBarInfo.IsOnline && (!Convert.ToBoolean(stsResult & TPCANStatus.PCAN_ERROR_QRCVEMPTY)));
        }

        

#endregion
    }
}
