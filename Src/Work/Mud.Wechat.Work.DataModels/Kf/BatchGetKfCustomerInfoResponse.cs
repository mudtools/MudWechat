// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Kf;

/// <summary>
/// 批量获取客户基础信息响应体（<c>/cgi-bin/kf/customer/batchget</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class BatchGetKfCustomerInfoResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置客户基础信息列表。
    /// </summary>
    [JsonPropertyName("customer_list")]
    public List<KfCustomerInfo>? CustomerList { get; set; }

    /// <summary>
    /// 获取或设置无效的 external_userid 列表
    /// （非最近 48 小时内触发过「用户进入会话事件」或发过消息的客户）。
    /// </summary>
    [JsonPropertyName("invalid_external_userid")]
    public List<string>? InvalidExternalUserId { get; set; }
}
