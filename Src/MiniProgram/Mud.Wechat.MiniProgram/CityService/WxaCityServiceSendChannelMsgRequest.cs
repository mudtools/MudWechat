// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.CityService;

/// <summary>
/// 就医助手消息推送请求体（<c>POST /cityservice/sendchannelmsg</c>）。
/// </summary>
/// <remarks>
/// <para><b>主体与能力门槛</b>：仅支持公立医院及卫健委主体的公众号/小程序，须先开通就医助手；
/// 须使用同主体公众号的 <c>access_token</c> 调用。</para>
/// <para><b>同次就医流程复用同一 <c>order_id</c></b>；<c>msg_id</c> 须在同一用户同一 <c>order_id</c> 下唯一。</para>
/// <para><b><c>status</c> 是消息子状态</b>（如 1501001 预约挂号成功通知），全部取值见官方 status 说明；
/// <c>business_id</c> 固定 <c>150</c>。</para>
/// <para><b>敏感面</b>：<c>open_id</c> / <c>order_id</c> / <c>business_info</c> 中的患者信息一律禁入日志。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "CityService")]
public class WxaCityServiceSendChannelMsgRequest
{
    /// <summary>消息子状态（必填，如 1501001 预约挂号成功通知）。</summary>
    [JsonPropertyName("status")]
    public long? Status { get; set; }

    /// <summary>用户 openid（公众号/小程序）（必填）。敏感字段，禁入日志。</summary>
    [JsonPropertyName("open_id")]
    public string? OpenId { get; set; }

    /// <summary>业务方生成的唯一订单 ID（必填）；同次就医流程须相同。敏感字段，禁入日志。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>消息唯一标识（必填，同用户同 order_id 下唯一）。</summary>
    [JsonPropertyName("msg_id")]
    public string? MsgId { get; set; }

    /// <summary>公众号 appid（须已开通就医助手）（必填）。</summary>
    [JsonPropertyName("app_id")]
    public string? AppId { get; set; }

    /// <summary>业务 id（必填），固定值 <c>150</c>。</summary>
    [JsonPropertyName("business_id")]
    public int? BusinessId { get; set; }

    /// <summary>业务字段（可选；不同 status 对应不同结构）。</summary>
    [JsonPropertyName("business_info")]
    public WxaCityServiceSendChannelMsgBusinessInfo? BusinessInfo { get; set; }
}
