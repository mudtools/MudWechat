// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Kf;

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「微信客服」模块「升级服务」配置域公共 SDK
/// （获取配置的专员与客户群 + 为客户升级为专员或客户群服务 + 为客户取消推荐）。
/// <para>
/// 官方对三类应用开放完全一致的 3 个端点，全部收敛声明于本接口；应用类型子接口均为零差异端点空标记：
/// 自建应用见 <see cref="IWechatWorkInternalKfUpgradeService"/>，
/// 第三方应用见 <see cref="IWechatWorkThirdPartyKfUpgradeService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderKfUpgradeService"/>。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 三类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>）：
/// 企业自建为应用自身 access_token；第三方 / 代开发为授权企业级 access_token（scope = authCorpId），
/// 由多应用基座按当前应用上下文路由——企业级令牌须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。
/// </para>
/// <para>
/// 权限口径：自建应用须配置到「微信客服-可调用接口的应用」中；
/// 第三方 / 代开发应用须具有「微信客服-&gt;服务工具-&gt;配置『升级服务』」权限。
/// </para>
/// <para>
/// 官方约束：升级接口指定的 userid / chat_id 必须已配置在微信客服「升级服务」中专员服务或客户群服务，
/// 否则官方返回 95021 错误码，且 userid 须在「客户联系-&gt;权限配置-&gt;客户联系和客户群」使用范围内；
/// 三个端点均要求客服账号的接待人员与 userid/chatid 对应群主在应用的可见范围内；
/// 通过 API 指定后接待人员端会出现状态提示，取消推荐后提示同步消失；
/// 自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口（存量企业暂不受影响）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkKfUpgradeService
{
    /// <summary>
    /// 获取配置的专员与客户群
    /// <para>获取企业在「微信客服」-「升级服务」中配置的专员与客户群范围；
    /// 升级接口仅可从该已配置范围中选取（否则官方返回 95021 错误码）。</para>
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>专员服务配置范围（member_range）与客户群配置范围（groupchat_range）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94674"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94702"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96422"/></para>
    /// </remarks>
    [Get("/cgi-bin/kf/customer/get_upgrade_service_config")]
    Task<GetKfUpgradeServiceConfigResponse> GetUpgradeServiceConfigAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 为客户升级为专员或客户群服务
    /// <para>为微信客户推荐升级为专员服务（type = 1，填 member）或客户群服务（type = 2，填 groupchat）；
    /// 指定的 userid / chat_id 必须已配置在微信客服「升级服务」中，否则官方返回 95021 错误码。</para>
    /// <para>通过 API 指定后，接待人员端会出现特殊状态提示；取消推荐见「为客户取消推荐」。</para>
    /// </summary>
    /// <param name="request">升级请求体（<see cref="UpgradeKfServiceRequest"/>：open_kfid / external_userid / type / member / groupchat）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>升级结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94674"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94702"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96422"/></para>
    /// </remarks>
    [Post("/cgi-bin/kf/customer/upgrade_service")]
    Task<WechatWorkResponse> UpgradeServiceAsync(
        [Body] UpgradeKfServiceRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 为客户取消推荐
    /// <para>取消为客户推荐的升级服务（专员或客户群），接待人员端的状态提示同步消失。</para>
    /// </summary>
    /// <param name="request">取消请求体（<see cref="CancelKfUpgradeServiceRequest"/>：open_kfid / external_userid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>取消结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94674"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94702"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96422"/></para>
    /// </remarks>
    [Post("/cgi-bin/kf/customer/cancel_upgrade_service")]
    Task<WechatWorkResponse> CancelUpgradeServiceAsync(
        [Body] CancelKfUpgradeServiceRequest request,
        CancellationToken cancellationToken = default);
}
