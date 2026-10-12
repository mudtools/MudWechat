// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.OneCode;

// ---------------------------------------------------------------- 查询激活状态 / CODE_TICKET 换 CODE（共用响应）

/// <summary>
/// 查询二维码激活状态（<c>POST /intp/marketcode/codeactivequery</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// 官方注意事项原文：「支持通过 <c>application_id</c>+<c>code_index</c> <b>或</b> <c>code</c>/URL 两种查询方式」
/// ⇒ 两条路径互斥。官方字段表标注：<c>application_id</c> 非必填、<c>code_index</c>「传入
/// <c>application_id</c> 时必填」、<c>code_url</c> 与 <c>code</c> 二选一。SDK 全部可空、不本地校验
/// （官方文档缺陷：两条路径的优先级与互斥关系未闭合说明，照录）。
/// </para>
/// <para>
/// <b>类型口径（关键）</b>：官方请求字段表把 <c>code</c> 标为 <c>number</c>，但同表明文描述为
/// 「<b>九位的字符串原始码</b>」、且<b>返回体字段表</b>同名字段标 <c>string</c> —— 三处证据中两处为字符串，
/// 且「原始码」可能有<b>前导零</b>（数值类型会丢）⇒ 建模为 <c>string</c>（请求表的 number 标注为误标，照录）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OneCode")]
public class MpCodeActiveQueryRequest
{
    /// <summary>获取或设置申请单号（官方 <c>application_id</c>；配合 <see cref="CodeIndex"/> 使用）。</summary>
    [JsonPropertyName("application_id")]
    public long? ApplicationId { get; set; }

    /// <summary>获取或设置该码在批次中的偏移量（官方 <c>code_index</c>；传入 <c>application_id</c> 时必填）。</summary>
    [JsonPropertyName("code_index")]
    public int? CodeIndex { get; set; }

    /// <summary>获取或设置普通码字符（官方 <c>code_url</c>；28 位，与 <see cref="Code"/> 二选一）。</summary>
    [JsonPropertyName("code_url")]
    public string? CodeUrl { get; set; }

    /// <summary>获取或设置原始码（官方 <c>code</c>；九位字符串原始码，与 <see cref="CodeUrl"/> 二选一）。</summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }
}
