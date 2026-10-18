// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Service;

/// <summary>
/// 「获取服务商异步批量任务结果」响应体（<c>/cgi-bin/service/batch/getresult</c>，官方 91882）。
/// </summary>
/// <remarks>
/// <para>
/// <b>与自建应用侧同名端点的区别</b>：本端点走 <c>provider_access_token</c>，官方响应字段为
/// <c>status</c> / <c>type</c> / <c>result</c> / <c>url</c>，<b>不含</b>自建侧的
/// <c>total</c> / <c>percentage</c> / <c>remaintime</c> ⇒ 不复用 <c>GetBatchJobResultResponse</c>。
/// </para>
/// <para>
/// <b>结果 url 的安全语义</b>：<c>url</c>（含 <c>result.contact_id_translate.url</c>）为
/// 「通讯录转换成功后的下载地址」，官方要求须经用户 oauth2 授权登录或单点登录在<b>用户侧</b>打开、
/// 且<b>仅 24 小时有效</b> —— 属带身份的临时资源链接，不得进日志或对外持久化。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Service")]
public class GetServiceBatchJobResultResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置任务状态（<c>status</c>）：1 任务开始、2 任务进行中、3 任务已完成。
    /// </summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>
    /// 获取或设置操作类型（<c>type</c>）：目前有 <c>contact_id_translate</c>（通讯录 id 转译）。
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// 获取或设置详细的处理结果（<c>result</c>；官方语义为「任务完成后此字段有效」）。
    /// </summary>
    [JsonPropertyName("result")]
    public ServiceBatchJobResultDetail? Result { get; set; }

    /// <summary>
    /// 获取或设置通讯录转换成功后的 url（<c>url</c>）。
    /// <para>官方要求经用户 oauth2 授权登录或单点登录在用户侧打开，且该 url <b>24 小时有效</b>。</para>
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

/// <summary>
/// 异步任务处理结果（<c>result</c>，官方 91882）。
/// </summary>
/// <remarks>按 <c>type</c> 取对应子结构；官方现行样本仅出现 <c>contact_id_translate</c> 一个键。</remarks>
[HttpJsonSerializable(SerializerClassName = "Service")]
public class ServiceBatchJobResultDetail
{
    /// <summary>
    /// 获取或设置通讯录 id 转译结果（<c>contact_id_translate</c>）。
    /// </summary>
    [JsonPropertyName("contact_id_translate")]
    public ServiceContactIdTranslateResult? ContactIdTranslate { get; set; }
}

/// <summary>
/// 通讯录 id 转译结果（<c>result.contact_id_translate</c>，官方 91882）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Service")]
public class ServiceContactIdTranslateResult
{
    /// <summary>
    /// 获取或设置转译产物下载地址（<c>url</c>，24 小时有效、须用户侧授权打开，不得进日志）。
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}
