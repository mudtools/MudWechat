// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 发布智能文档响应体（<c>/cgi-bin/wedoc/smartdoc/publish</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class PublishSmartDocResponse : WechatWorkResponse
{
    /// <summary>获取或设置发布页分享码（官方 <c>share_code</c>）。</summary>
    [JsonPropertyName("share_code")]
    public string? ShareCode { get; set; }

    /// <summary>
    /// 获取或设置发布页访问链接（官方 <c>publish_url</c>），
    /// 格式为 <c>https://doc.weixin.qq.com/smartdoc/p/{share_code}</c>。
    /// </summary>
    [JsonPropertyName("publish_url")]
    public string? PublishUrl { get; set; }

    /// <summary>获取或设置发布版本号（官方 <c>version</c>，官方类型为 uint64）。</summary>
    [JsonPropertyName("version")]
    public ulong? Version { get; set; }

    /// <summary>获取或设置发布时间戳（官方 <c>publish_time</c>，单位为秒，官方类型为 uint64）。</summary>
    [JsonPropertyName("publish_time")]
    public ulong? PublishTime { get; set; }

    /// <summary>获取或设置发布页标题（官方 <c>publish_doc_title</c>）。</summary>
    [JsonPropertyName("publish_doc_title")]
    public string? PublishDocTitle { get; set; }
}
