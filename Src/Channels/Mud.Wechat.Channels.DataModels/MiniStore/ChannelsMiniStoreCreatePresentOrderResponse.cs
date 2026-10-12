// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.MiniStore;

/// <summary>创建并发送礼物（<c>presentorder/create</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "MiniStore")]
public class ChannelsMiniStoreCreatePresentOrderResponse : ChannelsResponse
{
    /// <summary>获取或设置礼物单 id（官方 <c>present_order_id</c>）。</summary>
    [JsonPropertyName("present_order_id")]
    public string? PresentOrderId { get; set; }

    /// <summary>获取或设置外部 app 跳转业务类型（官方 <c>business_type</c>，仅外部应用身份调用返回）。</summary>
    [JsonPropertyName("business_type")]
    public string? BusinessType { get; set; }

    /// <summary>获取或设置外部 app 跳转参数（官方 <c>query</c>，仅外部应用身份调用返回）。</summary>
    [JsonPropertyName("query")]
    public string? Query { get; set; }
}
