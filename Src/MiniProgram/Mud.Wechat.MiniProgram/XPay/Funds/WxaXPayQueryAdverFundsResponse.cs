// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 「查询广告金发放记录」响应：逐发放记录 + 总页数。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_query_adver_funds.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "XPay")]
public class WxaXPayQueryAdverFundsResponse : WxaResponse
{
    /// <summary>adver_funds_list（官方类型 <c>objarray</c>）：广告金发放记录列表</summary>
    [JsonPropertyName("adver_funds_list")]
    public List<WxaXPayAdverFund>? AdverFundsList { get; set; }

    /// <summary>total_page（官方类型 <c>number</c>）：查询命中总的页数</summary>
    [JsonPropertyName("total_page")]
    public long? TotalPage { get; set; }
}
