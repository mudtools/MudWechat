// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Marketing;

/// <summary>获取优惠券 ID 列表（<c>getcouponlist</c>）请求体。</summary>
/// <remarks>官方契约：<c>page_ctx</c> 第一次请求填空、后续请求填上次返回值；页码间隔不能超过 10，一页最大 200 条。</remarks>
[HttpJsonSerializable(SerializerClassName = "Marketing")]
public class ChannelsGetCouponListRequest
{
    /// <summary>获取或设置优惠券状态（官方 <c>status</c>，必填，见 <see cref="ChannelsCouponStatuses"/>，含官方复合值 200=过期或作废）。</summary>
    [JsonPropertyName("status")]
    public int Status { get; set; }

    /// <summary>获取或设置页码（官方 <c>page</c>，必填，最小填 1）。</summary>
    [JsonPropertyName("page")]
    public int Page { get; set; }

    /// <summary>获取或设置每页数量（官方 <c>page_size</c>，必填，不超过 200）。</summary>
    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }

    /// <summary>获取或设置翻页上下文（官方 <c>page_ctx</c>，必填；第一次请求填空，后续请求填上次返回值）。</summary>
    [JsonPropertyName("page_ctx")]
    public string PageCtx { get; set; } = string.Empty;
}
