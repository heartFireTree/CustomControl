using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using System.Web.Mvc
using Newtonsoft.Json;

namespace JDF.ERP.Sale.Control
{
    public class MallOrderPluginController : BaseController
    {
        public ActionResult OrderIndex()
        {
            //string resultInput = Request["resultInput"].ToString();
            //ViewBag.resultInput = resultInput;
            return PartialView("Index");
        }

        public ActionResult Search(SEExOrder model)
        {
            MemberResult mbr = BAdminUser.GetInstance().GetMallOrderList(model);

            return Json(mbr.Data);
        }
    }
    public class SEExOrder : BaseModel
    {
        [IsPrimaryKey]
        public int ExOrderID { get; set; }
        [DisplayName("订单号")]
        public string ExOrderNo { get; set; }
        [DisplayName("下单时间")]
        public DateTime? AddTime { get; set; }
        [DisplayName("下单用户")]
        public int? AddVIPID { get; set; }
        [DisplayName("取货方式")]
        public string GetWay { get; set; }
        [DisplayName("收货地址")]
        public string GetAddress { get; set; }
    }
    
     public class MemberResult
    {
        /// <summary>
        /// 0：表示成功 -1:错误 
        /// </summary>
        [JsonProperty("code")]
        public int Code { get; set; }
        /// <summary>
        /// 返回信息 返回信息
        /// </summary>
        [JsonProperty("msg")]
        public string Msg { get; set; }
        /// <summary>
        /// 返回数据
        /// </summary>
        [JsonProperty("data")]
        public object Data { get; set; }
        /// <summary>
        /// 返回页面
        /// </summary>
        [JsonProperty("back_url")]
        public string BackURL { get; set; }
    }
    
     public class BAdminUser : BaseBLL
     {
        /// <summary>
        /// 外部不准new
        /// </summary>
        public BAdminUser()
        {

        }
        private static volatile BAdminUser m_instance = null;
        
        /// 实现流程：
        ///     1.锁定类
        ///     2.获取对象实例 
        ///     3.结束 
        /// </remarks>
        /// <returns>返回一个已经存在的实体</returns>
        public static BAdminUser GetInstance()
        {
            // 通用的必要代码 iBatisNet双校检机制,如果实例不存在
            if (m_instance == null)
            {
                lock (typeof(BAdminUser))
                {
                    // 如果实例不存在
                    if (m_instance == null)
                        // 创建一个的实例
                        m_instance = new BAdminUser();
                }
            }
            // 返回业务逻辑对象
            return m_instance;
        }
        
        /// <summary>
        /// 获取积分商城订单数据
        /// </summary>
        /// <returns></returns>
        public MemberResult GetMallOrderList(SEExOrder model)
        {
            MemberResult ApiResul = new MemberResult { Code = 0 };
            
            string token = GetMemberToken();
            if (string.IsNullOrEmpty(token))
            {
                return ApiResul;
            }
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                string url = ConfigHelper.GetConfigString("JDFMemberURL").TrimEnd('/') + "/CRM/VIPMallOrder/Search";
                var dic = new Dictionary<string, string>();
                dic.Add("ExOrderNoLike", model.ExOrderNoLike);
                dic.Add("ExOrderGoodName", model.ExOrderGoodName);
                dic.Add("VipCusName", model.VipCusName);
                dic.Add("VIPNo", model.VIPNo);
                dic.Add("Mobile", model.Mobile);
                dic.Add("Page", model.Page.ToString());
                dic.Add("Rows", model.Rows.ToString());
                dic.Add("AddTime_From", model.AddTime_From.ToString());
                dic.Add("AddTime_To", model.AddTime_To.ToString());

                HttpContent httpContent = new FormUrlEncodedContent(dic);
                HttpResponseMessage httpResponseMessage = client.PostAsync(url, httpContent).Result;
                string callMsg = "";
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    callMsg = httpResponseMessage.Content.ReadAsStringAsync().Result;
                }
                if (string.IsNullOrWhiteSpace(callMsg))
                {
                    token = GetMemberToken(true);
                    if (string.IsNullOrEmpty(token))
                    {
                        return ApiResul;
                    }
                    client.DefaultRequestHeaders.Remove("Authorization");
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    httpResponseMessage = client.PostAsync(url, httpContent).Result;
                    if (httpResponseMessage.IsSuccessStatusCode)
                    {
                        callMsg = httpResponseMessage.Content.ReadAsStringAsync().Result;
                    }

                }
                if (string.IsNullOrWhiteSpace(callMsg))
                {
                    return ApiResul;
                }
                ApiResul = JsonConvert.DeserializeObject<MemberResult>(callMsg);
                if (ApiResul.Data != null)
                {
                    ApiResul = JsonConvert.DeserializeObject<MemberResult>(callMsg);
                }
            }
            return ApiResul;
        }
     }
}
