using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Common;
using System.Text.RegularExpressions;

namespace System.Web.Mvc
{
    public static class HtmlExtend
    {
        /// <summary>
        /// 分销商控件插件
        /// </summary>
        /// <param name="html"></param>
        /// <param name="mutil">是否多选（默认单选）</param>
        /// <param name="inputId">文本框id（默认值为agents）</param>
        /// <param name="divId">弹出框ID （多选需要填写，默认值为ddAgent）</param>
        /// <param name="serchid">搜索按钮ID（多选需要填写，默认为search）</param>
        /// <param name="height">弹出框高度</param>
        /// <param name="initAgentID">初始化分销商id 默认为空</param>
        /// <param name="initAgentName">初始化分销商Name 默认为空</param>
        /// <returns></returns>
        public static MvcHtmlString AgentPlugin(this HtmlHelper html, bool mutil = false, string inputId = "agents", string initAgentID = "", string initAgentName = "", int height = 450, int width = 141)
        {

            // validatebox-text textbox-prompt validatebox-invalid
            string txt = @"<span class='textbox' style='width: " + (width) +
                "px; '><input class='textbox-text txt_SelectAgent' IsMutil=" + mutil + " type='text' autocomplete='off' placeholder='' style='margin-left: 0px; margin-right: 0px; padding-top: 2px; height: 20px;line-height: 20px;padding-bottom: 2px; width: " + width + "px;' value='" + initAgentName + "' id='" + inputId + "' agentids='" + initAgentID + "' agentnames='" + initAgentName + "'><input type='hidden' value='" + initAgentID + "' name='" + inputId + "' /></span>";

            string a = @"<span class='textbox' style='width: " + (width + 8) + "px;'><span class='textbox-addon textbox-addon-right' style='right: 2px;'>"
                   + "<a href='#'  class='textbox-icon icon-search btn_AgentDailog' icon-index='0' tabindex='-1'"
                   + "style='width: 15px; ' DailogID=" + inputId + "ddAgent InputID=" + inputId + " id='" + inputId + "search'></a></span><input type='text' IsMutil=" + mutil + " btnserchid='" + inputId + "search' class='textbox-text txt_SelectAgent ' style='margin: 0px;padding-top: 0px;padding-bottom: 0px;height: 24px;line-height: 24px;width: 140px;' value='" + initAgentName + "'  id='" + inputId + "'  agentids='" + initAgentID + "' agentnames='" + initAgentName + "' /> <input type='hidden' value='" + initAgentID + "' name='" + inputId + "' /> </span>";


            string dialog =
                " <div id='" + inputId + "ddAgent' class='easyui-dialog' title='分销商筛选' style='width: 640px; height: " + height + "px;' data-options='iconCls:\"icon-search\",resizable:true,modal:true,closed:true'>";
            if (mutil)
                return MvcHtmlString.Create(a + dialog);
            else
                return MvcHtmlString.Create(txt);
        }

        /// <summary>
        /// 会员系统积分商城订单插件
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <returns></returns>
        public static MvcHtmlString MemberMallOrderPlugin(this HtmlHelper htmlHelper,string InputID = "searchMallOrders",string InputName = "MallOrders",int Width = 1020,int Height = 500)
        {
            string txt = @"<span class='textbox' style='width: 141px;'><span class='textbox-addon textbox-addon-right' style='right: 2px;'>"
          + "<a href='#'  class='textbox-icon icon-search' id='searchForMallOrder' InputID='" + InputID + "' icon-index='0' tabindex='-1'" + "style='width: 15px; height: 20px;'></a></span><input type='text' class='textbox-text' name='" + InputName + "' value='' style='padding:0;margin:0;height: 24px;line-height: 24px;width: 140px;' id='" + InputID + "'/></span>";

            string dialog =
                " <div id='ddMallOrder' class='easyui-dialog' title='订单筛选' style='overflow: hidden;width: " + Width + "px; height:" + Height + "px;' data-options='iconCls:\"icon-search\",resizable:true,modal:true,maximizable:true,closed:true'>";

            return MvcHtmlString.Create(txt + dialog);
        }
        /// <summary>
        /// 给JS或Css文件加上版本号
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="Js"></param>
        /// <returns></returns>
        public static MvcHtmlString JsCssRander(this HtmlHelper htmlHelper, string JsCss)
        {  //js版本号
            string v = ConfigHelper.GetConfigString("JSVersion");
            JsCss = JsCss.Replace(".js", ".js?v=" + v).Replace(".css", ".css?v=" + v);
            return MvcHtmlString.Create(JsCss);
        }



        #region 根据数据层的操作  来返回不同的json数据 - JsonResult creatJavaScriptResult(int result, int modelCount,string msg)
        /// <summary>
        /// 根据数据层的操作  来返回不同的json数据
        /// </summary>
        /// <param name="result">数据库返回的数据 受影响行数或者被插入数据的id</param>
        /// <param name="modelCount">待处理的数据条数</param>
        /// <param name="msg">弹框显示信息</param>
        /// <param name="opraterType">0 insert 1 update 2 delete 3 后台模型校验不通过 ,4 登录超时</param>
        /// <param name="data">后台josn 数据返回</param>
        ///  <param name="backUrl">前台处理完成后跳转页面</param>
        /// <returns></returns>
        public static JsonResult CreatJavaScriptResult(int result, int modelCount, string msg, int opraterType, string data, string backUrl)
        {
            AjaxMsgModel ajax = new AjaxMsgModel()
            {
                Statu = CommonHelp.EnumHelper.Results.失败.ToString(),
                Msg = "系统错误",
                Data = data,
                BackUrl = backUrl
            };
            if (opraterType == 0)
            {
                if (result > modelCount)
                {
                    ajax = new AjaxMsgModel()
                    {
                        Statu = CommonHelp.EnumHelper.Results.成功.ToString(),
                        Msg = msg,
                        Data = data,
                        BackUrl = backUrl
                    };
                }
            }
            else if (opraterType == 3)
            {
                ajax = new AjaxMsgModel()
                {
                    Statu = "noValid",
                    Msg = msg,
                    Data = data,
                    BackUrl = backUrl
                };
            }
            else if (opraterType == 4)
            {
                ajax = new AjaxMsgModel()
                {
                    Statu = "nologin",
                    Msg = msg,
                    Data = data,
                    BackUrl = backUrl
                };
            }
            else
            {
                if (result == modelCount)
                {
                    ajax = new AjaxMsgModel()
                    {
                        Statu = CommonHelp.EnumHelper.Results.成功.ToString(),
                        Msg = msg,
                        Data = data,
                        BackUrl = backUrl
                    };
                }
            }
            JsonResult res = new JsonResult();
            res.Data = ajax;
            // res.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            return res;
        }
        #endregion

        /// <summary>
        /// 根据英文符截取字符串
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="str"></param>
        /// <param name="strartIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static MvcHtmlString SubStringByEN(this HtmlHelper htmlHelper, string str, int strartIndex, int length)
        {
            string temp = str;
            int j = 0;
            int k = 0;
            for (int i = strartIndex; i < temp.Length; i++)
            {
                if (Regex.IsMatch(temp.Substring(i, 1), @"[\u4e00-\u9fa5]+"))
                    j += 2;
                else
                    j += 1;
                if (j <= length)
                    k += 1;
                if (j > length)
                    return MvcHtmlString.Create(temp.Substring(strartIndex, k) + "..");
            }
            return MvcHtmlString.Create(temp);
        }

        public class AjaxMsgModel
        {
            public string Msg { get; set; }
            public string Statu { get; set; }//
            public string BackUrl { get; set; }
            public object Data { get; set; }//数据对象
        }


    }
}
