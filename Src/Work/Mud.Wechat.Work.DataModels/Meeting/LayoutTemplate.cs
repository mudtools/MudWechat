// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 布局模板对象（获取布局模板列表响应 <c>layout_template_list</c> 嵌套对象）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class LayoutTemplate
{
    /// <summary>获取或设置布局模板 ID。</summary>
    [JsonPropertyName("layout_template_id")]
    public string? LayoutTemplateId { get; set; }

    /// <summary>获取或设置缩略图 URL。</summary>
    [JsonPropertyName("thumbnail_url")]
    public string? ThumbnailUrl { get; set; }

    /// <summary>
    /// 获取或设置布局图 URL。
    /// <para>布局图中包含宫格 ID，该 ID 用于确定布局宫格和成员座次的映射关系，设置和修改会议布局时需要当做入参传递。</para>
    /// </summary>
    [JsonPropertyName("picture_url")]
    public string? PictureUrl { get; set; }

    /// <summary>获取或设置渲染规则（提供成员个人渲染布局图片的规则说明，JSON 字符串）。</summary>
    [JsonPropertyName("render_rule")]
    public string? RenderRule { get; set; }
}
