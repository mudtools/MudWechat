// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 文件类型字段属性（官方 AttachmentFieldProperty）。
/// </summary>
/// <remarks>
/// <para>
/// 官方「添加字段」文档把 <c>display_mode</c> 的说明误写为「设置日期格式」，判断为官方文档描述错误；此处按「查询字段」文档的说明「展示样式」承载。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class SmartSheetAttachmentFieldProperty
{
    /// <summary>
    /// 获取或设置展示样式（官方 <c>display_mode</c>）。
    /// 官方取值：<c>DISPLAY_MODE_LIST</c> 列表样式、<c>DISPLAY_MODE_GRID</c> 宫格样式。
    /// </summary>
    [JsonPropertyName("display_mode")]
    public string? DisplayMode { get; set; }
}
