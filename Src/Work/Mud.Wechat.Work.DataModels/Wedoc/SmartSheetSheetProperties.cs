// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 智能表属性（官方 SmartSheet Properties；添加子表请求与响应、更新子表请求的 <c>properties</c> 共用）。
/// 字段按官方形态全部可空：添加子表只需 <c>title</c> / <c>index</c>，更新子表只需 <c>sheet_id</c>（必填）/ <c>title</c>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class SmartSheetSheetProperties
{
    /// <summary>
    /// 获取或设置智能表 ID，创建子表时生成的 6 位随机 ID（官方 <c>sheet_id</c>）。
    /// 更新子表时为必填（官方标注必填）；添加子表请求不传该字段。
    /// </summary>
    [JsonPropertyName("sheet_id")]
    public string? SheetId { get; set; }

    /// <summary>获取或设置智能表标题（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置智能表下标（官方 <c>index</c>）。</summary>
    [JsonPropertyName("index")]
    public int? Index { get; set; }
}
