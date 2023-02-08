using System;

namespace ConsoleApp.Models
{
    /// <summary>
    /// 人员基础dto
    /// </summary>
    public class PersonnelBaseDto
    {
        /// <summary>
        /// 记录流水号  必填
        /// </summary>
        public string RecodeFlow { get; set; }
        /// <summary>
        /// 员工Id--职工工号
        /// </summary>
        public string StaffId { get; set; }
        /// <summary>
        /// 员工姓名
        /// </summary>
        public string StaffName { get; set; }
        /// <summary>
        /// 出生日期
        /// </summary>
        public DateTime DateOfBirth { get; set; }
        /// <summary>
        /// 籍贯
        /// </summary>
        public string Native { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public string SexDesc { get; set; }
        /// <summary>
        /// 年龄
        /// </summary>
        public int AgeDesc { get; set; }
        /// <summary>
        /// 出生日期(yyyyMMdd)
        /// </summary>
        public string DataOfBirth { get; set; }
        /// <summary>
        /// 民族
        /// </summary>
        public string Nation { get; set; }
        /// <summary>
        /// 身份证号
        /// </summary>
        public string CertNo { get; set; }
        /// <summary>
        /// 所属科室--科室名称
        /// </summary>
        public string SupDeptName { get; set; }
        /// <summary>
        /// 所属科室编码
        /// </summary>
        public string SupDeptCode { get; set; }
        /// <summary>
        /// 联系方式-手机号
        /// </summary>
        public string MobilePhone { get; set; }
        /// <summary>
        /// 联系方式-短号
        /// </summary>
        public string ShotPhone { get; set; }
        /// <summary>
        /// 人事档案所属组织机构编码 -- 医疗机构代码
        /// </summary>
        public string OrganizationCode { get; set; }
        /// <summary>
        /// 密码（不需要填，备用）
        /// </summary>
        public string PassWord { get; set; }
        /// <summary>
        /// 生效时间（yyyyMMddHHmmss）
        /// </summary>
        public string EffTime { get; set; }
        /// <summary>
        /// 停用时间（yyyyMMddHHmmss）
        /// </summary>
        public string ExpiTime { get; set; }
        /// <summary>
        /// 院区编码
        /// </summary>
        public string HospitalCode { get; set; }
        /// <summary>
        /// 行政职务编码
        /// </summary>
        public string DutyID { get; set; }
        /// <summary>
        /// 行政职务
        /// </summary>
        public string DutyName { get; set; }
        /// <summary>
        /// 职称
        /// </summary>
        public string AdminLevels { get; set; }
        /// <summary>
        /// 职称代码
        /// </summary>
        public string AdminLevelsCode { get; set; }
        /// <summary>
        /// 拼音码
        /// </summary>
        public string Py { get; set; }
        /// <summary>
        /// 简介
        /// </summary>
        public string Introduction { get; set; }
        /// <summary>
        /// 医疗权限
        /// </summary>
        public string MedicalAuthority { get; set; }
        /// <summary>
        /// 特殊权限
        /// </summary>
        public string SpecialAuthority { get; set; }
        /// <summary>
        /// 质控指标
        /// </summary>
        public string QualityIndex { get; set; }
        /// <summary>
        /// 系统账号
        /// </summary>
        public string UserCode { get; set; }
        /// <summary>
        /// 参加工作时间（yyyyMMdd）
        /// </summary>
        public string TimeOfWork { get; set; }
        public string TimeOfHosp { get; set; }
        /// <summary>
        /// 政治面貌
        /// </summary>
        public string PoliticCountenance { get; set; }
        /// <summary>
        /// 临床工作天数-出勤天数
        /// </summary>
        public string ClinicalWorkDayS { get; set; }
        /// <summary>
        /// 学历（初始）
        /// </summary>
        public string EducationNew { get; set; }
        /// <summary>
        /// 学历（最高）
        /// </summary>
        public string EducationNow { get; set; }
        /// <summary>
        /// 现从事专业
        /// </summary>
        public string ProfessionalWork { get; set; }
        /// <summary>
        /// 专业工作年限
        /// </summary>
        public string ProfessionalWorkYears { get; set; } = string.Empty;
        /// <summary>
        /// 现专业技术资格
        /// </summary>
        public string QualificationsOfPT { get; set; }
        /// <summary>
        /// 现专业技术资格取得时间
        /// </summary>
        public DateTime? TimeOfPT { get; set; }
        /// <summary>
        /// 现聘任专业技术职务
        /// </summary>
        public string AppointmentPositions { get; set; }
        /// <summary>
        /// 现聘任专业技术职务时间
        /// </summary>
        public DateTime? TimeOfAppointmentPositions { get; set; }
        /// <summary>
        /// 医师资格类别
        /// </summary>
        public string MedicalQualificationCategory { get; set; }
        /// <summary>
        /// 注册范围-执业范围
        /// </summary>
        public string PracticeArea { get; set; }
        /// <summary>
        /// 是否有援助经历（≥1.5年）
        /// </summary>
        public bool IsAidExperience { get; set; }
        /// <summary>
        /// 单位考核情况（近3年）
        /// </summary>
        public string UnitAssessment { get; set; }
        /// <summary>
        /// 进修经历
        /// </summary>
        public string RefresherExperience { get; set; }
        /// <summary>
        ///  援助经历描述
        /// </summary>
        public DescriptionOfAidExperience DescriptionOfAidExperience { get; set; }
        /// <summary>
        /// 职工荣誉描述
        /// </summary>
        public EmployeeHonourDescription EmployeeHonourDescription { get; set; }
        /// <summary>
        /// 患者投诉、医疗纠纷
        /// </summary>
        public PatientComplaintsDescription PatientComplaintsDescription { get; set; }
        /// <summary>
        /// 履历描述
        /// </summary>
        public ResumeDescription ResumeDescription { get; set; }
    }
    /// <summary>
    /// 援助经历描述
    /// </summary>
    public class DescriptionOfAidExperience
    {
        /// <summary>
        /// 援助类型【援非、援疆、援藏、援贵、双下沉】
        /// </summary>
        public string ExperienceType { get; set; }
        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// 单位名称
        /// </summary>
        public string UnitName { get; set; }
        /// <summary>
        /// 专科名称
        /// </summary>
        public string SpecialtyName { get; set; }
        /// <summary>
        /// 职务名称
        /// </summary>
        public string PositionTitle { get; set; }
    }
    /// <summary>
    /// 职工荣誉描述
    /// </summary>
    public class EmployeeHonourDescription
    {
        /// <summary>
        /// 个人荣誉事件发生时间(yyyyMMddHHmmss)
        /// </summary>
        public string TimeOfPersonalHonour { get; set; }
        /// <summary>
        /// 个人荣誉情况
        /// </summary>
        public string PersonalHonour { get; set; }
        /// <summary>
        /// 人才荣誉事件发生时间(yyyyMMddHHmmss)
        /// </summary>
        public string TimeOfHonorsOfTalents { get; set; }
        /// <summary>
        /// 人才荣誉情况
        /// </summary>
        public string HonorsOfTalents { get; set; }
    }
    /// <summary>
    /// 患者投诉、医疗纠纷
    /// </summary>
    public class PatientComplaintsDescription
    {
        /// <summary>
        /// 事件发生日期(yyyyMMddHHmmss)
        /// </summary>
        public string DateOfPatientComplaints { get; set; }
        /// <summary>
        /// 患者投诉、医疗纠纷情况 
        /// </summary>
        public string PatientComplaints { get; set; }
    }
    /// <summary>
    /// 履历描述
    /// </summary>
    public class ResumeDescription
    {
        /// <summary>
        /// 进修开始时间（派去其他医院进修）(yyyyMMdd)
        /// </summary>
        public string StartTimeOfFurtherStudy { get; set; }
        /// <summary>
        /// 进修结束时间（返回本院时间）(yyyyMMdd)
        /// </summary>
        public string StopTimeOfFurtherStudy { get; set; }
        /// <summary>
        /// 进修医院名称
        /// </summary>
        public string FurtherStudyHosp { get; set; }
        /// <summary>
        /// 进修科室名称
        /// </summary>
        public string FurtherStudyDeptName { get; set; }
        /// <summary>
        /// 获得奖励时间
        /// </summary>
        public DateTime? AwardTime { get; set; }
        /// <summary>
        /// 患者投诉、医疗纠纷时间
        /// </summary>
        public DateTime? TimeOfPatientComplaints { get; set; }
    }
}
