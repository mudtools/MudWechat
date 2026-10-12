// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Funds;

/// <summary>订单流水分页信息（官方 <c>pagination_info</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsOrderFlowPagination
{
    /// <summary>获取或设置页大小（官方 <c>limit</c>，必填）。</summary>
    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    /// <summary>获取或设置偏移量（官方 <c>offset</c>，选填）。</summary>
    [JsonPropertyName("offset")]
    public int? Offset { get; set; }

    /// <summary>
    /// 获取或设置是否使用分页上下文（官方 <c>use_page_ctx</c>，选填）——可保证翻页的过程中订单不会遗漏和重复，
    /// 此时 <c>offset</c> 不生效。
    /// </summary>
    [JsonPropertyName("use_page_ctx")]
    public bool? UsePageCtx { get; set; }

    /// <summary>获取或设置分页上下文（官方 <c>page_ctx</c>，选填）——每次翻页传入上一页回参的 <c>page_ctx</c>，第一页可不传或者传空字符串。</summary>
    [JsonPropertyName("page_ctx")]
    public string? PageCtx { get; set; }
}
