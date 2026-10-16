// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.Catering;

/// <summary>点餐订单信息同步请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015863180"/>（菜品属性 <c>properties</c> 官方未给元素结构 ⇒ 以 <see cref="JsonElement"/> 原样承载）</summary>
[HttpJsonSerializable(SerializerClassName = "Catering")]
public class CateringOrderSyncStatusRequest
{
    /// <summary>微信支付分配的服务商商户号（官方字段路径 <c>sp_mchid</c>，<b>必填</b>，类型 <c>String(32)</c>）。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchid { get; set; }

    /// <summary>微信支付分配子商户商户号（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>String(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>服务商在微信公众平台申请服务号对应的APPID（官方字段路径 <c>sp_appid</c>，<b>必填</b>，类型 <c>String(32)</c>）。</summary>
    [JsonPropertyName("sp_appid")]
    public string? SpAppid { get; set; }

    /// <summary>子商户在微信公众平台申请服务号对应的APPID（官方字段路径 <c>sub_appid</c>，选填，类型 <c>String(32)</c>）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppid { get; set; }

    /// <summary>商户旗下门店的唯一编号（官方字段路径 <c>out_shop_no</c>，<b>必填</b>，类型 <c>String(32)</c>）。</summary>
    [JsonPropertyName("out_shop_no")]
    public string? OutShopNo { get; set; }

    /// <summary>用户标识，用户在服务商appid下的openid。 openid和sub_openid可以选传其中之一，如果选择传sub_openid,则必须传sub_appid（官方字段路径 <c>openid</c>，选填，类型 <c>String(32)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>用户子标识，用户在子商户appid下的openid。openid和sub_openid可以选传其中之一，如果选择传sub_openid,则必须传sub_appid（官方字段路径 <c>sub_openid</c>，选填，类型 <c>String(32)</c>）。</summary>
    [JsonPropertyName("sub_openid")]
    public string? SubOpenid { get; set; }

    /// <summary>微信用户登录接口返回的登录票据 公众号，填写页面授权access_token，详细参考； 小程序，填写session_key，详细参考（官方字段路径 <c>login_token</c>，<b>必填</b>，类型 <c>String(256)</c>）。</summary>
    [JsonPropertyName("login_token")]
    public string? LoginToken { get; set; }

    /// <summary>点餐入口，公众号：点餐页面完整URL 小程序：点餐页面path路径（官方字段路径 <c>order_entry</c>，<b>必填</b>，类型 <c>String(256)</c>）。</summary>
    [JsonPropertyName("order_entry")]
    public string? OrderEntry { get; set; }

    /// <summary>总价，单位为分（官方字段路径 <c>total_amount</c>，<b>必填</b>，类型 <c>int</c>）。</summary>
    [JsonPropertyName("total_amount")]
    public long? TotalAmount { get; set; }

    /// <summary>优惠金额，单位为分（官方字段路径 <c>discount_amount</c>，<b>必填</b>，类型 <c>int</c>）。</summary>
    [JsonPropertyName("discount_amount")]
    public long? DiscountAmount { get; set; }

    /// <summary>实际支付金额，单位为分（官方字段路径 <c>user_amount</c>，<b>必填</b>，类型 <c>int</c>）。</summary>
    [JsonPropertyName("user_amount")]
    public long? UserAmount { get; set; }

    /// <summary>订单状态，取值如下： CREATE_DEAL—用户下单； PAY_SUCCESS—支付完成，结账成功（官方字段路径 <c>status</c>，<b>必填</b>，类型 <c>String(32)</c>）。</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>状态发生变化的时间，格式为rfc3339格式，如2018-06-08T10:34:56+08:00 代表北京时间2018年06月08日10时34分56秒（官方字段路径 <c>action_time</c>，<b>必填</b>，类型 <c>String(64)</c>）。</summary>
    [JsonPropertyName("action_time")]
    public string? ActionTime { get; set; }

    /// <summary>支付时间，格式为rfc3339格式，如2018-06-08T10:34:56+08:00 代表北京时间2018年06月08日10时34分56秒（status为PAY_SUCCESS时必填）（官方字段路径 <c>pay_time</c>，选填，类型 <c>String(64)</c>）。</summary>
    [JsonPropertyName("pay_time")]
    public string? PayTime { get; set; }

    /// <summary>支付订单号（status为PAY_SUCCESS时必填）（官方字段路径 <c>transaction_id</c>，选填，类型 <c>String(64)</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>服务商系统内部支付订单号（status为PAY_SUCCESS时必填）（官方字段路径 <c>out_trade_no</c>，选填，类型 <c>String(64)</c>）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>服务商系统内部订单号（官方字段路径 <c>out_order_no</c>，<b>必填</b>，类型 <c>String(64)</c>）。</summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>桌位号（官方字段路径 <c>out_table_no</c>，选填，类型 <c>String(16)</c>）。</summary>
    [JsonPropertyName("out_table_no")]
    public string? OutTableNo { get; set; }

    /// <summary>消费人数（官方字段路径 <c>people_count</c>，选填，类型 <c>int</c>）。</summary>
    [JsonPropertyName("people_count")]
    public long? PeopleCount { get; set; }

    /// <summary>商户菜品ID（官方字段路径 <c>out_dish_no</c>，<b>必填</b>，类型 <c>String(48)</c>）。</summary>
    [JsonPropertyName("out_dish_no")]
    public string? OutDishNo { get; set; }

    /// <summary>菜品名称（官方字段路径 <c>name</c>，<b>必填</b>，类型 <c>String(128)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>菜品单价，单位为分（官方字段路径 <c>price</c>，<b>必填</b>，类型 <c>int</c>）。</summary>
    [JsonPropertyName("price")]
    public long? Price { get; set; }

    /// <summary>菜品单位，BY_SHARE-按份 BY_WEIGHT-按重量（官方字段路径 <c>unit</c>，<b>必填</b>，类型 <c>String(16)</c>）。</summary>
    [JsonPropertyName("unit")]
    public string? Unit { get; set; }

    /// <summary>菜品数量，保留小数点后2位有效数字（官方字段路径 <c>count</c>，<b>必填</b>，类型 <c>Float</c>）。</summary>
    [JsonPropertyName("count")]
    public double? Count { get; set; }

    /// <summary>菜品折扣，百分值，8折填80（官方字段路径 <c>discount</c>，选填，类型 <c>int</c>）。</summary>
    [JsonPropertyName("discount")]
    public long? Discount { get; set; }

    /// <summary>菜品分类，如等。详见参数规定《菜品类型列表》（官方字段路径 <c>type</c>，选填，类型 <c>String(32)</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>当前菜品在服务商平台的顺序，值越小越靠前，取值（1~100）（官方字段路径 <c>priority</c>，选填，类型 <c>int</c>）。</summary>
    [JsonPropertyName("priority")]
    public long? Priority { get; set; }

    /// <summary>菜品属性，详见参数规定《菜品属性列表》（官方字段路径 <c>properties</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("properties")]
    public JsonElement? Properties { get; set; }

    /// <summary>口味，取值参考参数规定中的“菜品属性”taste字段，可额外补充（官方字段路径 <c>taste</c>，<b>必填</b>，类型 <c>String(128)</c>）。</summary>
    [JsonPropertyName("taste")]
    public string? Taste { get; set; }

    /// <summary>做法，取值参考参数规定中的“菜品属性”cuisine字段，可额外补充（官方字段路径 <c>cuisine</c>，<b>必填</b>，类型 <c>String(128)</c>）。</summary>
    [JsonPropertyName("cuisine")]
    public string? Cuisine { get; set; }

}
