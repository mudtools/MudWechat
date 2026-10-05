// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Wedrive;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「微盘」模块高级功能账号管理域企业自建应用 SDK
/// （分配高级功能账号 + 取消高级功能账号 + 获取高级功能账号列表）。
/// <para>
/// 官方「高级功能账号管理」3 个端点官方仅向企业自建应用开放（代开发应用与第三方应用均标注「暂不支持」），
/// 零端点父接口 <see cref="IWechatWorkWedriveVipService"/> + 本接口承载全部端点，不设代开发/第三方子接口。
/// </para>
/// <para>官方契约陷阱：本族路由挂 <c>/cgi-bin/wedrive/vip/</c> 段（区别于族内其余端点的
/// <c>/cgi-bin/wedrive/</c> 直挂段）；账号列表分页用 cursor + limit（has_more / next_cursor），
/// 官方明确「不保证每次返回的数据刚好为指定 limit，必须用返回的 has_more 判断是否继续请求」。</para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，Query 注入 <c>access_token</c>）。
/// 官方权限口径：自建应用需配置到「协作 - 微盘 - 可调用接口的应用」中；
/// 代开发应用与第三方应用官方标注「暂不支持」。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Wedrive",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkWedriveVipService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.InternalAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalWedriveVipService : IWechatWorkWedriveVipService
{
    /// <summary>
    /// 分配高级功能账号
    /// <para>为企业成员分配微盘高级功能账号（已经拥有高级功能账号的成员计入成功列表）。</para>
    /// <para>官方限制：userid_list 单次操作最大限制 100 个。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="BatchAddWedriveVipRequest"/>：userid_list 官方必填，单次最多 100 个）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>分配成功的 userid 列表（succ_userid_list，含已是高级功能账号的 userid）与分配失败的 userid 列表（fail_userid_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99512"/></para>
    /// <para>官方权限：自建应用需配置到「协作 - 微盘 - 可调用接口的应用」中；代开发应用与第三方应用官方标注「暂不支持」。</para>
    /// </remarks>
    [Post("/cgi-bin/wedrive/vip/batch_add")]
    Task<BatchAddWedriveVipResponse> BatchAddVipAsync(
        [Body] BatchAddWedriveVipRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消高级功能账号
    /// <para>撤销分配应用可见范围企业成员的高级功能账号。</para>
    /// <para>官方限制：userid_list 单次操作最多限制 100 个。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="BatchDelWedriveVipRequest"/>：userid_list 官方必填，单次最多 100 个）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>撤销分配成功的 userid 列表（succ_userid_list）与撤销分配失败的 userid 列表（fail_userid_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99513"/></para>
    /// <para>官方权限：自建应用需配置到「协作 - 微盘 - 可调用接口的应用」中；代开发应用与第三方应用官方标注「暂不支持」。</para>
    /// </remarks>
    [Post("/cgi-bin/wedrive/vip/batch_del")]
    Task<BatchDelWedriveVipResponse> BatchDelVipAsync(
        [Body] BatchDelWedriveVipRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取高级功能账号列表
    /// <para>查询企业已分配高级功能且在应用可见范围的账号列表（cursor + limit 分页）。</para>
    /// <para>官方限制：limit 默认 100、最大 200；官方明确「不保证每次返回的数据刚好为指定 limit，
    /// 必须用返回的 has_more 判断是否继续请求」。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="ListWedriveVipRequest"/>：cursor / limit，均官方选填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>翻页标记（has_more / next_cursor）与已分配高级功能的成员 userid 列表（userid_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99514"/></para>
    /// <para>官方权限：自建应用需配置到「协作 - 微盘 - 可调用接口的应用」中；代开发应用与第三方应用官方标注「暂不支持」。</para>
    /// </remarks>
    [Post("/cgi-bin/wedrive/vip/list")]
    Task<ListWedriveVipResponse> ListVipAsync(
        [Body] ListWedriveVipRequest request,
        CancellationToken cancellationToken = default);
}
