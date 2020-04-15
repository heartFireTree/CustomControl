OperateUrl = new function () {
    this.Search = '/Sale/MallOrderPlugin/Search';
    this.GridID = 'dg';
}
var columns = [];
var lastColumns = [];
var flag1 = 0;
var checkNO;

var dataSource = [];
$(function () {
    $('#dg').datagrid({
        idField: "ExOrderNo",
        fitColumns: false,
        rownumbers: true,
        singleSelect: true,
        pagination: true,
        pageSize: 20,
        pageList: [20, 50, 100, 200],
        fit: true,
        view: detailview,
        detailFormatter: function (index, row) {
            var newRows = _.filter(row.GoodList, function (m) {
                return m.ExOrderNo == row.ExOrderNo;
            })
            if (newRows.length == 0) return '';
            return '<div style="position:relative;padding-bottom:10px"><table class="ddv1"></table></div>';
        },
        onExpandRow: function (index, row) {
            var newRows = _.filter(row.GoodList, function (m) {
                return m.ExOrderNo == row.ExOrderNo;
            })
            if (newRows.length == 0) return '';

            var ddv = $(this).datagrid('getRowDetail', index).find('table.ddv1');
            ddv.datagrid({
                data: newRows,
                fitColumns: false,
                singleSelect: true,
                rownumbers: true,
                loadMsg: '',
                height: 'auto',
                columns: [[
                    {
                        field: 'SKUImageURI', title: '商品图片', align: 'center', width: 120, formatter: function (value) {
                            if (value != "") {
                                return '<a target="top" href="' + value + '"><div style="text-align:center;height:60px;"><img src="' + value + '" style="width:auto;height:60px;" /></div></a>';
                            }
                        }
                    },
                    { field: 'ExOrderGoodSaleNo', title: '商品规格NO', align: 'center' },
                    { field: 'ExOrderGoodSalePrice', title: '销售价',align: 'center' },
                    { field: 'MarketPrice', title: '市场价',  align: 'center' },
                    { field: 'ExOrderGoodNo', title: '商品NO', align: 'center' },
                    { field: 'SaleName', title: '商品规格名称', align: 'center' },
                    { field: 'ExOrderGoodID', title: '订单商品ID', align: 'center' },

                ]],
                onResize: function () {
                    $('#dg').datagrid('fixDetailRowHeight', index);
                },
                onLoadSuccess: function () {
                    setTimeout(function () {
                        $('#dg').datagrid('fixDetailRowHeight', index);
                    }, 0);
                },
                onDblClickRow: function (index, row) {
                    //不能夸单销售,所以选择一个后只能选择此单的商品,固定单号(eaysui 赋值写法)

                    if (checkNO) {
                        if (row.ExOrderNo != checkNO) {
                            $.messager.show({
                                title: "温馨提示",
                                msg: '无法进行夸单操作!',
                                showType: 'slide',
                                style: {
                                    right: '',
                                    top: document.body.scrollTop + document.documentElement.scrollTop,
                                    bottom: ''
                                }
                            });
                            return;
                        }
                    } else {
                        checkNO = row.ExOrderNo;
                    }
                  
                    $('#ExOrderNoLike').textbox('setValue', row.ExOrderNo);
                    
                    var callBack = window.parent.backFunction;
                    if (callBack) {
                        callBack(row);
                    }
                    parent.CloseDialog("ddMallOrder");
                    $("#" + OperateUrl.GridID).datagrid("unselectAll").datagrid("selectRecord", row.ExOrderNo);
                    $(ddv).datagrid("unselectAll").datagrid("selectRow", index);
                }
            })

            $('#dg').datagrid('fixDetailRowHeight', index);
        },

    });

    //初始化分页控件
    var pg = $("#dg").datagrid("getPager");
    if (pg) {
        $(pg).pagination({
            onBeforeRefresh: function () {
            },
            onRefresh: function (pageNumber, pageSize) {
            },
            onChangePageSize: function () {
            },
            onSelectPage: function (pageNumber, pageSize) {
                DoSearch();
            }
        });
    }
    DoSearch();
})



function DoSearch() {
    var options = $('#dg').datagrid('getPager').data("pagination").options; 
    var para = {
        Page: options.pageNumber > 0 ? options.pageNumber : 1,
        Rows: options.pageSize,
        ExOrderNoLike: $("#ExOrderNoLike").val(),
        ExOrderGoodName: $("#ExOrderGoodName").val(),
        VIPNo: $("#VIPNo").val(),
        Mobile: $("#Mobile").val(),
        VipCusName: $("#VipCusName").val(),
        AddTime_From: $("#starTime").val(),
        AddTime_To: $("#endTime").val(),
    }
    $.ajax({
        url: OperateUrl.Search,
        data: para,
        type: 'POST',
        beforeSend: function () {
            $.messager.progress({ title: '温馨提示', msg: '正在处理中... ...' });
        },
        success: function (data) {
            dataSource = data;
            console.log(dataSource);
            $('#dg').datagrid('loadData', dataSource);
            $.messager.progress('close');
        },
        error: function (error) {
            $.messager.progress('close');
            $.messager.alert('温馨提示', error.responseText);
        }
    })
}
