// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Kf;

/// <summary>
/// 批量获取客户基础信息请求体（<c>/cgi-bin/kf/customer/batchget</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class BatchGetKfCustomerInfoRequest
{
    /// <summary>
    /// 获取或设置 external_userid 列表（官方必填，可填 1~100 个，超过 100 个需分批调用）。
    /// <para>
    /// 须为最近 48 小时内触发过「用户进入会话事件」或向该客服账号发过消息的客户，
    /// 否则将落入响应的 invalid_external_userid 列表。
    /// </para>
    /// </summary>
    [JsonPropertyName("external_userid_list")]
    public List<string>? ExternalUserIdList { get; set; }

    /// <summary>
    /// 获取或设置是否返回客户 48 小时内最后一次进入会话的上下文：0 - 不返回（默认），1 - 返回。
    /// </summary>
    [JsonPropertyName("need_enter_session_context")]
    public int? NeedEnterSessionContext { get; set; }
}
