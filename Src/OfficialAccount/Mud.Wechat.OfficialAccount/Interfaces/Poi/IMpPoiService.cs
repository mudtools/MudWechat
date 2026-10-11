// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount;

/// <summary>
/// 微信公众号 / 服务号「微信门店（旧版 POI）」域 SDK
/// （查询门店 / 查询门店列表 / 删除门店——官方「微信门店」分组的查询与删除面；
/// 门店的新建 / 更新走官方后台或小程序店铺 API，官方未开放 HTTP 新建入口）。
/// </summary>
/// <remarks>
/// <para>
/// <b>与 <c>IMpStoreService</c>（店铺域）的关系（官方两代接口，并存勿合并）</b>：
/// 本域是<b>旧版微信门店接口（POI，<c>/cgi-bin/poi/*</c>）</b>；
/// <c>Store</c> 域是新版小程序店铺 API（<c>/wxa/*</c>）。两代接口的门店 ID 体系不同（<c>poi_id</c> vs 店铺 ID）。
/// </para>
/// <para>
/// MUD005 已知接受风险：微信公众号官方契约强制令牌走 Query 参数（<c>access_token</c>），
/// 无法改用 Header；URL 遥测已由组件 <c>SensitiveUrlRedactor</c> 与 <c>MpException</c> 构造期脱敏处置。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Poi", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IMpPoiService
{
    /// <summary>
    /// 查询门店信息。官方文档：<see href="https://developers.weixin.qq.com/doc/offiaccount/WeChat_Stores/WeChat_Store_Interface.html#9"/>
    /// （官方接口英文名 <c>getpoi</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>poi_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>门店基础信息（类目 / 坐标 / 图片 / 状态等）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + 请求体 <c>{"poi_id": …}</c>；成功响应不含 <c>errcode</c>（缺省 0 视为成功）。</para>
    /// <para>官方错误码：<c>-1</c> / <c>40007</c>（invalid media_id）/ <c>61341</c>（invalid poi_id，照官方现状记录）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/poi/getpoi")]
    Task<MpPoiGetResponse> GetPoiAsync(
        [Body] MpPoiGetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询门店列表。官方文档：<see href="https://developers.weixin.qq.com/doc/offiaccount/WeChat_Stores/WeChat_Store_Interface.html#10"/>
    /// （官方接口英文名 <c>getpoilist</c>）。
    /// </summary>
    /// <param name="request">分页请求（<c>begin</c> 从 0 起；<c>limit</c> 建议 ≤ 50）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>门店列表与总数（每项基础信息额外携带 poi_id / 升级状态）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + 请求体 <c>{"begin": …, "limit": …}</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/poi/getpoilist")]
    Task<MpPoiListResponse> GetPoiListAsync(
        [Body] MpPoiListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除门店。官方文档：<see href="https://developers.weixin.qq.com/doc/offiaccount/WeChat_Stores/WeChat_Store_Interface.html#12"/>
    /// （官方接口英文名 <c>delpoi</c>）。
    /// </summary>
    /// <param name="request">删除请求（<c>poi_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>
    /// <b>覆盖删除语义</b>：删除后微信侧门店即时下线、不可恢复；重建同名门店会得到新的 <c>poi_id</c>。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/poi/delpoi")]
    Task<MpResponse> DeletePoiAsync(
        [Body] MpPoiDeleteRequest request,
        CancellationToken cancellationToken = default);
}
