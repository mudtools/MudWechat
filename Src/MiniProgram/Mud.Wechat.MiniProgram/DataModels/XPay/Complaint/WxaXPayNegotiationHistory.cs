// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 协商历史条目（官方 <c>Res.history</c> 元素）。
/// </summary>
/// <remarks>
/// <para><b><c>operate_type</c> 的取值字典只在本端点给出</b>（<see cref="WxaXPayOperateTypes"/> 20 值），其中两值官方明示「只存在于历史投诉单的协商历史中」⇒ 不得据本表推断投诉列表的状态机。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_get_negotiation_history.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "XPay")]
public class WxaXPayNegotiationHistory
{
    /// <summary>log_id（官方类型 <c>string</c>）：操作流水号</summary>
    [JsonPropertyName("log_id")]
    public string? LogId { get; set; }

    /// <summary>operator（官方类型 <c>string</c>）：当前投诉协商记录的操作人</summary>
    [JsonPropertyName("operator")]
    public string? Operator { get; set; }

    /// <summary>operate_time（官方类型 <c>string</c>）：当前操作时间，格式为yyyy-mm-dd'T'HH:MM:ssXXX，其中XXX为时区偏移，例如：2023-11-28T11:11:49+08:00</summary>
    [JsonPropertyName("operate_time")]
    public string? OperateTime { get; set; }

    /// <summary>operate_type（官方类型 <c>string</c>）：当前投诉协商记录的操作类型</summary>
    [JsonPropertyName("operate_type")]
    public string? OperateType { get; set; }

    /// <summary>operate_details（官方类型 <c>string</c>）：当前投诉协商记录的具体内容</summary>
    [JsonPropertyName("operate_details")]
    public string? OperateDetails { get; set; }

    /// <summary>complaint_media_list（官方类型 <c>objarray</c>）：投诉单执行操作时上传的资料凭证，包含用户、商户、微信支付客服等角色操作</summary>
    [JsonPropertyName("complaint_media_list")]
    public List<WxaXPayComplaintMedia>? ComplaintMediaList { get; set; }
}
