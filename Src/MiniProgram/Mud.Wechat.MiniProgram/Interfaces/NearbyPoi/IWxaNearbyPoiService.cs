// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「附近小程序」域 SDK（4 端点：添加地点 / 删除地点 / 查看地点列表 / 设置展示状态）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>（小程序服务端 API → 附近小程序，2026-10-10 依据官方清单核验）：
/// <c>nearby-poi/api_addnearbypoi.html</c>、<c>nearby-poi/api_deletenearbypoi.html</c>、
/// <c>nearby-poi/api_getnearbypoilist.html</c>、<c>nearby-poi/api_setshowstatus.html</c>。
/// </para>
/// <para>
/// <b>业务形态（官方原文）</b>：附近小程序是面向「线下门店」的场景能力——将小程序地点挂到官方
/// 地图组件（如「附近」页），用户进入附近页即可在小程序列表看到该地点。添加后进入<b>审核</b>流程
/// （审核结果以官方页面为准，SDK 不做本地模拟）；<c>poi_id</c> 由添加结果返回，删除/设置展示状态
/// 均以它为键。
/// </para>
/// <para>
/// <b>令牌与鉴权</b>：全部走应用级 <c>access_token</c>（Query）。四端点官方均为 <b>POST</b>。
/// 本域不做二进制通道（响应均为 JSON）。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "NearbyPoi", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaNearbyPoiService
{
    /// <summary>
    /// 添加地点（附近小程序，提交审核）。官方文档：<c>nearby-poi/api_addnearbypoi.html</c>。
    /// </summary>
    /// <param name="request">添加请求（<c>is_show</c> / <c>categories</c> / <c>lat</c> / <c>lng</c> 等必填），见 <see cref="DataModels.NearbyPoi.WxaAddNearbyPoiRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>标准应答（<c>errcode</c> / <c>errmsg</c>），见 <see cref="DataModels.WxaResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/wxa/addnearbypoi</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para>
    /// <b>必填要点（官方原文）</b>：<c>is_show</c>（是否展示）、<c>categories</c>（门店类目）、
    /// <c>address</c>/<c>district</c>/<c>city</c>/<c>province</c>、<c>lat</c>/<c>lng</c>（十进制经纬度）、
    /// <c>media_id</c>（小程序<b>永久素材</b>媒体 ID，经 <c>material/addMaterial</c> 上传）、
    /// <c>poi_id</c>（商户平台创建的门店 ID）。<c>kf_info</c> / <c>store_info</c> 选填。
    /// </para>
    /// <para><b>审核流转</b>：添加成功后进入审核，审核结果以官方页面为准；本接口<b>不返回审核结论</b>。</para>
    /// </remarks>
    [Post("/wxa/addnearbypoi")]
    Task<DataModels.WxaResponse> AddAsync(
        [Body] DataModels.NearbyPoi.WxaAddNearbyPoiRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除地点（附近小程序）。官方文档：<c>nearby-poi/api_deletenearbypoi.html</c>。
    /// </summary>
    /// <param name="request">删除请求（<c>poi_id</c> 必填），见 <see cref="DataModels.NearbyPoi.WxaDeleteNearbyPoiRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>标准应答（<c>errcode</c> / <c>errmsg</c>），见 <see cref="DataModels.WxaResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/wxa/delnearbypoi</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para>
    /// <b>键来源（官方原文）</b>：<c>poi_id</c> 取自已添加的地点（<see cref="AddAsync"/> 成功后返回），
    /// SDK 不做本地记忆；删除后不可恢复。
    /// </para>
    /// </remarks>
    [Post("/wxa/delnearbypoi")]
    Task<DataModels.WxaResponse> DeleteAsync(
        [Body] DataModels.NearbyPoi.WxaDeleteNearbyPoiRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查看地点列表（附近小程序，分页）。官方文档：<c>nearby-poi/api_getnearbypoilist.html</c>。
    /// </summary>
    /// <param name="request">分页查询请求（<c>page</c> 从 <c>0</c> 开始），见 <see cref="DataModels.NearbyPoi.WxaNearbyPoiListRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>地点分页列表，见 <see cref="DataModels.NearbyPoi.WxaNearbyPoiListResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/wxa/getnearbypoilist</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para><b>分页（官方原文）</b>：<c>page</c> / <c>page_rows</c> 为必填分页参数，<c>page</c> 从 <c>0</c> 开始；每页条数上限以官方页面为准。</para>
    /// </remarks>
    [Post("/wxa/getnearbypoilist")]
    Task<DataModels.NearbyPoi.WxaNearbyPoiListResponse> GetListAsync(
        [Body] DataModels.NearbyPoi.WxaNearbyPoiListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置展示状态（附近小程序，展示 / 取消展示）。官方文档：<c>nearby-poi/api_setshowstatus.html</c>。
    /// </summary>
    /// <param name="request">状态请求（<c>poi_id</c> + <c>status</c> 必填），见 <see cref="DataModels.NearbyPoi.WxaSetNearbyPoiShowStatusRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>标准应答（<c>errcode</c> / <c>errmsg</c>），见 <see cref="DataModels.WxaResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/wxa/setnearbypoishowstatus</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para><b>状态语义（官方原文）</b>：<c>status</c> = <c>1</c> 展示 / <c>0</c> 取消展示；仅对已通过审核的地点生效。</para>
    /// </remarks>
    [Post("/wxa/setnearbypoishowstatus")]
    Task<DataModels.WxaResponse> SetShowStatusAsync(
        [Body] DataModels.NearbyPoi.WxaSetNearbyPoiShowStatusRequest request,
        CancellationToken cancellationToken = default);
}