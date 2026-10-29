// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Tag;

/// <summary>
/// 批量打标签 / 取消标签响应（两端点共用）。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表只列 <c>errcode</c> / <c>errmsg</c>（由 <see cref="MpResponse"/> 承载）；
/// 但错误码 <c>45171</c>（some openid fail）的解决方案原文为「<b><c>fail_openid_list</c> 里会给出失败的
/// openid</b>，可以尝试对其进行重试」⇒ 失败响应体会额外携带 <c>fail_openid_list</c>。
/// 故本 DTO 显式建模该字段（可空：仅在 <c>45171</c> 等部分失败场景下出现），
/// 使调用方能按官方指引做定向重试而不必自行解析原始 JSON。
/// </para>
/// <para>
/// <b>部分失败语义</b>：<c>45171</c> 表示「部分 openid 失败」（非全量失败）⇒ 判错出口抛异常时，
/// 调用方应经本字段做定向重试，而不是整批重放（整批重放会对已成功的 openid 重复打标）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Tag")]
public class MpTagMembersResponse : MpResponse
{
    /// <summary>部分失败的 openid 列表（仅在 <c>45171</c> 等场景下由官方下发；其余场景为 <c>null</c>）。</summary>
    [JsonPropertyName("fail_openid_list")]
    public List<string>? FailOpenIdList { get; set; }
}
