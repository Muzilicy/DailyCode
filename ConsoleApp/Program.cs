using ConsoleApp.Formulas;
using ConsoleApp.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.IO;
using ConsoleApp.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.Net;
using System.Data;
using System.Xml;
using System.Collections;
using System.Web;
using RestSharp;
using ConsoleApp.Plugins;

namespace ConsoleApp
{
    #region class
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
        static void Main(string[] args)
        {
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
            DataSet ds = new DataSet();
            XmlNode xmlNode1 = null;
            var xd = new XmlDataDocument();
            StringBuilder sb;
            Hashtable ht = new Hashtable();
            //ht.Add("theIpAddress", "115.195.145.149");
            //xmlNode1 = Plugins.WebServiceCaller.QuerySoapWebService("http://www.webxml.com.cn/WebServices/IpAddressSearchWebService.asmx", "getCountryCityByIp", ht);

            //ht.Add("theIpAddress", "115.195.145.149");
            //xmlNode1 = Plugins.WebServiceCaller.QueryPostFormUrlWebService("http://www.webxml.com.cn/WebServices/IpAddressSearchWebService.asmx", "getCountryCityByIp", ht);
            ht.Add("daoQiBZ",1);
            ht.Add("yiLiaoJG", "800984424215744512");
            ht.Add("zhuangTai", null);

            string str = WebServiceCaller.QueryPostJsonWebService("http://localhost:20000/mediinfo-hrp-renlizy-renyuangl/api/v1/ShiXiDJ", "export", ht);

            Console.WriteLine(str);

            //ht.Add("theCityName", "九江");
            //string url = "http://www.webxml.com.cn/WebServices/WeatherWebService.asmx";
            //string method = "getWeatherbyCityName";
            //xmlNode1 = Plugins.WebServiceCaller.QuerySoapWebService(url, method, ht);
            if (xmlNode1 == null)
            {
                return;
            }
            string xmlstr = HttpUtility.HtmlDecode(xmlNode1.OuterXml);
            sb = new StringBuilder(xmlstr);
            if (sb.ToString().Equals(""))
            {
                return;
            }
            xd.LoadXml(sb.ToString());
            ds.ReadXml(new XmlNodeReader(xd));
            //ds可以返回出结果集

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                Console.WriteLine(row[0]);
            }
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

            Console.Read();
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

