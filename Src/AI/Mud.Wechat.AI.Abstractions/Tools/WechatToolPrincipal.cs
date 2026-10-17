// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.AI.Abstractions.Tools;

/// <summary>
/// 微信生态工具身份轴（跨七线同源）：把「谁在调工具」归一为四条正交轴。
/// </summary>
/// <remarks>
/// <para>
/// <b>四条轴的语义</b>：
/// <list type="bullet">
/// <item><description><see cref="AppKey"/>：应用维度标识（多应用注册表键；企微/公众号/小店/开放平台各线同义）；</description></item>
/// <item><description><see cref="AuthCorpId"/>：授权方（企业）CorpId —— <b>仅第三方/服务商代开发场景出现</b>（企微专属轴，企业级令牌一企一份的 scope 键）；</description></item>
/// <item><description><see cref="OpenId"/> / <see cref="UnionId"/>：用户维度标识（公众号/小程序/小店线的用户主轴；企微侧为 userid/open_userid 的宿主归一结果）。</description></item>
/// </list>
/// </para>
/// <para>
/// <b>为何是值对象而非接口</b>：身份轴是「结构化事实」的载体，各线在事件层从本线信封抽取后填入；
/// 中立回合管线只承载中性 <c>TenantKey</c> 等维度（Mud.AI 侧），本类型是微信侧把这些维度
/// <b>还原回生态词汇</b>的唯一归一口。
/// </para>
/// <para>
/// <b>绝不承载凭据</b>：<c>corpsecret</c> / <c>permanent_code</c> / 模型 API Key <b>不得</b>进入本类型
/// （凭据只活在令牌管理器与配置面；永久授权码由 <c>CorpTokenManager</c> 按 <see cref="AuthCorpId"/> 自行解析）。
/// </para>
/// <para>
/// <b>netstandard2.0 纪律</b>：禁 <c>record</c> / <c>init</c> —— 以 sealed class + 只读属性表达值语义。
/// </para>
/// </remarks>
public sealed class WechatToolPrincipal
{
    /// <summary>创建工具身份轴（四轴均可空；空值 = 该维度未知，由消费方按缺省语义处理）。</summary>
    /// <param name="appKey">应用维度标识（多应用注册表键）。</param>
    /// <param name="authCorpId">授权方（企业）CorpId（仅第三方/代开发；空 = 自建应用，无企业级 scope）。</param>
    /// <param name="openId">用户开放 id（公众号/小程序/小店语义）。</param>
    /// <param name="unionId">用户统一 id（跨应用同主体）。</param>
    public WechatToolPrincipal(string? appKey, string? authCorpId = null, string? openId = null, string? unionId = null)
    {
        // 空白即「未声明」：归一为 null，避免下游把 "  " 当成有效 appKey 去查注册表（空键会静默落到默认应用）。
        AppKey = string.IsNullOrWhiteSpace(appKey) ? null : appKey;
        AuthCorpId = string.IsNullOrWhiteSpace(authCorpId) ? null : authCorpId;
        OpenId = string.IsNullOrWhiteSpace(openId) ? null : openId;
        UnionId = string.IsNullOrWhiteSpace(unionId) ? null : unionId;
    }

    /// <summary>应用维度标识（多应用注册表键；<see langword="null"/> = 未声明归属，由消费方回落默认应用）。</summary>
    public string? AppKey { get; }

    /// <summary>授权方（企业）CorpId（<see langword="null"/> = 自建应用 / 未知；企业级令牌 scope 的唯一来源）。</summary>
    public string? AuthCorpId { get; }

    /// <summary>用户开放 id（可空）。</summary>
    public string? OpenId { get; }

    /// <summary>用户统一 id（可空）。</summary>
    public string? UnionId { get; }

    /// <summary>是否携带企业级 scope（<see cref="AuthCorpId"/> 非空 ⇒ 工具执行须经 <c>UseCorpScope</c> 类入口切换租户）。</summary>
    public bool HasCorpScope => AuthCorpId != null;

    /// <summary>空身份轴（四轴全未知；用于「无身份上下文」的显式表达，避免以 null 承载）。</summary>
    public static WechatToolPrincipal Empty { get; } = new WechatToolPrincipal(null);
}
