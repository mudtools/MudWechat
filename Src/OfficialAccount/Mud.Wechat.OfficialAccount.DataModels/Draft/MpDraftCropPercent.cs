// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Draft;

/// <summary>封面裁剪条目（官方 <c>cover_info.crop_percent_list</c> 元素；官方示例坐标为字符串形态）。</summary>
/// <remarks>ratio 取值：news 仅支持 "2.35_1"/"1_1"；newspic 支持 "1_1"/"16_9"/"2.35_1"（官方页面原文）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Draft")]
public class MpDraftCropPercent
{
    /// <summary>获取或设置裁剪比例（官方 <c>ratio</c>，字符串形态如 "2.35_1"）。</summary>
    [JsonPropertyName("ratio")]
    public string? Ratio { get; set; }

    /// <summary>获取或设置裁剪区域左上角 x（官方 <c>x1</c>）。</summary>
    [JsonPropertyName("x1")]
    public string? X1 { get; set; }

    /// <summary>获取或设置裁剪区域左上角 y（官方 <c>y1</c>）。</summary>
    [JsonPropertyName("y1")]
    public string? Y1 { get; set; }

    /// <summary>获取或设置裁剪区域右下角 x（官方 <c>x2</c>）。</summary>
    [JsonPropertyName("x2")]
    public string? X2 { get; set; }

    /// <summary>获取或设置裁剪区域右下角 y（官方 <c>y2</c>）。</summary>
    [JsonPropertyName("y2")]
    public string? Y2 { get; set; }
}
