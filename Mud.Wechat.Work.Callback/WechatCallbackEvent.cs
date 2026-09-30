// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Callback;

/// <summary>
/// 企业微信回调事件（解密后的结构化结果）。
/// </summary>
/// <remarks>
/// 事件组织对应 Mud.Feishu.EventCallback 职能；v1 覆盖授权变更事件族
/// （suite_ticket / change_auth / cancel_auth / contact_sync 等按 InfoType 原样透出）。
/// </remarks>
public class WechatCallbackEvent
{
    /// <summary>时间戳（URL 查询参数，验签用）。</summary>
    public string? TimeStamp { get; set; }

    /// <summary>随机数（URL 查询参数，验签用）。</summary>
    public string? Nonce { get; set; }

    /// <summary>事件类型（解密后 XML 的 InfoType 节点：suite_ticket / change_auth / cancel_auth / create_auth 等）。</summary>
    public string? InfoType { get; set; }

    /// <summary>服务商 SuiteId（SuiteId 节点）。</summary>
    public string? SuiteId { get; set; }

    /// <summary>最新推送的 suite_ticket（InfoType = suite_ticket 时非空）。</summary>
    public string? SuiteTicket { get; set; }

    /// <summary>授权方（企业）CorpId（AuthCorpId 节点；部分事件模板中为 FromUserName）。</summary>
    public string? AuthCorpId { get; set; }

    /// <summary>临时授权码（InfoType = create_auth 时非空，用于 get_permanent_code 换取永久授权码）。</summary>
    public string? AuthCode { get; set; }

    /// <summary>解密后的原始 XML 明文（供业务侧解析扩展字段）。</summary>
    public string? DecryptedXml { get; set; }

    /// <summary>是否为 suite_ticket 推送事件。</summary>
    public bool IsSuiteTicket => string.Equals(InfoType, "suite_ticket", StringComparison.Ordinal);

    /// <summary>是否为授权变更事件（change_auth）。</summary>
    public bool IsChangeAuth => string.Equals(InfoType, "change_auth", StringComparison.Ordinal);

    /// <summary>是否为取消授权事件（cancel_auth / del_auth）。</summary>
    public bool IsCancelAuth =>
        string.Equals(InfoType, "cancel_auth", StringComparison.Ordinal) ||
        string.Equals(InfoType, "del_auth", StringComparison.Ordinal);
}
