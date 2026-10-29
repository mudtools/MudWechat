// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.User;

/// <summary>
/// 单条 openid 转换结果（<c>result_list</c> 元素）。
/// </summary>
/// <remarks>
/// 官方字段：<c>ori_openid</c>（旧 openid）/ <c>new_openid</c>（新 openid）/
/// <c>err_msg</c>（错误描述，成功为 <c>ok</c>；<c>"ori_openid error"</c> 表示该 openid 目前没有关注旧账号）。
/// <para>
/// <b>逐项错误语义</b>：整包 <c>errcode</c> 为 0 时仍可能有单项失败 ⇒ 调用方必须逐项检查
/// <see cref="ErrMsg"/>，不得据整包成功推断全部转换成功。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "User")]
public class MpChangeOpenIdResult
{
    /// <summary>旧 openid。</summary>
    [JsonPropertyName("ori_openid")]
    public string? OriOpenId { get; set; }

    /// <summary>新 openid（该项失败时可能缺省）。</summary>
    [JsonPropertyName("new_openid")]
    public string? NewOpenId { get; set; }

    /// <summary>错误描述（成功为 <c>ok</c>；<c>ori_openid error</c> 表示该 openid 目前没有关注旧账号）。</summary>
    [JsonPropertyName("err_msg")]
    public string? ErrMsg { get; set; }
}
