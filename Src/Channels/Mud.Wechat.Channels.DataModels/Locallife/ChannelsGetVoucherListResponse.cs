// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Locallife;

/// <summary>查询用户的团购优惠列表（<c>findvoucher</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Locallife")]
public class ChannelsGetVoucherListResponse : ChannelsResponse
{
    /// <summary>获取或设置用户 openid（官方 <c>openid</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>获取或设置用户券列表（官方 <c>voucher_list</c>；官方未给出元素字段表，官方示例与 getvoucher 券对象同构 ⇒ 共用 <see cref="ChannelsVoucher"/>）。</summary>
    [JsonPropertyName("voucher_list")]
    public List<ChannelsVoucher>? VoucherList { get; set; }

    /// <summary>获取或设置翻页上下文（官方 <c>page_ctx</c>）。</summary>
    [JsonPropertyName("page_ctx")]
    public string? PageCtx { get; set; }
}
