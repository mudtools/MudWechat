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

/// <summary>假一赔三保障单详情（官方 <c>guarantee_order.fake_one_pay_four_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsGuaranteeFakeOnePayFourInfo
{
    /// <summary>获取或设置鉴定费用（官方 <c>identify_fee</c>）。</summary>
    [JsonPropertyName("identify_fee")]
    public long? IdentifyFee { get; set; }

    /// <summary>获取或设置商品费用（官方 <c>product_fee</c>）。</summary>
    [JsonPropertyName("product_fee")]
    public long? ProductFee { get; set; }

    /// <summary>获取或设置预计赔付（官方 <c>total_pay_fee</c>）。</summary>
    [JsonPropertyName("total_pay_fee")]
    public long? TotalPayFee { get; set; }

    /// <summary>获取或设置鉴定凭证 media_id 列表（官方 <c>identify_proof_pic_list</c>）。</summary>
    [JsonPropertyName("identify_proof_pic_list")]
    public List<string>? IdentifyProofPicList { get; set; }

    /// <summary>获取或设置费用凭证 media_id 列表（官方 <c>fee_proof_pic_list</c>）。</summary>
    [JsonPropertyName("fee_proof_pic_list")]
    public List<string>? FeeProofPicList { get; set; }

    /// <summary>获取或设置申请原因（官方 <c>apply_reason</c>）：1 假一赔三假冒注册商标；2 假一赔三假冒材质成分。</summary>
    [JsonPropertyName("apply_reason")]
    public int? ApplyReason { get; set; }

    /// <summary>获取或设置最终赔付金额（官方 <c>acctual_pay</c>）。</summary>
    [JsonPropertyName("acctual_pay")]
    public long? AcctualPay { get; set; }
}
