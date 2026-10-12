// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Mud.Wechat.Channels.DataModels.Callback;

/// <summary>
/// 「下单页获取初始会员权益」事件（<c>channels_ec_gen_order_info_init</c>）的结构化应答。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public class ChannelsGenOrderInfoInitReply
{
    /// <summary>获取或设置错误码（0 成功）。</summary>
    [JsonPropertyName("errcode")]
    public long ErrCode { get; set; }

    /// <summary>获取或设置错误信息。</summary>
    [JsonPropertyName("errmsg")]
    public string ErrMsg { get; set; } = string.Empty;

    /// <summary>获取或设置会员权益信息（官方 info 结构）。</summary>
    [JsonPropertyName("info")]
    public ChannelsGenOrderInfoInitReplyInfo? Info { get; set; }
}
