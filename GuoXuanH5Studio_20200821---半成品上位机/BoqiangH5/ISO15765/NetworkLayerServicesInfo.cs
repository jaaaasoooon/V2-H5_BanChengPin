using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoqiangH5.ISO15765
{
    public class NetworkLayerServicesInfo
    {
        /// <summary>
        /// Description: The parameter Mtype shalf be used to identify the type and range of address information
        /// parameters included in a service call. This part of ISO 15765 specifies a range of two values for
        /// this parameter. The intention is that users of the document can extend the range of values by
        /// specifying other types and combinations of address infonnation parameters to be used with the
        /// network layer protocol specified in this document. For each such new range of address
        /// information, a new value for the Mtype parameter shall be specified to identify the new address information.
        /// </summary>
        private MsgType mtype = MsgType.diagnostics;
        public MsgType MType
        {
            get { return mtype; }
            set
            {
                mtype = value;
            }
        }

        /// <summary>
        /// Network Source Address
        /// Description: The N_SA parameter shall be used to encode the sending network layer protocol entity_
        /// </summary>
        private byte n_SA;
        public byte N_SA
        {
            get { return n_SA; }
            set
            {
                n_SA = value;
            }
        }

        /// <summary>
        /// Network Target Address
        /// The N_TA parameter shall be used to encode the receiving network layer protocol entity.
        /// </summary>
        private byte n_TA;
        public byte N_TA
        {
            get { return n_TA; }
            set
            {
                n_TA = value;
            }
        }


        /// <summary>
        /// Network Target Address type
        /// Description: The parameter N_TAtype is an extension to the N_TA parameter. It shall be used to encode the
        /// communication model used by the communicating peer entities of the network layer. Two
        /// communication models are specified: 1 to 1 communication, called physical addressing, and
        /// 1 to n communication, called functional addressing.
        /// </summary>
        private TAtype n_TAtype = TAtype.physical;
        public TAtype N_Ttype
        {
            get { return n_TAtype; }
            set
            {
                n_TAtype = value;
            }
        }

        /// <summary>
        /// Network Address Extension
        /// Description: The N_AE parameter is used to extend the available address range for large networks, and to
        /// encode both sending and receiving network layer entities of subnets other than the local network:
        /// where the communication takes place. N_AE is only part of the addressing information if Mtype
        /// is set to remote diagnostics.
        /// </summary>
        private byte n_AE;
        public byte N_AE
        {
            get { return n_AE; }
            set
            {
                n_AE = value;
            }
        }


        /// <summary>
        /// 12Bit 字节长度
        /// Description: This parameter includes the length of data to be transmittedlreceived.
        /// </summary>
        private uint length;
        public uint Length
        {
            get { return length; }
            set
            {
                length = value;
            }
        }

        /// <summary>
        /// string of bytes
        /// Description: This parameter includes all data the higher layer entities exchange.
        /// </summary>
        private List<byte> messageData;
        public List<byte> MessageData
        {
            get { return messageData; }
            set
            {
                messageData = value;
            }
        }

        /// <summary>
        /// Parameter
        /// Description: This parameter identifies a parameter of the network layer.
        /// </summary>
        private ParameterType parameter = ParameterType.STmin;
        public ParameterType Parameter
        {
            get { return parameter; }
            set
            {
                parameter = value;
            }
        }

        /// <summary>
        /// Parameter-Value
        /// Description: This parameter is assigned to a protocol parameter <Parameter> as indicated in the service section of this document.
        /// </summary>
        private byte parameter_Value;
        public byte Parameter_Value
        {
            get { return parameter_Value; }
            set
            {
                parameter_Value = value;
            }
        }


        /// <summary>
        /// Description: This parameter contains the status relating to the outcome of a service execution. If two or more
        ///errors are discovered at the same time, then the network layer entity shall use the parameter
        ///value first found in this list in the error indication to the higher layers.
        /// </summary>
        private Result n_Result;
        public Result N_Result
        {
            get { return n_Result; }
            set
            {
                n_Result = value;
            }
        }

        /// <summary>
        /// Description: This parameter contains the status relating to the outcome of a service execution.
        /// </summary>
        //private ChangeParameterResult resuIt_ChangeParameter;
        public ChangeParameterResult ResuIt_ChangeParameter { get; set; }



    }

    /// <summary>
    /// 消息类型
    /// </summary>
    public enum MsgType
    {
        diagnostics,        //If Mtype = diagnostics, then the address information N-.AI shall consist of the parameters
                            //N_SA, N_TA, and N_TAtype.

        remote_diagnostics  //If Mtype = remote diagnostics, then the address
                            //parameters N_SA, N_TA, N_TAtype, and N_AE.
    }

    /// <summary>
    /// 目标地址类型
    /// </summary>
    public enum TAtype
    {
        physical,
        functional
    }

    public enum ParameterType
    {
        STmin,
        BS
    }

    /// <summary>
    /// Description: This parameter contains the status relating to the outcome of a service execution. If two or more
    ///errors are discovered at the same time, then the network layer entity shall use the parameter
    ///value first found in this list in the error indication to the higher layers.
    /// </summary>
    public enum Result
    {
        N_OK,                //This value means that the service execution has completed successfully;it can be issued to a service user on both the sender and receiver side.

        N_TIMEOUT_A,         //This value is issued to the protocol user when the timer N.fir/N_As has passed its time-out value N_Asmax'N_Armax;it can be issued to service user on both the sender and receiver side.

        N_TIMEOUT_Bs,       //This value is issued to the service user when the timer N_Bs has passed its time-out value N_Bsmax;it can be issued to the service user on the sender side only.

        N_TIMEOUT_Cr,       //This value is issued to the service user when the timer N_Cr has passed its time-out value N_Crmax; it can be issued to the service user on the receiver side only.

        N_WRONG_SN,         //This value is issued to the service user upon reception of an unexpected sequence number (PCI.SN) value; it can be issued to the service user on the receiver side only.

        N_INVALID_FS,       //This value is issued to the service user when an invalid or unknown FlowStatus value has been received in a flow control (FC) N_PDU; it can be issued to the service user on the sender side only.

        N_UNEXP_PDU,        //This value is issued to the service user upon reception of an unexpected protocol data unit;it can be issued to the service user on the receiver side only.

        N_WFT_OVRN,         //This value is issued to the service user upon reception of flow control WAIT frame that exceeds the maximum counter N-WFTmax.

        N_BUFFER_OVFLW,     //This value is issued to the service user upon reception of a flow control (FC) N_PDU with
                            //FlowStatus =OVFLW. It indicates that the buffer on the receiver side of a segmented
                            //message transmission cannot store the number of bytes specified by the FirstFrame
                            //DataLength (FF_DL) parameter in the FirstFrame and therefore the transmission of the
                            //segmented message was aborted. It can be issued to the service user on the sender side
                            //only.

        N_ERROR             //This is the general error value. It shall be issued to the service user when an error has been
                            //detected by the network layer and no other parameter value can be used to better describe
                            //the error. It can be issued to the service user on both the sender and receiver side.
    }

    /// <summary>
    /// Description: This parameter contains the status relating to the outcome of a service execution.
    /// </summary>
    public enum ChangeParameterResult
    {
        N_OK,               //This value means that the service execution has completed successfully; it can be issued to
                            //a service user on both the sender and receiver side.

        N_RX_ON,            //This value is issued to the service user to indicate that the service did not execute since a
                            //reception of the message identified by <AI> was taking place. It can be issued to the service
                            //user on the receiver side only.

        N_WRONG_PARAMETER,  //This value is issued to the service user to indicate that the service did not execute due to an
                            //undefined <Parameter>; it can be issued to the service user on both the receiver and sender side.

        N_WRONG_VALUE,      //This value is issued to the service user to indicate that the service did not execute due to an
                            //out of range <Parameter_Value>; it can be issued to the service user on both the receiver
                            //and sender side.
    }
}
