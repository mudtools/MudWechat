// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「短剧媒资 · 授权」域 SDK（11 端点：剧目授权 3 + 账号授权 3 + 版权授权 4 + 被授权查询 1；
/// 官方 minidrama 分区 → authorizeapp / authorizedrama / authorizecopyright）。
/// </summary>
/// <remarks>
/// <para>
/// <b>三族授权对象、两种视角</b>：剧目授权（<c>*drama</c>）、账号授权（<c>*app</c>）、版权授权（<c>*copyright</c>）
/// 各自成对（增加 / 解除 / 查询），另有「查询被授权信息」（<see cref="GetAuthorizedObjectsAsync"/>）与
/// 「查询被版权授权信息」（<see cref="GetCopyrightAuthorizedListAsync"/>）两个<b>被授权方视角</b>端点。
/// <b>视角决定字段名</b>：授权方视角用 <c>authorized_appid</c>（被授权方），被授权方视角用 <c>authorizer_appid</c>（授权方）
/// ⇒ 一字母之差方向相反，读错即把「我授出去的权利」当成「我得到的权利」，守卫 DV4 逐字段锁定。
/// </para>
/// <para>
/// <b>两个到期字段名不同源</b>：剧目/账号授权族用 <c>authz_expire_time</c>，版权授权族用 <c>expire_time</c>
/// ⇒ 照录两侧官方原文，<b>不得</b>「对齐成一个名字」，守卫 DV4 锁定该不对齐裁决。
/// </para>
/// <para>
/// <b>类目前置</b>（官方原文）：剧目授权与账号授权要求授权方、被授权方<b>都</b>已申请【文娱-微短剧】类目；
/// 版权授权放宽为【文娱-微短剧】<b>或</b>【文娱-互动微短剧】。指定到期时间时授权时长<b>不得小于 7 天</b>
/// （从当前时间算起），<c>authz_expire_time</c> 不传或传 <c>0</c> 表示永久有效。
/// </para>
/// <para>
/// <b>令牌形态（MP-X2 / MP-X3 同口径）</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入
/// <c>access_token</c>（权限集 153）。域构成、适用范围段异常留档与全域 20 错误码共用表的说明见
/// <see cref="IWxaVodUploadService"/>。
/// </para>
/// <para><b>调用限制</b>：11 端点官方均未公布额度数字（除 <see cref="AuthorizeDramaAsync"/> 的类目与时长前置）。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Vod", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaVodAuthorizationService
{
    /// <summary>
    /// 查询被授权信息（<c>POST /wxa/sec/vod/getauthorizedobjects</c>，被授权方视角，不支持云调用）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/authorizeapp/api_getauthorizedobjects.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>authorizer_appid</c> / <c>offset</c> / <c>limit</c> 全选填），见 <see cref="WxaVodGetAuthorizedObjectsRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>总数 + 记录列表（<c>objects</c>，元素为 <see cref="WxaVodAuthorizedObject"/>），见 <see cref="WxaVodGetAuthorizedObjectsResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体；官方标注「本接口无特殊注意事项」。</para>
    /// <para>
    /// <b>与「查询授权信息」的元素类型不共用</b>：本页元素标识字段是 <c>authorizer_appid</c>（谁授给我），
    /// 那页是 <c>authorized_appid</c>（我授给谁）⇒ 分立为 <see cref="WxaVodAuthorizedObject"/> 与
    /// <see cref="WxaVodAuthorizeObject"/>，守卫 DV4 锁定两字段面不交叉。
    /// </para>
    /// </remarks>
    [Post("/wxa/sec/vod/getauthorizedobjects")]
    Task<WxaVodGetAuthorizedObjectsResponse> GetAuthorizedObjectsAsync(
        [Body] WxaVodGetAuthorizedObjectsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 增加剧目授权（<c>GET /wxa/sec/vod/authorizedrama</c>，不支持云调用）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/authorizedrama/api_authorizedrama.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>drama_id</c> 数组 + <c>authorized_appid</c> 必填，<c>authz_expire_time</c> 选填），见 <see cref="WxaVodAuthorizeDramaRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>逐剧目结果数组（<c>result</c>），见 <see cref="WxaVodAuthorizeResultResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>GET</b>（本域 31 端点里唯一写作 GET 的一页）<b>＋ JSON 请求体</b>——官方请求示例是裸 JSON 对象
    /// （<c>{"authorized_appid": "wx***", "drama_id": [100200, 100205], "authz_expire_time": 0}</c>）而非 Query 串
    /// ⇒ 与直播域、交易保障域「参数出现在请求地址行 ⇒ 按 <c>[Query]</c> 落地」的裁决<b>不适用</b>于本页：
    /// 本 SDK 按官方示例落地为 GET + <c>[Body]</c>（生成器产出的实现为 <c>HttpRequestMessage(HttpMethod.Get, …)</c>
    /// 且带 <c>Content</c>，与官方线格式一致）。<b>同页错误码表里的 <c>43002</c>「HTTP 请求必须使用 POST 方法」与本行
    /// 自相矛盾</b>，属官方全域共用同一张码表所致 ⇒ 照录调用方式行，不据此改成 POST（裁决留档，守卫 DV4 锁定动词）。
    /// </para>
    /// <para>
    /// <b>业务限制（官方原文三条）</b>：授权方与被授权方都需申请【文娱-微短剧】类目；被授权剧目必须已审核通过；
    /// 指定到期时间时授权时长不得小于 7 天（从当前时间算起）。
    /// </para>
    /// <para><b>批量逐项结果</b>：接口级 <c>errcode</c> 为 0 不代表全部剧目授权成功，须逐项读 <c>result</c>（官方示例即给出「100200 成功 + 100205 失败」的混合形态）。</para>
    /// <para>官方错误码：<c>10090038</c>（被授权账号没有【文娱-微短剧】类目）/ <c>10090040</c>（剧集已经被占用）。</para>
    /// </remarks>
    [Get("/wxa/sec/vod/authorizedrama")]
    Task<WxaVodAuthorizeResultResponse> AuthorizeDramaAsync(
        [Body] WxaVodAuthorizeDramaRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 解除剧目授权（<c>POST /wxa/sec/vod/deauthorizedrama</c>，不支持云调用）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/authorizedrama/api_deauthorizedrama.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>drama_id</c> 数组 + <c>authorized_appid</c>，均必填），见 <see cref="WxaVodDeauthorizeDramaRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>逐剧目结果数组（<c>result</c>），见 <see cref="WxaVodAuthorizeResultResponse"/>（四支写端点共用应答）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体；官方标注「本接口无特殊注意事项」。</para>
    /// <para>
    /// <b>本支参数名是 <c>authorized_appid</c>，「增加剧目授权」页参数表列作 <c>authorized</c></b>（官方两页参数表自相矛盾，
    /// 且与同域其余各页不一致）⇒ 增加侧按官方示例改名，本页照表格原样，两请求体字段名交集不完全相同故不共用 DTO，
    /// 见 <see cref="WxaVodAuthorizeDramaRequest"/>。
    /// </para>
    /// <para><b>破坏性操作</b>：解除后被授权方立即失去播放资格 ⇒ 宿主侧须自行做二次确认；官方错误码 <c>10090039</c>（已经被解除授权）/ <c>10090043</c>（不存在授权关系）。</para>
    /// </remarks>
    [Post("/wxa/sec/vod/deauthorizedrama")]
    Task<WxaVodAuthorizeResultResponse> DeauthorizeDramaAsync(
        [Body] WxaVodDeauthorizeDramaRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询授权信息（<c>POST /wxa/sec/vod/getauthorizeobjects</c>，授权方视角，不支持云调用）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/authorizedrama/api_getauthorizeobjects.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>drama_id</c> / <c>authorized_appid</c> / <c>offset</c> / <c>limit</c> <b>四支全选填</b>），见 <see cref="WxaVodGetAuthorizeObjectsRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>总数 + 记录列表（<c>objects</c>，元素为 <see cref="WxaVodAuthorizeObject"/>），见 <see cref="WxaVodGetAuthorizeObjectsResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体；官方标注「本接口无特殊注意事项」。</para>
    /// <para><b>全选填 = 全量查询</b>（官方原文）：不填 <c>drama_id</c> 表示查所有剧目、不填 <c>authorized_appid</c> 表示查所有被授权方 ⇒ 宿主须自带分页遍历，官方未给游标。</para>
    /// </remarks>
    [Post("/wxa/sec/vod/getauthorizeobjects")]
    Task<WxaVodGetAuthorizeObjectsResponse> GetAuthorizeObjectsAsync(
        [Body] WxaVodGetAuthorizeObjectsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 增加账号授权（<c>POST /wxa/sec/vod/authorizeapp</c>，不支持云调用）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/authorizeapp/api_authorizeapp.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>authorized_appid</c> 必填 + <c>authz_expire_time</c> 选填），见 <see cref="WxaVodAuthorizeAppRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>（官方返回体表除通用两字段外无业务字段）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体。</para>
    /// <para><b>账号授权是「整号」粒度</b>：本端点授出后被授权方可看到授权方的剧目全集（区别于 <see cref="AuthorizeDramaAsync"/> 的逐剧目授权）。</para>
    /// <para><b>业务限制（官方原文）</b>：授权方与被授权方都需申请【文娱-微短剧】类目；指定到期时间时授权时长不得小于 7 天（从当前时间算起），不传或传 0 表示永久有效。</para>
    /// <para><b>应答无结果数组</b>：账号级授权是单目标操作，成败直接由接口级 <c>errcode</c> 表达，与剧目级的逐目标 <c>result</c> 形态不同。</para>
    /// </remarks>
    [Post("/wxa/sec/vod/authorizeapp")]
    Task<WxaResponse> AuthorizeAppAsync(
        [Body] WxaVodAuthorizeAppRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 解除账号授权（<c>POST /wxa/sec/vod/deauthorizeapp</c>，不支持云调用）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/authorizeapp/api_deauthorizeapp.html"/>。
    /// </summary>
    /// <param name="request">请求体（只有 <c>authorized_appid</c> 一字段，必填），见 <see cref="WxaVodDeauthorizeAppRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体；官方标注「本接口无特殊注意事项」。</para>
    /// <para><b>破坏性操作</b>：解除后该被授权账号失去全部剧目访问资格 ⇒ 宿主侧须自行做二次确认；官方错误码 <c>10090043</c>（不存在授权关系）。</para>
    /// </remarks>
    [Post("/wxa/sec/vod/deauthorizeapp")]
    Task<WxaResponse> DeauthorizeAppAsync(
        [Body] WxaVodDeauthorizeAppRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询账号授权信息（<c>POST /wxa/sec/vod/getauthorizeapps</c>，不支持云调用）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/authorizeapp/api_getauthorizeapps.html"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>被授权账号列表（<c>objects</c>，元素为 <see cref="WxaVodAuthorizeAppObject"/>），见 <see cref="WxaVodGetAuthorizeAppsResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b>，<b>请求体官方标注为「无」</b>（返回示例的请求行只有 <c>?access_token=…</c>）
    /// ⇒ SDK 不声明请求体参数，<b>不得</b>人为造请求体 DTO（同 <c>Vip</c> 域「空请求体规则」）；
    /// 本端点也是本域唯一无入参的业务端点，故<b>无分页</b>——全量返回。
    /// </para>
    /// <para><b>路由名与「查询被授权信息」极易混读</b>：本页 <c>getauthorizeapps</c>（我授出的账号，<c>authorized_appid</c>），那页 <c>getauthorizedobjects</c>（我得到的授权，<c>authorizer_appid</c>）⇒ 守卫 DV4 与零声明守卫（拼写变体 <c>getauthorizedapps</c> 在册）双向锁定。</para>
    /// </remarks>
    [Post("/wxa/sec/vod/getauthorizeapps")]
    Task<WxaVodGetAuthorizeAppsResponse> GetAuthorizeAppsAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 增加版权授权（<c>POST /wxa/sec/vod/authorizecopyright</c>，不支持云调用）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/authorizecopyright/api_authorizecopyright.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>authorization_type</c> 必填，其余四支出目标与有效期，选填），见 <see cref="WxaVodAuthorizeCopyrightRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>逐剧目结果数组（<c>result</c>），见 <see cref="WxaVodAuthorizeResultResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体。</para>
    /// <para>
    /// <b><c>authorization_type</c> 决定必填的出参字段</b>（<see cref="WxaVodAuthorizationTypes"/>）：
    /// 按主体授权须传 <c>authorized_subject_cert_no</c>（官方原文：该主体下有相应类目的小程序<b>全部自动获得授权</b>）；
    /// 按小程序授权须传 <c>authorized_appid</c> ⇒ 二选一，官方表格全列选填，校验属宿主业务层。
    /// </para>
    /// <para><b>版权授权的前置</b>（官方原文）：受版权保护的剧目才能授权，剧目提审时可申请版权保护（见 <see cref="WxaVodCopyright.ApplyForCopyrightProtection"/>）。</para>
    /// <para><b>类目面比剧目/账号授权宽</b>：本族官方要求【文娱-微短剧】<b>或</b>【文娱-互动微短剧】，另两族只认前者。</para>
    /// <para><b>本族到期字段名是 <c>expire_time</c></b>（不是剧目/账号侧的 <c>authz_expire_time</c>）⇒ 照录原文，守卫 DV4 锁定。</para>
    /// </remarks>
    [Post("/wxa/sec/vod/authorizecopyright")]
    Task<WxaVodAuthorizeResultResponse> AuthorizeCopyrightAsync(
        [Body] WxaVodAuthorizeCopyrightRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 解除版权授权（<c>POST /wxa/sec/vod/deauthorizecopyright</c>，不支持云调用）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/authorizecopyright/api_deauthorizecopyright.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>authorization_type</c> 必填，出目标字段选填），见 <see cref="WxaVodDeauthorizeCopyrightRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>逐剧目结果数组（<c>result</c>），见 <see cref="WxaVodAuthorizeResultResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体；官方标注「本接口无特殊注意事项」。</para>
    /// <para><b>与增加侧的字段面差异</b>：解除侧官方请求体表内<b>没有</b> <c>expire_time</c>（解除无须指定时间）⇒ 两请求体不共用 DTO，守卫 DV4 锁定。</para>
    /// <para><b>破坏性操作</b>：按主体解除会一次影响该主体名下全部符合条件的小程序 ⇒ 宿主侧须自行做二次确认。</para>
    /// </remarks>
    [Post("/wxa/sec/vod/deauthorizecopyright")]
    Task<WxaVodAuthorizeResultResponse> DeauthorizeCopyrightAsync(
        [Body] WxaVodDeauthorizeCopyrightRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询版权授权信息（<c>POST /wxa/sec/vod/getcopyrightauthorizationlist</c>，授权方视角，不支持云调用）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/authorizecopyright/api_getcopyrightauthorizationlist.html"/>。
    /// </summary>
    /// <param name="request">请求体（六支过滤参数全选填），见 <see cref="WxaVodGetCopyrightAuthorizationListRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>总数 + 记录列表（<c>list</c>，元素为 <see cref="WxaVodCopyrightAuthorization"/>），见 <see cref="WxaVodGetCopyrightAuthorizationListResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体；官方标注「本接口无特殊注意事项」。</para>
    /// <para>
    /// <b>本端点与「查询被版权授权信息」的元素类型共用</b>：官方两页 <c>list</c> 子表字段名完全相同
    /// （<b>同时</b>含 <c>authorizer_appid</c> 与 <c>authorized_appid</c> 两侧标识）⇒ 共用
    /// <see cref="WxaVodCopyrightAuthorization"/>，两页只是视角不同，守卫 DV4 锁定该共用裁决。
    /// </para>
    /// <para><b>过滤维度含剧目与主体</b>：<c>drama_id</c>（单值，非数组）与 <c>authorized_subject_cert_no</c> 可与 <c>authorization_type</c> 组合 ⇒ 逐字段口径见 DTO 注释。</para>
    /// </remarks>
    [Post("/wxa/sec/vod/getcopyrightauthorizationlist")]
    Task<WxaVodGetCopyrightAuthorizationListResponse> GetCopyrightAuthorizationListAsync(
        [Body] WxaVodGetCopyrightAuthorizationListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询被版权授权信息（<c>POST /wxa/sec/vod/getcopyrightauthorizedlist</c>，被授权方视角，不支持云调用）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/authorizecopyright/api_getcopyrightauthorizedlist.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>authorizer_appid</c> / <c>offset</c> / <c>limit</c> 全选填），见 <see cref="WxaVodGetCopyrightAuthorizedListRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>总数 + 记录列表（<c>list</c>，元素与授权侧共用 <see cref="WxaVodCopyrightAuthorization"/>），见 <see cref="WxaVodGetCopyrightAuthorizedListResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体；官方标注「本接口无特殊注意事项」。</para>
    /// <para><b>入参侧字段名是 <c>authorizer_appid</c></b>（过滤「哪个授权方给我的」）：与同域写端点侧的 <c>authorized_appid</c> 一字母之差方向相反，是本族最易写反的一处，守卫 DV4 逐字段锁定。</para>
    /// <para><b>与「查询被授权信息」不共用</b>：那页元素是剧目/账号授权记录（<c>authz_expire_time</c>），本页是版权授权记录（<c>expire_time</c>）⇒ 两元素类型分立。</para>
    /// </remarks>
    [Post("/wxa/sec/vod/getcopyrightauthorizedlist")]
    Task<WxaVodGetCopyrightAuthorizedListResponse> GetCopyrightAuthorizedListAsync(
        [Body] WxaVodGetCopyrightAuthorizedListRequest request,
        CancellationToken cancellationToken = default);
}
