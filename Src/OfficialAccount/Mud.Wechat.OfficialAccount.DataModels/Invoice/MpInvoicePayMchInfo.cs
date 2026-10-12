// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Invoice;

/// <summary>微信支付商户号与开票平台关系信息（官方 <c>paymch_info</c>；请求与响应同构 ⇒ 共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoicePayMchInfo
{
    /// <summary>获取或设置微信支付商户号（官方 <c>mchid</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>获取或设置开票平台 id（官方 <c>s_pappid</c>；为该商户提供开票服务的开票平台标识）。</summary>
    [JsonPropertyName("s_pappid")]
    public string? SPappId { get; set; }
}
