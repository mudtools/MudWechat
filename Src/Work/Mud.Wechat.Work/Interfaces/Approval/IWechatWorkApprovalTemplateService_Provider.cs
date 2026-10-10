// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Approval;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「审批」模块审批模板域服务商代开发 SDK。
/// <para>
/// 官方对代开发应用开放与三类应用公共面一致的「获取审批模板详情」端点（继承自
/// <see cref="IWechatWorkApprovalTemplateService"/>），并额外开放 2 个差异端点：
/// 「创建审批模板」与「更新审批模板」（官方权限表对第三方应用标注暂不支持；
/// 自建应用亦开放，见 <see cref="IWechatWorkInternalApprovalTemplateService"/>）。
/// </para>
/// <para>企业自建应用见 <see cref="IWechatWorkInternalApprovalTemplateService"/>；
/// 第三方应用见 <see cref="IWechatWorkThirdPartyApprovalTemplateService"/>。</para>
/// </summary>
/// <remarks>
/// <para>
/// 消费代开发授权企业级 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，scope = authCorpId），
/// 须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。官方权限口径：代开发应用须具有「审批」权限。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Approval",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkApprovalTemplateService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.CorpAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkProviderApprovalTemplateService : IWechatWorkApprovalTemplateService
{
    /// <summary>
    /// 创建审批模板
    /// <para>创建审批模板；创建新模板后，管理后台及审批应用内将生成对应模板，并生效默认流程和规则配置。</para>
    /// <para>官方限制：接口调用频率限制为 600 次/分钟；模版名称不得和现有模版名称重复且长度不得超过 40 个字符；
    /// 控件名称不得和现有控件名称重复且长度不得超过 40 个字符（Attendance-外出/出差/加班控件 title 固定为外出/出差/加班，暂不支持自定义）；
    /// 控件说明长度不得超过 80 个字符；一个模版中只能拥有一类假勤控件类型（Vacation-假期；Attendance-外出/出差/加班 均为假勤控件类型）；
    /// 明细控件不能为空数组且明细中不能设置假勤控件、明细控件；当模板的控件为必填属性时，表单中对应的控件必须有值。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="CreateApprovalTemplateRequest"/>：template_name / template_content）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>模版创建成功后返回的模版id（template_id）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97437"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97439"/></para>
    /// <para>官方权限：自建应用须配置到「审批 - 可调用接口的应用」中；代开发应用须具有「审批」权限；官方权限表对第三方应用标注暂不支持。</para>
    /// </remarks>
    [Post("/cgi-bin/oa/approval/create_template")]
    Task<CreateApprovalTemplateResponse> CreateTemplateAsync(
        [Body] CreateApprovalTemplateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新审批模板
    /// <para>更新审批模板；更新模板后，管理后台及审批应用内将更新原模板的内容，已配置的审批流程和规则不变。</para>
    /// <para>官方限制：仅能更新自身应用模板；接口调用频率限制为 600 次/分钟；
    /// 模板已配置自定义打印格式时不支持 API 修改模板（错误码 301115）；
    /// 模版名称与控件名称的重复性、长度限制同创建审批模板。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="UpdateApprovalTemplateRequest"/>：template_id / template_name / template_content）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97438"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97440"/></para>
    /// <para>官方权限：自建应用须配置到「审批 - 可调用接口的应用」中；代开发应用须具有「审批」权限；官方权限表对第三方应用标注暂不支持。</para>
    /// </remarks>
    [Post("/cgi-bin/oa/approval/update_template")]
    Task<WechatWorkResponse> UpdateTemplateAsync(
        [Body] UpdateApprovalTemplateRequest request,
        CancellationToken cancellationToken = default);
}
