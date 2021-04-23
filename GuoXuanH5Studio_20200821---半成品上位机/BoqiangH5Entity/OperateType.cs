using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoqiangH5Entity
{
    public enum OperateType
    {
        NoAction,

        #region 主控参数DID起止(1400, 15FF)
        WriteMasterPara,
        WriteAllMasterPara,
        #endregion

        #region 主控校准DID起止(1600, 17FF)
        //GetMasterAdjustPara,
        WriteMasterAdjustPara,
        //GetAllMasterAdjustPara,
        WriteAllMasterAdjustPara,
        #endregion

        #region 主控控制DID起止(1800, 18FF)
        WriteMasterControlPara,
        ReadBmsTime,
        WriteBmsTime,
        #endregion
    }
}
