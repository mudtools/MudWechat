// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 经营者 / 法人其他类型证件信息（<c>/shop/wxpay/apply</c>，证件类型为来往内地通行证 / 来往大陆通行证 / 护照时填写）。
/// </summary>

[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopWxPayIdDocInfo
{
    /// <summary>证件姓名（<b>必填</b>）。</summary>
    [JsonPropertyName("id_doc_name")]
    public string? IdDocName { get; set; }

    /// <summary>证件号码（<b>必填</b>）：7～11 位数字（个人敏感信息，不得进日志）。</summary>
    [JsonPropertyName("id_doc_number")]
    public string? IdDocNumber { get; set; }

    /// <summary>证件正面照片（<b>必填</b>）。</summary>
    [JsonPropertyName("wxa_id_doc_copy")]
    public WxaShopWxPayPicFile? WxaIdDocCopy { get; set; }

    /// <summary>证件反面照片（选填）。</summary>
    [JsonPropertyName("wxa_id_doc_copy_back")]
    public WxaShopWxPayPicFile? WxaIdDocCopyBack { get; set; }

    /// <summary>证件居住地址：主体类型为企业时需填写（个人敏感信息，不得进日志）。</summary>
    [JsonPropertyName("id_doc_address")]
    public string? IdDocAddress { get; set; }

    /// <summary>证件开始日期（<b>必填</b>）：<c>yyyy-MM-dd</c>，结束须大于开始。</summary>
    [JsonPropertyName("doc_period_begin")]
    public string? DocPeriodBegin { get; set; }

    /// <summary>证件结束日期（<b>必填</b>）：<c>yyyy-MM-dd</c>，长期填「长期」。</summary>
    [JsonPropertyName("doc_period_end")]
    public string? DocPeriodEnd { get; set; }
}
