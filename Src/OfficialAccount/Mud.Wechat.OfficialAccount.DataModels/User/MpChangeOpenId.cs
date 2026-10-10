// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.User;

/// <summary>
/// 转换 openid 请求体（<c>changeopenid</c>，<c>POST /cgi-bin/changeopenid</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表：<c>from_appid</c>（必填，<b>原账号的原始 id，不是 appid</b>，示例 <c>gh_91ae50dfeb1c</c>）
/// 与 <c>openid_list</c>（必填，第 1 步中拉取的原账号用户列表，
/// <b>必须是旧账号目前仍关注的用户</b>，否则会出错；<b>一次最多 100 个</b>）。
/// </para>
/// <para><b>官方无 <c>to_appid</c> 字段</b>（目标账号由调用方令牌身份隐含）⇒ SDK 不建模。</para>
/// <para>
/// <b>时效与前置条件（官方「注意事项」原文）</b>：①原账号为<b>个人主体</b>的不支持使用该接口；
/// ②必须在<b>原账号被冻结之前</b>（最好在提交审核前）获取原账号用户列表，否则转换工具不可用；
/// ③可在<b>账号迁移审核完成后</b>开始调用，<b>最多保留 15 天</b>——迁移未完成时调用无返回结果或报错，
/// 15 天后接口<b>失效</b>、无法拉取数据。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "User")]
public class MpChangeOpenIdRequest
{
    /// <summary>原账号的<b>原始 id</b>（<c>gh_</c> 开头，不是 appid）。</summary>
    [JsonPropertyName("from_appid")]
    public string FromAppId { get; set; } = string.Empty;

    /// <summary>原账号用户 openid 列表（<b>单次最多 100 个</b>；必须是旧账号目前仍关注的用户）。</summary>
    [JsonPropertyName("openid_list")]
    public List<string> OpenIdList { get; set; } = new();
}

/// <summary>
/// 转换 openid 响应（<c>changeopenid</c>）。
/// </summary>
/// <remarks>
/// 官方响应：<c>result_list</c>（openid 结果列表）+ <c>errcode</c>/<c>errmsg</c>。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "User")]
public class MpChangeOpenIdResponse : MpResponse
{
    /// <summary>openid 结果列表（成功时逐项给出新 openid 或该项错误）。</summary>
    [JsonPropertyName("result_list")]
    public List<MpChangeOpenIdResult>? ResultList { get; set; }
}

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
