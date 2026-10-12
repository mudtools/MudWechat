// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 售后管理（Aftersale）域「保障单」DTO（aftersale/getguaranteeorder、aftersale/searchguaranteeorder、
// aftersale/merchantacceptguarantee、aftersale/merchantmodifyguarantee、aftersale/merchantproofguarantee、
// aftersale/merchantrefuseguarantee）+ 保障单状态常量。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Aftersale。

namespace Mud.Wechat.Channels.DataModels.Aftersale;

/// <summary>坏损包退保障单详情（官方 <c>guarantee_order.bad_pay_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsGuaranteeBadPayInfo
{
    /// <summary>获取或设置损坏程度（官方 <c>bad_level</c>）。</summary>
    [JsonPropertyName("bad_level")]
    public int? BadLevel { get; set; }

    /// <summary>获取或设置损坏凭证文字（官方 <c>content</c>）。</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>获取或设置损坏凭证图片 media_id 列表（官方 <c>pic_list</c>）。</summary>
    [JsonPropertyName("pic_list")]
    public List<string>? PicList { get; set; }

    /// <summary>获取或设置赔付金额（官方 <c>pay_fee</c>，含运费）。</summary>
    [JsonPropertyName("pay_fee")]
    public long? PayFee { get; set; }

    /// <summary>获取或设置商家协商损坏程度（官方 <c>merchant_modify_level</c>）。</summary>
    [JsonPropertyName("merchant_modify_level")]
    public int? MerchantModifyLevel { get; set; }

    /// <summary>获取或设置商家备注（官方 <c>merchant_remark</c>）。</summary>
    [JsonPropertyName("merchant_remark")]
    public string? MerchantRemark { get; set; }

    /// <summary>获取或设置平台调整损坏程度（官方 <c>platform_modify_level</c>）。</summary>
    [JsonPropertyName("platform_modify_level")]
    public int? PlatformModifyLevel { get; set; }

    /// <summary>获取或设置退款类型（官方 <c>refund_type</c>）：1 仅退款；2 退货退款；3 换货。</summary>
    [JsonPropertyName("refund_type")]
    public int? RefundType { get; set; }

    /// <summary>获取或设置是否收到货（官方 <c>receive_product</c>）：1 未收到货；2 收到货。</summary>
    [JsonPropertyName("receive_product")]
    public int? ReceiveProduct { get; set; }

    /// <summary>获取或设置退款原因（官方 <c>refund_reason</c>）。</summary>
    [JsonPropertyName("refund_reason")]
    public int? RefundReason { get; set; }

    /// <summary>获取或设置退款原因文案（官方 <c>refund_reason_text</c>）。</summary>
    [JsonPropertyName("refund_reason_text")]
    public string? RefundReasonText { get; set; }

    /// <summary>获取或设置凭证图片列表（官方 <c>title_pic_list</c>，按图片类型分组）。</summary>
    [JsonPropertyName("title_pic_list")]
    public List<ChannelsGuaranteeTitlePic>? TitlePicList { get; set; }
}
