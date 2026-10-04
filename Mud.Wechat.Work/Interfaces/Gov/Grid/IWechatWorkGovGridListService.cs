// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Gov;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「政民沟通」模块获取网格列表公共 SDK。
/// <para>
/// 官方仅向企业自建应用开放本域 1 个端点（获取网格列表 grid/list，官方权限表标注
/// 代开发应用与第三方应用均「暂不支持」），端点收敛声明于本接口，
/// 继承链上仅声明自建子接口 <see cref="IWechatWorkInternalGovGridService"/> 同族的
/// <see cref="IWechatWorkInternalGovGridListService"/>
/// （形态对齐家校沟通健康上报域「官方开放面收敛」模式，能力漂移守卫）。
/// </para>
/// <para>自建/代开发公共面的网格结构端点落位于 <see cref="IWechatWorkGovGridService"/> 家族，
/// 不进入本家族。</para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，
/// Query 注入 <c>access_token</c>）。官方权限口径：自建应用须配置到
/// 「居民联系 - 可调用接口的应用」中；代开发应用暂不支持；第三方应用暂不支持。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkGovGridListService
{
    /// <summary>
    /// 获取网格列表
    /// <para>可以查询目标网格的父网格及全部子网格 ID 列表。</para>
    /// <para>官方业务限制：grid_id 不填则表示拉取根节点及其儿子节点。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GovGetGridListRequest"/>，grid_id 可选）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>网格列表（grid_list，含网格 id / 名称 / 父节点 id / 管理员与成员列表）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94481"/></para>
    /// <para>官方权限：自建应用须配置到「居民联系 - 可调用接口的应用」中；代开发应用暂不支持；第三方应用暂不支持。</para>
    /// </remarks>
    [Post("/cgi-bin/report/grid/list")]
    Task<GovGetGridListResponse> GetGridListAsync(
        [Body] GovGetGridListRequest request,
        CancellationToken cancellationToken = default);
}
