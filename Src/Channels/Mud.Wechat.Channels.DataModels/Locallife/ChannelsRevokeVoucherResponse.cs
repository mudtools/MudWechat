// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Locallife;

/// <summary>撤销用户已核销团购优惠（<c>withdrawconsume</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Locallife")]
public class ChannelsRevokeVoucherResponse : ChannelsResponse
{
    /// <summary>获取或设置用户券详情列表（官方 <c>voucher_list</c>，与详情/核销响应券对象同构 ⇒ 共用 <see cref="ChannelsVoucher"/>）。</summary>
    [JsonPropertyName("voucher_list")]
    public List<ChannelsVoucher>? VoucherList { get; set; }

    /// <summary>获取或设置无法操作的券（官方 <c>fail_voucher_list</c>；明确有券无法核销时返回，含无法核销原因）。</summary>
    [JsonPropertyName("fail_voucher_list")]
    public List<ChannelsVoucherFailItem>? FailVoucherList { get; set; }
}
