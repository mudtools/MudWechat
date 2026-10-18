// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopFunds;

/// <summary>获取资金流水列表请求（<c>POST /shop/funds/scanorderflow</c>）。</summary>
/// <remarks>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/funds/scanorderflow.html"/>。
/// <para><b>按「记账日期」整日查询</b>：<c>date</c> 必填、格式 <c>yyyyMMdd</c>（官方说明原文「格式：20210101」），无起止区间 ⇒ 跨日对账须逐日循环。</para>
/// <para><b><c>page_size</c> 官方明写「必须小于等于 100」</b>（本域唯一规模上限，属参数校验而非频率限制）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopFunds")]
public class WxaShopFundsFlowListRequest
{
    /// <summary>页码（<c>page_num</c>，必填；官方本页未写起始值，勿按同族提现列表的「从 1 开始」推断）。</summary>
    [JsonPropertyName("page_num")]
    public int? PageNum { get; set; }

    /// <summary>每页数据大小（<c>page_size</c>，必填，官方原文「必须小于等于 100」）。</summary>
    [JsonPropertyName("page_size")]
    public int? PageSize { get; set; }

    /// <summary>流水记账日期（<c>date</c>，必填，格式 <c>yyyyMMdd</c>，示例 <c>20211120</c>）。</summary>
    [JsonPropertyName("date")]
    public string? Date { get; set; }
}
