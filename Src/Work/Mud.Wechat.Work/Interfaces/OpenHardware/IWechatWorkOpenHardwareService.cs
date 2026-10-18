// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.OpenHardware;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「开放硬件」（智能硬件 · 硬件云端接入）接口族 SDK（17 端点）。
/// </summary>
/// <remarks>
/// <para>
/// <b>令牌形态（本域契约核心）</b>：官方硬件云端接入面<b>不消费</b> <c>access_token</c> /
/// <c>suite_access_token</c> / <c>provider_access_token</c> 任一应用令牌链路，而是自成两级凭证：
/// </para>
/// <para>
/// ① <c>model_access_token</c>（设备型号调用凭证，由 <c>model_id</c> + <c>model_secret</c> +
/// 企业微信后台推送的 <c>model_ticket</c> 换取，见 95992）；<br/>
/// ② <c>device_access_token</c>（设备调用凭证，由 <c>device_sn</c> + <c>device_secret</c> 换取，见 96022）。
/// </para>
/// <para>
/// 二者均为「调用方（硬件厂商）持有的外部凭证」，<b>不属于</b>本 SDK 的多应用令牌管理面，
/// 故本接口<b>不声明</b> <c>[Token]</c>、<b>无</b>应用类型父接口与子接口，形态对齐
/// <see cref="IWechatWorkPromotionQrCodeContactSyncService"/> 与 <see cref="IWechatWorkProviderAuthenticationUrl"/>
/// 的「显式 Query 传令牌」例外；相应地<b>不进入</b>契约守卫 G5（Query 令牌注入白名单）与令牌归属域守卫 TO 系列。
/// 两支凭证参数名含 <c>token</c>，由 G7 的组件脱敏词表 / 豁免清单裁定（见该守卫）。
/// </para>
/// <para>
/// <b>凭证获取与刷新由宿主负责</b>：<c>model_ticket</c> 由企业微信以「设备型号 model_ticket 推送事件」
/// （官方 96053）下发到厂商回调地址，属接收面而非本 HTTP 面，故本域不承载；宿主须自行接收、缓存并
/// 在调用前经 <see cref="GetModelTokenAsync"/> / <see cref="GetDeviceTokenAsync"/> 取令牌后显式传入。
/// </para>
/// <para>
/// <b>官方文档树归属</b>：本域 17 端点全部在现行「智能硬件开发 &gt; 硬件云端接入」目录树内逐页取证
/// （追踪号 P2-QY-OPENHW-TOKENFACE-04，2026-10-13 裁定；2026-10-18 逐页复核补齐二维码 2 支并更正
/// 缺口清单中 <c>get_userinfo_by_page</c>→95984、<c>model/register_sn</c>→95980 的两处文档号错配）。
/// 官方 <c>service/*</c> 硬件族（旧 90626）与 <c>hardware/get_device_feature</c>（旧 92739）已脱树或正文清空，
/// 由零声明守卫锁定，<b>不得</b>并入本域。
/// </para>
/// </remarks>
[HttpClientApi(HttpClient = nameof(IEnhancedHttpClient), RegistryGroupName = "OpenHardware")]
public interface IWechatWorkOpenHardwareService
{
    /// <summary>
    /// 获取设备型号调用凭证（<c>model_access_token</c>）
    /// <para>以 <c>model_id</c> + <c>model_secret</c> + 企业微信后台推送的 <c>model_ticket</c> 三元组换取
    /// 型号级调用凭证，供本域「型号面」端点以 Query 参数传入。</para>
    /// <para><b>本端点无需令牌</b>：官方本端点不带任何 Query 凭证参数，凭证即请求体三元组本身。</para>
    /// <para><b>官方硬约束</b>：<c>model_ticket</c> 由企业微信推送事件滚动更新，取值须用最新一次推送的值；
    /// 用旧 ticket 换取会失败。返回的 <c>model_access_token</c> 最长 512 字节、有效期由 <c>expires_in</c> 给出
    /// （官方示例 7200 秒），宿主须自行缓存并在过期前刷新（本 SDK 不缓存）。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetModelTokenRequest"/>：model_id / model_secret / model_ticket）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>型号调用凭证与有效期。</returns>
    /// <remarks>
    /// <para><b>硬件云端接入</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95992"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// <para><b>凭据保密</b>：请求体的 <c>model_secret</c> / <c>model_ticket</c> 与应答的
    /// <c>model_access_token</c> 均不得写入日志、遥测或异常消息。</para>
    /// </remarks>
    [Post("/cgi-bin/openhw/get_model_token")]
    Task<GetModelTokenResponse?> GetModelTokenAsync(
        [Body] GetModelTokenRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取设备授权密钥（消费一次性 <c>auth_code</c>）
    /// <para>企业绑定设备时企业微信回调厂商的授权 <c>auth_code</c>，据此换取该设备的长期
    /// <c>device_secret</c> 与首个 <c>device_access_token</c>。</para>
    /// <para><b>官方硬约束（不可重试）</b>：<c>auth_code</c> 长度 64~512 字节且<b>仅支持消费一次</b>，
    /// 重复调用必失败；故本 SDK 不做 errcode 驱动的重试包装。</para>
    /// <para><b>危险操作约束</b>：<c>device_secret</c> 是设备长期授权密钥（泄露即可冒名换取设备凭证），
    /// 官方要求由厂商安全存储；本 SDK 只在应答中透传、不落地、不缓存。</para>
    /// </summary>
    /// <param name="modelAccessToken">设备型号调用凭证（Query 显式传入，见 <see cref="GetModelTokenAsync"/>）。</param>
    /// <param name="request">请求体（<see cref="GetDeviceSecretRequest"/>：auth_code）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>设备授权密钥 + 设备调用凭证 + 凭证有效期。</returns>
    /// <remarks>
    /// <para><b>硬件云端接入</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95993"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/openhw/get_device_secret")]
    Task<GetDeviceSecretResponse?> GetDeviceSecretAsync(
        [Query("model_access_token")] string modelAccessToken,
        [Body] GetDeviceSecretRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取设备调用凭证（<c>device_access_token</c>）
    /// <para>以 <c>device_sn</c> + <c>device_secret</c> 换取设备级调用凭证，供本域「设备面」端点以 Query 传入。</para>
    /// <para><b>有效期</b>：官方示例 <c>expires_in</c> 7200 秒，宿主须自行缓存并在过期前刷新（本 SDK 不缓存）。</para>
    /// </summary>
    /// <param name="modelAccessToken">设备型号调用凭证（Query 显式传入）。</param>
    /// <param name="request">请求体（<see cref="GetDeviceTokenRequest"/>：device_sn / device_secret）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>设备调用凭证与有效期。</returns>
    /// <remarks>
    /// <para><b>硬件云端接入</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96022"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/openhw/get_device_token")]
    Task<GetDeviceTokenResponse?> GetDeviceTokenAsync(
        [Query("model_access_token")] string modelAccessToken,
        [Body] GetDeviceTokenRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 录入设备（登记设备 SN 并取得设备静态二维码）
    /// <para>厂商将自有设备 SN 登记到设备型号下；官方应答直接返回该设备的<b>静态二维码</b>内容
    /// （字段名为 <c>qr_code</c>，非 <c>qrcode_content</c>，与二维码族字段名不同，勿混淆）。</para>
    /// <para><b>官方口径</b>：<c>device_sn</c> 全局唯一；重复录入同一 SN 由官方报错，本 SDK 不做幂等封装。</para>
    /// </summary>
    /// <param name="modelAccessToken">设备型号调用凭证（Query 显式传入）。</param>
    /// <param name="request">请求体（<see cref="RegisterDeviceSnRequest"/>：device_sn）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>设备静态二维码内容。</returns>
    /// <remarks>
    /// <para><b>硬件云端接入</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95980"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/openhw/model/register_sn")]
    Task<RegisterDeviceSnResponse?> RegisterDeviceSnAsync(
        [Query("model_access_token")] string modelAccessToken,
        [Body] RegisterDeviceSnRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除设备（注销设备 SN 录入）
    /// <para>官方应答仅 errcode / errmsg，故复用统一响应基底 <see cref="WechatWorkResponse"/>。</para>
    /// <para><b>危险操作约束</b>：注销后该 SN 的设备凭证（<c>device_secret</c> /
    /// <c>device_access_token</c>）随之失去意义，设备需重新录入才能绑定；不可由重试机制无条件重复调用。</para>
    /// </summary>
    /// <param name="modelAccessToken">设备型号调用凭证（Query 显式传入）。</param>
    /// <param name="request">请求体（<see cref="UnregisterDeviceSnRequest"/>：device_sn）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅含 errcode / errmsg。</returns>
    /// <remarks>
    /// <para><b>硬件云端接入</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95981"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/openhw/model/unregister_sn")]
    Task<WechatWorkResponse?> UnregisterDeviceSnAsync(
        [Query("model_access_token")] string modelAccessToken,
        [Body] UnregisterDeviceSnRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取设备详情
    /// <para>按 <c>device_sn</c> 查询型号名、出厂名、静态二维码、绑定状态与绑定企业信息、备注名。</para>
    /// <para><b>官方条件返回</b>：<c>bind_status</c> 为 1（已绑定）时才返回 <c>bind_corpinfo</c> 与
    /// <c>remark_name</c>；<c>remark_name</c> 在用户未设置时返回<b>空字符串</b>而非省略字段。</para>
    /// <para><b>官方结构</b>：业务负载包在顶层 <c>device_detail</c> 对象下，非平铺字段。</para>
    /// </summary>
    /// <param name="modelAccessToken">设备型号调用凭证（Query 显式传入）。</param>
    /// <param name="request">请求体（<see cref="GetDeviceDetailRequest"/>：device_sn）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>设备详情。</returns>
    /// <remarks>
    /// <para><b>硬件云端接入</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95982"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/openhw/device/get_device_detail")]
    Task<GetDeviceDetailResponse?> GetDeviceDetailAsync(
        [Query("model_access_token")] string modelAccessToken,
        [Body] GetDeviceDetailRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新设备状态（在线状态与固件版本上报）
    /// <para>设备面端点，凭证为 <c>device_access_token</c>（官方本页描述文字写「获取方式见获取设备型号调用凭证」，
    /// 但参数名与示例均为 <c>device_access_token</c>，以参数名为准 ⇒ 逐页取证结论）。</para>
    /// <para><b>官方必填</b>：<c>online_status</c>（1-在线，2-离线）与 <c>cur_version</c>（最长 32 字节）；
    /// <c>upgradable_version</c> / <c>upgradable_version_desc</c>（最长 256 字节）为可选。</para>
    /// </summary>
    /// <param name="deviceAccessToken">设备调用凭证（Query 显式传入，见 <see cref="GetDeviceTokenAsync"/>）。</param>
    /// <param name="request">请求体（<see cref="ReportDeviceStatusRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅含 errcode / errmsg。</returns>
    /// <remarks>
    /// <para><b>硬件云端接入</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95983"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/openhw/device/report_device_status")]
    Task<WechatWorkResponse?> ReportDeviceStatusAsync(
        [Query("device_access_token")] string deviceAccessToken,
        [Body] ReportDeviceStatusRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 全量获取设备成员列表（分页 + 通讯录版本号）
    /// <para>设备侧据此增量同步门禁放行名单。<c>next_cursor</c> 为空即表示已拉完；
    /// <c>perm_version</c> 为云端通讯录版本号，配合 <c>only_perm_version=true</c> 可短路成「只问版本号」。</para>
    /// <para><b>官方短路语义</b>：<c>only_perm_version</c> 为 <c>true</c> 时官方<b>忽略其余入参</b>。</para>
    /// <para><b>官方上限</b>：<c>limit</c> 最大 200、默认 100。</para>
    /// </summary>
    /// <param name="deviceAccessToken">设备调用凭证（Query 显式传入）。</param>
    /// <param name="request">请求体（<see cref="GetHardwareUserPageRequest"/>：cursor / limit / only_perm_version）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>分页游标、通讯录版本号与成员列表（官方包一层 <c>userinfo.useritems</c>）。</returns>
    /// <remarks>
    /// <para><b>硬件云端接入</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95984"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// <para><b>个人信息</b>：成员姓名与放行规则属用户身份信息，不得写入日志、遥测或异常消息。</para>
    /// </remarks>
    [Post("/cgi-bin/openhw/device/get_userinfo_by_page")]
    Task<GetHardwareUserPageResponse?> GetHardwareUserByPageAsync(
        [Query("device_access_token")] string deviceAccessToken,
        [Body] GetHardwareUserPageRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取设备中指定成员的信息
    /// <para>按 <c>open_userid</c> + <c>user_type</c> 列表增量拉取成员详情（含门禁放行规则）。</para>
    /// <para><b>官方上限</b>：<c>user_item</c> 最多 100 条；超出由官方报错，本 SDK 不做自动分批。</para>
    /// <para><b>官方应答形态差异</b>：本端点<b>不返回</b> <c>next_cursor</c> / <c>perm_version</c>
    /// （与 <see cref="GetHardwareUserByPageAsync"/> 字段集不同 ⇒ 各建一型，勿共用）。</para>
    /// </summary>
    /// <param name="deviceAccessToken">设备调用凭证（Query 显式传入）。</param>
    /// <param name="request">请求体（<see cref="GetHardwareUserByIdsRequest"/>：user_item 列表）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成员列表（官方包一层 <c>userinfo.useritems</c>）。</returns>
    /// <remarks>
    /// <para><b>硬件云端接入</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96037"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/openhw/device/get_userinfo_by_ids")]
    Task<GetHardwareUserByIdsResponse?> GetHardwareUserByIdsAsync(
        [Query("device_access_token")] string deviceAccessToken,
        [Body] GetHardwareUserByIdsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 上报设备固件升级结果
    /// <para>厂商执行完固件升级后，把结果回写企业微信（升级事件由官方下发，本端点只回结果）。</para>
    /// <para><b>字段名陷阱</b>：请求体 <c>errcode</c> 是<b>设备侧升级结果</b>（0-成功、1-设备离线、
    /// 其他为厂商自定义失败），与响应基类的接口调用返回码同名不同义；官方要求失败时在 <c>errmsg</c>
    /// 简短描述原因。</para>
    /// <para><b>官方必填</b>：<c>oper_id</c>（取自固件升级事件）、请求体 <c>errcode</c>、<c>cur_version</c>。</para>
    /// </summary>
    /// <param name="deviceAccessToken">设备调用凭证（Query 显式传入）。</param>
    /// <param name="request">请求体（<see cref="ReportFirmwareUpgradeResultRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅含 errcode / errmsg。</returns>
    /// <remarks>
    /// <para><b>硬件云端接入</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95999"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/openhw/device/report_firmware_upgrade_result")]
    Task<WechatWorkResponse?> ReportFirmwareUpgradeResultAsync(
        [Query("device_access_token")] string deviceAccessToken,
        [Body] ReportFirmwareUpgradeResultRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 上报门禁通行记录
    /// <para>批量回写成员通行流水（进门 / 出门、人脸 / 二维码 / 远程开门 / 指纹）。</para>
    /// <para><b>官方上限</b>：<c>access_data</c> 单次最多 100 条，超出由官方拒绝，本 SDK 不自动分批。</para>
    /// <para><b>部分失败语义</b>：顶层 <c>errcode</c> 为 0 但 <c>fail_list</c> 非空 ⇒ <b>部分</b>记录失败，
    /// 失败项以 <c>open_userid</c> + <c>user_type</c> + <c>timestamp</c> 三键定位；宿主须自行决定补报策略
    /// （本 SDK 不重放，避免重复通行记录）。</para>
    /// </summary>
    /// <param name="deviceAccessToken">设备调用凭证（Query 显式传入）。</param>
    /// <param name="request">请求体（<see cref="ReportAccessDataRequest"/>：access_data 列表）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>接口调用结果与失败记录列表。</returns>
    /// <remarks>
    /// <para><b>硬件云端接入</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95997"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// <para><b>个人信息</b>：通行记录（成员身份 + 时间 + 通行方式）属个人行踪信息，
    /// 不得写入日志、遥测或异常消息。</para>
    /// </remarks>
    [Post("/cgi-bin/openhw/device/report_access_data")]
    Task<ReportAccessDataResponse?> ReportAccessDataAsync(
        [Query("device_access_token")] string deviceAccessToken,
        [Body] ReportAccessDataRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 上报成员识别信息变化结果
    /// <para>企业微信客户端发起「录入指纹 / 人脸」时回调厂商，厂商执行后回写录入结果。</para>
    /// <para><b>官方二选一形态</b>：<c>oper_id</c> 存在时以其定位事件；<c>oper_id</c> 不存在或为空时，
    /// 官方改取 <c>open_userid</c> + <c>user_type</c> + <c>bio_info_type</c> 三字段。两组形态互斥使用，
    /// 故请求侧一律可空、由宿主按场景填。</para>
    /// <para><b>个人敏感信息</b>：本端点只回传「录入成 / 败 + 生物类型」，官方与本法均不承载任何
    /// 指纹 / 人脸模板数据。</para>
    /// </summary>
    /// <param name="deviceAccessToken">设备调用凭证（Query 显式传入）。</param>
    /// <param name="request">请求体（<see cref="ReportBioInfoResultRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅含 errcode / errmsg。</returns>
    /// <remarks>
    /// <para><b>硬件云端接入</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96000"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/openhw/device/report_bio_info_result")]
    Task<WechatWorkResponse?> ReportBioInfoResultAsync(
        [Query("device_access_token")] string deviceAccessToken,
        [Body] ReportBioInfoResultRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 上报考勤打卡记录
    /// <para>批量回写设备侧打卡流水。</para>
    /// <para><b>官方上限</b>：<c>checkin_data</c> 单次最多 100 条。</para>
    /// <para><b>官方两级失败语义</b>（本页响应说明明示，勿合并处理）：顶层 <c>errcode</c> 非 0 ⇒
    /// <b>整个接口调用失败</b>；顶层 <c>errcode</c> 为 0 但 <c>fail_list</c> 非空 ⇒ <b>部分</b>数据上报失败。</para>
    /// </summary>
    /// <param name="deviceAccessToken">设备调用凭证（Query 显式传入）。</param>
    /// <param name="request">请求体（<see cref="ReportCheckinDataRequest"/>：checkin_data 列表）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>接口调用结果与失败记录列表。</returns>
    /// <remarks>
    /// <para><b>硬件云端接入</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95985"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// <para><b>个人信息</b>：打卡记录属成员考勤信息，不得写入日志、遥测或异常消息。</para>
    /// </remarks>
    [Post("/cgi-bin/openhw/device/report_checkin_data")]
    Task<ReportCheckinDataResponse?> ReportCheckinDataAsync(
        [Query("device_access_token")] string deviceAccessToken,
        [Body] ReportCheckinDataRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 上报远程开门结果
    /// <para>用户发起远程开门 / 扫码开门后，官方回调厂商携带操作 id，厂商执行完毕后回写结果。</para>
    /// <para><b>官方必填</b>：<c>oper_id</c>、<c>type</c>（上报类型字符串，官方示例
    /// <c>remote_open_door</c> 等，拼写照抄官方原文）、请求体 <c>errcode</c>（0-成功，1-失败）。</para>
    /// </summary>
    /// <param name="deviceAccessToken">设备调用凭证（Query 显式传入）。</param>
    /// <param name="request">请求体（<see cref="ReportRemoteOpenResultRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅含 errcode / errmsg。</returns>
    /// <remarks>
    /// <para><b>硬件云端接入</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96048"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/openhw/device/report_remote_open_result")]
    Task<WechatWorkResponse?> ReportRemoteOpenResultAsync(
        [Query("device_access_token")] string deviceAccessToken,
        [Body] ReportRemoteOpenResultRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 上报体温检测记录
    /// <para>批量回写测温流水。</para>
    /// <para><b>官方字段类型陷阱</b>：<c>temperature</c> 官方类型为 <b>string</b>（非数值），
    /// 本 SDK 照抄官方契约，避免精度与格式丢失。</para>
    /// <para><b>官方上限</b>：<c>temperature_data</c> 单次最多 100 条；失败项在本族中<b>逐条携带</b>
    /// <c>errcode</c> / <c>errmsg</c>（门禁与打卡两族的失败项仅带定位三键 ⇒ 三族响应型分别建型）。</para>
    /// </summary>
    /// <param name="deviceAccessToken">设备调用凭证（Query 显式传入）。</param>
    /// <param name="request">请求体（<see cref="ReportTemperatureDataRequest"/>：temperature_data 列表）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>接口调用结果与失败记录列表。</returns>
    /// <remarks>
    /// <para><b>硬件云端接入</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95986"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// <para><b>健康生理信息</b>：体温读数与「发热 / 温度过低」状态属个人敏感信息，
    /// 不得写入日志、遥测或异常消息，亦不得作为缓存键。</para>
    /// </remarks>
    [Post("/cgi-bin/openhw/device/report_temperature_data")]
    Task<ReportTemperatureDataResponse?> ReportTemperatureDataAsync(
        [Query("device_access_token")] string deviceAccessToken,
        [Body] ReportTemperatureDataRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取设备管理二维码（静态 / 动态同一端点，以 <c>type</c> 分派）
    /// <para>返回设备管理二维码 url，厂商自行渲染成图供企业管理员扫码绑定。</para>
    /// <para><b>官方硬约束</b>：<c>type</c> 为 1（动态二维码）时有效期固定 <b>300 秒</b>（5 分钟）后过期，
    /// 官方在应答中以 <c>expires_in</c> 返回该值；<c>type</c> 为 0（静态）时官方应答不含 <c>expires_in</c>
    /// ⇒ 同一端点两形态共用一型，字段是否出现由官方决定。</para>
    /// <para><c>state</c> 由扫码事件透传回设备，用于厂商侧关联扫码上下文。</para>
    /// </summary>
    /// <param name="deviceAccessToken">设备调用凭证（Query 显式传入）。</param>
    /// <param name="request">请求体（<see cref="GenDeviceLoginQrcodeRequest"/>：state / type）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>二维码 url 与（动态形态下的）有效期。</returns>
    /// <remarks>
    /// <para><b>硬件云端接入</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98023"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/openhw/device/gen_login_qrcode")]
    Task<GenDeviceQrcodeResponse?> GenDeviceLoginQrcodeAsync(
        [Query("device_access_token")] string deviceAccessToken,
        [Body] GenDeviceLoginQrcodeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取设备身份识别二维码（动态，供设备侧展示、成员扫码识别身份）
    /// <para><b>凭证层级为型号面</b>：官方本页凭证为 <c>model_access_token</c>（而非同前缀其余
    /// <c>/device/*</c> 端点的 <c>device_access_token</c>）⇒ 逐页取证结论，勿按路由前缀推断令牌。</para>
    /// <para><b>官方硬约束</b>：动态二维码固定 300 秒过期；<c>state</c> 由扫码事件透传回设备。</para>
    /// </summary>
    /// <param name="modelAccessToken">设备型号调用凭证（Query 显式传入）。</param>
    /// <param name="request">请求体（<see cref="GenDeviceIdQrcodeRequest"/>：device_sn / state）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>二维码 url 与有效期。</returns>
    /// <remarks>
    /// <para><b>硬件云端接入</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98024"/></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/openhw/device/gen_id_dynamic_qrcode")]
    Task<GenDeviceQrcodeResponse?> GenDeviceIdQrcodeAsync(
        [Query("model_access_token")] string modelAccessToken,
        [Body] GenDeviceIdQrcodeRequest request,
        CancellationToken cancellationToken = default);
}
