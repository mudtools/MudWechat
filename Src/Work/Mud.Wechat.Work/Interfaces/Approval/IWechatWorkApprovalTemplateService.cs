// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Approval;

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「审批」模块审批模板域公共 SDK（获取审批模板详情，公共端点收敛面）。
/// <para>
/// 官方对三类应用开放一致的「获取审批模板详情」端点，收敛声明于本接口；应用类型子接口承载官方开放面差异端点：
/// 企业自建应用见 <see cref="IWechatWorkInternalApprovalTemplateService"/>（额外开放创建/更新审批模板），
/// 服务商代开发见 <see cref="IWechatWorkProviderApprovalTemplateService"/>（额外开放创建/更新审批模板），
/// 第三方应用见 <see cref="IWechatWorkThirdPartyApprovalTemplateService"/>（额外开放复制/更新模板到企业，
/// 官方对第三方标注创建/更新模板暂不支持）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 令牌路由键说明：三类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>）——
/// 自建应用消费应用自身 access_token；第三方/服务商代开发消费授权企业级 access_token（scope = authCorpId），
/// 须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。官方权限口径：自建应用须配置到
/// 「审批 - 可调用接口的应用」中；代开发应用与第三方应用须具有「审批」权限。
/// 自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口（存量企业暂不受影响）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkApprovalTemplateService
{
    /// <summary>
    /// 获取审批模板详情
    /// <para>按模板 id 获取审批模板详情（模板名称与模板控件数组，控件含基础属性与控件配置）。</para>
    /// <para>官方限制：接口调用频率限制为 600 次/分钟。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetApprovalTemplateDetailRequest"/>：template_id 官方必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>模板名称（template_names）与模板控件信息（template_content：controls，含 property / config）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91982"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/92631"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96506"/></para>
    /// <para>官方权限与可见范围：第三方应用可以获取第三方应用添加的模板详情；自建应用的 Secret 可获取企业自建模板的模板详情。</para>
    /// </remarks>
    [Post("/cgi-bin/oa/gettemplatedetail")]
    Task<GetApprovalTemplateDetailResponse> GetTemplateDetailAsync(
        [Body] GetApprovalTemplateDetailRequest request,
        CancellationToken cancellationToken = default);
}
