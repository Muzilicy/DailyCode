using ConsoleApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ConsoleApp
{
    #region class
    public class ResponseModel
    {
        public ResponseTest Response { get; set; }
    }

    public class ResponseTest
    {
        public HeadTest Head { get; set; }
        public BodyTest Body { get; set; }
        public string Msg { get; set; }
    }
    public class HeadTest
    {
        public ReturnTest Return { get; set; }
        public string Test { get; set; }
    }

    public class BodyTest
    {
        public PatientInfoModel PatientInfo { get; set; }
    }
    public class PatientInfoModel
    {
        public string Name { get; set; }
    }
    public class ReturnTest
    {
        public string Source
        {
            get;
            set;
        }
        public string RecordFlow
        {
            get;
            set;
        }
        public string TransNo
        {
            get;
            set;
        }
        /// <summary>
        /// 1：成功，0：失败
        /// </summary>
        public int RetCode
        {
            get;
            set;
        }
        /// <summary>
        /// 处理类型必填
        /// 1-不需要提示，允许该项目录入；
        /// 2-需要进行提示，允许该项目录入；
        /// 3-需要进行提示，不允许改项目录入；
        /// 4-需要进行提示，用户选择【是】公费，【否】自费，【取消】取消；
        /// 5-需要进行提示，是否确认按自费使用；
        /// </summary>
        public string RetType
        {
            get;
            set;
        }
        public string RetCont
        {
            get;
            set;
        }
    }
    public class Publisher
    {
        public string Name { get; set; }
    }
    public class Author
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
    public class Subject
    {
        public string Name { get; set; }

    }
    public class Book
    {
        public string Title { get; set; }
        public Publisher Publisher { get; set; }

        public Author[] Authors { get; set; }

        public int PageCount { get; set; }

        public decimal Price { get; set; }

        public DateTime PublicationDate { get; set; }

        public string Isbn { get; set; }

        public Subject Subject { get; set; }
    }
    static public class SampleData
    {
        static public Publisher[] Publishers =
        {
          new Publisher {Name="FunBooks"},
          new Publisher {Name="Joe Publishing"},
          new Publisher {Name="I Publisher"}
        };

        static public Author[] Authors =
        {
      new Author {FirstName="Johnny", LastName="Good"},
      new Author {FirstName="Graziella", LastName="Simplegame"},
      new Author {FirstName="Octavio", LastName="Prince"},
      new Author {FirstName="Jeremy", LastName="Legrand"}
    };

        static public Subject[] Subjects =
        {
          new Subject {Name="Software development"},
          new Subject {Name="Novel"},
          new Subject {Name="Science fiction"}
        };

        static public Book[] Books =
        {
          new Book {
            Title="Funny Stories",
            Publisher=Publishers[0],
            Authors=new[]{Authors[0], Authors[1]},
            PageCount=101,
            Price=25.55M,
            PublicationDate=new DateTime(2004, 11, 10),
            Isbn="0-000-77777-2",
            Subject=Subjects[0]
          },
          new Book {
            Title="LINQ rules",
            Publisher=Publishers[1],
            Authors=new[]{Authors[2]},
            PageCount=300,
            Price=12M,
            PublicationDate=new DateTime(2007, 9, 2),
            Isbn="0-111-77777-2",
            Subject=Subjects[0]
          },
          new Book {
            Title="C# on Rails",
            Publisher=Publishers[1],
            Authors=new[]{Authors[2]},
            PageCount=256,
            Price=35.5M,
            PublicationDate=new DateTime(2007, 4, 1),
            Isbn="0-222-77777-2",
            Subject=Subjects[0]
          },
          new Book {
            Title="All your base are belong to us",
            Publisher=Publishers[1],
            Authors=new[]{Authors[3]},
            PageCount=1205,
            Price=35.5M,
            PublicationDate=new DateTime(2006, 5, 5),
            Isbn="0-333-77777-2",
            Subject=Subjects[2]
          },
          new Book {
            Title="Bonjour mon Amour",
            Publisher=Publishers[0],
            Authors=new[]{Authors[1], Authors[0]},
            PageCount=50,
            Price=29M,
            PublicationDate=new DateTime(1973, 2, 18),
            Isbn="2-444-77777-2",
            Subject=Subjects[1]
          }
        };
    }
    //https://www.cnblogs.com/scottckt/archive/2010/08/11/1797716.html 
    #endregion

    class Program
    {
        private static string GetGangWeiLBName(string code)
        {
            if (string.IsNullOrEmpty(code)) return string.Empty;
            Dictionary<string, object> dic = new Dictionary<string, object>()
            {
                {"A1","医生"},
                {"A2","护士"},
                {"A3","药剂"},
                {"A4","卫生技术"},
                {"A5","兽医"},
                {"B1","经济"},
                {"B2","会计"},
                {"B3","统计"},
                {"C1","高校教师"},
                {"C2","中专教师"},
                {"C3","技校教师"},
                {"C4","中学教师"},
                {"D1","科学研究"},
                {"D2","科研实验"},
                {"D3","工程技术"},
                {"E1","文档资料"},
                {"E2","出版"},
                {"E3","翻译"},
                {"E4","新闻"},
                {"F1","行政管理"},
                {"F2","政治"},
                {"F3","后勤"},
                {"G1","门诊收款员"},
                {"G2","住院收款员"},
                {"G3","自助机"},
                {"Z1","其他"}
            };
            return dic.ContainsKey(code) ? dic[code].ToString() : string.Empty;
        }

        /// <summary>
        /// 根据传入的编码创建新的编号
        /// </summary>
        /// <param name="str">当前最新的编号</param>
        /// <param name="qianZhui">当前最新的编号</param>
        /// <param name="count">编号后几位需要自增</param>
        /// <param name="step">步长</param>
        /// <returns></returns>
        static string CreateGongHao(string str, string qianZhui, int count , int step = 1)
        {
            if(string.IsNullOrWhiteSpace(qianZhui))
            {
                qianZhui = string.Empty;
            }
            if(count == 0)
            {
                count = 1;
            }
            //如果传入的最新编码为空或不存在对应的编码
            if (string.IsNullOrWhiteSpace(str) || !str.Contains(qianZhui))
            {
                return qianZhui + step.ToString().PadLeft(count, '0');
            }

            //最新编码的后缀
            var houZhui = str.Replace(qianZhui, "");
            if (houZhui.Length > count)
            {
                houZhui = houZhui.Substring(houZhui.Length - count, count);
            }

            int number;
            if(!int.TryParse(houZhui,out number))
            {
                return qianZhui + step.ToString().PadLeft(count, '0');
            }
            return qianZhui + (number + step).ToString().PadLeft(count, '0');
        }

        static bool IsBaseType(Type type)
        {
            List<Type> list = new List<Type> { typeof(DateTime?), typeof(string), typeof(int) };
            return list.Contains(type);
        }
        static void Main(string[] args)
        {
            #region linq
            //var min = from file in System.IO.Directory.GetFiles(@"D:\daily\inside\WM3Core\Doc\WM3.Doc.Rule")
            //          where File.Exists(file)
            //          select new { key = File.GetCreationTime(file), val = file } into filelist
            //          orderby filelist.key descending
            //          select filelist.val;
            //var min = from file in System.IO.Directory.GetFiles(@"D:\daily\inside\WM3Core\Doc\WM3.Doc.Rule")
            //          where File.Exists(file)
            //          let time = File.GetLastWriteTime(file)
            //          orderby time descending
            //          select new { key = File.GetLastWriteTime(file), val = file };


            //var str2 = "[{\"hidden\":false,\"minWidth\":120,\"prop\":\"xingMing\",\"label\":\"姓名\",\"field\":true,\"slot\":\"xingMing\",\"fixed\":false,\"sortable\":true,\"disable\":false},{\"hidden\":false,\"prop\":\"renShiKS\",\"label\":\"所在科室\",\"field\":true,\"slot\":\"renShiKS\",\"fixed\":false,\"sortable\":true,\"disable\":false},{\"hidden\":false,\"minWidth\":120,\"prop\":\"minZu\",\"label\":\"民族\",\"field\":true,\"slot\":\"minZu\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":false,\"minWidth\":120,\"prop\":\"jiGuan\",\"label\":\"籍贯\",\"field\":true,\"slot\":\"jiGuan\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":false,\"minWidth\":120,\"prop\":\"diERXL\",\"label\":\"最高学历\",\"field\":true,\"slot\":\"diERXL\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":false,\"minWidth\":170,\"prop\":\"shenFenZH\",\"label\":\"身份证号\",\"field\":true,\"slot\":\"shenFenZH\",\"fixed\":false,\"sortable\":true,\"disable\":false},{\"hidden\":false,\"minWidth\":120,\"prop\":\"chuShengRQ\",\"label\":\"出生日期\",\"field\":true,\"slot\":\"chuShengRQ\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":false,\"minWidth\":120,\"prop\":\"dangAnCSRQ\",\"label\":\"档案出生日期\",\"field\":true,\"slot\":\"dangAnCSRQ\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":false,\"minWidth\":120,\"prop\":\"shenFenLB\",\"label\":\"身份类别\",\"field\":true,\"slot\":\"shenFenLB\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":false,\"minWidth\":120,\"prop\":\"zhiGongXZ\",\"label\":\"职工性质\",\"field\":true,\"slot\":\"zhiGongXZ\",\"fixed\":false,\"sortable\":true,\"disable\":false},{\"hidden\":false,\"prop\":\"gangWeiLB\",\"label\":\"岗位类别\",\"field\":true,\"slot\":\"gangWeiLB\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"haiWaiLXBZ\",\"label\":\"海外留学标志\",\"field\":true,\"slot\":\"haiWaiLXBZ\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"zhiGongGH\",\"label\":\"职工工号\",\"field\":true,\"slot\":\"zhiGongGH\",\"fixed\":false,\"sortable\":true,\"disable\":false},{\"hidden\":true,\"prop\":\"zhuanYeKS\",\"label\":\"专业科室\",\"field\":true,\"slot\":\"zhuanYeKS\",\"fixed\":false,\"sortable\":true,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"canJiaGZRQ\",\"label\":\"参加工作日期\",\"field\":true,\"slot\":\"canJiaGZRQ\",\"fixed\":false,\"sortable\":true,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"xingZhengZW\",\"label\":\"行政职务\",\"field\":true,\"slot\":\"xingZhengZW\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"zhengZhiMM\",\"label\":\"政治面貌\",\"field\":true,\"slot\":\"zhengZhiMM\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"dangNeiZW\",\"label\":\"党内职务\",\"field\":true,\"slot\":\"dangNeiZW\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"dangNeiZWRMRQ\",\"label\":\"党内职务任命时间\",\"field\":true,\"slot\":\"dangNeiZWRMRQ\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"jiaTingDZ\",\"label\":\"家庭地址\",\"field\":true,\"slot\":\"jiaTingDZ\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"zhiYeZSHQSJ\",\"label\":\"取得时间\",\"field\":true,\"slot\":\"zhiYeZSHQSJ\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"duoJiGYXQKSRQ\",\"label\":\"多机构有效期开始日期\",\"field\":true,\"slot\":\"duoJiGYXQKSRQ\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"zhiChengHDRQ\",\"label\":\"职称获得日期\",\"field\":true,\"slot\":\"zhiChengHDRQ\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"cengYongMing\",\"label\":\"曾用名\",\"field\":true,\"slot\":\"cengYongMing\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"diErXZ\",\"label\":\"最高学制\",\"field\":true,\"slot\":\"diErXZ\",\"fixed\":false,\"sortable\":true,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"yinHangKH\",\"label\":\"银行卡号\",\"field\":true,\"slot\":\"yinHangKH\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"xueLi\",\"label\":\"初始学历\",\"field\":true,\"slot\":\"xueLi\",\"fixed\":false,\"sortable\":true,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"zhiCheng\",\"label\":\"职称\",\"field\":true,\"slot\":\"zhiCheng\",\"fixed\":false,\"sortable\":true,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"daoYuanRQ\",\"label\":\"到单位日期\",\"field\":true,\"slot\":\"daoYuanRQ\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"ruDangRQ\",\"label\":\"入团党日期\",\"field\":true,\"slot\":\"ruDangRQ\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"suoZaiZB\",\"label\":\"所在支部\",\"field\":true,\"slot\":\"suoZaiZB\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"renShiDAGLDW\",\"label\":\"档案管理单位\",\"field\":true,\"slot\":\"renShiDAGLDW\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"zengJiaFS\",\"label\":\"入院方式\",\"field\":true,\"slot\":\"zengJiaFS\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"shiYongQBZ\",\"label\":\"是否试用期\",\"field\":true,\"slot\":\"shiYongQBZ\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"shiYongKSRQ\",\"label\":\"试用开始日期\",\"field\":true,\"slot\":\"shiYongKSRQ\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"shiYongJSRQ\",\"label\":\"试用结束日期\",\"field\":true,\"slot\":\"shiYongJSRQ\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"jiaTingDH\",\"label\":\"家庭电话\",\"field\":true,\"slot\":\"jiaTingDH\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"xiXueZBZ\",\"label\":\"双肩挑\",\"field\":true,\"slot\":\"xiXueZBZ\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"zhiYeZSBH\",\"label\":\"执业证号\",\"field\":true,\"slot\":\"zhiYeZSBH\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"gangWeiDJ\",\"label\":\"岗位等级\",\"field\":true,\"slot\":\"gangWeiDJ\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"bianZhi\",\"label\":\"编制\",\"field\":true,\"slot\":\"bianZhi\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"zhuXiaoRQ\",\"label\":\"注销日期\",\"field\":true,\"slot\":\"zhuXiaoRQ\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"shenFenZXM\",\"label\":\"身份证姓名\",\"field\":true,\"slot\":\"shenFenZXM\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"zhuanYeGW\",\"label\":\"专业岗位\",\"field\":true,\"slot\":\"zhuanYeGW\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"gongQinGW\",\"label\":\"工勤岗位\",\"field\":true,\"slot\":\"gongQinGW\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"guanLiGW\",\"label\":\"管理岗位\",\"field\":true,\"slot\":\"guanLiGW\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"jinBianRQ\",\"label\":\"进编时间\",\"field\":true,\"slot\":\"jinBianRQ\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"biYeRQ\",\"label\":\"毕业日期\",\"field\":true,\"slot\":\"biYeRQ\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"biYeYX\",\"label\":\"毕业院校\",\"field\":true,\"slot\":\"biYeYX\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"diERXLBYRQ\",\"label\":\"最高学历毕业日期\",\"field\":true,\"slot\":\"diERXLBYRQ\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"diERXLBYYX\",\"label\":\"最高学历毕业院校\",\"field\":true,\"slot\":\"diERXLBYYX\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"jiaoYuLB\",\"label\":\"教育类别\",\"field\":true,\"slot\":\"jiaoYuLB\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"diERXLJYLB\",\"label\":\"最高学历教育类别\",\"field\":true,\"slot\":\"diERXLJYLB\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"pinRenRQ\",\"label\":\"聘任日期\",\"field\":true,\"slot\":\"pinRenRQ\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"suoXueZY\",\"label\":\"所学专业\",\"field\":true,\"slot\":\"suoXueZY\",\"fixed\":false,\"sortable\":true,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"xueZhi\",\"label\":\"学制\",\"field\":true,\"slot\":\"xueZhi\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"xueWei\",\"label\":\"学位\",\"field\":true,\"slot\":\"xueWei\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"xueWeiRQ\",\"label\":\"学位日期\",\"field\":true,\"slot\":\"xueWeiRQ\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"liTuiRQ\",\"label\":\"离退日期\",\"field\":true,\"slot\":\"liTuiRQ\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"yingWenMing\",\"label\":\"老考勤\",\"field\":true,\"slot\":\"yingWenMing\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"yiShengDJ\",\"label\":\"医生等级\",\"field\":true,\"slot\":\"yiShengDJ\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"jinJiLXRXM\",\"label\":\"紧急联系人姓名\",\"field\":true,\"slot\":\"jinJiLXRXM\",\"fixed\":false,\"sortable\":true,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"jinJiLXRGX\",\"label\":\"紧急联系人关系\",\"field\":true,\"slot\":\"jinJiLXRGX\",\"fixed\":false,\"sortable\":true,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"jinJiLXRDH\",\"label\":\"紧急联系人电话\",\"field\":true,\"slot\":\"jinJiLXRDH\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"bianZhiXZ\",\"label\":\"编制性质\",\"field\":true,\"slot\":\"bianZhiXZ\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"diErXW\",\"label\":\"最高学位\",\"field\":true,\"slot\":\"diErXW\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"zhiWuDJ\",\"label\":\"职务等级\",\"field\":true,\"slot\":\"zhiWuDJ\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"zhaoPian\",\"label\":\"照片\",\"field\":true,\"slot\":\"zhaoPian\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"huLingZSKSRQ\",\"label\":\"护龄折算开始日期\",\"field\":true,\"slot\":\"huLingZSKSRQ\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"ziDuan10\",\"label\":\"测试字段10\",\"field\":true,\"slot\":\"ziDuan10\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"dangAnHao\",\"label\":\"档案号\",\"field\":true,\"slot\":\"dangAnHao\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"xingBie\",\"label\":\"性别\",\"field\":true,\"slot\":\"xingBie\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"shouJiHao\",\"label\":\"手机号\",\"field\":true,\"slot\":\"shouJiHao\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"hunYinZK\",\"label\":\"婚姻状况\",\"field\":true,\"slot\":\"hunYinZK\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"huKouXZ\",\"label\":\"户口性质\",\"field\":true,\"slot\":\"huKouXZ\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"huKouSZD\",\"label\":\"户口所在地\",\"field\":true,\"slot\":\"huKouSZD\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"beiZhu\",\"label\":\"备注\",\"field\":true,\"slot\":\"beiZhu\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"yiLiaoJG\",\"label\":\"所属机构\",\"field\":true,\"slot\":\"yiLiaoJG\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"kaoQinZu\",\"label\":\"考勤组\",\"field\":true,\"slot\":\"kaoQinZu\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"zhiGongLB\",\"label\":\"职工类别\",\"field\":true,\"slot\":\"zhiGongLB\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"huLiDJ\",\"label\":\"护理等级\",\"field\":true,\"slot\":\"huLiDJ\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"huShiXH\",\"label\":\"护士鞋号\",\"field\":true,\"slot\":\"huShiXH\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"yiFuCC\",\"label\":\"衣服尺寸\",\"field\":true,\"slot\":\"yiFuCC\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"zhiYeZGBH\",\"label\":\"资格证号\",\"field\":true,\"slot\":\"zhiYeZGBH\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"zhiYeFW\",\"label\":\"执业范围\",\"field\":true,\"slot\":\"zhiYeFW\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"zhiYeZG\",\"label\":\"执业资格\",\"field\":true,\"slot\":\"zhiYeZG\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"zhiYeLB\",\"label\":\"执业类别\",\"field\":true,\"slot\":\"zhiYeLB\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"zhiYeNX\",\"label\":\"执业年限\",\"field\":true,\"slot\":\"zhiYeNX\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"zhiYeJB\",\"label\":\"执业级别\",\"field\":true,\"slot\":\"zhiYeJB\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"zhiYeZT\",\"label\":\"执业状态\",\"field\":true,\"slot\":\"zhiYeZT\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"zhiYeKSRQ\",\"label\":\"执业开始日期\",\"field\":true,\"slot\":\"zhiYeKSRQ\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"duoJiGMC\",\"label\":\"多机构名称\",\"field\":true,\"slot\":\"duoJiGMC\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"duoJiGZYFW\",\"label\":\"多机构执业范围\",\"field\":true,\"slot\":\"duoJiGZYFW\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"duoJiGYXQJSRQ\",\"label\":\"多机构有效期结束日期\",\"field\":true,\"slot\":\"duoJiGYXQJSRQ\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"zhiYeDD\",\"label\":\"执业地点\",\"field\":true,\"slot\":\"zhiYeDD\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"zhuYaoZYJG\",\"label\":\"主要执业机构\",\"field\":true,\"slot\":\"zhuYaoZYJG\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"peiXuJG\",\"label\":\"培训机构\",\"field\":true,\"slot\":\"peiXuJG\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"qianFaJG\",\"label\":\"签发机关\",\"field\":true,\"slot\":\"qianFaJG\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"xuNiWDH\",\"label\":\"虚拟网电话\",\"field\":true,\"slot\":\"xuNiWDH\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"dianZiYJ\",\"label\":\"电子邮件\",\"field\":true,\"slot\":\"dianZiYJ\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"gongZuoDH\",\"label\":\"工作电话\",\"field\":true,\"slot\":\"gongZuoDH\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"zaiZhiZT\",\"label\":\"在职状态\",\"field\":true,\"slot\":\"zaiZhiZT\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"zhuXiaoYY\",\"label\":\"注销原因\",\"field\":true,\"slot\":\"zhuXiaoYY\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"yunXuDLBZ\",\"label\":\"允许登录标志\",\"field\":true,\"slot\":\"yunXuDLBZ\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"chuShengDi\",\"label\":\"出生地\",\"field\":true,\"slot\":\"chuShengDi\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"pinRenZC\",\"label\":\"聘任职称\",\"field\":true,\"slot\":\"pinRenZC\",\"fixed\":false,\"sortable\":true,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"jiShuZW\",\"label\":\"临床职务\",\"field\":true,\"slot\":\"jiShuZW\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":true,\"minWidth\":120,\"prop\":\"bianZhiDW\",\"label\":\"编制单位\",\"field\":true,\"slot\":\"bianZhiDW\",\"fixed\":false,\"sortable\":false,\"disable\":false},{\"hidden\":false,\"slot\":\"operate\",\"control\":true,\"fixed\":\"right\",\"width\":120,\"sortable\":false,\"disable\":false}]";

            //Console.WriteLine(str2.Length);

            //var min = System.IO.Directory.GetFiles("D:\\daily\\inside\\WM3Core\\Doc\\WM3.Doc.Rule")
            //                   .Where(x => File.Exists(x)).OrderBy(o => File.GetLastWriteTime(o));
            //foreach (var item in min)
            //{
            //    Console.WriteLine(item.key + " " + item.val);
            //} 
            #endregion

            #region generateGH
            //int i = 6,j = 0;
            //string maxNo = "test2022x1";
            //while (j < 10)
            //{
            //    maxNo = CreateGongHao(maxNo, "test", i);
            //    Console.WriteLine(maxNo);
            //    j++;
            //} 
            #endregion

            XmlDocument xmlDoc = new XmlDocument();
            //创建类型声明的节点
            XmlNode noticeNode = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", string.Empty);
            xmlDoc.AppendChild(noticeNode);
            ////创建根结点
            //XmlNode root = xmlDoc.CreateElement("root");
            //xmlDoc.AppendChild(root);

            var response = new ResponseModel
            {
                Response = new ResponseTest
                {
                    Head = new HeadTest
                    {
                        Return = new ReturnTest
                        {
                            Source = "1",
                            RecordFlow = "1432089583748759900012",
                            TransNo = "HTIP.CM.PAT.0001",
                            RetCode = 1,
                            RetType = "1",
                            RetCont = "成功时为空，失败是说明失败原因"
                        },
                        Test = "1"
                    },
                    Body = new BodyTest
                    {
                        PatientInfo = new PatientInfoModel
                        {
                            Name = "1"
                        }
                    },
                    Msg = "1"
                }
            };

            Type type = response.GetType();

            if(!IsBaseType(type))
            {
                var propList = type.GetProperties().ToList();
                foreach (var prop in propList)
                {
                    //创建根结点
                    XmlNode node = xmlDoc.CreateElement(prop.Name);

                    GetChildNodes(prop, xmlDoc, node);

                    xmlDoc.AppendChild(node);
                }
            }
            Console.WriteLine(xmlDoc.ToString());
            Console.ReadKey();

            #region history
            //string[] jsonArray = new string[]
            //{
            //    "科室信息",
            //    "职工信息",
            //    "企业单位字典",

            //};
            //string str = "1,262.00";

            //DateTime bjrq = Convert.ToDateTime("2022-7-28 19:40");
            //DateTime startX = Convert.ToDateTime(DateTime.Now.AddMinutes(11).ToString("yyyy-MM-dd"));
            //DateTime endX = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
            //TimeSpan span = startX.Subtract(endX);
            //int dayDiff = span.Days;//相差天数

            //List<string> keQxList = new List<string> { "1", "2", "3" };

            //List<string> bukeQxList = new List<string> { "11", "12", "13" }.Except(keQxList).ToList();

            //Console.WriteLine($"{dayDiff}"); 
            #endregion

            #region xml格式
            //DataSet ds = new DataSet();
            //XmlNode xmlNode1 = null;
            //var xd = new XmlDataDocument();
            //StringBuilder sb;
            //Hashtable ht = new Hashtable();
            ////ht.Add("theIpAddress", "115.195.145.149");
            ////xmlNode1 = Plugins.WebServiceCaller.QuerySoapWebService("http://www.webxml.com.cn/WebServices/IpAddressSearchWebService.asmx", "getCountryCityByIp", ht);

            ////ht.Add("theIpAddress", "115.195.145.149");
            ////xmlNode1 = Plugins.WebServiceCaller.QueryPostFormUrlWebService("http://www.webxml.com.cn/WebServices/IpAddressSearchWebService.asmx", "getCountryCityByIp", ht);
            //ht.Add("daoQiBZ",1);
            //ht.Add("yiLiaoJG", "800984424215744512");
            //ht.Add("zhuangTai", null);

            //string str = WebServiceCaller.QueryPostJsonWebService("http://localhost:20000/mediinfo-hrp-renlizy-renyuangl/api/v1/ShiXiDJ", "export", ht);

            //Console.WriteLine(str);

            ////ht.Add("theCityName", "九江");
            ////string url = "http://www.webxml.com.cn/WebServices/WeatherWebService.asmx";
            ////string method = "getWeatherbyCityName";
            ////xmlNode1 = Plugins.WebServiceCaller.QuerySoapWebService(url, method, ht);
            //if (xmlNode1 == null)
            //{
            //    return;
            //}
            //string xmlstr = HttpUtility.HtmlDecode(xmlNode1.OuterXml);
            //sb = new StringBuilder(xmlstr);
            //if (sb.ToString().Equals(""))
            //{
            //    return;
            //}
            //xd.LoadXml(sb.ToString());
            //ds.ReadXml(new XmlNodeReader(xd));
            ////ds可以返回出结果集

            //foreach (DataRow row in ds.Tables[0].Rows)
            //{
            //    Console.WriteLine(row[0]);
            //}
            #endregion

            #region 可用版本
            //DataSet ds = new DataSet();
            //XmlNode xmlNode1;
            //XmlDataDocument xd = new XmlDataDocument();
            //StringBuilder sb;
            //Hashtable ht = new Hashtable();
            //ht.Add("theIpAddress", "115.195.145.149");
            //xmlNode1 = Plugins.WebServiceCaller.QueryPostWebService("http://www.webxml.com.cn/WebServices/IpAddressSearchWebService.asmx", "getCountryCityByIp", ht);
            //if (xmlNode1 == null)
            //{
            //    return;
            //}
            //string xmlstr = HttpUtility.HtmlDecode(xmlNode1.OuterXml);
            //sb = new StringBuilder(xmlstr);
            //if (sb.ToString().Equals(""))
            //{
            //    return;
            //}
            //xd.LoadXml(sb.ToString());
            //ds.ReadXml(new XmlNodeReader(xd)); 
            #endregion

            #region dict
            //Dictionary<string, object> dic = new Dictionary<string, object>();
            //dic.TryAdd("1", "2");
            //dic.TryAdd("1", "3");
            //foreach (var item in dic)
            //{
            //    Console.WriteLine(item.Key + ":" + item.Value);
            //}


            //string str = @"%7B%22%E5%B7%A5%E4%BD%9C%E4%BF%A1%E6%81%AF%22:%221%22,%22%E6%89%A7%E4%B8%9A%E4%BF%A1%E6%81%AF%22:%222%22,%22%E5%90%88%E5%90%8C%E4%BF%A1%E6%81%AF%22:%223%22,%22%E6%8A%A4%E7%90%86%E4%BF%A1%E6%81%AF%22:%224%22,%22%E8%81%8C%E7%A7%B0%E5%8F%98%E5%8A%A8%22:%225%22,%22%E8%81%8C%E5%8A%A1%E5%8F%98%E5%8A%A8%22:%226%22,%22%E8%81%8C%E7%BA%A7%E5%8F%98%E5%8A%A8%22:%227%22,%22%E6%95%99%E8%82%B2%E7%BB%8F%E5%8E%86%22:%228%22,%22%E5%B7%A5%E4%BD%9C%E7%BB%8F%E5%8E%86%22:%229%22,%22%E5%B7%A5%E9%BE%84%E5%8F%98%E5%8A%A8%22:%2210%22,%22%E5%AE%B6%E5%BA%AD%E6%88%90%E5%91%98%22:%2211%22,%22%E5%B7%A5%E4%BA%BA%E7%AD%89%E7%BA%A7%E5%8F%98%E5%8A%A8%22:%2212%22,%22%E6%B5%8B%E8%AF%95%E7%BB%84%22:%2213%22,%22zhengShuDA%22:%2214%22%7D";
            //string result = HttpUtility.UrlDecode(str);
            //var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(result); 
            #endregion

            #region httpclient-get
            //var client = new HttpClient();
            //var request = new HttpRequestMessage
            //{
            //    Method = HttpMethod.Get,
            //    RequestUri = new Uri("http://172.19.80.10:30003/mcrp-peizhi-zidian/api/v1/xingzhengqhbm"),
            //};
            //using (var response = client.SendAsync(request).GetAwaiter().GetResult())
            //{
            //    response.EnsureSuccessStatusCode();
            //    var body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            //    Console.WriteLine(body);
            //}
            //Console.WriteLine(result); 
            #endregion

            #region UrlEncode
            //while (true)
            //{
            //    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //    string s = System.Web.HttpUtility.UrlEncode("中国1", Encoding.GetEncoding("GBK"));
            //    Console.WriteLine(s);
            //} 
            #endregion
        }

        static void GetChildNodes(PropertyInfo prop, XmlDocument xmlDoc, XmlNode node)
        {
            if(prop == null || xmlDoc == null || node == null)
            {
                return;
            }
            Type type = prop.PropertyType;
            var propList = type.GetProperties().ToList();
            foreach (var childProp in propList)
            {
                Type childType = childProp.PropertyType;
                if (!IsBaseType(childType))
                {
                    //创建父结点
                    XmlNode parentNode = xmlDoc.CreateElement(childProp.Name);
                    GetChildNodes(childProp, xmlDoc, parentNode);
                    node.AppendChild(parentNode);
                }
                else
                {
                    //叶子结点
                    XmlNode childNode = xmlDoc.CreateNode(XmlNodeType.Element, childProp.Name, string.Empty);
                    childNode.InnerText = "1";
                    node.AppendChild(childNode);
                }
            }
        }

        private static Person person = new Person
        {
            Name = "Abe Lincoln",
            Age = 25,
            HeightInches = 6f + 4f + 12f,
            Addresses = new Dictionary<string, Address>
            {
                {
                    "home",
                    new Address()
                    {
                        Street = "2720  Sundown Lane",
                        City = "Kentucketsville",
                        State = "Calousiyorkida"
                    }
                },
                {
                    "work",
                    new Address()
                    {
                        Street = "1600 Pennsylvania Avenue NW",
                        City = "Washington",
                        State = "District of Columbia",
                        Zip = "20500"
                    }
                }
            }
        };

        private readonly Regex _regex = new Regex(@"([i][f])|([e][l][s][e])|([t][h][e][n])|([e][n][d])");
        static void Main2(string[] args)
        {
            #region 测试
            List<SalaryItem> expressions = new List<SalaryItem>()
            {
//                new SalaryItem()
//                {
//                    ItemId= "01",
//                    ItemName = "基本工资",
//                    expression = "5000"
//                },
//                new SalaryItem()
//                {
//                    ItemId= "02",
//                    ItemName = "岗位工资",
//                    expression = "[薪酬标准].[岗位工资]"
//                },
//                new SalaryItem()
//                {
//                    ItemId= "03",
//                    ItemName = "薪级工资",
//                    expression = "[薪酬标准].[薪级工资]"
//                },
//                new SalaryItem()
//                {
//                    ItemId= "04",
//                    ItemName = "护理10%补贴",
//                    expression = @"if [职工信息].[职工类别] == 护士类别 then
//  ([薪酬项目].[岗位工资] + [薪酬项目].[薪级工资])*0.1
//else
//  0"
//                },
                new SalaryItem()
                {
                    ItemId= "05",
                    ItemName = "绩效工资",
                    expression = "((50*3 + 100)+50)*3"
                }
                ,
                //new SalaryItem()
                //{
                //    ItemId= "06",
                //    ItemName = "工龄补贴",
                //    expression = "(if [职工信息].[合同工龄] > 26 then 26 else [职工信息].[合同工龄])*100"
                //}
            };
            //expressions.Select(o => SalaryFormula.CreateFormula(o.expression)).ToList().ForEach(salary =>
            //{
            //    //Console.WriteLine(salary.Compulete());
            //    var stack = salary.GetQueue();
            //    if (stack.Count() > 0)
            //    {
            //        stack.ToList().ForEach(s =>
            //        {
            //            string result = MathUtilHelper.calculatingSuffix(s);
            //            Console.WriteLine(result?.Trim());
            //        });
            //    }
            //});
            //string str = "((3+2)*2+108)/5";
            //string expressioneee = @"if  {xingBie}=='1'";
            //expressioneee = expressioneee.TrimEnd(';').Replace("{xingBie}", "1").Replace("{", " ").Replace("}", " ").Replace("'", "/\"");


            //Console.WriteLine("请输入表达式(按q提出):");
            //string strIn = Console.ReadLine();
            //while (strIn != "q")
            //{
            //    string oldstr = "a=aXc\n\r\n vee;a= \n\r\n . \n\r\n ;";
            //    List<string> newstrList = MidStrEx_New(oldstr, "a=", ";");
            //    foreach (string str in newstrList)
            //    {
            //        string dealStr = str.Trim();
            //        int index = oldstr.IndexOf(str);
            //        if(dealStr == ".")
            //        {
            //            oldstr = oldstr.Remove(index, str.Length);
            //            Console.WriteLine(oldstr);
            //        }
            //        //string aEqual = oldstr.Substring(index - 2, 2);
            //        //Console.WriteLine(index);
            //        //Console.WriteLine(str);
            //    }
            //    Console.WriteLine("请输入表达式(按q提出):");
            //    strIn = Console.ReadLine();
            //}


            //int[] sub1 = GetSubStrCountInStr("12332133544", "a=", 0);

            //foreach (var item in sub1)
            //{
            //    Console.WriteLine(item);
            //}

            //string str = string.Empty;
            //GetCount();
            //int[] sub1 = GetSubStrCountInStr("12332133544","33",0);
            //foreach (var item in sub1)
            //{
            //    Console.WriteLine(item);
            //}
            //int[] sub2 = GetSubStrCountInStr("12332133544", "44", 0);

            //Console.WriteLine("请输入表达式(按q提出):");
            //str = Console.ReadLine();
            //while (str != "q")
            //{
            //    //decimal result = new MixedOperation().GetMixedOperationRes(str);
            //    //Console.WriteLine("结果：" + result);

            //    //new LuoJiYSFOperation().PretreatmentTest(str);
            //    //bool result = new LuoJiYSFOperation().GetMixedOperationRes(str);
            //    //Console.WriteLine("结果：" + result);
            //    //Console.WriteLine(1 > 2 && 2 < 3);
            //    //new CompareOperation().PretreatmentTest(str);
            //    new GongShiOperate().Calculate(str);
            //    Console.WriteLine("请输入表达式(按q提出):");
            //    str = Console.ReadLine();
            //}
            //Console.WriteLine("您已退出!");
            //Console.ReadKey(); 
            #endregion

            //string canShuZ = "rxRQ8MHJKd6OmekLNB1cJoUPMgqo2JvS3e1ZrnhLeEhfBYkeObyI3VGQodJI1tLg";
            //string secretKey = "db1d1971ed5a49febda715db39ffe92b";
            //string ticket = canShuZ.Replace(" ", "+");
            //var res = AESProvider.Decrypt(Convert.FromBase64String(ticket), secretKey.Substring(0, 16));
            //var result = Encoding.UTF8.GetString(res);
            //Console.WriteLine(result);

            #region 1. 组连接 group by

            var groupQuery = from publisher in SampleData.Publishers
                             join book in SampleData.Books
                             on publisher.Name equals book.Publisher.Name into publisherBooks
                             select new
                             {
                                 PublisherName = publisher.Name,
                                 Books = publisherBooks
                             };

            //与上面等同的GroupBy语句
            //使用 group   by
            var groupQuery2 = from book in SampleData.Books
                              group book by book.Publisher into grouping
                              select new
                              {
                                  PublisherName = grouping.Key.Name,
                                  Books = grouping
                              };

            #endregion

            #region 2. 内连接，相当于Sql语句中的inner join，即找出两个序列的交集

            var joinQuery = from publisher in SampleData.Publishers
                            join book in SampleData.Books
                            on publisher equals book.Publisher
                            select new
                            {
                                PublisherName = publisher.Name,
                                BookName = book.Title
                            };
            //foreach (var item in joinQuery)
            //{
            //    Console.WriteLine(item.PublisherName);
            //    Console.WriteLine(item.BookName);
            //}

            //与上边等同的查询操作符
            var joinQuery2 = SampleData.Publishers.Join(
                SampleData.Books,         //join对象

                publisher => publisher,   //外部的key

                book => book.Publisher,   //内部的key

                (publisher, book) => new   //结果
                {
                    PublisherName = publisher.Name,
                    BookName = book.Title
                }
            );

            //foreach (var item in joinQuery2)
            //{
            //    Console.WriteLine(item.PublisherName);
            //    Console.WriteLine(item.BookName);
            //}

            //Console.WriteLine();

            #endregion

            #region 3. 左外连接，与Sql中的left join 一样

            //left join,为空时用default

            //注：上例中使用了DefaultIfEmpty操作符，它能够为实序列提供一个默认的元素。
            //DefaultIfEmpty使用了泛型中的default关键字。
            //default关键字对于引用类型将返回null,而对于值类型则返回0。
            //对于结构体类型，则会根据其成员类型将它们相应地初始化为null(引用类型)或0(值类型)
            var leftJoinQueryByDefault = from publisher in SampleData.Publishers
                                         join book in SampleData.Books
                                         on publisher equals book.Publisher into publisherBooks
                                         from book in publisherBooks.DefaultIfEmpty()
                                         select new
                                         {
                                             PublisherName = publisher.Name,
                                             BookName = (book == default(Book)) ? "no book" : book.Title
                                         };

            //foreach (var item in leftJoinQueryByDefault)
            //{
            //    Console.WriteLine(item.PublisherName);
            //    Console.WriteLine(item.BookName);
            //}

            //不使用default 关键字，但要在DefaultEmpty中给定出为空时的默认对象值

            //left join 为空时使用默认对象
            var leftJoinQuery2 = from publisher in SampleData.Publishers
                                 join book in SampleData.Books
                                 on publisher equals book.Publisher into publisherBooks
                                 from book in publisherBooks.DefaultIfEmpty(new Book
                                 {
                                     Title = ""
                                 })
                                 select new
                                 {
                                     PublisherName = publisher.Name,
                                     BookName = book.Title
                                 };

            //foreach (var item in leftJoinQuery2)
            //{
            //    Console.WriteLine(item.PublisherName + "," + item.BookName);
            //}

            #endregion


            #region 4. 交叉连接，与Sql中Cross join一样

            //交叉连接查询语句
            var crossJoinQuery = from publisher in SampleData.Publishers
                                 from book in SampleData.Books
                                 select new
                                 {
                                     PublisherName = publisher.Name,
                                     BookName = book.Title
                                 };
            //foreach (var item in crossJoinQuery)
            //{
            //    Console.WriteLine(item.PublisherName + "," + item.BookName);
            //}


            //查询操作符语句，不使用查询表达式
            var crossJoinQuery2 = SampleData.Publishers.SelectMany(publisher =>
                SampleData.Books.Select(book => new
                {
                    PublisherName = publisher.Name,
                    BookName = book.Title
                })
            );

            foreach (var item in crossJoinQuery2)
            {
                Console.WriteLine(item.PublisherName + "," + item.BookName);
            }

            #endregion


            #region object序列化成yaml

            var serializer = new SerializerBuilder()
                             .WithNamingConvention(CamelCaseNamingConvention.Instance)
                             .Build();
            string yaml = serializer.Serialize(person);

            Console.WriteLine(yaml);

            #endregion

            #region 反序列化

            var deserializer = new DeserializerBuilder()
                               .WithNamingConvention(CamelCaseNamingConvention.Instance)
                               .Build();

            var p = deserializer.Deserialize<Person>(yaml);

            //string addresses = System.Text.Json.JsonSerializer.Serialize(p.Addresses);
            //Console.WriteLine(addresses);

            #endregion

            #region yaml转成json格式

            var deserializerJson = new DeserializerBuilder().Build();

            var yamlObject = deserializerJson.Deserialize(new StringReader(yaml));

            var serializerJson = new SerializerBuilder()
                                    .JsonCompatible()
                                    .Build();

            var json = serializerJson.Serialize(yamlObject);

            Console.WriteLine(json);


            #endregion

            string ip = GetIpAddress();
            Console.WriteLine($"ip={ip}");
            Console.ReadKey();
        }

        public static string GetIpAddress()
        {
            try
            {
                string hostName = Dns.GetHostName();
                IPHostEntry iPHostEntry = Dns.GetHostEntry(hostName);
                var addressV = iPHostEntry.AddressList.FirstOrDefault(q => q.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);//ipv4地址
                if(addressV != null)
                {
                    return addressV.ToString();
                }
            }
            catch
            {
                return "127.0.0.1";
            }
            return "127.0.0.1";
        }

        public static List<string> MidStrEx_New(string sourse, string startstr, string endstr)
        {
            Regex rg = new Regex("(?<=(" + startstr + "))[.\\s\\S]*?(?=(" + endstr + "))");
            List<string> result = new List<string>();
            MatchCollection matches = rg.Matches(sourse);
            foreach (Match item in matches)
            {
                result.Add(item.Groups[0]?.Value);
            }
            return result;
        }

        static void GetCount(string str = "123321")
        {
            Dictionary<string, int> count = new Dictionary<string, int>();

            List<int> temp_index = str.Select((item, index) => new { item, index }).Where(t => t.item == '3').Select(t => t.index).ToList();

            foreach (int item in temp_index)
            {
                Console.WriteLine(item);
            }
        }

        static int[] GetSubStrCountInStr(string str, String substr, int StartPos)
        {
            int foundPos = -1;
            int count = 0;
            List<int> foundItems = new List<int>();
            do
            {
                foundPos = str.IndexOf(substr, StartPos);
                if (foundPos > -1)
                {
                    StartPos = foundPos + 1;
                    count++;
                    foundItems.Add(foundPos);
                }
            } while (foundPos > -1 && StartPos < str.Length);

            return ((int[])foundItems.ToArray());
        }

        public class AESProvider
        {
            public static byte[] Encrypt(byte[] toEncrypt, string key)
            {
                byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);

                RijndaelManaged rDel = new RijndaelManaged();
                rDel.Key = keyArray;
                rDel.Mode = CipherMode.ECB;
                rDel.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = rDel.CreateEncryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncrypt, 0, toEncrypt.Length);

                return resultArray;
            }

            public static string EncryptToMysql(string toEncrypt, string key)
            {
                byte[] keyByte = UTF8Encoding.UTF8.GetBytes(key);
                byte[] keyArray = new byte[16];
                keyByte.CopyTo(keyArray, 0);

                byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

                var resultArray = Encrypt(toEncryptArray, keyArray);

                var result = Convert.ToBase64String(resultArray, Base64FormattingOptions.None);
                return result;
            }

            public static byte[] Encrypt(byte[] toEncrypt, byte[] keyArray)
            {
                RijndaelManaged rDel = new RijndaelManaged();
                rDel.Key = keyArray;
                rDel.Mode = CipherMode.ECB;
                rDel.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = rDel.CreateEncryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncrypt, 0, toEncrypt.Length);

                return resultArray;
            }

            public static byte[] Decrypt(byte[] toDecrypt, string key)
            {
                byte[] keyArray = Encoding.UTF8.GetBytes(key);
                RijndaelManaged rDel = new RijndaelManaged();
                rDel.Key = keyArray;
                rDel.Mode = CipherMode.ECB;
                rDel.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = rDel.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toDecrypt, 0, toDecrypt.Length);

                return resultArray;
            }

            public static byte[] Decrypt(byte[] toDecrypt, byte[] keyArray)
            {
                RijndaelManaged rDel = new RijndaelManaged();
                rDel.Key = keyArray;
                rDel.Mode = CipherMode.ECB;
                rDel.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = rDel.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toDecrypt, 0, toDecrypt.Length);

                return resultArray;
            }

            /// <summary>
            /// AES解密
            /// </summary>
            /// <param name="toDecrypt">16进制字符串</param>
            /// <param name="key">密钥</param>
            /// <param name="iv">偏移量</param>
            /// <returns></returns>
            public static string Decrypt(string toDecrypt, string key, string iv)
            {
                toDecrypt = toDecrypt.Replace(" ", "");
                byte[] buffer = new byte[toDecrypt.Length / 2];
                for (int i = 0; i < toDecrypt.Length; i += 2)
                {
                    buffer[i / 2] = (byte)Convert.ToByte(toDecrypt.Substring(i, 2), 16);
                }
                using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
                {
                    aesAlg.Key = Encoding.UTF8.GetBytes(key.Substring(0, 32));
                    aesAlg.IV = Encoding.UTF8.GetBytes(iv.Substring(0, 16));

                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    using (MemoryStream msEncrypt = new MemoryStream(buffer))
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srEncrypt = new StreamReader(csEncrypt))
                            {
                                return srEncrypt.ReadToEnd();
                            }
                        }
                    }
                }
            }
        }

        class SalaryItem
        {
            public string ItemId { get; set; }
            public string ItemName { get; set; }
            public string expression { get; set; }
        }

        enum ScaleType
        {
            Post = 1,
            Scale = 2
        }

        class SalaryScale
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public ScaleType Type { get; set; }
            public decimal Salary { get; set; }

            public static List<SalaryScale> SalaryScales { get; private set; }

            static SalaryScale()
            {
                SalaryScales = new List<SalaryScale>()
                {
                    new SalaryScale(){ Id="01", Name="管理一级", Type=ScaleType.Post, Salary=8000 },
                    new SalaryScale(){ Id="02", Name="管理二级", Type=ScaleType.Post, Salary=7000 },
                    new SalaryScale(){ Id="03", Name="管理三级", Type=ScaleType.Post, Salary=6000 },
                    new SalaryScale(){ Id="04", Name="管理四级", Type=ScaleType.Post, Salary=5000 },
                    new SalaryScale(){ Id="11", Name="1级", Type=ScaleType.Post, Salary=2000 },
                    new SalaryScale(){ Id="12", Name="2级", Type=ScaleType.Post, Salary=3000 },
                    new SalaryScale(){ Id="13", Name="3级", Type=ScaleType.Post, Salary=4000 },
                    new SalaryScale(){ Id="14", Name="4级", Type=ScaleType.Post, Salary=5000 },
                };
            }
        }
    }
}

