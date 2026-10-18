// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OpenPlatform.DataModels.Ams;

namespace Mud.Wechat.OpenPlatform;

/// <summary>
/// 微信开放平台「流量主代运营（AMS）」域 SDK · 平台代授权方管理面（<b>19 端点</b>，恒为 <b>POST</b> <c>/wxa/operationams</c>，以 <c>action</c> query 分派）。
/// </summary>
/// <remarks>
/// <para><b>路由面</b>：全域 19 条端点共用一条路由 <c>/wxa/operationams</c>，由 <c>action</c> query 参数区分（守卫逐条锁定 action 字面量），
/// 动词恒为 POST（含「获取」类查询端点官方亦为 POST）。</para>
/// <para><b>令牌面</b>：全域 19 页「注意事项」均写 <c>access_token</c> 取 <c>authorizer_access_token</c>（第三方平台代授权方小程序调用），
/// Query 参数名恒为 <c>access_token</c>（2026-10-17 逐页核验）。</para>
/// <para><b>应答信封陷阱</b>：本域应答是 <c>{ret, err_msg}</c> 而非本线其它域的 <c>{errcode, errmsg}</c>，故判错基底另立
/// <see cref="OpenPlatformAmsResponse"/>（<c>ErrorCode =&gt; Ret</c>，判错契约仍走公用层 <c>IWechatApiResponse</c>）。</para>
/// <para><b>落位</b>：<c>Interfaces/Ams/IWechatOpenPlatformAmsService.cs</c>；DTO 命名空间 <c>Mud.Wechat.OpenPlatform.DataModels.Ams</c>；
/// <c>OpenPlatformModule.Ams</c>，注册组 <c>Ams</c>。分账比例与服务商已结算收入（component 令牌面）另见 <see cref="IWechatOpenPlatformAmsShareService"/>。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Ams", TokenManage = nameof(IOpenPlatformAppManager))]
[Token(TokenType = OpenPlatformTokenTypes.AuthorizerAccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWechatOpenPlatformAmsService
{
    /// <summary>
    /// 「创建广告单元」（<c>agency_create_adunit</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/ad-mgnt/AgencyCreateAdunit.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformAmsAdUnitCreateRequest"/>。</param>
    /// <param name="action">官方 <c>action</c> 分派值，缺省 <c>agency_create_adunit</c>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformAmsCreateAdUnitResponse"/>。</returns>
    [Post("/wxa/operationams")]
    Task<OpenPlatformAmsCreateAdUnitResponse> CreateAdUnitAsync(
        [Body] OpenPlatformAmsAdUnitCreateRequest request,
        [Query("action")] string action = "agency_create_adunit",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「更新广告单元」（<c>agency_update_adunit</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/ad-mgnt/AgencyUpdateAdunit.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformAmsAdUnitUpdateRequest"/>。</param>
    /// <param name="action">官方 <c>action</c> 分派值，缺省 <c>agency_update_adunit</c>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformAmsResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方页面复制缺陷</b>：本页代码示例把 <c>action</c> 误写为 <c>agency_create_adunit</c>（与「创建广告单元」页重复），
    /// 但本页正文标题、请求体（含 <c>ad_unit_id</c> + <c>status</c>）与页面文件名 <c>AgencyUpdateAdunit</c> 均为「更新」语义 ⇒
    /// 本仓以正确分派值 <c>agency_update_adunit</c> 声明（守卫 AM-U1 附此留档）。若官方网关实际按页面示例拒绝，属官方文档缺陷，不据示例造重复端点。</para>
    /// </remarks>
    [Post("/wxa/operationams")]
    Task<OpenPlatformAmsResponse> UpdateAdUnitAsync(
        [Body] OpenPlatformAmsAdUnitUpdateRequest request,
        [Query("action")] string action = "agency_update_adunit",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「获取广告单元代码」（<c>agency_get_adunit_code</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/ad-mgnt/GetAdunitCode.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>ad_unit_id</c>），见 <see cref="OpenPlatformAmsAdUnitIdRequest"/>。</param>
    /// <param name="action">官方 <c>action</c> 分派值，缺省 <c>agency_get_adunit_code</c>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformAmsAdUnitCodeResponse"/>。</returns>
    /// <remarks>请求体与 <c>agency_get_tmpl_type</c> 逐字相同（仅 <c>ad_unit_id</c>）⇒ 共用 <see cref="OpenPlatformAmsAdUnitIdRequest"/>。</remarks>
    [Post("/wxa/operationams")]
    Task<OpenPlatformAmsAdUnitCodeResponse> GetAdUnitCodeAsync(
        [Body] OpenPlatformAmsAdUnitIdRequest request,
        [Query("action")] string action = "agency_get_adunit_code",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「获取广告位或指定广告单元的信息」（<c>agency_get_adunit_list</c>，官方为 POST）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/ad-mgnt/GetAdunitList.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformAmsAdUnitListRequest"/>。</param>
    /// <param name="action">官方 <c>action</c> 分派值，缺省 <c>agency_get_adunit_list</c>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformAmsAdUnitListResponse"/>。</returns>
    [Post("/wxa/operationams")]
    Task<OpenPlatformAmsAdUnitListResponse> GetAdUnitListAsync(
        [Body] OpenPlatformAmsAdUnitListRequest request,
        [Query("action")] string action = "agency_get_adunit_list",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「获取原生模板广告模板类型」（<c>agency_get_tmpl_type</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/ad-mgnt/AgencyGetTmplType.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>ad_unit_id</c>），见 <see cref="OpenPlatformAmsAdUnitIdRequest"/>。</param>
    /// <param name="action">官方 <c>action</c> 分派值，缺省 <c>agency_get_tmpl_type</c>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformAmsTmplTypeResponse"/>。</returns>
    [Post("/wxa/operationams")]
    Task<OpenPlatformAmsTmplTypeResponse> GetTmplTypeAsync(
        [Body] OpenPlatformAmsAdUnitIdRequest request,
        [Query("action")] string action = "agency_get_tmpl_type",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「获取封面广告位场景设置」（<c>agency_get_cover_adpos_scene</c>，无请求体）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/ad-mgnt/GetCoverAdposScene.html"/>。
    /// </summary>
    /// <param name="action">官方 <c>action</c> 分派值，缺省 <c>agency_get_cover_adpos_scene</c>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformAmsCoverSceneResponse"/>。</returns>
    /// <remarks>官方示例请求体为空对象 <c>{}</c> ⇒ 不得造请求 DTO。</remarks>
    [Post("/wxa/operationams")]
    Task<OpenPlatformAmsCoverSceneResponse> GetCoverAdposSceneAsync(
        [Query("action")] string action = "agency_get_cover_adpos_scene",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「获取封面广告位状态」（<c>agency_get_cover_adpos_status</c>，无请求体）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/ad-mgnt/GetCoverAdposStatus.html"/>。
    /// </summary>
    /// <param name="action">官方 <c>action</c> 分派值，缺省 <c>agency_get_cover_adpos_status</c>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformAmsCoverStatusResponse"/>。</returns>
    /// <remarks>官方示例请求体为空对象 <c>{}</c> ⇒ 不得造请求 DTO。</remarks>
    [Post("/wxa/operationams")]
    Task<OpenPlatformAmsCoverStatusResponse> GetCoverAdposStatusAsync(
        [Query("action")] string action = "agency_get_cover_adpos_status",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「设置封面广告位场景值」（<c>agency_set_cover_adpos_scene</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/ad-mgnt/SetCoverAdposScene.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>cover_scene_list</c>），见 <see cref="OpenPlatformAmsCoverSceneSetRequest"/>。</param>
    /// <param name="action">官方 <c>action</c> 分派值，缺省 <c>agency_set_cover_adpos_scene</c>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformAmsResponse"/>。</returns>
    [Post("/wxa/operationams")]
    Task<OpenPlatformAmsResponse> SetCoverAdposSceneAsync(
        [Body] OpenPlatformAmsCoverSceneSetRequest request,
        [Query("action")] string action = "agency_set_cover_adpos_scene",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「设置封面广告位开关状态」（<c>agency_set_cover_adpos_status</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/ad-mgnt/SetCoverAdposStatus.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>status</c>），见 <see cref="OpenPlatformAmsCoverStatusSetRequest"/>。</param>
    /// <param name="action">官方 <c>action</c> 分派值，缺省 <c>agency_set_cover_adpos_status</c>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformAmsResponse"/>。</returns>
    [Post("/wxa/operationams")]
    Task<OpenPlatformAmsResponse> SetCoverAdposStatusAsync(
        [Body] OpenPlatformAmsCoverStatusSetRequest request,
        [Query("action")] string action = "agency_set_cover_adpos_status",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「获取原生模板广告自定义模板绑定情况」（<c>get_agency_ad_unit_list</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/ad-mgnt/GetAgencyTmplIdList.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformAmsAgencyAdUnitListRequest"/>。</param>
    /// <param name="action">官方 <c>action</c> 分派值，缺省 <c>get_agency_ad_unit_list</c>（注意本端点 action 无 <c>agency_</c> 前缀，照官方原文）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformAmsAgencyAdUnitListResponse"/>。</returns>
    [Post("/wxa/operationams")]
    Task<OpenPlatformAmsAgencyAdUnitListResponse> GetAgencyAdUnitListAsync(
        [Body] OpenPlatformAmsAgencyAdUnitListRequest request,
        [Query("action")] string action = "get_agency_ad_unit_list",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「获取屏蔽的广告主」（<c>agency_get_black_list</c>，无请求体）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/ad-black/GetBlackList.html"/>。
    /// </summary>
    /// <param name="action">官方 <c>action</c> 分派值，缺省 <c>agency_get_black_list</c>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformAmsBlackListResponse"/>。</returns>
    [Post("/wxa/operationams")]
    Task<OpenPlatformAmsBlackListResponse> GetBlackListAsync(
        [Query("action")] string action = "agency_get_black_list",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「设置屏蔽的广告主」（<c>agency_set_black_list</c>，覆盖式）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/ad-black/SetBlackList.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>op</c> + <c>list</c>），见 <see cref="OpenPlatformAmsBlackListSetRequest"/>。</param>
    /// <param name="action">官方 <c>action</c> 分派值，缺省 <c>agency_set_black_list</c>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformAmsResponse"/>。</returns>
    [Post("/wxa/operationams")]
    Task<OpenPlatformAmsResponse> SetBlackListAsync(
        [Body] OpenPlatformAmsBlackListSetRequest request,
        [Query("action")] string action = "agency_set_black_list",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「获取行业屏蔽信息」（<c>agency_get_mp_amscategory_blacklist</c>，无请求体）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/ad-black/GetAmsCategoryBlackList.html"/>。
    /// </summary>
    /// <param name="action">官方 <c>action</c> 分派值，缺省 <c>agency_get_mp_amscategory_blacklist</c>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformAmsCategoryBlackListResponse"/>。</returns>
    [Post("/wxa/operationams")]
    Task<OpenPlatformAmsCategoryBlackListResponse> GetAmsCategoryBlackListAsync(
        [Query("action")] string action = "agency_get_mp_amscategory_blacklist",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「设置行业屏蔽信息」（<c>agency_set_mp_amscategory_blacklist</c>，覆盖式）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/ad-black/SetAmsCategoryBlackList.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>ams_category</c>），见 <see cref="OpenPlatformAmsCategoryBlackListSetRequest"/>。</param>
    /// <param name="action">官方 <c>action</c> 分派值，缺省 <c>agency_set_mp_amscategory_blacklist</c>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformAmsResponse"/>。</returns>
    [Post("/wxa/operationams")]
    Task<OpenPlatformAmsResponse> SetAmsCategoryBlackListAsync(
        [Body] OpenPlatformAmsCategoryBlackListSetRequest request,
        [Query("action")] string action = "agency_set_mp_amscategory_blacklist",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「获取小程序广告汇总数据」（<c>agency_get_adpos_genenral</c>，官方拼写 <c>genenral</c> 非 <c>general</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/ad-data/GetAdposGenenral.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformAmsAdposGeneralRequest"/>。</param>
    /// <param name="action">官方 <c>action</c> 分派值，缺省 <c>agency_get_adpos_genenral</c>（拼写照官方原文）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformAmsAdposGeneralResponse"/>。</returns>
    [Post("/wxa/operationams")]
    Task<OpenPlatformAmsAdposGeneralResponse> GetAdposGeneralAsync(
        [Body] OpenPlatformAmsAdposGeneralRequest request,
        [Query("action")] string action = "agency_get_adpos_genenral",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「获取小程序广告细分数据」（<c>agency_get_adunit_general</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/ad-data/GetAdposDetail.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformAmsAdunitGeneralRequest"/>。</param>
    /// <param name="action">官方 <c>action</c> 分派值，缺省 <c>agency_get_adunit_general</c>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformAmsAdunitGeneralResponse"/>。</returns>
    [Post("/wxa/operationams")]
    Task<OpenPlatformAmsAdunitGeneralResponse> GetAdUnitGeneralAsync(
        [Body] OpenPlatformAmsAdunitGeneralRequest request,
        [Query("action")] string action = "agency_get_adunit_general",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「检测是否能开通流量主」（<c>agency_check_can_open_publisher</c>，无请求体）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/open/AgencyCheckCanOpenPublisher.html"/>。
    /// </summary>
    /// <param name="action">官方 <c>action</c> 分派值，缺省 <c>agency_check_can_open_publisher</c>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformAmsCheckCanOpenPublisherResponse"/>。</returns>
    [Post("/wxa/operationams")]
    Task<OpenPlatformAmsCheckCanOpenPublisherResponse> CheckCanOpenPublisherAsync(
        [Query("action")] string action = "agency_check_can_open_publisher",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「开通流量主」（<c>agency_create_publisher</c>，无请求体）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/open/AgencyCreatePublisher.html"/>。
    /// </summary>
    /// <param name="action">官方 <c>action</c> 分派值，缺省 <c>agency_create_publisher</c>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformAmsResponse"/>。</returns>
    [Post("/wxa/operationams")]
    Task<OpenPlatformAmsResponse> CreatePublisherAsync(
        [Query("action")] string action = "agency_create_publisher",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 「获取小程序结算收入数据」（<c>agency_get_settlement</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/settlement/GetSettlement.html"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="OpenPlatformAmsRevenuePageRequest"/>。</param>
    /// <param name="action">官方 <c>action</c> 分派值，缺省 <c>agency_get_settlement</c>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>见 <see cref="OpenPlatformAmsSettlementResponse"/>。</returns>
    /// <remarks>应答形状与 <see cref="IWechatOpenPlatformAmsShareService.GetAgencySettledRevenueAsync"/> 逐字相同 ⇒ 共用 <see cref="OpenPlatformAmsSettlementResponse"/>。</remarks>
    [Post("/wxa/operationams")]
    Task<OpenPlatformAmsSettlementResponse> GetSettlementAsync(
        [Body] OpenPlatformAmsRevenuePageRequest request,
        [Query("action")] string action = "agency_get_settlement",
        CancellationToken cancellationToken = default);
}
