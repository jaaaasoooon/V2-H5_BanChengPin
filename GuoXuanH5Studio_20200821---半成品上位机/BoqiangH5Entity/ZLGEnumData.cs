using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoqiangH5Entity
{
    /// <summary>
    /// CAN数据事件类型
    /// </summary>
    public enum CANEventType
    {
        SendEvent,//发送数据
        ReceEvent,//接收数据
        Other//其他信息
    };

    public enum DataLayerEventType
    {
        DataLayerSendEvent, //数据链路层发送事件
        DataLayerReceiveEvent,   //数据链路层接收事件
        Other
    };


    /// <summary>
    /// 网络层事件类型
    /// </summary>
    public enum NetWorkEventType
    {
        NetworkSendEvent,       //网络层发送事件
        NetworkReceiveEvent,    //网络层接收事件
        Other                   //其它事件
    };



    /// <summary>
    /// 应用层事件类型
    /// </summary>
    public enum AppEventType
    {
        AppSendEvent,       //应用层发送事件
        AppReceiveEvent,    //应用层接收事件
        Other               //其它事件
    };




    /// <summary>
    /// 具体的ZLG接口卡类型定义
    /// </summary>
    public enum ZLGType
    {
        VCI_PCI5121 = 1,
        VCI_PCI9810 = 2,
        VCI_USBCAN1 = 3,
        VCI_USBCAN2 = 4,
        VCI_USBCAN2A = 4,
        VCI_PCI9820 = 5,
        VCI_CAN232 = 6,
        VCI_PCI5110 = 7,
        VCI_CANLITE = 8,
        VCI_ISA9620 = 9,
        VCI_ISA5420 = 10,
        VCI_PC104CAN = 11,
        VCI_CANETUDP = 12,
        VCI_CANETE = 12,
        VCI_DNP9810 = 13,
        VCI_PCI9840 = 14,
        VCI_PC104CAN2 = 15,
        VCI_PCI9820I = 16,
        VCI_CANETTCP = 17,
        VCI_PEC9920 = 18,
        VCI_PCI5010U = 19,
        VCI_USBCAN_E_U = 20,
        VCI_USBCAN_2E_U = 21,
        VCI_PCI5020U = 22,
        VCI_EG20T_CAN = 23,
        VCI_PCIE9221 = 24,
        VCI_USBCANFD_100U = 42,  //2020.4.1  lipeng增加CAN驱动
        PCAN = 25//2020.04.22  lipeng增加PCAN驱动
    };

    /// <summary>
    /// 波特率定义
    /// </summary>
    public enum BaudRate
    {
        _5Kbps = 5,
        _10Kbps = 10,
        _20Kbps = 20,
        _40Kbps = 40,
        _50Kbps = 50,
        _80Kbps = 80,
        _100Kbps = 100,
        _125Kbps = 125,
        _200Kbps = 200,
        _250Kbps = 250,
        _400Kbps = 400,
        _500Kbps = 500,
        _666Kbps = 666,
        _800Kbps = 800,
        _1000Kbps = 1000
    };

    /// <summary>
    /// 仲裁域波特率定义
    /// </summary>
    public enum ArbitrationBaudRate
    {
        _1Mbps = 1000,
        _800Kbps = 800,
        _500Kbps = 500,
        _250Kbps = 250,
        _125Kbps =125,
        _100Kbps = 100,
        _50Kbps = 50,
    };

    /// <summary>
    /// 数据域波特率定义
    /// </summary>
    public enum DataBaudRate
    {
        _5Mbps = 5,
        _4Mbps = 4,
        _2Mbps = 2,
        _1Mbps = 1,
    };
}
