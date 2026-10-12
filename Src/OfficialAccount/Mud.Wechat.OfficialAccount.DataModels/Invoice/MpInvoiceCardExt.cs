// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Invoice;

/// <summary>发票具体内容（官方 <c>card_ext</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceCardExt
{
    /// <summary>获取或设置随机字符串（官方 <c>nonce_str</c>，必填；防重复）。</summary>
    [JsonPropertyName("nonce_str")]
    public string NonceStr { get; set; } = string.Empty;

    /// <summary>获取或设置用户信息结构体（官方 <c>user_card</c>，必填）。</summary>
    [JsonPropertyName("user_card")]
    public MpInvoiceUserCard UserCard { get; set; } = new MpInvoiceUserCard();
}
