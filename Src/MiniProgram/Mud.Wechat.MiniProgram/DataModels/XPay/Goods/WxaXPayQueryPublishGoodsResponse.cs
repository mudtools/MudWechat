// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 「查询批量发布道具任务」响应：逐道具结果 + 任务总状态。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_query_publish_goods.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "XPay")]
public class WxaXPayQueryPublishGoodsResponse : WxaResponse
{
    /// <summary>publish_item（官方类型 <c>objarray</c>）：发布的道具列表</summary>
    [JsonPropertyName("publish_item")]
    public List<WxaXPayPublishGoodsResult>? PublishItem { get; set; }

    /// <summary>status（官方类型 <c>number</c>）：发布状态</summary>
    [JsonPropertyName("status")]
    public long? Status { get; set; }
}
