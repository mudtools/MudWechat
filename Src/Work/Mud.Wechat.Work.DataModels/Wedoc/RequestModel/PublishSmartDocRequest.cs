// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 发布智能文档请求体（<c>/cgi-bin/wedoc/smartdoc/publish</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class PublishSmartDocRequest
{
    /// <summary>获取或设置智能文档的文档 ID（官方 <c>docid</c>，必填），可通过「新建文档」接口创建并获取。</summary>
    [JsonPropertyName("docid")]
    public string? Docid { get; set; }

    /// <summary>
    /// 获取或设置发布可见范围（官方 <c>publish_range</c>，非必填，默认 <c>1</c>）。
    /// 官方取值：<c>1</c> 企业内可见、<c>3</c> 企业内外可见、<c>4</c> 指定成员可见。
    /// </summary>
    [JsonPropertyName("publish_range")]
    public uint? PublishRange { get; set; }

    /// <summary>
    /// 获取或设置指定可见成员列表（官方 <c>auth_list</c>），<c>publish_range</c> 为 <c>4</c> 时必填。
    /// </summary>
    [JsonPropertyName("auth_list")]
    public List<SmartDocPublishAuth>? AuthList { get; set; }
}
