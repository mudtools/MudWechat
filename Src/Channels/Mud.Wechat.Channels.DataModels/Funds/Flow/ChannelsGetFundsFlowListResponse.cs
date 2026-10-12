// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Funds;

/// <summary>获取资金流水列表（<c>getfundsflowlist</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsGetFundsFlowListResponse : ChannelsResponse
{
    /// <summary>获取或设置流水单号列表（官方 <c>flow_ids</c>）。</summary>
    [JsonPropertyName("flow_ids")]
    public List<string>? FlowIds { get; set; }

    /// <summary>获取或设置是否还有下一页（官方 <c>has_more</c>）。</summary>
    [JsonPropertyName("has_more")]
    public bool? HasMore { get; set; }

    /// <summary>获取或设置分页参数（官方 <c>next_key</c>，深翻页时使用）。</summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }
}
