// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Pay;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「企业支付」模块收款商户号管理域企业自建应用 SDK
/// （查询商户号详情 + 设置商户号使用范围）。
/// <para>
/// 官方仅向自建应用开放本域 2 个端点（代开发应用与第三方应用均暂不支持），
/// 全部声明于本接口（形态对齐 <see cref="IWechatWorkInternalKfKnowledgeService"/>）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>）。官方权限口径：
/// 自建应用须配置到「对外收款 - 可调用接口的应用」中；范围设置仅对已绑定商户号生效。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Pay",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkPayMerchantService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.InternalAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalPayMerchantService : IWechatWorkPayMerchantService
{
    /// <summary>
    /// 查询商户号详情
    /// <para>查询企业已绑定的微信支付商户号信息及使用范围。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetPayMerchantRequest"/>：mch_id，不超过 32 字节）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>商户号信息（mch_id / merchant_name / bind_status：1 申请中、2 已绑定、3 已撤销）
    /// 与使用范围（allow_use_scope，仅已绑定时返回）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93666"/></para>
    /// <para>官方业务限制：代开发应用、第三方应用均暂不支持本接口。</para>
    /// </remarks>
    [Post("/cgi-bin/externalpay/getmerchant")]
    Task<GetPayMerchantResponse> GetMerchantAsync(
        [Body] GetPayMerchantRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置商户号使用范围
    /// <para>设置企业已绑定微信支付商户号的使用范围，支持按成员、部门或标签设置。</para>
    /// </summary>
    /// <param name="request">设置请求体（<see cref="SetPayMerchantUseScopeRequest"/>：
    /// mch_id + allow_use_scope（user / partyid / tagid））。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>设置结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93666"/></para>
    /// <para>官方业务限制：代开发应用、第三方应用均暂不支持本接口；范围设置仅对已绑定商户号生效。</para>
    /// </remarks>
    [Post("/cgi-bin/externalpay/set_mch_use_scope")]
    Task<WechatWorkResponse> SetMerchantUseScopeAsync(
        [Body] SetPayMerchantUseScopeRequest request,
        CancellationToken cancellationToken = default);
}
