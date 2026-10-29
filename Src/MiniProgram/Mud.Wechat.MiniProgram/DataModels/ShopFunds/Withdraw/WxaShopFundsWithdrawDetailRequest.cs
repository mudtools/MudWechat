// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopFunds;

/// <summary>获取提现记录详情请求（<c>POST /shop/funds/getwithdrawdetail</c>）。</summary>
/// <remarks>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/wxafunds/API/funds/getwithdrawdetail.html"/>。
/// <para><b>三支全必填</b>：提现单号 + 业务类型（填 1）+ 商户号；本域无「按商户号列全量详情」的形态，翻页须走 <c>scanwithdraw</c>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopFunds")]
public class WxaShopFundsWithdrawDetailRequest
{
    /// <summary>提现单号（<c>out_request_no</c>，必填，即发起提现应答返回的单号）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>业务类型（<c>biz_type</c>，必填，官方参数表原文「业务类型，填 1」）。</summary>
    [JsonPropertyName("biz_type")]
    public int? BizType { get; set; }

    /// <summary>商户号（<c>mchid</c>，必填）。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }
}
