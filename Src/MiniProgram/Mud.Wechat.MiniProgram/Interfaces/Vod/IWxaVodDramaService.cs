// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「短剧媒资 · 剧目提审」域 SDK（7 端点：剧目提审 / 获取剧目列表 / 获取剧目信息 /
/// 提交替换剧集审核 / 替换审核通过的剧集 / 修改剧目基本信息 / 查询剧目审核信息；官方 minidrama 分区 → auditdrama）。
/// </summary>
/// <remarks>
/// <para>
/// <b>四条审核流与 <c>audit_type</c> 一一对应</b>（官方 <see cref="WxaVodDramaAuditTypes"/> 枚举）：
/// <c>0</c> 首次提审 = <see cref="AuditDramaAsync"/>、<c>1</c> 再次提审 = 同端点的重提形态、
/// <c>2</c> 替换剧集提审 = <see cref="SubmitReplaceDramaMediasAsync"/>、
/// <c>3</c> 修改基本信息审核 = <see cref="ModifyDramaBasicInfoAsync"/> ⇒
/// <see cref="GetDramaLatestAuditInfoAsync"/> 的 <c>audit_type</c> 必须按要查的那条流程传，传错拿到的是另一条流的结论。
/// </para>
/// <para>
/// <b>令牌形态（MP-X2 / MP-X3 同口径）</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入
/// <c>access_token</c>（权限集 153）。域构成、适用范围段异常留档与全域 20 错误码共用表的说明见
/// <see cref="IWxaVodUploadService"/>。
/// </para>
/// <para>
/// <b>两套状态不可互换</b>：剧目整体状态 <see cref="WxaVodDramaStatuses"/>（0 正常可播 / 1 审核中 / 2 审核失败 / 3 平台下架）
/// 与审核流水状态 <see cref="WxaVodDramaAuditStatuses"/>（0 无效值 / 1 审核中 / 2 最终失败 / 3 审核通过 / 4 驳回重填）
/// 是官方两张不同的表，媒资侧还有第三张（<see cref="WxaVodMediaAuditStatuses"/>）⇒ 三个枚举分立建模，守卫 DV5 锁定各自值集。
/// </para>
/// <para>
/// <b>临时素材 <c>material_id</c> 的账号一致性</b>（官方原文，提审与修改两页均给出）：本域使用的图片
/// <c>material_id</c> 由「新增临时素材」接口上传得到，<b>调用素材接口的小程序账号必须与提审账号同一个</b>，
/// 否则提交审核时无法识别素材 id。
/// </para>
/// <para><b>调用限制</b>：七端点官方均未公布额度数字；审核结果通过事件通知下发（官方原文「审核完成后会发送事件通知」）。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Vod", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaVodDramaService
{
    /// <summary>
    /// 剧目提审（<c>POST /wxa/sec/vod/auditdrama</c>，不支持云调用）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/auditdrama/api_auditdrama.html"/>。
    /// </summary>
    /// <param name="request">请求体（官方 24 字段，其中 10 字段首次提审必填），见 <see cref="WxaVodAuditDramaRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>剧目 ID（<c>drama_id</c>），见 <see cref="WxaVodAuditDramaResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + JSON 请求体；<b>必填面随提审次数变化</b>（官方原文）：剧目信息与审核材料
    /// 「首次提审时为必填，重新提审时根据是否需要修改选填」⇒ DTO 一律可空承载，必填校验属宿主业务层而非类型层。
    /// </para>
    /// <para>
    /// <b>备案机制时间线（官方原文，逐条照录）</b>：2024 年 5 月 27 日起针对制作成本 30 万元以下的微短剧需上传
    /// 《成本配置比例情况报告》（<c>cost_commitment_letter_material_id</c>），<b>2026 年 1 月 14 日起成本阈值调整为 100 万元以下</b>，
    /// 剧目经平台审核后由平台下发备案号（<c>registration_number</c>，备案号仅适用微信小程序平台）。
    /// </para>
    /// <para>
    /// <b><c>drama_type</c> 决定演员信息是否必填</b>：1 漫剧 / 2 真人 / <b>3 数字真人（2026 年 4 月 20 日 11:00 起生效）</b>，
    /// 漫剧与数字真人<b>均无需</b>填写 <c>actor_list</c>，真人剧目须填 <b>2~5 位</b>演员（官方原文）。
    /// </para>
    /// <para>
    /// <b>版权保护子表的日期开关</b>：<c>apply_for_copyright_protection</c> 自 2026 年 7 月 28 日 12:00 起默认传 1
    /// 且不支持修改；<c>other_platform_publication_proof</c> 同日起生效。逐字段口径见 <see cref="WxaVodCopyright"/>。
    /// </para>
    /// <para><b><c>media_count</c> 重提时若填必须与首次一致</b>（官方原文），剧集数与 <c>media_id_list</c> 元素个数在首次提审时必须相等。</para>
    /// <para><b>剧集替换推荐用 <c>replace_media_list</c></b>（官方原文「推荐使用 replace_media_list 进行替换，避免顺序和原列表不一致」）而非重传整个 <c>media_id_list</c>。</para>
    /// <para>官方错误码：<c>10090041</c>（剧目名称不符合规范）/ <c>10090040</c>（剧集已经被占用）。</para>
    /// </remarks>
    [Post("/wxa/sec/vod/auditdrama")]
    Task<WxaVodAuditDramaResponse> AuditDramaAsync(
        [Body] WxaVodAuditDramaRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取剧目列表（<c>POST /wxa/sec/vod/listdramas</c>，不支持云调用）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/auditdrama/api_listdramas.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>limit</c> 官方最大值 100 + <c>offset</c>，均选填），见 <see cref="WxaVodListDramasRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>剧目信息列表（<c>drama_info_list</c>），见 <see cref="WxaVodListDramasResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体（只读端点官方仍用 POST）。</para>
    /// <para><b>图片链接均为临时链接</b>（官方原文「不应将其保存下来」）⇒ 落库只存 <c>drama_id</c>。</para>
    /// <para>
    /// <b>列表元素是剧集 id 而非剧集对象</b>：<c>media_list</c> 官方示例形态为 <c>[{"media_id":10001}]</c>，
    /// 逐集审核详情须走媒资侧（<see cref="IWxaVodMediaService.ListMediaAsync"/>）——官方原文即给出该指引。
    /// </para>
    /// </remarks>
    [Post("/wxa/sec/vod/listdramas")]
    Task<WxaVodListDramasResponse> ListDramasAsync(
        [Body] WxaVodListDramasRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取剧目信息（<c>POST /wxa/sec/vod/getdrama</c>，不支持云调用）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/auditdrama/api_getdrama.html"/>。
    /// </summary>
    /// <param name="request">请求体（只有 <c>drama_id</c> 一字段，必填），见 <see cref="WxaVodGetDramaRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>剧目信息（<c>drama_info</c>：状态 / 剧集列表 / 演员信息 / 审核详情），见 <see cref="WxaVodGetDramaResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体。</para>
    /// <para>
    /// <b>本端点是「可播」判定的唯一入口</b>：<c>drama_info.status</c> 见 <see cref="WxaVodDramaStatuses"/>，
    /// 官方口径 0 才「正常可播」；<c>audit_detail.status</c> 只是最近一次审核流水结论，两者不得混用。
    /// </para>
    /// <para><b>演员信息为包装形态</b>：官方 <c>actor_list</c> 是含 <c>actor</c> 数组的对象而非裸数组（提审与查询四处同形），见 <see cref="WxaVodActorList"/>。</para>
    /// <para>与列表端点同理：剧目审核失败/驳回时逐集理由与截图只在媒资侧接口给出（官方原文）。</para>
    /// </remarks>
    [Post("/wxa/sec/vod/getdrama")]
    Task<WxaVodGetDramaResponse> GetDramaAsync(
        [Body] WxaVodGetDramaRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 提交替换剧集审核（<c>POST /wxa/sec/vod/submitreplacedramamedias</c>，不支持云调用）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/auditdrama/api_submitreplacemedias.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>drama_id</c> + <c>replace_media_list</c>，均必填），见 <see cref="WxaVodSubmitReplaceDramaMediasRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>（替换结论与理由须经 <see cref="GetDramaLatestAuditInfoAsync"/> 或媒资侧回读）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体；<b>前置条件</b>（官方原文）：剧目必须<b>审核通过</b>后才允许提交替换剧集审核。</para>
    /// <para>
    /// <b>这是「送审」不是「替换」</b>：本端点只提交审核（对应 <c>audit_type = 2</c>），审核完成后才生效；
    /// 已审核通过的剧集之间的即时替换是 <see cref="ReplaceDramaMediaAsync"/>，两端口径不同勿混用。
    /// </para>
    /// <para><b>异步结论</b>（官方原文）：审核完成后会发送事件通知 ⇒ 宿主须准备事件回调或轮询审核信息。</para>
    /// <para><b>与提审端点共用 <c>replace_media_list</c> 元素类型</b>：<c>old</c> / <c>new</c> 两字段逐字段相同 ⇒ <see cref="WxaVodReplaceMediaItem"/>，守卫 DV4 锁定。</para>
    /// </remarks>
    [Post("/wxa/sec/vod/submitreplacedramamedias")]
    Task<WxaResponse> SubmitReplaceDramaMediasAsync(
        [Body] WxaVodSubmitReplaceDramaMediasRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 替换审核通过的剧集（<c>POST /wxa/sec/vod/replacedramamedia</c>，不支持云调用）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/auditdrama/api_replacedramamedia.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>drama_id</c> + <c>old_media_id</c> + <c>new_media_id</c>，均必填），见 <see cref="WxaVodReplaceDramaMediaRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + JSON 请求体；<b>双前置条件</b>（官方原文）：剧目必须审核通过，<b>而且新剧集也必须审核通过</b>
    /// 才允许替换 ⇒ 与新剧集的媒资审核状态判据见 <see cref="IWxaVodMediaService.GetMediaAsync"/>。
    /// </para>
    /// <para>
    /// <b>字段名形态与 <c>submitreplacedramamedias</c> 不同</b>：本端点用 <c>old_media_id</c> / <c>new_media_id</c>
    /// 两个平铺字段（单集替换），那端用 <c>replace_media_list</c> 数组（批量送审）⇒ 不共用 DTO，守卫 DV4 锁定两侧字段面。
    /// </para>
    /// </remarks>
    [Post("/wxa/sec/vod/replacedramamedia")]
    Task<WxaResponse> ReplaceDramaMediaAsync(
        [Body] WxaVodReplaceDramaMediaRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改剧目基本信息（<c>POST /wxa/sec/vod/modifydramabasicinfo</c>，不支持云调用）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/auditdrama/api_submitmodifydramabasicinforeq.html"/>。
    /// </summary>
    /// <param name="request">请求体（15 字段，仅 <c>drama_id</c> 必填，其余 14 选填），见 <see cref="WxaVodModifyDramaBasicInfoRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>；修改须重新审核，结论走 <see cref="GetDramaLatestAuditInfoAsync"/>（<c>audit_type = 3</c>）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体；<b>前置条件</b>（官方原文）：剧目必须已经审核通过；审核完成后会发送事件通知。</para>
    /// <para>
    /// <b>页面文件名与路由不同名（照录官方）</b>：官方页文件名为 <c>api_submitmodifydramabasicinforeq</c>，
    /// 正文标题与路由为「修改剧目基本信息」<c>modifydramabasicinfo</c> ⇒ 文档链接取页面、方法名取路由。
    /// </para>
    /// <para>
    /// <b>版权子表必填面比提审侧松</b>：本页 <c>copyright_verification</c> / <c>proof_of_production</c> 官方列作选填，
    /// 提审侧列必填 ⇒ 两页共用 <see cref="WxaVodCopyright"/>，差异由宿主按场景赋值承载（守卫 DV4 锁定该共用裁决）。
    /// </para>
    /// <para><b>不可修改项</b>：官方请求体表内没有 <c>name</c> / <c>media_count</c> / <c>media_id_list</c> / <c>drama_type</c> ⇒ 剧目正名与剧集构成不经本端点变更（<c>alternate_name</c>「备用剧名」可改）。</para>
    /// </remarks>
    [Post("/wxa/sec/vod/modifydramabasicinfo")]
    Task<WxaResponse> ModifyDramaBasicInfoAsync(
        [Body] WxaVodModifyDramaBasicInfoRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询剧目审核信息（<c>POST /wxa/sec/vod/getdramalatestauditinfo</c>，不支持云调用）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/auditdrama/api_getdramalatestauditinfo.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>drama_id</c> + <c>audit_type</c>，均必填），见 <see cref="WxaVodGetDramaLatestAuditInfoRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>最近一次审核详情（<c>audit_detail</c>），见 <see cref="WxaVodGetDramaLatestAuditInfoResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体；官方标注「本接口无特殊注意事项」。</para>
    /// <para>
    /// <b><c>audit_type</c> 是流程选择器</b>（必填，取值见 <see cref="WxaVodDramaAuditTypes"/>）：本端点回答的是
    /// 「<b>那条流程</b>的最近一次结论」，不传对流程会拿到无关结论 ⇒ 与 <see cref="GetDramaAsync"/> 的
    /// <c>drama_info.audit_detail</c>（剧目当前态）不等价。
    /// </para>
    /// <para><b>只给整剧结论</b>：逐集驳回理由与证据截图仍须回媒资侧接口取（官方在列表/详情两页给出的指引）。</para>
    /// </remarks>
    [Post("/wxa/sec/vod/getdramalatestauditinfo")]
    Task<WxaVodGetDramaLatestAuditInfoResponse> GetDramaLatestAuditInfoAsync(
        [Body] WxaVodGetDramaLatestAuditInfoRequest request,
        CancellationToken cancellationToken = default);
}
