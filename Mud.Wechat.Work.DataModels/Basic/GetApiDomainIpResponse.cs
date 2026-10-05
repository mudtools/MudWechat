// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Basic;

/// <summary>
/// 获取企业微信接口IP段响应体（<c>/cgi-bin/get_api_domain_ip</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方参数表将 <c>ip_list</c> 标注为 StringArray；失败示例（如 <c>access_token</c> 过期的 42001）
/// 返回的是<b>空数组</b>而非缺省字段，故以非空集合承载——调用方遍历前无需判空。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Basic")]
public class GetApiDomainIpResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置企业微信服务器 IP 段列表
    /// <para>即 <c>qyapi.weixin.qq.com</c>（开发者调用企业微信端的接入 API 域名）的全部解析地址，
    /// 用于企业侧防火墙放行配置。</para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// 官方注意事项：IP 段有变更可能，当 IP 段变更时，新旧 IP 段会同时保留一段时间；
    /// 建议企业每天定时拉取 IP 段、更新防火墙设置，避免因 IP 段变更导致网络不通。
    /// </para>
    /// </remarks>
    [JsonPropertyName("ip_list")]
    public List<string> IpList { get; set; } = new();
}
