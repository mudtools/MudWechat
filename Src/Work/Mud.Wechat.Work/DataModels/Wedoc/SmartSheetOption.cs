// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 选项（官方 Option；单选 / 多选字段属性与记录单元格值的选项项共用）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class SmartSheetOption
{
    /// <summary>获取或设置选项 ID（官方 <c>id</c>）。</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>获取或设置选项内容（官方 <c>text</c>）。</summary>
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    /// <summary>
    /// 获取或设置选项颜色（官方 <c>style</c>）。
    /// 官方取值：1 浅红、9 灰、10 浅蓝、11 浅蓝、12 浅橙 / 蓝、13 浅天蓝 / 天蓝、14 浅绿、15 浅紫 / 浅绿、16 浅粉红 / 绿、17 浅灰 / 浅红、18 白 / 红、19 浅橙、20 橙、21 浅黄、22 浅黄、23 黄、24 浅紫、25 紫、26 浅粉红、27 粉红（官方原表存在重复项，照抄原文）。
    /// </summary>
    [JsonPropertyName("style")]
    public int? Style { get; set; }
}
