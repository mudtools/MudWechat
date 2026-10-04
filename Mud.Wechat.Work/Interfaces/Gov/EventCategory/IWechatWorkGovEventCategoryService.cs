// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Gov;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「政民沟通」模块配置事件类别域公共 SDK。
/// <para>
/// 官方对企业自建应用与服务商代开发应用开放一致的 4 个端点（添加事件类别 + 修改事件类别 +
/// 删除事件类别 + 获取事件类别列表），全部收敛声明于本接口；
/// 应用类型子接口均为零差异端点空标记：自建见 <see cref="IWechatWorkInternalGovEventCategoryService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderGovEventCategoryService"/>。
/// </para>
/// <para>
/// 第三方应用对配置事件类别域全部端点标注「暂不支持」，故本家族不设第三方子接口；
/// 配置网格结构域落位于 <see cref="IWechatWorkGovGridService"/> 与
/// <see cref="IWechatWorkGovGridListService"/> 家族。
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
public interface IWechatWorkGovEventCategoryService
{
    /// <summary>
    /// 添加事件类别
    /// <para>可以添加一级或二级事件类别，网格员在提交「巡查上报」或「居民上报」时将填入配置的事件类别。</para>
    /// <para>官方业务限制：分类名称不能超过 30 个字，同一一级分类下的二级分类名字不能一样；
    /// 分类层级只能传 1 或者 2，level 为 2 时 parent_category_id 必传。</para>
    /// </summary>
    /// <param name="request">添加请求体（<see cref="GovAddEventCategoryRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>新创建的分类 id。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94536"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97141"/></para>
    /// </remarks>
    [Post("/cgi-bin/report/grid/add_cata")]
    Task<GovAddEventCategoryResponse> AddEventCategoryAsync(
        [Body] GovAddEventCategoryRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改事件类别
    /// <para>可以修改已配置的一级或二级事件类别，网格员在提交「巡查上报」或「居民上报」时将填入配置的事件类别。</para>
    /// <para>官方业务限制：分类名称不能超过 30 个字，同一一级分类下的二级分类名字不能一样；
    /// 分类层级只能传 1 或者 2，level 为 2 时 parent_category_id 必传。</para>
    /// </summary>
    /// <param name="request">修改请求体（<see cref="GovUpdateEventCategoryRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>修改结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94537"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97142"/></para>
    /// </remarks>
    [Post("/cgi-bin/report/grid/update_cata")]
    Task<WechatWorkResponse> UpdateEventCategoryAsync(
        [Body] GovUpdateEventCategoryRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除事件类别
    /// <para>删除已配置的事件类别。</para>
    /// </summary>
    /// <param name="request">删除请求体（<see cref="GovDeleteEventCategoryRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>删除结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94538"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97143"/></para>
    /// </remarks>
    [Post("/cgi-bin/report/grid/delete_cata")]
    Task<WechatWorkResponse> DeleteEventCategoryAsync(
        [Body] GovDeleteEventCategoryRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取事件类别列表
    /// <para>可以获取已配置的一级或二级事件类别，网格员在提交「巡查上报」或「居民上报」时将填入配置的事件类别。</para>
    /// <para>官方业务限制：本端点无请求包体（仅 access_token Query 参数）；
    /// 响应列表字段官方 JSON 示例为 category_list、参数说明表误写为 cata_list，以示例为准。</para>
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>分类列表（category_list，含分类 id / 名称 / 层级 / 所属一级分类 id）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94540"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99548"/></para>
    /// </remarks>
    [Post("/cgi-bin/report/grid/list_cata")]
    Task<GovGetEventCategoryListResponse> GetEventCategoryListAsync(
        CancellationToken cancellationToken = default);
}
