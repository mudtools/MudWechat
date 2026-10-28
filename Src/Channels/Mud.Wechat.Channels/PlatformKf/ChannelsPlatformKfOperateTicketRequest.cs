// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.PlatformKf;

/// <summary>商户处理商家工单（<c>operatemchticket</c>）请求体。</summary>
/// <remarks><c>description</c> / <c>confer_time</c> 在对应选项（经「查询工单分类配置」取得）的
/// <c>is_must_description</c> / <c>is_must_confer_time</c> 为 true 时必填。</remarks>
[HttpJsonSerializable(SerializerClassName = "PlatformKf")]
public class ChannelsPlatformKfOperateTicketRequest
{
    /// <summary>获取或设置工单 id（官方 <c>ticket_id</c>，必填）。</summary>
    [JsonPropertyName("ticket_id")]
    public string TicketId { get; set; } = string.Empty;

    /// <summary>获取或设置回复选项 id（官方 <c>reply_option</c>，必填；对应查询工单分类配置接口返回的 option_id）。</summary>
    [JsonPropertyName("reply_option")]
    public int ReplyOption { get; set; }

    /// <summary>获取或设置补充说明文字（官方 <c>description</c>；对应选项 is_must_description=true 时必填）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>获取或设置商家与用户协商的时间（官方 <c>confer_time</c>，Unix 时间戳，秒；对应选项 is_must_confer_time=true 时必填）。</summary>
    [JsonPropertyName("confer_time")]
    public long? ConferTime { get; set; }
}
