// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Kf;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「微信客服」模块客户基础信息域公共 SDK（批量获取客户基础信息）。
/// <para>
/// 官方对三类应用开放完全一致的 1 个端点，收敛声明于本接口；应用类型子接口均为零差异端点空标记：
/// 自建应用见 <see cref="IWechatWorkInternalKfCustomerService"/>，
/// 第三方应用见 <see cref="IWechatWorkThirdPartyKfCustomerService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderKfCustomerService"/>。
/// </para>
/// <para>
/// 官方「其他基础信息获取」目录下的「获取企业状态信息」（第三方 95153）为概述页、无独立 API
/// （其内容归属应用授权「获取企业授权信息」接口），不落入本域接口面。
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
/// 第三方 / 代开发应用须具有「微信客服-&gt;获取基础信息」权限。
/// 官方约束：查询对象须为最近 48 小时内触发过「用户进入会话事件」或向客服账号发过消息的客户；
/// 头像 / 性别 / unionid 对第三方与代开发应用受限；「营销获客」应用仅能获取其自身带来的客户。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkKfCustomerService
{
    /// <summary>
    /// 获取客户基础信息
    /// <para>批量获取微信客户的基础信息（昵称、头像等）与 48 小时内最后一次进入会话的上下文；
    /// external_userid_list 可填 1~100 个，超过 100 个需分批调用。</para>
    /// <para>第三方 / 代开发应用不可获取头像，性别统一返回 0，unionid 不可获取。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="BatchGetKfCustomerInfoRequest"/>：external_userid_list / need_enter_session_context）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>客户基础信息列表（customer_list）与无效 external_userid 列表（invalid_external_userid）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95159"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95149"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96429"/></para>
    /// </remarks>
    [Post("/cgi-bin/kf/customer/batchget")]
    Task<BatchGetKfCustomerInfoResponse> BatchGetCustomerInfoAsync(
        [Body] BatchGetKfCustomerInfoRequest request,
        CancellationToken cancellationToken = default);
}
