using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoqiangH5Entity
{

    public enum H5Protocol
    {
        BO_QIANG = 0,
        DI_DI,        
    }

    //public enum H5_CAN_ID
    //{
    //    BO_QIANG_ID = 0x1CEB0300,
    //    DI_DI_ID = 0x14050320
    //}

    public enum BoqiangH5Wnd
    {
        BMS_WND = 0,
        EEPROM_WND,
        MCU_WND,
        RECORD_WND,
        ADJUST_WND,
    }
}
