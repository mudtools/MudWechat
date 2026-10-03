// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.AcquisitionComponent;

/// <summary>
/// 获客助手组件获取组件授权信息响应体（<c>/cgi-bin/externalcontact/customer_acquisition/get_comp_auth_info</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "AcquisitionComponent")]
public class GetAcquisitionComponentAuthInfoResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置付费模式（返回值 0：扣企业费用；返回值 1：扣服务商费用，即服务商代支付模式）。
    /// </summary>
    [JsonPropertyName("pay_mode")]
    public int? PayMode { get; set; }

    /// <summary>
    /// 获取或设置获客链接代付单价，单位分（<see cref="PayMode"/> 为 1 时返回）。
    /// </summary>
    [JsonPropertyName("price")]
    public long? Price { get; set; }

    /// <summary>
    /// 获取或设置链接授权模式（返回值 0：企业选择链接授权；返回值 1：关联应用创建的链接自动授权）。
    /// </summary>
    [JsonPropertyName("link_auth_mode")]
    public int? LinkAuthMode { get; set; }

    /// <summary>
    /// 获取或设置授权的应用列表。
    /// </summary>
    [JsonPropertyName("auth_apps")]
    public List<AcquisitionComponentAuthApp>? AuthApps { get; set; }
}

/// <summary>
/// 获客助手组件授权信息中的授权应用条目。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "AcquisitionComponent")]
public class AcquisitionComponentAuthApp
{
    /// <summary>
    /// 获取或设置授权的应用 agentid。
    /// </summary>
    [JsonPropertyName("agentid")]
    public int? AgentId { get; set; }
}
