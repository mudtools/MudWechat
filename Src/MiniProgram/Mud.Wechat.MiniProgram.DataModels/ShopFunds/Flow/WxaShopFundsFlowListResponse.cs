// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopFunds;

/// <summary>获取资金流水列表应答（<c>POST /shop/funds/scanorderflow</c>）。</summary>
/// <remarks>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/funds/scanorderflow.html"/>。
/// <para><b>应答只含流水单号列表</b>（官方类型「string列表」），明细须逐条调 <c>getorderflow</c>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopFunds")]
public class WxaShopFundsFlowListResponse : WxaResponse
{
    /// <summary>流水单号列表（<c>funds_flow_ids</c>，官方类型 string列表）。</summary>
    [JsonPropertyName("funds_flow_ids")]
    public List<string>? FundsFlowIds { get; set; }

    /// <summary>总数（<c>total_num</c>，官方回包参数表与示例均给出）。</summary>
    [JsonPropertyName("total_num")]
    public long? TotalNum { get; set; }
}
