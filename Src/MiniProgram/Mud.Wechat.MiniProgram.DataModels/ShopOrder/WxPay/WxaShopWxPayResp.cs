// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 进件状态信息（<c>/shop/wxpay/get</c> 回包的 <c>wxpay_resp</c>，官方注明参考「查询申请状态」API）。
/// </summary>

[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopWxPayResp
{
    /// <summary>申请单状态（官方未在本页枚举取值）。</summary>
    [JsonPropertyName("applyment_state")]
    public string? ApplymentState { get; set; }

    /// <summary>申请单状态描述。</summary>
    [JsonPropertyName("applyment_state_desc")]
    public string? ApplymentStateDesc { get; set; }

    /// <summary>签约链接（敏感，不得进日志）。</summary>
    [JsonPropertyName("sign_url")]
    public string? SignUrl { get; set; }

    /// <summary>微信支付子商户号（敏感，不得进日志）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>结算账户验证信息（可能需要打款验证时返回）。</summary>
    [JsonPropertyName("account_validation")]
    public WxaShopWxPayAccountValidation? AccountValidation { get; set; }

    /// <summary>审核驳回明细列表。</summary>
    [JsonPropertyName("audit_detail")]
    public List<WxaShopWxPayAuditDetail>? AuditDetail { get; set; }

    /// <summary>法人验证链接（敏感，不得进日志）。</summary>
    [JsonPropertyName("legal_validation_url")]
    public string? LegalValidationUrl { get; set; }

    /// <summary>商户自定义进件单号（回显）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>微信支付申请单号（官方示例为数字）。</summary>
    [JsonPropertyName("applyment_id")]
    public long? ApplymentId { get; set; }
}
