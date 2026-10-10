// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 文档的安全设置（官方 <c>secure_setting</c>；获取文档权限信息响应体嵌套对象）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class WedocDocSecureSetting
{
    /// <summary>获取或设置仅浏览权限的成员是否允许导出、复制、打印（官方 <c>enable_readonly_copy</c>）。</summary>
    [JsonPropertyName("enable_readonly_copy")]
    public bool? EnableReadonlyCopy { get; set; }

    /// <summary>获取或设置文档水印设置（官方 <c>watermark</c>）。</summary>
    [JsonPropertyName("watermark")]
    public WedocDocWatermark? Watermark { get; set; }

    /// <summary>获取或设置是否允许只读评论（官方 <c>enable_readonly_comment</c>）。</summary>
    [JsonPropertyName("enable_readonly_comment")]
    public bool? EnableReadonlyComment { get; set; }
}
