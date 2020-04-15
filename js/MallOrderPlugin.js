var backFunction = null;
(function ($) {
    var MallOrder = function (ele, opt) {
        this.$element = ele;
            this.defaults = {
                dialogId: 'ddMallOrder',
                resultInput: this.$element.attr("InputID"),
            },
            this.options = $.extend({}, this.defaults, opt);
        backFunction = opt.backFunction; //回调函数
    }
    MallOrder.prototype = {
       DailogShow: function () {
            var self = this;
            var flag = 0;
            this.$element.bind("click", function () {
                if (flag == 0) {
                    $("#" + self.options.dialogId).html("<iframe src='/Sale/MallOrderPlugin/OrderIndex/?resultInput=" + self.options.resultInput + "' name='mallOrder_frame' id='mallOrder_frame' key='123' style='width: 100%; height: 100%;' frameborder='0' style='overflow:hidden'></iframe>");
                }
                flag++;
                $("#" + self.options.dialogId).dialog({
                    title: '筛选订单',
                    iconCls: 'icon-search',
                    onOpen: function () {

                        var dg1 = $("iframe[name='mallOrder_frame']")[0].contentWindow;
                        if (dg1) {

                            var orders = $("#" + self.options.resultInput).val();

                        }
                    }
                }).dialog('open').dialog('center');
            });
        }
    }
    $.fn.initMall = function (options) {

        var orderDailog = new MallOrder(this, options);

        $("#searchForMallOrder").unbind();
        return orderDailog.DailogShow();

    }

})(jQuery);

$(function () {
    $("#searchForMallOrder").initMall({
        ValChangFunc: function () {
            var $txt = $("#" + this.resultInput);
        }
    })
})


///关闭弹框
function CloseDialog(id) {

    $("#" + id).dialog('close');

    if (backFunction) {
        backFunction(); //调用回调函数
    }
}
