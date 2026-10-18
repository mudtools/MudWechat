// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopFunds;

/// <summary>获取提现记录列表应答（<c>POST /shop/funds/scanwithdraw</c>）。</summary>
/// <remarks>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/wxafunds/API/funds/scanwithdraw.html"/>。
/// <para><b>应答只含提现单号列表</b>，逐条状态与金额仍须调 <c>getwithdrawdetail</c>；官方把列表字段类型写作「string列表」⇒ 建成 <c>List&lt;string&gt;</c>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopFunds")]
public class WxaShopFundsScanWithdrawResponse : WxaResponse
{
    /// <summary>提现单号列表（<c>out_request_nos</c>，官方类型 string列表）。</summary>
    [JsonPropertyName("out_request_nos")]
    public List<string>? OutRequestNos { get; set; }

    /// <summary>总数（<c>total_num</c>）。<b>只出现在官方回包示例、未进回包参数表</b> ⇒ 可空处理，宿主不得假定必有。</summary>
    [JsonPropertyName("total_num")]
    public long? TotalNum { get; set; }
}
