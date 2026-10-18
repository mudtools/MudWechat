// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.MiniProgram.DataModels.ShopAccount;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「自定义交易组件 · 资质提审与审核结果」域 SDK（4 端点：品牌审核 / 类目审核 /
/// 获取小程序提交过的入驻资质 / 查询审核结果；官方 minishopopencomponent2 分区 → audit 子目录
/// 「接入商品前必需接口」）。
/// </summary>
/// <remarks>
/// <para>
/// <b>令牌形态（MP-X2 / MP-X3 同口径）</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入 <c>access_token</c>；
/// 4 页均无第二把凭据参数、无权限集编号、无第三方代调用与云调用声明（官方本页空缺，留档不得补数）。
/// </para>
/// <para>
/// <b>「成功仅代表受理」是本域的第一陷阱</b>：<see cref="AuditBrandAsync"/> 与 <see cref="AuditCategoryAsync"/>
/// 的同步应答只有一个 <c>audit_id</c>，官方原文「请求成功后将会创建一个审核单」，审核结论只出现在
/// 品牌/类目审核<b>回调</b>或 <see cref="GetAuditResultAsync"/> 轮询里 ⇒ 两个提审端点<b>非幂等</b>，
/// 不得放进自动重试管道（类目侧重复提审已通过的类目直接报 <c>1050003</c>）。
/// </para>
/// <para>
/// <b>图片凭据统一出自 <see cref="IWxaShopImageService"/></b>：本域所有资质图片位官方一律接受
/// 「图片 url / media_id」，并强制经上传图片接口换取临时链接（上传后微信侧转永久链接）；
/// 自有 url 打不开、超过 2MB 或格式非 jpg / jpeg / png 会<b>直接导致审核驳回</b>。
/// </para>
/// <para>
/// <b>两套审核状态取值表并存</b>：本域 <c>/shop/audit/result</c> 是 <c>0</c> 审核中 / <c>1</c> 审核成功 /
/// <c>9</c> 审核拒绝；<c>/shop/register/check</c> 的场景状态是 <c>0</c> / <c>1</c> / <c>2</c> / <c>3</c> ⇒
/// 判定器不得共用（守卫 SHOPA6）。
/// </para>
/// <para>
/// <b>敏感面</b>：营业执照、商标注册证、销售授权书、报关单等证照链接与商标权人姓名 / 注册号属实名与证照信息，
/// <b>不得</b>写入日志、遥测或异常消息。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ShopAccount", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaShopAuditService
{
    /// <summary>
    /// 品牌审核（<c>POST /shop/audit/audit_brand</c>；权限集：官方本页未声明；第三方代调用：官方本页未声明）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/audit/audit_brand.html"/>。
    /// </summary>
    /// <param name="request">请求体（顶层唯一键 <c>audit_req</c>，内含 <c>license</c> / <c>brand_info</c> / <c>scene_group_list</c>），见 <see cref="WxaShopBrandAuditRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>审核单 id（<c>audit_id</c>），见 <see cref="WxaShopAuditIdResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体，必须 <c>audit_req</c> 一层包装；官方本页无端点级频率数字。</para>
    /// <para><b>必填集是四列矩阵</b>（官方参数表按 自有品牌 R 标 / 自有品牌 TM 标 / 代理品牌 R 标 / 代理品牌 TM 标
    /// 四列分别标注）⇒ 同一支字段在不同品牌类型下必填性不同，逐字段口径见 <see cref="WxaShopBrandAuditInfo"/>。</para>
    /// <para><b>官方图片三条硬约束</b>（正文原文）：图片大小 2MB 以下、格式 jpg / jpeg / png、url 必须能正常打开，
    /// 「如图片不能正常显示，会导致审核驳回」⇒ 驳回原因不在本端点应答里，须接品牌审核回调或轮询 <see cref="GetAuditResultAsync"/>。</para>
    /// <para><b><c>scene_group_list</c> 官方要求显式传空数组</b>（原文「组件开通流程中以及未接入场景时，请保持为空
    /// <c>"scene_group_list":[]</c>」，同时「未明确开通此场景的商家请勿传值」）⇒ 缺键与空数组不等价。</para>
    /// <para><b>审核成功后 <c>brand_id</c> 由回调给出</b>（官方原文），<see cref="GetAuditResultAsync"/> 亦可回读；
    /// 本端点应答<b>不含</b> brand_id。</para>
    /// </remarks>
    [Post("/shop/audit/audit_brand")]
    Task<WxaShopAuditIdResponse> AuditBrandAsync(
        [Body] WxaShopBrandAuditRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 类目审核（<c>POST /shop/audit/audit_category</c>；权限集：官方本页未声明；第三方代调用：官方本页未声明）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/audit/audit_category.html"/>。
    /// </summary>
    /// <param name="request">请求体（顶层唯一键 <c>audit_req</c>，内含 <c>license</c> / <c>category_info</c> / <c>scene_group_list</c>），见 <see cref="WxaShopCategoryAuditRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>审核单 id（<c>audit_id</c>），见 <see cref="WxaShopAuditIdResponse"/>（与品牌侧共用应答类型）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体，同为 <c>audit_req</c> 包装；官方本页无端点级频率数字。</para>
    /// <para><b>重复提审已通过的类目会报 <c>1050003</c></b>（官方正文原文）⇒ 本端点<b>不是</b>可重复调用的 upsert，
    /// 宿主须先用 <see cref="IWxaShopAccountService.GetCategoryListAsync"/> 或 <see cref="IWxaShopCategoryService"/>
    /// 的资质类型判重。</para>
    /// <para><b>只有 <c>qualification_type = 1</c>（必填）的类目才需要提审</b>：官方 <c>/shop/cat/get</c> 页原文
    /// 「若该类目资质不需要，则该类目自动拥有，无需申请，如依然调用，会报错 <c>1050011</c>」。</para>
    /// <para><b>类目定位键是 <c>level1</c> / <c>level2</c> / <c>level3</c></b>（官方原文，说明列还写「字符类型，最长不超过 10」
    /// 而类型列写 <c>uint32</c>）⇒ 与 <c>*_cat_*</c> / <c>*_category_*</c> 两套读侧键名不同源，守卫 SHOPA5 锁定。</para>
    /// </remarks>
    [Post("/shop/audit/audit_category")]
    Task<WxaShopAuditIdResponse> AuditCategoryAsync(
        [Body] WxaShopCategoryAuditRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取小程序提交过的入驻资质信息（<c>POST /shop/audit/get_miniapp_certificate</c>；权限集：官方本页未声明；第三方代调用：官方本页未声明）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/audit/get_miniapp_certificate.html"/>。
    /// </summary>
    /// <param name="request">请求体（只有 <c>req_type</c> 一支，必填；<c>1</c> 类目 / <c>2</c> 品牌），见 <see cref="WxaShopMiniappCertificateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>历史资质（顶层 <c>brand_info</c> / <c>category_info_list</c>，<b>无 <c>data</c> 包装</b>），见 <see cref="WxaShopMiniappCertificateResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体；官方本页无端点级频率数字。</para>
    /// <para><b>本页读的是「小程序方」的历史提交，不是组件入驻审核</b>（官方原文注：
    /// 「该接口返回的是曾经在小程序方提交过的审核，非组件的入驻审核！」）⇒
    /// 不能用它回读 <see cref="AuditBrandAsync"/> / <see cref="AuditCategoryAsync"/> 提交的内容。</para>
    /// <para><b>记录条数两侧不对称</b>（官方原文）：「请求类目会返回多次的请求记录，请求品牌只会返回最后一次的提交记录」。</para>
    /// <para><b>空历史是错误码而不是空数组</b>（官方原文）：「如果曾经没有提交，没有储存历史文件，或是获取失败，
    /// 接口会返回 <c>1050006</c>」⇒ 须按 <c>errcode</c> 分支。</para>
    /// <para><b>图片经转链</b>（官方原文「图片经过转链，请使用高版本 chrome 浏览器打开」）⇒ 回给的是
    /// <c>store.mp.video.tencent-cloud.com</c> 等微信侧域名，不可回写成商家自有 url。</para>
    /// </remarks>
    [Post("/shop/audit/get_miniapp_certificate")]
    Task<WxaShopMiniappCertificateResponse> GetMiniappCertificateAsync(
        [Body] WxaShopMiniappCertificateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询审核结果（<c>POST /shop/audit/result</c>；权限集：官方本页未声明；第三方代调用：官方本页未声明）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/audit/audit_result.html"/>。
    /// </summary>
    /// <param name="request">请求体（只有 <c>audit_id</c> 一支，必填），见 <see cref="WxaShopAuditResultRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>审核状态（<c>data.status</c> / <c>data.brand_id</c> / <c>data.reject_reason</c>），见 <see cref="WxaShopAuditResultResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体；官方本页无端点级频率数字 ⇒ 轮询节奏由宿主自定，
    /// 官方推荐通路是「品牌审核回调 / 类目审核回调」（本域无额度可依据）。</para>
    /// <para><b>一支端点服务两张审核单</b>（官方原文「根据审核 id，查询品牌和类目的审核结果」）⇒ 无品牌/类目分支参数。</para>
    /// <para><b><c>audit_id</c> 的官方类型列自相矛盾</b>：本页参数表写 <c>number</c>，提审侧回包表写 <c>string</c>，
    /// 而官方示例值 <c>"HIDFUSHJRIYAAAAAAAA"</c> / <c>"RQAAAPX1nuJsAAAAFTrfXW"</c> 均为非定长串 ⇒
    /// 本仓两侧一律 <see cref="string"/>（守卫 SHOPA5 锁定，数值化会让 id 无法回传）。</para>
    /// <para><b>状态取值 0 / 1 / 9</b>（审核中 / 审核成功 / 审核拒绝）——与 <c>/shop/register/check</c> 的
    /// <c>0 / 1 / 2 / 3</c> 表不同；官方示例即 <c>"brand_id": 0</c>，故 <c>brand_id</c> 非 0 只是品牌单的附带结果，
    /// 成功判据恒为 <c>data.status == 1</c>。</para>
    /// </remarks>
    [Post("/shop/audit/result")]
    Task<WxaShopAuditResultResponse> GetAuditResultAsync(
        [Body] WxaShopAuditResultRequest request,
        CancellationToken cancellationToken = default);
}
