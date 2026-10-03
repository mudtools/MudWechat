// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.AccountId;

/// <summary>
/// external_userid 查询 pending_id 请求体
/// （<c>/cgi-bin/idconvert/batch/external_userid_to_pending_id</c>）：将外部联系人 id 批量转换为
/// 临时外部联系人 ID（pending_id），以打通 unionid = pending_id = external_userid 的映射关系。
/// </summary>
/// <remarks>
/// 官方限制：<b>请求体数组字段名为 <c>external_userid</c>（无 <c>_list</c> 后缀，与其它转换接口不同）</b>，
/// 最多可同时查询 100 个外部联系人；仅认证企业可调用；客户跟进人或客户群群主须在应用可见范围内；
/// 必须曾通过 unionid 转换接口获取过该客户的 pending_id；pending_id 超过 90 天失效；
/// 传入 chat_id 时只检查群主是否在可见范围，同时会忽略在该群以外的 external_userid。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "AccountId")]
public class ConvertExternalUserIdToPendingIdRequest
{
    /// <summary>
    /// 获取或设置该企业的外部联系人 ID 列表（官方必填，最多可同时查询 100 个）。
    /// <para>官方契约陷阱：JSON 字段名为 <c>external_userid</c>（不带 <c>_list</c> 后缀）。</para>
    /// </summary>
    [JsonPropertyName("external_userid")]
    public List<string>? ExternalUserId { get; set; }

    /// <summary>
    /// 获取或设置客户群 ID（选填；传入则只检查群主是否在可见范围，并忽略该群以外的 external_userid）。
    /// </summary>
    [JsonPropertyName("chat_id")]
    public string? ChatId { get; set; }
}
