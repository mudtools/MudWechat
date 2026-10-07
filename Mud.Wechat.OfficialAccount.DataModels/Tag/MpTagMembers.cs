// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Tag;

/// <summary>
/// 批量打标签 / 取消标签请求体（<c>batchTagging</c> 与 <c>batchUntagging</c> <b>共用</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何两端点共用同一 DTO</b>：官方两页的请求体字段表<b>逐字段一致</b>（<c>openid_list</c> + <c>tagid</c>），
/// 差异仅在路径与语义（打标 / 取消）⇒ 拆成两个同构类属纯冗余。响应体也一致，故
/// <see cref="MpTagMembersResponse"/> 同样共用。
/// </para>
/// <para>
/// <b>数量上限（两页声明不一致，按各自语义分别约束）</b>：<c>batchTagging</c> 页明确「<c>openid_list</c>
/// 粉丝 openid 列表，<b>最多 50 个</b>」；<c>batchUntagging</c> 页字段表<b>未给出</b>数量上限，仅在错误码中列
/// <c>40032</c>（不合法的 openid 列表长度）。调用方对取消标签应按「不超过打标侧同值」自律，
/// 但不得把 50 写成官方对取消标签的明文约束。
/// </para>
/// <para>
/// <b>单用户标签上限</b>：<c>batchTagging</c> 页「注意事项」原文为「标签功能目前支持公众号为用户打上
/// 最多 20 个标签」（超限错误码 <c>45059</c>）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Tag")]
public class MpTagMembersRequest
{
    /// <summary>粉丝 openid 列表（打标签侧官方上限 50 个/次）。</summary>
    [JsonPropertyName("openid_list")]
    public List<string> OpenIdList { get; set; } = new();

    /// <summary>标签 id。</summary>
    [JsonPropertyName("tagid")]
    public int TagId { get; set; }
}

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
