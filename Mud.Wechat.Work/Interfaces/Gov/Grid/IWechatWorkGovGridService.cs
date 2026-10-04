// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Gov;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「政民沟通」模块配置网格结构域公共 SDK。
/// <para>
/// 官方对企业自建应用与服务商代开发应用开放一致的 4 个端点（添加网格 + 编辑网格 + 删除网格 +
/// 获取用户负责及参与的网格列表），全部收敛声明于本接口；
/// 应用类型子接口均为零差异端点空标记：自建见 <see cref="IWechatWorkInternalGovGridService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderGovGridService"/>。
/// </para>
/// <para>
/// 官方仅向企业自建应用开放的差异端点（获取网格列表 grid/list，官方权限表标注代开发应用「暂不支持」）
/// 落位于 <see cref="IWechatWorkGovGridListService"/> 家族，不进入本家族；
/// 第三方应用对配置网格结构域全部端点标注「暂不支持」，故本家族不设第三方子接口。
/// </para>
/// <para>
/// 配置事件类别域（add_cata / update_cata / delete_cata / list_cata）落位于
/// <see cref="IWechatWorkGovEventCategoryService"/> 家族。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 自建与代开发消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>）：
/// 企业自建为应用自身 access_token；代开发为授权企业级 access_token（scope = authCorpId），
/// 由多应用基座按当前应用上下文路由——企业级令牌须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。
/// </para>
/// <para>
/// 权限口径：自建应用须配置到「居民联系 - 可调用接口的应用」中；
/// 代开发应用支持，需勾选「居民联系权限」；第三方应用暂不支持。
/// 自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口（存量企业暂不受影响）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkGovGridService
{
    /// <summary>
    /// 添加网格
    /// <para>在政民沟通「居民联系」网格结构中添加网格节点。</para>
    /// <para>官方业务限制：网格名称不能超过 30 个字，同一个目标网格下不能存在同名的同级子网格；
    /// 网格结构层级最多支持 10 层；每个网格至少 1 个、最多 20 个负责人；
    /// 成员列表不能超过 100 个；同一成员最多能成功担任 10 个网格的管理员。</para>
    /// </summary>
    /// <param name="request">添加请求体（<see cref="GovAddGridRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>新创建的网格 id 与不合法的 userid 列表。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94478"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97136"/></para>
    /// </remarks>
    [Post("/cgi-bin/report/grid/add")]
    Task<GovAddGridResponse> AddGridAsync(
        [Body] GovAddGridRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 编辑网格
    /// <para>编辑政民沟通「居民联系」网格结构中的网格节点。</para>
    /// <para>官方业务限制：网格名称不能超过 30 个字，同一个目标网格下不能存在同名的同级子网格；
    /// 网格结构层级最多支持 10 层；每个网格至少 1 个、最多 20 个负责人；
    /// 成员列表不能超过 100 个，为空则表示清空所有的成员；同一成员最多能成功担任 10 个网格的管理员。</para>
    /// </summary>
    /// <param name="request">编辑请求体（<see cref="GovUpdateGridRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>不合法的 userid 列表。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94479"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97137"/></para>
    /// </remarks>
    [Post("/cgi-bin/report/grid/update")]
    Task<GovUpdateGridResponse> UpdateGridAsync(
        [Body] GovUpdateGridRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除网格
    /// <para>删除政民沟通「居民联系」网格结构中的网格节点。</para>
    /// <para>官方业务限制：当目标网格及其子网格中不包含网格员时，可以删除目标网格及其全部子网格；
    /// 根节点的 grid_id 可以填空。</para>
    /// </summary>
    /// <param name="request">删除请求体（<see cref="GovDeleteGridRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>删除结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94480"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97138"/></para>
    /// </remarks>
    [Post("/cgi-bin/report/grid/delete")]
    Task<WechatWorkResponse> DeleteGridAsync(
        [Body] GovDeleteGridRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取用户负责及参与的网格列表
    /// <para>获取指定成员作为管理员管理的网格与作为成员参与的网格列表。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GovGetUserGridInfoRequest"/>，userid 官方必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>负责及参与的网格列表（manage_grids 管理的网格 / joined_grids 参与的网格）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94482"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97140"/></para>
    /// </remarks>
    [Post("/cgi-bin/report/grid/get_user_grid_info")]
    Task<GovGetUserGridInfoResponse> GetUserGridInfoAsync(
        [Body] GovGetUserGridInfoRequest request,
        CancellationToken cancellationToken = default);
}
