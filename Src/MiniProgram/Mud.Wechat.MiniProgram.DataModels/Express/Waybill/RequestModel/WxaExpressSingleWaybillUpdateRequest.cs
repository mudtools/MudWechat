// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>运力更新订单状态请求体（<c>POST /cgi-bin/express/delivery/single_waybill/update</c>，散单寄件）。</summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-provider/api_scatterupdateorderstatus"/>。</para>
/// <para>
/// <c>token</c> 为商户侧下单事件中推送的 Token 字段；<c>action_type</c> 枚举见附录：
/// 90001 网点接单 / 90002 分配业务员 / 90010 待支付 / 90011 已支付 / 100001 揽件成功 / 100002 揽件失败 /
/// 200001 更新运输轨迹 / 300002 派送中 / 300003 签收成功 / 300004 签收失败 / 300005 第三方代收入库 /
/// 300006 第三方代收取出 / 300007 代签收 / 400001 订单取消 / 400002 订单滞留 / 400003 订单退回 /
/// 400004 订单拒收 / 400005 问题件 / 500001 其他未分类。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressSingleWaybillUpdateRequest
{
    /// <summary>商户侧下单事件中推送的 Token 字段（<c>token</c>，必填）。</summary>
    [JsonPropertyName("token")]
    public string? Token { get; set; }

    /// <summary>运单 ID（<c>waybill_id</c>，可选）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>轨迹变化 Unix 时间戳（<c>action_time</c>，必填）。</summary>
    [JsonPropertyName("action_time")]
    public long ActionTime { get; set; }

    /// <summary>轨迹变化类型（<c>action_type</c>，必填；与普通单保持一致，参见附录 action_type 定义）。</summary>
    [JsonPropertyName("action_type")]
    public int ActionType { get; set; }

    /// <summary>轨迹变化具体信息说明（<c>action_msg</c>，必填；展示在快递轨迹详情页，若有手机号码则直接写 11 位手机号码，UTF-8 编码）。</summary>
    [JsonPropertyName("action_msg")]
    public string? ActionMessage { get; set; }

    /// <summary>取件员姓名（<c>pickup_courier_name</c>，必填）。</summary>
    [JsonPropertyName("pickup_courier_name")]
    public string? PickupCourierName { get; set; }

    /// <summary>取件员电话（<c>pickup_courier_phone</c>，必填）。</summary>
    [JsonPropertyName("pickup_courier_phone")]
    public string? PickupCourierPhone { get; set; }

    /// <summary>派件员姓名（<c>delivery_courier_name</c>，必填）。</summary>
    [JsonPropertyName("delivery_courier_name")]
    public string? DeliveryCourierName { get; set; }

    /// <summary>派件员电话（<c>delivery_courier_phone</c>，必填）。</summary>
    [JsonPropertyName("delivery_courier_phone")]
    public string? DeliveryCourierPhone { get; set; }
}
