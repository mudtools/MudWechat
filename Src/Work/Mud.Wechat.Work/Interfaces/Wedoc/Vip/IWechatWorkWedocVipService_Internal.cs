// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Wedoc;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「文档」模块高级功能账号管理域企业自建应用 SDK（承载本族全部 3 端点）。
/// <para>
/// 官方「高级功能账号管理」3 个端点官方文档面仅出现在企业自建应用开发树
/// （第三方应用开发与服务商代开发树无对应页面），端点全部声明于本接口；
/// 继承链上不得出现代开发 / 第三方子接口（能力漂移守卫），父接口见 <see cref="IWechatWorkWedocVipService"/>。
/// </para>
/// <para>官方契约陷阱：本族路由挂 <c>/cgi-bin/wedoc/vip/</c> 段；账号列表分页以 <c>next_cursor</c> 续页，
/// 官方续页判据是「无更多数据时 next_cursor 返回空」，官方参数表<b>未</b>列出 has_more。</para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，Query 注入 <c>access_token</c>）。
/// </para>
/// <para>
/// 官方权限（三页同口径的权限表原文）：自建应用「配置到「协作 - 文档 - 可调用接口的应用」中」，
/// 代开发应用与第三方应用官方均标注「<b>暂不支持</b>」⇒ 本族只有自建子接口。
/// </para>
/// <para>
/// 官方频率口径：99516 / 99517 / 99518 三页<b>均无</b>独立的数值型频率限制，
/// 走官方「附录 - 访问频率限制」的默认阈值（每企业调用单个 cgi/api 不可超过 1 万次/分、15 万次/小时，
/// <see href="https://developer.work.weixin.qq.com/document/path/90312"/>）；
/// 本族各端点<b>不</b>写「接口调用频率限制为 N 次/分钟」——该空缺是官方正文口径，不得凭第三方 SDK 注释或记忆补数。
/// 官方给出的<b>规模</b>限制是逐端点的：批量分配/撤销单次上限 100 个成员，列表侧 limit 取值 1~1000（默认 100）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Wedoc",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkWedocVipService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.InternalAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalWedocVipService : IWechatWorkWedocVipService
{
    /// <summary>
    /// 分配高级功能账号
    /// <para>为企业成员分配文档高级功能账号。</para>
    /// <para>官方响应形态：成功与失败成员列表分别以 <c>succ_userid_list</c> / <c>fail_userid_list</c> 返回
    /// （官方字段名为 succ 而非 success），两者均为可选返回。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="BatchAddWedocVipRequest"/>：userid_list 官方必填，单次操作最大限制 100 个）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>分配成功的成员账号列表（succ_userid_list）与分配失败的成员账号列表（fail_userid_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99516"/></para>
    /// <para>官方权限：自建应用需配置到「协作 - 文档 - 可调用接口的应用」中，代开发应用与第三方应用官方均标注「暂不支持」
    /// （即仅企业自建应用可调用，第三方/代开发文档树无对应页面）。</para>
    /// <para>官方限制：本接口为 POST；<c>userid_list</c> 必填且<b>单次操作最大限制 100 个</b>；
    /// 官方口径「分配成功的列表<b>包括已经是高级功能账号的 userid</b>」，故 succ 列表不可理解为「本次新增」；
    /// 本接口用于分配<b>应用可见范围内</b>企业成员的高级功能。</para>
    /// <para>本页面未给出独立的数值型频率限制，走官方「附录 - 访问频率限制」的默认阈值（见本接口 <c>remarks</c> 汇总口径）。</para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/vip/batch_add")]
    Task<BatchAddWedocVipResponse> BatchAddVipAsync(
        [Body] BatchAddWedocVipRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消高级功能账号
    /// <para>撤销企业成员的文档高级功能账号。</para>
    /// <para>官方响应形态：与分配接口同构，成功与失败成员列表分别以 <c>succ_userid_list</c> / <c>fail_userid_list</c> 返回。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="BatchDelWedocVipRequest"/>：userid_list 官方必填，单次操作最多限制 100 个）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>撤销成功的成员账号列表（succ_userid_list）与撤销失败的成员账号列表（fail_userid_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99517"/></para>
    /// <para>官方权限：自建应用需配置到「协作 - 文档 - 可调用接口的应用」中，代开发应用与第三方应用官方均标注「暂不支持」
    /// （即仅企业自建应用可调用，第三方/代开发文档树无对应页面）。</para>
    /// <para>官方限制：本接口为 POST，与分配接口同构；<c>userid_list</c> 必填且<b>单次操作最多限制 100 个</b>；
    /// 官方口径为本接口撤销的是「分配<b>应用可见范围</b>企业成员的高级功能」。</para>
    /// <para>本页面未给出独立的数值型频率限制，走官方「附录 - 访问频率限制」的默认阈值（见本接口 <c>remarks</c> 汇总口径）。</para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/vip/batch_del")]
    Task<BatchDelWedocVipResponse> BatchDelVipAsync(
        [Body] BatchDelWedocVipRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取高级功能账号列表
    /// <para>分页查询已分配文档高级功能的成员账号列表（cursor + limit）。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="ListWedocVipRequest"/>：limit / cursor，均官方选填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成员账号列表（userid_list）与翻页标记（next_cursor，无更多数据时官方返回空）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99518"/></para>
    /// <para>官方权限：自建应用需配置到「协作 - 文档 - 可调用接口的应用」中，代开发应用与第三方应用官方均标注「暂不支持」
    /// （即仅企业自建应用可调用，第三方/代开发文档树无对应页面）。</para>
    /// <para>官方限制：本接口为 POST，以 <c>cursor</c> + <c>limit</c> 分页，两参数官方均为选填；<c>limit</c> 的官方语义是「每次返回条数最大值」
    /// 而非游标，取值 <b>1~1000</b>、默认 100；<c>cursor</c> 官方为字符串，「第一次请求传入空，之后填入上一次调用返回的 next_cursor」；
    /// 续页判据官方只给一条：<b>无更多数据时 next_cursor 返回空</b>（参数表未列出 has_more，本仓虽有该字段位但不得据其判定终点）；
    /// 本接口查询的是「企业已分配高级功能<b>且在应用可见范围</b>的账号列表」。</para>
    /// <para>本页面未给出独立的数值型频率限制，走官方「附录 - 访问频率限制」的默认阈值（见本接口 <c>remarks</c> 汇总口径）。</para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/vip/list")]
    Task<ListWedocVipResponse> ListVipAsync(
        [Body] ListWedocVipRequest request,
        CancellationToken cancellationToken = default);
}
