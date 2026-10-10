// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 查询智能表格子表权限请求体（<c>/cgi-bin/wedoc/smartsheet/content_priv/get_sheet_priv</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约陷阱：<c>rule_id_list</c> 官方参数表为 uint32 数组，官方 JSON 示例以字符串占位（如 <c>"RULEID1"</c>），
/// 本模型以参数表为准（整数数组）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class GetWedocSheetPrivRequest
{
    /// <summary>获取或设置智能表 id（官方 <c>docid</c>，必填），通过新建文档接口创建后获得。</summary>
    [JsonPropertyName("docid")]
    public string? Docid { get; set; }

    /// <summary>
    /// 获取或设置权限规则类型（官方 <c>type</c>，必填）。
    /// 官方取值：<c>1</c> 全员权限、<c>2</c> 额外权限。
    /// </summary>
    [JsonPropertyName("type")]
    public uint? Type { get; set; }

    /// <summary>获取或设置需要查询的规则 id 列表（官方 <c>rule_id_list</c>），查询额外权限时填写。</summary>
    [JsonPropertyName("rule_id_list")]
    public List<uint>? RuleIdList { get; set; }
}
