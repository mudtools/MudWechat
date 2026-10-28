// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopFunds;

/// <summary>获取资金流水详情请求（<c>POST /shop/funds/getorderflow</c>）。</summary>
/// <remarks>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/funds/getorderflow.html"/>。
/// <para>
/// <b>字段名取官方参数表而非请求示例（本页自相矛盾）</b>：参数表写 <c>funds_flow_no</c>「流水单号」必填，
/// 同页示例却写 <c>funds_flow_id</c>；应答侧另有 <c>funds_flow_id</c>「流水id」⇒ 本仓请求只暴露
/// <c>funds_flow_no</c>，两字段不得合并（守卫 SHOPF5 锁定）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopFunds")]
public class WxaShopFundsFlowDetailRequest
{
    /// <summary>流水单号（<c>funds_flow_no</c>，必填，取自 <c>scanorderflow</c> 应答的单号列表）。</summary>
    [JsonPropertyName("funds_flow_no")]
    public string? FundsFlowNo { get; set; }
}
