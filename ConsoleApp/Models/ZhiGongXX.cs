using System.ComponentModel.DataAnnotations.Schema;

namespace ConsoleApp.Models
{
    /// <summary>
    /// 职工信息
    /// </summary>
    [Table("JWZN_ZHIGONGXX")]
    public class ZhiGongXX
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 职工工号
        /// </summary>
        public string ZHIGONGGH { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string XINGMING { get; set; }
        /// <summary>
        /// 性别代码
        /// </summary>
        public string XINGBIE { get; set; }

        public override string ToString()
        {
            string xingBie = this.XINGBIE == "1" ? "男" : "女";
            return $"主键={this.ID},工号={this.ZHIGONGGH},\n姓名={this.XINGMING},性别={xingBie}";
        }
    }
}