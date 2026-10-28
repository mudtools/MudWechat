// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.MiniStore;

/// <summary>获取文件下载链接（<c>get_download_url</c>）响应。</summary>
/// <remarks>官方契约：链接有效期 10 分钟；order_id 维度限频 10 次/天；订单已结算或超售后期 7 天不可获取。</remarks>
[HttpJsonSerializable(SerializerClassName = "MiniStore")]
public class ChannelsMiniStoreGetDownloadUrlResponse : ChannelsResponse
{
    /// <summary>获取或设置文件下载链接（官方 <c>url</c>；官方变更记录：原 download_url 字段已修正为 url）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>获取或设置链接到期时间（官方 <c>expire_time</c>，秒级时间戳，有效期 10 分钟）。</summary>
    [JsonPropertyName("expire_time")]
    public long? ExpireTime { get; set; }
}
