using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoqiangH5Entity
{

    /// <summary>
    /// // 主控控制DID起止(1800, 18FF)
    /// </summary>
    public class MasterAdjustInfo : H5BmsInfo
    {
        //{ 1800,"参数恢复出厂值"}
        // 1801	参数写入FLASH
        // 1802	BMS系统时间
        // 1803	调试模式开关
        // 1804	继电器强制开关状态
        // 1805	工厂模式高压上下电
        // 1806	电流小环采样零点校准
        // 1807	电流小环采样比例校准
        // 1808	电流大环采样零点校准
        // 1809	电流大环采样比例校准
        // 180A	主电源采样校准
        // 180B	RTC电源采样校准
        // 180C	PCB温度采样校准
        // 180D	EXT_AD1采样校准
        // 180E	EXT_AD2采样校准
        // 180F	HVP采样零点校准
        // 1810	HVP采样比例校准
        // 1811	HVB采样零点校准
        // 1812	HVB采样比例校准
        // 1813	负极绝缘采样零点校准
        // 1814	正极绝缘采样零点校准
        // 1815	电池额定容量校准
        // 1816	电池当前容量校准
        // 1817	校准写入FLASH
        // 1818	校准恢复出厂值
        // 1819	版本信息

        //public event PropertyChangedEventHandler AdjustInfoPropertyChanged;

        public bool IsComBoxVisibility { get; set; }  // 设置 ComboBox 是否隐藏

        public bool IsFunctionEnabled { get; set; }   // 设置 “执行” 按钮是否可用

        private List<string> listComBoxOperate;       // ComboBox 选项

        public List<string> ListComBoxOperate
        {
            get { return listComBoxOperate; }
            set
            {
                if (listComBoxOperate != value)
                    listComBoxOperate = value;
            }
        }

        public int SelectedIndex { get; set; }       // 记录 ComboBox 的 SelectedIndex

    }
}
