// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Mail;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「邮件」模块高级功能账号族企业自建应用 SDK
/// （分配高级功能账号 + 取消高级功能账号 + 获取高级功能账号列表）。
/// <para>
/// 官方仅向企业自建应用开放本域 3 个端点，全部收敛声明于本接口
/// （形态对齐 <see cref="IWechatWorkInternalSecurityVipService"/> 仅自建子接口承载）。
/// 官方未向第三方应用与服务商代开发应用开放本域，故本家族不声明对应应用类型子接口
/// （能力漂移守卫：继承链上恰好只有自建子接口）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>）。官方权限口径：
/// 需使用配置到「可调用应用」列表中的应用 secret 所获取的 access_token 调用。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Mail",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkMailVipService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalMailVipService : IWechatWorkMailVipService
{
    /// <summary>
    /// 分配高级功能账号
    /// <para>给应用可见范围内的企业成员分配邮件高级功能（同步批量，非异步任务）。</para>
    /// <para>官方业务限制：单次操作 userid_list 最大限制 100 个成员。</para>
    /// </summary>
    /// <param name="request">分配请求体（<see cref="BatchAddMailVipRequest"/>：userid_list 官方必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>分配成功的用户列表（succ_userid_list，包括之前已经分配过的用户）与分配失败的用户列表（fail_userid_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99316"/></para>
    /// </remarks>
    [Post("/cgi-bin/exmail/vip/batch_add")]
    Task<BatchAddMailVipResponse> BatchAddVipAsync(
        [Body] BatchAddMailVipRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消高级功能账号
    /// <para>撤销分配应用可见范围内的企业成员的邮件高级功能（同步批量，非异步任务）。</para>
    /// <para>官方业务限制：单次操作 userid_list 最多限制 100 个。</para>
    /// </summary>
    /// <param name="request">取消请求体（<see cref="BatchDelMailVipRequest"/>：userid_list 官方必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>撤销分配成功的用户列表（succ_userid_list）与撤销分配失败的用户列表（fail_userid_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99317"/></para>
    /// </remarks>
    [Post("/cgi-bin/exmail/vip/batch_del")]
    Task<BatchDelMailVipResponse> BatchDelVipAsync(
        [Body] BatchDelMailVipRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取高级功能账号列表
    /// <para>查询企业已分配高级功能且在应用可见范围的账号列表（cursor + has_more 翻页拉取）。</para>
    /// <para>官方业务限制：不保证每次返回的数据刚好为指定 limit，必须用返回的 has_more 判断是否继续请求。</para>
    /// </summary>
    /// <param name="request">分页请求体（<see cref="ListMailVipRequest"/>：cursor / limit 官方选填；limit 默认 100，最大 200）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>是否还有更多数据（has_more）、下一次请求游标（next_cursor）与成员 userid 列表（userid_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99318"/></para>
    /// <para>官方注明：除邮件外，安全、文档、会议、微盘等模块也有各自的「获取高级功能账号列表」接口（本 SDK
    /// 安全管理域见 <see cref="IWechatWorkInternalSecurityVipService"/>），各域路由互不相通。</para>
    /// </remarks>
    [Post("/cgi-bin/exmail/vip/list")]
    Task<ListMailVipResponse> ListVipAsync(
        [Body] ListMailVipRequest request,
        CancellationToken cancellationToken = default);
}
