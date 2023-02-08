namespace ConsoleApp.Models
{
    /// <summary>
    /// 平湖 / 海泰人员修改 注销
    /// </summary>
    public class PersonnelUpdateDto : PersonnelBaseDto
    {
        /// <summary>
        /// UpdateType	更新类型 【1：修改；0：删除】
        /// </summary>
        public string UpdateType { get; set; }
    }
}
