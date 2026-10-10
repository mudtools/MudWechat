// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 编辑收集表请求体（<c>/cgi-bin/wedoc/modify_form</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方业务限制：<c>oper</c> 为 <c>1</c> 时全量修改问题（form_title / form_desc / form_header / form_question）、
/// 为 <c>2</c> 时全量修改设置（form_setting），两者对应不同字段组；
/// 若收集表当前为家校范围，<c>fill_out_auth</c> 无法修改且问题 id 从 2 开始；
/// <c>timed_finish</c> 与定时重复互斥，若都填优先定时重复。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class ModifyWedocFormRequest
{
    /// <summary>
    /// 获取或设置操作类型（官方 <c>oper</c>，必填）。
    /// 官方取值：<c>1</c> 全量修改问题、<c>2</c> 全量修改设置。
    /// </summary>
    [JsonPropertyName("oper")]
    public uint? Oper { get; set; }

    /// <summary>获取或设置收集表 id（官方 <c>formid</c>，必填）。</summary>
    [JsonPropertyName("formid")]
    public string? Formid { get; set; }

    /// <summary>获取或设置收集表信息（官方 <c>form_info</c>），按 <c>oper</c> 选传对应字段组。</summary>
    [JsonPropertyName("form_info")]
    public WedocFormInfo? FormInfo { get; set; }
}
