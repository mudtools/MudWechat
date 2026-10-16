// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Fapiao;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「电子发票」域 SDK（APIv3 区块链电子发票，<b>18 端点</b>：开具 + 查询 + 冲红 +
/// 获取发票下载信息 + 插入用户卡包（直连商户面 5 支），
/// 以及 P1-5-B 补入的<b>服务商 / 数电发票面 13 支</b>（卡券模板、通用行业与不动产租赁开具、
/// 能力邀请、商户开票信息与开发选项、税收分类编码、用户抬头）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>（<b>普通商户</b>文档中心，2026-10-09 逐页核验，更新时间均为 2025.09.26）：
/// 开具 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012538301"/>、
/// 查询 <see href="https://pay.weixin.qq.com/docs/merchant/apis/fapiao/fapiao-applications/get-fapiao-applications.html"/>、
/// 冲红 <see href="https://pay.weixin.qq.com/docs/merchant/apis/fapiao/fapiao-applications/reverse-fapiao-applications.html"/>、
/// 获取下载信息 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012538335"/>、
/// 插入卡包 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012538365"/>。
/// 各页均标注<b>支持商户：【普通商户】</b>。
/// </para>
/// <para>
/// <b>✅ 更正设计方案 §2.5 的一条记录（并二次更正其「不覆盖」裁定）</b>：该处记「电子发票 · 普通商户文档中心
/// <b>无独立入口</b>；服务商侧 <c>partner/4015792574</c>」—— <b>实测不成立</b>：普通商户侧有完整电子发票文档
/// （本域各页均在其内）。服务商侧另有一套「数电发票 / 服务商电子发票」开放面
/// （开具<b>通用行业</b>电子发票 <c>/fapiao-applications/<b>issue-general</b></c>、能力邀请、受邀商户查询等，
/// 与商户侧路由不同）—— <b>P1-5-B 已按官方逐页取证纳入本接口</b>（13 支，守卫
/// <c>WechatPayFapiaoExtensionContractGuards</c> FPX-B1~B6），原「属另一套面，本域不覆盖」的裁定已作废。
/// </para>
/// <para>
/// <b>服务商面与商户面的三处形态差异</b>（取证结论，逐支写在方法 remarks 里）：
/// ① 同一支路由官方常印<b>商户页 + 服务商页</b>两页，字段名册取<b>并集</b>建模、必填口径不一致只在注释标注，
/// <b>一面一方法</b>（不因身份拆建）；② 服务商面的子商户定位键<b>不统一</b>——
/// 多数支用 query/path <c>sub_mchid</c>，但「查询/配置开发选项」两支用 <c>sub_mch_code</c>，
/// 照页不套，不得凭同名语义互换；③ 服务商面新支的应答节多为<b>「无应答包体」</b>（含两支开具与一支校验）
/// ⇒ 返回裸 <see cref="Task"/>，业务结果由异步通知或查询支取得。
/// </para>
/// <para>
/// <b>两支开具接口只是「受理」</b>：<c>issue-general</c> 与 <c>real-estate-leasing</c> 的官方应答节原文均为
/// <b>「无应答包体」</b>（正常示例状态码 <b>202</b>）⇒ 返回裸 <see cref="Task"/>；
/// 开票结果须经【查询电子发票】或开票回调取得，<b>不得</b>据 HTTP 2xx 判定开票成功。
/// 「检查子商户电子发票功能状态」同款（原文「无应答包体」，正常示例 <b>204</b>）。
/// </para>
/// <para>
/// <b>⚠️ 两个非 JSON 端点<b>刻意不在本生成式接口内</b></b>（它们不是「漏实现」）：
/// </para>
/// <list type="bullet">
/// <item>
/// <b>上传电子发票文件</b>（<c>POST /v3/new-tax-control-fapiao/fapiao-applications/upload-fapiao-file</c>，已核）：
/// 官方契约为 <c><b>multipart/form-data</b></c>（表单字段 <c>file</c> + <c>meta</c>），
/// 且 <c>meta.digest</c> 要求 <b>SM3</b> 摘要（官方字段名写作 <c>digest_alogrithm</c>，<b>少一个 r，属官方拼写</b>）——
/// <b>SM3 不在 .NET BCL 内</b>，须由调用方计算后传入 ⇒ 声明式 <c>[Body]</c> JSON 端点无法承载。
/// </item>
/// <item>
/// <b>下载发票文件</b>：官方注明该下载<b>不支持签名和验签</b>，且 URL 由「获取下载信息」动态给出
/// （官方《下载发票文件》页称有效期 <b>30s</b>）⇒ 与账单下载同属「动态 URL + 非 JSON」形态。
/// </item>
/// </list>
/// <para>
/// 当前落点：<see cref="GetFapiaoFilesAsync"/> 只<b>返回</b> <c>download_url</c>，宿主可自行取用；
/// 待后续按账单下载通道（<c>IWechatPayBillDownloadService</c>）的同款模式单独建模
/// （含主机白名单前置闸）。<b>不要凭推断补路由</b>。
/// </para>
/// <para>
/// <b>通知已覆盖（归回调包）</b>：<c>FAPIAO.USER_APPLIED</c>（抬头填写完成）、
/// <c>FAPIAO.ISSUED</c>（开具成功）、<c>FAPIAO.CARD_INSERTED</c>（插卡成功）、
/// <c>FAPIAO.REVERSED</c>（冲红成功）、<c>FAPIAO.CARD_DISCARDED</c>（卡券作废）——
/// 见 <c>WechatPayCallbackContext.GetFapiao</c> / <c>GetFapiaoUserApplied</c>。
/// </para>
/// <para>
/// <b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1）：形态与
/// <see cref="IWechatPayTransactionsService"/> 一致，详见其 remarks。
/// </para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "NewTaxControlFapiao", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayFapiaoService
{
    /// <summary>
    /// 开具电子发票。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012538301"/>。
    /// </summary>
    /// <param name="request">开票请求体，见 <see cref="FapiaoIssueRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/new-tax-control-fapiao/fapiao-applications</c>；无 path / query 参数。</para>
    /// <para>
    /// <b>返回类型为何是 <c>Task</c></b>：官方标注应答状态 <b>202 Accepted</b> 且<b>无任何应答字段</b>
    /// ⇒ 与 204 类端点同款处置（声明为无返回值，避免调用方去解一个不存在的包体）。
    /// </para>
    /// <para>
    /// <b>异步语义（官方原文）</b>：成功返回<b>仅代表开票请求已被受理</b>；
    /// 开票完成须经<b>回调通知</b>或 <see cref="QueryFapiaoApplicationsAsync"/> 获取 ——
    /// <b>不得</b>把 202 当成开票成功。
    /// </para>
    /// </remarks>
    [Post("/v3/new-tax-control-fapiao/fapiao-applications")]
    Task IssueFapiaoAsync(
        [Body] FapiaoIssueRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询电子发票。官方文档：<see href="https://pay.weixin.qq.com/docs/merchant/apis/fapiao/fapiao-applications/get-fapiao-applications.html"/>。
    /// </summary>
    /// <param name="fapiaoApplyId">发票申请单号（官方 path <c>fapiao_apply_id</c>，必填 string(32)）。</param>
    /// <param name="fapiaoId">商户发票单号（官方 query <c>fapiao_id</c>，<b>选填</b> string(32)）：用于在申请单内过滤单张发票。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>发票列表与开票结果，见 <see cref="FapiaoQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/new-tax-control-fapiao/fapiao-applications/{fapiao_apply_id}?fapiao_id=…</c>；
    /// path 必带 <c>fapiao_apply_id</c>，query 的 <c>fapiao_id</c> <b>选填</b>。</para>
    /// <para><b>官方建议</b>：「【将电子发票插入微信用户卡包】接口成功后，应调用本接口查询电子发票开票结果」。</para>
    /// <para>
    /// <b>✅ 值表可交叉引用（原「值域未核验」留档已关闭）</b>：本应答的 <c>status</c> 与
    /// <c>card_information.card_status</c> 取值见常量 <c>FapiaoStatuses</c> / <c>FapiaoCardStatuses</c>
    /// —— 二者由官方<b>四张通知页</b>（开具成功 / 插卡成功 / 冲红成功 / 卡券作废，字段表逐项一致）逐字取得。
    /// </para>
    /// <para>
    /// <b>⚠️ 保留一处边界（不夸大核验范围）</b>：本轮<b>未</b>在「查询电子发票」页本身找到单独的取值表
    /// （该页只给出字段名）⇒ 常量与该页字段的对应是「<b>同名字段、同域取值</b>」的合理映射，
    /// 而非该页逐字确认。若联调发现查询页返回常量之外的取值，须先核验官方再补，<b>勿臆造</b>。
    /// </para>
    /// </remarks>
    [Get("/v3/new-tax-control-fapiao/fapiao-applications/{fapiaoApplyId}")]
    Task<FapiaoQueryResponse> QueryFapiaoApplicationsAsync(
        [Path] string fapiaoApplyId,
        [Query("fapiao_id")] string? fapiaoId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 冲红电子发票。官方文档：
    /// <see href="https://pay.weixin.qq.com/docs/merchant/apis/fapiao/fapiao-applications/reverse-fapiao-applications.html"/>。
    /// </summary>
    /// <param name="fapiaoApplyId">发票申请单号（官方 path <c>fapiao_apply_id</c>，必填 string(32)）。</param>
    /// <param name="request">冲红请求体，见 <see cref="FapiaoReverseRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/new-tax-control-fapiao/fapiao-applications/{fapiao_apply_id}/reverse</c>；
    /// 更新时间 2025.09.26，<b>支持商户：【普通商户】</b>。</para>
    /// <para>
    /// <b>无应答包体</b>：官方标注 <b>202 Accepted</b> 且无任何应答字段 ⇒ 返回 <c>Task</c>。
    /// 官方原文：「本接口成功返回<b>仅代表冲红请求已被受理</b>，当冲红完成时，微信支付会根据商户配置的
    /// 回调地址进行回调通知，商户也可以通过【查询电子发票】接口获取冲红结果及红票信息」。
    /// </para>
    /// <para>
    /// <b>重试约束（官方原文）</b>：红字发票开具<b>失败</b>时，可用<b>相同的</b>
    /// <c>fapiao_information.fapiao_id</c> 重新填写冲红原因<b>重试</b>，<b>每次重试间隔为 5 分钟</b>。
    /// </para>
    /// <para>
    /// <b>前置约束</b>：<b>仅</b>在微信支付侧开具的电子发票才允许冲红；冲红会同时把发票从用户卡包中删除。
    /// </para>
    /// </remarks>
    [Post("/v3/new-tax-control-fapiao/fapiao-applications/{fapiaoApplyId}/reverse")]
    Task ReverseFapiaoAsync(
        [Path] string fapiaoApplyId,
        [Body] FapiaoReverseRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取发票下载信息。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012538335"/>。
    /// </summary>
    /// <param name="fapiaoApplyId">发票申请单号（官方 path <c>fapiao_apply_id</c>，必填 string(32)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>发票下载信息列表（含 <c>download_url</c>），见 <see cref="FapiaoFilesResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/new-tax-control-fapiao/fapiao-applications/{fapiao_apply_id}/fapiao-files</c>；
    /// 仅 path 参数，无 body / query（更新时间 2025.09.26）。</para>
    /// <para>
    /// <b>⚠️ 有下载链接才算真能下载</b>：官方原文「开票状态为 <c>ISSUED</c> 的发票才能获取到发票文件下载链接」
    /// ⇒ <c>download_url</c> <b>仅当</b>该张发票 <c>status = ISSUED</c> 时存在（其它状态下为 <c>null</c>），
    /// 消费侧<b>不得</b>假定列表里每项都有链接。
    /// </para>
    /// <para>
    /// <b>本端点只返回地址，不下载</b>：文件须再用官方《下载发票文件》按其 <c>download_url</c> 取回
    /// （该下载<b>不支持签名和验签</b>，与普通 APIv3 请求不同）。本域暂未建模下载动作，
    /// 故 <c>download_url</c> 须由宿主自行取用（<b>严禁</b>改写或拼接该 URL）。
    /// </para>
    /// </remarks>
    [Get("/v3/new-tax-control-fapiao/fapiao-applications/{fapiaoApplyId}/fapiao-files")]
    Task<FapiaoFilesResponse> GetFapiaoFilesAsync(
        [Path] string fapiaoApplyId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 将电子发票插入微信用户卡包。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012538365"/>。
    /// </summary>
    /// <param name="fapiaoApplyId">发票申请单号（官方 path <c>fapiao_apply_id</c>，必填 string(32)）。</param>
    /// <param name="request">插卡请求体，见 <see cref="FapiaoInsertCardsRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/new-tax-control-fapiao/fapiao-applications/{fapiao_apply_id}/insert-cards</c>；
    /// 更新时间 2025.09.26，<b>支持商户：【普通商户】</b>。</para>
    /// <para>
    /// <b>无应答包体</b>：官方标注 <b>202 Accepted</b> ⇒ 返回 <c>Task</c>；官方原文「返回成功仅代表请求被受理，
    /// 插卡完成后由微信支付回调通知，也可通过【查询电子发票】接口获取插卡结果及卡券信息」。
    /// </para>
    /// <para>
    /// <b>两段式前置（官方原文）</b>：调用本接口前<b>必须</b>先调《上传电子发票文件》拿到
    /// <c>fapiao_media_id</c>（<b>三天内有效</b>）—— 该上传接口是 <c>multipart/form-data</c>，
    /// <b>不是</b> JSON 端点，故不在本生成式接口内。
    /// </para>
    /// <para>
    /// <b>非微信支付场景</b>：须先经《获取用户授权链接》取得用户授权后才能调用本接口；
    /// 微信支付场景无需额外授权。
    /// </para>
    /// </remarks>
    [Post("/v3/new-tax-control-fapiao/fapiao-applications/{fapiaoApplyId}/insert-cards")]
    Task InsertFapiaoCardsAsync(
        [Path] string fapiaoApplyId,
        [Body] FapiaoInsertCardsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建电子发票卡券模板。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012529382"/>（【普通商户】）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012474026"/>（【普通服务商】）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4015792562"/>（【普通服务商】）。
    /// </summary>
    /// <param name="request">创建电子发票卡券模板请求体，字段见 <see cref="FapiaoCardTemplateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>创建电子发票卡券模板，见 <see cref="FapiaoCardTemplateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/new-tax-control-fapiao/card-template</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>双页并集</b>：本支路由官方印 3 页（商户页、服务商页、服务商页），Path/Query 与应答均按<b>并集</b>建模，仅某页印出的字段与必填口径差异已在参数注释内逐字段标注 ⇒ 一面一方法，不因身份拆建。</para>
    /// <para><b>应答</b>见 <see cref="FapiaoCardTemplateResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 APPID_MCHID_NOT_MATCH：AppID和mch_id不匹配 / 400 INVALID_REQUEST：请求参数符合参数格式，但不符合业务规则 / 401 SIGN_ERROR：签名错误或签名信息不完整 / 403 NO_AUTH：商户无权限 / 429 FREQUENCY_LIMITED：频率超限 / 400 PARAM_ERROR：参数错误 / 400 INVALID_REQUEST：HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR：验证不通过 / 500 SYSTEM_ERROR：系统异常，请稍后重试 / 400 APPID_MCHID_NOT_MATCH：appid与mchid不匹配。</para>
    /// </remarks>
    [Post("/v3/new-tax-control-fapiao/card-template")]
    Task<FapiaoCardTemplateResponse> SetFapiaoCardTemplateAsync(
        [Body] FapiaoCardTemplateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 开具通用行业电子发票。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015792574"/>（【普通服务商】）。
    /// </summary>
    /// <param name="request">开具通用行业电子发票请求体，字段见 <see cref="FapiaoGeneralIssueRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>官方应答体无业务字段（官方应答节原文「无应答包体」/「无参数返回」，正常示例状态码 <b>202 Accepted</b>） ⇒ 返回裸 <see cref="Task"/>，结果以 HTTP 码 + <c>code</c>/<c>message</c> 判定。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/new-tax-control-fapiao/fapiao-applications/issue-general</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR：参数错误 / 400 INVALID_REQUEST：HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR：验证不通过 / 500 SYSTEM_ERROR：系统异常，请稍后重试 / 400 INVALID_REQUEST：请求参数符合参数格式，但不符合业务规则 / 400 RESOURCE_ALREADY_EXISTS：发票申请单已存在 / 400 INVALID_REQUEST：当前发票开具中，不可申请重开发票 / 400 INVALID_REQUEST：当前发票冲红中，不可申请重开发票 / 400 INVALID_REQUEST：申请开票次数，超过最大限制 / 400 INVALID_REQUEST：开票请求已被受理，请调用【查询电子发票】接口获取开票结果 / 400 INVALID_REQUEST：交易单已申请开票，两次申请的交易金额不相同 / 400 INVALID_REQUEST：交易可剩余开票金额不足 / 403 NO_AUTH：商户乐企不动产租赁发票能力未授权 / 403 NO_AUTH：商户无权限 / 403 RULE_LIMIT：商户尚未配置电子发票卡券模板信息 / 403 RULE_LIMIT：商户不可对该货物或应税劳务、服务编码开具发票 / 403 RULE_LIMIT：商户提交的购买方信息与用户填写的发票抬头不一致 / 403 RULE_LIMIT：微信支付订单所属商户与当前商户不一致（仅微信支付场景） / 404 RESOURCE_NOT_EXISTS：用户尚未完成发票抬头填写（仅非微信支付场景） / 429 FREQUENCY_LIMITED：频率超限。</para>
    /// </remarks>
    [Post("/v3/new-tax-control-fapiao/fapiao-applications/issue-general")]
    Task IssueGeneralFapiaoAsync(
        [Body] FapiaoGeneralIssueRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 开具不动产租赁行业电子发票。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015941740"/>（【普通服务商】）。
    /// </summary>
    /// <param name="request">开具不动产租赁行业电子发票请求体，字段见 <see cref="FapiaoRealEstateLeasingIssueRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>官方应答体无业务字段（官方应答节原文「无应答包体」/「无参数返回」，正常示例状态码 <b>202 Accepted</b>） ⇒ 返回裸 <see cref="Task"/>，结果以 HTTP 码 + <c>code</c>/<c>message</c> 判定。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/new-tax-control-fapiao/fapiao-applications/real-estate-leasing</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR：参数错误 / 400 INVALID_REQUEST：HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR：验证不通过 / 500 SYSTEM_ERROR：系统异常，请稍后重试 / 400 INVALID_REQUEST：请求参数符合参数格式，但不符合业务规则 / 400 RESOURCE_ALREADY_EXISTS：发票申请单已存在 / 400 INVALID_REQUEST：交易单已申请开票，两次申请的交易金额不相同 / 400 INVALID_REQUEST：当前发票开具中，不可申请重开发票 / 400 INVALID_REQUEST：当前发票冲红中，不可申请重开发票 / 400 INVALID_REQUEST：申请开票次数，超过最大限制 / 400 INVALID_REQUEST：开票请求已被受理，请调用【查询电子发票】接口获取开票结果 / 400 INVALID_REQUEST：交易可剩余开票金额不足 / 403 NO_AUTH：商户乐企不动产租赁发票能力未授权 / 403 NO_AUTH：商户无权限 / 403 RULE_LIMIT：商户尚未配置电子发票卡券模板信息 / 403 RULE_LIMIT：商户不可对该货物或应税劳务、服务编码开具发票 / 403 RULE_LIMIT：商户提交的购买方信息与用户填写的发票抬头不一致 / 403 RULE_LIMIT：微信支付订单所属商户与当前商户不一致（仅微信支付场景） / 404 RESOURCE_NOT_EXISTS：用户尚未完成发票抬头填写（仅非微信支付场景） / 429 FREQUENCY_LIMITED：频率超限。</para>
    /// </remarks>
    [Post("/v3/new-tax-control-fapiao/fapiao-applications/real-estate-leasing")]
    Task IssueRealEstateLeasingFapiaoAsync(
        [Body] FapiaoRealEstateLeasingIssueRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取服务商电子发票能力邀请链接。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015941495"/>（【普通服务商】）。
    /// </summary>
    /// <param name="operationType">【操作类型】 商户接受请求后的操作类型。可选取值：`AUTH_BINDING` 授权绑定（官方 query <c>operation_type</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="fapiaoMode">【开票模式】 邀请商户开通服务商电子发票的可选开票模式。可选取值：`THIRD_OR_SELF_FAPIAO` 第三方/自建发票，即使用自有或其他第三方发票系统生成发票 / `TENCENT_DIGITAL_TAX` 腾讯数电发票，即使用腾讯数电发票系统生成发票（官方 query <c>fapiao_mode</c>，<b>必填</b>，类型 <c>array[string]</c>，官方线格式为 URL 编码 JSON ⇒ 以 <c>string</c> 承载）。</param>
    /// <param name="inviteCode">【服务商邀请code】 服务商邀请的唯一标识，由服务商生成，可根据邀请code查询邀请开通情况（官方 query <c>invite_code</c>，<b>必填</b>，类型 <c>string(64)</c>）。</param>
    /// <param name="fapiaoAbilityTypeList">【发票能力类型】 邀请商户开通的腾讯数电发票能力类型，可以选择多个。当开票模式为TENCENT_DIGITAL_TAX时必填。可选取值：`BASE_ABILITY` 基础开票能力（适合餐饮、酒店等） / `REAL_ESTATE_ABILITY` 不动产租赁能力（适合停车场等） / `PASSENGER_TRANSPORT_ABILITY` 旅客运输能力（适合公共交通等行业） / `REFINED_OIL_ABILITY` 成品油能力（适合石油石化等行业）（官方 query <c>fapiao_ability_type_list</c>，选填，类型 <c>array[string]</c>，官方线格式为 URL 编码 JSON ⇒ 以 <c>string</c> 承载）。</param>
    /// <param name="inviteChannel">【服务商邀请渠道】 服务商自定义的渠道信息，用于标识该邀请从哪个渠道发起（官方 query <c>invite_channel</c>，选填，类型 <c>string(128)</c>）。</param>
    /// <param name="operateUser">【操作人员】 服务商邀请操作人员信息，用于标识该邀请是哪个人员发起。 注意：该字段为密文字段，需要使用微信支付公钥加密（推荐），参考获取微信支付公钥ID说明以及微信支付公钥加密敏感信息指引；也可以使用微信支付平台证书公钥加密，参考获取平台证书序列号、平台证书加密敏感信息指引（官方 query <c>operate_user</c>，选填，类型 <c>string</c>）。</param>
    /// <param name="subMchid">【受邀请商户号】 受服务商邀请开通电子发票能力的商户号，受邀商户号和受邀税号不能同时填（官方 query <c>sub_mchid</c>，选填，类型 <c>string(32)</c>）。</param>
    /// <param name="taxpayerId">【受邀请商户税号】 受服务商邀请开通电子发票能力的税号，保证受邀请商户提交的商户信息与提交邀请的税号保持一致，受邀商户号和受邀税号不能同时填（官方 query <c>taxpayer_id</c>，选填，类型 <c>string(20)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>获取服务商电子发票能力邀请链接，见 <see cref="FapiaoSpInviteUrlResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/new-tax-control-fapiao/fapiaomerchant/getspinviteurl</c>；query <c>operation_type</c> / <c>fapiao_mode</c> / <c>invite_code</c> / <c>fapiao_ability_type_list</c> / <c>invite_channel</c> / <c>operate_user</c> / <c>sub_mchid</c> / <c>taxpayer_id</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>operation_type</c> 与 <c>fapiao_mode</c> 与 <c>invite_code</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR：参数错误 / 400 INVALID_REQUEST：HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR：验证不通过 / 500 SYSTEM_ERROR：系统异常，请稍后重试 / 400 INVALID_REQUEST：请求参数符合参数格式，但不符合业务规则 / 403 NO_AUTH：商户无权限 / 429 FREQUENCY_LIMITED：频率超限。</para>
    /// </remarks>
    [Get("/v3/new-tax-control-fapiao/fapiaomerchant/getspinviteurl")]
    Task<FapiaoSpInviteUrlResponse> GetFapiaoSpInviteUrlAsync(
        [Query("operation_type")] string operationType,
        [Query("fapiao_mode")] string fapiaoMode,
        [Query("invite_code")] string inviteCode,
        [Query("fapiao_ability_type_list")] string? fapiaoAbilityTypeList = null,
        [Query("invite_channel")] string? inviteChannel = null,
        [Query("operate_user")] string? operateUser = null,
        [Query("sub_mchid")] string? subMchid = null,
        [Query("taxpayer_id")] string? taxpayerId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取邀请开通的商户信息。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015941524"/>（【普通服务商】）。
    /// </summary>
    /// <param name="queryTimeStart">【查询时间起】 查询邀请商户的起始时间，遵循 RFC3339 标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE（官方 query <c>query_time_start</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="queryTimeEnd">【查询时间止】 查询商户邀请的结束时间，与query_time_start的间隔时间不能大于7天。遵循 RFC3339 标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE（官方 query <c>query_time_end</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="offset">【本次查询的起始位置】 当前查询的分页页数。从0开始计数，如offset=10，表示从第11条记录开始返回（官方 query <c>offset</c>，<b>必填</b>，类型 <c>integer</c>）。</param>
    /// <param name="limit">【本次查询的最大数量】 每页所查询的商户信息数量（官方 query <c>limit</c>，<b>必填</b>，类型 <c>integer</c>）。</param>
    /// <param name="mchInviteStatus">【商户邀请状态】 商户邀请状态。可选取值：`MCH_INVITE_SUCC` 商户已接受邀请，通过 [【检查子商户开票功能状态】](https://pay.weixin.qq.com/doc/v3/partner/4015792561.md) 接口查询具体状态 / `MCH_INVITE_FAILED` 商户邀请开通失败，具体失败原因查看invite_failed_reason字段（官方 query <c>mch_invite_status</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="inviteCode">【服务商邀请code】 邀请链接唯一标识（官方 query <c>invite_code</c>，选填，类型 <c>string</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>获取邀请开通的商户信息，见 <see cref="FapiaoSpInviteMerchantListResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/new-tax-control-fapiao/fapiaomerchant/listspinvitemchinfo</c>；query <c>query_time_start</c> / <c>query_time_end</c> / <c>offset</c> / <c>limit</c> / <c>mch_invite_status</c> / <c>invite_code</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>query_time_start</c> 与 <c>query_time_end</c> 与 <c>offset</c> 与 <c>limit</c> 与 <c>mch_invite_status</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR：参数错误 / 400 INVALID_REQUEST：HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR：验证不通过 / 500 SYSTEM_ERROR：系统异常，请稍后重试 / 400 INVALID_REQUEST：请求参数符合参数格式，但不符合业务规则 / 403 NO_AUTH：商户无权限 / 429 FREQUENCY_LIMITED：频率超限。</para>
    /// </remarks>
    [Get("/v3/new-tax-control-fapiao/fapiaomerchant/listspinvitemchinfo")]
    Task<FapiaoSpInviteMerchantListResponse> ListFapiaoSpInviteMerchantInfoAsync(
        [Query("query_time_start")] string queryTimeStart,
        [Query("query_time_end")] string queryTimeEnd,
        [Query("offset")] int offset,
        [Query("limit")] int limit,
        [Query("mch_invite_status")] string mchInviteStatus,
        [Query("invite_code")] string? inviteCode = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查子商户电子发票功能状态。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012474022"/>（【普通服务商】）。
    /// </summary>
    /// <param name="subMchid">【子商户号】 微信支付分配的子商户号（官方 path <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>官方应答体无业务字段（官方应答节原文「无应答包体」/「无参数返回」，正常示例状态码 <b>204 No Content</b>） ⇒ 返回裸 <see cref="Task"/>，结果以 HTTP 码 + <c>code</c>/<c>message</c> 判定。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/new-tax-control-fapiao/merchant/{sub_mchid}/check</c>；path <c>sub_mchid</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：请求参数符合参数格式，但不符合业务规则 / 401 SIGN_ERROR：签名错误或签名信息不完整 / 403 NO_AUTH：服务商与子商户之间无受理关系，请确认子商户是否对服务商进行相关授权 / 403 NO_AUTH：商户未完成服务商电子发票的开通设置 / 429 FREQUENCY_LIMITED：频率超限。</para>
    /// </remarks>
    [Post("/v3/new-tax-control-fapiao/merchant/{subMchid}/check")]
    Task CheckFapiaoMerchantAsync(
        [Path] string subMchid,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询受邀商户开票能力开通状态。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015792561"/>（【普通服务商】）。
    /// </summary>
    /// <param name="subMchid">【子商户号】 微信支付分配的子商户号，具体请参考 服务商模式开发必要参数说明（官方 path <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询受邀商户开票能力开通状态，见 <see cref="FapiaoMerchantCheckStatusResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/new-tax-control-fapiao/merchant/{sub_mchid}/check-status</c>；path <c>sub_mchid</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR：参数错误 / 400 INVALID_REQUEST：HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR：验证不通过 / 500 SYSTEM_ERROR：系统异常，请稍后重试 / 400 INVALID_REQUEST：请求参数符合参数格式，但不符合业务规则 / 403 NO_AUTH：商户无权限 / 429 FREQUENCY_LIMITED：频率超限。</para>
    /// </remarks>
    [Get("/v3/new-tax-control-fapiao/merchant/{subMchid}/check-status")]
    Task<FapiaoMerchantCheckStatusResponse> QueryFapiaoMerchantCheckStatusAsync(
        [Path] string subMchid,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询商户开票基础信息。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012538180"/>（【普通商户】）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012474053"/>（【普通服务商】）。
    /// </summary>
    /// <param name="subMchid">【子商户号】 微信支付分配的子商户号，服务商模式下必传（官方 query <c>sub_mchid</c>，选填，类型 <c>string(32)</c>，<b>仅服务商页</b>印发）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询商户开票基础信息，见 <see cref="FapiaoMerchantBaseInformationResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/new-tax-control-fapiao/merchant/base-information</c>；query <c>sub_mchid</c>。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>双页并集</b>：本支路由官方印 2 页（商户页、服务商页），Path/Query 与应答均按<b>并集</b>建模，仅某页印出的字段与必填口径差异已在参数注释内逐字段标注 ⇒ 一面一方法，不因身份拆建。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：请求参数符合参数格式，但不符合业务规则 / 401 SIGN_ERROR：签名错误或签名信息不完整 / 403 NO_AUTH：商户无权限 / 429 FREQUENCY_LIMITED：频率超限。</para>
    /// </remarks>
    [Get("/v3/new-tax-control-fapiao/merchant/base-information")]
    Task<FapiaoMerchantBaseInformationResponse> QueryFapiaoMerchantBaseInformationAsync(
        [Query("sub_mchid")] string? subMchid = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询商户开发选项。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012529526"/>（【普通商户】）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012474033"/>（【普通服务商】）。
    /// </summary>
    /// <param name="subMchCode">【子商户号】 微信支付分配的子商户号，服务商为子商户设置开发配置信息时必填（官方 query <c>sub_mch_code</c>，选填，类型 <c>string(32)</c>，<b>仅服务商页</b>印发）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询商户开发选项，见 <see cref="FapiaoMerchantDevelopmentConfigResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/new-tax-control-fapiao/merchant/development-config</c>；query <c>sub_mch_code</c>。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>双页并集</b>：本支路由官方印 2 页（商户页、服务商页），Path/Query 与应答均按<b>并集</b>建模，仅某页印出的字段与必填口径差异已在参数注释内逐字段标注 ⇒ 一面一方法，不因身份拆建。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：请求参数符合参数格式，但不符合业务规则 / 401 SIGN_ERROR：签名错误或签名信息不完整 / 403 NO_AUTH：商户无权限 / 429 FREQUENCY_LIMITED：频率超限。</para>
    /// </remarks>
    [Get("/v3/new-tax-control-fapiao/merchant/development-config")]
    Task<FapiaoMerchantDevelopmentConfigResponse> QueryFapiaoMerchantDevelopmentConfigAsync(
        [Query("sub_mch_code")] string? subMchCode = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 配置商户开发选项。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012529457"/>（【普通商户】）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012474031"/>（【普通服务商】）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4015792563"/>（【普通服务商】）。
    /// </summary>
    /// <param name="request">配置商户开发选项请求体，字段见 <see cref="FapiaoMerchantDevelopmentConfigUpdateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>配置商户开发选项，见 <see cref="FapiaoMerchantDevelopmentConfigUpdateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>PATCH</b> <c>/v3/new-tax-control-fapiao/merchant/development-config</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>双页并集</b>：本支路由官方印 3 页（商户页、服务商页、服务商页），Path/Query 与应答均按<b>并集</b>建模，仅某页印出的字段与必填口径差异已在参数注释内逐字段标注 ⇒ 一面一方法，不因身份拆建。</para>
    /// <para><b>应答</b>见 <see cref="FapiaoMerchantDevelopmentConfigUpdateResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：请求参数符合参数格式，但不符合业务规则 / 401 SIGN_ERROR：签名错误或签名信息不完整 / 403 NO_AUTH：商户无权限 / 429 FREQUENCY_LIMITED：频率超限 / 400 PARAM_ERROR：参数错误 / 400 INVALID_REQUEST：HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR：验证不通过 / 500 SYSTEM_ERROR：系统异常，请稍后重试。</para>
    /// </remarks>
    [Patch("/v3/new-tax-control-fapiao/merchant/development-config")]
    Task<FapiaoMerchantDevelopmentConfigUpdateResponse> UpdateFapiaoMerchantDevelopmentConfigAsync(
        [Body] FapiaoMerchantDevelopmentConfigUpdateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询商户税收分类编码对照表。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012538236"/>（【普通商户】）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012474057"/>（【普通服务商】）。
    /// </summary>
    /// <param name="offset">【本次查询的起始位置】 本次查询的起始位置（官方 query <c>offset</c>，<b>必填</b>，类型 <c>integer</c>）。</param>
    /// <param name="limit">【本次查询的最大数量】 本次查询的最大数量（官方 query <c>limit</c>，<b>必填</b>，类型 <c>integer</c>）。</param>
    /// <param name="subMchid">【子商户号】 微信支付分配的子商户号，服务商模式下必传（官方 query <c>sub_mchid</c>，选填，类型 <c>string(32)</c>，<b>仅服务商页</b>印发）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>查询商户税收分类编码对照表，见 <see cref="FapiaoTaxCodeListResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/new-tax-control-fapiao/merchant/tax-codes</c>；query <c>offset</c> / <c>limit</c> / <c>sub_mchid</c>。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>双页并集</b>：本支路由官方印 2 页（商户页、服务商页），Path/Query 与应答均按<b>并集</b>建模，仅某页印出的字段与必填口径差异已在参数注释内逐字段标注 ⇒ 一面一方法，不因身份拆建。</para>
    /// <para><b><c>offset</c> 与 <c>limit</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：请求参数符合参数格式，但不符合业务规则 / 401 SIGN_ERROR：签名错误或签名信息不完整 / 403 NO_AUTH：商户无权限 / 429 FREQUENCY_LIMITED：频率超限。</para>
    /// </remarks>
    [Get("/v3/new-tax-control-fapiao/merchant/tax-codes")]
    Task<FapiaoTaxCodeListResponse> QueryFapiaoTaxCodesAsync(
        [Query("offset")] int offset,
        [Query("limit")] int limit,
        [Query("sub_mchid")] string? subMchid = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取用户填写抬头信息。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012538112"/>（【普通商户】）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012474048"/>（【普通服务商】）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4015784260"/>（【普通服务商】）。
    /// </summary>
    /// <param name="fapiaoApplyId">【发票申请单号】 发票申请单号，唯一标识一次开票行为。当开票场景为WITHOUT_WECHATPAY时，为调用【获取抬头填写链接】接口时指定的发票申请单号；当开票场景为WITH_WECHATPAY时，为与本次开票关联的微信支付订单号，且必须是属于相应商户的订单（官方 query <c>fapiao_apply_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="scene">【开票场景】 开票场景。可选取值：`WITH_WECHATPAY` 微信支付场景 / `WITHOUT_WECHATPAY` 非微信支付场景（官方 query <c>scene</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="subMchid">【子商户号】 微信支付分配的子商户号，服务商模式下必传（官方 query <c>sub_mchid</c>，选填，类型 <c>string(32)</c>，<b>仅服务商页</b>印发，服务商页页标选填、服务商页页标必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>获取用户填写抬头信息，见 <see cref="FapiaoUserTitleResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/new-tax-control-fapiao/user-title</c>；query <c>fapiao_apply_id</c> / <c>scene</c> / <c>sub_mchid</c>。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>双页并集</b>：本支路由官方印 3 页（商户页、服务商页、服务商页），Path/Query 与应答均按<b>并集</b>建模，仅某页印出的字段与必填口径差异已在参数注释内逐字段标注 ⇒ 一面一方法，不因身份拆建。</para>
    /// <para><b><c>fapiao_apply_id</c> 与 <c>scene</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：请求参数符合参数格式，但不符合业务规则 / 401 SIGN_ERROR：签名错误或签名信息不完整 / 403 NO_AUTH：商户无权限 / 404 RESOURCE_NOT_EXISTS：用户尚未完成发票抬头的填写 / 429 FREQUENCY_LIMITED：频率超限 / 400 PARAM_ERROR：参数错误 / 400 INVALID_REQUEST：HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR：验证不通过 / 500 SYSTEM_ERROR：系统异常，请稍后重试。</para>
    /// </remarks>
    [Get("/v3/new-tax-control-fapiao/user-title")]
    Task<FapiaoUserTitleResponse> QueryFapiaoUserTitleAsync(
        [Query("fapiao_apply_id")] string fapiaoApplyId,
        [Query("scene")] string scene,
        [Query("sub_mchid")] string? subMchid = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取用户抬头填写链接。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012538106"/>（【普通商户】）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012474040"/>（【普通服务商】）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4015770776"/>（【普通服务商】）。
    /// </summary>
    /// <param name="fapiaoApplyId">【发票申请单号】 发票申请单号，唯一标识一次开票行为。只能是字母、数字、中划线-、下划线_、竖线&amp;#124;、星号*这些英文半角字符，且该单号在每个商户下必须唯一。当用户完成抬头填写后，微信支付会以该单号回调商户，商户可通过【获取用户填写的抬头】接口查询用户填写的抬头信息（官方 query <c>fapiao_apply_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="appid">【商户AppID】 若开票来源是WEB，则为商户的公众号AppID；若开票来源是MINIPROGRAM，则为商户的小程序AppID（官方 query <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="openid">【用户OpenID】 需要填写发票抬头的用户在商户AppID下的OpenID（官方 query <c>openid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="totalAmount">【总金额】 展示在抬头填写页面的开票金额，对该发票申请单开票时所有发票的总价税合计之和不能超过此金额，单位：分（官方 query <c>total_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</param>
    /// <param name="source">【开票来源】 开票来源。可选取值：`WEB` 微信H5 / `MINIPROGRAM` 微信小程序（官方 query <c>source</c>，<b>必填</b>，类型 <c>string</c>）。</param>
    /// <param name="sellerName">【销售方名称】 展示在抬头填写页的销售方名称，若不传则默认取商户名称（官方 query <c>seller_name</c>，选填，类型 <c>string(32)</c>）。</param>
    /// <param name="showPhoneCell">【是否需要展示手机号填写栏】 是否需要展示手机号填写栏（官方 query <c>show_phone_cell</c>，选填，类型 <c>boolean</c>）。</param>
    /// <param name="mustInputPhone">【用户是否必须填写手机号】 用户是否必须填写手机号，仅当需要展示手机号填写栏时生效（官方 query <c>must_input_phone</c>，选填，类型 <c>boolean</c>）。</param>
    /// <param name="showEmailCell">【是否需要展示邮箱地址填写栏】 是否需要展示邮箱地址填写栏（官方 query <c>show_email_cell</c>，选填，类型 <c>boolean</c>）。</param>
    /// <param name="mustInputEmail">【用户是否必须填写邮箱地址】 用户是否必须填写邮箱地址，仅当需要展示邮箱地址填写栏时生效（官方 query <c>must_input_email</c>，选填，类型 <c>boolean</c>）。</param>
    /// <param name="subMchid">【子商户号】 微信支付分配的子商户号，服务商模式下必传（官方 query <c>sub_mchid</c>，选填，类型 <c>string(32)</c>，<b>仅服务商页</b>印发，服务商页页标选填、服务商页页标必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>获取用户抬头填写链接，见 <see cref="FapiaoUserTitleUrlResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/new-tax-control-fapiao/user-title/title-url</c>；query <c>fapiao_apply_id</c> / <c>appid</c> / <c>openid</c> / <c>total_amount</c> / <c>source</c> / <c>seller_name</c> / <c>show_phone_cell</c> / <c>must_input_phone</c> / <c>show_email_cell</c> / <c>must_input_email</c> / <c>sub_mchid</c>。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 FREQUENCY_LIMITED</c> 一类限频码）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>双页并集</b>：本支路由官方印 3 页（商户页、服务商页、服务商页），Path/Query 与应答均按<b>并集</b>建模，仅某页印出的字段与必填口径差异已在参数注释内逐字段标注 ⇒ 一面一方法，不因身份拆建。</para>
    /// <para><b><c>fapiao_apply_id</c> 与 <c>appid</c> 与 <c>openid</c> 与 <c>total_amount</c> 与 <c>source</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST：请求参数符合参数格式，但不符合业务规则 / 401 SIGN_ERROR：签名错误或签名信息不完整 / 403 NO_AUTH：商户无权限 / 403 RULE_LIMIT：商户获取抬头链接指定的用户与已提交抬头的用户不一致 / 429 FREQUENCY_LIMITED：频率超限 / 403 NO_AUTH：RULE_LIMIT / 400 PARAM_ERROR：参数错误 / 400 INVALID_REQUEST：HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR：验证不通过 / 500 SYSTEM_ERROR：系统异常，请稍后重试。</para>
    /// </remarks>
    [Get("/v3/new-tax-control-fapiao/user-title/title-url")]
    Task<FapiaoUserTitleUrlResponse> GetFapiaoUserTitleUrlAsync(
        [Query("fapiao_apply_id")] string fapiaoApplyId,
        [Query("appid")] string appid,
        [Query("openid")] string openid,
        [Query("total_amount")] int totalAmount,
        [Query("source")] string source,
        [Query("seller_name")] string? sellerName = null,
        [Query("show_phone_cell")] bool? showPhoneCell = null,
        [Query("must_input_phone")] bool? mustInputPhone = null,
        [Query("show_email_cell")] bool? showEmailCell = null,
        [Query("must_input_email")] bool? mustInputEmail = null,
        [Query("sub_mchid")] string? subMchid = null,
        CancellationToken cancellationToken = default);

}
